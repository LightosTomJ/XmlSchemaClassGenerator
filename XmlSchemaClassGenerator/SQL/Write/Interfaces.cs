using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSchemaClassGenerator.Validation;

namespace XmlSchemaClassGenerator.SQL.Write
{
    public class Interfaces
    {
        public bool Output(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path)
        {
            try
            {
                List<CodeAttributeDeclaration> lAttributes = new List<CodeAttributeDeclaration>();
                List<CodeMemberField> lFields = new List<CodeMemberField>();

                foreach (CodeAttributeDeclaration at in ctd.CustomAttributes)
                { lAttributes.Add(at); }
                lAttributes.OrderBy(a => a.Name);

                foreach (CodeMemberField m in ctd.Members)
                { lFields.Add(m); }
                lFields.OrderBy(f => f.Name);


                using (var sw = new StreamWriter(path))
                {
                    lAttributes.ForEach(a => sw.WriteLine("using " + a.Name + ";"));



                    sw.Close();
                }
            }
            catch (System.Exception ae)
            {
                string s = ae.ToString();
                return false;
            }
            return true;
        }
    }
}
