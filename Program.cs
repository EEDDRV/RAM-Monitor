using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyTitle("")]
[assembly: AssemblyCompany("")]
[assembly: NeutralResourcesLanguage("en")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyProduct("TrueFalseFS")]
[assembly: AssemblyDescription("Bool Fullscreen Mode")]
//[assembly: Guid("ThisNeed-ToBe-Hidd-ing?-Overshadowed")]
[assembly: AssemblyCopyright("Copyright © 2021 Ethan Dureus")]
[assembly: AssemblyTrademark("Copyright © 2021 Ethan Dureus")]

namespace RAM_Monitor
{
	public class Program
	{
		partial class Form1
		{
			/// <summary>
			/// Required designer variable.
			/// </summary>
			private System.ComponentModel.IContainer components = null;

			/// <summary>
			/// Clean up any resources being used.
			/// </summary>
			/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
			protected override void Dispose(bool disposing)
			{
				if (disposing && (components != null))
				{
					components.Dispose();
				}
				base.Dispose(disposing);
			}

			#region Windows Form Designer generated code

			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				this.components = new System.ComponentModel.Container();
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.ClientSize = new System.Drawing.Size(800, 450);
				this.Text = "Form1";
			}

			#endregion
		}

		public partial class Form1 : Form
		{
			bool Display_Messages = false;
			Icon high_Usage_Icon;
			Icon warning_Usage_Icon;
			Icon low_Usage_Icon;
			NotifyIcon RAM_ICON;
			Thread RAM_MONITOR_WORKER;
			
			string[] args = Environment.GetCommandLineArgs();
			public Form1(bool Display_Messages_)
			{
				Display_Messages = Display_Messages_;
				InitializeComponent();
				Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

				Assembly assembly = Assembly.GetExecutingAssembly();
				high_Usage_Icon = new Icon(assembly.GetManifestResourceStream("Resources.High.ico"));
				warning_Usage_Icon = new Icon(assembly.GetManifestResourceStream("Resources.Warning.ico"));
				low_Usage_Icon = new Icon(assembly.GetManifestResourceStream("Resources.Low.ico"));

				RAM_ICON = new NotifyIcon();
				RAM_ICON.Icon = low_Usage_Icon;
				RAM_ICON.Visible = true;

				// Create the context menu items and add them to the notication tray icon.
				//MenuItem programNameMenuItem = new MenuItem("Program Name");
				MenuItem quitMenuItem = new MenuItem("Quit");
				ContextMenu contextMenu = new ContextMenu();
				//contextMenu.MenuItems.Add(programNameMenuItem);
				contextMenu.MenuItems.Add(quitMenuItem);
				RAM_ICON.ContextMenu = contextMenu;

				quitMenuItem.Click += quitMenuItem_Click;


				// Hide the form.
				this.WindowState = FormWindowState.Minimized;
				this.ShowInTaskbar = false;

				RAM_MONITOR_WORKER = new Thread(new ThreadStart(RAM_ActivityThread));
				RAM_MONITOR_WORKER.Start();
			}

			void OnApplicationExit(object sender, EventArgs e)
			{	
				RAM_ICON.Dispose();
			}

			void quitMenuItem_Click(object sender,EventArgs e)
			{
				RAM_MONITOR_WORKER.Abort();
				RAM_ICON.Dispose();
				this.Close();
			}

			void RAM_ActivityThread()
			{
				PerformanceCounter mem = new PerformanceCounter("Memory", "Available MBytes");
				try
				{
					while(true)
					{
						float mem_value = mem.NextValue();
						if (Display_Messages == true)
						{
							Console.WriteLine("Memory Available: {0}", mem_value);
						}
						if(mem_value <= 1024)
						{
							if(mem_value <= 512)
							{
								RAM_ICON.Icon = high_Usage_Icon;
							}
							else
							{
								RAM_ICON.Icon = warning_Usage_Icon;
							}
						}
						else
						{
							RAM_ICON.Icon = low_Usage_Icon;
						}
						Thread.Sleep(900); // To decrease cpu load.
					}	
				}
				catch( ThreadAbortException e )
				{
					mem.Dispose();
					Console.WriteLine("Thread aborted: {0}", e.Message);
				}
				
			}
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			bool Display_Messages= false;
			//Check if the user has passed any command line arguments.
			if (Environment.GetCommandLineArgs().Length > 1)
			{
				// Get first argument from command line
				string arg = Environment.GetCommandLineArgs()[1];
				if(arg.ToLower() == "--help" || arg.ToLower() == "-h" ||arg.ToLower() == "/h"|| arg.ToLower() == "-help" || arg.ToLower() == "/help")
				{
					Console.WriteLine("Usage:\tMemoryMonitor.exe [--help | -h]");
					Console.WriteLine("-D:\tDisplays available ram in console.");
					Environment.Exit(0);
				}
				else
				{
					if (arg.ToLower()== "-d"|| arg.ToLower()== "/d")
					{
						Display_Messages = true;
					}
				}
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1(Display_Messages));
		}
	}
}
/*
HOW TO COMPILE:

csc /resource:High.ico,Resources.High.ico /resource:Low.ico,Resources.Low.ico /resource:Warning.ico,Resources.Warning.ico /target:winexe Program.cs
*/
