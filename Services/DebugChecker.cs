namespace OurGuardian.Services;

public class DebugChecker
{
    private readonly bool _isDebug;

    public DebugChecker()
    {
#if DEBUG
        _isDebug = true;
#else
        _isDebug = false;
#endif
    }

    public bool IsDebug => _isDebug;
}