using System.Collections.Generic;

namespace ExchangeBot.Companies;

public interface IExchangeCompany
{
    string Name { get; }
    string PublicSite { get; }
    List<string> Addresses { get; }
}