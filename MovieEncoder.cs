using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BatchEncoder
{
	public class EncodeSettings
	{
		public string path;
		public string videoCodec;
		public string videoBitrate;
		public string framerate;
		public string videoSize;
		public string audioCodec;
		public string startSec;
		public string duration;
	}

	public static class MovieEncoder
	{
		static readonly Queue<EncodeSettings> Queue = new Queue<EncodeSettings>();

		public static void AddQueue(Queue<EncodeSettings> newQueue)
		{
			foreach (var item in newQueue) Queue.Enqueue(item);
			ProcessNextQueue(default, default);
		}

		static void ProcessNextQueue(object sender, EventArgs eventArgs)
		{
			if (Queue.Count == 0) return;

			var encoder = Run(Queue.Dequeue());
			encoder.EnableRaisingEvents = true;
			encoder.Exited += ProcessNextQueue;
		}

		static Process Run(EncodeSettings settings)
		{
			var encoder = new Process {StartInfo = {FileName = "ffmpeg"}};

			var arguments = new ArgumentsComposer();

			arguments.Add($"-i \"{settings.path}\"");
			arguments.Add($"-vcodec {settings.videoCodec}");
			arguments.Add($"-b:v {settings.videoBitrate}", settings.videoBitrate);
			arguments.Add($"-r {settings.framerate}", settings.framerate);
			arguments.Add($"-s {settings.videoSize}", settings.videoSize);
			arguments.Add($"-acodec {settings.audioCodec}");
			arguments.Add($"-ss {settings.startSec}", settings.startSec);
			arguments.Add($"-t {settings.duration}", settings.duration);
			var extension = ExtensionChecker.IsSameExtension(settings.path) ? "Encoded.mp4" : "mp4";
			arguments.Add($"\"{Path.ChangeExtension(settings.path, extension)}\"");

			encoder.StartInfo.Arguments = arguments.ToString();
			encoder.Start();
			return encoder;
		}
	}
}