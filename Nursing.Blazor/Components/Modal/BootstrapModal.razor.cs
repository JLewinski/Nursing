using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Nursing.Blazor.Models;

namespace Nursing.Blazor.Components.Modal;

public partial class BootstrapModal : IDisposable
{
    private bool _setFocus;
    [CascadingParameter] private ModalManager Parent { get; set; } = default!;

    [Parameter] public Guid Id { get; set; }

    [Parameter, EditorRequired] public RenderFragment Content { get; set; } = default!;

    [Parameter] public required string Title { get; set; }
    [Parameter] public bool HideCloseButton { get; set; } = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JsRuntime.InvokeVoidAsync("showModal", "modal-" + Id);
    }

    internal async Task Close()
    {
        await Close(ModalResult.Ok());
    }

    internal async Task Close(ModalResult result)
    {
        await JsRuntime.InvokeVoidAsync("closeModal", "modal-" + Id);
        await Parent.DismissInstance(Id, result);
    }
    void IDisposable.Dispose()
        => Parent.OnModalClosed -= AttemptFocus;

    private void AttemptFocus()
        => _setFocus = true;
}
