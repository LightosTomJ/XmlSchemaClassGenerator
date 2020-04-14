using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.SQL.Write
{
    public class Classes
    {
        public static List<Key> keys = new List<Key>();
        public DBRoles Output(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path, GeneratorConfiguration configuration)
        {
            DBRoles roleNamespace = new DBRoles();
            try
            {
                roleNamespace.Name = cu.Namespaces[0].Name;

                List<CodeAttributeDeclaration> lAttributes = new List<CodeAttributeDeclaration>();
                List<CodeMemberEvent> lFieldEvents = new List<CodeMemberEvent>();
                List<CodeMemberField> lFieldMembers = new List<CodeMemberField>();
                List<CodeMemberMethod> lFieldMethods = new List<CodeMemberMethod>();
                List<CodeMemberProperty> lFieldProperties = new List<CodeMemberProperty>();

                List<string> lAttributeNames = new List<string>();

                foreach (CodeAttributeDeclaration at in ctd.CustomAttributes)
                {
                    lAttributes.Add(at);
                    lAttributeNames.Add(at.Name.Substring(0, at.Name.LastIndexOf(".")));
                }
                lAttributeNames = lAttributeNames.Distinct().OrderBy(a => a).ToList();

                for (int i = 0; i < ctd.Members.Count; i++)
                {
                    if (ctd.Members[i].GetType() == typeof(CodeMemberEvent))
                    {
                        lFieldEvents.Add((CodeMemberEvent)ctd.Members[i]);
                    }
                    if (ctd.Members[i].GetType() == typeof(CodeMemberField))
                    {
                        lFieldMembers.Add((CodeMemberField)ctd.Members[i]);
                    }
                    if (ctd.Members[i].GetType() == typeof(CodeMemberMethod))
                    {
                        lFieldMethods.Add((CodeMemberMethod)ctd.Members[i]);
                    }
                    if (ctd.Members[i].GetType() == typeof(CodeMemberProperty))
                    {
                        lFieldProperties.Add((CodeMemberProperty)ctd.Members[i]);
                    }
                }
                lFieldEvents.OrderBy(e => e.Name);
                lFieldMembers.OrderBy(f => f.Name);
                lFieldMethods.OrderBy(m => m.Name);
                lFieldProperties.OrderBy(p => p.Name);

                //Enums must be separated into separate table, populate and linked via foreign key reference
                if (ctd.IsEnum)
                {
                    Table t = Enums.CreateSchema(ctd, cu, lFieldMembers);
                    if (t != null) roleNamespace.Schemas.Add(t);

                    DataSet ds = Enums.CreateDataSet(ctd, cu, lFieldMembers);

                    if (roleNamespace.deployScript == null) roleNamespace.deployScript = new DeployScript();
                    roleNamespace.deployScript.DataTables.Add(t.Name);
                }
                else if (ctd.IsClass)
                { 
                    Table t = Tables.CreateSchema(ctd, cu, lFieldMembers);
                    if (t != null) roleNamespace.Schemas.Add(t);
                }
                else if (ctd.IsInterface)
                { }
                else
                { }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                return null;
            }
            //roleNamespace.Keys = keys;
            return roleNamespace;
        }

        
    }
}
