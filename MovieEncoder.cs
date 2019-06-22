using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BatchEncoder
{
	public class EncodeSettings
	{
		public string input;
		public string videoCodec;
		public string videoBitrate;
		public string framerate;
		public string videoSize;
		public string audioCodec;
		public string startSec;
		public string duration;
		public bool concatenate;
		public string output;
	}

	public static class MovieEncoder
	{
		static readonly Queue<EncodeSettings> Queue = new Queue<EncodeSettings>();
		static bool running;

		public static void AddQueue(Queue<EncodeSettings> newQueue, bool concatenate)
		{
			if (concatenate)
			{
				var queue = CreateConcatQueue(newQueue);
				Queue.Enqueue(queue);
			}
			else
			{
				foreach (var item in newQueue) Queue.Enqueue(item);
			}

			ProcessNextQueue(default, default);
		}

		static EncodeSettings CreateConcatQueue(Queue<EncodeSettings> newQueue)
		{
			var concat = new StringBuilder();
			foreach (var q in newQueue) concat.AppendLine("file '" + q.input + "'");

			var fileList = Path.Combine(Directory.GetCurrentDirectory(), "concat.txt");
			File.WriteAllText(fileList, concat.ToString());

			var queue = newQueue.Dequeue();
			queue.input = fileList;
			return queue;
		}

		static void ProcessNextQueue(object sender, EventArgs eventArgs)
		{
			if (Queue.Count == 0)
			{
				running = false;
				return;
			}

			var encoder = Run(Queue.Dequeue());
			running = true;
			encoder.EnableRaisingEvents = true;
			encoder.Exited += ProcessNextQueue;
		}

		static Process Run(EncodeSettings settings)
		{
			var encoder = new Process {StartInfo = {FileName = "ffmpeg"}};

			var arguments = new ArgumentsComposer();

			arguments.Add($"-f concat -safe 0", settings.concatenate);
			arguments.Add($"-i \"{settings.input}\"");
			arguments.Add($"-vcodec {settings.videoCodec}");
			arguments.Add($"-b:v {settings.videoBitrate}", settings.videoBitrate);
			arguments.Add($"-r {settings.framerate}", settings.framerate);
			arguments.Add($"-s {settings.videoSize}", settings.videoSize);
			arguments.Add($"-acodec {settings.audioCodec}");
			arguments.Add($"-ss {settings.startSec}", settings.startSec);
			arguments.Add($"-t {settings.duration}", settings.duration);
			var extension = ExtensionChecker.GetAttributedExtension(settings);
			arguments.Add($"\"{Path.ChangeExtension(settings.output, extension)}\"");

			encoder.StartInfo.Arguments = arguments.ToString();
			encoder.Start();
			return encoder;
		}
	}
}