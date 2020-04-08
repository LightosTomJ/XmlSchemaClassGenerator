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
        public static DataType SQLToBase(CodeTypeReference ctr, CodeMemberField cmf,
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
                            DataType dtInner = SQLToBase(ctrInner, cmf, ctd);
                            dtInner.IsList = true;
                            dtInner.IsNullable = false;
                            return dtInner;
                        }
                    }
                    else
                    {
                        if (ctr.BaseType.Contains("System.Collections.ObjectModel"))
                        {
                            //Foreign key table found
                            //Recusive function required to loop to bottom key table
                            Constraint constraint = new Constraint();
                            if (ctr.BaseType.Contains("<") && ctr.BaseType.Contains(">"))
                            {
                                constraint.ChildTable = ctr.BaseType.Substring(ctr.BaseType.IndexOf("<") + 1, ctr.BaseType.IndexOf(">") - ctr.BaseType.IndexOf("<") - 1);
                                constraint.PrimaryTable = ctd.Name;
                                constraint.ChildField = "";     //Unknown, guess will have to be made by table on table field comparison
                                constraint.PrimaryField = cmf.Name;
                                SQL.Write.Classes.constraints.Add(constraint);

                                return new DataType()
                                {
                                    IsBaseType = false,
                                    Name = constraint.ChildTable,
                                    HasTypeError = false,
                                    IsList = true
                                };
                            }
                            else
                            {
                                DataType sqlT = SQLToBase(ctr, cmf, ctd);
                                return sqlT;
                            }
                        }
                    }
                }
                else if (ctr.BaseType == "System.Nullable`1")
                {
                    //Iterate into lower levels
                    foreach (CodeTypeReference ctrInner in ctr.TypeArguments)
                    {
                        DataType dtInner = SQLToBase(ctrInner, cmf, ctd);
                        dtInner.IsList = true;
                        dtInner.IsNullable = true;
                        return dtInner;
                    }
                }
                else
                {
                    return GetTypeByName(ctr);
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
                return null;
            }
            return new DataType();
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
                        Name = "OBJECT",
                        Para1 = 255
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

        public static SQLDataType IsBaseLevel(CodeTypeReference ctr, out List<Constraint> constraints)
        {
            constraints = null;
            try
            {
                if (ctr.BaseType == "System.Boolean")
                {
                    return SQLDataType.BIT;
                }
                else if (ctr.BaseType == "System.Single" || ctr.BaseType == "System.Int32")
                {
                    return SQLDataType.INT;
                }
                else if (ctr.BaseType == "System.Double" || ctr.BaseType == "System.Int64")
                {
                    return SQLDataType.BIGINT;
                }
                else if (ctr.BaseType == "System.String")
                {
                    //look for MaxLengthAttribute and add data accordingly
                    return SQLDataType.NVARCHAR_MAX_;
                }
                else if (ctr.BaseType == "System.DateTime")
                {
                    return SQLDataType.DATETIME;
                }
                else if (ctr.BaseType == "System.Nullable`1")
                {

                    return SQLDataType.NVARCHAR_MAX_;
                }
                else if (ctr.BaseType == "System.Decimal")
                {
                    return SQLDataType.DECIMAL_P_S_;
                }
                else if (ctr.BaseType == "System.Object")
                {
                    //Objects may indicate errors within the XSD
                    return SQLDataType.NVARCHAR;
                }
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                //else if (ctr.BaseType == "")
                //{

                //    return SQLDataType.NVARCHAR_MAX_;
                //}
                else
                {
                    //Look for foreign key link
                    return SQLDataType.OTHER;
                    //Other needs separate funcitionality to map
                    //variables together
                }
            }
            catch (Exception ae)
            {
                string s = ae.ToString();
            }
            return SQLDataType.NVARCHAR_MAX_;
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
            if (cmf.Name.IndexOf(" ") > 0)
            { return cmf.Name.Substring(0, cmf.Name.IndexOf(" ")); }
            else
            { return cmf.Name; }
        }
    }
}
