using System;
using Common;

namespace Consumer
{
    /// <summary>
    /// A static instance wrapper around <see cref="DiamondPathEventHandlingSequencer"/> for easy access. 
    /// </summary>
    internal static class EventSequencer
    {
        private static Int32 SequencingBufferSize = 1024 * 512;

        private static DiamondPathEventHandlingSequencer<SequencerEntry, Byte[], Event> _instance;

        /// <summary>
        /// Sets up the Disruptor with the <see cref="IEventHandlers"/>s to be invoked in a Diamond Path configuration.
        /// </summary>
        internal static void Initialze(EventHandlingStep inclusionFlag)
        {
            if (_instance != null)
                throw new InvalidOperationException("EventSequencer already Initialized.");

            _instance = new DiamondPathEventHandlingSequencer<SequencerEntry, Byte[], Event>(
                SequencingBufferSize, SequencerEntry.Empty);

            bool shouldByPassJournaling = ((EventHandlingStep.Journal & inclusionFlag) == 0);
            bool shouldByPassUnMarshalling = ((EventHandlingStep.UnMarshall & inclusionFlag) == 0);
            bool shouldByPassTracking = ((EventHandlingStep.Track & inclusionFlag) == 0);
            
            _instance.StartWith(new EventJournaler(shouldByPassJournaling)).
                        ForkTo(new EventAssembler(shouldByPassUnMarshalling)).
                        EndBy(new EventTracker(shouldByPassTracking)).
                        HandleExeceptionWith(new ExceptionHandler());

            _instance.Start();
        }

        /// <summary>
        /// Publishes event to <see cref=" Distruptor"/>
        /// </summary>
        /// <param name="eventDTO"></param>
        internal static void Publish(Byte[] eventDTO)
        {
            if (_instance == null)
                throw new InvalidOperationException("EventSequencer is not Initialized.");

            _instance.PublishEvent(eventDTO);
        }

        /// <summary>
        /// Invokes Dispose on the <see cref=" Distruptor"/>.
        /// </summary>
        internal static void Dispose()
        {
            if (_instance == null)
                throw new InvalidOperationException("EventSequencer is not Initialized.");

            _instance.Dispose();
            _instance = null;
        }
    }
}
