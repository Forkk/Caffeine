using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Caffeine
{
	public partial class MainForm : Form
	{
		Thread wakeThread;
		NotifyIcon trayIcon;
		ContextMenu trayMenu;

		MenuItem toggleCaffeineMenuItem;

		public MainForm()
		{
			InitializeComponent();

			trayIcon = new NotifyIcon();
			trayIcon.DoubleClick += (object o, EventArgs args) =>
			{
				Visible = true;
			};
			trayIcon.Icon = Properties.Resources.FullCup;
			InitContextMenu();
		}

		public void InitContextMenu()
		{
			MenuItem[] menu = new[]
			{
				toggleCaffeineMenuItem =
					new MenuItem("Caffeine", new EventHandler(toggleCaffeine), Shortcut.None),
			};

			trayMenu = new ContextMenu(menu);
			trayIcon.ContextMenu = trayMenu;
		}

		public bool CaffeineEnabled
		{
			get { return _caffeineEnabled; }
			set
			{
				_caffeineEnabled = value;
				toggleCaffeineMenuItem.Checked = value;
			}
		}
		private bool _caffeineEnabled;

		private void toggleCaffeine(object sender, EventArgs e)
		{
			CaffeineEnabled = !CaffeineEnabled;
		}

		private void enableCaffeineMenuItem_Click(object sender, EventArgs e)
		{
			CaffeineEnabled = !CaffeineEnabled;
		}

		public void CaffeineThreadStart()
		{
			while (true)
			{
				if (CaffeineEnabled)
				{
					SendKeys.SendWait("{F15}");
				}
				Thread.Sleep(new TimeSpan(0, 0, 59));
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Visible = false;

			wakeThread = new Thread(CaffeineThreadStart);
			wakeThread.Start();

			trayIcon.Visible = true;
		}
	}
}
