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

        public static string CamelCaseId(string name)
        {
            if (name == "Id" || name == "ID")
            {
                return name;
            }
            else if (name.Substring(name.Length - 2, 2) == "ID")
            {
                return name.Substring(0, name.Length - 2) + "Id";
            }
            else if (name.Substring(name.Length - 2, 2) == "id")
            {
                return name.Substring(0, name.Length - 2) + "Id";
            }
            else
            {
                return name;
            }
        }

        public static string TabSoAllNamesFit(int max, string name)
        {
            int length = 0;
            //determine max length. If max % 4 == 0, additional length needs to be added
            if ((max % 4) == 0)
            {
                length = (max / 4) + 1;
            }
            else
            {
                length = ((max - (max % 4)) / 4) + 1;
            }

            if (max == name.Length)
            { return Format.Tabs(1); }
            else
            {
                int tab = 0;
                if ((name.Length % 4) == 0)
                {
                    tab = (name.Length / 4);
                }
                else
                {
                    tab = ((name.Length - (name.Length % 4)) / 4) + 1;
                }
                
                return Format.Tabs(length - tab);
            }
        }

        public static string TabSoAllDataTypesFit(int max, string name)
        {
            int length = 0;
            //determine max length. If max % 4 == 0, additional length needs to be added
            if ((max % 4) == 0)
            {
                length = (max / 4) + 1;
            }
            else
            {
                length = ((max - (max % 4)) / 4) + 1;
            }

            if (max == name.Length)
            { return Format.Tabs(1); }
            else
            {
                int tab = 0;
                if ((name.Length % 4) == 0)
                {
                    tab = (name.Length / 4);
                }
                else
                {
                    tab = ((name.Length - (name.Length % 4)) / 4) + 1;
                }

                return Format.Tabs(length - tab);
            }
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
