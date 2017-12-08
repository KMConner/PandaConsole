﻿using System;
using System.Runtime.Serialization;



namespace PandaConsole
{
    [DataContract]
    public class SakaiResource
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "modifiedDate")]
        public string ModifiedDate { get; set; }

        [DataMember(Name = "url")]
        public Uri Url { get; set; }

        [DataMember(Name = "size")]
        public int Size { get; set; }
    }
}
