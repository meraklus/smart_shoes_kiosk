using System;
using InTheHand.Bluetooth;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth;
using System.Linq; 
using Windows.Devices.Enumeration;

using BluetoothDevice = InTheHand.Bluetooth.BluetoothDevice;

using Windows.Devices.Bluetooth;


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
        private static BLEManager _instance;

        // 인스턴스 존재 여부 확인 메서드 추가
        public static bool HasInstance()
        {
            return _instance != null;
        }

        public static BLEManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BLEManager();
                }
                return _instance;
            }
        }

        private BLEManager()
        {
            _time = 30; // 기본값 설정
        }
        #endregion

        #region Fields
        private BluetoothDevice _leftDevice;
        private BluetoothDevice _rightDevice;
        private readonly List<string> _leftData = new List<string>();
        private readonly List<string> _rightData = new List<string>();
        private bool _isScanning;
        private BluetoothLEAdvertisementWatcher _watcher;
        private int _time;

        // 데이터 수집 상태 관리
        private bool _isLeftShoeCollecting = false;
        private bool _isRightShoeCollecting = false;

        // 디바이스 연결 상태 저장
        private bool _isLeftDeviceConnected = false;
        private bool _isRightDeviceConnected = false;
        #endregion

        #region Events
        public event EventHandler<BluetoothDataEventArgs> DataReceived;
        public event EventHandler<BluetoothConnectionEventArgs> ConnectionStatusChanged;
        public event EventHandler<BluetoothDataCollectionEventArgs> DataCollectionCompleted;
        #endregion

        #region Properties
        public bool IsConnected => _leftDevice != null && _rightDevice != null;
        public bool IsScanning => _isScanning;

        // 디바이스 연결 상태를 리턴하는 프로퍼티 추가
        public bool IsLeftDeviceConnected => _isLeftDeviceConnected;
        public bool IsRightDeviceConnected => _isRightDeviceConnected;
        #endregion

        #region Public Methods
        public async Task InitializeConnection(string leftMacAddress, string rightMacAddress, int time)
        {
            try
            {
                _time = time;
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
                if (_leftDevice != null) await SendToDevice(_leftDevice, data);
                if (_rightDevice != null) await SendToDevice(_rightDevice, data);
                Console.WriteLine($"데이터 전송: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"데이터 전송 중 오류 발생: {ex.Message}");
            }
        }

        public void SetTimer(int time)
        {
            _time = time;
        }

        public async Task Start()
        {
            byte[] data = Encoding.UTF8.GetBytes($"@START#{_time}#");
            if (_leftDevice != null) await SendToDevice(_leftDevice, data);
            if (_rightDevice != null) await SendToDevice(_rightDevice, data);
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

        public List<string> GetLeftData() => new List<string>(_leftData);
        public List<string> GetRightData() => new List<string>(_rightData);

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
                // 주소를 16진수 형태로 변환하여 로그 출력
                string hexAddress = args.BluetoothAddress.ToString("X").PadLeft(12, '0');
                string formattedAddress = string.Join(":", Enumerable.Range(0, 6)
                    .Select(i => hexAddress.Substring(i * 2, 2)));
                // 이미 발견된 장치는 무시
                if (foundAddresses.Contains(args.BluetoothAddress))
                    return;

                if (args.BluetoothAddress == leftAddress || args.BluetoothAddress == rightAddress)
                {
                    // 발견된 장치 기록
                    foundAddresses.Add(args.BluetoothAddress);
                    Console.WriteLine($"장치 발견: {(args.BluetoothAddress == leftAddress ? "왼쪽" : "오른쪽")} 신발");

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

            if (!foundAddresses.Contains(leftAddress) || !foundAddresses.Contains(rightAddress))
            {
                throw new Exception("블루투스 탐색에 실패했습니다.");
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

                // 디바이스 연결 상태 업데이트
                if (isLeft)
                {
                    _isLeftDeviceConnected = true;
                }
                else
                {
                    _isRightDeviceConnected = true;
                }

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
            try
            {
                // 추가예정
                var service = await device.Gatt.GetPrimaryServiceAsync(_serviceUuid);
                var characteristic = await service.GetCharacteristicAsync(_writeUuid);
                characteristic.WriteValueWithoutResponseAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendToDevice error: {ex.Message}");
            }

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

                // 디바이스 연결 상태 업데이트
                if (isLeft)
                {
                    _isLeftDeviceConnected = false;
                }
                else
                {
                    _isRightDeviceConnected = false;
                }
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
                var rawData = e.Value;
                var device = ((GattCharacteristic)sender).Service.Device;
                bool isLeft = device.Id == _leftDevice.Id;

                // 데이터를 문자열로 변환
                string dataString = Encoding.ASCII.GetString(rawData).Trim();
                string hexData = BitConverter.ToString(rawData).Replace("-", "");

                if (dataString == "DATAEND")
                {
                    if (isLeft)
                    {
                        byte[] data = Encoding.UTF8.GetBytes("@SDATA#");
                        SendToDevice(_leftDevice, data);
                    }
                    else
                    {
                        byte[] data = Encoding.UTF8.GetBytes("@SDATA#");
                        SendToDevice(_rightDevice, data);
                    }
                    return;
                }
                // 시작 신호 확인
                if (hexData == "AAAAAAAAAAAA")
                {
                    if (isLeft)
                    {
                        _isLeftShoeCollecting = true;
                        _leftData.Clear();
                    }
                    else
                    {
                        _isRightShoeCollecting = true;
                        _rightData.Clear();
                    }

                    Console.WriteLine($"{(isLeft ? "왼쪽" : "오른쪽")} 신발 데이터 수집 시작");
                    return;
                }

                // 종료 신호 확인
                if (hexData == "555555555555")
                {
                    if (isLeft)
                        _isLeftShoeCollecting = false;
                    else
                        _isRightShoeCollecting = false;

                    Console.WriteLine($"{(isLeft ? "왼쪽" : "오른쪽")} 신발 데이터 수집 완료");

                    // 양쪽 모두 수집 완료 확인
                    if (!_isLeftShoeCollecting && !_isRightShoeCollecting)
                    {
                        OnDataCollectionCompleted();
                    }

                    return;
                }

                // 데이터 수집 중인 경우에만 처리
                if ((isLeft && _isLeftShoeCollecting) || (!isLeft && _isRightShoeCollecting))
                {
                    // 데이터 파싱 및 저장
                    Console.WriteLine($"hexData: {hexData}");
                    if (isLeft)
                        _leftData.Add(hexData);
                    else
                        _rightData.Add(hexData);

                    // 데이터 수신 이벤트 발생
                    // DataReceived?.Invoke(this, new BluetoothDataEventArgs(isLeft, data));
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

        // private string ParseData(byte[] rawData)
        // {
        //     var decodedString = Encoding.ASCII.GetString(rawData).Trim();
        //     var strValues = decodedString.Split(',');
        //     return Array.ConvertAll(strValues, s => s.Trim());
        // }

        // 데이터 수집 완료 이벤트 발생
        private void OnDataCollectionCompleted()
        {
            Console.WriteLine("양쪽 신발 데이터 수집 완료");

            var leftDataCopy = new List<string>(_leftData);
            var rightDataCopy = new List<string>(_rightData);

            DataCollectionCompleted?.Invoke(this,
                new BluetoothDataCollectionEventArgs(leftDataCopy, rightDataCopy));
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

    // 데이터 수집 완료 이벤트 인자
    public class BluetoothDataCollectionEventArgs : EventArgs
    {
        public List<string> LeftData { get; }
        public List<string> RightData { get; }

        public BluetoothDataCollectionEventArgs(List<string> leftData, List<string> rightData)
        {
            LeftData = leftData;
            RightData = rightData;
        }
    }
}
