using System;
using System.Runtime.Serialization;

namespace httpclient
{
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
}
