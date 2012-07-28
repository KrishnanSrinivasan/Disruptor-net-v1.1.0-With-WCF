using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Common;

namespace Publisher
{
    /// <summary>
    /// Simulates Multiple Producers to send messages to a single consumer service.
    /// </summary>
    public class Program 
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Using LAMX Distruptor .Net 2.x With WCF - Publisher");
            Console.WriteLine();
            
            Console.WriteLine("NOTE:Start the Consumer before starting the publisher!");
            Console.WriteLine("Press <ENTER> to start Publisher.");
            Console.ReadLine();
            Console.WriteLine("Creating {0} Publishers...", Config.Test.MaxPublisers);

            Thread[] publisherThreads = new Thread[Config.Test.MaxPublisers];
            EventPublisher[] publishers = new EventPublisher[Config.Test.MaxPublisers];
            Barrier syncPublisherStartBarrier = new Barrier(Config.Test.MaxPublisers);

            for (Int16 i = 0; i < Config.Test.MaxPublisers; i++)
            {
                publishers[i] = new EventPublisher(i, Config.Test.EventsPerPublisher, syncPublisherStartBarrier);
            }

            for (Int16 i = 0; i < Config.Test.MaxPublisers-1; i++)
            {
                publisherThreads[i] = new Thread(publishers[i].Start);
                publisherThreads[i].Name = String.Format("Publisher {0}", i);
                publisherThreads[i].Start();
            }

            var stopwatch = Stopwatch.StartNew();

            publishers[Config.Test.MaxPublisers - 1].Start();

            if (publisherThreads.GetLength(0) > 1)
            {
                for (Int16 i = 0; i < Config.Test.MaxPublisers; i++)
                {
                    if(publisherThreads[i]!=null)
                        publisherThreads[i].Join();
                }
            }

            stopwatch.Stop();

            var totalEventsPublished = Config.Test.MaxPublisers * Config.Test.EventsPerPublisher;
            var eventsPublishedPerSecond = ( totalEventsPublished * 1000L) / stopwatch.ElapsedMilliseconds;

            StringBuilder summaryBuilder = new StringBuilder();
            summaryBuilder.AppendLine("\nEvent Publish Summary");
            summaryBuilder.AppendLine("------------------------------------------------------------------------");
            summaryBuilder.AppendFormat("Total Events Published\t:{0:###,###,###,###}\n", totalEventsPublished);
            summaryBuilder.AppendFormat("Elapsed Duration(ms)\t:{0:###,###}\n", stopwatch.ElapsedMilliseconds);
            summaryBuilder.AppendFormat("Throughput(Tps)\t\t:{0:###,###,###,###}", eventsPublishedPerSecond);
            Log.Write(summaryBuilder.ToString());

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
        }
    }
}
