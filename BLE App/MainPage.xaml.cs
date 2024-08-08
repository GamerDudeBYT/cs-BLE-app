namespace BLE_App
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count+=10;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void Alert_Test(object sender, EventArgs e)
        {
            DisplayAlert("Alert", "This is an alert", "I know that this is an alert");
        }

        private async void ChangePage1(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("NewPage");
        }
    }

}

