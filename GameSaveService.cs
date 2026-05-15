#nullable enable

using System.IO;
using System.Text.Json;

namespace BichinhoVirtualTCC;

public static class GameSaveService
{
    public static void Save(string path, GameSaveData data)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(path, json);
    }

    public static GameSaveData? Load(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<GameSaveData>(json);
            if (data is null)
            {
                return null;
            }

            if (data.StartUtc == default || data.LastSeenUtc == default)
            {
                return null;
            }

            if (data.LastSeenUtc < data.StartUtc)
            {
                data.LastSeenUtc = data.StartUtc;
            }

            return data;
        }
        catch
        {
            return null;
        }
    }
}


