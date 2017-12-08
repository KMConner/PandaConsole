using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections;

namespace PandaConsole
{
    [DataContract]
    public class SakaiSiteCollection
    {
        [DataMember(Name = "site_collection")]
        public List<SakaiSite> Items;
        public SakaiSiteCollection()
        {
            Items = new List<SakaiSite>();
        }

        public static SakaiSiteCollection Create(string jsonString)
        {
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            SakaiSiteCollection colleciton;
            using (var memoryStream = new MemoryStream(jsonBytes))
            {
                var deserializer = new DataContractJsonSerializer(typeof(SakaiSiteCollection));
                colleciton = (SakaiSiteCollection)deserializer.ReadObject(memoryStream);
                memoryStream.Dispose();
            }
            return colleciton;
        }

    }
}
