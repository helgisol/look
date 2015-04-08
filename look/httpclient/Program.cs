using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace httpclient
{
	class Program
    {
		static async Task<string> ReadFromRedirectorSrvAsync(string uri)
		{
			using (HttpClient client = new HttpClient())
			{
				return await client.GetStringAsync(uri);
            }
		}

		static async Task<bool> SendMessagrToCloudSrvAsync(string uri, string msg)
		{
			using (HttpClient client = new HttpClient())
			using (HttpContent httpContent = new StringContent(msg, Encoding.UTF8, "application/json"))
			{
				HttpResponseMessage x = await client.PostAsync(uri, httpContent);
				return x.IsSuccessStatusCode;
			}
		}

		static void Main(string[] args)
        {
			DataShare dataShare = new DataShare(
				DataShare.LookLicenseType.Free,
				Guid.NewGuid().ToString(),
				"1.0.free",
				"http://localhost:8081/datashareaddress/");

			for (int i = 0; i < 10000; i++)
			{
				//Console.WriteLine("i: " + i + ", dataShare.LookBlock: " + dataShare.LookBlock);
				Console.Write(i);

				if (i % 100 == 0)
				{
					dataShare.ProccessData("aaa", new List<DataShareImage> { });
				}

			}

			dataShare.Dispose();


			//Task <string> redirectorTask = ReadFromRedirectorSrvAsync("http://localhost:8081/datashareaddress/");
			//redirectorTask.Wait();
			//string cloudSrvAddressStr = redirectorTask.Result;
			//Console.WriteLine("cloudSrvAddressStr: " + cloudSrvAddressStr);

			//DataContractJsonSerializer jsonDataShareData = new DataContractJsonSerializer(typeof(CloudSrvAddress));
			//CloudSrvAddress address = (CloudSrvAddress)jsonDataShareData.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(cloudSrvAddressStr)));

			//Console.WriteLine("address.host: " + address.Host.ToString() + ", address.port: " + address.Port.ToString());

			//DataShareMessage msg = new DataShareMessage();
			//DataContractJsonSerializer jsonDataShareMessage = new DataContractJsonSerializer(typeof(DataShareMessage));
			//MemoryStream streamDataShareMessage = new MemoryStream();
			//jsonDataShareMessage.WriteObject(streamDataShareMessage, msg);
			//string dataShareMessageStr = Encoding.UTF8.GetString(streamDataShareMessage.ToArray());
			//Console.WriteLine("\ndataShareMessageStr: " + dataShareMessageStr);


			//Task<bool> cloudTask = SendMessagrToCloudSrvAsync(
			//	string.Format("http://{0}:{1}/datashare/", address.Host, address.Port),
			//	dataShareMessageStr);
			//cloudTask.Wait();
			//bool cloudResult = cloudTask.Result;
			//Console.WriteLine("cloudResult: " + cloudResult);


			Console.WriteLine("Press a key to quit.");
			Console.ReadLine();
        }
    }
}