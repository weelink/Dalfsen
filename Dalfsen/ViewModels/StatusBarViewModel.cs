namespace Dalfsen.ViewModels
{
    public class StatusBarViewModel : ViewModel
    {
        private string? text;
        private int minimum;
        private int maximum;
        private int value;
        private bool indeterminate;

        public StatusBarViewModel()
        {
            Reset();
        }

        public void Reset()
        {
            Text = "";
            Minimum = 0;
            Maximum = 100;
            Value = 0;
            Indeterminate = false;
        }

        public string? Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        public int Minimum
        {
            get { return minimum; }
            set { SetProperty(ref minimum, value); }
        }

        public int Maximum
        {
            get { return maximum; }
            set { SetProperty(ref maximum, value); }
        }

        public int Value
        {
            get { return value; }
            set { SetProperty(ref this.value, value); }
        }

        public bool Indeterminate
        {
            get { return indeterminate; }
            set { SetProperty(ref indeterminate, value); }
        }
    }
}
