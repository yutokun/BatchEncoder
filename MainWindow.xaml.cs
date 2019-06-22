using System.Collections.Generic;
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
			Application.Current.Exit += OnExit;
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
			var queue = new Queue<EncodeSettings>();
			foreach (var file in files)
			{
				if (ExtensionChecker.IsUnsupportedExtension(file)) continue;

				var settings = new EncodeSettings
				{
					input = file,
					videoCodec = VideoCodec.Text,
					videoBitrate = VideoBitrate.Text,
					framerate = Framerate.Text,
					videoSize = (string.IsNullOrEmpty(VideoWidth.Text) || string.IsNullOrEmpty(VideoHeight.Text)) ? "" : $"{VideoWidth.Text}x{VideoHeight.Text}",
					audioCodec = AudioCodec.Text,
					startSec = StartSec.Text,
					duration = Duration.Text,
					concatenate = Concatenate.IsChecked == true,
					output = file
				};
				queue.Enqueue(settings);
			}

			MovieEncoder.AddQueue(queue, Concatenate.IsChecked == true);
		}

		void CheckTextContainsNumber(object sender, TextCompositionEventArgs e)
		{
			var regex = new Regex("[^0-9.-]+");
			e.Handled = regex.IsMatch(StartSec.Text + e.Text);
		}

		void OnExit(object sender, ExitEventArgs e)
		{
			MovieEncoder.RemoveConcatFile();
			Application.Current.Exit -= OnExit;
		}
	}
}