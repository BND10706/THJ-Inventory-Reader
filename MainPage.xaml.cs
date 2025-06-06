using System.Collections.ObjectModel;

namespace THJ_Inventory_Reader;

public partial class MainPage : ContentPage
{
	private Dictionary<string, Label> equipmentSlots = new();

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
			{ "Face", FaceSlot },  // New Face slot
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
			{ "Fingers", RingSlot1 },  // Handle "Fingers" as Ring alias from inventory files
			{ "Primary", PrimarySlot },
			{ "Secondary", SecondarySlot },
			{ "Range", RangeSlot },
			{ "Ammo", AmmoSlot },
			{ "Charm", CharmSlot }  // New Charm slot
		};
	}
	private void OnFileMenuClicked(object? sender, EventArgs e)
	{
		Console.WriteLine("File menu clicked!");
		System.Diagnostics.Debug.WriteLine("File menu clicked!");
		FileMenu.IsVisible = !FileMenu.IsVisible;
		Console.WriteLine($"File menu visibility set to: {FileMenu.IsVisible}");
	}
	private async void OnCloseMenuClicked(object? sender, EventArgs e)
	{
		Console.WriteLine("Close menu clicked!");
		System.Diagnostics.Debug.WriteLine("Close menu clicked!");
		await DisplayAlert("Info", "Closing menu...", "OK");
		FileMenu.IsVisible = false;
	}
	private async void OnOpenFileClicked(object? sender, EventArgs e)
	{
		Console.WriteLine("Open file clicked!");
		System.Diagnostics.Debug.WriteLine("Open file clicked!");
		try
		{
			// Add debug feedback
			await DisplayAlert("Info", "Opening file picker...", "OK");

			// Simplified file picker that should work better on macOS
			var result = await FilePicker.PickAsync(new PickOptions
			{
				PickerTitle = "Select Inventory File"
			});

			if (result != null)
			{
				await LoadInventoryFile(result);
				FileMenu.IsVisible = false;
			}
			else
			{
				await DisplayAlert("Info", "No file selected", "OK");
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
			ParseInventoryData(content);            // Update UI
			FileNameLabel.Text = $"File: {file.FileName}";
			MainContent.IsVisible = false;
			InventoryView.IsVisible = true;
			InventoryView.InputTransparent = false;
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
					else if (location == "Ring" || location == "Fingers")
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
		EarSlot2.Text = "EAR";
		EarSlot2.TextColor = Colors.White;
		WristSlot2.Text = "WRIST";
		WristSlot2.TextColor = Colors.White;
		RingSlot2.Text = "RING";
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
		if (slot == HeadSlot) return "HEAD";
		if (slot == FaceSlot) return "FACE";
		if (slot == NeckSlot) return "NECK";
		if (slot == EarSlot1) return "EAR";
		if (slot == ShoulderSlot) return "SHLD";
		if (slot == ArmsSlot) return "ARMS";
		if (slot == WristSlot1) return "WRIST";
		if (slot == HandsSlot) return "HANDS";
		if (slot == ChestSlot) return "CHEST";
		if (slot == BackSlot) return "BACK";
		if (slot == WaistSlot) return "WAIST";
		if (slot == LegsSlot) return "LEGS";
		if (slot == FeetSlot) return "FEET";
		if (slot == RingSlot1) return "RING";
		if (slot == PrimarySlot) return "PRI";
		if (slot == SecondarySlot) return "SEC";
		if (slot == RangeSlot) return "RNG";
		if (slot == AmmoSlot) return "AMMO";
		if (slot == CharmSlot) return "CHARM";
		return "Empty";
	}
	private void LoadDefaultInventory()
	{
		try
		{           // Create a default inventory with sample EverQuest items
			var defaultInventoryData = @"Location	Name	ID	Count	Slots
Charm	Intricate Wooden Figurine	12034	1	6
Head	Crown of the Froglok King	12034	1	8
Face	Mask of the Ancients	19041	1	8
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

			ParseInventoryData(defaultInventoryData);           // Update UI to show character sheet
			FileNameLabel.Text = "Default EverQuest Character";
			MainContent.IsVisible = false;
			InventoryView.IsVisible = true;
			InventoryView.InputTransparent = false;
		}
		catch (Exception ex)
		{
			// Since this is no longer async, we'll just use console output for errors
			System.Diagnostics.Debug.WriteLine($"Failed to load default inventory: {ex.Message}");
		}
	}
	private async void OnLoadDefaultClicked(object? sender, EventArgs e)
	{
		Console.WriteLine("Load default clicked!");
		System.Diagnostics.Debug.WriteLine("Load default clicked!");
		try
		{
			await DisplayAlert("Info", "Loading default character...", "OK");
			LoadDefaultInventory();
			FileMenu.IsVisible = false;
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to load default character: {ex.Message}", "OK");
		}
	}
}
