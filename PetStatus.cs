namespace BichinhoVirtualTCC;

public class PetStatus
{
    public int Fome { get; private set; } = 100;
    public int Felicidade { get; private set; } = 100;
    public int Energia { get; private set; } = 100;

    public bool IsAlive => Fome > 0 && Felicidade > 0 && Energia > 0;

    public void Tick()
    {
        if (!IsAlive)
        {
            return;
        }

        Fome -= 4;
        Felicidade -= 3;
        Energia -= 2;

        if (Fome < 30)
        {
            Felicidade -= 2;
        }

        if (Energia < 20)
        {
            Felicidade -= 2;
        }

        Limitar();
    }

    public void Alimentar()
    {
        if (!IsAlive)
        {
            return;
        }

        Fome += 20;
        Energia -= 4;
        Limitar();
    }

    public void Brincar()
    {
        if (!IsAlive)
        {
            return;
        }

        Felicidade += 20;
        Energia -= 10;
        Fome -= 6;
        Limitar();
    }

    public void Dormir()
    {
        if (!IsAlive)
        {
            return;
        }

        Energia += 25;
        Fome -= 10;
        Felicidade += 3;
        Limitar();
    }

    public void Limitar()
    {
        Fome = Clamp(Fome);
        Felicidade = Clamp(Felicidade);
        Energia = Clamp(Energia);
    }

    private static int Clamp(int value)
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
}


