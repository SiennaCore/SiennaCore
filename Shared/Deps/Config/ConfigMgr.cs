using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Shared
{
    static public class ConfigMgr
    {
        static public List<aConfig> _Configs = new List<aConfig>();

        static public void LoadConfigs()
        {
            if (_Configs.Count > 0)
                return;

            Log.Debug("ConfigMgr", "Loading Config files");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true)
                        continue;

                    object[] attrib = type.GetCustomAttributes(typeof(aConfigAttributes), true);
                    if (attrib.Length <= 0)
                        continue;

                    aConfigAttributes[] ConfigAttribs = (aConfigAttributes[])type.GetCustomAttributes(typeof(aConfigAttributes), true);

                    if (ConfigAttribs.Length > 0)
                    {
                        Log.Debug("ConfigMgr", "Deserializing : " + type.Name);

                        aConfig Obj = null;
                        XmlSerializer Xml = new XmlSerializer(type);

                        try
                        {
                            FileInfo FInfo = new FileInfo(ConfigAttribs[0].FileName);
                            Directory.CreateDirectory(FInfo.DirectoryName);
                        }
                        catch(Exception)
                        {
                        }

                        FileStream fs = new FileStream(ConfigAttribs[0].FileName, FileMode.OpenOrCreate);

                        if (fs.Length > 0)
                        {
                            Obj = Xml.Deserialize(fs) as aConfig;
                            fs.Close();
                        }
                        else
                        {
                            Obj = Activator.CreateInstance(type) as aConfig;
                            Xml.Serialize(fs, Obj);
                            fs.Close();
                        }

                        Log.Succes("ConfigMgr", "Registering config : " + ConfigAttribs[0].FileName);
                        _Configs.Add(Obj);
                    }
                }
            }
        }

        static public T GetConfig<T>()
        {
            aConfig Conf = null;
            Conf = _Configs.Find(info => info.GetType() == typeof(T));

            if(Conf != null)
                return (T)Convert.ChangeType(Conf, typeof(T));
            else
                return (T)Convert.ChangeType(null, typeof(T));
        }
    }
}
