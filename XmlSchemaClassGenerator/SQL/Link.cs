using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.SQL
{
    public static class Link
    {
        public static DBRoles MergeNamespaces(List<DBRoles> roles)
        {
            DBRoles dbOut = new DBRoles();
            try
            {
                List<string> sqlTypes = GetSQLTypes();
                dbOut.Name = "MergedXSDs";
                foreach (DBRoles db in roles)
                {
                    //TODO check that duplicate table names do not exist
                    dbOut.Schemas.AddRange(db.Schemas);
                    //dbOut.Keys.AddRange(db.Keys);
                }

                List<string> tabNames = dbOut.Schemas.Select(s => s.Name).OrderBy(s => s).ToList();

                dbOut = Validation.PrimaryKey.ConfirmPrimaryKeys(dbOut);

                //Search for SQL data types that aren't base names but
                //other objects within XSD sets
                foreach (Table t in dbOut.Schemas)
                {
                    foreach (Field f in t.Fields)
                    {
                        string dataType = f.DataType.Name;
                        if (dataType.Contains("(") == true)
                        {
                            dataType = f.DataType.Name.Substring(0, f.DataType.Name.IndexOf("(") - 1);
                        }
                        if (sqlTypes.Count(s => s.Equals(dataType)) != 1)
                        {
                            //Find different table that contains this same field name + "Id"
                            List<Table> otherTables = dbOut.Schemas.ToList();
                            otherTables.Remove(t);

                            //Field name match
                            List<Table> tMatches = TablesWithMatchingName(otherTables, f.Name);
                            if (tMatches.Count == 1)
                            {
                                Key k = CreateKeyIfDoesNotExist(t, tMatches[0], f);
                                if (k != null) t.Keys.Add(k);
                            }
                            else if (tMatches.Count > 1)
                            { }
                            else
                            {
                                //Field data type match
                                tMatches.Clear();
                                tMatches = TablesWithMatchingName(otherTables, dataType);
                                if (tMatches.Count == 1)
                                {
                                    Key k = CreateKeyIfDoesNotExist(t, tMatches[0], f);
                                    if (k != null) t.Keys.Add(k);
                                }
                                else if (tMatches.Count > 1)
                                {
                                    Table tG = GuessRelevantTable(tMatches, t);
                                    if (tG != null)
                                    {
                                        Key k = CreateKeyIfDoesNotExist(t, tG, f);
                                        if (k != null) t.Keys.Add(k);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Base type detected
                        }
                    }
                }

            }
            catch (Exception ae)
            {
                string s = ae.ToString();
            }
            return dbOut;
        }
        private static List<string> GetSQLTypes()
        {
            return new List<string>()
            {
                "CHAR",
                "VARCHAR",
                "TEXT",
                "NCHAR",
                "NVARCHAR",
                "NTEXT",
                "BINARY",
                "VARBINARY",
                "IMAGE",
                "BIT",
                "TINYINT",
                "SMALLINT",
                "INT",
                "BIGINT",
                "DECIMAL",
                "NUMERIC",
                "SMALLMONEY",
                "MONEY",
                "FLOAT",
                "REAL",
                "DATETIME",
                "DATETIME2",
                "SMALLDATETIME",
                "DATE",
                "TIME",
                "DATETIMEOFFSET",
                "TIMESTAMP",
                "SQL_VARIANT",
                "UNIQUEIDENTIFIER",
                "XML",
                "CURSOR",
                "TABLE"
            };
        }

        private static List<Table> TablesWithMatchingName(List<Table> tables, string Name)
        {
            List<Table> lTabs = new List<Table>();
            try
            {
                foreach (Table t in tables)
                {
                    if (t.Name == Name)
                    {
                        lTabs.Add(t);
                    }
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
            }
            return lTabs;
        }

        private static Key CreateKeyIfDoesNotExist(Table pt, Table ft, Field f)
        {
            try
            {
                //Replace class with base type
                Field fO = ft.Fields.FirstOrDefault(fc => fc.Name == ft.Name + "Id");

                //Check other field DataType in a base type
                if (fO.DataType.IsBaseType)
                { f.DataType.Name = fO.DataType.Name; }
                else
                { f.DataType.Name = fO.DataType.Name; }

                foreach (Key k in pt.Keys)
                {
                    if (k.Name == "FK_" + pt.Name + "_" + ft.Name)
                    {
                        return null;
                    }
                }

                return new Key()
                {
                    Name = "FK_" + pt.Name + "_" + ft.Name,
                    ForeignKeyTable = ft.Name,
                    PrimaryKeyTable = pt.Name,
                    ForeignKeyField = ft.Name + "Id",
                    PrimaryKeyField = f.Name
                };
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                return null;
            }
        }

        private static Table GuessRelevantTable(List<Table> tables, Table t)
        {
            try
            {
                //Namespace test
                List<Table> ltSameNamespace = tables.Where(tc => tc.Namespace == t.Namespace).ToList();
                if (ltSameNamespace.Count == 1) return ltSameNamespace[0];
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
            }
            return null;
        }
    }
}
