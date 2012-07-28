using System;
using Disruptor;
using Common;

namespace Consumer
{
    /// <summary>
    /// Simple type Deserializer. Writes back the deserialized object  
    /// to the <cref=ISequencerEntry/> for further downstream processing.
    /// </summary>
    internal sealed class EventAssembler
        : IEventHandler<ISequencerEntry<Byte[], Event>>
    {
        private readonly Boolean _byPass;

        internal EventAssembler(Boolean byPass) 
        {
            _byPass = byPass;
        }
        
        void IEventHandler<ISequencerEntry<byte[], Event>>.OnNext(ISequencerEntry<byte[], Event> data, long sequence, bool endOfBatch)
        {
            if (!_byPass)
            {
                Event @event = ProtoBufSerializer.DeSerialize<Event>(data.GetEventDTO());
                data.Write(@event);
            }
        }
    }
}

