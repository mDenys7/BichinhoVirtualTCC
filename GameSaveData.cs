#nullable enable

namespace BichinhoVirtualTCC;

public sealed class GameSaveData
{
    public System.DateTime StartUtc { get; set; }
    public System.DateTime LastSeenUtc { get; set; }
    public int Fome { get; set; }
    public int Felicidade { get; set; }
    public int Energia { get; set; }
}


