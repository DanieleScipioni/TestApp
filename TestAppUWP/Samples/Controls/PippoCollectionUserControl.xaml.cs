using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace TestAppUWP.Samples.Controls
{
    public sealed partial class PippoCollectionUserControl
    {
        private PippoCollection _pippoCollection;

        public PippoCollectionUserControl()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                if (_pippoCollection == args.NewValue || args.NewValue == null) return;
                _pippoCollection = (PippoCollection) args.NewValue;
                if (RootGrid.ColumnDefinitions.Count != _pippoCollection.Count)
                {
                    RootGrid.Children.Clear();
                    RootGrid.ColumnDefinitions.Clear();
                    for (var index = 0; index < _pippoCollection.Count; index++)
                    {
                        RootGrid.ColumnDefinitions.Add(new ColumnDefinition
                        {
                            Width = new GridLength(1, GridUnitType.Star)
                        });
                        var border = new Border
                        {
                            BorderBrush = new SolidColorBrush(Colors.BlueViolet),
                            BorderThickness = new Thickness(1),
                            Child = new TextBlock {VerticalAlignment = VerticalAlignment.Center}
                        };
                        Grid.SetColumn(border, index);
                        RootGrid.Children.Add(border);
                    }
                }

                for (var index = 0; index < _pippoCollection.Count; index++)
                {
                    Pippo pippo = _pippoCollection[index];
                    ((TextBlock)((Border)RootGrid.Children[index]).Child).Text = pippo.Intero.ToString();
                }
            };
            RootGrid.Tapped += async (sender, args) =>
            {
                Point position = args.GetPosition(null);
                List<UIElement> elements =
                    VisualTreeHelper.FindElementsInHostCoordinates(position, RootGrid).ToList();

                if (elements.Count > 0 && elements[0] is TextBlock textBlock)
                {
                    args.Handled = true;
                    await BigDynamicListPage.DoSomething(textBlock.Text);
                }
            };
            RootGrid.Tapped += async (sender, args) =>
            {
                await BigDynamicListPage.DoSomething("second tap");
            };
        }

        public BigDynamicListPage BigDynamicListPage { get; set; }
    }
}
