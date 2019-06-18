using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BatchEncoder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		Queue<EncodeSettings> queue = new Queue<EncodeSettings>();

		public MainWindow()
		{
			InitializeComponent();
		}

		void MainWindow_OnDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.All;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}

		void MainWindow_OnDrop(object sender, DragEventArgs e)
		{
			var files = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
			foreach (var file in files)
			{
				var settings = new EncodeSettings
				{
					path = file,
					videoCodec = VideoCodec.Text,
					videoBitrate = VideoBitrate.Text,
					framerate = Framerate.Text,
					videoSize = (string.IsNullOrEmpty(Width.Text) || string.IsNullOrEmpty(Height.Text)) ? "" : $"{Width.Text}x{Height.Text}",
					audioCodec = AudioCodec.Text,
					startSec = StartSec.Text,
					duration = Duration.Text
				};
				queue.Enqueue(settings);
			}

			ProcessNextQueue(default, default);
		}

		void CheckTextContainsNumber(object sender, TextCompositionEventArgs e)
		{
			var regex = new Regex("[^0-9.-]+");
			e.Handled = regex.IsMatch(StartSec.Text + e.Text);
		}

		void ProcessNextQueue(object sender, EventArgs eventArgs)
		{
			if (queue.Count == 0) return;

			var encoder = Run(queue.Dequeue());
			encoder.EnableRaisingEvents = true;
			encoder.Exited += ProcessNextQueue;
		}

		Process Run(EncodeSettings settings)
		{
			var encoder = new Process {StartInfo = {FileName = "ffmpeg"}};

			var arguments = new ArgumentComposer();

			arguments.Add($"-i \"{settings.path}\"");
			arguments.Add($"-vcodec {settings.videoCodec}");
			arguments.Add($"-b:v {settings.videoBitrate}", settings.videoBitrate);
			arguments.Add($"-r {settings.framerate}", settings.framerate);
			arguments.Add($"-s {settings.videoSize}", settings.videoSize);
			arguments.Add($"-acodec {settings.audioCodec}");
			arguments.Add($"-ss {settings.startSec}", settings.startSec);
			arguments.Add($"-t {settings.duration}", settings.duration);
			arguments.Add($"\"{settings.path.Replace(".mp4", "Encoded.mp4")}\"");

			encoder.StartInfo.Arguments = arguments.ToString();
			encoder.Start();
			return encoder;
		}
	}
}