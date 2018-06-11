using System;

namespace BuchhaltungV1
{
    public class Product : IComparable<Product>,IDisposable
    {
        #region Vars

        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public int Tax { get; set; }

        public double Amount { get; set; }

        public string KindOfAmount { get; set; }

        #endregion

        public Product(string name, double price, int tax, double amount, string kindOfAmount, int id)
        {
            Name = name;
            Price = price;
            Tax = tax;
            Amount = amount;
            KindOfAmount = kindOfAmount;
            Id = id + 1;
        }

        public int CompareTo(Product other)
        {
            return Id - other.Id;
        }

        public void Dispose()
        {
            
        }
    }

}
