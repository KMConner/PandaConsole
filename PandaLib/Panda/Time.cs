using System;
using System.Runtime.Serialization;

namespace PandaLib.Panda
{
    public class Time
    {
        [DataMember(Name = "time")]
        public long TickMilliseconds { get; set; }

        [IgnoreDataMember]
        public DateTimeOffset DateTime
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(TickMilliseconds);
            set => TickMilliseconds = value.ToUnixTimeMilliseconds();
        }
    }
}
