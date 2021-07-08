using System;
using System.Collections.Generic;
using System.Threading;

namespace MetricApiTest
{
    public class LongSumMetric : IMetric
    {
        public string Name { get; }

        public KeyValuePair<string, object>[] Tags { get; }

        public MetricType MetricType { get; private set; }

        public DateTime EndTime { get; }

        public DateTime BeginTime { get; }

        public long Sum;

        public bool IsDelta;


        public LongSumMetric(string name, KeyValuePair<string, object>[] tags, long value, bool isDelta, DateTime endTime, DateTime beginTime)
        {
            this.Name = name;
            this.Tags = tags;
            this.Sum = value;
            this.IsDelta = isDelta;
            this.MetricType = MetricType.LongSum;
            this.EndTime = endTime;
            this.BeginTime = beginTime;
        }
    }
}