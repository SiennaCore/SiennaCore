using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna
{
    [RegisterCommands("server", "ChatServer", 1)]
    public class ChatServer
    {
        public static bool ChatHandler(string[] command)
        {
            int nargs = command.Length - 1;

            return false;
        }
        public static void Help()
        {
            Log.Info(">> Command : .server");
            Log.Info(">> No sub command available");
        }
    }
}
