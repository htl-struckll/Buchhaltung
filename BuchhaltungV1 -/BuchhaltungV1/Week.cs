using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BuchhaltungV1
{
    public class Week : IComparable<Week>
    {
        #region Vars
        private List<Day> _days;
        public List<Day> DaysInWeek
        {
            get { return _days; }
            set { _days = value; }
        }

        private string _weekNr;
        public string WeekNr
        {
            get { return _weekNr; }
            set { _weekNr = value; }
        }
        #endregion

        public Week(string wkNr)
        {
            this.WeekNr = wkNr;
            DaysInWeek = new List<Day>();
        }

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

        //For a new Entry to add it to the correct Day
        public void GetCurrentDayAndAddEntry(Entry e)
        {
            try
            {
                foreach (Day d in DaysInWeek)
                {
                    if (d.Name == Buchhaltung.currDay)
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

        //Displays all Entrys in this week (DBG)
        public void ShowDays()
        {
            try
            {
                foreach (Day d in DaysInWeek)
                {
                    MessageBox.Show(d.Name.ToString());
                    for (int i = 0; i < d.Entrys.Count; i++)
                    {
                        MessageBox.Show("Name: " + d.Entrys[i].ProductForEntry.Name + "; Menge: " + d.Entrys[i].Amount + "; Preis: " + d.Entrys[i].Price);
                    }
                }
            }
            catch (Exception e)
            {
                Buchhaltung.SaveErrorMsg(e);
            }
        }

        //This is a very nice CompareTo Let it be
        public int CompareTo(Week other)
        {
            return 0;
        }

        public override string ToString()
        {
            string days = null;
            days += "WeekNr: " + WeekNr + "\n";
            DaysInWeek.ForEach((day) => { days += $"\n[{day.Name}]\n"; day.Entrys.ForEach((entry) => { days += $"Name: {entry.ProductForEntry.Name}|Amout: {entry.Amount}|AmountOnTheHouse {entry.AmountOnTheHouse}|TotalPrice {entry.Price}\n"; }); });
            return days;
        }

        public Day GetCurrentDay()
        {
            foreach (Day d in DaysInWeek)
            {
                if (Buchhaltung.currDay == d.Name)
                    return d;
            }
            return null;
        }
    }
}
