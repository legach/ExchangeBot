using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeBot.Companies;

namespace ExchangeBot;

public class Rate
{
    public Rate(IExchangeCompany company)
    {
        Company = company;
        Sales = new Dictionary<Currency, double>();
        Buys = new Dictionary<Currency, double>();
        Timestamp = DateTime.Now;
    }
    public IExchangeCompany Company { get; set; }
    public Dictionary<Currency,double> Sales { get; set; }
    public Dictionary<Currency,double> Buys { get; set; }
    public DateTime Timestamp { get; set; }

    public override string ToString()
    {
        var company = $"[{Company.Name}]({Company.PublicSite})";
        var rates = string.Empty;
        foreach (var salesKey in Sales.Keys)
        {
            var buyRate = Buys.ContainsKey(salesKey) ? Buys[salesKey] : 0 ;
            rates += $"{salesKey}: {buyRate} / {Sales[salesKey]}\n";
        }
        return $"{company}\n{rates}".Replace(".", "\\."); ;
    }

    public string ToString(Currency currency)
    {
        var company = $"[{Company.Name}]({Company.PublicSite})";
        var rates = string.Empty;
        var buyRate = Buys.ContainsKey(currency) ? Buys[currency] : 0;
        var saleRate = Sales.ContainsKey(currency) ? Sales[currency] : 0;
        rates += $"{currency}: {buyRate} / {saleRate}\n";
        return $"{company}\n{rates}".Replace(".","\\.");
    }

}