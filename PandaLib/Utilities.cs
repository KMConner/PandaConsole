using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Diagnostics;

namespace PandaLib
{
    static class Utilities
    {
        /// <summary>
        /// Deserializes the json and retrives object of specified type.
        /// </summary>
        /// <returns>The object parsed from the specified json string.</returns>
        /// <param name="jsonString">Json string.</param>
        /// <typeparam name="T">The type to parse the json into.</typeparam>
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

        /// <summary>
        /// Opens the file with default app.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public static void OpenFileDefaultWithApp(string filePath)
        {
            Process.Start(filePath);
        }
    }
}
