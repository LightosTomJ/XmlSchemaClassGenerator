using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSchemaClassGenerator.SQL.Components;
using XmlSchemaClassGenerator.Validation;

namespace XmlSchemaClassGenerator.SQL.Write
{
    public class Classes
    {
        public static List<Constraint> constraints = new List<Constraint>();
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
                    //Create data insert deployment script

                    return null;
                }
                else
                {
                    Table t = new Table();
                    t.Name = ctd.Name;
                    
                    foreach (CodeMemberField cmf in lFieldMembers)
                    {
                        string fName = ConvertTypes.GetNameFromCodeMemberField(cmf);

                        Field f = new Field()
                        {
                            Name = fName,
                            AllowNull = false
                        };

                        
                        if (fName.ToUpper() == "ID" || fName.Substring(fName.Length - 2,2).ToUpper() == "ID")
                        {
                            if (t.Fields.Count(fi => fi.IsPrimary == true) == 0)
                            {
                                f.IsPrimary = true;
                                f.PrimaryInfo = new PrimaryKey();
                                f.AllowNull = false;
                            }
                        }
                        
                        f.DataType = ConvertTypes.SQLToBase(cmf.Type, cmf, ctd);
                        if (constraints != null && constraints.Count > 0)
                        {
                            //Sort constraints
                        }

                        t.Fields.Add(f);
                    }

                    roleNamespace.Tables.Add(t);
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                return null;
            }
            roleNamespace.Constraints = constraints;
            return roleNamespace;
        }
    }
}
