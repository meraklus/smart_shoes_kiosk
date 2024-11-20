using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InTheHand.Bluetooth;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using BluetoothDevice = InTheHand.Bluetooth.BluetoothDevice;


namespace SmartShoes.Common.Forms
{
	public class BLEManager
	{
		// Singleton Instance
		private static BLEManager _instance;

        // Connected devices
        private BluetoothDevice _deviceR;
		private BluetoothDevice _deviceL;

		private List<BluetoothDevice> listDeviceR =  new List<BluetoothDevice>();
		private List<BluetoothDevice> listDeviceL = new List<BluetoothDevice>();

		public event EventHandler ThresholdReachedL;
		public event EventHandler ThresholdReachedR;

		public List<int[]> _parsedDataL = new List<int[]>();
		public List<int[]> _parsedDataR = new List<int[]>();

		public List<List<int[]>> listParsedDataL = new List<List<int[]>>();
		public List<List<int[]>> listParsedDataR = new List<List<int[]>>();

		private bool connectSignal = true;

		// Private constructor to prevent external instantiation
		private BLEManager() { }

		// Public property to access the singleton instance
		public static BLEManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new BLEManager();
				return _instance;
			}
		}

		// Connect to BLE devices
		public async Task<bool> ConnectAndReceiveAsync(string deviceAddrR, string deviceAddrL, int datalen, Guid serviceUUID, Guid notifyUUID)
		{
			try
			{
				connectSignal = true;

				var devices = await Bluetooth.ScanForDevicesAsync();

				Console.WriteLine($"devices found: {devices.Count}");
				if(!connectSignal) 
				{
					return false;
				}

				_deviceR = devices.FirstOrDefault(d => d.Id.ToString() == deviceAddrR);
				_deviceL = devices.FirstOrDefault(d => d.Id.ToString() == deviceAddrL);

				if (_deviceR != null)
				{
					Console.WriteLine($"Found left device: {_deviceR.Name}, Address: {_deviceR.Id}");
					_parsedDataR.Clear();
					await _deviceR.Gatt.ConnectAsync(); // Connect to right device
					await SetupNotifications(_deviceR, datalen, serviceUUID, notifyUUID); // Set up notification for right device
				}
				else
				{
					Console.WriteLine("Right device not found.");
					return false;
				}

				if (_deviceL != null)
				{
					Console.WriteLine($"Found left device: {_deviceL.Name}, Address: {_deviceL.Id}");
					_parsedDataL.Clear();
					await _deviceL.Gatt.ConnectAsync(); // Connect to left device
					await SetupNotifications(_deviceL, datalen, serviceUUID, notifyUUID); // Set up notification for left device
				}
				else
				{
					Console.WriteLine("Left device not found.");
					return false;
				} 

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				return false;
			}
		}


        private void OnConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            switch (sender.ConnectionStatus)
            {
                case BluetoothConnectionStatus.Connected:
                    Console.WriteLine("###. Device connected.");
                    break;
                case BluetoothConnectionStatus.Disconnected:
                    Console.WriteLine("###. Device disconnected.");
                    sender.ConnectionStatusChanged -= OnConnectionStatusChanged;
                    sender.Dispose();
                    break;
                default:
                    Console.WriteLine("###. Connection status changed.");
                    break;
            }
        }

        public async Task<bool> ConnectAndReceiveAsync2(string deviceAddrR, string deviceAddrL, int datalen, Guid serviceUUID, Guid notifyUUID)
		{
			try
			{
				connectSignal = true;

				// 찾은 장치를 저장할 리스트
				var devices = new List<BluetoothDevice>();

				// 검색 완료 플래그
				bool isDeviceRFound = false;
				bool isDeviceLFound = false;

				// Watcher 설정
				var watcher = new BluetoothLEAdvertisementWatcher();
				watcher.Received += async (sender, args) =>
				{
					// 장치의 BluetoothAddress를 MAC 주소 형식으로 변환
					string deviceAddress = args.BluetoothAddress.ToString("X").PadLeft(12, '0'); // 16진수 변환
					deviceAddress = string.Join(":", Enumerable.Range(0, 6).Select(i => deviceAddress.Substring(i * 2, 2))); // "XX:XX:XX:XX:XX:XX" 형식

					// 사용자가 원하는 MAC 주소와 비교
					if (deviceAddress.Equals(deviceAddrR, StringComparison.OrdinalIgnoreCase) && !isDeviceRFound)
					{
						// 오른쪽 장치 발견
						isDeviceRFound = true;
						var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
						device.ConnectionStatusChanged += OnConnectionStatusChanged;
                        //var device = BluetoothDevice.FromIdAsync(args.Advertisement.LocalName).GetAwaiter().GetResult();
                        if (device != null)
						{
							_deviceR = device;
							Console.WriteLine($"오른쪽 장치 발견: {_deviceR.Name}, 주소: {_deviceR.Id}");
						}
					}

					if (deviceAddress.Equals(deviceAddrL, StringComparison.OrdinalIgnoreCase) && !isDeviceLFound)
					{
						// 왼쪽 장치 발견
						isDeviceLFound = true;
						var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
                        device.ConnectionStatusChanged += OnConnectionStatusChanged;

                        //var device = BluetoothDevice.FromIdAsync(args.Advertisement.LocalName).GetAwaiter().GetResult();
                        if (device != null)
						{
							_deviceL = device;
							Console.WriteLine($"왼쪽 장치 발견: {_deviceL.Name}, 주소: {_deviceL.Id}");
						}
					}
				};

				// Watcher 시작
				watcher.Start();
				Console.WriteLine("장치 검색 시작...");

				// 원하는 모든 장치를 찾을 때까지 기다림
				while (!(isDeviceRFound && isDeviceLFound))
				{
					if (!connectSignal) // 검색을 중단해야 할 경우
					{
						watcher.Stop();
						return false;
					}

					await Task.Delay(500); // CPU 과부하를 막기 위해 잠시 대기
				}

				watcher.Stop();
				Console.WriteLine("장치 검색 완료!");

				// 오른쪽 장치 연결 및 알림 설정
				if (_deviceR != null)
				{
					_parsedDataR.Clear();
					await _deviceR.Gatt.ConnectAsync(); // 오른쪽 장치 연결
					await SetupNotifications(_deviceR, datalen, serviceUUID, notifyUUID); // 알림 설정
				}
				else
				{
					Console.WriteLine("오른쪽 장치를 찾을 수 없습니다.");
					return false;
				}

				// 왼쪽 장치 연결 및 알림 설정
				if (_deviceL != null)
				{
					_parsedDataL.Clear();
					await _deviceL.Gatt.ConnectAsync(); // 왼쪽 장치 연결
					await SetupNotifications(_deviceL, datalen, serviceUUID, notifyUUID); // 알림 설정
				}
				else
				{
					Console.WriteLine("왼쪽 장치를 찾을 수 없습니다.");
					return false;
				}

				return true; // 성공적으로 연결 및 설정 완료
			}
			catch (Exception ex)
			{
				Console.WriteLine($"오류 발생: {ex.Message}");
				return false;
			}
		}


		private int riint = 0;
        private int leint = 0;

        // Set up Notifications for a device
        private async Task SetupNotifications(BluetoothDevice device, int datalen, Guid serviceUUID, Guid notifyUUID)
		{
			var services = await device.Gatt.GetPrimaryServicesAsync(serviceUUID);
			foreach (var service in services)
			{
				var characteristic = await service.GetCharacteristicAsync(notifyUUID);
				if (characteristic.Uuid == notifyUUID)
				{
					// Enable notifications
					await characteristic.StartNotificationsAsync();
					Console.WriteLine($"Started notifications for {device.Name}");
					characteristic.CharacteristicValueChanged += (s, e) =>
					{
						var data = e.Value;
						
						if (data.Length > 0)
						{
							string decodedString = Encoding.ASCII.GetString(data).Trim();

							// 문자열을 정수 배열로 변환
							string[] strValues = decodedString.Split(',');
							int[] intValues = Array.ConvertAll(strValues, int.Parse);

							// 데이터 리스트에 추가
							if (device.Id.Equals(_deviceR.Id)) 
							{
								_parsedDataR.Add(intValues);
        //                        Console.WriteLine("right data : @@@@@@" + data);
        //                        Console.WriteLine("right int data : @@@@@@" + intValues);
								//riint++;
        //                        Console.WriteLine("right int : @@@@@@" + riint);
                                if (_parsedDataR.Count == datalen)
								{
									OnThresholdReachedR(EventArgs.Empty); // 이벤트 발생
								}
							}
							if (device.Id.Equals(_deviceL.Id)) 
							{
								_parsedDataL.Add(intValues);
                                //Console.WriteLine("left data : @@@@@@" + data);
                                //Console.WriteLine("left int data : @@@@@@" + intValues);
                                //leint++;
                                //Console.WriteLine("left int : @@@@@@" + leint);
                                if (_parsedDataL.Count == datalen)
								{
									OnThresholdReachedL(EventArgs.Empty); // 이벤트 발생
								}
							}	
						}
					};
				}
			}
		}


		// Get Connected right device
		public BluetoothDevice GetRightDevice()
		{
			return _deviceR;
		}

		// Get Connected left device
		public BluetoothDevice GetLeftDevice()
		{
			return _deviceL;
		}


		// Disconnect devices
		public async Task DisconnectDevicesAsync(Guid serviceUUID)
		{
			connectSignal = false;

			// Right device disconnection
			if (_deviceR != null)
			{
				// Stop notifications
				await StopNotificationsAsync(_deviceR, serviceUUID);
                // Disconnect
                _deviceR.Gatt.Disconnect();
				_deviceR = null;
                await Task.Delay(500);
            }

			// Left device disconnection
			if (_deviceL != null)
			{
				// Stop notifications
				await StopNotificationsAsync(_deviceL, serviceUUID);
                // Disconnect
                _deviceL.Gatt.Disconnect();
				_deviceL = null;
                await Task.Delay(500);
            }
			

        }

        //private async Task StopNotificationsAsync(BluetoothDevice device, Guid serviceUUID)
        //{
        //	var services = await device.Gatt.GetPrimaryServicesAsync();
        //	foreach (var service in services)
        //	{
        //		if (service.Uuid.Equals(serviceUUID)) 
        //		{
        //			var characteristics = await service.GetCharacteristicsAsync();
        //			foreach (var characteristic in characteristics)
        //			{
        //				if (characteristic.Uuid.Equals("6e400003-b5a3-f393-e0a9-e50e24dcca9e"))
        //				{
        //					await characteristic.StopNotificationsAsync();
        //				}

        //				Console.WriteLine($"Stopped notifications for {device.Name}");

        //			}
        //		}	
        //	}

        //}


        private async Task StopNotificationsAsync(BluetoothDevice device, Guid serviceUUID)
		{
			try
			{
				// 기본 서비스 가져오기
				var services = await device.Gatt.GetPrimaryServicesAsync();
				foreach (var service in services)
				{
					// UUID가 일치하는 서비스 찾기
					if (service.Uuid.Equals(serviceUUID))
					{
						var characteristics = await service.GetCharacteristicsAsync();
						foreach (var characteristic in characteristics)
						{
							// 특정 Characteristic UUID와 일치하는지 확인 후 알림 해제 시도
							if (characteristic.Uuid.Equals(new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e")))
							{
								await characteristic.StopNotificationsAsync();
								Console.WriteLine($"Stopped notifications for {device.Name}");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error stopping notifications for {device.Name}: {ex.Message}");
			}
		}

		#region :: Event ::
		// 왼쪽
		protected virtual void OnThresholdReachedL(EventArgs e)
		{
            Console.WriteLine($" 인보크 왼쪽 실행");
            ThresholdReachedL?.Invoke(this, e);
		}
		// 오른쪽
		protected virtual void OnThresholdReachedR(EventArgs e)
		{
            Console.WriteLine($" 인보크 오른쪽 실행");
            ThresholdReachedR?.Invoke(this, e);
		}
		#endregion

	}
}
