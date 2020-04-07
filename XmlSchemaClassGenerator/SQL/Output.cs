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
            try
            {
                string dirPath = Path.Combine(OutputDirectory, Validation.Namespace.NameIsValid(cn.Name));

                var ccu = new CodeCompileUnit();
                var cns = new CodeNamespace(Validation.Namespace.NameIsValid(cn.Name));

                cns.Imports.AddRange(cn.Imports.Cast<CodeNamespaceImport>().ToArray());
                cns.Comments.AddRange(cn.Comments);
                ccu.Namespaces.Add(cns);

                foreach (CodeTypeDeclaration ctd in cn.Types)
                {
                    string path = Path.Combine(dirPath, ctd.Name + ".sql");
                    cns.Types.Clear();
                    cns.Types.Add(ctd);
                    Configuration?.WriteLog(path);

                    Write.Classes c = new Write.Classes();
                    c.Output(ctd, ccu, path, Configuration);

                    //Add new file to list
                    WrittenFiles.Add(path);
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                if (ae.InnerException != null) s = ae.InnerException.Message;
            }
        }
    }
}
