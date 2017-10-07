using System;
using TestAppUWP.Core;

namespace TestAppUWP.Samples.ColorAnimation
{
    public class ColorAnimationViewModel : BindableBase
    {
        private string _text;
        public string Text
        {
            get => _text;
            private set => SetProperty(ref _text, value);
        }

        public DelegateCommand ChangeTextCommand;

        private readonly Random _random;

        public ColorAnimationViewModel()
        {
            _random = new Random(DateTime.UtcNow.Millisecond);
            _text = _random.Next().ToString();
            ChangeTextCommand = new DelegateCommand(ChangeTextAction);
        }

        private void ChangeTextAction(object obj)
        {
            int next = _random.Next(10);
            Text = next == 0 ? _random.Next(10).ToString() : _text + next;
        }
    }
}