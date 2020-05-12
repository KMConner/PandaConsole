using System.Runtime.Serialization;

namespace PandaLib.Panda
{
    [DataContract]
    class AssignmentCollection
    {
        [DataMember(Name = "assignment_collection")]
        public Assignment[] Assignments { get; set; }
    }
}