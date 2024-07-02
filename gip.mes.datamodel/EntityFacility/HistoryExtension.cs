using System;
using System.Linq;

namespace gip.mes.datamodel
{
    public static class HistoryExtension
    {
        public static void DefineStartEndHistoryTime(DatabaseApp dbApp, gip.mes.datamodel.GlobalApp.TimePeriods defaultTimePeriod, History history, ref DateTime startTime, ref DateTime endTime)
        {
            startTime = DateTime.Now;
            endTime = DateTime.Now.Date.AddDays(1);

            if (history != null)
            {
                History endHistory = history;
                History startHistory = dbApp.History.Where(x => 
                    x.TimePeriodIndex == history.TimePeriodIndex 
                    && x.BalanceDate < history.BalanceDate
                    && x.HistoryID != history.HistoryID
                    ).OrderByDescending(x => x.BalanceDate).FirstOrDefault();
                endTime = endHistory.BalanceDate;
                if (startHistory != null)
                {
                    startTime = startHistory.BalanceDate;
                }
                else
                {
                    startTime = DefineStartDateViaPeriod(history.TimePeriod, endTime);
                }
            }
            else
            {
                startTime = DefineStartDateViaPeriod(defaultTimePeriod, endTime);
            }
        }

        /// <summary>
        /// Define a start date when history record before current is not present
        /// </summary>
        /// <param name="periodFilter"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private static DateTime DefineStartDateViaPeriod(GlobalApp.TimePeriods periodFilter, DateTime endTime)
        {
            DateTime startTime = DateTime.Now;
            switch (periodFilter)
            {
                case GlobalApp.TimePeriods.Day:
                    startTime = endTime.AddDays(-1);
                    break;
                case GlobalApp.TimePeriods.Month:
                    startTime = new DateTime(endTime.Year, endTime.Month, 1);
                    break;
                case GlobalApp.TimePeriods.Week:
                    DateTime input = endTime.Date;
                    int delta = DayOfWeek.Monday - input.DayOfWeek;
                    startTime = input.AddDays(delta);
                    break;
                case GlobalApp.TimePeriods.Year:
                    startTime = new DateTime(endTime.Year, 1, 1);
                    break;
            }
            return startTime;
        }
    }
}
