using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SmartShoes.Common.Forms
{
	public class DelphiHelper : IDisposable
	{
		private IntPtr libHandle = IntPtr.Zero;
		private bool disposed = false;

		public DelphiHelper(string relativeDllPath)
		{
			string basePath = AppDomain.CurrentDomain.BaseDirectory;
			string fullPath = Path.Combine(basePath, relativeDllPath);

			Console.WriteLine($"Trying to load DLL from: {fullPath}");

			libHandle = LoadLibrary(fullPath);
			if (libHandle == IntPtr.Zero)
			{
				int errorCode = Marshal.GetLastWin32Error();
				throw new Exception($"Failed to load DLL from {fullPath}. Error code: {errorCode}");
			}
		}

		~DelphiHelper()
		{
			Dispose(false);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		private void UnloadDll()
		{
			if (libHandle != IntPtr.Zero)
			{
				FreeLibrary(libHandle);
				libHandle = IntPtr.Zero;
			}
		}

		public T GetFunction<T>(string functionName) where T : Delegate
		{
			IntPtr procAddress = GetProcAddress(libHandle, functionName);
			if (procAddress == IntPtr.Zero)
				throw new Exception($"Function {functionName} not found in DLL.");


			return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TShowForm(IntPtr handle, ushort top, ushort left, ushort width, ushort height, bool button);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TCloseForm();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TShowFormOnly();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TShowFormAuto(ushort time, ushort top, ushort left, ushort width, ushort height);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TShowFormStop(bool save);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TMeasurestart(ushort time);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TMeasurestop(bool save);

		// Dispose 패턴 구현
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					// 관리되는 리소스 해제 (필요할 경우)
				}

				// 관리되지 않는 리소스 해제
				UnloadDll();

				disposed = true;
			}
		}
	}
}
