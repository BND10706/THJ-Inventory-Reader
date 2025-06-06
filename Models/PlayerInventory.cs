using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace THJ_Inventory_Reader.Models
{
    public class PlayerInventory
    {
        // Main equipment slots
        public InventoryItem Charm { get; set; } = new InventoryItem { Location = "Charm" };
        public InventoryItem LeftEar { get; set; } = new InventoryItem { Location = "Ear" }; // First ear slot
        public InventoryItem RightEar { get; set; } = new InventoryItem { Location = "Ear" }; // Second ear slot
        public InventoryItem Head { get; set; } = new InventoryItem { Location = "Head" };
        public InventoryItem Face { get; set; } = new InventoryItem { Location = "Face" };
        public InventoryItem Neck { get; set; } = new InventoryItem { Location = "Neck" };
        public InventoryItem Shoulders { get; set; } = new InventoryItem { Location = "Shoulders" };
        public InventoryItem Arms { get; set; } = new InventoryItem { Location = "Arms" };
        public InventoryItem Back { get; set; } = new InventoryItem { Location = "Back" };
        public InventoryItem Chest { get; set; } = new InventoryItem { Location = "Chest" };
        public InventoryItem Waist { get; set; } = new InventoryItem { Location = "Waist" };
        public InventoryItem LeftWrist { get; set; } = new InventoryItem { Location = "Wrist" }; // First wrist slot
        public InventoryItem RightWrist { get; set; } = new InventoryItem { Location = "Wrist" }; // Second wrist slot
        public InventoryItem Hands { get; set; } = new InventoryItem { Location = "Hands" };
        public InventoryItem Legs { get; set; } = new InventoryItem { Location = "Legs" };
        public InventoryItem Feet { get; set; } = new InventoryItem { Location = "Feet" };
        public InventoryItem LeftRing { get; set; } = new InventoryItem { Location = "Ring" }; // First ring slot
        public InventoryItem RightRing { get; set; } = new InventoryItem { Location = "Ring" }; // Second ring slot
        public InventoryItem Primary { get; set; } = new InventoryItem { Location = "Primary" };
        public InventoryItem Secondary { get; set; } = new InventoryItem { Location = "Secondary" };
        public InventoryItem Range { get; set; } = new InventoryItem { Location = "Range" };
        public InventoryItem Ammo { get; set; } = new InventoryItem { Location = "Ammo" };

        // Character stats
        public int Strength { get; set; }
        public int Stamina { get; set; }
        public int Agility { get; set; }
        public int Dexterity { get; set; }
        public int Wisdom { get; set; }
        public int Intelligence { get; set; }
        public int Charisma { get; set; }

        // Health/Mana/Endurance
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public int CurrentMana { get; set; }
        public int MaxMana { get; set; }
        public int CurrentEndurance { get; set; }
        public int MaxEndurance { get; set; }

        // Alternative currency
        public ObservableCollection<AltCurrency> AltCurrencies { get; set; } = new ObservableCollection<AltCurrency>();

        // All inventory items (for the inventory grid)
        public ObservableCollection<InventoryItem> Items { get; set; } = new ObservableCollection<InventoryItem>();

        // Dictionary of equipped items by slot for easy access
        private Dictionary<string, InventoryItem> EquippedItems { get; set; } = new Dictionary<string, InventoryItem>();

        public InventoryItem GetEquipmentBySlot(string slotName)
        {
            if (EquippedItems.ContainsKey(slotName))
            {
                return EquippedItems[slotName];
            }
            return new InventoryItem { Location = slotName };
        }

        public void AddEquipmentItem(InventoryItem item)
        {
            // First, check if this is an equipped item
            if (item.IsMainSlot)
            {
                // Map the location to the appropriate property
                switch (item.Location)
                {
                    case "Charm":
                        Charm = item;
                        break;
                    case "Head":
                        Head = item;
                        break;
                    case "Face":
                        Face = item;
                        break;
                    case "Ear": // Handle multiple ear slots
                        if (LeftEar.IsEmpty)
                            LeftEar = item;
                        else
                            RightEar = item;
                        break;
                    case "Neck":
                        Neck = item;
                        break;
                    case "Shoulders":
                        Shoulders = item;
                        break;
                    case "Arms":
                        Arms = item;
                        break;
                    case "Back":
                        Back = item;
                        break;
                    case "Chest":
                        Chest = item;
                        break;
                    case "Waist":
                        Waist = item;
                        break;
                    case "Wrist": // Handle multiple wrist slots
                        if (LeftWrist.IsEmpty)
                            LeftWrist = item;
                        else
                            RightWrist = item;
                        break;
                    case "Hands":
                        Hands = item;
                        break;
                    case "Legs":
                        Legs = item;
                        break;
                    case "Feet":
                        Feet = item;
                        break;
                    case "Ring":
                    case "Fingers": // Handle multiple ring slots
                        if (LeftRing.IsEmpty)
                            LeftRing = item;
                        else
                            RightRing = item;
                        break;
                    case "Primary":
                        Primary = item;
                        break;
                    case "Secondary":
                        Secondary = item;
                        break;
                    case "Range":
                        Range = item;
                        break;
                    case "Ammo":
                        Ammo = item;
                        break;
                }

                // Add to the equipped items dictionary
                if (!EquippedItems.ContainsKey(item.Location))
                {
                    EquippedItems[item.Location] = item;
                }
            }

            // Add to the general items collection
            if (!item.IsEmpty)
            {
                Items.Add(item);
            }
        }

        public void Clear()
        {
            // Reset all equipment slots
            Charm = new InventoryItem { Location = "Charm" };
            LeftEar = new InventoryItem { Location = "Ear" };
            RightEar = new InventoryItem { Location = "Ear" };
            Head = new InventoryItem { Location = "Head" };
            Face = new InventoryItem { Location = "Face" };
            Neck = new InventoryItem { Location = "Neck" };
            Shoulders = new InventoryItem { Location = "Shoulders" };
            Arms = new InventoryItem { Location = "Arms" };
            Back = new InventoryItem { Location = "Back" };
            Chest = new InventoryItem { Location = "Chest" };
            Waist = new InventoryItem { Location = "Waist" };
            LeftWrist = new InventoryItem { Location = "Wrist" };
            RightWrist = new InventoryItem { Location = "Wrist" };
            Hands = new InventoryItem { Location = "Hands" };
            Legs = new InventoryItem { Location = "Legs" };
            Feet = new InventoryItem { Location = "Feet" };
            LeftRing = new InventoryItem { Location = "Ring" };
            RightRing = new InventoryItem { Location = "Ring" };
            Primary = new InventoryItem { Location = "Primary" };
            Secondary = new InventoryItem { Location = "Secondary" };
            Range = new InventoryItem { Location = "Range" };
            Ammo = new InventoryItem { Location = "Ammo" };

            // Clear collections
            EquippedItems.Clear();
            Items.Clear();
            AltCurrencies.Clear();

            // Reset stats
            Strength = 0;
            Stamina = 0;
            Agility = 0;
            Dexterity = 0;
            Wisdom = 0;
            Intelligence = 0;
            Charisma = 0;

            CurrentHP = 0;
            MaxHP = 0;
            CurrentMana = 0;
            MaxMana = 0;
            CurrentEndurance = 0;
            MaxEndurance = 0;
        }
    }

    public class AltCurrency
    {
        public string Name { get; set; } = string.Empty;
        public int Amount { get; set; }
    }
}
