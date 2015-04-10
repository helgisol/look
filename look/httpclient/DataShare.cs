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

        private enum DataShareInternalState { Worked, Error, Reconnect };

        private DataShareInternalState InternalState { get; set; }

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

        private string LookGUID { get; }

        private string LookVersion { get; }

        private List<string> DataList { get; set; }

        private DataShareTaskState taskState;

		private Task<bool> task;

        //private Timer delayTimer;

		public DataShare(
			string lookGUID,
			string lookVersion,
			string redirectorSrvUri)
		{
            InternalState = DataShareInternalState.Worked;
            LookGUID = lookGUID;
            LookVersion = lookVersion;
            DataList = new List<string>();
            taskState = new DataShareTaskState(redirectorSrvUri);
			task = new Task<bool>(state => DataShareTaskCode.TaskProccessData((DataShareTaskState)state), taskState);
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

        private void DelayTimerCallback(Object state)
        {
            //State = DataShareState.Worked;
        }

        static private string CreateMessage(List<string> DataList)
        {
            return "";
        }

        public DataShareState ProccessData(string nowDataStr, List<DataShareImage> images)
		{
            if (InternalState == DataShareInternalState.Error)
            {
                return ExternalState;
            }
            else if (InternalState == DataShareInternalState.Error || InternalState == DataShareInternalState.Reconnect)
            {
                return ExternalState;
            }

            //if (delayTimer != null)
            //{
            //    delayTimer.Dispose();
            //    delayTimer = null;
            //}



            //         if (((task.Status == TaskStatus.RanToCompletion && !task.Result) ||
            //	task.Status == TaskStatus.Faulted || task.Status == TaskStatus.Canceled) && LookLicense == LookLicenseType.Free)
            //{
            //	LookBlock = LookBlockStatus.Block;
            //	return LookBlock;
            //}

            //if (String.IsNullOrEmpty(nowDataStr))
            //{
            //	if (DataList.Count == 0)
            //	{
            //		return LookBlock;
            //	}
            //}
            //else
            //{
            //	string dataShareDataStr = CreateDataShareDataStr(nowDataStr, images);

            //	//Console.WriteLine("\ndataShareDataStr: " + dataShareDataStr);

            //	DataList.Add(dataShareDataStr);
            //}

            //if (task.Status == TaskStatus.Running)
            //{
            //	return LookBlock;
            //}

            //if (task.Status == TaskStatus.RanToCompletion)
            //{
            //	task = new Task<bool>(state => DataShareTaskCode.TaskProccessData((DataShareTaskState)state), taskState);
            //}

            //if (task.Status == TaskStatus.Created)
            //{
            //	taskState.DataList.Clear();
            //	taskState.DataList.AddRange(DataList);
            //	DataList.Clear();

            //	task.Start();
            //         }

            return ExternalState;
		}

		internal TaskStatus GetTaskStatus()
		{
			return task.Status;
        }

		public void Dispose()
		{
			taskState.Dispose();
        }
	}
}
