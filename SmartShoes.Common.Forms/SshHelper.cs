using System;
using Renci.SshNet;
using System.Net.Sockets;

public class SshHelper
{
	private string host;
	private string username;
	private string password;
	private SshClient client;

	public SshHelper(string host, string username, string password)
	{
		this.host = host;
		this.username = username;
		this.password = password;
		this.client = new SshClient(host, username, password);
	}

	public void Connect()
	{
		try
		{
			if (!client.IsConnected)
			{
				client.Connect();
			}
		}
		catch (SocketException ex)
		{
			throw new InvalidOperationException("호스트를 찾을 수 없습니다. IP 주소나 도메인이 올바른지 확인하십시오.", ex);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("SSH 연결 중 오류가 발생했습니다.", ex);
		}
	}

	public void Disconnect()
	{
		if (client.IsConnected)
		{
			client.Disconnect();
		}
	}

	public string ExecuteCommand(string command)
	{
		try
		{
			if (!client.IsConnected)
			{
				throw new InvalidOperationException("SSH client is not connected.");
			}

			var sshCommand = client.CreateCommand(command);
			return sshCommand.Execute();
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("명령어 실행 중 오류가 발생했습니다.", ex);
		}
	}

	public bool IsConnected
	{
		get { return client.IsConnected; }
	}
}