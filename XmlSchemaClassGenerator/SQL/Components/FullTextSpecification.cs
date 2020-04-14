using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class FullTextSpecification
    {
        public bool IsFullTextIndexable { get; set; }
        public string FullTextTypeColumn { get; set; }
        public string Language { get; set; }
        public bool StatisticalSemantics { get; set; }

        public FullTextSpecification()
        { }
    }
}
