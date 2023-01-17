using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeBot
{
    public class DataRetriever
    {
        private List<Rate> rates;

        public List<Rate> GetAllRates()
        {
            if (!rates.Any())
            {
                rates = FillRates();
            }

            return rates;
        }

        private List<Rate> FillRates()
        {
            throw new NotImplementedException();
        }

        public Rate GetMaximumBuysRate(Currency currency)
        {
            var rate = rates.MaxBy(r => r.Buys[currency]);
            return rate;
        }

        public Rate GetMinimumSalesRate(Currency currency)
        {
            var rate = rates.MinBy(r => r.Sales[currency]);
            return rate;
        }
    }
}
