//Fredric Lagedal AH2318, 2017-09-19, Assignment 1

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UtilitiesLib
{
    /// <summary>
    /// Klass som används för binary och xml serialization och deserialization
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Sparar ett objekt till vald fil genom binary serialization
        /// </summary>
        public static void BinaryFileSerialize(Object obj, string filePath)
        {           
            using (Stream output = File.Create(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(output, obj);
            }     
        }

        /// <summary>
        /// Laddar och returnerar ett obj genom binary deserialization
        /// </summary>
        public static object BinaryFileDeserialize(string filePath)
        {           
            using(Stream input = File.OpenRead(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(input);
            }

        }


        /// <summary>
        /// Sparar ett objekt av typ T till vald fil genom xml serialization
        /// </summary>
        public static void XmlFileSerialize<T>(string filePath, T obj)
        {             
            using (TextWriter stream = new StreamWriter(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);
            }             
        }


        /// <summary>
        /// Laddar och returnerar ett obj av typ T genom xml deserialization
        /// </summary>
        public static T XmlFileDeserialize<T>(string filePath)
        {           
            using (TextReader stream = new StreamReader(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stream);
            }     
        }
    }
}
