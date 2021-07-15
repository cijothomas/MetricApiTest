using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricApiTest
{
    public class MeterProvider : IDisposable
    {
        private bool disposedValue;
        private object exportLock = new object();
        private MeterListener meterListener;
        private MetricPipeline[] metricPipelines;

        public MeterProvider(string libraryName,
            MetricPipeline[] metricPipelines)
        {
            this.metricPipelines = metricPipelines;
            this.meterListener = new MeterListener();
            this.meterListener.InstrumentPublished = (instrument, listener) =>
            {
                if (instrument.Meter.Name.Equals(libraryName, StringComparison.InvariantCultureIgnoreCase))
                {
                    listener.EnableMeasurementEvents(instrument, null);
                }
            };

            this.meterListener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
            {
                for (int i = 0; i < this.metricPipelines.Length; i++)
                {
                    this.metricPipelines[i].Process(instrument, measurement, tags);
                }
            });

            this.meterListener.Start();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    for (int i = 0; i < this.metricPipelines.Length; i++)
                    {
                        this.metricPipelines[i].Dispose();
                    }
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
