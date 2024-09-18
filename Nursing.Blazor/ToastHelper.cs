using Blazored.Toast.Services;

namespace Nursing.Blazor;

public static class ToastHelper
{
    public static void ShowToast(this IToastService toastService, string message, bool success)
    {
        if (success)
        {
            toastService.ShowSuccess(message);
        }
        else
        {
            toastService.ShowError(message);
        }
    }
}
