using System;
using System.Collections.Generic;
using System.Windows;

namespace BuchhaltungV1
{
    public class Week : IComparable<Week>
    {
        #region Vars

        public List<Day> DaysInWeek { get; set; }

        public string WeekNr { get; set; }

        public DateTime ExactTime { get; set; }

        public string EditName { get; set; }


        public double LastChashDesk { get; set; }

        #endregion

        #region ctors

        public Week(string wkNr, DateTime date, string editName, double lastCashDesk)
        {
            ExactTime = date;
            WeekNr = wkNr;
            DaysInWeek = new List<Day>();
            EditName = editName;
            LastChashDesk = lastCashDesk;
        }

        public Week(string wkNr)
        {
            WeekNr = wkNr;
            DaysInWeek = new List<Day>();
        }

        #endregion

        #region functions

        /// <summary>
        /// Adds info to week
        /// </summary>
        /// <param name="date"></param>
        /// <param name="editName"></param>
        /// <param name="lastCashDesk"></param>
        public void AddInfo(DateTime date, string editName, double lastCashDesk)
        {
            ExactTime = date;
            DaysInWeek = new List<Day>();
            EditName = editName;
            LastChashDesk = lastCashDesk;
        }

        /// <summary>
        /// adds day with day name
        /// </summary>
        /// <param name="dayToAdd"></param>
        /// <returns></returns>
        public bool AddDay(Day dayToAdd)
        {
            bool success = true;
            foreach (Day d in DaysInWeek)
            {
                if (d.Name == dayToAdd.Name)
                {
                    success = false;
                    break;
                }
            }

            if (success)
                DaysInWeek.Add(dayToAdd);
            return success;
        }

        /// <summary>
        /// For a new Entry to add it to the correct Day
        /// </summary>
        /// <param name="e"></param>
        public void GetCurrentDayAndAddEntry(Entry e)
        {
            try
            {
                foreach (Day d in DaysInWeek)
                {
                    if (d.Name == Buchhaltung.CurrDay)
                    {
                        d.AddEntry(e);
                        break;
                    }
                }
            }
            catch (Exception x)
            {
                Buchhaltung.SaveErrorMsg(x);
            }
        }

        /// <summary>
        /// Displays all Entrys in this week (DBG)
        /// </summary>
        public void ShowDays()
        {
            try
            {
                foreach (Day d in DaysInWeek)
                {
                    MessageBox.Show(d.Name.ToString());
                    foreach (Entry t in d.Entrys)
                    {
                        MessageBox.Show("Name: " + t.ProductForEntry.Name + "; Menge: " + t.Amount + "; Preis: " +
                                        t.Price);
                    }
                }
            }
            catch (Exception e)
            {
                Buchhaltung.SaveErrorMsg(e);
            }
        }

        /// <summary>
        /// This is a very nice CompareTo Let it be
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        int IComparable<Week>.CompareTo(Week other) => 0;

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string days = null;
            days += "WeekNr: " + WeekNr + "\n";
            DaysInWeek.ForEach((day) =>
            {
                days += $"\n[{day.Name}]\n";
                day.Entrys.ForEach((entry) =>
                {
                    days +=
                        $"Name: {entry.ProductForEntry.Name}|Amout: {entry.Amount}|AmountOnTheHouse {entry.AmountOnTheHouse}|TotalPrice {entry.Price}\n";
                });
            });
            return days;
        }

        /// <summary>
        /// gets current day
        /// </summary>
        /// <returns></returns>
        public Day GetCurrentDay()
        {
            foreach (Day d in DaysInWeek)
            {
                if (Buchhaltung.CurrDay == d.Name)
                    return d;
            }

            return null;
        }

        /// <summary>
        /// Clears all entrys of this week
        /// </summary>
        public void ClearAllDaysEntrys()
        {
            foreach (Day d in DaysInWeek)
            {
                d.Entrys.Clear();
            }
        }

        #endregion
    }
}