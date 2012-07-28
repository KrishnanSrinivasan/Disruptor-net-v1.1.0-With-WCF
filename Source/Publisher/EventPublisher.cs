using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Common;

namespace Publisher
{
    /// <summary>
    /// Publishes event to the <see cref="IEventService"/> for the configured number of iterations
    /// </summary>
    internal sealed class EventPublisher 
    {
        private readonly Int32 _Id;
        private readonly Int64 _iterations;
        private readonly Barrier _syncStartBarrier;

        internal EventPublisher(Int32 id, Int64 iterations, Barrier syncStartBarrier) 
        {
            _Id = id;
            _iterations = iterations;
            _syncStartBarrier = syncStartBarrier;
        }

        internal void Start() 
        {
            //Create a proxy to the event service
            EndpointAddress remoteAddress = new EndpointAddress(Config.Communincation.ServiceURI);
            NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            IChannelFactory<IEventService> channelFactory = new ChannelFactory<IEventService>(netNamedPipeBinding, remoteAddress);
            IEventService eventService = channelFactory.CreateChannel(remoteAddress);
            
            //Signal ready and wait for other threads to join.
            _syncStartBarrier.SignalAndWait();

            EventSource eventSource = new EventSource() { ID = Guid.NewGuid(), Name = string.Format("Publisher:{0}", _Id) };
            Console.WriteLine("{0} Running...", eventSource);

            Event @event = new Event() { Source = eventSource, Info = String.Format("EVENT PUBLISHED AT[{0}]", DateTime.Now.ToLongTimeString()), RecordedAt = DateTime.Now };
            Byte[] bytes = ProtoBufSerializer.Serialize<Event>(@event);

            //Start publishing events
            for (Int64 i = 0; i < _iterations; i++)
            {
                eventService.Handle(bytes);
            }

            channelFactory.Close();
        }
    }
}
