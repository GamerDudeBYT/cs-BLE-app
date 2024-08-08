using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System.Runtime.CompilerServices;

namespace BLE_App;

public partial class BluetoothPage : ContentPage
{
    IBluetoothLE ble = CrossBluetoothLE.Current;
    IAdapter adapter = CrossBluetoothLE.Current.Adapter;

    public BluetoothPage()
	{
		InitializeComponent();

        connectButton.IsVisible = false;
    }



    private async void Discover_Button_Clicked(object sender, EventArgs e)
    {
        var status = PermissionStatus.Unknown;

        status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();


        if (Permissions.ShouldShowRationale<Permissions.Bluetooth>())
        {
            await Shell.Current.DisplayAlert("Needs Permissions", "We cannot locate bluetooth devices", "Ok");
        }

        status = await Permissions.RequestAsync<Permissions.Bluetooth>();
        var items = new List<BluetoothDevice>
        {
        };

        

        adapter.DeviceDiscovered += (s, a) =>
        {
            var deviceName = "No Name";
            if (a.Device.Name != null)
            {
                deviceName = a.Device.Name;
            }
            items.Add(new BluetoothDevice { Name = deviceName, Device = a.Device });
        };

        adapter.ScanTimeout = 5000;
        await adapter.StartScanningForDevicesAsync();

        if (items.Count <= 0)
        {
            items.Add(new BluetoothDevice { Name = "NO DEVICES FOUND" });
        }

        btDevicesListView.ItemsSource = items;
    }

    private async void Connect_Button_Clicked(object sender, EventArgs e)
    {
        var device = btDevicesListView.SelectedItem as BluetoothDevice;
        try
        {   
            var connectedDevices = adapter.ConnectedDevices;
            foreach (var connectedDevice in connectedDevices)
            {
                await adapter.DisconnectDeviceAsync(connectedDevice);
            }
            await adapter.ConnectToDeviceAsync(device.Device);
            "4fafc201-1fb5-459e-8fcc-c5c9c331914b";
        }
        catch (DeviceConnectionException dcx)
        {
            statusLabel.Text = dcx.Message;
        }

        if (adapter.ConnectedDevices.Count >= 1)
        {
            statusLabel.Text = $"Connected to: {adapter.ConnectedDevices[0].Name}";
        }


    }
    
    private async void Characteristic_Button_Clicked(object sender, EventArgs e)
    {
        var services = await adapter.ConnectedDevices.GetServicesAsync();
    }

    private void btDevicesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        connectButton.IsVisible = true;
    }
}

public class BluetoothDevice
{
    public string Name { get; set; }
    public IDevice Device { get; set; }
}