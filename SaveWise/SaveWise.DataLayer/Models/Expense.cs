using System;

namespace SaveWise.DataLayer.Models
{
    public class Expense
    {
        public DateTime Date { get; set; }

        public float Amount { get; set; }

        public string Category { get; set; }

        public string Type { get; set; }

        public string Comment { get; set; }
    }
}