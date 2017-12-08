using System;
using System.Runtime.Serialization;

namespace PandaConsole.Sakai
{
    [DataContract]
    public class SakaiSite
    {
        [DataMember(Name = "title")]
		public string Title { set; get; }

        [DataMember(Name = "description")]
        public string Description { set; get; }

        [DataMember(Name = "id")]
        public string Id { get; set; }
    }
}
