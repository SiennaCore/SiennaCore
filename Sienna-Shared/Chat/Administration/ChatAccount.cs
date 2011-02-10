using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sienna
{
    [RegisterCommands("account", "ChatAccount", 4)]
    public class ChatAccount
    {
        public static bool ChatHandler(string[] command)
        {
            int nargs = command.Length - 1;

            if (nargs < 4)
            {
                Log.Error("[Error] Bad argument count");
                return false;
            }
            else if ((command[1].StartsWith("create")) && (nargs == 4))
            {
                Log.Info("[Debug] create account");

                if (command[3] != command[4])
                {
                    Log.Warning("[Warn] Passwords are differents");
                    return false;
                }

                string username = command[2];
                string password = command[3];

                if (!AccountManager.CreateAccount(username, password))
                {
                    Log.Error("[Error] Account " + username + " already exists");
                    return false;
                }
                else
                {
                    Log.Info(">> Account " + username + " created");
                    return true;
                }
            }
            else if (command[1].StartsWith("set"))
            {
                if ((command[2].StartsWith("password")) && (nargs == 5))
                {
                    Log.Info("[Debug] change password");

                    if (command[4] != command[5])
                    {
                        Log.Warning("[Warn] Passwords are differents");
                        return false;
                    }

                    string username = command[3];
                    string password = command[4];

                    if (!AccountManager.ChangePassword(username, password))
                    {
                        Log.Error("[Error] Account " + username + " not exists");
                        return false;
                    }
                    else
                    {
                        Log.Info(">> Password changed for account " + username);
                        return true;
                    }
                }
                else if ((command[2].StartsWith("gmlevel")) && (nargs == 3))
                {
                    Log.Info("[Debug] change gmlevel");

                    return false;
                }
            }

            return false;
        }
        public static void Help()
        {
            Log.Info(">> Command : .account");
            Log.Info(">> .account create username password password");
            Log.Info(">> .account set password username password password");
        }
    }
}
