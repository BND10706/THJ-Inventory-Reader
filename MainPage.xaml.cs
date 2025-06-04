namespace THJ_Inventory_Reader;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}
	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Inventory Reader Started ({count} time)";
		else
			CounterBtn.Text = $"Inventory Reader Started ({count} times)";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}
