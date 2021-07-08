using System;
using System.Collections.Generic;

namespace MetricApiTest
{
    public interface IAggregator
    {
        void Update<T>(T value);
        IMetric Collect(DateTime endTime, bool reset = true);
    }
}