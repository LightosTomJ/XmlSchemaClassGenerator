using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class IdentitySpecification
    {
        public bool IsIdentity { get; set; } = true;
        public int IdentityIncrement { get; set; } = 1;
        public int IdentitySeed { get; set; } = 1;

        public IdentitySpecification()
        { }
    }
}
