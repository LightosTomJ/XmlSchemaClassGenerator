using System;
using System.Collections.Generic;
using System.Text;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.Validation
{
    public static class Field
    {
        public static string HasValidName(SQL.Components.Field f, SQL.Components.Table t)
        {
            try
            {
                if (f.Name == null || f.Name == "")
                {
                    string s = t.Name + "Missing";
                    return s;
                }
            }
            catch (Exception ae)
            { string s = ae.ToString(); }
            return f.Name;
        }
    }
}
