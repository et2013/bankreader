using System.Collections.Generic;

namespace BankReader.API.Services
{
    public interface IParser
    {
        bool CanParse(IList<string> lines);
        IEnumerable<BankTransfer> Parse(IList<string> lines);
    }
}