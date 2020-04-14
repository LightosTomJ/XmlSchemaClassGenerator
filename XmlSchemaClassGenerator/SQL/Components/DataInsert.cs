using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class DataInsert
    {
        public bool CheckForExisting { get; set; } = false;

        // 'int' can be used as the key regardless of content length as that is stipulated at
        // schema creation stage
        public List<KeyValuePair<int, string>> Items { get; set; } = new List<KeyValuePair<int, string>>();

        public DataInsert()
        { }
    }
}
