using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Text;

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

    private async void foundBleChars_ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (selectedService != null)
        {
            characteristic = characteristicsList[e.ItemIndex];
            bleChar_Label.Text = characteristic.Name + "\n" +
                "UUID: " + characteristic.Uuid.ToString() + "\n" +
                "Read: " + characteristic.CanRead + "\n" +
                "Write: " + characteristic.CanWrite + "\n" +
                "Update: " + characteristic.CanUpdate;

            var charDescriptors = await characteristic.GetDescriptorsAsync();

            bleChar_Label.Text += "\nDescriptors (" + charDescriptors.Count + "): ";
            for (int i = 0; i < charDescriptors.Count; i++)
            {
                bleChar_Label.Text += charDescriptors[i].Name + ", ";
            }
        }
    }

    private async void recieveCommandButton_Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (characteristic != null)
            {
                if (characteristic.CanRead)
                {
                    var recievedBytes = await characteristic.ReadAsync();
                    var result = Encoding.UTF8.GetString(recievedBytes.Item1, 0, recievedBytes.Item1.Length) + Environment.NewLine;
                    charData_Label.Text += result;


                }
                else
                {
                    await DisplayAlert("Characteristic Does Not Support Read", GetTimeNow(), "Ok");
                }
            }
            else
                await DisplayAlert("No Characteristic Selected", GetTimeNow(), "Ok");
        }
        catch
        {
            await DisplayAlert("Error Recieving Characteristic", GetTimeNow(), "Ok");
        }
    }

    private async void sendCommandButton_Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (characteristic != null)
            {
                if (characteristic.CanWrite)
                {
                    byte[] array = Encoding.UTF8.GetBytes(command_Entry.Text);
                    await characteristic.WriteAsync(array);
                }
                else
                {
                    await DisplayAlert("Characteristic Does Not Support Write", GetTimeNow(), "Ok");
                }
            }
        }
        catch
        {
            await DisplayAlert("Error Writing Characteristic", GetTimeNow(), "Ok");
        }
    }

    private async void registerCommandButton_Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (characteristic != null) // Ensure the characteristic exists
            {
                if (characteristic.CanUpdate) // Check if characteristic supports notifications
                {
                    characteristic.ValueUpdated += (o, args) => // Define a callback function
                    {
                        var receivedBytes = args.Characteristic.Value; // Read received bytes
                        Console.WriteLine("byte array: " + BitConverter.ToString(receivedBytes)); // Debug output

                        string _charStr = ""; // Initialize the string to hold the output

                        if (receivedBytes != null)
                        {
                            _charStr = "Bytes: " + BitConverter.ToString(receivedBytes); // Display the byte array as a string
                            _charStr += " | UTF8: " + Encoding.UTF8.GetString(receivedBytes, 0, receivedBytes.Length); // Display as UTF8 string
                        }

                        if (receivedBytes.Length <= 4) // Check if the received bytes could represent an integer
                        {
                            int char_val = 0;
                            for (int i = 0; i < receivedBytes.Length; i++)
                            {
                                char_val |= (receivedBytes[i] << i * 8);
                            }
                            _charStr += " | int: " + char_val.ToString();
                        }

                        _charStr += Environment.NewLine; // Append a new line

                        // Ensure the GUI is updated on the main thread in .NET MAUI
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            charData_Label.Text += _charStr;
                        });
                    };

                    await characteristic.StartUpdatesAsync(); // Start notifications

                    await DisplayAlert("Notify callback function registered successfully", GetTimeNow(), "Ok");
                }
                else
                {
                    await DisplayAlert("Notify callback function registered successfully", GetTimeNow(), "Ok");
                }
            }
            else
            {
                await DisplayAlert("Notify callback function registered successfully", GetTimeNow(), "Ok");
            }
        }
        catch (Exception ex) // Provide more detailed error handling
        {
            await DisplayAlert($"Error Initialising UART GATT Service: {ex.Message}", GetTimeNow(), "Ok");
        }
    }
    private string GetTimeNow()
    {
        var timestamp = DateTime.Now;
        return timestamp.Hour.ToString() + ":" + timestamp.Minute.ToString() + ":" + timestamp.Second.ToString();
    }
}

