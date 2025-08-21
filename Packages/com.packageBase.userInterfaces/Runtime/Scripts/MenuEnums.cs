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
/// Enum to represent the different slider types in the menus.
/// </summary>
public enum SliderTypes
{
    MasterVolume,
    SFXVolume,
    MusicVolume
}

/// <summary>
/// Enum to represent the different types of loading bars that can exist in the project.
/// </summary>
public enum LoadingBarTypes
{
    SmoothLinear,
    ChunkLinear,
    SmoothRadial,
    ChunkRadial,
    Pulse,
    Percentage,
}