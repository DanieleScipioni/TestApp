using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TestAppUWP.AppShell.Samples.Chess
{
    public sealed partial class ChessBoardUc
    {
        public ChessBoardUc()
        {
            InitializeComponent();
        }

        private void Image_OnDragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.Data.Properties.Add("chessman", sender);
        }

        private void Board_OnDragEnter(object sender, DragEventArgs e)
        {
            e.DragUIOverride.IsCaptionVisible = false;
            e.AcceptedOperation = DataPackageOperation.None;
        }

        private void Board_OnDragOver(object sender, DragEventArgs e)
        {
            var canvas = (Canvas)sender;
            DataPackageView dataPackageView = e.DataView;
            var chessman = (Image)dataPackageView.Properties["chessman"];

            (int x, int y)[] others = Board.Children.Where(c => c != chessman).Select(i => (GetCoordinates(canvas, i))).ToArray();

            (int x, int y) chessmanPoint = GetCoordinates(canvas, e.GetPosition(Board));

            foreach ((int x, int y) other in others)
            {
                if (other.x == chessmanPoint.x || other.y == chessmanPoint.y || Math.Abs(other.x - chessmanPoint.x) == Math.Abs(other.y - chessmanPoint.y))
                {
                    e.AcceptedOperation = DataPackageOperation.None;
                    return;
                }

                e.AcceptedOperation = DataPackageOperation.Move;
            }
        }

        private static (int x, int y) GetCoordinates(Canvas canvas, Point point)
        {
            return GetCoordinates(canvas, point.X, point.Y);
        }

        private static (int x, int y) GetCoordinates(Canvas canvas, UIElement uiElement)
        {
            double top = Canvas.GetTop(uiElement);
            double left = Canvas.GetLeft(uiElement);

            return GetCoordinates(canvas, left, top);
        }

        private static (int x, int y) GetCoordinates(Canvas canvas, double left, double top)
        {
            double cellWidth = canvas.ActualWidth / 8;
            double cellHeight = canvas.ActualHeight / 8;

            var x = (int)Math.Floor(left / cellWidth);
            var y = (int)Math.Floor(top / cellHeight);

            return (x, y);
        }
    }
}
