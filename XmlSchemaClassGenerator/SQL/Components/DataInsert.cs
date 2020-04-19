using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class DataInsert
    {
        public bool CheckForExisting { get; set; } = false;
        public int Position { get; set; }
        public string ColumnName { get; set; }
        public string Value { get; set; }

        public DataInsert()
        { }
    }
}
