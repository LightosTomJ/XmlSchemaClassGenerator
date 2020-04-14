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
                            Table tMatch = TablesWithMatchingFieldName(otherTables, t, f);
                            if (tMatch != null)
                            {
                                //Key pair match found
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
        
        private static Table TablesWithMatchingFieldName(List<Table> tables, Table tl, Field f)
        {
            try
            {
                List<Table> tByName = new List<Table>();
                List<Table> tByClass = new List<Table>();
                foreach (Table t in tables)
                {
                    if (t.Fields.Count(fl => fl.Name == f.Name) > 0)
                    {
                        tByName.Add(t);
                    }
                    if (t.Fields.Count(fl => fl.Name == f.DataType.Name) > 0)
                    {
                        tByClass.Add(t);
                    }
                }
                if (tByName.Count == 0)
                {
                    if (tByClass.Count == 0)
                    { }
                    if (tByClass.Count == 1)
                    { return tByClass[0]; }
                    else
                    {
                        //Multiple table matches
                        return tByClass[0];
                    }

                }
                if (tByName.Count == 1)
                { return tByName[0]; }
                else
                {
                    //Multiple table matches
                    return tByName[0];
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                return null;
            }
        }
    }
}
