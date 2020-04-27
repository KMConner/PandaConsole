using System;
using System.Runtime.Serialization;

namespace PandaLib.Panda
{
    [DataContract]
    public class Assignment
    {
        /// <summary>
        /// Gets or sets the final deadline of this assignment.
        /// </summary>
        [DataMember(Name ="closeTime")]
        public Time CloseTime { get; set; }

        /// <summary>
        /// Gets or sets the Site id associated with this assignment.
        /// </summary>
        [DataMember(Name ="context")]
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the due time of this assignment.
        /// </summary>
        [DataMember(Name ="dueTime")]
        public Time DueTime { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of this assignment.
        /// </summary>
        [DataMember(Name ="id")]
        public Guid id { get; set; }

        /// <summary>
        /// Gets or sets the instruction of this assignment in HTML format. 
        /// </summary>
        [DataMember(Name = "instructions")]
        public string Instructions { get; set; }
        
        /// <summary>
        /// Gets or sets the time when this assignment is opened.
        /// </summary>
        [DataMember(Name ="openTime")]
        public Time OpenTime { get; set; }
        
        /// <summary>
        /// Gets or sets the creation time of this assignment.
        /// </summary>
        [DataMember(Name = "timeCreated")]
        public Time TimeCreated { get; set; }
        
        /// <summary>
        /// Gets or sets the title of this assignment.
        /// </summary>
        [DataMember(Name ="title")]
        public string Title { get; set; }
    }
}
