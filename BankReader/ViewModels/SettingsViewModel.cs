using System.Collections.Generic;
using BankReader.API;
using BankReader.API.Services;
using Caliburn.Micro;

namespace BankReader.ViewModels
{
    public class SettingsViewModel : Screen
    {
        public SettingsViewModel()
        {
            DisplayName = "Settings";
            Expressions = BankParser.Expressions;
        }

        public List<RegexCategory> Expressions { get; set; }
    }
}