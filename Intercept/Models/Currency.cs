using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intercept.Models
{
    public class Currency
    {
        public string CurrToConvert { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public double NewAmount { get; set; }
    }
}