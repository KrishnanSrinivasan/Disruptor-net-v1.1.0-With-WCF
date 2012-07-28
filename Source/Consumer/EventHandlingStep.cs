using System;

namespace Consumer
{
    /// <summary>
    /// Simple flag to bypass / include the processing of an <see cref="IEventHandler"/>. 
    /// If bypassed the processing portion is not executed when the <see cref="IEventHandler"/> is invoked.
    /// </summary>
    [Flags]
    internal enum EventHandlingStep
    {
        /// <summary>
        /// Set this only to Journal the events.
        /// </summary>
        Journal = 0x2,
        
        /// <summary>
        /// Set this only yo UnMarshall the eventDTO to an event.
        /// </summary>
        UnMarshall = 0x4,
        
        /// <summary>
        /// Set this to Track events. This step needs un-marshalling turned on. 
        /// And cannot be used as a standalone step.
        /// </summary>
        Track = 0x8,

        /// <summary>
        /// Set this to by-pass Journaling.
        /// </summary>
        UnMarshallAndTrack = UnMarshall | Track,
        
        /// <summary>
        /// Set this to include all steps
        /// </summary>
        JournalUnMarshallTrack = Journal | UnMarshall | Track,
        
        /// <summary>
        /// Set this to bypass all steps.
        /// </summary>
        BypassAllSteps = ~JournalUnMarshallTrack
    }
}
