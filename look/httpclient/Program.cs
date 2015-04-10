using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Threading;

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

            TaskStatus taskStatus = TaskStatus.Faulted;
            for (int i = 0; i < 100000; i++)
			{
				//Console.WriteLine("i: " + i + ", dataShare.LookBlock: " + dataShare.LookBlock);
				//Console.Write(i + ", ");
                Thread.Sleep(100);

				if (i % 100 == 0)
				{
                    Console.WriteLine("\n*i: " + i + ", taskStatus: " + taskStatus + ", InternalState: " + dataShare.InternalState);
					dataShare.ProccessData("aaa", new List<DataShareImage> { });
				}
				else
				{
					dataShare.ProccessData("", new List<DataShareImage> { });
				}

				TaskStatus taskStatusNew = dataShare.GetTaskStatus();
                if (/*statusNew == TaskStatus.RanToCompletion ||*/ taskStatusNew != taskStatus)
				{
                    Console.WriteLine("\ni: " + i + ", taskStatus: " + taskStatus + ", taskStatusNew: " + taskStatusNew);
				}
                taskStatus = taskStatusNew;
            }

			dataShare.Dispose();

			Console.WriteLine("Press a key to quit.");
			Console.ReadLine();
        }
    }
}