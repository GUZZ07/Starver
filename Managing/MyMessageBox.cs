using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Starvers.Managing
{
	public static class MyMessageBox
	{
		public static MessageBoxResult Show(string text, string title = "")
		{
			var messagebox = new FakeMessageBoxOK();
			messagebox.Text.Text = text;
			messagebox.Title.Content = title;
			messagebox.ShowDialog();
			return messagebox.Result;
		}
		public static MessageBoxResult ShowOKCancel(string text, string title = "")
		{
			var messagebox = new FakeMessageBoxOKCancel();
			messagebox.Text.Text = text;
			messagebox.Title.Content = title;
			messagebox.ShowDialog();
			return messagebox.Result;
		}
	}
}
