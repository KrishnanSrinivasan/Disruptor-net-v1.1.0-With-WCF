using System;
using ProtoBuf;

namespace Common
{
    /// <summary>
    /// Identifies the Source of an Event.
    /// </summary>
    [ProtoContract]
    public sealed class EventSource
    {
        [ProtoMember(1)]
        public Guid ID { get; set; }
        
        [ProtoMember(2)]
        public String Name { get; set; }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;

            EventSource o = obj as EventSource;
            if (ReferenceEquals(o, null)) return false;

            return o.ID.Equals(this.ID);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
