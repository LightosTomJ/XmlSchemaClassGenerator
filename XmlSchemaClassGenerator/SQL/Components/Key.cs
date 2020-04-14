using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class Key
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrimaryKeyTable { get; set; }
        public string ForeignKeyTable { get; set; }
        public string PrimaryKeyField { get; set; }
        public string ForeignKeyField { get; set; }
        public bool UpdateCascade { get; set; } = false;
        public bool DeleteCascate { get; set; } = false;
        public bool EnforceForReplication { get; set; } = true;
        public bool EnforceForeignKeyConstraint { get; set; } = true;

        public bool CheckConstraint { get; set; }

        public Key()
        { }
    }
}
