﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace httpclient
{
	public class DataShare
	{
		public enum LookLicenseType { Free, Pro };

		public enum LookBlockStatus { Block, Unblock };

		private LookLicenseType LookLicense { get; set; }

		private List<string> DataList { get; set; }

		private LookBlockStatus LookBlock { get; set; }

		private DataShareTaskState taskState;

		private Task<bool> task;

		public DataShare(
			LookLicenseType lookLicense,
			string lookGUID,
			string lookVersion,
			string redirectorSrvUri)
		{
			LookLicense = lookLicense;
			LookBlock = LookBlockStatus.Unblock;
			DataList = new List<string>();
            taskState = new DataShareTaskState(lookGUID, lookVersion, redirectorSrvUri);
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

		public LookBlockStatus ProccessData(string nowDataStr, List<DataShareImage> images)
		{
			if (LookBlock == LookBlockStatus.Block)
			{
				return LookBlock;
			}

			if (((task.Status == TaskStatus.RanToCompletion && !task.Result) ||
				task.Status == TaskStatus.Faulted || task.Status == TaskStatus.Canceled) && LookLicense == LookLicenseType.Free)
			{
				LookBlock = LookBlockStatus.Block;
				return LookBlock;
			}

			if (String.IsNullOrEmpty(nowDataStr))
			{
				if (DataList.Count == 0)
				{
					return LookBlock;
				}
			}
			else
			{
				string dataShareDataStr = CreateDataShareDataStr(nowDataStr, images);

				//Console.WriteLine("\ndataShareDataStr: " + dataShareDataStr);

				DataList.Add(dataShareDataStr);
			}

			if (task.Status == TaskStatus.Running)
			{
				return LookBlock;
			}

			if (task.Status == TaskStatus.RanToCompletion)
			{
				task = new Task<bool>(state => DataShareTaskCode.TaskProccessData((DataShareTaskState)state), taskState);
			}

			if (task.Status == TaskStatus.Created)
			{
				taskState.DataList.Clear();
				taskState.DataList.AddRange(DataList);
				DataList.Clear();

				task.Start();
			}

			return LookBlock;
		}

		public TaskStatus GetTaskStatus()
		{
			return task.Status;
        }

		public void Dispose()
		{
			taskState.CloudClient.Dispose();
        }
	}
}
