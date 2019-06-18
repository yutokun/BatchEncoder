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

			MovieEncoder.AddQueue(queue);
		}

		void CheckTextContainsNumber(object sender, TextCompositionEventArgs e)
		{
			var regex = new Regex("[^0-9.-]+");
			e.Handled = regex.IsMatch(StartSec.Text + e.Text);
		}
	}
}