# THJ Inventory Reader

A .NET MAUI cross-platform application for reading and managing EverQuest character inventory files.

## Features

- **Simple File-Based Interface**: Clean menu-driven UI for opening inventory files
- **EverQuest Inventory Support**: Reads tab-delimited inventory dump files
- **Filtered Display**: Shows main equipment items (up to Ammo) with Location and Name columns
- **Cross-Platform**: Runs on Windows, macOS, Android, and iOS

## Development Setup

### Prerequisites

- .NET 9 SDK
- Visual Studio 2022 (Windows) or Visual Studio for Mac/VS Code (macOS)
- .NET MAUI workload installed

### Getting Started

1. Clone the repository
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
3. Build the project:
   ```bash
   dotnet build
   ```

### Platform Support

This application is designed to run on:

- Windows 10/11 (Primary target)
- Android
- iOS
- macOS (Mac Catalyst)

Note: Currently configured to target Windows primarily for development.

## Usage

1. Launch the application
2. Click "File" in the menu bar
3. Select "Open Inventory File"
4. Choose your EverQuest inventory dump text file
5. View the parsed inventory data showing equipped items

## Inventory File Format

The application expects tab-delimited text files with the following format:

```
Location	Name	ID	Count	Slots
Charm	Intricate Wooden Figurine (Legendary)	2035004	1	6
...
```

Currently displays main equipment slots up to and including the Ammo slot.

## Project Structure

- `MainPage.xaml/cs` - Main application interface with file menu and inventory display
- `SampleInventory.txt` - Example EverQuest inventory file for testing
- `App.xaml/cs` - Application entry point
- `AppShell.xaml/cs` - Application shell and navigation
- `Platforms/` - Platform-specific code
- `Resources/` - Images, fonts, styles, and other resources

1. Push to GitHub repository
2. Pull on Windows development machine
3. Implement inventory reading functionality
4. Add UI for inventory management
