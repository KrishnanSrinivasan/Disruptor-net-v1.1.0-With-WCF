using System;
using System.ServiceModel;
using Messaging.SharedLib;
using System.Threading;
using System.Diagnostics;

namespace Messaging.Consumer
{
    class Test
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Using LAMX Distruptor With WCF - Consumer");

            Console.WriteLine("Press <ENTER> to start");
            Console.ReadLine();

            ManualResetEvent mru = new ManualResetEvent(false);
            Barrier _testStartBarrier = new Barrier(Settings.Messaging.MaxProducers);

            TSingleton<MP1CSequencingBuffer>.Instance.SetUp(
                Settings.Messaging.DefaultMessageStoreBufferSize,
                    EventBufferEntryFactory.Create,
                        Settings.Messaging.TotalMessageDispatched,
                            mru);
            
            EventPublisher[] publishers = new EventPublisher[Settings.Messaging.MaxProducers];
            for (int i = 0; i < Settings.Messaging.MaxProducers; i++)
                publishers[i] = new EventPublisher(_testStartBarrier);

            for (var i = 0; i < Settings.Messaging.MaxProducers - 1; i++)
                (new Thread(publishers[i].Publish)).Start("Event producer " + i );

            var sw = Stopwatch.StartNew();

            publishers[Settings.Messaging.MaxProducers - 1].Publish(
                "Event producer " + Settings.Messaging.MaxProducers);

            mru.WaitOne();

            var opsPerSecond = (Settings.Messaging.TotalMessageDispatched * 1000L) / sw.ElapsedMilliseconds;

            sw.Stop();

            TSingleton<MP1CSequencingBuffer>.Instance.Dispose();

            Console.WriteLine("TotalElapsedTime(ms):{0:###,###,###,###}", sw.ElapsedMilliseconds);
            Console.WriteLine("TotalOps:{0}", Settings.Messaging.TotalMessageDispatched);
            Console.WriteLine("{0:###,###,###,###}op/sec", opsPerSecond);
            Console.ReadLine();
        }
    }

    class EventPublisher 
    {
        private readonly Barrier _barrier;
        
        internal EventPublisher(Barrier barrier)
        {
            _barrier = barrier;
        }
        
        internal void Publish(object param)
        {
            _barrier.SignalAndWait();

            EventSource eventSource = new EventSource(Guid.NewGuid(), param.ToString());

            for (Int64 i = 0; i < Settings.Messaging.MessagePerProducer; i++)
            {
                Event @event = new Event(eventSource, String.Format("EVENT PUBLISHED AT[{0}]", 
                    DateTime.Now.ToLongTimeString()));

                Byte[] bytes = BinarySerializer.Serialize<Event>(@event);
                TSingleton<MP1CSequencingBuffer>.Instance.Write(bytes);
            }
        }
    }
}
