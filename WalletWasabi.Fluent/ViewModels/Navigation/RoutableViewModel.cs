using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using WalletWasabi.Fluent.ViewModels.Dialogs;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;

namespace WalletWasabi.Fluent.ViewModels.Navigation;

public abstract partial class RoutableViewModel : ViewModelBase, INavigatable
{
	[AutoNotify] private bool _isBusy;
	[AutoNotify] private bool _enableCancelOnPressed;
	[AutoNotify] private bool _enableCancelOnEscape;
	[AutoNotify] private bool _enableBack;
	[AutoNotify] private bool _enableCancel;
	[AutoNotify] private bool _isActive;

	public abstract string Title { get; protected set; }

	private CompositeDisposable? _currentDisposable;

	public NavigationTarget CurrentTarget { get; internal set; }

	public virtual NavigationTarget DefaultTarget => NavigationTarget.HomeScreen;

	protected RoutableViewModel()
	{
		BackCommand = ReactiveCommand.Create(() => Navigate().Back());
		CancelCommand = ReactiveCommand.Create(() => Navigate().Clear());
	}

	public virtual string IconName { get; protected set; } = "navigation_regular";
	public virtual string IconNameFocused { get; protected set; } = "navigation_regular";

	public ICommand? NextCommand { get; protected set; }

	public ICommand? SkipCommand { get; protected set; }

	public ICommand BackCommand { get; protected set; }

	public ICommand CancelCommand { get; protected set; }

	private void DoNavigateTo(bool isInHistory)
	{
		if (_currentDisposable is { })
		{
			throw new Exception("Can't navigate to something that has already been navigated to.");
		}

		_currentDisposable = new CompositeDisposable();

		OnNavigatedTo(isInHistory, _currentDisposable);
	}

	protected virtual void OnNavigatedTo(bool isInHistory, CompositeDisposable disposables)
	{
	}

	private void DoNavigateFrom(bool isInHistory)
	{
		OnNavigatedFrom(isInHistory);

		_currentDisposable?.Dispose();
		_currentDisposable = null;
	}

	public INavigationStack<RoutableViewModel> Navigate()
	{
		var currentTarget = CurrentTarget == NavigationTarget.Default ? DefaultTarget : CurrentTarget;

		return Navigate(currentTarget);
	}

	public INavigationStack<RoutableViewModel> Navigate(NavigationTarget currentTarget)
	{
		return UIContext.Navigate(currentTarget);
	}

	public void SetActive()
	{
		if (NavigationState.Instance.HomeScreenNavigation.CurrentPage is { } homeScreen)
		{
			homeScreen.IsActive = false;
		}

		if (NavigationState.Instance.DialogScreenNavigation.CurrentPage is { } dialogScreen)
		{
			dialogScreen.IsActive = false;
		}

		if (NavigationState.Instance.FullScreenNavigation.CurrentPage is { } fullScreen)
		{
			fullScreen.IsActive = false;
		}

		if (NavigationState.Instance.CompactDialogScreenNavigation.CurrentPage is { } compactDialogScreen)
		{
			compactDialogScreen.IsActive = false;
		}

		IsActive = true;
	}

	public void OnNavigatedTo(bool isInHistory)
	{
		DoNavigateTo(isInHistory);
	}

	void INavigatable.OnNavigatedFrom(bool isInHistory)
	{
		DoNavigateFrom(isInHistory);
	}

	protected virtual void OnNavigatedFrom(bool isInHistory)
	{
	}

	protected void EnableAutoBusyOn(params ICommand[] commands)
	{
		foreach (var command in commands)
		{
			(command as IReactiveCommand)?.IsExecuting
				.ObserveOn(RxApp.MainThreadScheduler)
				.Skip(1)
				.Subscribe(x => IsBusy = x);
		}
	}

	public async Task<DialogResult<TResult>> NavigateDialogAsync<TResult>(DialogViewModelBase<TResult> dialog)
		=> await NavigateDialogAsync(dialog, CurrentTarget);

	public async Task<DialogResult<TResult>> NavigateDialogAsync<TResult>(DialogViewModelBase<TResult> dialog, NavigationTarget target, NavigationMode navigationMode = NavigationMode.Normal)
	{
		return await UIContext.Navigate(target).NavigateDialogAsync(dialog, navigationMode);
	}

	protected async Task ShowErrorAsync(string title, string message, string caption, NavigationTarget navigationTarget = NavigationTarget.Default)
	{
		var target =
			navigationTarget != NavigationTarget.Default
			? navigationTarget
			: CurrentTarget == NavigationTarget.CompactDialogScreen
				? NavigationTarget.CompactDialogScreen
				: NavigationTarget.DialogScreen;

		await Navigate(target).ShowErrorAsync(title, message, caption);
	}

	protected void SetupCancel(bool enableCancel, bool enableCancelOnEscape, bool enableCancelOnPressed)
	{
		EnableCancel = enableCancel;
		EnableCancelOnEscape = enableCancelOnEscape;
		EnableCancelOnPressed = enableCancelOnPressed;
	}
}
