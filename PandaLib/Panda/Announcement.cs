using System;
using System.Runtime.Serialization;

namespace PandaLib.Panda
{
    [DataContract]
    public class Announcement
    {
        [DataMember(Name = "announcementId")]
        public Guid Id { get; set; }

        [DataMember(Name = "siteId")]
        public string SiteId { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "createdByDisplayName")]
        public string CreatedBy { get; set; }


        [DataMember(Name = "createdOn")]
        public long CreatedOnTick { get; set; }

        [IgnoreDataMember]
        public DateTimeOffset CreatedAt
            => DateTimeOffset.FromUnixTimeMilliseconds(CreatedOnTick);
    }
}
