using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Core
{
    public static class Utils
    {
        public static List<DateTime> GetDaysInWeek(this DateTime Date, DayOfWeek firstdayofweek)
        {
            List<DateTime> d = new List<DateTime>();

            int days = Date.DayOfWeek - firstdayofweek;
            DateTime dt = Date.AddDays(-days);
            d.Add(dt);
            d.AddRange(new DateTime[] { dt.AddDays(1), dt.AddDays(2), dt.AddDays(3), dt.AddDays(4), dt.AddDays(5), dt.AddDays(6) });

            return d;
        }
    }
}
