using System;
using System.Diagnostics;
using System.Text;
using Common;

namespace Consumer
{
    /// <summary>
    /// Entry point for the Consumer
    /// </summary>
    class Program
    {
        //Simple flag to toggle inclusion of Eventhandlers.
        private static EventHandlingStep eventHandlingSteps = EventHandlingStep.BypassAllSteps;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Using LAMX Distruptor .Net 2.x With WCF - Consumer");
            Console.WriteLine();

            Console.WriteLine("NOTE:Redis is used for Journaling. Ensure its started before you proceed.");
            Console.WriteLine("Press <ENTER> to continue.");
            Console.ReadLine();

            //Process command args.
            if (args == null || args.GetLength(0) < 1)
            {
                PrintUsage();
                return;
            }

            try
            {
                eventHandlingSteps = (EventHandlingStep)Enum.Parse(typeof(EventHandlingStep), args[0]);
            }
            catch 
            {
                PrintUsage();
                return;
            }

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;

            //Setup the service host for hosting the EventService
            EventServiceHost eventServiceHost = new EventServiceHost();
            eventServiceHost.Start();
            Console.WriteLine();

            //Setup the EventSequencer
            EventSequencer.Initialze(eventHandlingSteps);
            Console.WriteLine();

            try
            {
                //Setup RedisDB
                RedisDB.Initialize();
            }
            catch(Exception ex) 
            {
                Console.WriteLine("Error connecting to Redis.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press <ENTER> exit.");
                Console.ReadLine();
                return;
            }

            //Setup the simple session to track the throughput of the events processed
            var iterations = Config.Test.MaxPublisers * Config.Test.EventsPerPublisher;
            PerfTestSession.Initialize(iterations);
            PerfTestSession.Instance.SessionEnded += DisplayPerfTestSessionSummary;

            Console.WriteLine("Consumer started with Option <{0}>.", eventHandlingSteps);
            Console.WriteLine("Press <ENTER> to stop consumer.");
            Console.ReadLine();

            //Disposing all instances
            PerfTestSession.Instance.SessionEnded -= DisplayPerfTestSessionSummary;
            eventServiceHost.Stop();
            EventSequencer.Dispose();
            RedisDB.Instance.Dispose();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
        }

        private static void DisplayPerfTestSessionSummary(Int64 iterationsProcessed, Int64 elapsedMilliseconds, String gcSummary)
        {
            var tps = (iterationsProcessed / elapsedMilliseconds) * 1000L;

            StringBuilder summaryBuilder = new StringBuilder();
            summaryBuilder.AppendFormat("\nPerf Test Session Results for [{0}] Path\n", eventHandlingSteps);
            summaryBuilder.AppendLine("------------------------------------------------------------------------");
            summaryBuilder.AppendFormat("Total Transactions Processed\t:{0:###,###,###,###}\n", iterationsProcessed);
            summaryBuilder.AppendFormat("Elapsed Duration(ms)\t\t:{0:###,###,###,###}\n", elapsedMilliseconds);
            summaryBuilder.AppendFormat("Throughput(Tps)\t\t\t:{0:###,###,###,###}\n", tps);
            summaryBuilder.AppendFormat("GC(0-1-2)\t\t\t:{0}", gcSummary);

            Log.Write(summaryBuilder.ToString());
        }

        private static void PrintUsage()
        {
            Type options = typeof(EventHandlingStep);
            var names = Enum.GetNames(options);
            var values = Enum.GetValues(options);

            StringBuilder usageStringBuilder = new StringBuilder();
            usageStringBuilder.AppendLine("Usage: Consumer.exe <EventHandlingStep>");
            usageStringBuilder.AppendLine();
            usageStringBuilder.AppendFormat("{0} Options:", options.Name);
            usageStringBuilder.AppendLine();
            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];
                var value = (int)values.GetValue(i);
                usageStringBuilder.AppendFormat("{0} \t :[{1}]", value, name);
                usageStringBuilder.AppendLine();
            }

            usageStringBuilder.AppendLine();
            usageStringBuilder.AppendFormat("Example: Consumer.exe {0}", EventHandlingStep.UnMarshallAndTrack);
            Console.WriteLine(usageStringBuilder.ToString());
        }
    }
}

