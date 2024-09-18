using System.Collections;

namespace Nursing.Blazor.Models;

public class ModalResult
{
    public required object Data { get; init; }
    public bool Completed => !Cancelled;
    public bool Cancelled { get; init; }
    public static ModalResult Ok() => new() { Cancelled = false, Data = "Ok" };
    public static ModalResult Cancel() => new() { Cancelled = true, Data = "Cancelled" };
    public static ModalResult Ok(object data) => new() { Cancelled = false, Data = data };
    public static ModalResult Cancel(object data) => new() { Cancelled = true, Data = data };
}

public class ModalParameters : IEnumerable<KeyValuePair<string, object>>
{
    private readonly Dictionary<string, object> _parameters = new();
    public void Add(string key, object value)
    {
        _parameters.Add(key, value);
    }
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        => _parameters.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _parameters.GetEnumerator();
}
