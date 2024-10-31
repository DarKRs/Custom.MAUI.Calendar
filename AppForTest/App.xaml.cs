using System.Globalization;

namespace AppForTest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            /*var culture = new CultureInfo("fr-FR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;*/

            MainPage = new AppShell();
        }
    }
}
