using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;

namespace MetricApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MetricPipeline[] pipelines = new MetricPipeline[2];
            pipelines[0] = new MetricPipeline(new MeasurementProcessor(),
                new PushMetricProcessor(3000),
                true);

            var pullMetricProcessor = new PullMetricProcessor();
            pipelines[1] = new MetricPipeline(new MeasurementProcessor(),
                pullMetricProcessor,
                true);

            using MeterProvider provider = new MeterProvider("cijolibrary",
                pipelines,
                5000);
            Meter meter = new Meter("cijolibrary", "v1.0");
            Counter<long> counter = meter.CreateCounter<long>("RequestCount");
            var requestCount = GC.GetTotalAllocatedBytes();
            //var observableCounter = meter.CreateObservableCounter<long>("RequestCountObservable", () =>
            //{
            //    // fake RequestCount
            //    var requestCount = GC.GetTotalAllocatedBytes();
            //    Console.WriteLine(requestCount);
            //    return new List<Measurement<long>>()
            //            {
            //                new Measurement<long>(
            //                    requestCount,
            //                     new KeyValuePair<string, object>("verb", "GET"),
            //                     new KeyValuePair<string, object>("success", "true"),
            //                     new KeyValuePair<string, object>("statuscode", "400")),

            //                new Measurement<long>(
            //                    requestCount,
            //                     new KeyValuePair<string, object>("verb", "GET"),
            //                     new KeyValuePair<string, object>("success", "true"),
            //                     new KeyValuePair<string, object>("statuscode", "200"))
            //            };
            //});

            var token = new CancellationTokenSource();
            Task serverTask = new Task(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    counter.Add(10,
                        KeyValuePair.Create<string, object>("verb", "GET"),
                        KeyValuePair.Create<string, object>("statuscode", "400"),
                        KeyValuePair.Create<string, object>("success", "true"));

                    counter.Add(100,
                        KeyValuePair.Create<string, object>("verb", "GET"),
                        KeyValuePair.Create<string, object>("statuscode", "200"),
                        KeyValuePair.Create<string, object>("success", "true"));

                    Task.Delay(100).Wait();
                }
            });

            serverTask.Start();
            char input;
            do
            {
                input = Console.ReadKey().KeyChar;
                if (input == 'c')
                {
                    pullMetricProcessor.Export();
                }
            }
            while (input != 'q');

            token.Cancel();
        }
    }
}
