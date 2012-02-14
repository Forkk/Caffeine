using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Windows.Forms;

namespace Caffeine
{
	public class Caffeine : ApplicationContext
	{
		Thread wakeThread;
		NotifyIcon trayIcon;
		ContextMenu trayMenu;

		MenuItem toggleMenuItem;

		public Caffeine()
		{
			trayIcon = new NotifyIcon();

			MenuItem[] menu = new[]
			{
				toggleMenuItem = 
					new MenuItem("Enable Caffeine", new EventHandler(ToggleClicked), Shortcut.None),
				new MenuItem("-"),
				new MenuItem("Quit", new EventHandler(Quit), Shortcut.None),
			};
			trayMenu = new ContextMenu(menu);

			trayIcon.ContextMenu = trayMenu;
			toggleMenuItem.DefaultItem = true;
			Enabled = false;
			trayIcon.DoubleClick += (o, args) => toggleMenuItem.PerformClick();

			Application.ApplicationExit += (o, args) =>
			{
				// Hide the tray icon
				trayIcon.Visible = false;

				// This will quit the wake thread upon the next iteration of its loop.
				Running = false;

				// Interrupt the thread if it's sleeping so that it will quit.
				if (wakeThread.ThreadState == ThreadState.WaitSleepJoin)
					wakeThread.Interrupt();
			};

			Start();
		}

		public void Start()
		{
			trayIcon.Visible = true;
			trayIcon.ShowBalloonTip(3000,
									Properties.Resources.StartupMessageTitle,
									Properties.Resources.StartupMessage,
									ToolTipIcon.Info);

			wakeThread = new Thread(new ThreadStart(CaffeineThreadStart));
			wakeThread.Start();
		}

		private void Quit(object sender, EventArgs e)
		{
			OnMainFormClosed(this, EventArgs.Empty);
		}

		private void ToggleClicked(object sender, EventArgs e)
		{
			Enabled = !Enabled;
		}

		public void CaffeineThreadStart()
		{
			Running = true;
			while (Running)
			{
				if (Enabled)
				{
					SendKeys.SendWait("{F15}");
				}
				try
				{
					Thread.Sleep(new TimeSpan(0, 0, 59));
				}
				catch (ThreadInterruptedException) { }
			}
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				toggleMenuItem.Checked = value;
				trayIcon.Icon = (value ? Properties.Resources.FullCup : Properties.Resources.EmptyCup);
			}
		}
		bool _enabled;

		public bool Running
		{
			get;
			protected set;
		}
	}
}
