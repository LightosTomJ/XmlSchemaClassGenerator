using System;
using System.Collections.Generic;
using System.Linq;
using XmlSchemaClassGenerator.SQL.Components;

namespace XmlSchemaClassGenerator.Validation
{
    public static class ForeignKey
    {
        public static DBRoles IsNameUnique(DBRoles dbRoles)
        {
            try
            {
                foreach (Table t in dbRoles.Schemas)
                {
                    if (t.Keys.Count > t.Keys.Select(k => k.Name).Distinct().Count())
                    {
                        //Duplicate key name
                        List<string> lNames = t.Keys.Select(k => k.Name).ToList();
                        foreach (string name in lNames)
                        {
                            if (t.Keys.Count(k => k.Name == name) > 1)
                            {
                                List<Key> lDupKeys = t.Keys.Where(k => k.Name == name).ToList();
                                for (int i = 0; i < lDupKeys.Count; i++)
                                {
                                    if (lDupKeys.Count >= 10 && lDupKeys.Count < 100)
                                    {
                                        lDupKeys[i].Name = lDupKeys[i].Name + i.ToString("00");
                                    }
                                    else if (lDupKeys.Count >= 100 && lDupKeys.Count < 1000)
                                    {
                                        lDupKeys[i].Name = lDupKeys[i].Name + i.ToString("000");
                                    }
                                    else if (lDupKeys.Count >= 1000 && lDupKeys.Count < 10000)
                                    {
                                        lDupKeys[i].Name = lDupKeys[i].Name + i.ToString("0000");
                                    }
                                    else if (lDupKeys.Count >= 10000 && lDupKeys.Count < 100000)
                                    {
                                        lDupKeys[i].Name = lDupKeys[i].Name + i.ToString("00000");
                                    }
                                    else
                                    {
                                        lDupKeys[i].Name = lDupKeys[i].Name + (i + 1).ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ae)
            { string s = ae.ToString(); }

            return dbRoles;
        }
    }
}
