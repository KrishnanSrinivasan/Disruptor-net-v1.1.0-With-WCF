using System;

namespace Consumer
{
    /// <summary>
    /// Interface for Type that is stored in the <see cref="DiamondPathEventHandlingSequencer"/>. This class 
    /// allows an <see cref="IEventHandler"/> to directly write its output  
    /// to the instance that can be consumed by downstream <see cref="IEventHandler"/>s.
    /// </summary>
    /// <typeparam name="TEventDTO">Marhalled Type of the <see cref="TEvent"/></typeparam>
    /// <typeparam name="TEvent">Un-Marhalled Type of the <see cref="TEvent"/></typeparam>
    internal interface ISequencerEntry<TEventDTO, TEvent>
    {
        /// <summary>
        /// Identifier to track the sequence entry
        /// </summary>
        Guid ID { get; set; }
        
        /// <summary>
        /// Writes(Copy) the raw DTO of type <see cref="TEventDTO"/> to the <see cref="IEventSequencerEntry"/>.
        /// </summary>
        /// <param name="eventDTO"></param>
        void Write(TEventDTO eventDTO);

        /// <summary>
        /// Write(Copy) the un-marshalled instance of type <see cref="TEvent"/> to the <see cref="IEventSequencerEntry">.
        /// </summary>
        /// <param name="event"></param>
        void Write(TEvent @event);

        /// <summary>
        /// Gets the stored marshalled value of the event.
        /// </summary>
        /// <returns><see cref="TEventDTO"/></returns>
        TEventDTO GetEventDTO();

        /// <summary>
        /// Gets the stored un-marshalled value of  event.
        /// </summary>
        /// <returns><see cref="TEvent"/></returns>
        TEvent GetEvent();
    }
}
