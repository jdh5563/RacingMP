/// <summary>
/// Enum to represent the different Menu options.
/// </summary>
public enum Menus
{
    None,
    MainMenu,
    LoadingScreen,
    PauseMenu,
    SettingsMenu,
    HostMenu,
    JoinMenu,
    Credits,
    MultiplayerDisplay
}

/// <summary>
/// Enum to represent the different menu button options.
/// </summary>
public enum MenuButtonTypes
{
    Back,
    Credits,
    Resume,
    Settings,
    Host,
    HostServer,
    Join,
    Quit
}

/// <summary>
/// Enum to represent the different menu input types.
/// </summary>
public enum MenuInputTypes
{
    Point,
    LeftClick,
    MiddleClick,
    RightClick,
    ScrollWheel,
    Move,
    Submit,
    Cancel
}

/// <summary>
/// Enum to represent the different slider types in the menus.
/// </summary>
public enum SliderTypes
{
    MasterVolume,
    SFXVolume,
    MusicVolume
}

public enum LoadingBarTypes
{
    SmoothLinear,
    ChunkLinear,
    SmoothRadial,
    ChunkRadial,
    Pulse,
    Percentage,
}