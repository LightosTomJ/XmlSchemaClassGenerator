﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XmlSchemaClassGenerator;
using XmlSchemaClassGenerator.Validation;

namespace XmlSchemaSQLClassGenerator
{
    public class FileOutputWriter
    {
        public GeneratorConfiguration Configuration { get; set; }

        //private void WriteEnum(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path)
        //{
        //    try
        //    {
        //        List<CodeAttributeDeclaration> lAttributes = new List<CodeAttributeDeclaration>();
        //        List<CodeMemberField> lFields = new List<CodeMemberField>();

        //        List<string> lAttributeNames = new List<string>();
        //        foreach (CodeAttributeDeclaration at in ctd.CustomAttributes)
        //        {
        //            lAttributes.Add(at);
        //            lAttributeNames.Add(at.Name.Substring(0, at.Name.LastIndexOf(".")));
        //        }
        //        lAttributeNames = lAttributeNames.Distinct().OrderBy(a => a).ToList();

        //        foreach (CodeMemberField m in ctd.Members)
        //        { lFields.Add(m); }
        //        lFields.OrderBy(f => f.Name);

        //        using (var sw = new StreamWriter(path))
        //        {
        //            //using
        //            lAttributeNames.ForEach(a => sw.WriteLine("using " + a + ";"));
        //            sw.WriteLine("");

        //            //namespace
        //            sw.WriteLine("namespace " + Namespace.NameIsValid(cu.Namespaces[0].Name));
        //            sw.WriteLine("{");

        //            //Custom attributes
        //            GetCustomAttributes(ctd, lAttributeNames).ForEach(s => sw.WriteLine(Tabs(1) + s));

        //            //enum
        //            sw.WriteLine(Tabs(1) + DeclareEnum(ctd));
        //            sw.WriteLine(Tabs(1) + "{");

        //            //Fields
        //            foreach (CodeMemberField f in lFields)
        //            {
        //                bool isFinalField = false;
        //                if (lFields.IndexOf(f) == lFields.Count - 1)
        //                { isFinalField = true; }

        //                //Custom field attributes
        //                GetFieldCustomAttributes(f, lAttributeNames).ForEach(s => sw.WriteLine(Tabs(2) + s));

        //                DeclareField(ctd, f, isFinalField).ForEach(s => sw.WriteLine(Tabs(2) + s));
        //            }

        //            //Enclose class
        //            sw.WriteLine(Tabs(1) + "}");

        //            //Enclose Namespace
        //            sw.WriteLine("}");
        //            sw.Close();
        //        }
        //    }
        //    catch (Exception ae)
        //    {
        //        string s = ae.ToString();
        //    }
        //}

        //private void WriteClass(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path)
        //{
        //    try
        //    {
        //        List<CodeAttributeDeclaration> lAttributes = new List<CodeAttributeDeclaration>();
        //        List<CodeMemberEvent> lFieldEvents = new List<CodeMemberEvent>();
        //        List<CodeMemberField> lFieldMembers = new List<CodeMemberField>();
        //        List<CodeMemberMethod> lFieldMethods = new List<CodeMemberMethod>();
        //        List<CodeMemberProperty> lFieldProperties = new List<CodeMemberProperty>();

        //        List<string> lAttributeNames = new List<string>();

        //        foreach (CodeAttributeDeclaration at in ctd.CustomAttributes)
        //        {
        //            lAttributes.Add(at);
        //            lAttributeNames.Add(at.Name.Substring(0, at.Name.LastIndexOf(".")));
        //        }
        //        lAttributeNames = lAttributeNames.Distinct().OrderBy(a => a).ToList();

        //        for (int i = 0; i < ctd.Members.Count; i++)
        //        {
        //            if (ctd.Members[i].GetType() == typeof(CodeMemberEvent))
        //            {
        //                lFieldEvents.Add((CodeMemberEvent)ctd.Members[i]);
        //            }
        //            if (ctd.Members[i].GetType() == typeof(CodeMemberField))
        //            {
        //                lFieldMembers.Add((CodeMemberField)ctd.Members[i]);
        //            }
        //            if (ctd.Members[i].GetType() == typeof(CodeMemberMethod))
        //            {
        //                lFieldMethods.Add((CodeMemberMethod)ctd.Members[i]);
        //            }
        //            if (ctd.Members[i].GetType() == typeof(CodeMemberProperty))
        //            {
        //                lFieldProperties.Add((CodeMemberProperty)ctd.Members[i]);
        //            }
        //        }
        //        lFieldEvents.OrderBy(e => e.Name);
        //        lFieldMembers.OrderBy(f => f.Name);
        //        lFieldMethods.OrderBy(m => m.Name);
        //        lFieldProperties.OrderBy(p => p.Name);

        //        using (var sw = new StreamWriter(path))
        //        {
        //            //using
        //            lAttributeNames.ForEach(a => sw.WriteLine("using " + a + ";"));
        //            sw.WriteLine("");

        //            //namespace
        //            sw.WriteLine("namespace " + Namespace.NameIsValid(cu.Namespaces[0].Name));
        //            sw.WriteLine("{");

        //            //Custom attributes
        //            GetCustomAttributes(ctd, lAttributeNames).ForEach(s => sw.WriteLine(Tabs(1) + s));

        //            sw.WriteLine(Tabs(1) + DeclareClass(ctd));
        //            sw.WriteLine(Tabs(1) + "{");
        //            //Fields
        //            foreach (CodeMemberField f in lFieldMembers)
        //            {
        //                //Custom field attributes
        //                GetFieldCustomAttributes(f, lAttributeNames).ForEach(s => sw.WriteLine(Tabs(2) + s));

        //                DeclareField(ctd, f, true).ForEach(s => sw.WriteLine(Tabs(2) + s));
        //            }

        //            //Enclose class
        //            sw.WriteLine(Tabs(1) + "}");

        //            //Enclose Namespace
        //            sw.WriteLine("}");
        //            sw.Close();
        //        }
        //    }
        //    catch (Exception ae)
        //    {
        //        string s = ae.ToString();
        //    }
        //}

        //private void WriteInterface(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path)
        //{
        //    try
        //    {
        //        List<CodeAttributeDeclaration> lAttributes = new List<CodeAttributeDeclaration>();
        //        List<CodeMemberField> lFields = new List<CodeMemberField>();

        //        foreach (CodeAttributeDeclaration at in ctd.CustomAttributes)
        //        { lAttributes.Add(at); }
        //        lAttributes.OrderBy(a => a.Name);

        //        foreach (CodeMemberField m in ctd.Members)
        //        { lFields.Add(m); }
        //        lFields.OrderBy(f => f.Name);


        //        using (var sw = new StreamWriter(path))
        //        {
        //            lAttributes.ForEach(a => sw.WriteLine("using " + a.Name + ";"));



        //            sw.Close();
        //        }
        //    }
        //    catch (System.Exception ae)
        //    {
        //        string s = ae.ToString();
        //    }
        //}

        //private void WriteStruct(CodeTypeDeclaration ctd, CodeCompileUnit cu, string path)
        //{
        //    try
        //    {
        //        List<CodeAttributeDeclaration> lAttributes = new List<CodeAttributeDeclaration>();
        //        List<CodeMemberField> lFields = new List<CodeMemberField>();

        //        foreach (CodeAttributeDeclaration at in ctd.CustomAttributes)
        //        { lAttributes.Add(at); }
        //        lAttributes.OrderBy(a => a.Name);

        //        foreach (CodeMemberField m in ctd.Members)
        //        { lFields.Add(m); }
        //        lFields.OrderBy(f => f.Name);


        //        using (var sw = new StreamWriter(path))
        //        {
        //            lAttributes.ForEach(a => sw.WriteLine("using " + a.Name + ";"));



        //            sw.Close();
        //        }
        //    }
        //    catch (System.Exception ae)
        //    {
        //        string s = ae.ToString();
        //    }
        //}

        //private string DeclareClass(CodeTypeDeclaration ctd)
        //{
        //    string declare = "";
        //    if (Configuration.AssemblyVisible) declare = "internal ";
        //    else declare = "public ";

        //    if (ctd.IsPartial) declare = declare + "partial ";

        //    declare = declare + "class " + ctd.Name;

        //    return declare;
        //}

        //private string DeclareEnum(CodeTypeDeclaration ctd)
        //{
        //    string declare = Tabs(1);
        //    if (Configuration.AssemblyVisible) declare = "internal ";
        //    else declare = "public ";


        //    declare = declare + "enum " + ctd.Name;

        //    return declare;
        //}

        //private List<string> DeclareField(CodeTypeDeclaration ctd, CodeMemberField f, bool ExcludeComma)
        //{
        //    List<string> code = new List<string>();
        //    string comma = ",";
        //    if (ExcludeComma) comma = "";

        //    if (Configuration.DisableComments == false)
        //    {
        //        foreach (var c in f.Comments)
        //        {
        //            code.Add("///" + c);
        //        }
        //    }

        //    string props = "";
        //    if (f.Attributes.ToString() == "Public")
        //    { props = props + "public "; }
        //    else
        //    { }

        //    props = props + SystemTypesToBaseTypes(f.Type.BaseType) + " " + f.Name + comma;

        //    code.Add(props);

        //    return code;
        //}

        //private List<string> GetCustomAttributes(CodeTypeDeclaration ctd, List<string> usingDirectives)
        //{
        //    List<string> custom = new List<string>();
        //    try
        //    {
        //        foreach (CodeAttributeDeclaration cd in ctd.CustomAttributes)
        //        {
        //            string ca = "[";
        //            ca = ca + UsingDirectiveReduction(cd.Name, usingDirectives);
        //            if (cd.Arguments.Count > 0)
        //            {
        //                ca = ca + "(";
        //                for (int i = 0; i < cd.Arguments.Count; i++)
        //                {
        //                    CodeAttributeArgument aa = cd.Arguments[i];
        //                    if (aa.Value.GetType() == typeof(CodePrimitiveExpression))
        //                    {
        //                        CodePrimitiveExpression pe = (CodePrimitiveExpression)aa.Value;
        //                        string prefix = "";
        //                        if (pe.Value.ToString().Contains("http"))
        //                        { prefix = "Namespace="; }
        //                        if (i == cd.Arguments.Count - 1)
        //                        { ca = ca + prefix + "\"" + pe.Value + "\""; }
        //                        else
        //                        { ca = ca + prefix + "\"" + pe.Value + "\", "; }
        //                    }
        //                    else if (aa.Value.GetType() == typeof(CodeTypeOfExpression))
        //                    {
        //                        CodeTypeOfExpression te = (CodeTypeOfExpression)aa.Value;
        //                        ca = ca + "typeof(" + te.Type.BaseType + ")";
        //                    }
        //                    else
        //                    { }

        //                }
        //                ca = ca + ")";
        //            }
        //            else
        //            {
        //                ca = ca + "()";
        //            }
        //            ca = ca + "]";
        //            custom.Add(ca);
        //        }
        //    }
        //    catch (Exception ae)
        //    {
        //        string strError = ae.ToString();
        //        if (ae.InnerException != null) strError = ae.InnerException.Message.ToString();
        //    }

        //    return custom;
        //}

        //private List<string> GetFieldCustomAttributes(CodeMemberField f, List<string> usingDirectives)
        //{
        //    List<string> custom = new List<string>();
        //    try
        //    {
        //        foreach (CodeAttributeDeclaration cd in f.CustomAttributes)
        //        {
        //            string ca = "[";
        //            ca = ca + UsingDirectiveReduction(cd.Name, usingDirectives);
        //            if (cd.Arguments.Count > 0)
        //            {
        //                ca = ca + "(";
        //                for (int i = 0; i < cd.Arguments.Count; i++)
        //                {
        //                    CodeAttributeArgument aa = cd.Arguments[i];
        //                    if (aa.Value.GetType() == typeof(CodePrimitiveExpression))
        //                    {
        //                        CodePrimitiveExpression pe = (CodePrimitiveExpression)aa.Value;
        //                        if (i == cd.Arguments.Count - 1)
        //                        { ca = ca + "\"" + pe.Value + "\""; }
        //                        else
        //                        { ca = ca + "\"" + pe.Value + "\", "; }
        //                    }
        //                    else
        //                    { }
        //                }
        //                ca = ca + ")";
        //            }
        //            else
        //            {
        //                ca = ca + "()";
        //            }
        //            ca = ca + "]";
        //            custom.Add(ca);
        //        }
        //    }
        //    catch (Exception ae)
        //    {
        //        string strError = ae.ToString();
        //        if (ae.InnerException != null) strError = ae.InnerException.Message.ToString();
        //    }

        //    return custom;
        //}

        


        //private string SystemTypesToBaseTypes(string systemType)
        //{
        //    switch (systemType)
        //    {
        //        //TODO List, dictionary, enumerate (including nesting) need to be hadled here too
        //        case "System.Void": //usually an enum value
        //            return "";
        //        case "System.SByte":
        //            return "sbyte";
        //        case "System.Byte":
        //            return "byte";
        //        case "System.Int16":
        //            return "short";
        //        case "System.UInt16":
        //            return "ushort";
        //        case "System.Int32":
        //            return "int";
        //        case "System.UInt32":
        //            return "uint";
        //        case "System.Int64":
        //            return "long";
        //        case "System.UInt64":
        //            return "ulong";
        //        case "System.Double":
        //            return "double";
        //        case "System.Char":
        //            return "char";
        //        case "System.String":
        //            return "string";
        //        default:
        //            return systemType;
        //    }
        //}

        //private static string Tabs(int n)
        //{
        //    return new String('\t', n);
        //}
    }
}
