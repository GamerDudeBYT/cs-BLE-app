namespace BLE_App;

public partial class InternetTest : ContentPage
{
	public InternetTest()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		var hasInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

		DisplayAlert("Has Internet: ", $"{hasInternet}", "Ok");
    }
}