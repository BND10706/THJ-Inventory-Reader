using System.Collections.ObjectModel;

namespace THJ_Inventory_Reader;

public partial class MainPage : ContentPage
{
	private Dictionary<string, Label> equipmentSlots;

	public MainPage()
	{
		InitializeComponent();
		InitializeEquipmentSlots();
		LoadDefaultInventory();
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
	private void LoadDefaultInventory()
	{
		try
		{
			// Create a default inventory with sample EverQuest items
			var defaultInventoryData = @"Location	Name	ID	Count	Slots
Head	Crown of the Froglok King	12034	1	8
Neck	Necklace of Superiority	5521	1	0
Ear	Black Sapphire Electrum Earring	14761	1	8
Ear	Earring of the Solstice	19041	1	8
Shoulders	Pauldrons of the Deep Flame	25987	1	8
Arms	Sleeves of the Keeper	8942	1	8
Wrist	Bracelet of Clarification	6633	1	8
Wrist	Bracer of Benevolence	7455	1	8
Hands	Gauntlets of Fiery Might	11287	1	8
Chest	Breastplate of Virulent Protection	31045	1	8
Back	Cloak of Flames	9876	1	8
Waist	Belt of the Great Turtle	4521	1	8
Legs	Greaves of the Penitent	18934	1	8
Feet	Boots of the Storm	3456	1	8
Ring	Ring of the Ancients	2134	1	8
Ring	Band of Eternal Flame	7789	1	8
Primary	Fiery Defender	20456	1	0
Secondary	Shield of the Righteous	15678	1	0
Range	Elvish Longbow	8901	1	0
Ammo	Platinum Tipped Arrow	234	200	0";

			ParseInventoryData(defaultInventoryData);

			// Update UI to show character sheet
			FileNameLabel.Text = "Default EverQuest Character";
			MainContent.IsVisible = false;
			InventoryView.IsVisible = true;
		}
		catch (Exception ex)
		{
			// Since this is no longer async, we'll just use console output for errors
			System.Diagnostics.Debug.WriteLine($"Failed to load default inventory: {ex.Message}");
		}
	}
	private void OnLoadDefaultClicked(object? sender, EventArgs e)
	{
		LoadDefaultInventory();
		FileMenu.IsVisible = false;
	}
}
