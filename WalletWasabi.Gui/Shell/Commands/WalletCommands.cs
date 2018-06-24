﻿using AvalonStudio.Commands;
using AvalonStudio.Documents;
using AvalonStudio.Extensibility;
using AvalonStudio.Shell;
using ReactiveUI;
using System.Linq;
using WalletWasabi.Gui.Tabs.WalletManager;

namespace WalletWasabi.Gui.Shell.Commands
{
	internal class WalletCommands
	{
		public WalletCommands()
		{
			GenerateWalletCommand = new CommandDefinition(
				"Generate Wallet", null, ReactiveCommand.Create(OnGenerateWallet));

			RecoverWalletCommand = new CommandDefinition(
				"Recover Wallet", null, ReactiveCommand.Create(OnRecoverWallet));

			LoadWallet = new CommandDefinition(
				"Load Wallet", null, ReactiveCommand.Create(OnLoadWallet));
		}

		private void OnGenerateWallet()
		{
			IoC.Get<IShell>().GetOrCreate<WalletManagerViewModel>().SelectGenerateWallet();
		}

		private void OnRecoverWallet()
		{
			IoC.Get<IShell>().GetOrCreate<WalletManagerViewModel>().SelectRecoverWallet();
		}

		private void OnLoadWallet()
		{
			IoC.Get<IShell>().GetOrCreate<WalletManagerViewModel>().SelectLoadWallet();
		}

		[ExportCommandDefinition("File.GenerateWallet")]
		public CommandDefinition GenerateWalletCommand { get; }

		[ExportCommandDefinition("File.RecoverWallet")]
		public CommandDefinition RecoverWalletCommand { get; }

		[ExportCommandDefinition("File.LoadWallet")]
		public CommandDefinition LoadWallet { get; }
	}
}
