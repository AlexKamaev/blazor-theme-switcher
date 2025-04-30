using DevExpress.Blazor;
using DevExpress.Blazor.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using switcher.Services;

public class ThemeJsChangeDispatcher : ComponentBase, IThemeChangeRequestDispatcher, IAsyncDisposable {
    [Parameter] public string InitialThemeName { get; set; }

    [Inject] private ISafeJSRuntime JsRuntime { get; set; }

    [Inject] private DemoThemeService ThemesService { get; set; }
    [Inject] protected IThemeChangeService DxThemesService { get; set; }

    private DemoTheme _pendingDemoTheme;
    private IJSObjectReference _module;

    protected override void OnInitialized() {
        base.OnInitialized();
        ThemesService.ThemeChangeRequestDispatcher = this;
        if(ThemesService.ActiveTheme == null)
            ThemesService.SetActiveThemeByName(InitialThemeName);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./switcher-resources/js/theme-controller.js");
    }

    public async void RequestThemeChange(DemoTheme demoTheme) {
        if(_pendingDemoTheme == demoTheme) return;
        _pendingDemoTheme = demoTheme;

        await DxThemesService.SetTheme(demoTheme);

        await _module.InvokeVoidAsync(
        "ThemeController.switchTheme",
        demoTheme.IsFluent, 
        demoTheme.IsBootstrapDark, 
        demoTheme.Name,
        switcher.Services.DemoThemeService.ThemeCookieKey,
        DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public async Task ThemeLoadedAsync() {
        if(ThemesService.ThemeLoadNotifier != null) {
            await ThemesService.ThemeLoadNotifier.NotifyThemeLoadedAsync(_pendingDemoTheme);
        }

        _pendingDemoTheme = null;
    }

    public async ValueTask DisposeAsync() {
        try {
            if(_module != null)
                await _module.DisposeAsync();
        } catch(JSDisconnectedException) { }
    }
}
