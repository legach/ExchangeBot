using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeBot.Companies;

namespace ExchangeBot
{
    public class DataRetriever
    {
        private List<Rate> _rates;
        private readonly List<IExchangeCompanyRetriever> _companyRetrievers;

        public DataRetriever()
        {
            _rates = new List<Rate>();
            _companyRetrievers = new List<IExchangeCompanyRetriever>()
            {
                new AltaPay(),
                new Panter()
            };
        }

        public List<Rate> GetAllRates()
        {
            //if (!_rates.Any())
            //{
                _rates = FillRates();
            //}

            return _rates;
        }

        private List<Rate> FillRates()
        {
            var list = new List<Rate>();
            var taskList = new List<Task>();
            foreach (var exchangeCompanyRetriever in _companyRetrievers)
            {
                var task = exchangeCompanyRetriever.GetRate().ContinueWith(r =>
                {
                    if (r.IsCompletedSuccessfully)
                    {
                        list.Add(r.Result);
                    }
                });
                taskList.Add(task);
            }

            Task.WaitAll(taskList.ToArray());
            return list;
        }

        public Rate GetMaximumBuysRate(Currency currency)
        {
            if (!_rates.Any())
            {
                _rates = FillRates();
            }
            var rate = _rates.MaxBy(r => r.Buys[currency]);
            return rate;
        }

        public Rate GetMinimumSalesRate(Currency currency)
        {
            if (!_rates.Any())
            {
                _rates = FillRates();
            }
            var rate = _rates.MinBy(r => r.Sales[currency]);
            return rate;
        }
    }
}
