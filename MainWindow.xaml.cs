using System;
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
			Console.WriteLine(files[0]);
			var settings = new EncodeSettings
			{
				path = files[0],
				videoCodec = VideoCodec.Text,
				videoBitrate = VideoBitrate.Text,
				framerate = Framerate.Text,
				videoSize = (string.IsNullOrEmpty(Width.Text) || string.IsNullOrEmpty(Height.Text)) ? "" : $"{Width.Text}x{Height.Text}",
				audioCodec = AudioCodec.Text,
				startSec = StartSec.Text,
				duration = Duration.Text
			};
			Run(settings);
		}

		class EncodeSettings
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

		void CheckTextContainsNumber(object sender, TextCompositionEventArgs e)
		{
			var regex = new Regex("[^0-9.-]+");
			e.Handled = regex.IsMatch(StartSec.Text + e.Text);
		}

		void Run(EncodeSettings settings)
		{
			var encoder = new Process {StartInfo = {FileName = "ffmpeg"}};

			var input = $"-i \"{settings.path}\"";
			var vcodec = $"-vcodec {settings.videoCodec}";
			var vBitrate = string.IsNullOrEmpty(settings.videoBitrate) ? "" : $"-b:v {settings.videoBitrate}";
			var framerate = string.IsNullOrEmpty(settings.framerate) ? "" : $"-r {settings.framerate}";
			var vSize = string.IsNullOrEmpty(settings.videoSize) ? "" : $"-s {settings.videoSize}";
			var acodec = $"-acodec {settings.audioCodec}";
			var startSec = string.IsNullOrEmpty(settings.startSec) ? "" : $"-ss {settings.startSec}";
			var duration = string.IsNullOrEmpty(settings.duration) ? "" : $"-t {settings.duration}";
			var output = $"\"{settings.path.Replace(".mp4", "Encoded.mp4")}\"";

			encoder.StartInfo.Arguments = $"{input} {vcodec} {vBitrate} {framerate} {vSize} {acodec} {startSec} {duration} {output}";
			encoder.Start();
		}
	}
}