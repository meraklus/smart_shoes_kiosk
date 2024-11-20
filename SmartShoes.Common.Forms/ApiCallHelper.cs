using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartShoes.Common.Forms
{
	public class ApiCallHelper
	{

		private readonly HttpClient _httpClient;

		public ApiCallHelper()
		{
			_httpClient = new HttpClient();
		}

		public async Task<string> GetAsync(string url)
		{
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync(url);
				response.EnsureSuccessStatusCode();
				return await response.Content.ReadAsStringAsync();
			}
			catch (HttpRequestException e)
			{
				// 예외 처리 로직 추가
				throw new Exception($"Request error: {e.Message}");
			}
		}

		public async Task<string> PostAsync(string url, object postData)
		{
			try
			{
				string json = JsonConvert.SerializeObject(postData);
				StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
				HttpResponseMessage response = await _httpClient.PostAsync(url, content);
				response.EnsureSuccessStatusCode();
				return await response.Content.ReadAsStringAsync();
			}
			catch (HttpRequestException e)
			{
				// 예외 처리 로직 추가
				throw new Exception($"Request error: {e.Message}");
			}
		}

	}
}
