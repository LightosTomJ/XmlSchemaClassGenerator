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
                if (Directory.Exists(path) == false)
                { Directory.CreateDirectory(path); }

                //Iterate tables
                foreach (Table t in db.Schemas)
                {
                    string tablePath = Path.Combine(path, t.Name + ".sql");

                    using (var sw = new StreamWriter(tablePath))
                    {
                        //Create table
                        sw.WriteLine("CREATE TABLE [dbo].[" + t.Name + "]");
                        sw.WriteLine("(");
                        sw.WriteLine(Format.Tabs(1) + "--From " + db.Name + " XSD");
                        //TODO table validation required
                        //Number of primary keys
                        //Primary key name

                        //if (t.Fields.Count(f => f.IsPrimary == true) == 1)
                        //{
                        //    Field pk = t.Fields.FirstOrDefault(f => f.IsPrimary == true);
                        //    if (t.Fields[t.Fields.IndexOf(pk)].Name != t.Name + "Id")
                        //    {
                        //        if (t.Name.Substring(0, t.Name.Length - 2).ToUpper() != "ID")
                        //        {
                        //            t.Fields[t.Fields.IndexOf(pk)].Name = t.Name + "Id";
                        //        }
                        //        else if (t.Name.Substring(0, t.Name.Length - 2) == "ID")
                        //        {
                        //            t.Fields[t.Fields.IndexOf(pk)].Name = t.Name.Substring(0, t.Name.Length - 2) + "Id";
                        //        }
                        //        else if (t.Name.Substring(0, t.Name.Length - 2) == "id")
                        //        {
                        //            t.Fields[t.Fields.IndexOf(pk)].Name = t.Name.Substring(0, t.Name.Length - 2) + "Id";
                        //        }
                        //        else
                        //        {

                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    t.Fields.Insert(0, CreateGenericPrimaryKey(t));
                        //}

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
                        sw.WriteLine(Format.Tabs(1) + "CONSTRAINT [PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED ([" + t.Name + "Id] ASC)");

                        ////Insert Primary Key information if it exists
                        //if (t.Fields.Count(f => f.IsPrimary == true) == 1)
                        //{
                        //    if (db.Constraints.Count > 0)
                        //    { sw.WriteLine("CONSTRAINT[PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED ([" + t.Name + "Id] ASC),"); }
                        //    else
                        //    { sw.WriteLine("CONSTRAINT[PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED ([" + t.Name + "Id] ASC)"); }
                        //}

                        ////Add constraints
                        //foreach (Constraint c in db.Constraints)
                        //{
                        //    sw.Write("CONSTRAINT [FK_" + c.ChildTable + "_" + c.PrimaryTable + "] ");
                        //    sw.Write("FOREIGN KEY ([" + c.ChildField + "]) ");
                        //    sw.Write("REFERENCES [" + db.Name + "].[" + c.PrimaryTable + "] (" + c.PrimaryField + "])");
                        //    if (c.DeleteCascate) sw.Write(" ON DELETE CASCADE");
                        //    if (c.UpdateCascade) sw.Write(" ON UPDATE CASCADE");
                        //    if (db.Constraints.IndexOf(c) == db.Constraints.Count - 1)
                        //    { sw.WriteLine(""); }
                        //    else
                        //    { sw.WriteLine(","); }
                        //}

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
    }
}
