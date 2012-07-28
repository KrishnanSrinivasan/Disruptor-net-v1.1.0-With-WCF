using System;
using System.ServiceModel;
using Common;

namespace Consumer
{
    /// <summary>
    /// Host class for the <see cref="IEventService"/>
    /// </summary>
    class EventServiceHost
    {
        private ServiceHost eventServiceHost;

        internal void Start()
        {
            Console.WriteLine("Starting EventService Host...");

            eventServiceHost = CreateHost();
            eventServiceHost.Open();
            
            Console.WriteLine(eventServiceHost.GetHostedServiceEndPoints());
            Console.WriteLine("EventService Host Running...");
        }

        internal void Stop()
        {
            Console.WriteLine("Stopping EventService Host...");
            
            eventServiceHost.Close();
            
            Console.WriteLine("EventService stopped.");
        }

        private ServiceHost CreateHost()
        {
            NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            ServiceHost eventServiceHost = new ServiceHost(typeof(EventService));
            eventServiceHost.AddServiceEndpoint(typeof(IEventService), netNamedPipeBinding, Config.Communincation.ServiceURI);
            eventServiceHost.AddDefaultMEXEndPoint();
            eventServiceHost.EnableIncludeExceptionInFaultBehavior();
            return eventServiceHost;
        }
    }
}

