#nullable enable

namespace BichinhoVirtualTCC;

public sealed class OfflineProgressResult
{
    public System.DateTime EffectiveNowUtc { get; init; }
    public System.TimeSpan ElapsedToApply { get; init; }
}

