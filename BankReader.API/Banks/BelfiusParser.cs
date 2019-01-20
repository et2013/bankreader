using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BankReader.API.Services;

namespace BankReader.API.Banks
{
    public class BelfiusParser : IParser
    {
        public bool CanParse(IList<string> lines)
        {
            var containsStart = lines[0].ToLower().StartsWith("boekingsdatum vanaf");
            var containsEnd = lines[1].ToLower().StartsWith("boekingsdatum tot en met");
            return containsStart && containsEnd;
        }

        public IEnumerable<BankTransfer> Parse(IList<string> lines)
        {
            var enumerable = lines as string[] ?? lines.ToArray();
            if (!enumerable.First().StartsWith("Boekingsdatum vanaf"))
            {
                return null;
            }

            return enumerable.Select(x => x.Split(';'))
                .Skip(14) // First 14 lines are just bullshit
                .Select(x => new Belfius()
                {
                    // Let's parse the file into workable objects
                    Bankname = "Belfius",
                    Date = DateTime.ParseExact(x[1], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Value = decimal.Parse(x[10], CultureInfo.GetCultureInfo("nl-be")),
                    Description = string.Concat(x[5], " ", x[8]),
                    Category = string.Concat(x[8], x[5]).ParseCategory()
                });
        }
    }
}