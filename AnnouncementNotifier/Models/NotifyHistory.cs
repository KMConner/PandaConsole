using System;
using System.ComponentModel.DataAnnotations;

namespace AnnouncementNotifier.Models
{
    class NotifyHistory
    {
        [Key]
        public Guid AnnouncementId { get; set; }
    }
}
