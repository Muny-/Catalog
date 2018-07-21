using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Catalog.CatalogApp
{
    public class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}