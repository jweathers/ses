using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SES.Client;
using SES.Samples.Messages;
using SES.Serialization;

namespace SES.Samples.Client
{

    class Program
    {

        private static bool processInput=true;
        private static Random random = new Random();
        private static List<string> strings = new List<string>();
        static async Task Main(string[] args)
        {
            Console.CancelKeyPress+=(s,o)=>processInput=false;
            LoadSampleString();
            IAsyncEventSerializer serializer = new SES.Serialization.Json.JsonSerializer();
            using(var p = new Publisher<SampleEvent>(new PublisherOptions(){Endpoint= "http://localhost:5000",
                ProxyEnabled = true,
                Proxy = new System.Net.WebProxy("localhost", 8888),
            },serializer))
            {
                Console.Write("Enter a max wait time in ms between message:");
                var time = int.Parse(Console.ReadLine());
                while (processInput)
                {
                    await p.PublishAsync(new SampleEvent { Data = RandomString() });
                    await Task.Delay(random.Next(0,time) % time);
                }
            }
        }

        public static string RandomString()
        {
            var index = random.Next(0, strings.Count - 1);
            return strings[index];
        }

        public static void LoadSampleString()
        {
            using(var fin = File.OpenRead("SampleMessages.txt"))
            using(var sr = new StreamReader(fin))
            {
                string line;
                while(true)
                {
                    line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)){ break; }
                        
                    strings.Add(line);
                }
                sr.Close();
            }
        }
    }
}
