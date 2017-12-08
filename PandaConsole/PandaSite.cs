﻿using System.Runtime.Serialization;

using System;
namespace PandaConsole
{
    [DataContract]
    public class PandaSite
    {
        [DataMember(Name = "title")]
		public string Title { set; get; }

        [DataMember(Name = "description")]
        public string Description { set; get; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        public PandaSite()
        {
        }

    }
}
