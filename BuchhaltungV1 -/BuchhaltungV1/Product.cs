using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuchhaltungV1
{
    public class Product : IComparable<Product>
    {
        #region Vars
        private int _id;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private double _price;

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        private int _tax;

        public int Tax
        {
            get { return _tax; }
            set { _tax = value; }
        }

        private double _amount;

        public double Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        private string _kindOfAmount;

        public string KindOfAmount
        {
            get { return _kindOfAmount; }
            set { _kindOfAmount = value; }
        }

        

        #endregion

        public Product(string name, double price, int tax, double amount, string kindOfAmount, int id)
        {
            this.Name = name;
            this.Price = price;
            this.Tax = tax;
            this.Amount = amount;
            this.KindOfAmount = kindOfAmount;
            this.ID = id + 1;
        }

        public int CompareTo(Product other)
        {
            return this.ID - other.ID;
        }

    }

}
