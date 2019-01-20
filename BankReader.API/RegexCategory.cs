using System;
using System.Linq.Expressions;

namespace BankReader.API
{
    public class RegexCategory
    {
        public string Category { get; set; }
        public Expression<Func<string, bool>> Parser { get; set; }
        public string StringExpression { get; set; }
    }
}