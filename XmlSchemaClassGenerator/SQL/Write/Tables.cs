using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSchemaClassGenerator.SQL.Components;
using XmlSchemaClassGenerator.Validation;
namespace XmlSchemaClassGenerator.SQL.Write
{
    public static class Tables
    {
        public static Table CreateSchema(CodeTypeDeclaration ctd, CodeCompileUnit cu, List<CodeMemberField> lFieldMembers)
        {
            Table t = new Table();
            t.Name = Format.CamelCaseId(ctd.Name);
            t.Namespace = cu.Namespaces[0].Name;

            List<CodeMemberField> publicFields = lFieldMembers.Where(fm => fm.Attributes == MemberAttributes.Public).ToList();

            foreach (CodeMemberField cmf in publicFields)
            {
                string fName = ConvertTypes.GetNameFromCodeMemberField(cmf);
                Field f = new Field();

                if (fName == "Id")
                {
                    fName = t.Name + "Id";

                    f.Name = fName;
                    f.IsPrimary = true;
                    f.IsClustered = true;
                    f.IdentitySpecification = new IdentitySpecification();
                    f.AllowNull = false;
                }
                else if (fName.ToUpper() == "ID" || fName.Substring(fName.Length - 2, 2).ToUpper() == "ID")
                {
                    if (t.Fields.Count(fi => fi.IsPrimary == true) == 0)
                    {
                        f.IsPrimary = true;
                        f.IsClustered = true;
                        f.IdentitySpecification = new IdentitySpecification();
                        f.AllowNull = false;
                    }
                    fName = Format.CamelCaseId(fName);
                }
                else
                {
                    f.Name = fName;
                }

                Table tTemp = ConvertTypes.SQLToBase(t, f, cmf.Type, cmf, ctd);
                if (tTemp != null)
                {
                    t = tTemp;
                    //Catch un-named fields, code error only. Not possible to complie XSD with empty field names
                    if (t.Fields.Count(ft => ft.Name == null) > 0)
                    { }
                }
            }
            return t;
        }
    }
}
