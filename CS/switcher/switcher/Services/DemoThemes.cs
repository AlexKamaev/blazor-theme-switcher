using System.Globalization;
using DevExpress.Blazor;

namespace switcher.Services;

public interface IThemeChangeRequestDispatcher {
    void RequestThemeChange(DemoTheme demoTheme);
}

public interface IThemeLoadNotifier {
    Task NotifyThemeLoadedAsync(DemoTheme demoTheme);
}

public class DemoTheme : ITheme {
    private readonly ITheme _theme;
    public DemoTheme(ITheme theme) {
        _theme = theme;
    }
    
    public string Title => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Name.Replace("-", " "));

    public bool IsFluent => _theme is DxThemeFluent;
    public bool IsBootstrapDark { get; init; } = false;

    public string Name => _theme.Name;

    public List<string> GetFilePaths() {
        return _theme.GetFilePaths();
    }
}

public static class DemoThemes
{
    private const string BootstrapPath = "css/bootstrap/bootstrap.min.css";
    private const string FluentCustomCssPath = "css/site-fluent.css";
    private const string FluentLightAdditionalCssPath = "switcher-resources/css/fluent-light.min.css";
    private const string FluentDarkAdditionalCssPath = "switcher-resources/css/fluent-dark.min.css";
    
    public static readonly DemoTheme BlazingBerry = new(Themes.BlazingBerry.Clone(properties => properties.Name = "blazing-berry"));
    public static readonly DemoTheme BlazingDark = new(Themes.BlazingDark.Clone(properties => properties.Name = "blazing-dark"));
    public static readonly DemoTheme Purple = new(Themes.Purple);
    public static readonly DemoTheme OfficeWhite = new(Themes.OfficeWhite.Clone(properties => properties.Name = "office-white"));

    public static readonly DemoTheme Bootstrap = new(Themes.BootstrapExternal.Clone(properties => {
        properties.Name = "default";

        properties.AddFilePaths(BootstrapPath);
    }));
    
    public static readonly DemoTheme BootstrapDark = new(Themes.BootstrapExternal.Clone(properties => {
        properties.Name = "default-dark";

        properties.AddFilePaths(BootstrapPath);
    })) { IsBootstrapDark = true };

    public static readonly DemoTheme FluentLight = new(Themes.Fluent.Clone(properties => {
        properties.Name = "fluent-light";

        properties.AddFilePaths(
            FluentCustomCssPath,
            FluentLightAdditionalCssPath);
    }));

    public static readonly DemoTheme FluentDark = new(Themes.Fluent.Clone(properties => {
        properties.Name = "fluent-dark";
        properties.Mode = ThemeMode.Dark;
        
        properties.AddFilePaths(
            FluentCustomCssPath, 
            FluentDarkAdditionalCssPath);
    }));
}

public class DemoThemeService {
    public const string ThemeCookieKey = "DXBZCurrentTheme";
    public List<ThemeSet> ThemeSets { get; } = CreateSets();
    public DemoTheme ActiveTheme { get; private set; } = DemoThemes.BlazingBerry;
    
    public IThemeLoadNotifier ThemeLoadNotifier { get; set; }
    public IThemeChangeRequestDispatcher ThemeChangeRequestDispatcher { get; set; }

    public void SetActiveThemeByName(string themeName) {
        var theme = FindThemeByName(themeName);

        ActiveTheme = theme ?? DemoThemes.BlazingBerry;
    }

    private DemoTheme? FindThemeByName(string themeName) {
        var themes = ThemeSets.SelectMany(ts => ts.Themes);
        
        return themes.SingleOrDefault(theme => theme.Name == themeName);
    }

    public class ThemeSet(string title, params DemoTheme[] themes) {
        public string Title { get; } = title;
        public DemoTheme[] Themes { get; } = themes;
    }

    private static List<ThemeSet> CreateSets() {
        return
        [
            new ThemeSet("DevExpress Themes", DemoThemes.BlazingBerry, DemoThemes.BlazingDark, DemoThemes.Purple, DemoThemes.OfficeWhite),
            new ThemeSet("Bootstrap Themes", DemoThemes.Bootstrap, DemoThemes.BootstrapDark),
            new ThemeSet("Fluent Themes", DemoThemes.FluentLight, DemoThemes.FluentDark)
        ];
    }
}
