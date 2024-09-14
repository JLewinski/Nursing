using Microsoft.JSInterop;

namespace Nursing.Mobile.Services;

public static class JavaScriptHelper
{
    public static async Task ToggleTheme(this IJSRuntime JSRuntime, bool isDarkTheme)
    {
        await JSRuntime.InvokeVoidAsync("themeManager.toggleTheme", isDarkTheme);
    }

    public static async Task<bool> CheckIsDarkPrefered(this IJSRuntime JSRuntime)
    {
        return await JSRuntime.InvokeAsync<bool>("themeManager.isDarkPreferred");
    }

    public static async Task<T> ShowLoadingModal<T>(this IJSRuntime JSRuntime, Func<Task<T>> awaitableTask, string message = "Loading")
    {
        await JSRuntime.ShowLoadingModal(message);
        var result = await awaitableTask();
        await JSRuntime.HideLoadingModal();
        return result;
    }

    public static async Task ShowLoadingModal(this IJSRuntime JSRuntime, string message = "Loading")
    {
        await JSRuntime.InvokeVoidAsync("loadingModal.showLoadingModal", message);
    }

    public static async Task HideLoadingModal(this IJSRuntime JSRuntime)
    {
        await JSRuntime.InvokeVoidAsync("loadingModal.hideLoadingModal");
    }
}
