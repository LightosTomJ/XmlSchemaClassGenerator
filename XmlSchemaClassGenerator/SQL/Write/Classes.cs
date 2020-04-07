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
        public bool Output(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path, GeneratorConfiguration configuration)
        {
            try
            {
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

                Table t = new Table();
                t.Name = cu.Namespaces[0].Name;
                foreach (CodeMemberField cmf in lFieldMembers)
                {
                    Field f = new Field()
                    {
                        Name = cmf.Name,
                        AllowNull = false
                    };
                    if (f.Name == "Id" || f.Name == "ID")
                    {
                        f.IsPrimary = true;
                        f.PrimaryInfo = new PrimaryKey();
                    }

                    f.DataType = ConvertTypes.SQLToBase(cmf.Attributes.ToString());
                    t.Fields.Add(f);
                }

                using (var sw = new StreamWriter(path))
                {
                    //Create table
                    sw.WriteLine("CREATE TABLE [" + cu.Namespaces[0].Name + "].[" + t.Name + "]");
                    sw.WriteLine("(");

                    //Create fields
                    foreach (Field f in t.Fields)
                    {
                        sw.Write(Format.Tabs(1) + "[" + f.Name.ToString() + "]" );
                        sw.Write(Format.Tabs(4) + f.DataType.ToString());

                        //Allow NULL
                        if (f.AllowNull == true)
                        { sw.Write(Format.Tabs(3) + "NULL"); }
                        else
                        { sw.Write(Format.Tabs(3) + "NOT NULL"); }

                        //End of line (last field?)
                        if (t.Fields.IndexOf(f) == t.Fields.Count - 1)
                        { sw.WriteLine(""); }
                        else
                        { sw.WriteLine(","); }
                    }

                    //Insert Primary Key information if it exists
                    if (t.Fields.Count(f => f.IsPrimary == true) == 1)
                    {
                        if (t.Constraints.Count > 0)
                        { sw.WriteLine("CONSTRAINT[PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED([" + t.Name + "Id] ASC),"); }
                        else
                        { sw.WriteLine("CONSTRAINT[PK_" + t.Name + "Id] PRIMARY KEY CLUSTERED([" + t.Name + "Id] ASC)"); }
                    }

                    //Add constraints
                    foreach (Constraint c in t.Constraints)
                    {
                        sw.Write("CONSTRAINT [FK_" + c.ChildTable + "_" + c.PrimanyTable + "] ");
                        sw.Write("FOREIGN KEY ([" + c.ChildField + "]) ");
                        sw.Write("REFERENCES [" + cu.Namespaces[0].Name.ToString() + "].[" + t.Name + "] (" + c.PrimanyField + "])");
                        if (c.DeleteCascate) sw.Write(" ON DELETE CASCADE");
                        if (c.UpdateCascade) sw.Write(" ON UPDATE CASCADE");
                        if (t.Constraints.IndexOf(c) == t.Constraints.Count - 1)
                        { sw.WriteLine(""); }
                        else
                        { sw.WriteLine(","); }
                    }

                    sw.WriteLine(");");
                    sw.Close();
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                return false;
            }
            return true;
        }
    }
}
