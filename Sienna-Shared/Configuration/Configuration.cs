using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Xml.Serialization;

namespace Sienna
{
    public class Configuration<T>
    {
        public static T Load(String Filename)
        {
            XmlSerializer xmls = new XmlSerializer(typeof(T));

            if (!File.Exists(Filename))
                Configuration<T>.CreateConfigTemplate(Filename);

            FileStream fs = new FileStream(Filename, FileMode.Open);
            T res = (T)xmls.Deserialize(fs);
            return res;
        }

        public static void CreateConfigTemplate(string Filename)
        {
            T Cfg = Activator.CreateInstance<T>();
            XmlSerializer xmls = new XmlSerializer(typeof(T));
            FileStream fs = new FileStream(Filename, FileMode.OpenOrCreate);
            xmls.Serialize(fs, Cfg);
            fs.Close();
        }
    }
}
