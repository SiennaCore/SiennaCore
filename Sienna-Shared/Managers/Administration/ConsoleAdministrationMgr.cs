using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna
{
    public static class ConsoleAdministrationMgr
    {
        public static void ExecuteConsoleCommands()
        {
            string ClientCommand;

            while (true)
            {
                try
                {
                    ClientCommand = Console.ReadLine();

                    if ((ClientCommand != null) && (ClientCommand.StartsWith(".")))
                        Commands.ParseCommand(ClientCommand);
                    else
                        Log.Error("[Error] Read error");
                }
                catch (Exception e)
                {
                    Log.Error("[Error] Read error : " + e);
                }

            }
        }
    }
}
