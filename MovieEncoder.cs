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
		public bool yuv420p;
		public bool simultaneously;
		public string audioCodec;
		public string startSec;
		public string duration;
		public bool concatenate;
		public string output;
	}

	public static class MovieEncoder
	{
		static readonly Queue<EncodeSettings> Queue = new Queue<EncodeSettings>();
		static int EncoderCount;

		static int EmptySlot
		{
			get
			{
				if (Queue.Count == 0) return 0;

				var simultaneously = Queue.Peek().simultaneously;
				var maxSlot = simultaneously ? 2 : 1;
				return maxSlot - EncoderCount;
			}
		}

		static bool Runnable
		{
			get
			{
				if (Queue.Count == 0) return false;
				return EmptySlot > 0;
			}
		}

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
			if (!Runnable) return;

			var emptySlot = EmptySlot;
			for (var i = 0; i < emptySlot; i++)
			{
				if (Queue.Count == 0) break;

				var arguments = CreateArguments(Queue.Dequeue());
				var encoder = Run(arguments);
				++EncoderCount;
				encoder.EnableRaisingEvents = true;
				encoder.Exited += EncoderOnExited;
			}
		}

		public static string CreateArguments(EncodeSettings settings)
		{
			var arguments = new ArgumentsComposer();

			arguments.Add($"-f concat -safe 0", settings.concatenate);
			arguments.Add($"-ss {settings.startSec}", settings.startSec);
			arguments.Add($"-i \"{settings.input}\"");
			arguments.Add($"-vcodec {settings.videoCodec}");
			arguments.Add($"-b:v {settings.videoBitrate}", settings.videoBitrate);
			arguments.Add($"-r {settings.framerate}", settings.framerate);
			arguments.Add($"-s {settings.videoSize}", settings.videoSize);
			arguments.Add($"-pix_fmt yuv420p", settings.yuv420p);
			arguments.Add($"-acodec {settings.audioCodec}");
			arguments.Add($"-t {settings.duration}", settings.duration);
			var extension = ExtensionChecker.GetAttributedExtension(settings);
			arguments.Add($"\"{Path.ChangeExtension(settings.output, extension)}\"");

			return arguments.ToString();
		}

		static Process Run(string arguments)
		{
			var encoder = new Process {StartInfo = {FileName = "ffmpeg"}};
			encoder.StartInfo.Arguments = arguments;
			encoder.Start();
			return encoder;
		}

		static void EncoderOnExited(object sender, EventArgs e)
		{
			--EncoderCount;
			ProcessNextQueue(sender, e);
		}


		public static void RemoveConcatFile()
		{
			var path = Path.Combine(Directory.GetCurrentDirectory(), "concat.txt");
			File.Delete(path);
		}
	}
}