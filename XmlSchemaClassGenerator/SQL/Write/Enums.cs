using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSchemaClassGenerator.SQL.Components;
using XmlSchemaClassGenerator.Enums;
using XmlSchemaClassGenerator.Validation;

namespace XmlSchemaClassGenerator.SQL.Write
{
    public static class Enums
    {
        public static Table CreateSchema(CodeTypeDeclaration ctd, CodeCompileUnit cu, List<CodeMemberField> lFieldMembers)
        {
            Table t = new Table();
            t.Name = Format.CamelCaseId(ctd.Name);
            t.Namespace = cu.Namespaces[0].Name;

            Field pkF = new Field()
            {
                Name = t.Name + "Id",
                IsPrimary = true,
                IsClustered = true,
                IdentitySpecification = new IdentitySpecification(),
                AllowNull = false,
                DataType = new XmlSchemaClassGenerator.Enums.DataType()
                {
                    IsNullable = false,
                    Name = "TINYINT",
                    IsBaseType = true,
                    IsList = false
                }
            };
            t.Fields.Add(pkF);

            Field f = new Field()
            {
                Name = "Value",
                IsPrimary = false,
                AllowNull = false
            };

            if (lFieldMembers.Count < 255)
            {
                f.DataType = new DataType()
                {
                    IsNullable = false,
                    Name = SQLCondensedDataType.TINYINT.ToString(),
                    IsBaseType = true,
                    IsList = false
                };
            }
            else if (lFieldMembers.Count < 32767)
            {
                f.DataType = new DataType()
                {
                    IsNullable = false,
                    Name = SQLCondensedDataType.SMALLINT.ToString(),
                    IsBaseType = true,
                    IsList = false
                };
            }
            else if (lFieldMembers.Count < 2147483647)
            {
                f.DataType = new DataType()
                {
                    IsNullable = false,
                    Name = SQLCondensedDataType.INT.ToString(),
                    IsBaseType = true,
                    IsList = false
                };
            }
            //else if (lFieldMembers.Count < 9223372036854775807)
            //{
            //    f.DataType = new DataType()
            //    {
            //        IsNullable = false,
            //        Name = SQLCondensedDataType.BIGINT.ToString(),
            //        IsBaseType = true,
            //        IsList = false
            //    };
            //}
            else
            {
                f.DataType = new DataType()
                {
                    IsNullable = false,
                    Name = SQLCondensedDataType.UNIQUEIDENTIFIER.ToString(),
                    IsBaseType = true,
                    IsList = false
                };
            }
            t.Fields.Add(f);

            return t;
        }

        public static DataSet CreateDataSet(CodeTypeDeclaration ctd, CodeCompileUnit cu, List<CodeMemberField> lFieldMembers)
        {
            DataSet ds = new DataSet();
            ds.Name = Format.CamelCaseId(ctd.Name);

            DataInsert d = new DataInsert();
            foreach (CodeMemberField cm in lFieldMembers)
            {
                KeyValuePair<int, string> p = new KeyValuePair<int, string>(
                    lFieldMembers.IndexOf(cm), cm.Name.ToString());
                d.Items.Add(p);
            }
            ds.Entries.Add(d);

            return ds;
        }

        //public static DeployScript CreateDeploymentScript(CodeTypeDeclaration ctd, CodeCompileUnit cu, List<CodeMemberField> lFieldMembers)
        //{
        //    DeployScript dp = new DeployScript();
        //    dp.Name = Format.CamelCaseId(ctd.Name);
            

        //    return dp;
        //}

        public static bool Output(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path, GeneratorConfiguration configuration)
        {
            try
            {
                List<CodeAttributeDeclaration> lAttributes = new List<CodeAttributeDeclaration>();
                List<CodeMemberField> lFields = new List<CodeMemberField>();

                List<string> lAttributeNames = new List<string>();
                foreach (CodeAttributeDeclaration at in ctd.CustomAttributes)
                {
                    lAttributes.Add(at);
                    lAttributeNames.Add(at.Name.Substring(0, at.Name.LastIndexOf(".")));
                }
                lAttributeNames = lAttributeNames.Distinct().OrderBy(a => a).ToList();

                foreach (CodeMemberField m in ctd.Members)
                { lFields.Add(m); }
                lFields.OrderBy(f => f.Name);

                using (var sw = new StreamWriter(path))
                {
                    //using
                    lAttributeNames.ForEach(a => sw.WriteLine("using " + a + ";"));
                    sw.WriteLine("");

                    //namespace
                    sw.WriteLine("namespace " + Namespace.NameIsValid(cu.Namespaces[0].Name));
                    sw.WriteLine("{");

                    //Custom attributes
                    Custom.GetAttributes(ctd, lAttributeNames).ForEach(s => sw.WriteLine(Format.Tabs(1) + s));

                    //enum
                    sw.WriteLine(Format.Tabs(1) + Declare.Enums(ctd, configuration));
                    sw.WriteLine(Format.Tabs(1) + "{");

                    //Fields
                    foreach (CodeMemberField f in lFields)
                    {
                        bool isFinalField = false;
                        if (lFields.IndexOf(f) == lFields.Count - 1)
                        { isFinalField = true; }

                        //Custom field attributes
                        Custom.GetFieldAttributes(f, lAttributeNames).ForEach(s => sw.WriteLine(Format.Tabs(2) + s));

                        Declare.Fields(ctd, f, isFinalField, configuration).ForEach(s => sw.WriteLine(Format.Tabs(2) + s));
                    }

                    //Enclose class
                    sw.WriteLine(Format.Tabs(1) + "}");

                    //Enclose Namespace
                    sw.WriteLine("}");
                    sw.Close();
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                return false;
            }
            return true;
        }
    }
}