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
        private static readonly Random random = new Random();
        private static readonly List<string> strings = new List<string>();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1801 // Remove unused parameter
        static async Task Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1801 // Remove unused parameter
        {
            Console.CancelKeyPress+=(s,o)=>processInput=false;
            LoadSampleString();
            IAsyncEventSerializer serializer = new SES.Serialization.Json.JsonSerializer();
            using(var p = new Publisher<SampleEvent>(new PublisherOptions(){Endpoint= "http://localhost:5000",
                ProxyEnabled = false
            },serializer))
            {
                Console.Write("Enter a max wait time in ms between message:");
                var time = int.Parse(Console.ReadLine(),System.Globalization.NumberStyles.Integer,System.Globalization.NumberFormatInfo.InvariantInfo);
                while (processInput)
                {
                    await p.PublishAsync(new SampleEvent { Data = RandomString() }).ConfigureAwait(false);
                    await Task.Delay(random.Next(0, time) % time).ConfigureAwait(false);
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
            using(var fin = File.OpenRead(".\\SampleMessages.txt"))
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
