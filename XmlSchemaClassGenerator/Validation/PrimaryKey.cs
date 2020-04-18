using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.Validation
{
    public static class PrimaryKey
    {
        public static Table ConfirmPrimaryKeys(Table t)
        {
            try
            {
                //Test for tables without a primary key
                if (t.Fields.Count(f => f.IsPrimary) != 1)
                {
                    if (t.Fields.Count(f => f.IsPrimary) < 1)
                    {
                        //Table does NOT have a Primary Key
                        SQL.Components.Field pkN = CreatePrimaryKey(t);
                        t.Fields.Insert(0, pkN);

                    }
                    else if (t.Fields.Count(f => f.IsPrimary) > 1)
                    { }
                }

                //Test for tables with an unrecognised primary key name
                SQL.Components.Field pk = t.Fields.FirstOrDefault(f => f.IsPrimary);
                if (pk.Name != t.Name + "Id")
                {
                    SQL.Components.Field fpk = CreatePrimaryKey(t);
                    t.Fields.Insert(0, fpk);
                }
            }
            catch (Exception ae)
            { string s = ae.ToString(); }
            return t;
        }

        public static Table LimitedTextLengths(Table t)
        {
            try
            {
                SQL.Components.Field f = t.Fields.FirstOrDefault(fs => fs.IsPrimary == true);
                if (!f.DataType.Name.Contains("INT"))
                {


                }
            }
            catch (Exception ae)
            { string s = ae.ToString(); }
            return t;
        }

        private static SQL.Components.Field CreatePrimaryKey(Table t)
        {
            SQL.Components.Field f = null;
            try
            {
                f = new SQL.Components.Field()
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
