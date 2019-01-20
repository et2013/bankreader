using System;
using System.ComponentModel;

namespace BankReader.API.Services
{
    public abstract class BankTransfer
    {
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public decimal Value { get; set; }

        [DisplayName(@"Bank name")] public string Bankname { get; set; }

        public string Description { get; set; }
    }
}