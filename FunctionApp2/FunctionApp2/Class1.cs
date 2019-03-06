using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionApp2
{
    public static class Class1
    {
        public static async Task Run____(DurableOrchestrationContext context)
        {
            var parallelTasks = new List<Task<int>>();

            // get a list of N work items to process in parallel
            object[] workBatch = await context.CallActivityAsync<object[]>("F1", "");

            for (int i = 0; i < workBatch.Length; i++)
            {
                Task<int> task = context.CallActivityAsync<int>("F2", workBatch[i]);

                parallelTasks.Add(task);
            }

            await Task.WhenAll(parallelTasks);

            //aggregate all N outputs and sned result to F3
            int sum = parallelTasks.Sum(t => t.Result);

            await context.CallActivityAsync("F3", sum);
        }

        // HTTP-triggered function to start a new orchestrator function instance.
        public static async Task<HttpResponseMessage> Run (HttpRequestMessage req,
            DurableOrchestrationClient starter,
            string functionName,
            ILogger log)
        {
            // Function name comes from the request URL.
            // Function input comes from the request content.

            dynamic eventData = await req.Content.ReadAsAsync<object>();
            string instanceId = await starter.StartNewAsync(functionName, eventData);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        public static async Task GetTaskAsync([OrchestrationClient] DurableOrchestrationClient client,
            HttpRequestMessage req,
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            await client.StartNewAsync("Get_TaskAsync", "username");

            client.CreateCheckStatusResponse(req, context.InstanceId);
        }

        public static async Task Get_TaskAsync([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            await context.WaitForExternalEvent("hair");
        }


        public static void GetTaskAsync_([ActivityTrigger] DurableActivityContext context)
        {
            _ = context.GetInput<string>();

            Console.WriteLine(context.InstanceId);

            return;
        }

        public static async Task TaskAsync([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var aId = context.GetInput<string>();

            var g1 = context.WaitForExternalEvent("Approval1");
            var g2 = context.WaitForExternalEvent("Approval2");
            var g3 = context.WaitForExternalEvent("Approval3");

            await Task.WhenAll(g1, g2, g3);

            await context.CallActivityAsync("Permit", aId);
        }

        public static dynamic RunDynamic([ActivityTrigger] DurableActivityContext context)
        {
            (_, _) = context.GetInput<(string, int)>();

            string id = context.InstanceId;

            return new
            {
            };
        }

        //public static async Task<bool> Run([OrchestrationTrigger]DurableOrchestrationContext context)
        //{
        //    var pn = context.GetInput<string>();

        //    var challengeCode = await context.CallActivityAsync<int>("E4_SendSMSChallenge", pn);

        //    using (var timeoutchs = new CancellationTokenSource())
        //    {
        //        var expiration = context.CurrentUtcDateTime.AddSeconds(90);
        //        var timeoutTask = context.CreateTimer(expiration, timeoutchs.Token);
        //        var authorized = false;

        //        for (int i = 0; i <= 3; i++)
        //        {
        //            var challengeResponseTask = context.WaitForExternalEvent<int>("SmsChallengeeResponse");
        //            var winner = await Task.WhenAny(challengeResponseTask, timeoutTask);

        //            if (winner == challengeResponseTask)
        //            {
        //                if (challengeResponseTask.Result == challengeCode)
        //                {
        //                    authorized = true;
        //                    break;
        //                }
        //            }
        //            else
        //                break;
        //        }

        //        if (!timeoutTask.IsCompleted)
        //        {
        //            timeoutchs.Cancel();
        //        }

        //        return authorized;
        //    }
        //}

        //public static IActionResult Run(HttpRequest req, out object taskDocument, ILogger logger)
        //{
        //    var name = req.Query["name"];
        //    var task = req.Query["task"];
        //    var duedate = req.Query["duedate"];

        //    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(task))
        //    {
        //        taskDocument = new { name, duedate, task };
        //        return new OkResult();
        //    }
        //    else
        //    {
        //        taskDocument = null;
        //        return new BadRequestResult();
        //    }
        //}

        class MyBaseClass
        {
            public virtual string Name { get; set; }

            private int num;
            public virtual int Number
            {
                get { return num; }
                set { num = value; }
            }

            class MyDerivedClass : MyBaseClass
            {
                public string name;

                public override string Name { get => name; set => name = value; }

                public override int Number { get => base.Number; set => base.Number = value; }
            }
        }

        public abstract class Shape
        {
            public Shape(string id)
            {
                Id = id;
            }

            public string Id { get; set; }

            public abstract double Area { get; }

            public override string ToString()
            {
                return $"";
            }
        }

        public class BaggageInfo
        {
            private int flightNo;
            private string origin;
            private int location;

            internal BaggageInfo(int flight, string from, int carousel)
            {
                flightNo = flight;
                origin = from;
                location = carousel;
            }

            public int FlightNumber
            {
                get { return flightNo; }
            }

            public string From
            {
                get { return origin; }
            }

            public int Carousel
            {
                get { return location; }
            }
        }

        public class BaggageHandler : IObservable<BaggageInfo>
        {
            private List<IObserver<BaggageInfo>> observers;
            private List<BaggageInfo> flights;

            public BaggageHandler()
            {
                observers = new List<IObserver<BaggageInfo>>();
                flights = new List<BaggageInfo>();
            }

            public IDisposable Subscribe(IObserver<BaggageInfo> observer)
            {
                if (!observers.Contains(observer))
                {
                    observers.Add(observer);

                    foreach (var item in flights)
                    {
                        observer.OnNext(item);
                    }
                }

                return new Unsubscriber<BaggageInfo>(observers, observer);
            }

            public void BaggageStatus(int flightNo)
            {
                BaggageStatus(flightNo, string.Empty, 0);
            }

            public void BaggageStatus(int flightNo, string from, int carousel)
            {
                var info = new BaggageInfo(flightNo, from, carousel);

                if (carousel > 0 && !flights.Contains(info))
                {
                    flights.Add(info);

                    foreach (var item in observers)
                    {
                        item.OnNext(info);
                    }
                }
                else if (carousel == 0)
                {
                    var flightsToRemove = new List<BaggageInfo>();

                    foreach (var item in flights)
                    {
                        if (info.FlightNumber == item.FlightNumber)
                        {
                            flightsToRemove.Add(item);

                            foreach (var observer in observers)
                            {
                                observer.OnNext(info);
                            }
                        }
                    }

                    foreach (var item in flightsToRemove)
                    {
                        flights.Remove(item);
                    }

                    flightsToRemove.Clear();
                }
            }

            public void LastBaggageClaimed()
            {
                foreach (var item in observers)
                {
                    item.OnCompleted();
                }

                observers.Clear();
            }
        }
    }

    internal class Unsubscriber<BaggageInfo> : IDisposable
    {
        private List<IObserver<Class1.BaggageInfo>> observers;
        private IObserver<Class1.BaggageInfo> observer;

        public Unsubscriber(List<IObserver<Class1.BaggageInfo>> observers, IObserver<Class1.BaggageInfo> observer)
        {
            this.observers = observers;
            this.observer = observer;
        }

        public void Dispose()
        {
            if (observers.Contains(observer))
                observers.Remove(observer);
        }
    }
}
