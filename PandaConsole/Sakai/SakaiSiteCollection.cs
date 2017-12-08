using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections;

namespace PandaConsole.Sakai
{
    /// <summary>
    /// This class represents Sakai site collection.
    /// </summary>
    [DataContract]
    public class SakaiSiteCollection
    {
        /// <summary>
        /// The items of this collection.
        /// </summary>
        [DataMember(Name = "site_collection")]
        public List<SakaiSite> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PandaConsole.Sakai.SakaiSiteCollection"/> class.
        /// </summary>
        public SakaiSiteCollection()
        {
            Items = new List<SakaiSite>();
        }

        /// <summary>
        /// Create <see cref="T:PandaConsole.Sakai.SakaiSiteCollection"/> object from the specified json string.
        /// </summary>
        /// <returns>Created object.</returns>
        /// <param name="jsonString">Json string.</param>
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
