using AccessControlMobileApp.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControlMobileApp.Services
{
    public class BluetoothService
    {
        private readonly IAdapter bluetoothAdapter;
        private readonly List<IDevice> bluetoothDevices = new List<IDevice>();
        private ICharacteristic characteristic;

        public BluetoothService()
        {
            bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            bluetoothAdapter.DeviceDiscovered += (sender, foundBleDevice) =>
            {
                if (foundBleDevice.Device != null && !string.IsNullOrEmpty(foundBleDevice.Device.Name))
                    bluetoothDevices.Add(foundBleDevice.Device);
            };
            characteristic = null;
        }

        public async Task<List<IDevice>> ScanForDevices()
        {
            bluetoothDevices.Clear();

            try
            {
                if (!bluetoothAdapter.IsScanning)
                {
                    await bluetoothAdapter.StartScanningForDevicesAsync();
                }
                foreach (var device in bluetoothAdapter.ConnectedDevices)
                    bluetoothDevices.Add(device);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scanning for BLE devices: {ex.Message}");
            }

            return bluetoothDevices;
        }

        public async Task<bool> ConnectToDevice(object device)
        {

            IDevice selectedItem = device as IDevice;

            if (selectedItem.State == DeviceState.Connected)
            {
                return true;
            }
            else
            {
                try
                {
                    var connectParameters = new ConnectParameters(false, true);
                    await bluetoothAdapter.ConnectToDeviceAsync(selectedItem, connectParameters);
                    var servicesListReadOnly = await selectedItem.GetServicesAsync();
                    var selectedService = servicesListReadOnly[servicesListReadOnly.Count - 1];
                    var charListReadOnly = await selectedService.GetCharacteristicsAsync();
                    characteristic = charListReadOnly[charListReadOnly.Count - 1];
                    return true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error connecting to device: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> SendMessage(string message)
        {
            try
            {
                if (characteristic != null)
                {
                    if (characteristic.CanWrite)
                    {
                        byte[] array = Encoding.UTF8.GetBytes(message);
                        await characteristic.WriteAsync(array);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Characteristic does not support Write");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                Console.WriteLine("Error receiving Characteristic");
                return false;
            }
        }
    }
}
