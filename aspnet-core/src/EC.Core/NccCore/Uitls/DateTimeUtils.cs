using Abp.Timing;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NccCore.Uitls
{
    public class DateTimeUtils
    {
        // All now function use Clock.Provider.Now
        public static DateTime GetNow()
        {
            return Clock.Provider.Now;
        }

        public static string ToString(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        public static DateTime FirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static List<DateTime> GetListMonthForDebtPaymentPlan(DateTime startDate, DateTime endDate)
        {
            var date = FirstDayOfMonth(startDate);
            var result = new List<DateTime>();
            while (date < endDate)
            {
                result.Add(date.AddDays(4));
                date = date.AddMonths(1);
            }
            return result;
        }

        public static string ToMMYYYY(DateTime date)
        {
            return date.ToString("MM-yyyy");
        }

        public static List<DateTime> GetListMonthsForCreate()
        {
            var listDate = new List<DateTime>();
            var now = DateTimeUtils.GetNow();
            listDate.Add(now.AddMonths(0));
            for (int m = 1; m <= 2; m++)
            {
                var monthInPast = now.AddMonths(-m);
                var monthInFuture = now.AddMonths(m);
                listDate.Add(monthInPast);
                listDate.Add(monthInFuture);
            }
            return listDate.OrderByDescending(x => x).ToList();
        }
        public static Period CalRangeBetweenDate(DateTime fromDate)
        {
            var now = GetNow();
            LocalDate endDate = new LocalDate(now.Year, now.Month, now.Day);
            LocalDate startDate = new LocalDate(fromDate.Year, fromDate.Month, fromDate.Day);
            Period period = Period.Between(startDate, endDate);
            return period;
        }

        public static DateTime GetFirstDayOfMonth(DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, 1).Date; 
        }

        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return GetFirstDayOfMonth(date).AddMonths(1).AddDays(-1).Date;
        }

        public static int CountMonthBetweenTwoDate(DateTime fromDate, DateTime toDate)
        {
            int monthsApart = 12 * (fromDate.Year - toDate.Year) + fromDate.Month - toDate.Month;
            return Math.Abs(monthsApart);
        }

        public static bool IsTheSameYearMonth(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month;
        }

        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return date1 < date2 ? date1 : date2;
        }

        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return date1 < date2 ? date2 : date1;
        }

        public static int CaculateStandardWorkingDay(DateTime startDate, DateTime endDate, HashSet<DateTime> dateOffSettings)
        {
            if (endDate.Date < startDate)
            {
                return 0;
            }
            int result = 0;
            for(var date = startDate.Date; date <= endDate; date = date.AddDays(1))
            {

                if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                {
                    continue;
                }

                if (dateOffSettings != null && dateOffSettings.Contains(date))
                {
                    continue;
                }

                result++;
            }
            return result;

        }

        public static List<DateTime> GetListDateInMonth(int year, int month, List<DateTime> exceptDates = null)
        {
            List<DateTime> results = new List<DateTime>();
            var firstDate = new DateTime(year, month, 1);
            var firstDateNextMonth = firstDate.AddMonths(1);
            for (var date = firstDate; date < firstDateNextMonth; date = date.AddDays(1))
            {
                if (exceptDates == null || !exceptDates.Contains(date))
                {
                    results.Add(date);
                }                
            }

            return results;
        }


    }
}
