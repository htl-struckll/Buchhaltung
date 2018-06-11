using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BuchhaltungV1
{
    public class Day : IComparable<Day>
    {
        #region Vars
        private List<Entry> _entrys;
        public List<Entry> Entrys
        {
            get { return _entrys; }
            set { _entrys = value; }
        }

        private DayOfTheWeek _name;

        public DayOfTheWeek Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        public Day(DayOfTheWeek name)
        {
            Name = name;
            Entrys = new List<Entry>();
        }

        //adds an entry to this day
        public void AddEntry(Entry e)
        {
            Entrys.Add(e);
        }

        //pff
        public int CompareTo(Day other)
        {
            return 0;
        }

        /*public Entry GetEntrys()
        {
            foreach (Entry e in Entrys)
            {

            }
        }*/
    }

}
