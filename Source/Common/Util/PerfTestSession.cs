using System;
using System.Diagnostics;

namespace Common
{
    /// <summary>
    /// Utility class to track the performance metrics of a Test Session that involves asynchronous processing. 
    /// Fires an event <see cref="OnSessionEnded"/> when the expected number of iterations are processed.
    /// </summary>
    public sealed class PerfTestSession
    {
        private Int64 _iterationCounter;
        private Stopwatch _stopwatch;

        private readonly Int64 _iterations;
        private readonly GCWatch _gcWatch;

        public delegate void OnSessionEnded(Int64 iterationsProcessed, Int64 elapsedMilliseconds, String gcSummary);
        public event OnSessionEnded SessionEnded = delegate { };

        public static PerfTestSession Instance { get; private set; }

        public static void Initialize(Int64 iterations)
        {
            if(Instance == null)
            {
                Instance = new PerfTestSession(iterations);
                return;
            }

            throw new InvalidOperationException("PerfTestSession already initialized");
        }

        private PerfTestSession(Int64 iterations) 
        {
            _iterationCounter = 0;
            _iterations = iterations;
            _gcWatch = GCWatch.StartNew();
        }

        public void IncrementIterationCount()
        {
            if (_iterationCounter == 0) 
            {
                _stopwatch = Stopwatch.StartNew();
            }
            
            _iterationCounter++;
            
            if (_iterations == _iterationCounter)
            {
                _stopwatch.Stop();
                _gcWatch.Stop();
                SessionEnded(_iterations, _stopwatch.ElapsedMilliseconds, _gcWatch.ToString());
            }
        }
    }
}
