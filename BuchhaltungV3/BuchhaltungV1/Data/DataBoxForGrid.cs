namespace BuchhaltungV1
{
    public class DataBoxForGrid
    {
        #region Vars

        public int Id { get; set; }

        public string Name { get; set; }

        public int Amount { get; set; }

        public int OnTheHouse { get; set; }

        public double Price { get; set; }

        public double All { get; set; }

        public int C { get; set; }

        public DayOfTheWeek Dtw { get; set; }

        public double FinalAmount { get; set; }

        public string KindOfAmount { get; set; }

        #endregion

        public DataBoxForGrid(int id, string product, int amount, int amountOnTheHouse, double price, double all, int c,
            DayOfTheWeek d, double kindOfAmountInReal, string kindOfAmount)
        {
            Id = id;
            Name = product;
            Amount = amount;
            OnTheHouse = amountOnTheHouse;
            Price = price;
            All = all;
            C = c;
            Dtw = d;
            FinalAmount = amount * kindOfAmountInReal;
            KindOfAmount = kindOfAmount;
        }
    }
}