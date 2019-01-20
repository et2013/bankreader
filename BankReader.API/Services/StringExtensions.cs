using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NReco.Linq;

namespace BankReader.API.Services
{
    public static class StringExtensions
    {
        public static string ParseCategory(this string description)
        {
            var categories = ReadCategories();
            var result = categories.Select(x =>
            {
                if (!string.IsNullOrEmpty(x.StringExpression))
                {
                    var lambdaParser = new LambdaParser();
                    var varContext = new Dictionary<string, object> { ["description"] = description.ToLower() };
                    var isMatch = lambdaParser.Eval(x.StringExpression, varContext) as bool?;
                    if (isMatch.HasValue && isMatch.Value) return x.Category;
                }

                return string.Empty;
            }).FirstOrDefault(category => !string.IsNullOrEmpty(category));
            return result;
        }

        private static IEnumerable<string> GetFiles()
        {
            var path = GetFolder();
            var files = Directory.GetFiles(path, "*.json");
            return files;
        }

        private static IEnumerable<RegexCategory> ReadCategories()
        {
            var path = GetFolder();
            var result = GetFiles().SelectMany(file =>
           {
               var filePath = Path.Combine(path, file);
               var categories = JsonConvert.DeserializeObject<IEnumerable<RegexCategory>>(File.ReadAllText(filePath));
               return categories;
           });
            return result;
        }

        private static string GetFolder()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "categories");
            return path;
        }
    }
}