using System;
using BuchhaltungV1.Enumerations;

namespace BuchhaltungV4
{
    public class Entry : IComparable<Entry>
    {
        #region Vars

        public int Id { get; set; }

        public int WeekId { get; set; }

        public DayOfTheWeek DayOfEntry { get; set; }

        public Product ProductForEntry { get; set; }

        public int Amount { get; set; }

        public int AmountOnTheHouse { get; set; }

        public double Price { get; set; }

        public double Tax { get; set; }

        #endregion
        //test

        public Entry(int id, int weekId, int dayId, int productId, int amount, int amountOnTheHouse,double price)
        {
            Id = id;
            WeekId = weekId;
            DayOfEntry = (DayOfTheWeek) dayId; //todo check if this works
            ProductForEntry = GetProduct(productId);
            Amount = amount;
            AmountOnTheHouse = amountOnTheHouse;
            Price = price;
            Tax = ProductForEntry.Tax;
        }

        #region function´s

        /// <summary>
        /// gets the tax of the product
        /// </summary>
        /// <returns></returns>
        private Product GetProduct(int pId)
        {
            foreach (Product p in Buchhaltung.Products)
            {
                if (p.Id == pId)
                    return p;
            }

            return null; //error
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