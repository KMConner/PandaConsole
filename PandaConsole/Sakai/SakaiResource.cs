using System.Runtime.Serialization;

namespace PandaConsole.Sakai
{
    /// <summary>
    /// This class represents resource object of Sakai.
    /// </summary>
    [DataContract]
    public class SakaiResource
    {
        /// <summary>
        /// Gets or sets the file title.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the modified date of the file.
        /// </summary>
        [DataMember(Name = "modifiedDate")]
        public string ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the full URL to the file.
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        [DataMember(Name = "size")]
        public int Size { get; set; }
    }
}
