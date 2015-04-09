using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace httpclient
{
	class DataShareTaskState
	{
		public string LookGUID { get; }

		public string LookVersion { get; }

		public string RedirectorSrvUri { get; }

		public List<string> DataList { get; set; }

		public string CloudSrvUri { get; set; }

		public HttpClient CloudClient { get; set; }

		public DataShareTaskState(string lookGUID, string lookVersion, string redirectorSrvUri)
		{
			LookGUID = lookGUID;
            LookVersion = lookVersion;
			RedirectorSrvUri = redirectorSrvUri;
			DataList = new List<string>();
			CloudClient = new HttpClient();
        }
	}
}
