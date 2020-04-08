using System;
using System.Collections.Generic;
using System.Text;
using XmlSchemaClassGenerator.Enums;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class Field
    {
        public string Name { get; set; }
        public SQLDataType SQLDataType { get; set; }
        public DataType DataType { get; set; }
        public bool AllowNull { get; set; } = true;

        public bool IsPrimary { get; set; }

        public PrimaryKey PrimaryInfo { get; set; } = null;

        public Field()
        { }
    }
}
