using System;
using System.Net;

namespace httpserver
{
	class Program
	{
		static string cloudSrvHost = "localhost";
		static int cloudSrvPort = 8082;

		//public static string SampleSendResponse(HttpListenerRequest request)
		//{
		//	return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
		//}

		public static string RedirectorResponse(HttpListenerRequest request)
		{
			string temp = string.Format("{0}\"host\":\"{2}\", \"port\":{3}{1}", "{", "}", cloudSrvHost, cloudSrvPort);
			Console.WriteLine(temp);
			return temp;
		}
		public static string CloudResponse(HttpListenerRequest request)
		{
			return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
		}

		static void Main(string[] args)
		{
			//WebServer ws = new WebServer(SampleSendResponse, "http://localhost:8080/test/");
			//ws.Run();
			//Console.WriteLine("A simple webserver. Press a key to quit.");
			//Console.ReadKey();
			//ws.Stop();

			WebServer redirectorSrv = new WebServer(RedirectorResponse, "http://localhost:8081/datashareaddress/");
			Console.WriteLine("Redirector webserver starting.");
			redirectorSrv.Run();

			//WebServer cloudSrv = new WebServer(CloudResponse, string.Format("http://{0}:{1}/datashare/", cloudSrvHost, cloudSrvPort));
			//Console.WriteLine("Cloud webserver starting.");
			//cloudSrv.Run();

			Console.WriteLine("Press a key to quit.");
			Console.ReadKey();

			redirectorSrv.Stop();
			//cloudSrv.Stop();
		}
	}
}