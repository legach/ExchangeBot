using System.Threading.Tasks;

namespace ExchangeBot.Companies
{
    public interface IExchangeCompanyRetriever
    {
        string RatePage { get; }
        Task<Rate> GetRate();
    }
}
