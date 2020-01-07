using System;
using System.Threading.Tasks;
using SES.Client;
namespace SES.Samples.Client
{
        public class SampleEvent
        {
            public string SampleData{get;set;}
        }
    class Program
    {

        private static bool processInput=true;
        static async Task Main(string[] args)
        {
            Console.CancelKeyPress+=(s,o)=>processInput=false;
            using(var p = new Publisher<SampleEvent>(new PublisherOptions(){Endpoint="http://localhost:5000"}))
            {       
                while(processInput)
                {
                    Console.Write("Enter a message:");
                    var data=Console.ReadLine();
                    await p.PublishAsync(new SampleEvent{SampleData=data});
                }
            }
        }
    }
}
