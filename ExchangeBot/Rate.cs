using System;
using System.Collections.Generic;
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
}