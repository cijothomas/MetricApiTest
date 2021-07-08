using System;
using System.Collections.Generic;
using System.Threading;

namespace MetricApiTest
{
    public class LongSumAggregator : IAggregator
    {
        private KeyValuePair<string, object>[] tags { get; }
        private long value;
        private string name;
        private DateTime dateTimeStart;

        public LongSumAggregator(string name, KeyValuePair<string, object>[] tags)
        {
            this.tags = tags;
            this.name = name;
            this.dateTimeStart = DateTime.UtcNow;
        }

        public void Update<T>(T value)
        {
            if (typeof(T) == typeof(long))
            {
                Interlocked.Add(ref this.value, (long)(object)value);
            }
        }

        public IMetric Collect(DateTime endTime, bool reset = true)
        {
            var timeSnapShot = this.dateTimeStart;
            long snapShot;
            if (reset)
            {
                snapShot = Interlocked.Exchange(ref this.value, 0);
                this.dateTimeStart = DateTime.UtcNow;
            }
            else
            {
                snapShot = Interlocked.Read(ref this.value);
            }

            var longSumMetric = new LongSumMetric(this.name, this.tags, snapShot, reset, endTime, timeSnapShot);
            return longSumMetric;
        }
    }
}