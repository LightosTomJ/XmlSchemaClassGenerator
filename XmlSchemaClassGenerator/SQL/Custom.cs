using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSchemaClassGenerator.Validation;

namespace XmlSchemaClassGenerator.SQL
{
    public static class Custom
    {
        public static List<string> GetAttributes(CodeTypeDeclaration ctd, List<string> usingDirectives)
        {
            List<string> custom = new List<string>();
            try
            {
                foreach (CodeAttributeDeclaration cd in ctd.CustomAttributes)
                {
                    string ca = "[";
                    ca = ca + Format.UsingDirectiveReduction(cd.Name, usingDirectives);
                    if (cd.Arguments.Count > 0)
                    {
                        ca = ca + "(";
                        for (int i = 0; i < cd.Arguments.Count; i++)
                        {
                            CodeAttributeArgument aa = cd.Arguments[i];
                            if (aa.Value.GetType() == typeof(CodePrimitiveExpression))
                            {
                                CodePrimitiveExpression pe = (CodePrimitiveExpression)aa.Value;
                                string prefix = "";
                                if (pe.Value.ToString().Contains("http"))
                                { prefix = "Namespace="; }
                                if (i == cd.Arguments.Count - 1)
                                { ca = ca + prefix + "\"" + pe.Value + "\""; }
                                else
                                { ca = ca + prefix + "\"" + pe.Value + "\", "; }
                            }
                            else if (aa.Value.GetType() == typeof(CodeTypeOfExpression))
                            {
                                CodeTypeOfExpression te = (CodeTypeOfExpression)aa.Value;
                                ca = ca + "typeof(" + te.Type.BaseType + ")";
                            }
                            else
                            { }

                        }
                        ca = ca + ")";
                    }
                    else
                    {
                        ca = ca + "()";
                    }
                    ca = ca + "]";
                    custom.Add(ca);
                }
            }
            catch (Exception ae)
            {
                string strError = ae.ToString();
                if (ae.InnerException != null) strError = ae.InnerException.Message.ToString();
            }

            return custom;
        }

        public static List<string> GetFieldAttributes(CodeMemberField f, List<string> usingDirectives)
        {
            List<string> custom = new List<string>();
            try
            {
                foreach (CodeAttributeDeclaration cd in f.CustomAttributes)
                {
                    string ca = "[";
                    ca = ca + Format.UsingDirectiveReduction(cd.Name, usingDirectives);
                    if (cd.Arguments.Count > 0)
                    {
                        ca = ca + "(";
                        for (int i = 0; i < cd.Arguments.Count; i++)
                        {
                            CodeAttributeArgument aa = cd.Arguments[i];
                            if (aa.Value.GetType() == typeof(CodePrimitiveExpression))
                            {
                                CodePrimitiveExpression pe = (CodePrimitiveExpression)aa.Value;
                                if (i == cd.Arguments.Count - 1)
                                { ca = ca + "\"" + pe.Value + "\""; }
                                else
                                { ca = ca + "\"" + pe.Value + "\", "; }
                            }
                            else
                            { }
                        }
                        ca = ca + ")";
                    }
                    else
                    {
                        ca = ca + "()";
                    }
                    ca = ca + "]";
                    custom.Add(ca);
                }
            }
            catch (Exception ae)
            {
                string strError = ae.ToString();
                if (ae.InnerException != null) strError = ae.InnerException.Message.ToString();
            }

            return custom;
        }
    }
}
