using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExchangeBot.Companies
{
    public class AltaPay: IExchangeCompany, IExchangeCompanyRetriever
    {
        public string Name => "AltaPay";
        public string PublicSite => "https://www.altapay.rs/en";
        public List<string> Addresses { get; }


        public string RatePage => "https://www.altapay.rs/json/lista.json";//https://www.altapay.rs/json/lista.json?_=1673992871377
        public async Task<Rate> GetRate()
        {
            var rate = new Rate();
            rate.Company = this;
            rate.Buys = new Dictionary<Currency, double>();
            rate.Sales = new Dictionary<Currency, double>();
            rate.Timestamp = DateTime.Now;

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(RatePage);
                if (response.IsSuccessStatusCode)
                {
                    var contentResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic content = JsonConvert.DeserializeObject(contentResponse);
                        var euroSales = content[0]["prodajni"].ToString();
                        rate.Sales.Add(Currency.Euro, Double.Parse(euroSales));

                        var euroBuy = content[0]["kupovni"].ToString();
                        rate.Buys.Add(Currency.Euro, Double.Parse(euroBuy));
                    }
                    catch (JsonException e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return rate;
        }
    }
}
