using System;
using System.Collections.Generic;
using System.Text;

namespace SES.Client
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    internal class Stopwatch
    {
        private DateTimeOffset? startTime;
        private DateTimeOffset? endTime;
        private readonly bool startOnCreate;
        private readonly Dictionary<string, TimeSpan> splits;
        public Stopwatch(bool startOnCreate=true)
        {
            this.startOnCreate = startOnCreate;
            if(this.startOnCreate)
            {
                startTime = DateTimeOffset.Now;
            }
            splits = new Dictionary<string, TimeSpan>();
        }

        public void Start(bool restartIfRunning=false)
        {
            if(restartIfRunning)
            {
                this.startTime = DateTimeOffset.Now;
            }
            else
            {
                if(this.startTime==null)
                {
                    this.startTime = DateTimeOffset.Now;
                }
                else
                {
                    throw new InvalidOperationException($"This instance of {nameof(Stopwatch)} was created in a started state and can only be reset if {nameof(restartIfRunning)} is true when {nameof(Start)} is called.");
                }
            }
        }

        public void Split(string splitName)
        {
            splits.Add(splitName, DateTimeOffset.Now - (startTime ?? DateTimeOffset.Now));
        }
        public void Split()
        {
            Split((splits.Count + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
        }
        public TimeSpan Stop()
        {
            endTime = DateTimeOffset.Now;
            return ElapsedTime;
        }


        public TimeSpan ElapsedTime
        {
            get
            {
                if(endTime==null)
                {
                    throw new InvalidOperationException($"This instance of {nameof(Stopwatch)} has not been stopped.");
                }
                if(startTime==null)
                {
                    return TimeSpan.Zero;
                }
                return endTime.Value - startTime.Value;
            }
        }

        public TimeSpan SplitTime(int split) => SplitTime(split.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
        public TimeSpan SplitTime(string splitName)
        {
            TimeSpan result = TimeSpan.Zero;
            splits.TryGetValue(splitName, out result);
            return result;
        }

    }
}
