using System.Collections.Generic;
using System.Threading;

namespace MetricApiTest
{
    public enum MetricType
    {
        LongSum = 0,
        LongGauge = 1,
        Histogram = 2
    }
}