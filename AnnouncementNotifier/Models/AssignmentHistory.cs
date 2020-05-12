using System;
using System.ComponentModel.DataAnnotations;

namespace AnnouncementNotifier.Models
{
    class AssignmentHistory
    {
        [Key]
        public Guid AssignmentId { get; set; }
    }
}
