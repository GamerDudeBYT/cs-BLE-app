using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace BLE_App;

public partial class Bluetooth_Devices : ContentPage
{
    IBluetoothLE ble;
    IAdapter adapter;
    List<BluetoothDevice> devices = new List<BluetoothDevice>();

    public Bluetooth_Devices()
	{
		InitializeComponent();

        ble = CrossBluetoothLE.Current;
        adapter = CrossBluetoothLE.Current.Adapter;
        adapter.DeviceDiscovered += (sender, foundBleDevice) =>
        {
            if (foundBleDevice != null && !string.IsNullOrEmpty(foundBleDevice.Device.Name))
            {
                devices.Add(new BluetoothDevice { Name = foundBleDevice.Device.Name, Device = foundBleDevice.Device });
            }
        };
    }

    private async void scan_Button_Clicked(object sender, EventArgs e)
    {
        IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(scan_Button.IsEnabled = false);
        await Permissions.RequestAsync<Permissions.Bluetooth>();
        devices.Clear();

        if (!adapter.IsScanning)
        {
            await adapter.StartScanningForDevicesAsync();
        }

        bleDevices_ListView.ItemsSource = devices.ToArray();
        IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(scan_Button.IsEnabled = true);
    }

    private async void bleDevices_ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(scan_Button.IsEnabled = false);
        BluetoothDevice selectedItem = e.Item as BluetoothDevice;

        IDevice selectedDevice = selectedItem.Device;

        if (selectedDevice.State == DeviceState.Connected)
        {
            await Navigation.PushAsync(new Bluetooth_Services(selectedDevice));
        }
        else
        {
            try
            {
                var connectParameters = new ConnectParameters(false, true);
                await adapter.ConnectToDeviceAsync(selectedDevice);
                await Navigation.PushAsync(new Bluetooth_Services(selectedDevice));
            }
            catch
            {
                await DisplayAlert("Error Connecting", $"Error connecting to BLE device: {selectedDevice}", "Ok");
            }
        }

        IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(scan_Button.IsEnabled = true);
    }
}

public class BluetoothDevice
{
    public string Name { get; set; }
    public IDevice Device { get; set; }
}