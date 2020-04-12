using Avalonia.Controls.Notifications;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Concurrency;
using WalletWasabi.Gui.Controls.WalletExplorer;
using WalletWasabi.Wallets;

namespace WalletWasabi.Gui.Helpers
{
	public static class NotificationHelpers
	{
		private static INotificationManager NullNotificationManager { get; } = new NullNotificationManager();

		public static INotificationManager GetNotificationManager()
		{
			return Locator.Current.GetService<INotificationManager>() ?? NullNotificationManager;
		}

		public static void Notify(string message, string title, NotificationType type, Action onClick = null, object sender = null)
		{
			if (sender is Wallet wallet)
			{
				title = $"{title} - {wallet.WalletName}";
			}
			else if (sender is WalletViewModelBase walletViewModelBase)
			{
				title = $"{title} - {walletViewModelBase.WalletName}";
			}

			RxApp.MainThreadScheduler
				.Schedule(() => GetNotificationManager()
				.Show(new Notification(title, message, type, TimeSpan.FromSeconds(7), onClick)));
		}

		public static void Success(string message, string title = "Success!", object sender = null)
		{
			Notify(message, title, NotificationType.Success, sender: sender);
		}

		public static void Information(string message, string title = "Info", object sender = null)
		{
			Notify(message, title, NotificationType.Information, sender: sender);
		}

		public static void Warning(string message, string title = "Warning!", object sender = null)
		{
			Notify(message, title, NotificationType.Warning, sender: sender);
		}

		public static void Error(string message, string title = "Error!", object sender = null)
		{
			Notify(message, title, NotificationType.Error, sender: sender);
		}
	}
}
