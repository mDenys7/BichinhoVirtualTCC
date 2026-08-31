#nullable enable

using Godot;
using System;
using System.Collections.Generic;

namespace BichinhoVirtualTCC;

public partial class Main : Control
{
	private const double TickSeconds = 1.0;
	private static readonly TimeSpan TotalGameDuration = TimeSpan.FromDays(3);
	private const string SaveFileName = "pet_save.json";
	private static readonly string[] PetSpritePaths =
	{
		"res://sprites/pet_day_1.png",
		"res://sprites/pet_day_2.png",
		"res://sprites/pet_day_3_neutral.png"
	};

	private static readonly Dictionary<PetStatus.FinalState, string> FinalDaySprites = new()
	{
		{ PetStatus.FinalState.Good, "res://sprites/pet_day_3_good.png" },
		{ PetStatus.FinalState.Neutral, "res://sprites/pet_day_3_neutral.png" },
		{ PetStatus.FinalState.Evil, "res://sprites/pet_day_3_evil.png" }
	};

	private static readonly string[] FaintedSprites =
	{
		"res://sprites/pet_day_1_fainted.png",
		"res://sprites/pet_day_2_fainted.png"
	};

	private readonly PetStatus _pet = new();

	private Label _fomeLabel = null!;
	private Label _felicidadeLabel = null!;
	private Label _energiaLabel = null!;
	private Label _estadoLabel = null!;
	private Label _diaLabel = null!;
	private TextureRect _petSprite = null!;
	private Vector2 _petBasePosition;
	private Tween? _idleTween;
	private AudioStreamPlayer _bgmGame = null!;
	private AudioStreamPlayer _bgmDeath = null!;
	private bool _deathMusicPlayed;

	private Button _alimentarBtn = null!;
	private Button _brincarBtn = null!;
	private Button _dormirBtn = null!;
	private Button _saveGameBtn = null!;
	private Button? _voltarMenuBtn;

	private Timer _timer = null!;
	private DateTime _startUtc;
	private DateTime _lastSeenUtc;
	private Texture2D[] _petDayTextures = Array.Empty<Texture2D>();
	private Texture2D? _fallbackTexture;

	public override void _Ready()
	{
		_fomeLabel = GetNode<Label>("MarginContainer/VBoxContainer/FomeLabel");
		_felicidadeLabel = GetNode<Label>("MarginContainer/VBoxContainer/FelicidadeLabel");
		_energiaLabel = GetNode<Label>("MarginContainer/VBoxContainer/EnergiaLabel");
		_estadoLabel = GetNode<Label>("MarginContainer/VBoxContainer/EstadoLabel");
		_diaLabel = GetNode<Label>("MarginContainer/VBoxContainer/DiaLabel");
		_petSprite = GetNode<TextureRect>("MarginContainer/VBoxContainer/PetCenter/PetHolder/PetSprite");
		_bgmGame = GetNode<AudioStreamPlayer>("MarginContainer/BgmGame");
		_bgmDeath = GetNode<AudioStreamPlayer>("MarginContainer/BgmDeath");

		StartIdleAnimation();

		_alimentarBtn = GetNode<Button>("MarginContainer/VBoxContainer/AlimentarBtn");
		_brincarBtn = GetNode<Button>("MarginContainer/VBoxContainer/BrincarBtn");
		_dormirBtn = GetNode<Button>("MarginContainer/VBoxContainer/DormirBtn");
		_saveGameBtn = GetNode<Button>("MarginContainer/VBoxContainer/SaveGameBtn");
		_voltarMenuBtn = GetNodeOrNull<Button>("MarginContainer/VBoxContainer/VoltarMenuBtn");

		_alimentarBtn.Pressed += OnAlimentarBtnPressed;
		_brincarBtn.Pressed += OnBrincarBtnPressed;
		_dormirBtn.Pressed += OnDormirBtnPressed;
		_saveGameBtn.Pressed += OnSaveGameBtnPressed;
		if (_voltarMenuBtn != null)
		{
			_voltarMenuBtn.Pressed += OnVoltarMenuBtnPressed;
		}

		LoadPetSprites();
		LoadOrCreateGame();

		_timer = new Timer
		{
			WaitTime = TickSeconds,
			OneShot = false,
			Autostart = true
		};

		AddChild(_timer);
		_timer.Timeout += OnStatusTick;

		AtualizarUi();
		SaveGame();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest || what == NotificationPredelete)
		{
			SaveGame();
		}
	}

	private void LoadOrCreateGame()
	{
		var nowUtc = DateTime.UtcNow;
		var saveData = LoadGame();

		if (saveData is null)
		{
			_startUtc = nowUtc;
			_lastSeenUtc = nowUtc;
			return;
		}

		_startUtc = saveData.StartUtc;
		_lastSeenUtc = saveData.LastSeenUtc;
		_pet.LoadStatus(saveData.Fome, saveData.Felicidade, saveData.Energia);

		var offlineProgress = OfflineProgressCalculator.Calculate(saveData, nowUtc, TotalGameDuration);

		if (offlineProgress.ElapsedToApply > TimeSpan.Zero)
		{
			_pet.ApplyElapsed(offlineProgress.ElapsedToApply, TickSeconds);
		}

		_lastSeenUtc = offlineProgress.EffectiveNowUtc;
	}

	private void OnStatusTick()
	{
		if (!CanContinueGame())
		{
			AtualizarUi();
			SaveGame();
			return;
		}

		_pet.Tick();
		_lastSeenUtc = DateTime.UtcNow;
		AtualizarUi();
		SaveGame();
	}

	private void OnAlimentarBtnPressed()
	{
		if (!CanContinueGame())
		{
			AtualizarUi();
			return;
		}

		_pet.Alimentar();
		_lastSeenUtc = DateTime.UtcNow;
		AtualizarUi();
		SaveGame();
	}

	private void OnBrincarBtnPressed()
	{
		if (!CanContinueGame())
		{
			AtualizarUi();
			return;
		}

		_pet.Brincar();
		_lastSeenUtc = DateTime.UtcNow;
		AtualizarUi();
		SaveGame();
	}

	private void OnDormirBtnPressed()
	{
		if (!CanContinueGame())
		{
			AtualizarUi();
			return;
		}

		_pet.Dormir();
		_lastSeenUtc = DateTime.UtcNow;
		AtualizarUi();
		SaveGame();
	}

	private void OnSaveGameBtnPressed()
	{
		SaveGame();
		_estadoLabel.Text += "\nJogo salvo!";
	}

	private void OnVoltarMenuBtnPressed()
	{
		SaveGame();
		GetTree().ChangeSceneToFile("res://Menu.tscn");
	}

	private bool CanContinueGame()
	{
		return _pet.IsAlive && DateTime.UtcNow < _startUtc + TotalGameDuration;
	}

	private void AtualizarUi()
	{
		_fomeLabel.Text = $"Fome: {_pet.Fome}";
		_felicidadeLabel.Text = $"Felicidade: {_pet.Felicidade}";
		_energiaLabel.Text = $"Energia: {_pet.Energia}";

		var currentDay = GetCurrentDayIndex() + 1;
		_diaLabel.Text = $"Dia: {currentDay}/3";

		UpdatePetSprite();

		var remaining = (_startUtc + TotalGameDuration) - DateTime.UtcNow;
		if (remaining < TimeSpan.Zero)
		{
			remaining = TimeSpan.Zero;
		}

		var diasRestantes = Math.Floor(remaining.TotalDays);
		var horas = remaining.Hours;
		var minutos = remaining.Minutes;
		var tempoRestante = $"Tempo restante: {diasRestantes:0}d {horas:00}h {minutos:00}m";

		if (DateTime.UtcNow >= _startUtc + TotalGameDuration)
		{
			_estadoLabel.Text = $"Estado: Ciclo de 3 dias finalizado!\n{tempoRestante}";
		}
		else
		{
			_estadoLabel.Text = _pet.IsAlive
				? $"Estado: {GetEstadoTexto()}\n{tempoRestante}"
				: $"Estado: O pet desmaiou!\n{tempoRestante}";
		}

		var disableActions = !CanContinueGame();
		_alimentarBtn.Disabled = disableActions;
		_brincarBtn.Disabled = disableActions;
		_dormirBtn.Disabled = disableActions;
	}

	private void SaveGame()
	{
		var nowUtc = DateTime.UtcNow;
		if (nowUtc < _lastSeenUtc)
		{
			nowUtc = _lastSeenUtc;
		}

		_lastSeenUtc = nowUtc;

		var data = new GameSaveData
		{
			StartUtc = _startUtc,
			LastSeenUtc = _lastSeenUtc,
			Fome = _pet.Fome,
			Felicidade = _pet.Felicidade,
			Energia = _pet.Energia
		};

		GameSaveService.Save(GetSavePath(), data);
	}

	private static GameSaveData? LoadGame()
	{
		return GameSaveService.Load(GetSavePath());
	}

	private static string GetSavePath()
	{
		return ProjectSettings.GlobalizePath($"user://{SaveFileName}");
	}

	private void LoadPetSprites()
	{
		_petDayTextures = new Texture2D[PetSpritePaths.Length];
		for (var i = 0; i < PetSpritePaths.Length; i++)
		{
			_petDayTextures[i] = ResourceLoader.Load<Texture2D>(PetSpritePaths[i]);
			if (_petDayTextures[i] == null)
			{
				GD.PushWarning($"Falha ao carregar sprite: {PetSpritePaths[i]}");
			}
		}

		_fallbackTexture = null;
		for (var i = 0; i < _petDayTextures.Length; i++)
		{
			if (_petDayTextures[i] != null)
			{
				_fallbackTexture = _petDayTextures[i];
				break;
			}
		}
	}

	private void UpdatePetSprite()
	{
		if (_petDayTextures.Length == 0)
		{
			return;
		}

		var dayIndex = GetCurrentDayIndex();

		if (!_pet.IsAlive)
		{
			_idleTween?.Kill();
			_petSprite.Position = _petBasePosition;
			if (!_deathMusicPlayed)
			{
				_bgmGame.Stop();
				_bgmDeath.Play();
				_deathMusicPlayed = true;
			}
		}
		
		// Se o pet está desmaiado, mostra o sprite de desmaiado
		if (!_pet.IsAlive)
		{
			if (dayIndex < FaintedSprites.Length)
			{
				var faintedTexture = ResourceLoader.Load<Texture2D>(FaintedSprites[dayIndex]);
				if (faintedTexture != null)
				{
					_petSprite.Texture = faintedTexture;
					return;
				}
			}
		}
		
		// Se é o último dia, usa o sprite baseado no estado final
		if (dayIndex >= 2) // Dia 3 (índice 2)
		{
			var finalState = _pet.GetFinalState();
			var finalSpritePath = FinalDaySprites[finalState];
			var finalTexture = ResourceLoader.Load<Texture2D>(finalSpritePath);
			if (finalTexture != null)
			{
				_petSprite.Texture = finalTexture;
				return;
			}
		}

		for (var i = dayIndex; i >= 0; i--)
		{
			if (_petDayTextures[i] != null)
			{
				_petSprite.Texture = _petDayTextures[i];
				return;
			}
		}

		if (_fallbackTexture != null)
		{
			_petSprite.Texture = _fallbackTexture;
			return;
		}

		GD.PushWarning("Nenhum sprite de evolucao foi carregado em res://sprites.");
	}

	private string GetEstadoTexto()
	{
		if (_pet.Fome <= 30)
		{
			return "Com fome";
		}

		if (_pet.Energia <= 20)
		{
			return "Cansado";
		}

		if (_pet.Felicidade <= 40)
		{
			return "Triste";
		}

		return "Feliz :)";
	}

	private int GetCurrentDayIndex()
	{
		var elapsed = DateTime.UtcNow - _startUtc;
		if (elapsed < TimeSpan.Zero)
		{
			elapsed = TimeSpan.Zero;
		}

		var dayIndex = (int)Math.Floor(elapsed.TotalDays);
		if (dayIndex < 0)
		{
			return 0;
		}

		if (dayIndex >= PetSpritePaths.Length)
		{
			return PetSpritePaths.Length - 1;
		}

		return dayIndex;
	}

	private void StartIdleAnimation()
	{
		_idleTween?.Kill();
		_petBasePosition = _petSprite.Position;

		var tween = CreateTween();
		_idleTween = tween;
		tween.SetLoops();

		var step = 0.25f;
		var ease = Tween.EaseType.InOut;
		var trans = Tween.TransitionType.Sine;
		var dx = 6f;
		var dy = 6f;
		var basePos = _petBasePosition;

		var p1 = basePos + new Vector2(dx, 0);
		var p2 = basePos + new Vector2(dx * 2, 0);
		var p3 = basePos + new Vector2(dx * 3, 0);
		var p4 = basePos + new Vector2(dx * 2, 0);
		var p5 = basePos + new Vector2(dx, 0);
		var p6 = basePos + new Vector2(dx, -dy);
		var p7 = basePos + new Vector2(-dx, 0);
		var p8 = basePos + new Vector2(-dx * 2, 0);
		var p9 = basePos + new Vector2(-dx * 3, 0);
		var p10 = basePos + new Vector2(-dx * 2, 0);
		var p11 = basePos + new Vector2(-dx, 0);
		var p12 = basePos + new Vector2(-dx, -dy);

		tween.TweenProperty(_petSprite, "position", p1, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p2, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p3, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p4, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p5, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p6, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p7, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p8, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p9, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p10, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p11, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", p12, step)
			.SetEase(ease)
			.SetTrans(trans);
		tween.TweenProperty(_petSprite, "position", basePos, step)
			.SetEase(ease)
			.SetTrans(trans);
	}

}
