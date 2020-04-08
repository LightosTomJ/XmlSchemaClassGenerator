using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class Constraint
    {
        public string PrimaryTable { get; set; }
        public string ChildTable { get; set; }
        public string PrimaryField { get; set; }
        public string ChildField { get; set; }
        public bool UpdateCascade { get; set; }
        public bool DeleteCascate { get; set; }

        public bool CheckConstraint { get; set; }

        public Constraint()
        { }
    }
}
