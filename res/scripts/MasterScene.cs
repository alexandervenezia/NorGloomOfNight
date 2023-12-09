/*
@author Alexander Venezia (Blunderguy)
*/

using Combat;
using Data;
using Godot;
using Overworld;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MasterScene : Node
{
	[Export] private Godot.Collections.Array<string> _packedSceneUIDs = new();
	[Export] private string _defaultSceneUID;
	[Export] private string _combatSceneUID;
	public string CombatSceneUID => _combatSceneUID;
	//[Export] private string _combatSceneUID;
	private Dictionary<string, Node> _loadedScenes;
	private string _activeScene = "";
	private string _lastScene;

	private List<int> _enemyIDs;
	private int _combatID;
	private int _playerHP = -1;
	private int _coinsFound;
	public int TotalCoins;
	private bool _isBoss;

	private int _playerMaxHP = -1;

	private static MasterScene _instance;

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			HandleQuit();			
		}
	}

	private async void HandleQuit()
	{
		bool complete = false;
		EnemyAssetLookup.GetInstance().FreeMemory(out complete);

		GD.Print("Freeing memory");
		while (!complete)
		{
			await Task.Delay(100);
		}
		GD.Print("Memory freed");

		//Free();
		GetTree().Quit(); // default behavior
	}

	public static MasterScene GetInstance()
	{
		return _instance;
	}

	public override void _Ready()
	{
		GetTree().AutoAcceptQuit = false;

		_instance = this;
		_loadedScenes = new();

		//LoadScene(_combatSceneUID);
		LoadScene(_defaultSceneUID);

		ActivateScene(_defaultSceneUID, includeLoad: false);
		_activeScene = _defaultSceneUID;
		_lastScene = "";
	}

	public void ResetVars()
	{
		LoadEnemyIDs();
		_playerHP = _playerMaxHP;
		CollectCoins();
		QuestManager.GetInstance().Reset();
		ShopEntrance.ResetShops();
		MasterDeck.ResetPlayerDeck();

	}

	public List<int> LoadEnemyIDs()
	{
		return _enemyIDs;
	}

	// Overworld team - call this before switching scenes to combat to set the enemies the player will face.
	public void SetEnemyIDs(List<int> enemyIDs)
	{
		_enemyIDs = enemyIDs;
	}

	public void SetPlayerHP(int hp)
	{
		if (_playerMaxHP == -1)
			_playerMaxHP = hp;

		_playerHP = hp;
	}

	public bool GetIsBoss()
	{
		bool temp = _isBoss;
		_isBoss = false;
		return temp;
	}

	public void SetIsBoss(bool boss)
	{
		_isBoss = boss;
	}

	public int LoadPlayerHP()
	{
		return _playerHP;
	}

	public void AddCoins(int coins)
	{
		_coinsFound += coins;
		TotalCoins += coins;
	}

	public int CollectCoins()
	{
		int temp = _coinsFound;
		_coinsFound = 0;
		return temp;
	}

	public void SetCombatBackground(int id)
	{
		_combatID = id;
	}

	public int LoadCombatBackground()
	{
		int temp = _combatID;
		_combatID = 0;
		return temp;
	}

	public bool IsInCombat()
	{
		return _activeScene == _combatSceneUID;
	}


	public void LoadScene(string sceneUID)
	{
		if (_loadedScenes.ContainsKey(sceneUID))
			throw new Exception("ERROR: Attempted to load scene which is already loaded");
		PackedScene scene = GD.Load<PackedScene>(sceneUID);
		_loadedScenes.Add(sceneUID, scene.Instantiate());
	}

	public Node ActivateScene(string uid, bool includeLoad = false, bool forceReload = false)
	{
		if (forceReload && _loadedScenes.ContainsKey(uid))
		{
			GD.Print("\n\nWiping " + uid);
			WipeScene(uid);

		}

		if (!_loadedScenes.ContainsKey(uid))
		{
			if (!includeLoad)
				throw new Exception("ERROR: Attempted to active scene which has not been loaded");
			LoadScene(uid);
			GD.Print("Loading as activation");
		}

		_lastScene = _activeScene;
		if (_activeScene != "")
		{
			GD.Print("Removing " + _activeScene);
			GD.Print(_loadedScenes[_activeScene].Name);
			RemoveChild(_loadedScenes[_activeScene]);
		}
		_activeScene = uid;
		AddChild(_loadedScenes[_activeScene]);

		if (_loadedScenes[_activeScene] is ILevel)
			((ILevel)_loadedScenes[_activeScene]).Reactivate();

		return _loadedScenes[_activeScene];
	}


	public void ActivateSceneAndWipeCurrent(string destUID)
	{
		ActivateScene(destUID, false);
		WipeScene(_lastScene);
		_lastScene = "";
	}

	public void ActivatePreviousScene(bool wipe=false)
	{           
		ActivateScene(_lastScene, false);
		if (wipe)
		{
			WipeScene(_lastScene);
			_lastScene = "";
		}
	}

	public void WipeScene(string uid)
	{
		if (!_loadedScenes.ContainsKey(uid))
			throw new Exception("ERROR: Attempted to delete scene which has not been loaded");

		_loadedScenes[uid].QueueFree();
		_loadedScenes.Remove(uid);
	}

	public void Pause()
	{
		Pause(this);
	}

	private void Pause(Node curr)
	{
		foreach (Node child in curr.GetChildren())
		{
			if (child is IPausable)
			{
				(child as IPausable).Pause();
			}

			Pause(child);
		}
	}

	public void Unpause()
	{
		Unpause(this);
	}

	private void Unpause(Node curr)
	{
		foreach (Node child in curr.GetChildren())
		{
			if (child is IPausable)
			{
				(child as IPausable).Pause();
			}

			Unpause(child);
		}
	}

	public T GetActiveScene<T>()
	{
		return (T)(object)_loadedScenes[_activeScene];
	}
}
