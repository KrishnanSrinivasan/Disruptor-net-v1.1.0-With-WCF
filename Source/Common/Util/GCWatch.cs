using System;
using System.Diagnostics;

namespace Common
{
    /// <summary>
    /// Utility class to track Garbage collections.
    /// </summary>
    public sealed class GCWatch 
    {
        private readonly Int32 _startGen0GCCount;
        private readonly Int32 _startGen1GCCount;
        private readonly Int32 _startGen2GCCount;
        
        private readonly Stopwatch _stopwatch;

        private Int32 _gen0GCCount;
        private Int32 _gen1GCCount;
        private Int32 _gen2GCCount;
        private Int64 _watchDurationMilliseconds;

        public Int32 Gen0Collections { get { return _gen0GCCount; } }
        public Int32 Gen1Collections { get { return _gen1GCCount; } }
        public Int32 Gen2Collections { get { return _gen2GCCount; } }
        public Int64 WatchDurationMilliseconds { get { return _watchDurationMilliseconds; } }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="GCWatch"/> after running a GC.Collect.
        /// </summary>
        /// <returns></returns>
        public static GCWatch StartNew() 
        {
            return new GCWatch();
        }

        private GCWatch() 
        {
            GC.Collect();

            _startGen0GCCount = GC.CollectionCount(0);
            _startGen1GCCount = GC.CollectionCount(1);
            _startGen2GCCount = GC.CollectionCount(2);

            _stopwatch = Stopwatch.StartNew();
        }

        public void Stop()
        {
            _stopwatch.Stop();
            _watchDurationMilliseconds = _stopwatch.ElapsedMilliseconds;

            _gen0GCCount = GC.CollectionCount(0) - _startGen0GCCount;
            _gen1GCCount = GC.CollectionCount(1) - _startGen1GCCount;
            _gen2GCCount = GC.CollectionCount(2) - _startGen2GCCount;
        }

        public override String ToString()
        {
            return String.Format("[{0}-{1}-{2}], Elapsed(ms):{3:###,###,###,###}",
                _gen0GCCount, _gen1GCCount, _gen2GCCount, _watchDurationMilliseconds);
        }
    }
}
