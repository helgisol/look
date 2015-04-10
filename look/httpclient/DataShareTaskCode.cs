using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
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
				HttpResponseMessage resp = await client.PostAsync(uri, httpContent);
				return resp.IsSuccessStatusCode;
			}
		}

        public static bool TaskProccessData(CancellationToken ct, DataShareTaskState state)
		{
            const int redirectorDelayMs = 1 * 1000;
            const int cloudDelayMs = 1 * 1000;
            const int redirectorAttemptCount = 1;
            const int cloudAttemptCount = 1;

            if (String.IsNullOrEmpty(state.CloudSrvUri))
			{
				for (int i = 0; i < redirectorAttemptCount; i++)
				{
                    ct.ThrowIfCancellationRequested();
					DateTime startTime = DateTime.Now;
					try
					{
						Task<string> redirectorTask = ReadCloudSrvUrl(state.RedirectorSrvUri);
						redirectorTask.Wait();
						state.CloudSrvUri = redirectorTask.Result;
						break;
					}
					catch
					{
						if (i == redirectorAttemptCount - 1)
						{
							return false;
						}
						TimeSpan timeDiff = DateTime.Now - startTime;
						int timeDiffMs = (int)timeDiff.TotalMilliseconds;
                        int delay = redirectorDelayMs - timeDiffMs;
                        if (delay > 0)
                        {
                            Thread.Sleep(delay);
                        }
					}
				}
            }

            for (int i = 0; i < cloudAttemptCount; i++)
            {
                DateTime startTime = DateTime.Now;
                try
                {
                    ct.ThrowIfCancellationRequested();
                    Task<bool> cloudTask = SendMessagrToCloudSrvAsync(state.CloudClient, state.CloudSrvUri, state.Message);
                    cloudTask.Wait();
                    bool cloudResult = cloudTask.Result;
                    //Console.WriteLine("cloudResult: " + cloudResult);
                    return cloudResult;
                }
                catch
                {
                    if (i == cloudAttemptCount - 1)
                    {
                        return false;
                    }
                    if (i == 0)
                    {
                        try
                        {
                            Task<string> redirectorTask = ReadCloudSrvUrl(state.RedirectorSrvUri);
                            redirectorTask.Wait();
                            state.CloudSrvUri = redirectorTask.Result;
                        }
                        catch
                        {
                        }
                    }
                    TimeSpan timeDiff = DateTime.Now - startTime;
                    int timeDiffMs = (int)timeDiff.TotalMilliseconds;
                    int delay = cloudDelayMs - timeDiffMs;
                    if (delay > 0)
                    {
                        Thread.Sleep(delay);
                    }
                }
            }

			return false;
		}

	}
}
