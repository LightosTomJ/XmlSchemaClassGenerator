using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSchemaClassGenerator.Validation;

namespace XmlSchemaClassGenerator.SQL.Write
{
    public class Classes
    {
        public bool Output(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path, GeneratorConfiguration configuration)
        {
            try
            {
                List<CodeAttributeDeclaration> lAttributes = new List<CodeAttributeDeclaration>();
                List<CodeMemberEvent> lFieldEvents = new List<CodeMemberEvent>();
                List<CodeMemberField> lFieldMembers = new List<CodeMemberField>();
                List<CodeMemberMethod> lFieldMethods = new List<CodeMemberMethod>();
                List<CodeMemberProperty> lFieldProperties = new List<CodeMemberProperty>();

                List<string> lAttributeNames = new List<string>();

                foreach (CodeAttributeDeclaration at in ctd.CustomAttributes)
                {
                    lAttributes.Add(at);
                    lAttributeNames.Add(at.Name.Substring(0, at.Name.LastIndexOf(".")));
                }
                lAttributeNames = lAttributeNames.Distinct().OrderBy(a => a).ToList();

                for (int i = 0; i < ctd.Members.Count; i++)
                {
                    if (ctd.Members[i].GetType() == typeof(CodeMemberEvent))
                    {
                        lFieldEvents.Add((CodeMemberEvent)ctd.Members[i]);
                    }
                    if (ctd.Members[i].GetType() == typeof(CodeMemberField))
                    {
                        lFieldMembers.Add((CodeMemberField)ctd.Members[i]);
                    }
                    if (ctd.Members[i].GetType() == typeof(CodeMemberMethod))
                    {
                        lFieldMethods.Add((CodeMemberMethod)ctd.Members[i]);
                    }
                    if (ctd.Members[i].GetType() == typeof(CodeMemberProperty))
                    {
                        lFieldProperties.Add((CodeMemberProperty)ctd.Members[i]);
                    }
                }
                lFieldEvents.OrderBy(e => e.Name);
                lFieldMembers.OrderBy(f => f.Name);
                lFieldMethods.OrderBy(m => m.Name);
                lFieldProperties.OrderBy(p => p.Name);

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

                    sw.WriteLine(Format.Tabs(1) + Declare.Class(ctd, configuration));
                    sw.WriteLine(Format.Tabs(1) + "{");
                    //Fields
                    foreach (CodeMemberField f in lFieldMembers)
                    {
                        //Custom field attributes
                        Custom.GetFieldAttributes(f, lAttributeNames).ForEach(s => sw.WriteLine(Format.Tabs(2) + s));

                        Declare.Fields(ctd, f, true, configuration).ForEach(s => sw.WriteLine(Format.Tabs(2) + s));
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
