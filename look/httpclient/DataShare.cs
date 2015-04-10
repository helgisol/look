using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace httpclient
{
	public class DataShare
	{
        public enum DataShareState { Worked, Error };

        public enum DataShareInternalState { Worked, Error, Reconnect };

        public DataShareInternalState InternalState { get; set; }

        private DataShareState ExternalState
        {
            get
            {
                if (InternalState == DataShareInternalState.Worked)
                {
                    return DataShareState.Worked;
                }
                else
                {
                    return DataShareState.Error;
                }
            }
        }

        private string LookGUID { get; set; }

        private string LookVersion { get; set; }

        private List<string> DataList { get; set; }

        private DataShareTaskState TaskState { get; set; }

        private Task<bool> SendingTask { get; set; }

        private Timer DelayTimer { get; set; }

        private CancellationTokenSource cts;

		public DataShare(
			string lookGUID,
			string lookVersion,
			string redirectorSrvUri)
		{
            InternalState = DataShareInternalState.Worked;
            LookGUID = lookGUID;
            LookVersion = lookVersion;
            DataList = new List<string>();
            TaskState = new DataShareTaskState(redirectorSrvUri);
            cts = new CancellationTokenSource(10 * 60 * 1000);
            SendingTask = new Task<bool>(() => DataShareTaskCode.TaskProccessData(cts.Token, TaskState), cts.Token);
            //Task cwt = SendingTask.ContinueWith(task => new Timer(DelayTimerCallback, null, 1 * 1 * 1000, Timeout.Infinite)); 
        }

		private string CreateDataShareDataStr(string nowDataStr, List<DataShareImage> images)
		{
			DataContractJsonSerializer jsonDataShareMessage = new DataContractJsonSerializer(typeof(List<DataShareImage>));
			MemoryStream streamDataShareImages = new MemoryStream();
			jsonDataShareMessage.WriteObject(streamDataShareImages, images);
			string dataShareImagesStr = Encoding.UTF8.GetString(streamDataShareImages.ToArray());

			//Console.WriteLine("\ndataShareImagesStr: " + dataShareImagesStr);

			return string.Format("{{now:{0},imgs:{1}}}", nowDataStr, dataShareImagesStr);
		}

        private string CreateMessage()
        {
            return string.Format("{{look:\"{0}\",ver:\"{1}\",data:[{2}]}}", LookGUID, LookVersion, String.Join(",", DataList));
        }

        private void DelayTimerCallback(Object aState)
        {
            InternalState = DataShareInternalState.Reconnect;
            SendingTask = new Task<bool>(() => DataShareTaskCode.TaskProccessData(cts.Token, TaskState), cts.Token);
            SendingTask.Start();
        }

        public DataShareState ProccessData(string nowDataStr, List<DataShareImage> images)
		{
            if (InternalState == DataShareInternalState.Error)
            {
                return ExternalState;
            }

            if (DelayTimer != null)
            {
                DelayTimer.Dispose();
                DelayTimer = null;
            }

            if (SendingTask.Status == TaskStatus.Faulted || SendingTask.Status == TaskStatus.Canceled || (SendingTask.Status == TaskStatus.RanToCompletion && !SendingTask.Result))
            {
                InternalState = DataShareInternalState.Error;
                DelayTimer = new Timer(DelayTimerCallback, null, 1 * 1 * 1000, Timeout.Infinite);
                return ExternalState;
            }

            if (InternalState == DataShareInternalState.Reconnect)
            {
                if (SendingTask.Status != TaskStatus.RanToCompletion)
                {
                    return ExternalState;
                }
                else
                {
                    InternalState = DataShareInternalState.Worked;
                }
            }

            if (String.IsNullOrEmpty(nowDataStr))
            {
                if (DataList.Count == 0)
                {
                    return ExternalState;
                }
            }
            else
            {
                if (DataList.Count < 10000)
                {
                    string dataShareDataStr = CreateDataShareDataStr(nowDataStr, images);
                    //Console.WriteLine("\ndataShareDataStr: " + dataShareDataStr);
                    DataList.Add(dataShareDataStr);
                }
            }

            if (SendingTask.Status == TaskStatus.Running)
            {
                return ExternalState;
            }

            if (SendingTask.Status == TaskStatus.RanToCompletion)
            {
                SendingTask = new Task<bool>(() => DataShareTaskCode.TaskProccessData(cts.Token, TaskState), cts.Token);
            }

            if (SendingTask.Status == TaskStatus.Created)
            {
                TaskState.Message = CreateMessage();
                DataList.Clear();
                SendingTask.Start();
            }

            return ExternalState;
		}


        public void Stop()
        {
            cts.Cancel();
        }

		internal TaskStatus GetTaskStatus()
		{
			return SendingTask.Status;
        }

		public void Dispose()
		{
			TaskState.Dispose();
        }
	}
}
