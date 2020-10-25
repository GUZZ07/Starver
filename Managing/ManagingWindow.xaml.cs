using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Starvers.Managing
{
	/// <summary>
	/// ManagingWindow.xaml 的交互逻辑
	/// </summary>
	public partial class ManagingWindow : Window
	{
		public ManagingWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Hide();
			PlayersView.ItemsSource = Starver.Instance.Players;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs args)
		{
			if (!Starver.Instance.Unloading)
			{
				args.Cancel = true;
				Hide();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			MyMessageBox.Show("emmm", "!");
		}

		private void PlayersView_SelectionChanged(object sender, SelectionChangedEventArgs args)
		{
			MyMessageBox.Show(Starver.Instance.Players[PlayersView.SelectedIndex]?.Name ?? "", PlayersView.SelectedIndex.ToString());
		}
	}
}
