using System;
using System.Collections.Generic;

namespace MetricApiTest
{
    public interface IMetric
    {
        MetricType MetricType { get; }

        string Name { get; }

        KeyValuePair<string, object>[] Tags { get; }

        DateTime EndTime { get; }
    }
}