using Plugin.BLE.Abstractions.Contracts;

namespace BLE_App;

public partial class Bluetooth_Services : ContentPage
{
    IDevice connectedDevice;
    List<IService> servicesList = new List<IService>();

    public Bluetooth_Services(IDevice recievedDevice)
	{
		InitializeComponent();

        connectedDevice = recievedDevice;
        bleDevice_Label.Text = "Selected BLE Device: " + connectedDevice.Name;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var servicesListReadOnly = await connectedDevice.GetServicesAsync();

            servicesList.Clear();
            var servicesListStr = new List<String>();
            for (int i = 0; i < servicesListReadOnly.Count; i++)
            {
                servicesList.Add(servicesListReadOnly[i]);
                servicesListStr.Add(servicesListReadOnly[i].Name + ", UUID: " + servicesListReadOnly[i].Id.ToString());
            }
            foundBLEServices_ListView.ItemsSource = servicesListStr;
        }
        catch
        {
            await DisplayAlert("Error Initialising", $"Error initialising UART GATT service.", "Ok");
        }
    }

    private async void foundBLEServices_ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var selectedService = servicesList[e.ItemIndex];
        if (selectedService != null)
        {
            await Navigation.PushAsync(new Bluetooth_Characteristics(connectedDevice, selectedService));
        }
    }
}