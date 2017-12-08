using System;
using System.Runtime.Serialization;
using System.Collections.Generic;


namespace PandaConsole.Sakai
{
    [DataContract]
    public class SakaiResourceCollection
    {
        [DataMember(Name = "content_collection")]
        public List<SakaiResource> Items;

        public SakaiResourceCollection()
        {
            Items = new List<SakaiResource>();
        }

        public static SakaiResourceCollection CreateFromJson(string jsonString)
        {
            return Utilities.DeserializeJson<SakaiResourceCollection>(jsonString);
        }
    }
}
