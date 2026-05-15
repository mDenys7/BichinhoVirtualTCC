#nullable enable

namespace BichinhoVirtualTCC;

public static class OfflineProgressCalculator
{
	public static OfflineProgressResult Calculate(
		GameSaveData saveData,
		System.DateTime nowUtc,
		System.TimeSpan totalGameDuration)
	{
		var gameEndUtc = saveData.StartUtc + totalGameDuration;
		var effectiveNowUtc = nowUtc > gameEndUtc ? gameEndUtc : nowUtc;

		if (effectiveNowUtc < saveData.StartUtc)
		{
			effectiveNowUtc = saveData.StartUtc;
		}

		var elapsedToApply = System.TimeSpan.Zero;
		if (saveData.LastSeenUtc < effectiveNowUtc)
		{
			elapsedToApply = effectiveNowUtc - saveData.LastSeenUtc;
		}

		return new OfflineProgressResult
		{
			EffectiveNowUtc = effectiveNowUtc,
			ElapsedToApply = elapsedToApply
		};
	}
}

