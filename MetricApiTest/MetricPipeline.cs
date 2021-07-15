using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetricApiTest
{
    public class MetricPipeline
    {
        private MeasurementProcessor measurementProcessor;
        private MetricProcessor metricProcessor;
        private object exportLock = new object();
        private bool isDelta;
        private bool disposedValue;
        private CancellationTokenSource token;
        public MetricPipeline(MeasurementProcessor measurementProcessor,
            MetricProcessor metricProcessor,
            bool isDelta)
        {
            this.measurementProcessor = measurementProcessor;
            this.metricProcessor = metricProcessor;
            this.metricProcessor.SetGetMetric(this.GetMetrics);
            this.isDelta = isDelta;
        }

        public void Process(Instrument instrument,
            long measurement,
            ReadOnlySpan<KeyValuePair<string, object?>> tags)
        {
            this.measurementProcessor.Process<long>(instrument, measurement, tags);
        }

        private List<IMetric> GetMetrics()
        {
            lock (exportLock)
            {
                var metrics = this.measurementProcessor.GetMetrics(DateTime.UtcNow, this.isDelta);
                return metrics;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.token.Cancel();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
