using NUnit.Framework;

namespace BichinhoVirtualTCC.Tests;

[TestFixture]
public class OfflineProgressCalculatorTests
{
    private static readonly TimeSpan TotalGameDuration = TimeSpan.FromDays(3);

    [Test]
    [Category("Regression")]
    public void Calculate_ReturnsElapsedSinceLastSeenWhenWithinCycle()
    {
        var saveData = new GameSaveData
        {
            StartUtc = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc),
            LastSeenUtc = new DateTime(2026, 5, 15, 12, 0, 0, DateTimeKind.Utc)
        };
        var nowUtc = new DateTime(2026, 5, 15, 15, 30, 0, DateTimeKind.Utc);

        var result = OfflineProgressCalculator.Calculate(saveData, nowUtc, TotalGameDuration);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveNowUtc, Is.EqualTo(nowUtc));
            Assert.That(result.ElapsedToApply, Is.EqualTo(TimeSpan.FromHours(3.5)));
        });
    }

    [Test]
    [Category("Regression")]
    public void Calculate_CapsProgressAtEndOfCycle()
    {
        var saveData = new GameSaveData
        {
            StartUtc = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc),
            LastSeenUtc = new DateTime(2026, 5, 17, 10, 0, 0, DateTimeKind.Utc)
        };
        var nowUtc = new DateTime(2026, 5, 20, 10, 0, 0, DateTimeKind.Utc);

        var result = OfflineProgressCalculator.Calculate(saveData, nowUtc, TotalGameDuration);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveNowUtc, Is.EqualTo(saveData.StartUtc + TotalGameDuration));
            Assert.That(result.ElapsedToApply, Is.EqualTo(TimeSpan.FromDays(1)));
        });
    }

    [Test]
    public void Calculate_ReturnsZeroWhenNoTimeHasPassed()
    {
        var saveData = new GameSaveData
        {
            StartUtc = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc),
            LastSeenUtc = new DateTime(2026, 5, 15, 12, 0, 0, DateTimeKind.Utc)
        };
        var nowUtc = saveData.LastSeenUtc;

        var result = OfflineProgressCalculator.Calculate(saveData, nowUtc, TotalGameDuration);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveNowUtc, Is.EqualTo(nowUtc));
            Assert.That(result.ElapsedToApply, Is.EqualTo(TimeSpan.Zero));
        });
    }

    [Test]
    public void Calculate_ReturnsZeroWhenLastSeenIsAlreadyAtEffectiveNow()
    {
        var saveData = new GameSaveData
        {
            StartUtc = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc),
            LastSeenUtc = new DateTime(2026, 5, 18, 10, 0, 0, DateTimeKind.Utc)
        };
        var nowUtc = new DateTime(2026, 5, 19, 10, 0, 0, DateTimeKind.Utc);

        var result = OfflineProgressCalculator.Calculate(saveData, nowUtc, TotalGameDuration);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveNowUtc, Is.EqualTo(saveData.StartUtc + TotalGameDuration));
            Assert.That(result.ElapsedToApply, Is.EqualTo(TimeSpan.Zero));
        });
    }

    [Test]
    public void Calculate_UsesStartTimeWhenNowIsBeforeCycleStart()
    {
        var saveData = new GameSaveData
        {
            StartUtc = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc),
            LastSeenUtc = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc)
        };
        var nowUtc = new DateTime(2026, 5, 15, 9, 0, 0, DateTimeKind.Utc);

        var result = OfflineProgressCalculator.Calculate(saveData, nowUtc, TotalGameDuration);

        Assert.Multiple(() =>
        {
            Assert.That(result.EffectiveNowUtc, Is.EqualTo(saveData.StartUtc));
            Assert.That(result.ElapsedToApply, Is.EqualTo(TimeSpan.Zero));
        });
    }
}

