using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL
{
    public static class Format
    {
        public static string Tabs(int n)
        {
            return new String('\t', n);
        }

        public static string UsingDirectiveReduction(string expression, List<string> directives)
        {
            string outExpression = expression;

            //Reduce reference by checking for 'using' directive
            foreach (string useDir in directives)
            {
                if (expression.Contains(useDir + "."))
                {
                    outExpression = expression.Replace(useDir + ".", "");
                    if (outExpression.Contains(".") == false)
                    {
                        return outExpression;
                    }
                    else
                    {
                        outExpression = expression;
                    }
                }
            }
            return outExpression;
        }
    }
}
