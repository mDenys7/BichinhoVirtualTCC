using Godot;

namespace BichinhoVirtualTCC;

public partial class Main : Control
{
	private readonly PetStatus _pet = new();

	private Label _fomeLabel = null!;
	private Label _felicidadeLabel = null!;
	private Label _energiaLabel = null!;
	private Label _estadoLabel = null!;

	private Button _alimentarBtn = null!;
	private Button _brincarBtn = null!;
	private Button _dormirBtn = null!;

	public override void _Ready()
	{
		_fomeLabel = GetNode<Label>("MarginContainer/VBoxContainer/FomeLabel");
		_felicidadeLabel = GetNode<Label>("MarginContainer/VBoxContainer/FelicidadeLabel");
		_energiaLabel = GetNode<Label>("MarginContainer/VBoxContainer/EnergiaLabel");
		_estadoLabel = GetNode<Label>("MarginContainer/VBoxContainer/EstadoLabel");

		_alimentarBtn = GetNode<Button>("MarginContainer/VBoxContainer/AlimentarBtn");
		_brincarBtn = GetNode<Button>("MarginContainer/VBoxContainer/BrincarBtn");
		_dormirBtn = GetNode<Button>("MarginContainer/VBoxContainer/DormirBtn");

		_alimentarBtn.Pressed += OnAlimentarBtnPressed;
		_brincarBtn.Pressed += OnBrincarBtnPressed;
		_dormirBtn.Pressed += OnDormirBtnPressed;

		var timer = new Timer
		{
			WaitTime = 2.0,
			OneShot = false,
			Autostart = true
		};

		AddChild(timer);
		timer.Timeout += OnStatusTick;

		AtualizarUi();
	}

	private void OnStatusTick()
	{
		_pet.Tick();
		AtualizarUi();
	}

	private void OnAlimentarBtnPressed()
	{
		_pet.Alimentar();
		AtualizarUi();
	}

	private void OnBrincarBtnPressed()
	{
		_pet.Brincar();
		AtualizarUi();
	}

	private void OnDormirBtnPressed()
	{
		_pet.Dormir();
		AtualizarUi();
	}

	private void AtualizarUi()
	{
		_fomeLabel.Text = $"Fome: {_pet.Fome}";
		_felicidadeLabel.Text = $"Felicidade: {_pet.Felicidade}";
		_energiaLabel.Text = $"Energia: {_pet.Energia}";
		_estadoLabel.Text = _pet.IsAlive ? "Estado: Feliz :)" : "Estado: O pet desmaiou!";

		_alimentarBtn.Disabled = !_pet.IsAlive;
		_brincarBtn.Disabled = !_pet.IsAlive;
		_dormirBtn.Disabled = !_pet.IsAlive;
	}
}
