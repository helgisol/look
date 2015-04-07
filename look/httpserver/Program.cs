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
			string result = string.Format("{{\"host\":\"{0}\", \"port\":{1}}}", cloudSrvHost, cloudSrvPort);
			Console.WriteLine(result);
			return result;
		}
		public static string CloudResponse(HttpListenerRequest request)
		{
			if (request.HttpMethod != "POST" || !request.HasEntityBody)
			{
				return "Error";
			}

			string msg;
			using (System.IO.Stream body = request.InputStream)
			{
				using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
				{
					msg = reader.ReadToEnd();
				}
			}
			Console.WriteLine("cloud msg: " + msg);

			return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
		}

		static void Main(string[] args)
		{
			//WebServer ws = new WebServer(SampleSendResponse, "http://localhost:8080/test/");
			//ws.Run();
			//Console.WriteLine("A simple webserver. Press a key to quit.");
			//Console.ReadKey();
			//ws.Stop();

			WebServer redirectorSrv = new WebServer(
				RedirectorResponse,
				"application/json; charset=utf-8",
				"http://localhost:8081/datashareaddress/");
			Console.WriteLine("Redirector webserver starting.");
			redirectorSrv.Run();

			WebServer cloudSrv = new WebServer(
				CloudResponse,
				"text/html",
				string.Format("http://{0}:{1}/datashare/", cloudSrvHost, cloudSrvPort));
			Console.WriteLine("Cloud webserver starting.");
			cloudSrv.Run();

			Console.WriteLine("Press a key to quit.");
			Console.ReadKey();

			redirectorSrv.Stop();
			cloudSrv.Stop();
		}
	}
}