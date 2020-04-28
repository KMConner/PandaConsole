using System.Runtime.Serialization;

namespace PandaLib.Panda
{
    [DataContract]
    class AnnouncementCollection
    {

        [DataMember(Name = "announcement_collection")]
        public Announcement[] Announcements { get; set; }
    }
}
