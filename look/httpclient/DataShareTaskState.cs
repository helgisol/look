using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace httpclient
{
	internal class DataShareTaskState
	{
		internal string RedirectorSrvUri { get; }

        internal string CloudSrvUri { get; set; }

        internal HttpClient CloudClient { get; set; }

        public string Message { get; set; }

        public DataShareTaskState(string redirectorSrvUri)
		{
			RedirectorSrvUri = redirectorSrvUri;
			CloudClient = new HttpClient();
        }

        public void Dispose()
        {
            CloudClient.Dispose();
        }
    }
}
