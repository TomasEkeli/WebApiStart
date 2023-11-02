namespace Backend.Tests;

public abstract class TestWithBackend : IAsyncLifetime, IDisposable
{
    protected readonly Backend_with_db _backend;
    protected readonly HttpClient _api;

    protected TestWithBackend()
    {
        _backend = new Backend_with_db();
        _api = _backend.CreateClient();
    }

    public void Dispose()
    {
        _api.Dispose();
        _backend.Dispose();
        GC.SuppressFinalize(this);
    }

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}