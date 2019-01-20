using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace BankReader.API.Services
{
    public static class BankParser
    {
        public static List<RegexCategory> Expressions { get; set; }

        public static List<BankTransfer> Parse(string filename)
        {
            // Read the first 10 lines
            var lines = File.ReadLines(filename).ToList();
            var parser = ServiceLocator
                .Current
                .GetAllInstances<IParser>().FirstOrDefault(x => x.CanParse(lines));

            if (parser == null)
                throw new Exception("No parser found");

            return parser.Parse(lines).ToList();
        }
    }
}