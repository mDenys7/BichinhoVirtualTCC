using Godot;
using System;
using System.IO;

namespace BichinhoVirtualTCC;

public partial class Menu : Control
{
	private Button _newGameBtn = null!;
	private Button _loadGameBtn = null!;
	private Button _exitBtn = null!;
	private Label _statusLabel = null!;
	private const string SaveFileName = "pet_save.json";

	public override void _Ready()
	{
		_newGameBtn = GetNode<Button>("VBoxContainer/NewGameBtn");
		_loadGameBtn = GetNode<Button>("VBoxContainer/LoadGameBtn");
		_exitBtn = GetNode<Button>("VBoxContainer/ExitBtn");
		_statusLabel = GetNode<Label>("VBoxContainer/StatusLabel");

		_newGameBtn.Pressed += OnNewGamePressed;
		_loadGameBtn.Pressed += OnLoadGamePressed;
		_exitBtn.Pressed += OnExitPressed;

		UpdateLoadButtonState();
	}

	private void UpdateLoadButtonState()
	{
		var hasSave = File.Exists(ProjectSettings.GlobalizePath($"user://{SaveFileName}"));
		_loadGameBtn.Disabled = !hasSave;
		_statusLabel.Text = hasSave ? "" : "Nenhum save encontrado";
	}

	private void OnNewGamePressed()
	{
		try
		{
			GD.Print("DEBUG: Botão 'Novo Jogo' clicado!");
			var path = ProjectSettings.GlobalizePath($"user://{SaveFileName}");
			GD.Print($"DEBUG: Caminho do save: {path}");

			if (File.Exists(path))
			{
				File.Delete(path);
				GD.Print($"DEBUG: Save deletado com sucesso");
			}
			else
			{
				GD.Print($"DEBUG: Nenhum save anterior encontrado");
			}

			GD.Print("DEBUG: Tentando ir para Game.tscn...");
			var error = GetTree().ChangeSceneToFile("res://Game.tscn");
			GD.Print($"DEBUG: ChangeSceneToFile retornou: {error}");

			if (error != Error.Ok)
			{
				GD.PrintErr($"DEBUG: Erro ao mudar de cena: {error}");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"DEBUG: Exceção em OnNewGamePressed: {ex.Message}");
			GD.PrintErr($"DEBUG: StackTrace: {ex.StackTrace}");
		}
	}

	private void OnLoadGamePressed()
	{
		try
		{
			GD.Print("DEBUG: Botão 'Continuar' clicado!");
			var error = GetTree().ChangeSceneToFile("res://Game.tscn");
			GD.Print($"DEBUG: ChangeSceneToFile retornou: {error}");

			if (error != Error.Ok)
			{
				GD.PrintErr($"DEBUG: Erro ao mudar de cena: {error}");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"DEBUG: Exceção em OnLoadGamePressed: {ex.Message}");
			GD.PrintErr($"DEBUG: StackTrace: {ex.StackTrace}");
		}
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}

