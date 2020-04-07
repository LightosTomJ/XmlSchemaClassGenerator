using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace XmlSchemaClassGenerator.Validation
{
    public static class Namespace
    {
        public static string NameIsValid(string name)
		{
            string validName = name;
			try
			{
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                validName = Regex.Replace(name, regexSearch, "_");
                validName = validName.Replace(".", "_");
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                if (ae.InnerException != null) s = ae.InnerException.Message;
                validName = "";
            }
            return validName;
        }
    }
}
