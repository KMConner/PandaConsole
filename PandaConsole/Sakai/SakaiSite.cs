using System.Runtime.Serialization;

namespace PandaConsole.Sakai
{
    /// <summary>
    /// This class represents Sakai site.
    /// </summary>
    [DataContract]
    public class SakaiSite
    {
        /// <summary>
        /// Gets or sets the site title.
        /// </summary>
        /// <value>The title.</value>
        [DataMember(Name = "title")]
		public string Title { set; get; }

        /// <summary>
        /// Gets or sets the description of this site.
        /// </summary>
        /// <value>The description.</value>
        [DataMember(Name = "description")]
        public string Description { set; get; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember(Name = "id")]
        public string Id { get; set; }
    }
}
