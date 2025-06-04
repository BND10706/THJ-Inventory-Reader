# THJ Inventory Reader

A .NET MAUI cross-platform application for inventory management and reading.

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

### Development Notes

- The project is set up for cross-platform development
- Developed on macOS, targeting Windows as primary platform
- Uses .NET MAUI for native cross-platform UI
- Ready for deployment to Windows machines

## Project Structure

- `MainPage.xaml/cs` - Main application page
- `App.xaml/cs` - Application entry point
- `AppShell.xaml/cs` - Application shell and navigation
- `Platforms/` - Platform-specific code
- `Resources/` - Images, fonts, styles, and other resources

## Next Steps

1. Push to GitHub repository
2. Pull on Windows development machine
3. Implement inventory reading functionality
4. Add UI for inventory management
