using System.Collections.ObjectModel;

namespace THJ_Inventory_Reader;

public partial class MainPage : ContentPage
{
	private ObservableCollection<InventoryItem> inventoryItems;

	public MainPage()
	{
		InitializeComponent();
		inventoryItems = new ObservableCollection<InventoryItem>();
		InventoryList.ItemsSource = inventoryItems;
	}

	private void OnFileMenuClicked(object? sender, EventArgs e)
	{
		FileMenu.IsVisible = !FileMenu.IsVisible;
	}

	private void OnCloseMenuClicked(object? sender, EventArgs e)
	{
		FileMenu.IsVisible = false;
	}

	private async void OnOpenFileClicked(object? sender, EventArgs e)
	{
		try
		{
			var filePickerOptions = new PickOptions
			{
				PickerTitle = "Select Inventory File",
				FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
				{
					{ DevicePlatform.WinUI, new[] { ".txt" } },
					{ DevicePlatform.macOS, new[] { "txt" } },
					{ DevicePlatform.iOS, new[] { "public.text" } },
					{ DevicePlatform.Android, new[] { "text/plain" } }
				})
			};

			var result = await FilePicker.PickAsync(filePickerOptions);
			if (result != null)
			{
				await LoadInventoryFile(result);
				FileMenu.IsVisible = false;
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to open file: {ex.Message}", "OK");
		}
	}

	private async Task LoadInventoryFile(FileResult file)
	{
		try
		{
			var content = await File.ReadAllTextAsync(file.FullPath);
			ParseInventoryData(content);

			// Update UI
			FileNameLabel.Text = $"File: {file.FileName}";
			MainContent.IsVisible = false;
			InventoryView.IsVisible = true;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to load inventory file: {ex.Message}", "OK");
		}
	}

	private void ParseInventoryData(string content)
	{
		inventoryItems.Clear();

		var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

		// Skip header line and process data
		for (int i = 1; i < lines.Length; i++)
		{
			var line = lines[i].Trim();
			if (string.IsNullOrEmpty(line)) continue;

			// Stop at Ammo line as requested
			if (line.StartsWith("Ammo\t"))
			{
				// Add the Ammo line itself
				var ammoData = line.Split('\t');
				if (ammoData.Length >= 2)
				{
					inventoryItems.Add(new InventoryItem
					{
						Location = ammoData[0],
						Name = ammoData[1]
					});
				}
				break;
			}

			var parts = line.Split('\t');
			if (parts.Length >= 2)
			{
				// Only show items that have a proper location (not slots)
				if (!string.IsNullOrEmpty(parts[0]) && !parts[0].Contains("-Slot"))
				{
					inventoryItems.Add(new InventoryItem
					{
						Location = parts[0],
						Name = parts[1]
					});
				}
			}
		}
	}
}

public class InventoryItem
{
	public string Location { get; set; } = "";
	public string Name { get; set; } = "";
}
