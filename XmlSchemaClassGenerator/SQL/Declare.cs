using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSchemaClassGenerator.Validation;

namespace XmlSchemaClassGenerator.SQL
{
    public static class Declare
    {

        public static string Class(CodeTypeDeclaration ctd, GeneratorConfiguration configuration)
        {
            string declare = "";
            if (configuration.AssemblyVisible) declare = "internal ";
            else declare = "public ";

            if (ctd.IsPartial) declare = declare + "partial ";

            declare = declare + "class " + ctd.Name;

            return declare;
        }
        public static string Enums(CodeTypeDeclaration ctd, GeneratorConfiguration configuration)
        {
            string declare = Format.Tabs(1);
            if (configuration.AssemblyVisible) declare = "internal ";
            else declare = "public ";


            declare = declare + "enum " + ctd.Name;

            return declare;
        }

        public static List<string> Fields(CodeTypeDeclaration ctd, CodeMemberField f, bool ExcludeComma, GeneratorConfiguration configuration)
        {
            List<string> code = new List<string>();
            string comma = ",";
            if (ExcludeComma) comma = "";

            if (configuration.DisableComments == false)
            {
                foreach (var c in f.Comments)
                {
                    code.Add("///" + c);
                }
            }

            string props = "";
            if (f.Attributes.ToString() == "Public")
            { props = props + "public "; }
            else
            { }

            props = props + ConvertTypes.SystemToBase(f.Type.BaseType) + " " + f.Name + comma;

            code.Add(props);

            return code;
        }
    }
}
