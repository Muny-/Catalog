using System;
using Avalonia;
using Avalonia.Markup.Xaml;

namespace Catalog
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>().UsePlatformDetect().UseReactiveUI();
    }
}