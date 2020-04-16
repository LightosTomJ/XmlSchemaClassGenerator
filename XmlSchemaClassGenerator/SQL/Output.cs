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

        public override void Write(List<CodeNamespace> lcn)
        {
            
            WriteSeparateFiles(lcn);
            //foreach (CodeNamespace cn in lcn)
            //{
            //    if (Configuration.SeparateClasses == true)
            //    {
            //        WriteSeparateFiles(cn);
            //    }
            //    else
            //    {
            //        var cu = new CodeCompileUnit();
            //        cu.Namespaces.Add(cn);

            //        var path = Path.Combine(OutputDirectory, cn.Name + ".cs");
            //        Configuration?.WriteLog(path);
            //        //WriteFile(path, cu);
            //    }
            //}
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
                    db.Schemas.AddRange(d.Schemas);
                    //db.Keys.AddRange(d.Keys);
                }



                List<bool> WrittenSuccess = new List<bool>();
                WrittenSuccess.Add(SQL.Write.Utils.WriteSQLFile(dirPath, db));

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

        private void WriteSeparateFiles(List<CodeNamespace> lcn)
        {
            List<DBRoles> dbRoles = new List<DBRoles>();
            try
            {
                foreach (CodeNamespace cn in lcn)
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
                        db.Schemas.AddRange(d.Schemas);
                        //db.Keys.AddRange(d.Keys);
                    }
                    //db.Keys = db.Keys.Distinct().ToList();
                    dbRoles.Add(db);
                    //List<bool> WrittenSuccess = new List<bool>();
                    //WrittenSuccess.Add(WriteSQLFile(dirPath, db));
                }

                //Cross check tables and objects for foreign key relationships
                DBRoles dbOut = Link.MergeNamespaces(dbRoles);

                List<bool> WrittenSuccess = new List<bool>();
                WrittenSuccess.Add(SQL.Write.Utils.WriteSQLFile(OutputDirectory, dbOut));

                //TODO create 'Data' script files from Enums
                //TODO create 'Deployment' script file linking all data files
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                if (ae.InnerException != null) s = ae.InnerException.Message;
            }
        }
    }
}
