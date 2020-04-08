using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.Enums
{
    public class DataType
    {
        public string Name { get; set; }
        public int Para1 { get; set; }
        public int Para2 { get; set; }
        public bool IsNullable { get; set; } = false;
        public bool IsBaseType { get; set; } = false;
        public bool IsList { get; set; } = false;
        public bool HasTypeError { get; set; } = false;

        public DataType()
        { }
    }
}
