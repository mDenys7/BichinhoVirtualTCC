using BichinhoVirtualTCC;
using NUnit.Framework;

namespace BichinhoVirtualTCC.Tests;

[TestFixture]
public class PetStatusTests
{
    [Test]
    public void NewPet_StartsWithFullStatusAndAlive()
    {
        var pet = new PetStatus();

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(100));
            Assert.That(pet.Felicidade, Is.EqualTo(100));
            Assert.That(pet.Energia, Is.EqualTo(100));
            Assert.That(pet.IsAlive, Is.True);
        });
    }

    [Test]
    public void LoadStatus_ClampsValuesIntoValidRange()
    {
        var pet = new PetStatus();

        pet.LoadStatus(-10, 120, 50);

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(0));
            Assert.That(pet.Felicidade, Is.EqualTo(100));
            Assert.That(pet.Energia, Is.EqualTo(50));
            Assert.That(pet.IsAlive, Is.False);
        });
    }

    [Test]
    public void ResetStatus_RestoresDefaultValues()
    {
        var pet = new PetStatus();
        pet.LoadStatus(12, 34, 56);

        pet.ResetStatus();

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(100));
            Assert.That(pet.Felicidade, Is.EqualTo(100));
            Assert.That(pet.Energia, Is.EqualTo(100));
            Assert.That(pet.IsAlive, Is.True);
        });
    }

    [Test]
    public void Alimentar_IncreasesHungerAndCostsEnergy()
    {
        var pet = new PetStatus();
        pet.LoadStatus(80, 50, 40);

        pet.Alimentar();

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(100));
            Assert.That(pet.Felicidade, Is.EqualTo(50));
            Assert.That(pet.Energia, Is.EqualTo(36));
        });
    }

    [Test]
    public void Brincar_IncreasesHappinessAndConsumesHungerAndEnergy()
    {
        var pet = new PetStatus();
        pet.LoadStatus(80, 50, 40);

        pet.Brincar();

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(74));
            Assert.That(pet.Felicidade, Is.EqualTo(70));
            Assert.That(pet.Energia, Is.EqualTo(30));
        });
    }

    [Test]
    public void Dormir_RecoversEnergyWithoutExceedingMaximum()
    {
        var pet = new PetStatus();
        pet.LoadStatus(80, 50, 90);

        pet.Dormir();

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(70));
            Assert.That(pet.Felicidade, Is.EqualTo(53));
            Assert.That(pet.Energia, Is.EqualTo(100));
        });
    }

    [Test]
    public void Actions_DoNothingWhenPetIsNotAlive()
    {
        var pet = new PetStatus();
        pet.LoadStatus(0, 50, 50);

        pet.Alimentar();
        pet.Brincar();
        pet.Dormir();

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(0));
            Assert.That(pet.Felicidade, Is.EqualTo(50));
            Assert.That(pet.Energia, Is.EqualTo(50));
            Assert.That(pet.IsAlive, Is.False);
        });
    }

    [Test]
    public void ActionSequence_StopsChangingStateAfterPetDies()
    {
        var pet = new PetStatus();
        pet.LoadStatus(5, 5, 5);

        pet.Brincar();
        pet.Dormir();

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(0));
            Assert.That(pet.Felicidade, Is.EqualTo(25));
            Assert.That(pet.Energia, Is.EqualTo(0));
            Assert.That(pet.IsAlive, Is.False);
        });
    }

    [Test]
    public void ApplyElapsed_AppliesExpectedDecayForElapsedTime()
    {
        var pet = new PetStatus();

        pet.ApplyElapsed(TimeSpan.FromHours(1), 2.0);

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(96));
            Assert.That(pet.Felicidade, Is.EqualTo(97));
            Assert.That(pet.Energia, Is.EqualTo(98));
            Assert.That(pet.IsAlive, Is.True);
        });
    }

    [Test]
    public void ApplyElapsed_IgnoresIncompleteTick()
    {
        var pet = new PetStatus();

        pet.ApplyElapsed(TimeSpan.FromSeconds(1), 2.0);

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(100));
            Assert.That(pet.Felicidade, Is.EqualTo(100));
            Assert.That(pet.Energia, Is.EqualTo(100));
        });
    }

    [Test]
    public void ApplyElapsed_AddsExtraPenaltyWhenHungerAndEnergyAreLow()
    {
        var pet = new PetStatus();
        pet.LoadStatus(29, 50, 19);

        pet.ApplyElapsed(TimeSpan.FromHours(2), 2.0);

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(21));
            Assert.That(pet.Felicidade, Is.EqualTo(41));
            Assert.That(pet.Energia, Is.EqualTo(14));
            Assert.That(pet.IsAlive, Is.True);
        });
    }

    [Test]
    public void ApplyElapsed_StopsProcessingAfterPetDies()
    {
        var pet = new PetStatus();
        pet.LoadStatus(1, 100, 100);

        pet.ApplyElapsed(TimeSpan.FromDays(1), 2.0);

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(0));
            Assert.That(pet.Felicidade, Is.EqualTo(99));
            Assert.That(pet.Energia, Is.EqualTo(99));
            Assert.That(pet.IsAlive, Is.False);
        });
    }

    [Test]
    public void ApplyElapsed_IgnoresZeroOrNegativeTickSeconds()
    {
        var pet = new PetStatus();
        pet.LoadStatus(80, 60, 40);

        pet.ApplyElapsed(TimeSpan.FromHours(1), 0);
        pet.ApplyElapsed(TimeSpan.FromHours(1), -1);

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(80));
            Assert.That(pet.Felicidade, Is.EqualTo(60));
            Assert.That(pet.Energia, Is.EqualTo(40));
        });
    }

    [Test]
    public void Tick_DoesNothingWhenPetIsAlreadyDead()
    {
        var pet = new PetStatus();
        pet.LoadStatus(0, 100, 100);

        pet.Tick();

        Assert.Multiple(() =>
        {
            Assert.That(pet.Fome, Is.EqualTo(0));
            Assert.That(pet.Felicidade, Is.EqualTo(100));
            Assert.That(pet.Energia, Is.EqualTo(100));
        });
    }

    [Test]
    public void GetFinalState_ReturnsGoodForHighScore()
    {
        var pet = new PetStatus();
        pet.LoadStatus(20, 100, 100);

        Assert.That(pet.GetFinalState(), Is.EqualTo(PetStatus.FinalState.Good));
    }

    [Test]
    public void GetFinalState_ReturnsNeutralForMidScore()
    {
        var pet = new PetStatus();
        pet.LoadStatus(50, 60, 50);

        Assert.That(pet.GetFinalState(), Is.EqualTo(PetStatus.FinalState.Neutral));
    }

    [Test]
    public void GetFinalState_ReturnsEvilForLowScore()
    {
        var pet = new PetStatus();
        pet.LoadStatus(100, 20, 20);

        Assert.That(pet.GetFinalState(), Is.EqualTo(PetStatus.FinalState.Evil));
    }

    [Test]
    public void GetFinalState_TreatsScoreOf60AsGood()
    {
        var pet = new PetStatus();
        pet.LoadStatus(25, 100, 50);

        Assert.That(pet.GetFinalState(), Is.EqualTo(PetStatus.FinalState.Good));
    }

    [Test]
    public void GetFinalState_TreatsScoreOf30AsEvil()
    {
        var pet = new PetStatus();
        pet.LoadStatus(100, 40, 100);

        Assert.That(pet.GetFinalState(), Is.EqualTo(PetStatus.FinalState.Evil));
    }
}

