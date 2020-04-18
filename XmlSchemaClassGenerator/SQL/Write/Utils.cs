using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.SQL.Write
{
    public static class Utils
    {
        public static bool WriteSQLFile(string path, DBRoles db)
        {
            try
            {
                //Test for directory
                if (Directory.Exists(Path.Combine(path, "dbo\\Tables\\")) == false)
                { Directory.CreateDirectory(Path.Combine(path, "dbo\\Tables\\")); }

                //Iterate tables
                foreach (Table t in db.Schemas)
                {
                    string tablePath = Path.Combine(path, "dbo\\Tables\\", t.Name + ".sql");

                    using (var sw = new StreamWriter(tablePath))
                    {
                        //Create table
                        sw.WriteLine("CREATE TABLE [dbo].[" + t.Name + "]");
                        sw.WriteLine("(");
                        sw.WriteLine(Format.Tabs(1) + "--From " + db.Name + " XSD");
                        sw.WriteLine(Format.Tabs(1) + "--From '" + t.Namespace + "' Namespace");

                        //Get longest field name length
                        int maxNameLength = GetLongestName(t.Fields);
                        //Get longest field data length
                        int maxDataLength = GetLongestDataType(t.Fields);

                        //Create fields
                        foreach (Field f in t.Fields)
                        {

                            sw.Write(Format.Tabs(1) + "[" + f.Name.ToString() + "]" + Format.TabSoAllNamesFit(maxNameLength, f.Name));
                            string sDataFieldName = GetDataSQLName(f);
                            sw.Write(sDataFieldName + Format.TabSoAllDataTypesFit(maxDataLength, sDataFieldName));

                            //Allow NULL
                            if (f.DataType.IsNullable == true)
                            { sw.WriteLine("NULL,"); }
                            else
                            { sw.WriteLine("NOT NULL,"); }
                        }

                        sw.WriteLine();
                        if (t.Keys.Count > 0)
                        { sw.WriteLine(Format.Tabs(1) + "CONSTRAINT [PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED ([" + t.Name + "Id] ASC),"); }
                        else
                        { sw.WriteLine(Format.Tabs(1) + "CONSTRAINT [PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED ([" + t.Name + "Id] ASC)"); }

                        //Add constraints
                        foreach (Key k in t.Keys)
                        {
                            sw.Write(Format.Tabs(1) + "CONSTRAINT [" + k.Name + "] ");
                            sw.Write("FOREIGN KEY ([" + k.PrimaryKeyField + "]) ");

                            string IsFinalLine = "";
                            if (t.Keys.IndexOf(k) < t.Keys.Count - 1)
                            { IsFinalLine = ","; }

                            if (k.UpdateCascade == false && k.DeleteCascate == false)
                            {
                                sw.WriteLine("REFERENCES [dbo].[" + k.ForeignKeyTable + "] ([" + k.ForeignKeyField + "])" + IsFinalLine + " --ON UPDATE CASCADE, ON DELETE CASCADE,");
                            }
                            else if (k.UpdateCascade == true && k.DeleteCascate == false)
                            {
                                sw.WriteLine("REFERENCES [dbo].[" + k.ForeignKeyTable + "] ([" + k.ForeignKeyField + "]) ON UPDATE CASCADE" + IsFinalLine + " --ON DELETE CASCADE,");
                            }
                            else if (k.UpdateCascade == false && k.DeleteCascate == true)
                            {
                                sw.WriteLine("REFERENCES [dbo].[" + k.ForeignKeyTable + "] ([" + k.ForeignKeyField + "]) ON DELETE CASCADE" + IsFinalLine + " --ON UPDATE CASCADE,");
                            }
                            else
                            {
                                sw.WriteLine("REFERENCES [dbo].[" + k.ForeignKeyTable + "] ([" + k.ForeignKeyField + "]) -ON UPDATE CASCADE, ON DELETE CASCADE" + IsFinalLine);
                            }
                        }
                        sw.WriteLine(");");
                        sw.Close();
                    }
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
            }
            return true;
        }

        public static Field CreateGenericPrimaryKey(Table t)
        {
            Field pk = new Field()
            {
                AllowNull = false,
                IsPrimary = true,
                Name = t.Name
            };

            if (pk.Name.Substring(pk.Name.Length - 2, 2) != "Id")
            { pk.Name = pk.Name + "Id"; }

            XmlSchemaClassGenerator.Enums.DataType dt = new XmlSchemaClassGenerator.Enums.DataType()
            {
                HasTypeError = false,
                IsBaseType = true,
                IsList = false,
                IsNullable = false,
                Name = "INT",
            };
            pk.DataType = dt;
            pk.IdentitySpecification = new IdentitySpecification();
            pk.IsClustered = true;
            return pk;
        }

        public static int GetLongestName(List<Field> fields)
        {
            List<int> nameLengths = new List<int>();
            try
            {
                fields.ForEach(f => nameLengths.Add(f.Name.Length));
            }
            catch (Exception ae)
            { string s = ae.ToString(); }
            return nameLengths.Max();
        }

        public static string GetDataSQLName(Field f)
        {
            if (f.DataType.Para1 != 0 && f.DataType.Para2 != 0)
            {
                return f.DataType.Name + "(" +
                    f.DataType.Para1.ToString() + ", " +
                    f.DataType.Para2.ToString() + ")";
            }
            else if (f.DataType.Para1 != 0 && f.DataType.Para2 == 0)
            {
                if (f.DataType.Name == "NVARCHAR" && f.DataType.Para1 > 4000)
                {
                    return f.DataType.Name + "(MAX)";
                }
                if (f.DataType.Name == "VARCHAR" && f.DataType.Para1 > 8000)
                {
                    return f.DataType.Name + "(MAX)";
                }
                else
                {
                    return f.DataType.Name + "(" +
                        f.DataType.Para1.ToString() + ")";
                }
            }
            else
            {
                return f.DataType.Name;
            }
        }

        public static int GetLongestDataType(List<Field> fields)
        {
            List<int> dataLengths = new List<int>();
            foreach (Field f in fields)
            {
                if (f.DataType.Para1.ToString() != "" &&
                    f.DataType.Para2.ToString() != "")
                {
                    dataLengths.Add(f.DataType.Name.Length + "(".Length +
                        f.DataType.Para1.ToString().Length + ", ".Length +
                        f.DataType.Para2.ToString().Length + ")".Length);
                }
                else if (f.DataType.Para1.ToString() != "" &&
                    f.DataType.Para2.ToString() == "")
                {
                    dataLengths.Add(f.DataType.Name.Length + "(".Length +
                        f.DataType.Para1.ToString().Length + ")".Length);
                }
                else
                {
                    dataLengths.Add(f.DataType.Name.Length);
                }
            }
            return dataLengths.Max();
        }

        public static bool IsNumber(string s)
        {
            bool value = true;
            if (s == string.Empty || s == null)
            {
                value = false;
            }
            else
            {
                foreach (char c in s.ToCharArray())
                {
                    value = value && char.IsDigit(c);
                }
            }
            return value;
        }
    }
}
