using System;
using System.Collections.Generic;
using System.Threading;

namespace MetricApiTest
{
    public class LongGaugeMetric : IMetric
    {
        public string Name { get; }

        public KeyValuePair<string, object>[] Tags { get; }

        public MetricType MetricType { get; private set; }

        public DateTime EndTime { get; }

        public long LastValue;

        public LongGaugeMetric(string name, KeyValuePair<string, object>[] tags, long value, DateTime endTime)
        {
            this.Name = name;
            this.Tags = tags;
            this.LastValue = value;
            this.MetricType = MetricType.LongGauge;
            this.EndTime = endTime;
        }
    }
}