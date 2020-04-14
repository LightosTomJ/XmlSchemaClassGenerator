using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class DBRoles
    {
        public string Name { get; set; }
        public List<Table> Schemas { get; set; } = new List<Table>();
        public List<DataSet> Data { get; set; } = new List<DataSet>();
        public DeployScript deployScript { get; set; } 

        public DBRoles()
        { }
    }
}
