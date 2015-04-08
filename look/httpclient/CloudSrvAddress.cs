using System.Runtime.Serialization;

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
}
