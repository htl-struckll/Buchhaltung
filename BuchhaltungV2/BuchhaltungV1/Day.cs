using System;
using System.Collections.Generic;

namespace BuchhaltungV1
{
    public class Day : IComparable<Day>
    {
        #region Vars

        public List<Entry> Entrys { get; set; }

        public DayOfTheWeek Name { get; set; }

        public double ToBank { get; set; }

        #endregion

        public Day(DayOfTheWeek name)
        {
            Name = name;
            Entrys = new List<Entry>();
            ToBank = 0;
        }

        //adds an entry to this day
        public void AddEntry(Entry e)
        {
            Entrys.Add(e);
        }

        //adds to Bank
        public void AddBank(double money)
        {
            ToBank += money;
        }

        //Sets bank
        public void SetBankAmount(double money)
        {
            ToBank = money;
        }

        //pff
        public int CompareTo(Day other)
        {
            return 0;
        }
    }

}
