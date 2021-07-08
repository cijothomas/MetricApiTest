using System;
using System.Collections.Generic;
using System.Threading;

namespace MetricApiTest
{
    public class LongHistogramMetric : IMetric
    {
        public string Name { get; }

        public KeyValuePair<string, object>[] Tags { get; }

        public MetricType MetricType { get; private set; }

        public DateTime EndTime { get; }

        public bool IsDelta;

        public long Sum, Count, Min, Max;

        public LongHistogramMetric(string name, KeyValuePair<string, object>[] tags, long sum, long count, long min, long max, bool isDelta, DateTime endTime)
        {
            this.Name = name;
            this.Tags = tags;
            this.Sum = sum;
            this.Count = count;
            this.Min = min;
            this.Max = max;
            this.IsDelta = isDelta;
            this.MetricType = MetricType.Histogram;
            this.EndTime = endTime;
        }
    }
}