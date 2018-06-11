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

        #region function´s

        /// <summary>
        /// gets the tax of the product
        /// </summary>
        /// <returns></returns>
        private double GetTax()
        {
            foreach (Product p in Buchhaltung.Products)
            {
                if (p.Id == ProductForEntry.Id)
                    return p.Tax;
            }

            return -1; //error
        }

        /// <summary>
        /// Comoares length
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Entry other) => ProductForEntry.Name.Length - other.ProductForEntry.Name.Length;

        /// <summary>
        /// DBG OUTPUT
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"Id: {ProductForEntry.Id} | Name: {ProductForEntry.Name} | AmountOfKindOfProduct:  {Amount} | AmountOnTheHouse  {AmountOnTheHouse} | Price:  {Price}";

        #endregion
    }
}