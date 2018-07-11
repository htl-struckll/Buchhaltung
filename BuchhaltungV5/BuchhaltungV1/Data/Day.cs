using System;
using System.Collections.Generic;
using BuchhaltungV1.Enumerations;

namespace BuchhaltungV4
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

        #region fuctions

        /// <summary>
        /// Adds new Entry to this day
        /// </summary>
        /// <param name="e"></param>
        public void AddEntry(Entry e) => Entrys.Add(e);

        /// <summary>
        /// Adds money to bank
        /// </summary>
        /// <param name="money"></param>
        public void AddBank(double money) => ToBank += money;

        /// <summary>
        /// Sets BankAmount
        /// </summary>
        /// <param name="money"></param>
        public void SetBankAmount(double money) => ToBank = money;


        /// <summary>
        /// pffff
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Day other) => 0;

        #endregion
    }
}