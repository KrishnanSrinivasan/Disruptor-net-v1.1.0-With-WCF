using System;
using Disruptor;
using Common;

namespace Consumer
{
    /// <summary>
    /// Writes every <see cref="TEventDTO"/> onto a simple Journal Log. 
    /// The Entries are stored in a Redis Database through the <see cref="RedisDB"/> 
    /// as a Key(String) - Value(Byte[]) pair.
    /// </summary>
    internal sealed class EventJournaler 
        : IEventHandler<ISequencerEntry<Byte[], Event>>
    {
        private readonly Boolean _byPass;
        
        internal EventJournaler(Boolean byPass) 
        {
            _byPass = byPass;
        }
        
        void IEventHandler<ISequencerEntry<byte[], Event>>.OnNext(ISequencerEntry<byte[], Event> data, long sequence, bool endOfBatch)
        {
            if (!_byPass)
            {
                data.ID = Guid.NewGuid();
                RedisDB.Instance.Write(data.ID.ToString(), data.GetEventDTO());
            }
        }
    }
}

