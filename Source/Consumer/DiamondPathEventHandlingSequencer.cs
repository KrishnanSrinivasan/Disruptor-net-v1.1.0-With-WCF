using System;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;

namespace Consumer
{
    /// <summary>
    /// Wrapper over the Distruptor 2.x. 
    /// <para>This class sets up the Disruptor for a Diamond Event Handling Path Configuration. 
    /// Can be used for a Muti-Producer Single-Consumer scenario.</para>
    /// Workflow: Muliple Producers ->[Start With Handler -> Fork To Multiple Parallel Handlers  -> End By Handler].
    /// </summary>
    /// <typeparam name="TSequencerEntry">Type that implements <see cref="ISequencerEntry"/> which would be stored in the RingBuffer</typeparam>
    /// <typeparam name="TEventDTO">Type that respresents the un-marshalled version of the event</typeparam>
    /// <typeparam name="TEvent">Type that respresents the marshalled version of the event</typeparam>
    internal class  DiamondPathEventHandlingSequencer<TSequencerEntry, TEventDTO, TEvent> : IDisposable
        where TSequencerEntry : ISequencerEntry<TEventDTO, TEvent>
    {
        private Disruptor<ISequencerEntry<TEventDTO, TEvent>> _disruptor;
        private RingBuffer<ISequencerEntry<TEventDTO, TEvent>> _ringBuffer;

        private IEventHandler<ISequencerEntry<TEventDTO, TEvent>> _startWithEventHandler;
        private IEventHandler<ISequencerEntry<TEventDTO, TEvent>>[] _forkToEventHandlers;
        private IEventHandler<ISequencerEntry<TEventDTO, TEvent>> _endByEventHandler;
        private IExceptionHandler _exceptionHandler;

        public DiamondPathEventHandlingSequencer(Int32 capacity, Func<ISequencerEntry<TEventDTO, TEvent>> sequencerEntryFactory)
            
        {
            _disruptor = new Disruptor<ISequencerEntry<TEventDTO, TEvent>>(
                            sequencerEntryFactory,
                            new MultiThreadedClaimStrategy(capacity),
                            new YieldingWaitStrategy(),
                            TaskScheduler.Default);
        }

        /// <summary>
        /// The first <see cref="IEventHandler"/> to process the event.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public DiamondPathEventHandlingSequencer<TSequencerEntry, TEventDTO, TEvent> StartWith(
            IEventHandler<ISequencerEntry<TEventDTO, TEvent>> handler) 
        {
            _startWithEventHandler = handler;
            return this;
        }

        /// <summary>
        /// The second set of parallel <see cref="IEventHandler"/>s that need to process the event.
        /// </summary>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public DiamondPathEventHandlingSequencer<TSequencerEntry, TEventDTO, TEvent> ForkTo(
            params IEventHandler<ISequencerEntry<TEventDTO, TEvent>>[] handlers)
        {
            _forkToEventHandlers = handlers;
            return this;
        }

        /// <summary>
        /// The last <see cref="IEventHandler"/> to process the event.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public DiamondPathEventHandlingSequencer<TSequencerEntry, TEventDTO, TEvent> EndBy(
            IEventHandler<ISequencerEntry<TEventDTO, TEvent>> handler)
        {
            _endByEventHandler = handler;
            return this;
        }

        /// <summary>
        /// Exception Handler. Handles all exceptions that get thrown from any <see cref="IEventHandler"/>.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public DiamondPathEventHandlingSequencer<TSequencerEntry, TEventDTO, TEvent> HandleExeceptionWith(
           IExceptionHandler handler)
        {
            _exceptionHandler = handler;
            return this;
        }

        /// <summary>
        /// Bootstraps the <see cref="Disruptor"/> and starts it.
        /// </summary>
        public void Start()
        {
            if(_startWithEventHandler == null)
                throw new ArgumentNullException("StartWith EventHandler cannot be null");

            if (_forkToEventHandlers == null || _forkToEventHandlers.GetLength(0) < 1)
                throw new ArgumentNullException("There mush be atleast one ForkTo EventHandler");

            if(_endByEventHandler == null)
                throw new ArgumentNullException("EndBy EventHandler cannot be null");

            _disruptor.HandleExceptionsWith(_exceptionHandler);

            _disruptor.
                HandleEventsWith(_startWithEventHandler).
                Then(_forkToEventHandlers).
                Then(_endByEventHandler);
            
            _ringBuffer = _disruptor.RingBuffer;

            Console.WriteLine("Starting DiamondPathEventHandlingSequencer...");
            
            _disruptor.Start();
            
            Console.WriteLine("DiamondPathEventHandlingSequencer (Capacity:{0:###,###,###,###}) Running...", _ringBuffer.BufferSize);
        }

        /// <summary>
        /// Publishes event onto <see cref="Disruptor"/> by claiming the next sequence.
        /// </summary>
        /// <param name="value"></param>
        public void PublishEvent(TEventDTO value)
        {
            var sequence = _ringBuffer.Next();
            ISequencerEntry<TEventDTO, TEvent> storeEntry = _ringBuffer[sequence];
            storeEntry.Write(value);
            _ringBuffer.Publish(sequence);
        }

        /// <summary>
        /// Disposes the <see cref="Disruptor"/>.
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine("DiamondPathEventHandlingSequencer shutting down...");
            
            _disruptor.Shutdown();
            
            Console.WriteLine("DiamondPathEventHandlingSequencer shutdown.");
        }
    }
}
