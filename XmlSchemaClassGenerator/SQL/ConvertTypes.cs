
namespace XmlSchemaClassGenerator.SQL
{
    public static class ConvertTypes
    {
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
    }
}
