using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InTheHand.Bluetooth;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using BluetoothDevice = InTheHand.Bluetooth.BluetoothDevice;


namespace SmartShoes.Common.Forms
{
    public class BLEManager
    {
        #region Constants
        private readonly Guid _serviceUuid = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
        private readonly Guid _notifyUuid = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");
        private readonly Guid _writeUuid = new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e");
        #endregion

        #region Singleton
        private static readonly Lazy<BLEManager> _instance = 
            new Lazy<BLEManager>(() => new BLEManager());
        public static BLEManager Instance => _instance.Value;
        private BLEManager() { }
        #endregion

        #region Fields
        private BluetoothDevice _leftDevice;
        private BluetoothDevice _rightDevice;
        private readonly List<int[]> _leftData = new List<int[]>();
        private readonly List<int[]> _rightData = new List<int[]>();
        private bool _isScanning;
        #endregion

        #region Events
        public event EventHandler<BluetoothDataEventArgs> DataReceived;
        public event EventHandler<BluetoothConnectionEventArgs> ConnectionStatusChanged;
        #endregion

        #region Properties
        public bool IsConnected => _leftDevice != null && _rightDevice != null;
        public bool IsScanning => _isScanning;
        #endregion

        #region Public Methods
        public async Task InitializeConnection(string leftMacAddress, string rightMacAddress)
        {
            try
            {
                await DisconnectDevicesAsync();
                await ConnectToDevices(leftMacAddress, rightMacAddress);
                OnConnectionStatusChanged(new BluetoothConnectionEventArgs(true));
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged(new BluetoothConnectionEventArgs(false, ex.Message));
                throw;
            }
        }

        public async Task SendData(byte[] data)
        {
            if (!IsConnected) 
                throw new InvalidOperationException("블루투스 장치가 연결되어 있지 않습니다.");
            
            await Task.WhenAll(
                SendToDevice(_leftDevice, data),
                SendToDevice(_rightDevice, data)
            );
        }

        public async Task DisconnectDevicesAsync()
        {
            _isScanning = false;
            
            if (_leftDevice != null)
            {
                await DisconnectDevice(_leftDevice);
                _leftDevice = null;
            }
            
            if (_rightDevice != null)
            {
                await DisconnectDevice(_rightDevice);
                _rightDevice = null;
            }
            
            _leftData.Clear();
            _rightData.Clear();
        }

        public List<int[]> GetLeftData() => new List<int[]>(_leftData);
        public List<int[]> GetRightData() => new List<int[]>(_rightData);

        // public async Task InitializeFromSavedSettings()
        // {
        //     string leftMac = Properties.Settings.Default.BLE_LEFT_MAC_ADDRESS;
        //     string rightMac = Properties.Settings.Default.BLE_RIGHT_MAC_ADDRESS;
            
        //     if (string.IsNullOrEmpty(leftMac) || string.IsNullOrEmpty(rightMac))
        //         throw new InvalidOperationException("블루투스 설정이 필요합니다. 설정 화면에서 신발을 선택해주세요.");
        
        //     await InitializeConnection(leftMac, rightMac);
        // }
        #endregion

        #region Private Methods
        private async Task ConnectToDevices(string leftMacAddress, string rightMacAddress)
        {
            _isScanning = true;
            var watcher = new BluetoothLEAdvertisementWatcher();
            var tcs = new TaskCompletionSource<bool>();

            watcher.Received += async (sender, args) =>
            {
                string deviceAddress = args.BluetoothAddress.ToString("X").PadLeft(12, '0');
                deviceAddress = string.Join(":", Enumerable.Range(0, 6)
                    .Select(i => deviceAddress.Substring(i * 2, 2)));

                if (deviceAddress.Equals(leftMacAddress, StringComparison.OrdinalIgnoreCase) && _leftDevice == null)
                {
                    _leftDevice = await ConnectToDevice(args.BluetoothAddress, "Left");
                }
                else if (deviceAddress.Equals(rightMacAddress, StringComparison.OrdinalIgnoreCase) && _rightDevice == null)
                {
                    _rightDevice = await ConnectToDevice(args.BluetoothAddress, "Right");
                }

                if (_leftDevice != null && _rightDevice != null)
                {
                    watcher.Stop();
                    _isScanning = false;
                    tcs.TrySetResult(true);
                }
            };

            watcher.Start();

            await Task.WhenAny(tcs.Task, Task.Delay(10000));
            watcher.Stop();
            _isScanning = false;

            if (_leftDevice == null || _rightDevice == null)
                throw new TimeoutException("블루투스 장치를 찾을 수 없습니다.");
        }

        private async Task<BluetoothDevice> ConnectToDevice(ulong address, string side)
        {
            try
            {
                var device = await BluetoothLEDevice.FromBluetoothAddressAsync(address);
                if (device == null) return null;

                device.ConnectionStatusChanged += OnDeviceConnectionStatusChanged;
                await SetupDeviceNotifications(device);
                
                return device;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{side} device connection error: {ex.Message}");
                return null;
            }
        }

        private async Task SetupDeviceNotifications(BluetoothDevice device)
        {
            var service = await device.Gatt.GetPrimaryServiceAsync(_serviceUuid);
            var characteristic = await service.GetCharacteristicAsync(_notifyUuid);
            
            await characteristic.StartNotificationsAsync();
            characteristic.CharacteristicValueChanged += OnCharacteristicValueChanged;
        }

        private async Task SendToDevice(BluetoothDevice device, byte[] data)
        {
            var service = await device.Gatt.GetPrimaryServiceAsync(_serviceUuid);
            var characteristic = await service.GetCharacteristicAsync(_writeUuid);
            await characteristic.WriteValueWithResponseAsync(data);
        }

        private async Task DisconnectDevice(BluetoothDevice device)
        {
            try
            {
                device.Gatt.Disconnect();
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disconnect error: {ex.Message}");
            }
        }

        private void OnCharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
        {
            try
            {
                var data = ParseData(e.Value);
                var device = ((GattCharacteristic)sender).Service.Device;
                
                if (device == _leftDevice)
                {
                    _leftData.Add(data);
                    DataReceived?.Invoke(this, new BluetoothDataEventArgs(true, data));
                }
                else if (device == _rightDevice)
                {
                    _rightData.Add(data);
                    DataReceived?.Invoke(this, new BluetoothDataEventArgs(false, data));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Data parsing error: {ex.Message}");
            }
        }

        private void OnDeviceConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            var isConnected = sender.ConnectionStatus == BluetoothConnectionStatus.Connected;
            OnConnectionStatusChanged(new BluetoothConnectionEventArgs(isConnected));
        }

        private void OnConnectionStatusChanged(BluetoothConnectionEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, e);
        }

        private int[] ParseData(byte[] rawData)
        {
            var decodedString = Encoding.ASCII.GetString(rawData).Trim();
            var strValues = decodedString.Split(',');
            return Array.ConvertAll(strValues, int.Parse);
        }
        #endregion
    }

    public class BluetoothDataEventArgs : EventArgs
    {
        public bool IsLeftDevice { get; }
        public int[] Data { get; }
        
        public BluetoothDataEventArgs(bool isLeftDevice, int[] data)
        {
            IsLeftDevice = isLeftDevice;
            Data = data;
        }
    }

    public class BluetoothConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; }
        public string ErrorMessage { get; }
        
        public BluetoothConnectionEventArgs(bool isConnected, string errorMessage = null)
        {
            IsConnected = isConnected;
            ErrorMessage = errorMessage;
        }
    }
}
