using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Shared;

namespace CharacterServer
{
    [ConsoleHandler("realm", 3, "<id(<21),type(1=Pvp,2=RP,3=PVP-RP),lang(1=En-3=Ger-5=Fr)>")]
    public class CreateRealm : IConsoleHandler
    {
        public bool HandleCommand(string command, List<string> args)
        {
            byte Id = byte.Parse(args[0]);
            byte Type = byte.Parse(args[1]);
            byte Lang = byte.Parse(args[2]);

            Realm Rm = Program.CharMgr.GetRealm(Id);
            if (Rm != null) // Realm already registered
            {
                Log.Error("CreateRealm", "Realm id : " + Id + " already exist");
                return false;
            }

            Rm = new Realm();
            Rm.RealmId = Id;
            Rm.Language = Lang;
            Rm.Online = 1;
            Rm.PVP = 0;
            Rm.RP = 0;
            Rm.GenerateName(); // Generate name by RealmId;

            if (Type == 1 || Type == 3)
                Rm.PVP = 1;

            if (Type == 2 || Type == 3)
                Rm.RP = 1;

            Program.CharMgr.AddObject(Rm);
            Program.CharMgr.LoadRealms();

            Log.Success("CreateRealm", "Realm '" + Rm.Name + "' Successsfully added to database.");
            return true;
        }
    }
}
