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
        private BluetoothLEAdvertisementWatcher _watcher;
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
                // 연결 해제 시작을 알림
                OnConnectionStatusChanged(new BluetoothConnectionEventArgs(true, false));
                OnConnectionStatusChanged(new BluetoothConnectionEventArgs(false, false));
                
                await DisconnectDevicesAsync();
                
                // MAC 주소 변환
                ulong leftAddress = Convert.ToUInt64(leftMacAddress.Replace(":", ""), 16);
                ulong rightAddress = Convert.ToUInt64(rightMacAddress.Replace(":", ""), 16);
                
                // 스캔 시작
                await ScanAndConnectDevices(leftAddress, rightAddress);
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged(new BluetoothConnectionEventArgs(true, false, ex.Message));
                OnConnectionStatusChanged(new BluetoothConnectionEventArgs(false, false, ex.Message));
                throw;
            }
        }

        public async Task SendData(string message)
        {
            try
            {
                if (!IsConnected)
                {
                    Console.WriteLine("블루투스 장치가 연결되어 있지 않습니다.");
                    return;
                }

                byte[] data = Encoding.UTF8.GetBytes(message);
                
                await SendToDevice(_leftDevice, data);
                await SendToDevice(_rightDevice, data);
            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"데이터 전송 중 오류 발생: {ex.Message}");
            }
        }

        public async Task DisconnectDevicesAsync()
        {
            _isScanning = false;
            
            if (_leftDevice != null)
            {
                await DisconnectDevice(_leftDevice, true);
                _leftDevice = null;
            }
            
            if (_rightDevice != null)
            {
                await DisconnectDevice(_rightDevice, false);
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
        private async Task ScanAndConnectDevices(ulong leftAddress, ulong rightAddress)
        {
            var tcs = new TaskCompletionSource<bool>();
            
            // 발견된 장치 주소 저장
            var foundAddresses = new HashSet<ulong>();
            
            _watcher = new BluetoothLEAdvertisementWatcher();
            _watcher.ScanningMode = BluetoothLEScanningMode.Active;
            
            _watcher.Received += async (sender, args) => {
                // 이미 발견된 장치는 무시
                if (foundAddresses.Contains(args.BluetoothAddress))
                    return;
                    
                if (args.BluetoothAddress == leftAddress || args.BluetoothAddress == rightAddress)
                {
                    // 발견된 장치 기록
                    foundAddresses.Add(args.BluetoothAddress);
                    Console.WriteLine($"장치 발견: {args.BluetoothAddress:X}");
                    
                    // 양쪽 모두 발견되면 스캔 중지 후 연결 시도
                    if (foundAddresses.Contains(leftAddress) && foundAddresses.Contains(rightAddress))
                    {
                        _watcher.Stop();
                        Console.WriteLine("양쪽 장치 모두 발견, 연결 시도 중...");
                        
                        try
                        {
                            // 양쪽 장치 동시에 연결 시도
                            var leftTask = ConnectToDevice(leftAddress, "Left");
                            var rightTask = ConnectToDevice(rightAddress, "Right");
                            
                            await Task.WhenAll(leftTask, rightTask);
                            
                            _leftDevice = await leftTask;
                            _rightDevice = await rightTask;
                            
                            if (_leftDevice != null && _rightDevice != null)
                            {
                                tcs.SetResult(true);
                            }
                            else
                            {
                                tcs.SetException(new Exception("장치 연결에 실패했습니다."));
                            }
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }
                }
            };
            
            _watcher.Start();
            
            // 30초 타임아웃
            var timeoutTask = Task.Delay(60000);
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);
            
            _watcher.Stop();
            
            if (completedTask == timeoutTask)
            {
                throw new TimeoutException("블루투스 장치 스캔 시간이 초과되었습니다.");
            }
            
            if (_leftDevice == null || _rightDevice == null)
            {
                throw new Exception("블루투스 연결에 실패했습니다.");
            }
        }

        private async Task<BluetoothDevice> ConnectToDevice(ulong address, string side)
        {
            try
            {
                var device = await BluetoothLEDevice.FromBluetoothAddressAsync(address);
                if (device == null) return null;

                await SetupDeviceNotifications(device);
                
                // 연결 성공 이벤트 발생
                var isLeft = side == "Left";
                OnConnectionStatusChanged(new BluetoothConnectionEventArgs(isLeft, true));
                
                Console.WriteLine($"{side} device connected");
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
            await characteristic.WriteValueWithoutResponseAsync(data);
        }

        private async Task DisconnectDevice(BluetoothDevice device, bool isLeft)
        {
            try
            {
                // 알림 구독 해제
                var service = await device.Gatt.GetPrimaryServiceAsync(_serviceUuid);
                var characteristic = await service.GetCharacteristicAsync(_notifyUuid);
                await characteristic.StopNotificationsAsync();
                characteristic.CharacteristicValueChanged -= OnCharacteristicValueChanged;

                // 연결 해제
                device.Gatt.Disconnect();
                await Task.Delay(500);

                // 연결 해제 이벤트 발생
                OnConnectionStatusChanged(new BluetoothConnectionEventArgs(isLeft, false));
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
            Console.WriteLine($"OnDeviceConnectionStatusChanged: {sender.BluetoothAddress}, {sender.ConnectionStatus}");
            var isLeft = true; //sender == _leftDevice;
            OnConnectionStatusChanged(new BluetoothConnectionEventArgs(isLeft, isConnected));
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
        public bool IsLeft { get; }  // 왼쪽/오른쪽 구분
        public bool IsConnected { get; }
        public string ErrorMessage { get; }
        
        public BluetoothConnectionEventArgs(bool isLeft, bool isConnected, string errorMessage = null)
        {
            IsLeft = isLeft;
            IsConnected = isConnected;
            ErrorMessage = errorMessage;
        }
    }
}
