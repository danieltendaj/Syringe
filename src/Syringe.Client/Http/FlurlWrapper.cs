using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Syringe.Client.Http
{
	public class FlurlWrapper
	{
		private IFlurlClient _client;
		public Flurl.Url Url { get; set; }

		public FlurlWrapper(CustomHttpClientFactory httpClientFactory, string controllerPath)
		{
			_client = Url.ConfigureClient(x =>
			{
				x.CookiesEnabled = true;
				x.HttpClientFactory = httpClientFactory;
			});

			Url = new Url(httpClientFactory.BaseUrl);
			Url = Url.AppendPathSegment(controllerPath);
		}

		public async Task<T> Get<T>(string action)
		{
			return await Url.GetJsonAsync<T>();
		}

		public async Task<T> Post<T>(string action, object body)
		{
			return await Url.PostJsonAsync(body).ReceiveJson<T>();
		}

		public async Task<T> Patch<T>(string action, object body)
		{
			return await Url.PatchJsonAsync(body).ReceiveJson<T>();
		}

		public async Task<T> Delete<T>(string action)
		{
			return await Url.DeleteAsync().ReceiveJson<T>();
		}

		public void AddParameter(string name, string value)
		{
			Url = Url.SetQueryParam(name, value);
		}
	}
}