using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SoundCollector.Utils
{
    class DataUtils
    {
        public static void SaveObjectAsXmlToFile(ISerializable toSerial, String filePath)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(toSerial.GetType());
            using (System.IO.TextWriter file = new System.IO.StreamWriter(filePath))
            {
                writer.Serialize(file, toSerial);
            }
        }

        public static T LoadObjectAsXmlFromFile<T>(String filePath) where T : ISerializable
        {
            System.Xml.Serialization.XmlSerializer deserializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (System.IO.TextReader reader = new System.IO.StreamReader(filePath))
            {
                return (T)deserializer.Deserialize(reader);
            }
        }

        public static T LoadObjectAsXmlFromStream<T>(System.IO.Stream fileStream) where T : ISerializable
        {
            System.Xml.Serialization.XmlSerializer deserializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (System.IO.TextReader reader = new System.IO.StreamReader(fileStream))
            {
                return (T)deserializer.Deserialize(reader);
            }
        }
    }
}
