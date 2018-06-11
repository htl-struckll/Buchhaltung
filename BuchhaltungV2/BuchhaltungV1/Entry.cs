using System;

namespace BuchhaltungV1
{
    public class Entry : IComparable<Entry>
    {
        
        #region Vars

        public Product ProductForEntry { get; set; }

        public int Amount { get; set; }

        public int AmountOnTheHouse { get; set; }

        public double Price { get; set; }

        public double Tax { get; set; }

        #endregion

        public Entry(Product p, int amount, int amountOnTheHouse, double price)
        {
            ProductForEntry = p;
            Amount = amount;
            AmountOnTheHouse = amountOnTheHouse;
            Price = price;
            Tax = GetTax();
        }

        private double GetTax()
        {
            foreach (Product p in Buchhaltung.Products)
            {
                if (p.Id == ProductForEntry.Id)
                    return p.Tax;
            }

            return 0; //error
        }

        public int CompareTo(Entry other)
        {
            return ProductForEntry.Name.Length - other.ProductForEntry.Name.Length;
        }

        public override string ToString()
        {
            return $"Id: {ProductForEntry.Id} | Name: {ProductForEntry.Name} | AmountOfKindOfProduct:  {Amount} | AmountOnTheHouse  {AmountOnTheHouse} | Price:  {Price}";
        }
    }

}
