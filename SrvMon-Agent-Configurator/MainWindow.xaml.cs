using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Bmo.Bubblegum;
using Common;
using Microsoft.Win32;

namespace Bmo
{
	public partial class MainWindow
	{
		private readonly SrvMonParams _jakeParams = new SrvMonParams();
		//wcf proxy class object
		private ServerClient _client;
		public MainWindow()
		{
			//System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-GB");
			InitializeComponent();
			//load current configuration from registry
			LoadConfig();
			//create wcf proxy class object
			try
			{
				_client = GetClient();
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 300);
			}
			//initial authentication based on credentials from registry
			Auth(_jakeParams.UserId, "UserPassword".GetRegString());
			//fill auth text boxes
			loginTextBox.Text = _jakeParams.UserId;
			passwordBox.Password = "UserPassword".GetRegString();
			//set window title
			Title = "Server Monitor Configurator " + Assembly.GetExecutingAssembly().GetName().Version;
#region PopulateUI
			//number ranges for combo boxes
			topRamCombo.ItemsSource = Enumerable.Range(1, 10);
			topCpuCombo.ItemsSource = Enumerable.Range(1, 10);
			historyCombo.ItemsSource = Enumerable.Range(1, 6);
			errorCombo.ItemsSource = Enumerable.Range(1, 23);
			timerCombo.ItemsSource = Enumerable.Range(1, 15);
			//strings from resources to ui
			authTab.Header = Properties.Resources.Login;
			loginTextBlock.Text = Properties.Resources.UserName;
			passwordTextBlock.Text = Properties.Resources.Password;
			loginButton.Content = Properties.Resources.Save;
			regButton.Content = Properties.Resources.Register;
			paramTab.Header = Properties.Resources.Parameters;
			paramHeader.Text = Properties.Resources.ParamHeader;
			topRamText.Text = Properties.Resources.TopRamText;
			topCpuText.Text = Properties.Resources.TopCpuText;
			historyText.Text = Properties.Resources.HistoryText;
			errorText.Text = Properties.Resources.ErrorText;
			timerText.Text = Properties.Resources.TimerText;
			srvTab.Header = Properties.Resources.SrvTab;
			srvHeader.Text = Properties.Resources.SrvHeader;
			srvNameColumn.Header = Properties.Resources.SrvNameColumn;
			srvStateColumn.Header = Properties.Resources.SrvStateColumn;
			startTab.Header = Properties.Resources.Management;
			nextTextBlock1.Text = Properties.Resources.Next1;
			nextTextBlock2.Text = Properties.Resources.Next2;
			connectRun.Text = Properties.Resources.Next3;
			siteButton.Content = Properties.Resources.MyServers;
#endregion
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!(e.Source is TabControl)) return;
			LoadConfig();
			if (paramTab.IsSelected)
			{
				topRamCombo.SelectedItem = _jakeParams.TopRamProcesses;
				topRamCombo.SelectionChanged += OnComboBox_Changed;
				topCpuCombo.SelectedItem = _jakeParams.TopCpuProcesses;
				topCpuCombo.SelectionChanged += OnComboBox_Changed;
				historyCombo.SelectedItem = TimeSpan.Parse(_jakeParams.HwMonTimeSpan).Hours;
				historyCombo.SelectionChanged += OnComboBox_Changed;
				errorCombo.SelectedItem = TimeSpan.Parse(_jakeParams.EvMonTimeSpan).Hours;
				errorCombo.SelectionChanged += OnComboBox_Changed;
				timerCombo.SelectedItem = _jakeParams.ServiceTimer / 60;
				timerCombo.SelectionChanged += OnComboBox_Changed;
			}
			if (srvTab.IsSelected)
			{
				GetServices();
			}
			if (startTab.IsSelected)
			{
				GetAgentState();
			}
		}

		//login button pressed
		private void loginButton_Click(object sender, RoutedEventArgs e)
		{
			Auth(loginTextBox.Text, passwordBox.Password);
		}

		//register button pressed
		private void regButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("https://servermonitor.online/Account/Register");
		}

		//change values in comboboxes
		private static void OnComboBox_Changed(object sender, SelectionChangedEventArgs e)
		{
			var combo = sender as ComboBox;
			switch (combo.Name)
			{
				case "topRamCombo":
					combo.SelectedItem.ToString().SetRegString("TopRamProcesses");
					break;
				case "topCpuCombo":
					combo.SelectedItem.ToString().SetRegString("TopCpuProcesses");
					break;
				case "historyCombo":
					(((int) (combo.SelectedItem)).ToString("00") + ":00:00").SetRegString("HwMonTimeSpan");
					break;
				case "errorCombo":
					(((int) (combo.SelectedItem)).ToString("00") + ":00:00").SetRegString("EvMonTimeSpan");
					break;
				case "timerCombo":
					((int) (combo.SelectedItem)*60).ToString().SetRegString("ServiceTimer");
					break;
			}
		}

		//service list item checked
		private void srvCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			//update list of monitored services in registry
			var sb = new StringBuilder();
			var selectedList = srvListView.SelectedItems;
			//40 services is maximum
			if (selectedList.Count >= 40)
			{
				MessageBox.Show(Properties.Resources.TooManyServices, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
				bottomTextBlock.Text = Properties.Resources.Selected + selectedList.Count + Properties.Resources.Services;
				selectedList.RemoveAt(39);
			}
			//list of selected services as a string
			foreach (var item in selectedList)
			{
				sb.Append(((ServiceMonitor)item).ServiceName);
				sb.Append(" ");
			}
			var selectedString = sb.ToString();
			selectedString.SetRegString("MonitoredServices");
			_jakeParams.MonitoredServices = selectedString;
		}

		//start or stop agent service
		private void startAgentButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var sc = new ServiceController("SrvMonWatcher"))
				{
					switch (sc.Status)
					{
						case ServiceControllerStatus.Running:
						case ServiceControllerStatus.ContinuePending:
						case ServiceControllerStatus.StartPending:
							sc.Stop();
							bottomTextBlock.Text = Properties.Resources.SrvMonStopped;
							GetAgentState();
							return;
						case ServiceControllerStatus.Stopped:
						case ServiceControllerStatus.Paused:
						case ServiceControllerStatus.PausePending:
						case ServiceControllerStatus.StopPending:
							sc.Start();
							bottomTextBlock.Text = Properties.Resources.SrvMonStarted;
							GetAgentState();
							return;
						default:
							GetAgentState();
							return;
					}
				}
			}
			catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
			{
				startAgentButton.Visibility = Visibility.Collapsed;
				startedTextBlock.Text = Properties.Resources.SrvMonNotInstalled;
				bottomTextBlock.Text = Properties.Resources.SrvMonNotInstalled;
			}
		}

		//get and show server monitor agent state
		private void GetAgentState()
		{
			try
			{
				using (var sc = new ServiceController("SrvMonWatcher"))
				{
					switch (sc.Status)
					{
						case ServiceControllerStatus.Running:
						case ServiceControllerStatus.ContinuePending:
						case ServiceControllerStatus.StartPending:
							startedTextBlock.Text = Properties.Resources.SrvMonStarted;
							startAgentButton.Foreground = new SolidColorBrush(Colors.Red);
							startAgentButton.Content = Properties.Resources.Stop;
							siteButton.Visibility = Visibility.Visible;
							nextTextBlock1.Visibility = Visibility.Visible;
							nextTextBlock2.Visibility = Visibility.Visible;
							nextTextBlock3.Visibility = Visibility.Visible;
							return;
						case ServiceControllerStatus.Stopped:
						case ServiceControllerStatus.Paused:
						case ServiceControllerStatus.PausePending:
						case ServiceControllerStatus.StopPending:
							startedTextBlock.Text = Properties.Resources.SrvMonStopped;
							startAgentButton.Foreground = new SolidColorBrush(Colors.Green);
							startAgentButton.Content = Properties.Resources.Start;
							return;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
			{
				startAgentButton.Visibility = Visibility.Collapsed;
				startedTextBlock.Text = Properties.Resources.SrvMonNotInstalled;
				bottomTextBlock.Text = Properties.Resources.SrvMonNotInstalled;
			}
		}

		//"my servers" button pressed
		private void siteButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("https://servermonitor.online/Servers");
		}

		//"contact me" pressed
		private void sendMail_Click(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			Process.Start(e.Uri.AbsoluteUri);
			e.Handled = true;
		}

		//read and validate current configuration from registry
		private void LoadConfig()
		{
			try
			{
				Registry.LocalMachine.OpenSubKey("Software", true).CreateSubKey("CandyKingdom");
				if ("UserPassword".GetRegString() == null)
					"".SetRegString("UserPassword");
				if ("ServerId".GetRegString() != null)
					_jakeParams.ServerId = Guid.Parse("ServerId".GetRegString());
				if ("UserId".GetRegString() != null)
					_jakeParams.UserId = (new System.Net.Mail.MailAddress("UserId".GetRegString())).ToString();
				if ("EvMonTimeSpan".GetRegString() != null)
					_jakeParams.EvMonTimeSpan = TimeSpan.Parse("EvMonTimeSpan".GetRegString()).ToString();
				if ("HwMonTimeSpan".GetRegString() != null)
					_jakeParams.HwMonTimeSpan = TimeSpan.Parse("HwMonTimeSpan".GetRegString()).ToString();
				if ("MonitoredServices".GetRegString() != null)
					_jakeParams.MonitoredServices = "MonitoredServices".GetRegString();
				if ("ServiceTimer".GetRegString() != null)
					_jakeParams.ServiceTimer = int.Parse("ServiceTimer".GetRegString());
				if ("TopCpuProcesses".GetRegString() != null)
					_jakeParams.TopCpuProcesses = int.Parse("TopCpuProcesses".GetRegString());
				if ("TopRamProcesses".GetRegString() != null)
					_jakeParams.TopRamProcesses = int.Parse("TopRamProcesses".GetRegString());
			}
			catch (Exception ex) when (ex is XamlParseException || ex is FormatException || ex is ArgumentException || ex is NullReferenceException)
			{
			}
			finally
			{
				_jakeParams.SaveConfig();
			}
		}

		//create wcf proxy object
		private ServerClient GetClient()
		{
			try
			{
				var client = new ServerClient();
				client.ClientCredentials.UserName.UserName = "ServiceLogin".GetRegString();
				client.ClientCredentials.UserName.Password = "ServicePassword".GetRegString();
				return client;
			}
			catch (NullReferenceException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 303);
				return null;
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 303);
				_client.Abort();
				return null;
			}
		}

		//check login and password
		private async void Auth(string login, string password)
		{
			authBar.IsIndeterminate = true;
			var authOk = false;
			bottomTextBlock.Text = Properties.Resources.Connecting;
			loginButton.IsEnabled = false;
			try
			{
				if ((_client == null) || (_client.State != CommunicationState.Opened))
				{
					_client = GetClient();
				}
				authOk = await _client.AuthAsync(login, password);
			}
			catch (FaultException ex)
			{
				(ex.Message).WriteLog(EventLogEntryType.Error, 305);
				MessageBox.Show(Properties.Resources.ServerInternalError, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception ex) when (ex is ServerTooBusyException || ex is WebException)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 305);
				MessageBox.Show(Properties.Resources.ServerConnectionError, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
				_client.Abort();
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 305);
				MessageBox.Show(Properties.Resources.ServerTimeoutError, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
				_client.Abort();
			}
			if (authOk)
			{
				login.SetRegString("UserId");
				password.SetRegString("UserPassword");
				bottomTextBlock.Text = Properties.Resources.AuthSuccess;
				paramTab.IsEnabled = true;
				srvTab.IsEnabled = true;
				startTab.IsEnabled = true;
			}
			else
			{
				bottomTextBlock.Text = Properties.Resources.AuthError;
			}
			loginButton.IsEnabled = true;
			authBar.IsIndeterminate = false;
		}

		//services list data structure
		private struct ServiceMonitor
		{
			public bool IsMonitored { get; set; }
			public string ServiceName { get; set; }
			public string ServiceDisplayName { get; set; }
			public string State { get; set; }
		}

		//populate services list
		private async void GetServices()
		{
			srvListView.ItemsSource = await Task.Run(() => GetServicesArray(_jakeParams.MonitoredServices));
			srvListView.SelectedItems.Clear();
			//select monitored services in list
			foreach (var item in srvListView.Items.Cast<object>().Where(item => ((ServiceMonitor)item).IsMonitored).Take(40))
			{
				srvListView.SelectedItems.Add(item);
			}
		}

		private static ServiceMonitor[] GetServicesArray(string monitoredServices)
		{
			return ServiceController.GetServices().Select(service => new ServiceMonitor
			{
				IsMonitored = monitoredServices.Split(null).Contains(service.ServiceName),
				ServiceName = service.ServiceName,
				ServiceDisplayName = service.DisplayName,
				State = service.Status.ToString()
			}).OrderBy(s => s.ServiceDisplayName).ToArray();
		}

		//force app shutdown on window close
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			Application.Current.Shutdown();
		}
	}
}
