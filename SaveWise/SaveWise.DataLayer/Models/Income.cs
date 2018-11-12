namespace SaveWise.DataLayer.Models
{
    public class Income
    {
        public IncomeCategory Category { get; set; }

        public float Amount { get; set; }

        public string Comment { get; set; }
    }
}