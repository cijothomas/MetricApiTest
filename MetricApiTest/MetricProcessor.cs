using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricApiTest
{
    public class MetricProcessor
    {
        public void Process(List<IMetric> metrics)
        {
            foreach (var metric in metrics)
            {
                var tags = metric.Tags;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < tags.Length; i++)
                {
                    sb.Append(tags[i].Key);
                    sb.Append(tags[i].Value.ToString());
                }
                var tagsAsPrintable = sb.ToString();
                switch (metric.MetricType)
                {
                    case MetricType.LongSum:
                        var longSumMetric = metric as LongSumMetric;
                        Console.WriteLine($"{longSumMetric.BeginTime} {metric.EndTime} :{metric.Name} Tags{{ {tagsAsPrintable} }} Value {longSumMetric.Sum}");
                        break;
                    case MetricType.LongGauge:
                        var longGaugeMetric = metric as LongGaugeMetric;
                        Console.WriteLine($"{metric.EndTime} :{metric.Name} Tags{{ {tagsAsPrintable} }} Value {longGaugeMetric.LastValue}");
                        break;
                }
            }

            Console.WriteLine("---------------------------------");
        }
    }
}
