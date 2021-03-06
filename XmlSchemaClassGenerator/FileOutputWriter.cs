﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace XmlSchemaClassGenerator
{
    public class FileOutputWriter : OutputWriter
    {
        public GeneratorConfiguration Configuration { get; set; }
        public string OutputDirectory { get; }
        public IList<string> WrittenFiles { get; } = new List<string>();


        public FileOutputWriter(string directory, bool createIfNotExists = true)
        {
            OutputDirectory = directory;

            if (createIfNotExists && !Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }
        }

        /// <summary>
        /// A list of all the files written.
        /// </summary>

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
                WriteFile(path, cu);
            }
        }
        public override void Write(List<CodeNamespace> lcn)
        {
            WriteSeparateFiles(lcn);
        }

        protected virtual void WriteFile(string path, CodeCompileUnit cu)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(path, FileMode.Create);
                using (var writer = new StreamWriter(fs))
                {
                    fs = null;
                    Write(writer, cu);
                }
                WrittenFiles.Add(path);
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
        }

        private void WriteSeparateFiles(CodeNamespace cn)
        {
            try
            {
                string dirPath = Path.Combine(OutputDirectory, Validation.Namespace.NameIsValid(cn.Name));
                DirectoryInfo di = null;
                //Create directory to hold the output files
                if (Directory.Exists(dirPath))
                {
                    di = Directory.GetParent(dirPath);
                }
                else
                {
                    di = Directory.CreateDirectory(dirPath);
                }


                var ccu = new CodeCompileUnit();
                var cns = new CodeNamespace(Validation.Namespace.NameIsValid(cn.Name));

                cns.Imports.AddRange(cn.Imports.Cast<CodeNamespaceImport>().ToArray());
                cns.Comments.AddRange(cn.Comments);
                ccu.Namespaces.Add(cns);

                foreach (CodeTypeDeclaration ctd in cn.Types)
                {
                    string path = Path.Combine(dirPath, ctd.Name + ".cs");
                    cns.Types.Clear();
                    cns.Types.Add(ctd);
                    Configuration?.WriteLog(path);
                    WriteFile(path, ccu);
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                if (ae.InnerException != null) s = ae.InnerException.Message;
            }
        }
        private void WriteSeparateFiles(List<CodeNamespace> lcn)
        {
            try
            {
                foreach (CodeNamespace cn in lcn)
                {
                    string dirPath = Path.Combine(OutputDirectory, Validation.Namespace.NameIsValid(cn.Name));
                    DirectoryInfo di = null;
                    //Create directory to hold the output files
                    if (Directory.Exists(dirPath))
                    {
                        di = Directory.GetParent(dirPath);
                    }
                    else
                    {
                        di = Directory.CreateDirectory(dirPath);
                    }

                    var ccu = new CodeCompileUnit();
                    var cns = new CodeNamespace(Validation.Namespace.NameIsValid(cn.Name));

                    cns.Imports.AddRange(cn.Imports.Cast<CodeNamespaceImport>().ToArray());
                    cns.Comments.AddRange(cn.Comments);
                    ccu.Namespaces.Add(cns);

                    foreach (CodeTypeDeclaration ctd in cn.Types)
                    {
                        string path = Path.Combine(dirPath, ctd.Name + ".cs");
                        cns.Types.Clear();
                        cns.Types.Add(ctd);
                        Configuration?.WriteLog(path);
                        WriteFile(path, ccu);
                    }
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