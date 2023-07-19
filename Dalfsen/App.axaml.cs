using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Dalfsen.Views;
using Material.Colors;
using Material.Styles.Themes;

namespace Dalfsen;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        /*
         
         		#013ec9
		#cecece

         */
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();

        var primaryColor = SwatchHelper.Lookup[MaterialColor.Blue500];
        var secondaryColor = SwatchHelper.Lookup[MaterialColor.Grey500];

        var theme = Theme.Create(Theme.Light, primaryColor, secondaryColor);
        var themeBootstrap = this.LocateMaterialTheme<MaterialThemeBase>();
        themeBootstrap.CurrentTheme = theme;
    }
}
