using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace Sienna
{
    [RegisterCommands("help", "ChatHelp", 0)]
    public class ChatHelp
    {
        public static bool ChatHandler(string[] command)
        {
            int nargs = command.Length - 1;

            if (nargs < 1)
            {
                Log.Info(">> Commands available :");

                foreach (KeyValuePair<string, string> sComm in Commands.sCommands)
                    Log.Info(">> ." + sComm.Key);

                return true;
            }
            else
            {
                // search for help in different ChatClasses
                string aClass;

                if (Commands.sCommands.TryGetValue(command[1], out aClass))
                {
                    Assembly a = Assembly.GetExecutingAssembly();
                    Type t = a.GetType("Sienna." + aClass);

                    try
                    {
                        object i = Activator.CreateInstance(t);

                        MethodInfo m = t.GetMethod("Help");

                        m.Invoke(i, new object[]{});
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
        }
        public static void Help()
        {
            Log.Info(">> Command : .help");
            Log.Info(">> Use .help command");
        }
    }
}
