using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace BankReader.Views
{
    /// <summary>
    ///     Interaction logic for TransactionsView.xaml
    /// </summary>
    public partial class TransactionsView
    {
        public TransactionsView()
        {
            InitializeComponent();
            Loaded += (s, e) => DateFilter.Focus();
        }

        private void Accounts_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var displayName = GetPropertyDisplayName(e.PropertyDescriptor);
            if (!string.IsNullOrEmpty(displayName)) e.Column.Header = displayName;
        }

        public static string GetPropertyDisplayName(object descriptor)
        {
            if (descriptor is PropertyDescriptor pd)
            {
                // Check for DisplayName attribute and set the column header accordingly
                if (pd.Attributes[typeof(DisplayNameAttribute)] is DisplayNameAttribute displayName &&
                    !Equals(displayName, DisplayNameAttribute.Default)) return displayName.DisplayName;
            }
            else
            {
                var pi = descriptor as PropertyInfo;
                if (pi == null) return null;
                // Check for DisplayName attribute and set the column header accordingly
                var attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                return (from t in attributes
                    select t as DisplayNameAttribute
                    into displayName
                    where displayName != null && !Equals(displayName, DisplayNameAttribute.Default)
                    select displayName.DisplayName).FirstOrDefault();
            }

            return null;
        }
    }
}