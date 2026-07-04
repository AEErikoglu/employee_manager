using frontend.ViewModels;

namespace frontend;

public partial class MainPage : ContentPage
{
	private const double DesignWidth = 404;
	private const double DesignHeight = 760;
	private readonly SignUpViewModel _viewModel = new();

	public MainPage()
	{
		InitializeComponent();
		BindingContext = _viewModel;
		SizeChanged += OnPageSizeChanged;
	}

	private void OnTogglePasswordClicked(object? sender, EventArgs e)
	{
		_viewModel.TogglePasswordVisibility();
	}

	// Scale the full page against the available viewport so the sign-up screen stays single-screen on short displays.
	private void OnPageSizeChanged(object? sender, EventArgs e)
	{
		if (Width <= 0 || Height <= 0)
		{
			return;
		}

		const double horizontalInset = 32;
		const double verticalInset = 40;

		var widthScale = (Width - horizontalInset) / DesignWidth;
		var heightScale = (Height - verticalInset) / DesignHeight;
		var scale = Math.Min(1d, Math.Min(widthScale, heightScale));

		ResponsiveContent.Scale = Math.Max(0.82d, scale);
	}
}
