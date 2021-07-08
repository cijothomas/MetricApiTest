using System;
using System.Collections.Generic;
using System.Threading;

namespace MetricApiTest
{
    public class HistogramSumAggregator : IAggregator
    {
        private KeyValuePair<string, object>[] tags { get; }
        private long sum;
        private long min = long.MinValue;
        private long max = long.MaxValue;
        private long count;
        private string name;
        private object updateLock = new object();

        public HistogramSumAggregator(string name, KeyValuePair<string, object>[] tags)
        {
            this.tags = tags;
            this.name = name;
        }

        public void Update<T>(T value)
        {
            if (typeof(T) == typeof(long))
            {
                lock (updateLock)
                {
                    var longValue = (long)(object)value;
                    this.sum+= longValue;
                    this.count++;
                    if (longValue < this.min || this.min == long.MinValue)
                    {
                        this.min = longValue;
                    }
                    if (longValue > this.max || this.max == long.MaxValue)
                    {
                        this.max = longValue;
                    }
                }

            }
        }

        public IMetric Collect(DateTime endTime, bool reset = true)
        {
            LongHistogramMetric histogramMetric;
            lock (updateLock)
            {
                histogramMetric = new LongHistogramMetric(this.name, this.tags, this.sum, this.count, this.min, this.max, reset, endTime);
                if (reset)
                {
                    this.Reset();
                }
            }

            return histogramMetric;
        }

        private void Reset()
        {
            this.count = 0;
            this.sum = 0;
            this.min = long.MinValue;
            this.max = long.MaxValue;
        }
    }
}