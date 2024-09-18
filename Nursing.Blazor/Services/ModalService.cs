using Microsoft.AspNetCore.Components;
using Nursing.Blazor.Components.Modal;
using Nursing.Blazor.Models;

namespace Nursing.Blazor.Services;

public class BootstrapModalService
{
    internal event Func<BootstrapModalReference, Task>? OnModalInstanceAdded;
    internal event Func<BootstrapModalReference, ModalResult, Task>? OnModalCloseRequested;

    public BootstrapModalReference Show<T>(string title, ModalParameters? parameters = null) where T : IComponent
    {
        BootstrapModalReference? modalReference = null;
        var modalInstanceId = Guid.NewGuid();
        var modalContent = new RenderFragment(builder =>
        {
            var i = 0;
            builder.OpenComponent<T>(i++);
            if (parameters != null)
            {
                foreach (var keyVal in parameters)
                {
                    builder.AddAttribute(i++, keyVal.Key, keyVal.Value);
                }
            }
            builder.CloseComponent();
        });
        var modalInstance = new RenderFragment(builder =>
        {
            builder.OpenComponent<BootstrapModal>(0);
            builder.SetKey("bootstrapBlazoredModalInstance_" + modalInstanceId);
            builder.AddAttribute(1, "Title", title);
            builder.AddAttribute(2, "Content", modalContent);
            builder.AddAttribute(3, "Id", modalInstanceId);
            builder.AddComponentReferenceCapture(4, compRef => modalReference!.ModalInstanceRef = (BootstrapModal)compRef);
            builder.CloseComponent();
        });
        modalReference = new BootstrapModalReference(modalInstanceId, modalInstance, this);

        OnModalInstanceAdded?.Invoke(modalReference);

        return modalReference;
    }

    internal void Close(BootstrapModalReference modal)
        => Close(modal, ModalResult.Ok());

    internal void Close(BootstrapModalReference modal, ModalResult result)
        => OnModalCloseRequested?.Invoke(modal, result);
}

public class BootstrapModalReference(Guid id, RenderFragment modalInstance, BootstrapModalService modalService)
{
    private readonly TaskCompletionSource<ModalResult> _resultCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);


    internal Guid Id { get; } = id;
    internal RenderFragment ModalInstance { get; } = modalInstance;
    internal BootstrapModalService ModalService { get; } = modalService;

    public Task<ModalResult> Result => _resultCompletion.Task;
    internal BootstrapModal ModalInstanceRef { get; set; } = null!;

    public void Close()
    {
        ModalService.Close(this);
    }

    public void Close(ModalResult result)
    {
        ModalService.Close(this, result);
    }

    internal void Dismiss(ModalResult result)
    {
        _ = _resultCompletion.TrySetResult(result);
    }
}