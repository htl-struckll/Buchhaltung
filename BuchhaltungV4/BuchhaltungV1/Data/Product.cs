using System;

namespace BuchhaltungV4
{
    public class Product : IComparable<Product>, IDisposable
    {
        #region Vars

        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; } //todo Abfgragen

        public int Tax { get; set; }

        public double Amount { get; set; }

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