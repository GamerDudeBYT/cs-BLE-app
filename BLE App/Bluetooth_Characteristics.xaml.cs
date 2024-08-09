using Plugin.BLE.Abstractions.Contracts;

namespace BLE_App;

public partial class Bluetooth_Characteristics : ContentPage
{
	IDevice connectedDevice;
	IService selectedService;
	List<ICharacteristic> characteristicsList = new List<ICharacteristic>();
	ICharacteristic characteristic;

	public Bluetooth_Characteristics(IDevice _connectedDevice, IService _selectedService)
	{
		InitializeComponent();

		connectedDevice = _connectedDevice;
		selectedService = _selectedService;
		characteristic = null;

		bleDevice_Label.Text = "Selected BLE Device: " + connectedDevice.Name;
		bleService_Label.Text = "Selected BLE Service: " + selectedService.Name;
	}

	protected async override void OnAppearing()
	{
		base.OnAppearing();
		try
		{
			if (selectedService != null)
			{
				var characterListReadOnly = await selectedService.GetCharacteristicsAsync();
				characteristicsList.Clear();
				var characteristicsListStr = new List<String>();

				for (int i = 0; i < characterListReadOnly.Count; i++)
				{
					characteristicsList.Add(characterListReadOnly[i]);
					characteristicsListStr.Add(characterListReadOnly[i].Name + ", UUID: " + characterListReadOnly[i].Id.ToString());
				}
				foundBleChars_ListView.ItemsSource = characteristicsListStr;
			}
			else
			{
				await DisplayAlert("UART GATT Service Not Found", GetTimeNow(), "Ok");
			}
		}
		catch
		{
			await DisplayAlert("Error Initialising UART GATT Service", GetTimeNow(), "Ok");
		}
	}

    private void foundBleChars_ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {

    }

    private string GetTimeNow()
    {
        var timestamp = DateTime.Now;
        return timestamp.Hour.ToString() + ":" + timestamp.Minute.ToString() + ":" + timestamp.Second.ToString();
    }
}