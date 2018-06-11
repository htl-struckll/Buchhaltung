namespace BuchhaltungV4
{
    class DataForWeekRecapTable
    {
        #region Vars

        public int Amount { get; set; }

        public string Group { get; set; }

        public double AmountKind { get; set; }

        public string KindOfAmount { get; set; }

        #endregion

        public DataForWeekRecapTable(int amount, string group, double amountOfKindOfProduct, string kindOfAmount)
        {
            Amount = amount;
            Group = group;
            AmountKind = amountOfKindOfProduct;
            KindOfAmount = kindOfAmount;
        }
    }
}