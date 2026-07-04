using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace frontend.ViewModels;

public sealed class SignUpViewModel : INotifyPropertyChanged
{
	private string _emailAddress = string.Empty;
	private string _password = string.Empty;
	private bool _hasAcceptedTerms;
	private bool _isPasswordHidden = true;

	public event PropertyChangedEventHandler? PropertyChanged;

	public string EmailAddress
	{
		get => _emailAddress;
		set
		{
			if (SetProperty(ref _emailAddress, value))
			{
				UpdateCanCreateAccount();
			}
		}
	}

	public string Password
	{
		get => _password;
		set
		{
			if (SetProperty(ref _password, value))
			{
				UpdateCanCreateAccount();
			}
		}
	}

	public bool HasAcceptedTerms
	{
		get => _hasAcceptedTerms;
		set
		{
			if (SetProperty(ref _hasAcceptedTerms, value))
			{
				UpdateCanCreateAccount();
			}
		}
	}

	public bool IsPasswordHidden
	{
		get => _isPasswordHidden;
		private set
		{
			if (SetProperty(ref _isPasswordHidden, value))
			{
				OnPropertyChanged(nameof(PasswordToggleText));
			}
		}
	}

	public bool CanCreateAccount => HasAcceptedTerms && !string.IsNullOrWhiteSpace(EmailAddress) && !string.IsNullOrWhiteSpace(Password);

	public Color CreateAccountButtonColor => CanCreateAccount
		? Color.FromArgb("#A8C8F0")
		: Color.FromArgb("#4E6281");

	public string PasswordToggleText => IsPasswordHidden ? "Show" : "Hide";

	public void TogglePasswordVisibility()
	{
		IsPasswordHidden = !IsPasswordHidden;
	}

	// Keep the submit state derived from the current inputs so the page stays reactive across screen sizes and future backend wiring.
	private void UpdateCanCreateAccount()
	{
		OnPropertyChanged(nameof(CanCreateAccount));
		OnPropertyChanged(nameof(CreateAccountButtonColor));
	}

	private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(storage, value))
		{
			return false;
		}

		storage = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
