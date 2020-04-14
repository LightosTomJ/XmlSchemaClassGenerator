using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class Table
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<Field> Fields { get; set; }
        public List<Key> Keys { get; set; }
        
        public Table()
        {
            Fields = new List<Field>();
            Keys = new List<Key>();
        }
    }
}
