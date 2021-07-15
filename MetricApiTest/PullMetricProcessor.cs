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
    public class PullMetricProcessor : MetricProcessor, IDisposable
    {
        private bool disposedValue;
        private Func<List<IMetric>> getMetrics;

        public override void SetGetMetric(Func<List<IMetric>> getMetrics)
        {
            this.getMetrics = getMetrics;
        }

        public void Export()
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
