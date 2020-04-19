using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class DataSet
    {
        public string Name { get; set; }
        public bool IdentityOff { get; set; } = false;
        public List<DataInsert> Entries { get; set; } = new List<DataInsert>();

        public DataSet()
        { }
    }
}
