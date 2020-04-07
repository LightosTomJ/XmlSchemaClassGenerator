using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSchemaClassGenerator.Validation;

namespace XmlSchemaClassGenerator.SQL.Write
{
    public class Enums
    {
        public bool Output(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path, GeneratorConfiguration configuration)
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
