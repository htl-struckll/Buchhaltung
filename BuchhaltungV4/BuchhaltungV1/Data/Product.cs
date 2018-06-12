using System;

namespace BuchhaltungV4
{
    public class Product : IComparable<Product>, IDisposable
    {
        #region Vars

        public int Id { get; set; }

        public string Name { get; set; }

        private double _price;
        public double Price
        {
            get => _price;
            set { if(value>0 && value < int.MaxValue) _price = value; }
        }

        private int _tax;
        public int Tax
        {
            get => _tax;
            set { if(value > 0 && value < 100) _tax = value; }
        }

        private double _amount;

        public double Amount
        {
            get => _amount;
            set { if(value > 0 && value < int.MaxValue) _amount = value; }
        }

        public string KindOfAmount { get; set; }

        public string Group { get; set; }

        #endregion

        public Product(string name, double price, int tax, double amount, string kindOfAmount, int id, string group)
        {
            Name = name;
            Price = price;
            Tax = tax;
            Amount = amount;
            KindOfAmount = kindOfAmount;
            Id = id + 1;
            Group = group;
        }

        #region functions

        /// <summary>
        /// Compares size of ids
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Product other) => Id - other.Id;

        public void Dispose()
        {
        }
        #endregion
    }
}