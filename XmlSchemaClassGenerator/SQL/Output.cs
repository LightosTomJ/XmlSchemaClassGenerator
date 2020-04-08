using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.SQL
{
    public class Output : OutputWriter
    {
        public GeneratorConfiguration Configuration { get; set; }

        public string OutputDirectory { get; }

        public IList<string> WrittenFiles { get; } = new List<string>();

        public Output(string directory, bool createIfNotExists = true)
        {
            OutputDirectory = directory;

            if (createIfNotExists && !Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }
        }

        public override void Write(CodeNamespace cn)
        {
            var cu = new CodeCompileUnit();
            cu.Namespaces.Add(cn);

            if (Configuration.SeparateClasses == true)
            {
                WriteSeparateFiles(cn);
            }
            else
            {
                var path = Path.Combine(OutputDirectory, cn.Name + ".cs");
                Configuration?.WriteLog(path);
                //WriteFile(path, cu);
            }
        }

        private void WriteSeparateFiles(CodeNamespace cn)
        {
            List<DBRoles> dbRoles = new List<DBRoles>();
            try
            {
                string dirPath = Path.Combine(OutputDirectory, Validation.Namespace.NameIsValid(cn.Name));

                var ccu = new CodeCompileUnit();
                var cns = new CodeNamespace(Validation.Namespace.NameIsValid(cn.Name));

                cns.Imports.AddRange(cn.Imports.Cast<CodeNamespaceImport>().ToArray());
                cns.Comments.AddRange(cn.Comments);
                ccu.Namespaces.Add(cns);

                List<DBRoles> dbLocalRoles = new List<DBRoles>();
                foreach (CodeTypeDeclaration ctd in cn.Types)
                {
                    string path = Path.Combine(dirPath, ctd.Name + ".sql");
                    cns.Types.Clear();
                    cns.Types.Add(ctd);
                    Configuration?.WriteLog(path);

                    Write.Classes c = new Write.Classes();
                    DBRoles dbTemp = c.Output(ctd, ccu, path, Configuration);
                    if (dbTemp != null)
                    { dbLocalRoles.Add(dbTemp); }

                    //Add new file to list
                    if (path != "")
                    { WrittenFiles.Add(path); }
                }

                //Consolidate DBRoles for same XSD file into new single DBRole
                DBRoles db = new DBRoles()
                { Name = ccu.Namespaces[0].Name };

                foreach (DBRoles d in dbLocalRoles)
                {
                    db.Tables.AddRange(d.Tables);
                    db.Constraints.AddRange(d.Constraints);
                }

                List<bool> WrittenSuccess = new List<bool>();
                WrittenSuccess.Add(WriteSQLFile(dirPath, db));

                ////Clear constraints from namespace
                ////TODO determine whether cross XSD namespaces have cross constraints
                //SQL.Write.Classes.constraints.Clear();
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                if (ae.InnerException != null) s = ae.InnerException.Message;
            }
        }

        private bool WriteSQLFile(string path, DBRoles db)
        {
            try
            {
                //Test for directory
                if (Directory.Exists(path) == false)
                { Directory.CreateDirectory(path); }

                //Iterate tables
                foreach (Table t in db.Tables)
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

                        if (t.Fields.Count(f => f.IsPrimary == true) == 1)
                        {
                            Field pk = t.Fields.FirstOrDefault(f => f.IsPrimary == true);
                            if (pk.Name != t.Name + "Id")
                            {
                                t.Fields[t.Fields.IndexOf(pk)].Name = t.Name + "Id";
                            }
                        }
                        else
                        {
                            t.Fields.Insert(0, CreateGenericPrimaryKey(t));
                        }

                        //Create fields
                        foreach (Field f in t.Fields)
                        {
                            
                            sw.Write(Format.Tabs(1) + "[" + f.Name.ToString() + "]");
                            sw.Write(Format.Tabs(4) + f.DataType.Name.ToString());

                            //Allow NULL
                            if (f.DataType.IsNullable == true)
                            { sw.WriteLine(Format.Tabs(3) + "NULL,"); }
                            else
                            { sw.WriteLine(Format.Tabs(3) + "NOT NULL,"); }
                        }

                        sw.WriteLine();
                        sw.WriteLine(Format.Tabs(1) + "CONSTRAINT [PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED ([" + t.Name + "Id] ASC)");

                        ////Insert Primary Key information if it exists
                        //if (t.Fields.Count(f => f.IsPrimary == true) == 1)
                        //{
                        //    if (db.Constraints.Count > 0)
                        //    { sw.WriteLine("CONSTRAINT[PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED([" + t.Name + "Id] ASC),"); }
                        //    else
                        //    { sw.WriteLine("CONSTRAINT[PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED([" + t.Name + "Id] ASC)"); }
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

        private Field CreateGenericPrimaryKey(Table t)
        {
            Field pk = new Field()
            {
                AllowNull = false,
                IsPrimary = true,
                Name = t.Name + "Id",
            };
            Enums.DataType dt = new Enums.DataType()
            {
                HasTypeError = false,
                IsBaseType = true,
                IsList = false,
                IsNullable = false,
                Name = "INT",
            };
            pk.DataType = dt;
            pk.PrimaryInfo = new PrimaryKey();
            return pk;
        }
    }
}
