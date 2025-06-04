using System.Collections.ObjectModel;

namespace THJ_Inventory_Reader;

public partial class MainPage : ContentPage
{
	private Dictionary<string, Label> equipmentSlots;

	public MainPage()
	{
		InitializeComponent();
		InitializeEquipmentSlots();
	}

	private void InitializeEquipmentSlots()
	{
		equipmentSlots = new Dictionary<string, Label>
		{
			{ "Head", HeadSlot },
			{ "Neck", NeckSlot },
			{ "Ear", EarSlot1 },  // We'll handle multiple ears/rings specially
			{ "Shoulders", ShoulderSlot },
			{ "Arms", ArmsSlot },
			{ "Wrist", WristSlot1 },  // We'll handle multiple wrists specially
			{ "Hands", HandsSlot },
			{ "Chest", ChestSlot },
			{ "Back", BackSlot },
			{ "Waist", WaistSlot },
			{ "Legs", LegsSlot },
			{ "Feet", FeetSlot },
			{ "Ring", RingSlot1 },  // We'll handle multiple rings specially
			{ "Primary", PrimarySlot },
			{ "Secondary", SecondarySlot },
			{ "Range", RangeSlot },
			{ "Ammo", AmmoSlot }
		};
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
		// Clear all equipment slots
		ClearEquipmentSlots();

		var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		var earCount = 0;
		var wristCount = 0;
		var ringCount = 0;

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
					SetEquipmentSlot("Ammo", ammoData[1]);
				}
				break;
			}

			var parts = line.Split('\t');
			if (parts.Length >= 2)
			{
				var location = parts[0];
				var itemName = parts[1];

				// Only show items that have a proper location (not slots)
				if (!string.IsNullOrEmpty(location) && !location.Contains("-Slot"))
				{
					// Handle multiple items of same type (ears, wrists, rings)
					if (location == "Ear")
					{
						if (earCount == 0)
							SetEquipmentSlot("Ear", itemName, EarSlot1);
						else if (earCount == 1)
							SetEquipmentSlot("Ear", itemName, EarSlot2);
						earCount++;
					}
					else if (location == "Wrist")
					{
						if (wristCount == 0)
							SetEquipmentSlot("Wrist", itemName, WristSlot1);
						else if (wristCount == 1)
							SetEquipmentSlot("Wrist", itemName, WristSlot2);
						wristCount++;
					}
					else if (location == "Ring")
					{
						if (ringCount == 0)
							SetEquipmentSlot("Ring", itemName, RingSlot1);
						else if (ringCount == 1)
							SetEquipmentSlot("Ring", itemName, RingSlot2);
						ringCount++;
					}
					else
					{
						SetEquipmentSlot(location, itemName);
					}
				}
			}
		}
	}

	private void ClearEquipmentSlots()
	{
		foreach (var slot in equipmentSlots.Values)
		{
			slot.Text = GetSlotDefaultText(slot);
			slot.TextColor = Colors.White;
			slot.FontSize = 10;
		}

		// Clear additional slots
		EarSlot2.Text = "Ear";
		EarSlot2.TextColor = Colors.White;
		WristSlot2.Text = "Wrist";
		WristSlot2.TextColor = Colors.White;
		RingSlot2.Text = "Ring";
		RingSlot2.TextColor = Colors.White;
	}

	private void SetEquipmentSlot(string slotType, string itemName, Label? specificSlot = null)
	{
		var targetSlot = specificSlot ?? equipmentSlots.GetValueOrDefault(slotType);
		if (targetSlot != null)
		{
			// Truncate long item names for display
			var displayName = itemName.Length > 15 ? itemName.Substring(0, 12) + "..." : itemName;
			targetSlot.Text = displayName;
			targetSlot.TextColor = Colors.Gold;  // Gold color for equipped items
			targetSlot.FontSize = 9;
		}
	}

	private string GetSlotDefaultText(Label slot)
	{
		if (slot == HeadSlot) return "Head";
		if (slot == NeckSlot) return "Neck";
		if (slot == EarSlot1) return "Ear";
		if (slot == ShoulderSlot) return "Shoulders";
		if (slot == ArmsSlot) return "Arms";
		if (slot == WristSlot1) return "Wrist";
		if (slot == HandsSlot) return "Hands";
		if (slot == ChestSlot) return "Chest";
		if (slot == BackSlot) return "Back";
		if (slot == WaistSlot) return "Waist";
		if (slot == LegsSlot) return "Legs";
		if (slot == FeetSlot) return "Feet";
		if (slot == RingSlot1) return "Ring";
		if (slot == PrimarySlot) return "Primary";
		if (slot == SecondarySlot) return "Secondary";
		if (slot == RangeSlot) return "Range";
		if (slot == AmmoSlot) return "Ammo";
		return "Empty";
	}
}
