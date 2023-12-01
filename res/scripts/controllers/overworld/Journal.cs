namespace Overworld;

using Godot;
using System;
using System.Threading.Tasks;


public partial class Journal : Node2D
{
	[Export] private string _inventoryUID;
	[Export] private string _settingsUID;
	[Export] private string _mainMenuUID;
	private ChangeSceneButton _returnButton;
	private JournalLine _openDeck;
	private JournalLine _openSettings;
	private JournalLine _exit;

	public override void _Ready()
	{
		GD.Print("Ready");
		MasterAudio.GetInstance().SetNoRestart();

		_returnButton = GetNode<ChangeSceneButton>("BackToWork");
		_returnButton.Pressed += Close;

		_openDeck = GetNode<JournalLine>("ViewDeck");
		_openDeck.OnLineClicked += OpenDeck;

		_openSettings = GetNode<JournalLine>("Settings");
		_openSettings.OnLineClicked += OpenSettings;

		_exit = GetNode<JournalLine>("Exit");
		_exit.OnLineClicked += MainMenu;
		
	}
	public override void _Process(double delta)
	{

	}

	public async void MainMenu()
	{		
		MasterScene oldMaster = MasterScene.GetInstance();
		MasterScene newMaster = GD.Load<PackedScene>(_mainMenuUID).Instantiate<MasterScene>();

		await Task.Delay(500);
		Engine.TimeScale = 1f;
		
		GD.Print("Assigning");

		GetTree().Root.AddChild(newMaster);
		oldMaster.Free();
		
		return;
	}

	public void Open()
	{
		Visible = true;
		Engine.TimeScale = 0f;
	}
	public void Close()
	{
		Visible = false;
		Engine.TimeScale = 1f;
		// QueueFree();        
	}

	private void OpenDeck()
	{
		GD.Print("Deck opened");
		Shop shop = (Shop)MasterScene.GetInstance().GetActiveScene<ILevel>().UseElevator(_inventoryUID);
	}

	private void OpenSettings()
	{
		MasterScene.GetInstance().GetActiveScene<ILevel>().UseElevator(_settingsUID);		
	}
}
