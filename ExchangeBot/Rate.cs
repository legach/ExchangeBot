using System;
using System.Collections.Generic;

namespace ExchangeBot;

public class Rate
{
    public string Name { get; set; }
    public Dictionary<Currency,double> Sales { get; set; }
    public Dictionary<Currency,double> Buys { get; set; }
    public DateTime Timestamp { get; set; }
}