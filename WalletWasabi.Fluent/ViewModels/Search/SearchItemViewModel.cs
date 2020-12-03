using System.Windows.Input;
using ReactiveUI;
using WalletWasabi.Fluent.ViewModels.Navigation;

namespace WalletWasabi.Fluent.ViewModels.Search
{
	public class SearchItemViewModel : RoutableViewModel
	{
		private readonly NavigationMetaData _metaData;

		public SearchItemViewModel(
			SearchPageViewModel owner,
			NavigationMetaData metaData,
			SearchCategory category)
		{
			SearchPageViewModel owner1 = owner;
			_metaData = metaData;
			Category = category;

			OpenCommand = ReactiveCommand.CreateFromTask(
				async () =>
				{
					owner1.IsBusy = true;
					var view = await NavigationManager.MaterialiseViewModel(metaData);
					Navigate(view.DefaultTarget).To(view);
					owner1.IsBusy = false;
				});
		}

		public string Title => _metaData.Title;

		public string Caption => _metaData.Caption;

		public int Order => _metaData.Order;

		public SearchCategory Category { get; }

		public string[] Keywords => _metaData.Keywords;

		public ICommand OpenCommand { get; }

		public string IconName => _metaData.IconName;
	}
}