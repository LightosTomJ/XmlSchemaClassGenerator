using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class ComputedColumnSpecification
    {
        public string Formula { get; set; }
        public bool IsPersisted { get; set; }

        public ComputedColumnSpecification()
        { }
    }
}
