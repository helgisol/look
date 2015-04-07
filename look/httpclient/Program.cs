using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;

namespace httpclient
{
	[DataContract]
	internal class CloudSrvAddress
	{
		[DataMember]
		internal string host;

		[DataMember]
		internal int port;
	}

	class Program
    {
        //static async void DownloadPageAsync()
        //{
        //    // ... Target page.
        //    const string page = "http://en.wikipedia.org/";

        //    // ... Use HttpClient.
        //    using (HttpClient client = new HttpClient())
        //    using (HttpResponseMessage response = await client.GetAsync(page))
        //    using (HttpContent content = response.Content)
        //    {
        //        // ... Read the string.
        //        string result = await content.ReadAsStringAsync();

        //        // ... Display the result.
        //        if (result != null && result.Length >= 50)
        //        {
        //            Console.WriteLine(result.Substring(0, 500) + "...");
        //        }
        //    }
        //}

		static async Task<string> ReadFromRedirectorSrvAsync(string uri)
		{
			// ... Target page.
			//const string uri = "http://localhost:8081/datashareaddress/";

			// ... Use HttpClient.
			using (HttpClient client = new HttpClient())
			using (HttpResponseMessage response = await client.GetAsync(uri))
			using (HttpContent content = response.Content)
			{
				// ... Read the string.
				string result = await content.ReadAsStringAsync();

				// ... Display the result.
				if (result != null)
				{
					//Console.WriteLine(result);
				}

				return result;
			}
		}

		static void Main(string[] args)
        {
			//Task t = new Task(DownloadPageAsync);
			//t.Start();

			Task<string> t = ReadFromRedirectorSrvAsync("http://localhost:8081/datashareaddress/");
			t.Wait();
			string cloudSrvAddressStr = t.Result;
            Console.WriteLine("cloudSrvAddressStr: " + cloudSrvAddressStr);

			DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(CloudSrvAddress));
			CloudSrvAddress address = (CloudSrvAddress)json.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(cloudSrvAddressStr)));

			Console.WriteLine("address.host: " + address.host.ToString() + ", address.port: " + address.port.ToString());

			//Console.WriteLine("Downloading page...");
			Console.ReadLine();
        }
    }
}