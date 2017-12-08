using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Diagnostics;


namespace PandaConsole
{
    public static class Utilities
    {
        public static T DeserializeJson<T>(string jsonString)
        {
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            T colleciton;
            using (var memoryStream = new MemoryStream(jsonBytes))
            {
                var deserializer = new DataContractJsonSerializer(typeof(T));
                colleciton = (T)deserializer.ReadObject(memoryStream);
                memoryStream.Dispose();
            }
            return colleciton;
        }

        public static void OpenFileDefaultWithApp(string filePath)
        {
            Process.Start(filePath);
        }
    }
}
