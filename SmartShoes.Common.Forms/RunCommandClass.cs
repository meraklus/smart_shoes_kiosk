using System;
using System.Diagnostics;

namespace SmartShoes.Common.Forms
{
	public class RunCommandClass
	{
		// 명령어와 인자를 받아 실행하는 메서드
		public void RunCommand(string command, string arguments)
		{
			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = command,               // 직접 실행할 파일 경로
					Arguments = arguments,            // 인자 전달
					WindowStyle = ProcessWindowStyle.Hidden, // 창 숨김
					CreateNoWindow = true,            // 창 생성 안 함
					UseShellExecute = false,          // 셸 실행 사용 안 함
					RedirectStandardOutput = true,    // 표준 출력 리디렉션
					RedirectStandardError = true      // 표준 오류 리디렉션
				};

				using (Process process = new Process())
				{
					process.StartInfo = startInfo;
					process.OutputDataReceived += (sender, e) =>
					{
						if (!string.IsNullOrEmpty(e.Data))
						{
							Console.WriteLine(e.Data); // 실시간 출력
						}
					};
					process.ErrorDataReceived += (sender, e) =>
					{
						if (!string.IsNullOrEmpty(e.Data))
						{
							Console.WriteLine("Error: " + e.Data); // 실시간 오류 출력
						}
					};

					process.Start(); // 프로세스 시작
					process.BeginOutputReadLine(); // 비동기적으로 표준 출력 읽기 시작
					process.BeginErrorReadLine();  // 비동기적으로 표준 오류 읽기 시작

					process.WaitForExit(); // 프로세스가 종료될 때까지 대기
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("명령어 실행 중 오류가 발생했습니다: " + ex.Message);
				Console.WriteLine($"명령어: {command}");
				Console.WriteLine($"인자: {arguments}");
			}
		}


	}
}