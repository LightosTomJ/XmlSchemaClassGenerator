using System;
using System.Collections.Generic;
using System.Text;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.Validation
{
    public static class Schema
    {
        public static DBRoles All(DBRoles dbRoles)
        {
            try
            {
                //Test for tables without a primary key
                foreach (Table t in dbRoles.Schemas)
                {
                    PrimaryKey.ConfirmPrimaryKeys(t);
                    PrimaryKey.LimitedTextLengths(t);

                    foreach (SQL.Components.Field f in t.Fields)
                    {
                        f.Name = Field.HasValidName(f, t);
                    }
                }
            }
            catch (Exception ae)
            { string s = ae.ToString(); }
            return dbRoles;
        }
    }
}
