using System;
using System.Collections.Generic;
using System.Text;
using XmlSchemaClassGenerator.Enums;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class Field
    {
        //Additional members, not found in generic SQL
        public string OriginalName { get; set; }
        //'General' section of SQL column properties
        public string Name { get; set; }
        public bool AllowNull { get; set; } = true;
        public DataType DataType { get; set; }
        public object DefaultValueOrBinding { get; set; }
        public int Length { get; set; }

        //'Database Designer' section of SQL column properties
        public SQLCondensedDataType SQLDataType { get; set; }

        public bool IsPrimary { get; set; } = false;
        public bool IsClustered { get; set; } = false;

        public ComputedColumnSpecification ComputedColumnSpecification { get; set; }
        public IdentitySpecification IdentitySpecification { get; set; }
        public FullTextSpecification FullTextSpecification { get; set; }

        public bool Indexable { get; set; } = true;
        public bool IsColumnSet { get; set; } = false;
        public bool IsSparse { get; set; } = false;
        public bool MergePublished { get; set; } = false;
        public bool NotForReplication { get; set; } = false;
        public bool Replicated { get; set; } = false;
        public bool RowGuid { get; set; } = false;
        public int Size { get; set; } 

        public Field()
        { }
    }
}
