# DevExpress Blazor Integration - ITSM.Web

## Overview

The ITSM.Web project has been successfully configured to use DevExpress UI Components for Blazor.

## Files Modified/Created

### 1. **ITSM.Web.csproj**
   - Added `DevExpress.Blazor` NuGet package (v26.1.5)

### 2. **Program.cs**
   - Added `using DevExpress.Blazor;`
   - Registered DevExpress Blazor services: `builder.Services.AddDevExpressBlazor();`

### 3. **Components/_Imports.razor**
   - Added `@using DevExpress.Blazor;` namespace declaration

### 4. **Components/App.razor**
   - Added DevExpress theme registration: `@DxResourceManager.RegisterTheme(Themes.Fluent)`
   - Added DevExpress scripts: `@DxResourceManager.RegisterScripts()`

### 5. **Components/Pages/DevExpressDemo.razor** (NEW)
   - Sample page demonstrating DevExpress components
   - Available at `/devexpress-demo` route
   - Includes Calendar and DateEdit component examples

## Available Components

DevExpress Blazor UI Library includes:

### Data Management
- **Grid** - Powerful data grid with sorting, filtering, grouping
- **TreeList** - Hierarchical data display
- **Filter Builder** - Advanced filtering interface
- **Pivot Table** - Data analysis and summarization

### Data Visualization
- **Chart** - Comprehensive chart library
- **Map** - Geographic data visualization
- **Dashboard** - Dashboard building
- **Sparkline** - In-cell charts

### Data Editors
- **Calendar** - Date selection calendar
- **DateEdit** - Date input component
- **ComboBox** - Dropdown list
- **TextBox** - Text input
- **CheckBox** - Checkbox control
- **And many more...**

### Scheduling
- **Scheduler** - Calendar event management

### Layouts
- **Splitter** - Resizable panel regions
- **Tabs** - Tabbed interface
- **Popup** - Modal dialogs
- **Accordion** - Collapsible sections

## Usage Example

```razor
@page "/my-page"
@rendermode InteractiveServer

<DxCalendar @bind-SelectedDate="@SelectedDate" />

@code {
	DateTime SelectedDate { get; set; } = DateTime.Now;
}
```

## Important Notes

1. **Interactive Render Mode Required**: Most DevExpress components require `@rendermode InteractiveServer` or `@rendermode InteractiveWebAssembly`

2. **Theme Selection**: Currently configured with `Themes.Fluent`. You can change to:
   - `Themes.Bootstrap`
   - `Themes.Office2019Black`
   - `Themes.Office2019Colorful`
   - And more...

3. **Licensing**: Ensure you have a valid DevExpress license or use the 30-day trial

## Next Steps

1. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Run the application and navigate to `/devexpress-demo` to see the demo

4. Visit the [DevExpress Blazor Documentation](https://docs.devexpress.com/Blazor/) for detailed component documentation

## Resources

- [DevExpress Blazor Components](https://www.devexpress.com/blazor/)
- [Documentation](https://docs.devexpress.com/Blazor/)
- [Live Demos](https://demos.devexpress.com/blazor/)
- [Support & Issue Tracker](https://www.devexpress.com/Support/)

## Configuration Details

The project is configured for:
- **.NET 10.0** target framework
- **Blazor Server** hosting model with interactive components
- **Fluent** design theme
- All necessary scripts and stylesheets loaded via `DxResourceManager`

Enjoy building rich user interfaces with DevExpress Blazor components! 🚀
