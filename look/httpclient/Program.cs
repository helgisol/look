using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace httpclient
{
	[DataContract]
	internal class CloudSrvAddress
	{
		[DataMember(Name = "host")]
		internal string Host { get; set; }

		[DataMember(Name = "port")]
		internal int Port { get; set; }
	}

	[DataContract]
	internal class NowObject
	{
		[DataMember]
		internal int id = 0;

		[DataMember]
		internal int age = 0;

		[DataMember]
		internal double age_confidence = 0.0;

		[DataMember]
		internal string gender = "male";

		[DataMember]
		internal double gender_confidence = 0.0;

		[DataMember]
		internal string race = "white";
	}

	[DataContract]
	internal class DataShareNow
	{
		[DataMember]
		internal string timestamp = DateTimeOffset.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");

		[DataMember]
		internal Guid session = Guid.NewGuid();

		[DataMember]
		internal string primary_gender = "mail";

		[DataMember]
		internal string majority_gender = "mail";

		[DataMember]
		internal string majority_emotion = "neutral";

		[DataMember]
		internal int main_person = 0;

		[DataMember]
		internal NowObject[] objects = { new NowObject(), new NowObject() };
	}

	[DataContract]
	internal class DataShareImage
	{
		[DataMember]
		internal int id = 0;

		[DataMember]
		internal string img = "1234567890";
	}

	[DataContract]
	internal class DataShareData
	{
		[DataMember]
		internal DataShareNow now = new DataShareNow();

		[DataMember]
		internal DataShareImage[] imgs = { new DataShareImage() };
	}

	[DataContract]
	internal class DataShareMessage
	{
		[DataMember]
		internal Guid look = Guid.NewGuid();

		[DataMember(Name = "ver")]
		internal string ver = "1.0.free";

		[DataMember]
		internal DataShareData[] data = { new DataShareData(), new DataShareData() };
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
			//Task t = new Task(DownloadPageAsync);
			//t.Start();

			Task<string> redirectorTask = ReadFromRedirectorSrvAsync("http://localhost:8081/datashareaddress/");
			redirectorTask.Wait();
			string cloudSrvAddressStr = redirectorTask.Result;
            Console.WriteLine("cloudSrvAddressStr: " + cloudSrvAddressStr);

			DataContractJsonSerializer jsonDataShareData = new DataContractJsonSerializer(typeof(CloudSrvAddress));
			CloudSrvAddress address = (CloudSrvAddress)jsonDataShareData.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(cloudSrvAddressStr)));

			Console.WriteLine("address.host: " + address.Host.ToString() + ", address.port: " + address.Port.ToString());

			DataShareMessage msg = new DataShareMessage();
			DataContractJsonSerializer jsonDataShareMessage = new DataContractJsonSerializer(typeof(DataShareMessage));
			MemoryStream streamDataShareMessage = new MemoryStream();
			jsonDataShareMessage.WriteObject(streamDataShareMessage, msg);
			string dataShareMessageStr = Encoding.UTF8.GetString(streamDataShareMessage.ToArray());
			Console.WriteLine("\ndataShareMessageStr: " + dataShareMessageStr);


			Task<bool> cloudTask = SendMessagrToCloudSrvAsync(
				string.Format("http://{0}:{1}/datashare/", address.Host, address.Port),
				dataShareMessageStr);
			cloudTask.Wait();
			bool cloudStr = cloudTask.Result;
			Console.WriteLine("cloudStr: " + cloudStr);



			//Console.WriteLine("Downloading page...");
			Console.ReadLine();
        }
    }
}