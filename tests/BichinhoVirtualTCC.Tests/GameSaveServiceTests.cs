using NUnit.Framework;

namespace BichinhoVirtualTCC.Tests;

[TestFixture]
public class GameSaveServiceTests
{
    private string _tempDirectory = null!;

    [SetUp]
    public void SetUp()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), "BichinhoVirtualTCC.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    [Test]
    [Category("Regression")]
    public void Save_CreatesDirectoryAndRoundTripsData()
    {
        var path = Path.Combine(_tempDirectory, "nested", "pet_save.json");
        var expected = new GameSaveData
        {
            StartUtc = new DateTime(2026, 5, 15, 12, 0, 0, DateTimeKind.Utc),
            LastSeenUtc = new DateTime(2026, 5, 15, 13, 0, 0, DateTimeKind.Utc),
            Fome = 80,
            Felicidade = 75,
            Energia = 60
        };

        GameSaveService.Save(path, expected);
        var loaded = GameSaveService.Load(path);

        Assert.Multiple(() =>
        {
            Assert.That(File.Exists(path), Is.True);
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded!.StartUtc, Is.EqualTo(expected.StartUtc));
            Assert.That(loaded.LastSeenUtc, Is.EqualTo(expected.LastSeenUtc));
            Assert.That(loaded.Fome, Is.EqualTo(expected.Fome));
            Assert.That(loaded.Felicidade, Is.EqualTo(expected.Felicidade));
            Assert.That(loaded.Energia, Is.EqualTo(expected.Energia));
        });
    }

    [Test]
    [Category("Regression")]
    public void Load_ReturnsNullWhenFileDoesNotExist()
    {
        var path = Path.Combine(_tempDirectory, "missing.json");

        var loaded = GameSaveService.Load(path);

        Assert.That(loaded, Is.Null);
    }

    [Test]
    [Category("Regression")]
    public void Load_ReturnsNullWhenJsonIsInvalid()
    {
        var path = Path.Combine(_tempDirectory, "invalid.json");
        File.WriteAllText(path, "{ not-valid-json }");

        var loaded = GameSaveService.Load(path);

        Assert.That(loaded, Is.Null);
    }

    [Test]
    public void Load_ReturnsNullWhenDatesAreMissing()
    {
        var path = Path.Combine(_tempDirectory, "missing-dates.json");
        File.WriteAllText(path, """
        {
          "StartUtc": "0001-01-01T00:00:00",
          "LastSeenUtc": "2026-05-15T13:00:00Z",
          "Fome": 80,
          "Felicidade": 75,
          "Energia": 60
        }
        """);

        var loaded = GameSaveService.Load(path);

        Assert.That(loaded, Is.Null);
    }

    [Test]
    public void Load_NormalizesLastSeenWhenItIsBeforeStart()
    {
        var path = Path.Combine(_tempDirectory, "normalize.json");
        File.WriteAllText(path, """
        {
          "StartUtc": "2026-05-15T13:00:00Z",
          "LastSeenUtc": "2026-05-15T12:00:00Z",
          "Fome": 80,
          "Felicidade": 75,
          "Energia": 60
        }
        """);

        var loaded = GameSaveService.Load(path);

        Assert.Multiple(() =>
        {
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded!.LastSeenUtc, Is.EqualTo(loaded.StartUtc));
        });
    }

    [Test]
    public void Save_OverwritesExistingFileWithNewData()
    {
        var path = Path.Combine(_tempDirectory, "pet_save.json");

        GameSaveService.Save(path, new GameSaveData
        {
            StartUtc = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc),
            LastSeenUtc = new DateTime(2026, 5, 15, 11, 0, 0, DateTimeKind.Utc),
            Fome = 10,
            Felicidade = 20,
            Energia = 30
        });

        GameSaveService.Save(path, new GameSaveData
        {
            StartUtc = new DateTime(2026, 5, 15, 14, 0, 0, DateTimeKind.Utc),
            LastSeenUtc = new DateTime(2026, 5, 15, 15, 0, 0, DateTimeKind.Utc),
            Fome = 90,
            Felicidade = 85,
            Energia = 80
        });

        var loaded = GameSaveService.Load(path);

        Assert.Multiple(() =>
        {
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded!.Fome, Is.EqualTo(90));
            Assert.That(loaded.Felicidade, Is.EqualTo(85));
            Assert.That(loaded.Energia, Is.EqualTo(80));
        });
    }
}

