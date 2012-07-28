using System;
using Common;

namespace Consumer
{
    /// <summary>
    /// Simple implementation of <see cref="ISequencerEntry"/> that can be stored in the Sequencer.
    /// The member functions allow <see cref="IEventHandler"/> to read and write data from and to the instance.
    /// </summary>
    internal class SequencerEntry : ISequencerEntry<Byte[], Event>
    {
        private static readonly Int32 EventDTOBufferLength = 1024;

        private Byte[] _eventDTO;
        private Event _event;

        private SequencerEntry(Int64 capacity)
        {
            _eventDTO = new Byte[capacity];
        }

        public Guid ID
        {
            get;
            set;
        }

        public void Write(Byte[] eventDTO)
        {
            Buffer.BlockCopy(eventDTO, 0, _eventDTO, 0, eventDTO.Length);
        }

        public void Write(Event @event)
        {
            _event = @event;
        }

        public Byte[] GetEventDTO()
        {
            return _eventDTO;
        }

        public Event GetEvent()
        {
            return _event;
        }

        public static SequencerEntry Empty()
        {
            return new SequencerEntry(EventDTOBufferLength);
        }
    }
}