namespace BichinhoVirtualTCC;

public class PetStatus
{
    private const double HungerDecayPerTick = 100.0 / 43200.0;
    private const double HappinessDecayPerTick = 70.0 / 43200.0;
    private const double EnergyDecayPerTick = 55.0 / 43200.0;
    private const double LowHungerPenaltyPerTick = 20.0 / 43200.0;
    private const double LowEnergyPenaltyPerTick = 15.0 / 43200.0;

    private double _fomeValue = 100;
    private double _felicidadeValue = 100;
    private double _energiaValue = 100;

    public int Fome => ToDisplay(_fomeValue);
    public int Felicidade => ToDisplay(_felicidadeValue);
    public int Energia => ToDisplay(_energiaValue);

    public bool IsAlive => _fomeValue > 0 && _felicidadeValue > 0 && _energiaValue > 0;

    public void Tick()
    {
        if (!IsAlive)
        {
            return;
        }

        _fomeValue -= HungerDecayPerTick;
        _felicidadeValue -= HappinessDecayPerTick;
        _energiaValue -= EnergyDecayPerTick;

        if (_fomeValue < 30)
        {
            _felicidadeValue -= LowHungerPenaltyPerTick;
        }

        if (_energiaValue < 20)
        {
            _felicidadeValue -= LowEnergyPenaltyPerTick;
        }

        Limitar();
    }

    public void ApplyElapsed(System.TimeSpan elapsed, double tickSeconds)
    {
        if (!IsAlive || tickSeconds <= 0)
        {
            return;
        }

        var ticks = (int)(elapsed.TotalSeconds / tickSeconds);
        for (var i = 0; i < ticks && IsAlive; i++)
        {
            Tick();
        }
    }

    public void LoadStatus(int fome, int felicidade, int energia)
    {
        _fomeValue = fome;
        _felicidadeValue = felicidade;
        _energiaValue = energia;
        Limitar();
    }

    public void ResetStatus()
    {
        _fomeValue = 100;
        _felicidadeValue = 100;
        _energiaValue = 100;
    }

    public void Alimentar()
    {
        if (!IsAlive)
        {
            return;
        }

        _fomeValue += 20;
        _energiaValue -= 4;
        Limitar();
    }

    public void Brincar()
    {
        if (!IsAlive)
        {
            return;
        }

        _felicidadeValue += 20;
        _energiaValue -= 10;
        _fomeValue -= 6;
        Limitar();
    }

    public void Dormir()
    {
        if (!IsAlive)
        {
            return;
        }

        _energiaValue += 25;
        _fomeValue -= 10;
        _felicidadeValue += 3;
        Limitar();
    }

    public void Limitar()
    {
        _fomeValue = Clamp(_fomeValue);
        _felicidadeValue = Clamp(_felicidadeValue);
        _energiaValue = Clamp(_energiaValue);
    }

    private static double Clamp(double value)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > 100)
        {
            return 100;
        }

        return value;
    }

    private static int ToDisplay(double value)
    {
        return (int)System.Math.Round(value, System.MidpointRounding.AwayFromZero);
    }
}
