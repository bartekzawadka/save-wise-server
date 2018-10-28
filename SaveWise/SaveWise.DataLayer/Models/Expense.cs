using System;

namespace SaveWise.DataLayer.Models
{
    public class Expense
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public float Amount { get; set; }

        public string Category { get; set; }

        public string Type { get; set; }

        public string Comment { get; set; }
    }
}