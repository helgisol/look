using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace httpclient
{
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

		static async void ReadFromRedirectorSrvAsync()
		{
			// ... Target page.
			const string uri = "http://localhost:8081/datashareaddress/";

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
					//Console.WriteLine(result.Substring(0, 500) + "...");
					Console.WriteLine(result);
				}
			}
		}

		static void Main(string[] args)
        {
			//Task t = new Task(DownloadPageAsync);
			//t.Start();

			ReadFromRedirectorSrvAsync();

			//Console.WriteLine("Downloading page...");
			Console.ReadLine();
        }
    }
}