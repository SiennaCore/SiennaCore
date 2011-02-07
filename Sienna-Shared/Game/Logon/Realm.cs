using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Sienna.Database;

namespace Sienna.Game
{
    public class Realm
    {
        public static List<Realm> Realmlist = new List<Realm>();

        protected byte[] _UnkData;
        protected byte _ID;
        protected byte _Population;
        protected byte _Language;
        protected byte _RealmType;
        protected byte _Online;

        public Realm(byte ID, byte Online, byte Population, byte Language, byte RealmType)
        {
            _ID = ID;
            _Population = Population;
            _Language = Language;
            _RealmType = RealmType;
            _Online = Online;
        }

        public byte[] ToLoginData(bool isSelectedRealm)
        {
            MemoryStream ms = new MemoryStream();

            ms.Write(new byte[] { 0xE5, 0x03, 0x02 }, 0, 3);
            ms.WriteByte(_ID);
            ms.WriteByte(0x12);
            ms.Write(new byte[] {  0x0A, 0xB6, 0xC3, 0x01 }, 0, 4);
            
            int RealmStatus = 0x00;

            RealmStatus = RealmStatus ^ _Population; // 80 medium, 112 low, 
            RealmStatus = RealmStatus ^ _Online; // 1 online 2 offline

            int RealmData = 0x00;
            RealmData = RealmData ^ _RealmType; // 16 PVP, 48 PVE, 80 PVE-RP, 0 Locked
            RealmData = RealmData ^ 8;
            RealmData = RealmData ^ 1;
                        
            ms.WriteByte((byte)RealmStatus);
            ms.WriteByte((byte)RealmData);
            ms.WriteByte(0x62); // Online + Locked + Part of lang ?
            ms.WriteByte(_Language); // 0 Unknown, 1 English, 2 Empty, 3 German

            ms.Write(new byte[] { 0x69, 0x07 }, 0, 2);

            return ms.ToArray();
        }

        public static int LoadRealms(SQLDatabase DB)
        {
            Realmlist.Clear();

            List<Row> Result = DB.Execute("SELECT * FROM realms");

            int count = Result.Count;

            if (count == 0)
                return 0;

            foreach (Row r in Result)
            {
                Realm realm = new Realm(byte.Parse(r["id"]), byte.Parse(r["isonline"]), byte.Parse(r["population"]), byte.Parse(r["language"]), byte.Parse(r["realmtype"]));
                Realmlist.Add(realm);
            }

            return count;
        }
    }
}
