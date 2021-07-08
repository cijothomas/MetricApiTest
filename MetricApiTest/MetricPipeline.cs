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
        private int exportIntervalMs;
        private bool disposedValue;
        private Task exportTask;
        private CancellationTokenSource token;
        public MetricPipeline(MeasurementProcessor measurementProcessor,
            MetricProcessor metricProcessor,
            bool isDelta,
            int exportIntervalMs = 0)
        {
            this.measurementProcessor = measurementProcessor;
            this.metricProcessor = metricProcessor;
            this.isDelta = isDelta;
            this.exportIntervalMs = exportIntervalMs;
            this.token = new CancellationTokenSource();

            if (exportIntervalMs != 0)
            {
                this.exportTask = new Task(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        Task.Delay(this.exportIntervalMs).Wait();
                        // this.meterListener.RecordObservableInstruments();
                        this.ExportMetrics();
                    }
                });

                this.exportTask.Start();
            }
        }

        public void Process(Instrument instrument,
            long measurement,
            ReadOnlySpan<KeyValuePair<string, object?>> tags)
        {
            this.measurementProcessor.Process<long>(instrument, measurement, tags);
        }

        public void ExportMetrics()
        {
            lock (exportLock)
            {
                var metrics = this.measurementProcessor.GetMetrics(DateTime.UtcNow, this.isDelta);
                this.metricProcessor.Process(metrics);
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
