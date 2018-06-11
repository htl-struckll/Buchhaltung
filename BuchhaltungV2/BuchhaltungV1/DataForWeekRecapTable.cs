namespace BuchhaltungV1
{
    class DataForWeekRecapTable
    {
        #region Vars
        public int Amount { get; set; }

        public string Name { get; set; }

        public double AmountOfKindOfProduct { get; set; }

        public string KindOfAmount { get; set; }
        #endregion

        public DataForWeekRecapTable(int amount, string name, double amountOfKindOfProduct , string kindOfAmount)
        {
            Amount = amount;
            Name = name;
            AmountOfKindOfProduct = amountOfKindOfProduct;
            KindOfAmount = kindOfAmount;
        }
    }
}
