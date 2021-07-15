using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetricApiTest
{
    public class PushMetricProcessor : MetricProcessor, IDisposable
    {
        private Task exportTask;
        private CancellationTokenSource token;
        private int exportIntervalMs;
        private bool disposedValue;
        private Func<List<IMetric>> getMetrics;

        public PushMetricProcessor(int exportIntervalMs)
        {
            this.exportIntervalMs = exportIntervalMs;
            if (exportIntervalMs != 0)
            {
                this.token = new CancellationTokenSource();
                this.exportTask = new Task(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        Task.Delay(this.exportIntervalMs).Wait();
                        Export();
                    }
                });

                this.exportTask.Start();
            }
        }

        public override void SetGetMetric(Func<List<IMetric>> getMetrics)
        {
            this.getMetrics = getMetrics;
        }

        private void Export()
        {
            if (this.getMetrics != null)
            {
                var metricsToExport = this.getMetrics();
                this.Process(metricsToExport);
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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PushMetricProcessor()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
