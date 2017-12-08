using System;
using System.Runtime.Serialization;
using System.Collections.Generic;


namespace PandaConsole.Sakai
{
    /// <summary>
    /// This class represents Sakai resource collection.
    /// </summary>
    [DataContract]
    public class SakaiResourceCollection
    {
        /// <summary>
        /// The items of this collection.
        /// </summary>
        [DataMember(Name = "content_collection")]
        public List<SakaiResource> Items;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PandaConsole.Sakai.SakaiResourceCollection"/> class.
        /// </summary>
        public SakaiResourceCollection()
        {
            Items = new List<SakaiResource>();
        }

        /// <summary>
        /// Creates <see cref="SakaiResourceCollection"/> object from json.
        /// </summary>
        /// <returns>The <see cref="SakaiResourceCollection"/> object created from specified json string.</returns>
        /// <param name="jsonString">Json string.</param>
        public static SakaiResourceCollection CreateFromJson(string jsonString)
        {
            return Utilities.DeserializeJson<SakaiResourceCollection>(jsonString);
        }
    }
}
