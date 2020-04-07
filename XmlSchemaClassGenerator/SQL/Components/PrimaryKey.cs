using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class PrimaryKey
    {
        public bool IsIdentity { get; set; } = true;
        public bool IsClustered { get; set; } = true;

        public PrimaryKey()
        { }
    }
}
