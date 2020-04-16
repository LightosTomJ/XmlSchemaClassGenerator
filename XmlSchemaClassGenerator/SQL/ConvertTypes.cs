using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using XmlSchemaClassGenerator.Enums;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.SQL
{
    public static class ConvertTypes
    {
        public static Table SQLToBase(Table t, Field f, CodeTypeReference ctr, CodeMemberField cmf,
                                         CodeTypeDeclaration ctd)
        {
            try
            {
                if (ctr.BaseType.Contains("System.Collections"))
                {
                    //Look for 'TypeArguments.InnerList'
                    if (ctr.TypeArguments.Count > 0)
                    {
                        foreach (CodeTypeReference ctrInner in ctr.TypeArguments)
                        {
                            t = SQLToBase(t, f, ctrInner, cmf, ctd);
                            f.DataType.IsList = true;
                            f.DataType.IsNullable = false;
                            f.OriginalName = ctrInner.BaseType;
                            return t;
                        }
                    }
                    else
                    {
                        if (ctr.BaseType.Contains("System.Collections.ObjectModel"))
                        {
                            //Determine whether member is public or private and 'XmlIgnoreAttribute()'
                            if (cmf.CustomAttributes.Count == 0)
                            { 
                                t = GenerateKeyLink(t, f, ctr, cmf, ctd);
                                f.OriginalName = ctr.BaseType;
                                return t;
                            }
                            else
                            {
                                if (XmlIgnoreAttributeCount(cmf.CustomAttributes) == 0)
                                {
                                    //Maybe do something
                                    t = GenerateKeyLink(t, f, ctr, cmf, ctd);
                                    f.OriginalName = ctr.BaseType;
                                    return t;
                                }
                                else
                                {
                                    //Ignore member
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
                else if (ctr.BaseType == "System.Nullable`1")
                {
                    //Iterate into lower levels
                    foreach (CodeTypeReference ctrInner in ctr.TypeArguments)
                    {
                        Table tTemp = SQLToBase(t, f, ctrInner, cmf, ctd);

                        f.DataType.IsList = true;
                        f.DataType.IsNullable = true;
                        return t;
                    }
                }
                else
                {
                    if (f.Name == null || f.Name == "")
                    { f.Name = ConvertTypes.GetNameFromCodeMemberField(cmf); }
                    f.DataType = GetTypeByName(ctr);
                    f.OriginalName = ctr.BaseType;
                    t.Fields.Add(f);
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                return null;
            }
            return t;
        }

        private static DataType GetTypeByName(CodeTypeReference ctr)
        {
            try
            {
                if (ctr.BaseType == "System.Boolean")
                {
                    return new DataType()
                    {
                        IsBaseType = true,
                        IsList = false,
                        Name = "BIT",
                    };
                }
                else if (ctr.BaseType == "System.Single" || ctr.BaseType == "System.Int32")
                {
                    return new DataType()
                    {
                        IsBaseType = true,
                        IsList = false,
                        Name = "INT",
                    };
                }
                else if (ctr.BaseType == "System.Double" || ctr.BaseType == "System.Int64")
                {
                    return new DataType()
                    {
                        IsBaseType = true,
                        IsList = false,
                        Name = "BIGINT",
                    };
                }
                else if (ctr.BaseType == "System.String")
                {
                    DataType dt = new DataType()
                    {
                        IsBaseType = true,
                        IsList = false,
                        Name = "NVARCHAR",
                    };
                    //Test for length restrictions
                    if (ctr.TypeArguments.Count > 0)
                    { dt.Para1 = 0; }
                    else
                    { dt.Para1 = 5000; }
                    return dt;
                }
                else if (ctr.BaseType == "System.DateTime")
                {
                    return new DataType()
                    {
                        IsBaseType = true,
                        IsList = false,
                        Name = "DATETIME",
                    };
                }
                
                else if (ctr.BaseType == "System.Decimal")
                {
                    return new DataType()
                    {
                        IsBaseType = true,
                        IsList = false,
                        Name = "DECIMAL",
                        Para1 = 18,
                        Para2 = 6
                    };
                }
                else if (ctr.BaseType == "System.Object")
                {
                    //Objects may indicate errors within the XSD
                    return new DataType()
                    {
                        IsBaseType = true,
                        IsList = false,
                        Name = "NVARCHAR",
                        Para1 = 255,
                        HasTypeError = true
                    };
                }
                else
                {
                    //Look for foreign key link
                    return new DataType()
                    {
                        IsBaseType = true,
                        IsList = false,
                        //Name = "OBJECT",
                        Name = ctr.BaseType
                    };
                    //Other needs separate funcitionality to map
                    //variables together
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                return null;
            }
        }

        public static SQLCondensedDataType IsBaseLevel(CodeTypeReference ctr, out List<Key> constraints)
        {
            constraints = null;
            try
            {
                if (ctr.BaseType == "System.Boolean")
                {
                    return SQLCondensedDataType.BIT;
                }
                else if (ctr.BaseType == "System.Single" || ctr.BaseType == "System.Int32")
                {
                    return SQLCondensedDataType.INT;
                }
                else if (ctr.BaseType == "System.Double" || ctr.BaseType == "System.Int64")
                {
                    return SQLCondensedDataType.BIGINT;
                }
                else if (ctr.BaseType == "System.String")
                {
                    //look for MaxLengthAttribute and add data accordingly
                    return SQLCondensedDataType.NVARCHAR_MAX_;
                }
                else if (ctr.BaseType == "System.DateTime")
                {
                    return SQLCondensedDataType.DATETIME;
                }
                else if (ctr.BaseType == "System.Nullable`1")
                {

                    return SQLCondensedDataType.NVARCHAR_MAX_;
                }
                else if (ctr.BaseType == "System.Decimal")
                {
                    return SQLCondensedDataType.DECIMAL_P_S_;
                }
                else if (ctr.BaseType == "System.Object")
                {
                    //Objects may indicate errors within the XSD
                    return SQLCondensedDataType.SQL_VARIANT;
                }
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                else
                {
                    //Look for foreign key link
                    return SQLCondensedDataType.OTHER;
                    //Other needs separate funcitionality to map
                    //variables together
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
            }
            return SQLCondensedDataType.NVARCHAR_MAX_;
        }

        public static string SystemToBase(string systemType)
        {
            switch (systemType)
            {
                //TODO List, dictionary, enumerate (including nesting) need to be hadled here too
                case "System.Void": //usually an enum value
                    return "";
                case "System.SByte":
                    return "sbyte";
                case "System.Byte":
                    return "byte";
                case "System.Int16":
                    return "short";
                case "System.UInt16":
                    return "ushort";
                case "System.Int32":
                    return "int";
                case "System.UInt32":
                    return "uint";
                case "System.Int64":
                    return "long";
                case "System.UInt64":
                    return "ulong";
                case "System.Double":
                    return "double";
                case "System.Char":
                    return "char";
                case "System.String":
                    return "string";
                default:
                    return systemType;
            }
        }

        public static string GetNameFromCodeMemberField(CodeMemberField cmf)
        {
            string sName = cmf.Name;
            if (sName.IndexOf(" ") > 0)
            { sName = sName.Substring(0, cmf.Name.IndexOf(" ")); }

            if (sName.Contains("\r")) sName = sName.Replace("\r", "");
            if (sName.Contains("\n")) sName = sName.Replace("\n", "");

            return sName.Trim();
        }

        private static int XmlIgnoreAttributeCount(CodeAttributeDeclarationCollection cac)
        {
            int c = 0;
            foreach (CodeAttributeDeclaration ca in cac)
            {
                if (ca.Name.Contains("XmlIgnoreAttribute")) c++;
            }
            return c;
        }

        private static Table GenerateKeyLink(Table t, Field f, CodeTypeReference ctr, CodeMemberField cmf,
                                         CodeTypeDeclaration ctd)
        {
            try
            {
                //Foreign key table found
                //Recusive function required to loop to bottom key table
                if (ctr.BaseType.Contains("<") && ctr.BaseType.Contains(">"))
                {
                    string sExtracted = ctr.BaseType.Substring(ctr.BaseType.IndexOf("<") + 1, ctr.BaseType.IndexOf(">") - ctr.BaseType.IndexOf("<") - 1);

                    if (ctd.Name == "BehaviourTypeAvailableAspects" && sExtracted == "BehaviourTypeAvailableAspectsAvailableAspect")
                    { }//sExtracted need to be "AvailableAspect"
                    
                    Key key = new Key()
                    {
                        Name = "FK_" + t.Name + "_" + sExtracted,
                        ForeignKeyTable = sExtracted,
                        PrimaryKeyTable = ctd.Name,
                        ForeignKeyField = sExtracted + "Id",
                        PrimaryKeyField = f.Name
                    };

                    f.DataType = new DataType()
                    {
                        IsBaseType = false,
                        Name = key.ForeignKeyTable,
                        HasTypeError = false,
                        IsList = true
                    };
                    t.Keys.Add(key);
                    t.Fields.Add(f);
                }
                else
                {
                    t = SQLToBase(t, f, ctr, cmf, ctd);
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                if (ae.InnerException != null) s = ae.InnerException.ToString();
                return null;
            }
            return t;
        }
    }
}
