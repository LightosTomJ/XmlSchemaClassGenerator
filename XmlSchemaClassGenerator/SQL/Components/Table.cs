using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class Table
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; }
        public List<Constraint> Constraints { get; set; }

        public Table()
        {
            Fields = new List<Field>();
            Constraints = new List<Constraint>();
        }
    }
}
