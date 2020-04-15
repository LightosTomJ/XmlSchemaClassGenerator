using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.Validation
{
    public static class PrimaryKey
    {
        public static DBRoles ConfirmPrimaryKeys(DBRoles dbRoles)
        {
            try
            {
                //Test for tables without a primary key
                foreach (Table t in dbRoles.Schemas)
                {
                    if (t.Fields.Count(f => f.IsPrimary) != 1)
                    {
                        if (t.Fields.Count(f => f.IsPrimary) < 1)
                        {
                            //Table does NOT have a Primary Key
                            Field pk = CreatePrimaryKey(t);
                            t.Fields.Insert(0, pk);

                        }
                        else if (t.Fields.Count(f => f.IsPrimary) > 1)
                        { }
                    }
                }

                //Test for tables with an unrecognised primary key name
                foreach (Table t in dbRoles.Schemas)
                {
                    Field pk = t.Fields.FirstOrDefault(f => f.IsPrimary);
                    if (pk.Name != t.Name + "Id")
                    {

                    }
                }
            }
            catch (Exception ae)
            { string s = ae.ToString(); }
            return dbRoles;
        }
        private static Field CreatePrimaryKey(Table t)
        {
            Field f = null;
            try
            {
                f = new Field()
                {
                    AllowNull = false,
                    IsPrimary = true,
                    IsClustered = true,
                    IdentitySpecification = new IdentitySpecification(),
                    Name = t.Name + "Id",
                    DataType = new Enums.DataType()
                    {
                        HasTypeError = false,
                        IsBaseType = true,
                        IsList = false,
                        IsNullable = false,
                        Name = Enums.SQLCondensedDataType.SMALLINT.ToString()
                    },
                    OriginalName = "Missing Primary Key"
                };
            }
            catch (Exception ae)
            { string s = ae.ToString(); }
            return f;
        }
    }
}
