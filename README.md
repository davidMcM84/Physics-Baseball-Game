# Physics Baseball Game (WPF / .NET 9)

Early-stage prototype of a physics-oriented baseball batting / player simulation game using WPF (.NET 9 / C# 13). The current focus is establishing project structure (MVVM), navigation, and a Player Profile editor with visual rating bars.

## Current Features

- Full-screen borderless start screen (MainWindow) with basic navigation buttons (Start, Options placeholder, Exit).
- In-memory navigation from Start Screen to Player Profile view (no navigation framework yet).
- Domain Model layer (Models) with:
  - `BallPlayer` abstract base class.
  - `BasicPlayer` concrete implementation for quick testing.
  - Enumerations: `PlayerPosition`, `PlayerRole`, `BatHand`, `ThrowHand`.
  - `PhysicalAttributes` record struct capturing measurable attributes (e.g., sprint speed, bat speed, reaction time).
- ViewModels layer with shared base:
  - `ViewModelBase` implementing `INotifyPropertyChanged` + `SetProperty` helper.
  - `PlayerProfileViewModel` providing editable player data and change tracking (Save/Cancel logic with snapshot diff + CanSave support).
- Commands:
  - `RelayCommand` for lightweight ICommand implementations.
- Views:
  - `PlayerProfileView` redesigned to look more like a modern web profile page (two-column layout + rating bars).
  - Progress-style rating bars visualize raw numeric metrics scaled into a 0–100 range via converter.
- Converters:
  - `ScaleConverter` turns raw numeric values (with min|max|(invert) parameter) into a normalized percentage for bar display (supports inverted scale for stats like Reaction Time where lower is better).
- Styling / UI Enhancements:
  - Custom gradients, compact inputs, rating bar template.
  - Themed card-like panels and action bar.

## Tech Stack

- .NET 9 (WPF)
- C# 13 language features enabled
- MVVM (manual, lightweight) without external MVVM frameworks (e.g., no Prism / MVVM Light yet)

## Project Structure

```
/Physics Baseball Game
  /Models
  /ViewModels
  /Views
  /Commands
  /Converters
  MainWindow.xaml / .cs
  Physics Baseball Game.csproj
README.md
```

## How Navigation Works (Current Minimal Approach)

`MainWindow` hosts two logical layers:
1. Start Screen (Grid named `StartScreenRoot`)
2. Dynamic `ContentControl` (`PageHost`) for pages like PlayerProfileView.

The Start Game button instantiates a temporary `BasicPlayer`, wraps it in a `PlayerProfileViewModel`, and displays the `PlayerProfileView`. Esc key returns to the start screen (or exits if already there).

## Player Profile View

The right panel shows metrics using `ProgressBar` controls styled to look like rating bars. Scaling rules (examples):
- Sprint Speed: `0|40`
- Acceleration: `0|10`
- Bat Speed: `0|120`
- Arm Strength: `0|110`
- Reaction Time: `0.08|0.35|invert` (lower raw value => higher normalized bar)

Adjust these ranges later to reflect simulation calibration.

## Building & Running

Prerequisites:
- .NET 9 SDK (preview if not RTM yet)
- Windows (WPF requirement)

Build:
```
dotnet build
```
Run:
```
dotnet run --project "Physics Baseball Game/Physics Baseball Game.csproj"
```

## Planned / Suggested Next Steps

Short Term:
- Add a Players List / Roster screen before profile editing.
- Introduce validation (numeric ranges, defensive parsing) in `PlayerProfileViewModel`.
- Move shared brushes/styles into `App.xaml` resource dictionaries.
- Implement persistent storage (start with JSON repository for player data; later optional SQLite/EF Core).

Mid Term:
- Add additional skill domains (plate discipline, contact ability, pitch recognition, etc.).
- Create simulation engine skeleton (swing timing, bat-ball collision, trajectory estimation).
- Introduce a navigation service abstraction (to decouple MainWindow from page logic).
- Add unit tests for converters and view models.

Long Term:
- Physics engine integration (spin, exit velocity, launch angle modeling).
- Procedural player generation and progression system.
- Modular stat overlays and dynamic dashboards.
- Save game / career mode architecture.

## Design Considerations

- The model layer is kept UI-agnostic; view models handle change notification.
- Value objects (`PhysicalAttributes`) are immutable (record struct) for clarity and potential thread safety.
- Ratings scaling is deliberately parameterized through the converter to avoid hardcoding UI-specific logic in the ViewModel.

## Known Limitations / Open Issues

- No persistence layer yet (all data ephemeral).
- Converter ranges are arbitrary placeholders pending real-world tuning.
- No input validation (invalid numeric text could throw binding conversion warnings in output window).
- XAML designer may occasionally fail to resolve converter (clean/rebuild resolves; runtime OK). Setting an explicit `<AssemblyName>` solved initial namespace resolution conflict.

## Contributing (Future)

When expanding:
- Keep domain logic (physics, stat calculations) separate from UI-specific code.
- Favor dependency injection once services (repositories, navigation, simulation) emerge.
- Consider separating core simulation into a class library for potential reuse or testing.

## License

(Not specified yet.) Add a LICENSE file before publishing publicly.

---
Feel free to request refinements (e.g., add screenshots section, CI build badge, or architectural diagrams) as the project evolves.
