using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSchemaClassGenerator.SQL.Components
{
    public class DeployScript
    {
        public string Name { get; set; } = "Script.PostDeployment";
        public List<string> DataTables { get; set; } = new List<string>();

        public DeployScript()
        { }
    }
}
