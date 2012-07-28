using System;

namespace Common
{
    /// <summary>
    /// Holds settings shared between Publisher and Consumer.
    /// </summary>
    public struct Config
    {
        public struct Test 
        {
            public const Int16 MaxPublisers = 10;
            public const Int64 EventsPerPublisher = 100000;
        }
        
        public struct Communincation 
        {
            public static readonly Uri ServiceURI = new Uri("net.pipe://127.0.0.1/EventService");
        }
    }
}
