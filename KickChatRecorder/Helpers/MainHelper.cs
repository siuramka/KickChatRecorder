using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickChatRecorder.Helpers
{
    public static class MainHelper
    {
        /// <summary>
        /// Gets a TimeSpan till the nearest future hour
        /// </summary>
        /// <param name="hour"></param>
        public static TimeSpan GetTillFutureHour(int hour)
        {
            var time = DateTime.Today.AddHours(hour);
            if (DateTime.Now.Hour > hour)
            {
                time.AddDays(1);
                return DateTime.Now - time;
            }
            return time - DateTime.Now;
        }
    }
}
