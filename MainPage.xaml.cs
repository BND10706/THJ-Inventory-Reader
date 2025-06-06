using System.Collections.ObjectModel;
using THJ_Inventory_Reader.Models;

namespace THJ_Inventory_Reader;

public partial class MainPage : ContentPage
{
	// Player inventory and collections
	private PlayerInventory playerInventory = new PlayerInventory();
	private ObservableCollection<InventoryItem> inventoryItems = new ObservableCollection<InventoryItem>();
	private ObservableCollection<AltCurrency> altCurrencies = new ObservableCollection<AltCurrency>();

	public MainPage()
	{
		InitializeComponent();

		// Set up the collection views
		InventoryItemsView.ItemsSource = inventoryItems;
		AltCurrencyListView.ItemsSource = altCurrencies;

		LoadDefaultInventory();
	}

	private async void OnOpenFileClicked(object? sender, EventArgs e)
	{
		try
		{
			// Simplified file picker
			var result = await FilePicker.PickAsync(new PickOptions
			{
				PickerTitle = "Select Inventory File"
			});

			if (result != null)
			{
				await LoadInventoryFile(result);
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
			FileNameLabel.Text = $"File: {file.FileName}";

			// Hide main menu and show the inventory window
			MainContent.IsVisible = false;
			InventoryView.IsVisible = true;
			InventoryView.InputTransparent = false;

			// Update the duplicate label in case there are two display areas
			if (FileNameLabel2 != null)
			{
				FileNameLabel2.Text = FileNameLabel.Text;
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to load inventory file: {ex.Message}", "OK");
		}
	}

	private void ParseInventoryData(string content)
	{
		// Clear the inventory
		playerInventory.Clear();
		inventoryItems.Clear();
		altCurrencies.Clear();

		// Parse the data
		var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

		// Skip header line if present
		int startIndex = 0;
		if (lines.Length > 0 && lines[0].Contains("Location") && lines[0].Contains("Name") && lines[0].Contains("ID"))
		{
			startIndex = 1;
		}

		// Process each line
		for (int i = startIndex; i < lines.Length; i++)
		{
			var line = lines[i].Trim();
			if (string.IsNullOrEmpty(line)) continue;

			var parts = line.Split('\t');
			if (parts.Length >= 5) // Full format with Location, Name, ID, Count, Slots
			{
				var item = new InventoryItem
				{
					Location = parts[0],
					Name = parts[1],
					ID = int.TryParse(parts[2], out var id) ? id : 0,
					Count = int.TryParse(parts[3], out var count) ? count : 0,
					Slots = int.TryParse(parts[4], out var slots) ? slots : 0
				};

				playerInventory.AddEquipmentItem(item);
			}
			else if (parts.Length >= 2) // Simple format with just Location and Name
			{
				var item = new InventoryItem
				{
					Location = parts[0],
					Name = parts[1],
					Count = 1
				};

				playerInventory.AddEquipmentItem(item);
			}
		}

		// Update the UI with equipment data
		UpdateEquipmentUI();

		// Update the inventory grid
		UpdateInventoryItems();

		// For demo purposes, add some alt currency
		if (altCurrencies.Count == 0)
		{
			altCurrencies.Add(new AltCurrency { Name = "Doubloons", Amount = 542 });
			altCurrencies.Add(new AltCurrency { Name = "Marks of Valor", Amount = 1250 });
			altCurrencies.Add(new AltCurrency { Name = "Chronobines", Amount = 375 });
		}
	}

	private void UpdateEquipmentUI()
	{
		// Update head slot
		UpdateSlotDisplay(HeadSlot, playerInventory.Head);

		// Update face slot
		UpdateSlotDisplay(FaceSlot, playerInventory.Face);

		// Update ear slots
		UpdateSlotDisplay(EarSlot1, playerInventory.LeftEar);
		UpdateSlotDisplay(EarSlot2, playerInventory.RightEar);

		// Update neck slot
		UpdateSlotDisplay(NeckSlot, playerInventory.Neck);

		// Update shoulder slot
		UpdateSlotDisplay(ShoulderSlot, playerInventory.Shoulders);

		// Update arms and back slots
		UpdateSlotDisplay(ArmsSlot, playerInventory.Arms);
		UpdateSlotDisplay(BackSlot, playerInventory.Back);

		// Update chest and waist slots
		UpdateSlotDisplay(ChestSlot, playerInventory.Chest);
		UpdateSlotDisplay(WaistSlot, playerInventory.Waist);

		// Update wrist slots
		UpdateSlotDisplay(WristSlot1, playerInventory.LeftWrist);
		UpdateSlotDisplay(WristSlot2, playerInventory.RightWrist);

		// Update hands, legs, and feet
		UpdateSlotDisplay(HandsSlot, playerInventory.Hands);
		UpdateSlotDisplay(LegsSlot, playerInventory.Legs);
		UpdateSlotDisplay(FeetSlot, playerInventory.Feet);

		// Update ring slots
		UpdateSlotDisplay(RingSlot1, playerInventory.LeftRing);
		UpdateSlotDisplay(RingSlot2, playerInventory.RightRing);

		// Update weapon slots
		UpdateSlotDisplay(PrimarySlot, playerInventory.Primary);
		UpdateSlotDisplay(SecondarySlot, playerInventory.Secondary);

		// Update range and ammo slots
		UpdateSlotDisplay(RangeSlot, playerInventory.Range);
		UpdateSlotDisplay(AmmoSlot, playerInventory.Ammo);

		// Update charm slot
		UpdateSlotDisplay(CharmSlot, playerInventory.Charm);

		// Update the stats panel
		UpdateStatsPanel();
	}

	private void UpdateSlotDisplay(Label slotLabel, InventoryItem item)
	{
		if (item.IsEmpty)
		{
			// Reset to default
			slotLabel.Text = GetSlotDefaultText(slotLabel);
			slotLabel.TextColor = Colors.White;
			slotLabel.FontSize = 8;
		}
		else
		{
			// Show item name
			slotLabel.Text = item.GetShortName();
			slotLabel.TextColor = Color.FromArgb(item.GetItemColor());
			slotLabel.FontSize = 8;
		}
	}

	private void UpdateInventoryItems()
	{
		inventoryItems.Clear();

		// Add all non-empty items from the player inventory
		foreach (var item in playerInventory.Items)
		{
			if (!item.IsEmpty)
			{
				inventoryItems.Add(item);
			}
		}
	}

	private void UpdateStatsPanel()
	{
		// Update the stats labels with player inventory stats
		StatSTR.Text = playerInventory.Strength.ToString();
		StatSTA.Text = playerInventory.Stamina.ToString();
		StatAGI.Text = playerInventory.Agility.ToString();
		StatDEX.Text = playerInventory.Dexterity.ToString();
		StatWIS.Text = playerInventory.Wisdom.ToString();
		StatINT.Text = playerInventory.Intelligence.ToString();
		StatCHA.Text = playerInventory.Charisma.ToString();

		StatHP.Text = $"{playerInventory.CurrentHP}/{playerInventory.MaxHP}";
		StatMANA.Text = $"{playerInventory.CurrentMana}/{playerInventory.MaxMana}";
		StatENDUR.Text = $"{playerInventory.CurrentEndurance}/{playerInventory.MaxEndurance}";
	}

	private string GetSlotDefaultText(Label slot)
	{
		// Extract the default text from the label name
		var name = slot.StyleId ?? slot.AutomationId ?? slot.Text;

		if (name?.Contains("Slot") == true)
		{
			// Extract the base name without "Slot" and any numbers
			var baseName = new string(name.TakeWhile(c => !char.IsDigit(c)).ToArray()).Replace("Slot", "").ToUpper();
			return baseName;
		}

		return slot.Text;
	}

	private void LoadDefaultInventory()
	{
		// Generate some sample data
		var sampleData = @"LocationNameIDCountSlots
CharmGolden Orb of Power1000111
HeadHelm of the Ancients1000211
FaceVeil of Shadows1000311
EarRuby Earring1000411
EarSapphire Earring1000511
NeckAmulet of Protection1000611
ShouldersPauldrons of Might1000711
ArmsBracers of Quickness1000811
BackCape of Invisibility1000911
ChestBreastplate of Valor1001011
WaistBelt of Giants1001111
WristWristguard of Defense1001211
WristBracelet of Speed1001311
HandsGloves of Dexterity1001411
LegsGreaves of Stability1001511
FeetBoots of Swiftness1001611
RingRing of Power (Legendary)2001711
RingBand of Protection (Enchanted)1001811
PrimarySword of Truth1001911
SecondaryShield of Faith1002011
RangeBow of Accuracy1002111
AmmoQuiver of Endless Arrows10022201";

		ParseInventoryData(sampleData);

		// Set some character stats for the demo
		playerInventory.Strength = 250;
		playerInventory.Stamina = 220;
		playerInventory.Agility = 180;
		playerInventory.Dexterity = 200;
		playerInventory.Wisdom = 150;
		playerInventory.Intelligence = 160;
		playerInventory.Charisma = 130;

		playerInventory.CurrentHP = 2500;
		playerInventory.MaxHP = 2500;
		playerInventory.CurrentMana = 1800;
		playerInventory.MaxMana = 2000;
		playerInventory.CurrentEndurance = 900;
		playerInventory.MaxEndurance = 1000;

		UpdateEquipmentUI();
	}

	private async void OnLoadDefaultClicked(object? sender, EventArgs e)
	{
		try
		{
			LoadDefaultInventory();

			// Hide main menu and show the inventory window
			MainContent.IsVisible = false;
			InventoryView.IsVisible = true;
			InventoryView.InputTransparent = false;

			// Update the filename label to indicate we're using default inventory
			FileNameLabel.Text = "File: Default Sample Inventory";
			if (FileNameLabel2 != null)
			{
				FileNameLabel2.Text = FileNameLabel.Text;
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to load default inventory: {ex.Message}", "OK");
		}
	}

	private async void OnExitClicked(object? sender, EventArgs e)
	{
		// Close the application
		Application.Current?.Quit();
	}

	private void OnCloseButtonClicked(object? sender, EventArgs e)
	{
		// Hide inventory view and show main menu
		MainContent.IsVisible = true;
		InventoryView.IsVisible = false;
		InventoryView.InputTransparent = true;
	}
}
