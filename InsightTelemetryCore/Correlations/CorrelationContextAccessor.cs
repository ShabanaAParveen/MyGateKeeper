namespace InsightTelemetryCore.Correlations;

public sealed class CorrelationContextAccessor : ICorrelationContextAccessor
{
    private static readonly AsyncLocal<CorrelationContext?> CurrentContext = new();

    public CorrelationContext? Current => CurrentContext.Value;

    public IDisposable BeginScope(CorrelationContext context)
    {
        var previous = CurrentContext.Value;
        CurrentContext.Value = context;
        return new Scope(previous);
    }

    private sealed class Scope : IDisposable
    {
        private readonly CorrelationContext? _previous;
        private bool _disposed;

        public Scope(CorrelationContext? previous)
        {
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            CurrentContext.Value = _previous;
            _disposed = true;
        }
    }
}
