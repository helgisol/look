using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace httpclient
{
	class DataShareTaskCode
	{
		static async Task<string> ReadCloudSrvUrl(string redirectorSrvUri)
		{
			string cloudSrvAddressStr;
            using (HttpClient redirectorClient = new HttpClient())
			{
				cloudSrvAddressStr = await redirectorClient.GetStringAsync(redirectorSrvUri);
			}

			//Console.WriteLine("cloudSrvAddressStr: " + cloudSrvAddressStr);

			DataContractJsonSerializer jsonDataShareData = new DataContractJsonSerializer(typeof(CloudSrvAddress));
			CloudSrvAddress address = (CloudSrvAddress)jsonDataShareData.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(cloudSrvAddressStr)));

			//Console.WriteLine("address.host: " + address.Host.ToString() + ", address.port: " + address.Port.ToString());

			return string.Format("http://{0}:{1}/datashare/", address.Host, address.Port);
        }

		static async Task<bool> SendMessagrToCloudSrvAsync(HttpClient client, string uri, string msg)
		{
			using (HttpContent httpContent = new StringContent(msg, Encoding.UTF8, "application/json"))
			{
				HttpResponseMessage x = await client.PostAsync(uri, httpContent);
				return x.IsSuccessStatusCode;
			}
		}

		public static bool TaskProccessData(DataShareTaskState state)
		{
			if (String.IsNullOrEmpty(state.CloudSrvUri))
			{
				Task<string> redirectorTask = ReadCloudSrvUrl(state.RedirectorSrvUri);
				redirectorTask.Wait();
				state.CloudSrvUri = redirectorTask.Result;
			}

			foreach (string dataStr in state.DataList)
			{
				string msg = string.Format("{{look:\"{0}\",ver:\"{1}\",data:{2}}}", state.LookGUID, state.LookVersion, dataStr);

				Task<bool> cloudTask = SendMessagrToCloudSrvAsync(state.CloudClient, state.CloudSrvUri, msg);
				cloudTask.Wait();
				bool cloudResult = cloudTask.Result;

				//Console.WriteLine("cloudResult: " + cloudResult);

				if (!cloudResult)
				{
					return false;
				}
			}

			return true;
		}

	}
}
