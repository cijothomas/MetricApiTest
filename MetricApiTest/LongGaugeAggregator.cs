using System;
using System.Collections.Generic;
using System.Threading;

namespace MetricApiTest
{
    public class LongGaugeAggregator : IAggregator
    {
        private KeyValuePair<string, object>[] tags { get; }
        private long value;
        private string name;

        public LongGaugeAggregator(string name, KeyValuePair<string, object>[] tags)
        {
            this.tags = tags;
            this.name = name;
        }

        public void Update<T>(T value)
        {
            if (typeof(T) == typeof(long))
            {
                this.value = (long)(object)value;
            }
        }

        public IMetric Collect(DateTime endTime, bool reset = true)
        {
            var snapShot = this.value;
            var longGaugeMetric = new LongGaugeMetric(this.name, this.tags, snapShot, endTime);
            return longGaugeMetric;
        }
    }
}