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
		static void Main(string[] args)
        {
			DataShare dataShare = new DataShare(
				Guid.NewGuid().ToString(),
				"1.0.free",
				"http://localhost:8081/datashareaddress/");

			TaskStatus status = TaskStatus.Faulted;
            for (int i = 0; i < 1000; i++)
			{
				//Console.WriteLine("i: " + i + ", dataShare.LookBlock: " + dataShare.LookBlock);
				Console.Write(i + ", ");

				if (i % 100 == 0)
				{
					Console.WriteLine("\n*i: " + i + ", status: " + status);
					dataShare.ProccessData("aaa", new List<DataShareImage> { });
				}
				else
				{
					dataShare.ProccessData("", new List<DataShareImage> { });
				}

				TaskStatus statusNew = dataShare.GetTaskStatus();
				if (/*statusNew == TaskStatus.RanToCompletion ||*/ statusNew != status)
				{
					Console.WriteLine("\ni: " + i + ", status: " + status + ", statusNew: " + statusNew);
				}
				status = statusNew;
            }

			dataShare.Dispose();

			Console.WriteLine("Press a key to quit.");
			Console.ReadLine();
        }
    }
}