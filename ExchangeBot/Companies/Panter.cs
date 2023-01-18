using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExchangeBot.Companies
{
    public class Panter: IExchangeCompany, IExchangeCompanyRetriever
    {
        public string Name => "Panter";
        public string PublicSite => "https://www.menjacnica-panter.co.rs/";
        public List<string> Addresses { get; }
        public string RatePage => "https://www.menjacnica-panter.co.rs/index.php#exchange";
        public async Task<Rate> GetRate()
        {
            var rate = new Rate(this);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                var response = await client.GetAsync(RatePage);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                   
                    var regexpEuro =
                        new Regex(
                            "Kupovni:(?<buy>[\\s\\d,]+)<br>\\s*Srednji:(?<avg>[\\s\\d,]+)<br>\\s*Prodajni:(?<sell>[\\s\\d,]+)<br>\\s*Naziv: Evro");
                    FindRate(regexpEuro, content, Currency.Euro, rate);

                    var regexpUsd =
                        new Regex(
                            "Kupovni:(?<buy>[\\s\\d,]+)<br>\\s*Srednji:(?<avg>[\\s\\d,]+)<br>\\s*Prodajni:(?<sell>[\\s\\d,]+)<br>\\s*Naziv: Američki Dolar");
                    FindRate(regexpUsd, content, Currency.Usd, rate);
                }
            }

            return rate;
        }

        private void FindRate(Regex regexp, string content, Currency currency, Rate rate)
        {
            try
            {
                var match = regexp.Match(content);
                if (match.Success)
                {
                    var buyString = match.Groups["buy"].Value;
                    if (!string.IsNullOrWhiteSpace(buyString))
                    {
                        rate.Buys.Add(currency, Double.Parse(buyString.Trim()));
                    }

                    var sellString = match.Groups["sell"].Value;
                    if (!string.IsNullOrWhiteSpace(sellString))
                    {
                        rate.Sales.Add(currency, Double.Parse(sellString.Trim()));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
