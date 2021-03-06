﻿using System;
using System.Collections.Generic;

namespace Entry
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CustomSettings.inputPath = @"C:\Users\TomJames_zyl8law\Lightos\Lightos Hub - Documents\01-Clients\Atkins\Rail\RailML\2.4\schema\";
                //CustomSettings.inputPath = @"C:\Users\TomJames_zyl8law\Lightos\Lightos Hub - Documents\01-Clients\Atkins\Rail\RailML\3.1\schema\";
                //CustomSettings.inputPath = @"C:\Users\TomJames_zyl8law\Lightos\Lightos Hub - Documents\01-Clients\Atkins\Rail\SDEF\XSDs\7.2\";
                CustomSettings.outputPath = @"C:\Users\TomJames_zyl8law\source\repos\XSDtoCSharp\XSDtoSQL\";

                List<string> errs = CustomSettings.Get();
                if (errs.Count > 0)
                {
                    Console.WriteLine("Custom settings errors were found:");
                    foreach (var s in errs)
                    {
                        Console.WriteLine(errs.IndexOf(s).ToString() + ": " + s);
                    }
                    Console.WriteLine("");
                    Console.WriteLine("Execution failed");
                }
                else
                {
                    CustomSettings.generator.Generate(CustomSettings.files);
                }

            }
            catch (Exception ae)
            {
                Console.WriteLine(ae.Message.ToString());
            }
        }
    }
}
