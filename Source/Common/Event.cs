using System;
using ProtoBuf;

namespace Common
{
    /// <summary>
    /// Container for Event publised from a Publisher.
    /// </summary>
    [ProtoContract]
    public sealed class Event
    {
        [ProtoMember(1)]
        public EventSource Source { get; set; }

        [ProtoMember(2)]
        public String Info { get; set; }

        [ProtoMember(3)]
        public DateTime RecordedAt { get; set; }

        public override string ToString()
        {
            return string.Format("Event From {0} : {1}", Source, Info);
        }
    }
}
