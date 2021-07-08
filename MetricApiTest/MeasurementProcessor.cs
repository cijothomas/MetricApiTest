using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricApiTest
{
    public class MeasurementProcessor
    {
        private ConcurrentDictionary<string, IAggregator> aggregators = new ConcurrentDictionary<string, IAggregator>();
        private readonly Dictionary<string, bool> views = new Dictionary<string, bool>();
        public MeasurementProcessor()
        {
            this.views.Add("verb", true);
            this.views.Add("success", true);
            this.views.Add("statuscode", true);
        }

        public void Process<T>(Instrument instrument,
            T measurement,
            ReadOnlySpan<KeyValuePair<string, object?>> tags)
            where T : struct
        {
            List<KeyValuePair<string, object>> tagsForStorageList = new List<KeyValuePair<string, object>>();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < tags.Length; i ++)
            {
                if (this.views.ContainsKey(tags[i].Key))
                {
                    tagsForStorageList.Add(new KeyValuePair<string, object>(tags[i].Key, tags[i].Value));
                }
            }

            KeyValuePair<string, object>[] tagsForStorage = new KeyValuePair<string, object>[tagsForStorageList.Count];
            int j = 0;
            foreach (var tag in tagsForStorageList)
            {
                tagsForStorage[j++] = new KeyValuePair<string, object>(tag.Key, tag.Value);
            }
            sb.Append(instrument.Name);
            for (int i = 0; i < tagsForStorage.Length; i++)
            {
                sb.Append(tagsForStorage[i].Key);
                sb.Append(tagsForStorage[i].Value.ToString());
            }
            var tagsAsKey = sb.ToString();
            if (!this.aggregators.TryGetValue(tagsAsKey, out var aggregator))
            {
                var instrumentTypeName = instrument.GetType().Name;
                if (instrumentTypeName.Contains("ObservableCounter"))
                {
                    aggregator = new LongGaugeAggregator(instrument.Name, tagsForStorage);
                }
                else if (instrumentTypeName.Contains("Counter"))
                {
                    aggregator = new LongSumAggregator(instrument.Name, tagsForStorage);
                }
                aggregators.TryAdd(tagsAsKey, aggregator);
            }

            if (typeof(T) == typeof(long))
            {
                aggregator.Update<long>((long)(object)measurement);
            }
        }

        public List<IMetric> GetMetrics(DateTime collectionTime, bool reset)
        {
            List<IMetric> metrics = new List<IMetric>();
            foreach (var aggregator in this.aggregators)
            {
                var value = aggregator.Value.Collect(collectionTime, reset);
                metrics.Add(value);
            }
            return metrics;
        }
    }
}
