using System;

namespace THJ_Inventory_Reader.Models
{
    public class InventoryItem
    {
        public string Location { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ID { get; set; }
        public int Count { get; set; }
        public int Slots { get; set; }
        public bool IsEmpty => ID == 0 && Count == 0;

        // True if item is equipped in a main slot (not a -Slot# item)
        public bool IsMainSlot => !Location.Contains("-Slot");

        // For item quality display
        public bool IsLegendary => Name.Contains("(Legendary)");
        public bool IsEnchanted => Name.Contains("(Enchanted)");

        // Calculate item color based on properties
        public string GetItemColor()
        {
            if (IsEmpty) return "#FFFFFF"; // White for empty slots
            if (IsLegendary) return "#FFD700"; // Gold for legendary
            if (IsEnchanted) return "#9370DB"; // Purple for enchanted
            return "#FFFFFF"; // White default
        }

        // Get shortened name for display in UI
        public string GetShortName()
        {
            // If item is empty, return "Empty"
            if (IsEmpty) return "Empty";

            // Remove any quality suffix from the name for cleaner display
            string displayName = Name.Replace("(Legendary)", "").Replace("(Enchanted)", "").Trim();

            // If name is too long, truncate it
            if (displayName.Length > 15)
            {
                return displayName.Substring(0, 12) + "...";
            }

            return displayName;
        }
    }
}
