using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuchhaltungV1
{
    public class Entry : IComparable<Entry>
    {
        
        #region Vars
        private Product _product;
        public Product ProductForEntry
        {
            get { return _product; }
            set { _product = value; }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        private int _amountOnTheHouse;

        public int AmountOnTheHouse
        {
            get { return _amountOnTheHouse; }
            set { _amountOnTheHouse = value; }
        }

        private double _price;

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        #endregion

        public Entry(Product p, int amount, int amountOnTheHouse, double price)
        {
            this.ProductForEntry = p;
            this.Amount = amount;
            this.AmountOnTheHouse = amountOnTheHouse;
            this.Price = price;
        }

        public int CompareTo(Entry other)
        {
            return this.ProductForEntry.Name.Length - other.ProductForEntry.Name.Length;
        }

        public override string ToString()
        {
            return $"Id: {ProductForEntry.ID} | Name: {ProductForEntry.Name} | Amount:  {Amount} | AmountOnTheHouse  {AmountOnTheHouse} | Price:  {Price}";
        }
    }

}
