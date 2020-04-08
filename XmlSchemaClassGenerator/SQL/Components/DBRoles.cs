using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class DBRoles
    {
        public string Name { get; set; }
        public List<Table> Tables { get; set; }= new List<Table>();
        public List<Constraint> Constraints { get; set; }= new List<Constraint>();

        public DBRoles()
        { }
    }
}
