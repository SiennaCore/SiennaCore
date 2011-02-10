using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Sienna
{
    public static class Commands
    {
        public static Dictionary<string, string> sCommands = new Dictionary<string, string>();

        public static void LoadCommands()
        {
            try
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type t in a.GetTypes())
                    {
                        object[] attrib = t.GetCustomAttributes(typeof(RegisterCommands), true);

                        if (attrib.Length <= 0)
                            continue;

                        var RegisterVar = (RegisterCommands[])t.GetCustomAttributes(typeof(RegisterCommands), true);

                        if (RegisterVar.Length > 0)
                        {
                            Log.Info(">> Command loaded : " + RegisterVar[0].Command + " (Method: " + RegisterVar[0] + ", Min args: " + RegisterVar[0].Minargs + ")");
                            sCommands.Add(RegisterVar[0].Command, RegisterVar[0].Method);
                        }
                    }
                }

                Log.Info(">> Loaded : " + sCommands.Count + " admin commands");
            }
            catch (ArgumentException)
            {
                Log.Error("[Error] Unable to load command table");
            }
        }

        public static bool LookupCommand(string prefix, string[] args)
        {
            string aClass;

            if (sCommands.TryGetValue(prefix.Replace(".", ""), out aClass))
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Type t = a.GetType("Sienna."+aClass);

                try
                {
                    object i = Activator.CreateInstance(t);

                    MethodInfo m = t.GetMethod("ChatHandler");

                    m.Invoke(i, new object[] { args });
                }
                catch (Exception e)
                {
                    Log.Error("[Error] Exception LookupCommand" + e);
                }

                return true;
            }
            else
                return false;
        }

        public static void ParseCommand(string command)
        {
            //List<string> args = new List<string>();

            int nargs = command.IndexOf(' ');
            if (nargs < 0)
                nargs = command.Length;

            string prefix = command.Substring(0, nargs);
            //command = command.Remove(0, nargs);

            string[] args = command.Split(' ');

            if (prefix.Length <= 0)
                Log.Warning("[Warn] Invalid command");

            else if (!LookupCommand(prefix, args))
                Log.Warning("[Warn] Unknown command : " + prefix);
        }
    }
}
