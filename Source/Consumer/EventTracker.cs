using System;
using Common;
using System.Collections.Generic;
using Disruptor;

namespace Consumer
{
    /// <summary>
    /// Simple Event Tracker that tracks the number of events published from each source. 
    /// This is the end of the event handling chain.
    /// </summary>
    internal sealed class EventTracker
        : IEventHandler<ISequencerEntry<Byte[], Event>> 
    {
        private readonly Dictionary<EventSource, Int64> _eventCounter =
            new Dictionary<EventSource, Int64>();

        private readonly Boolean _byPass;

        internal EventTracker(Boolean byPass) 
        {
            _byPass = byPass;
        }

        void IEventHandler<ISequencerEntry<byte[], Event>>.OnNext(ISequencerEntry<byte[], Event> data, long sequence, bool endOfBatch)
        {
            if (!_byPass)
            {
                Event @event = data.GetEvent();

                if (!_eventCounter.ContainsKey(@event.Source))
                    _eventCounter.Add(@event.Source, 0);

                _eventCounter[@event.Source]++;
            }

            PerfTestSession.Instance.IncrementIterationCount();
        }
    }
}

