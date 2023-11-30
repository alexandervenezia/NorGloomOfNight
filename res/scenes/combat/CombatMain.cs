/*
 @author Alexander Venezia (Blunderguy)
*/

namespace Combat;

using Data;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Systems.Combat;

enum CombatRewardState
{
	NOT_OFFERED,
	PLAYER_CHOOSING,
	AWAITING_CONFIRMATION,
	CHOICE_MADE,
}

public partial class CombatMain : Node2D
{
	[ExportGroup("Right click .tscn -> get UID -> paste here. Blame Godot for having to use freaking strings because they still haven't fixed a bug reported over a year ago: https:[slashslash]github.com[slash]godotengine[slash]godot[slash]issues[slash]62916")]
	[Export] private Godot.Collections.Array<string> _enemyTypePaths;
	[Export] private PackedScene _cardResource;
	[Export] private PackedScene _nullCardResource;
	[Export] private PackedScene _coinValueResource;
	[Export] private string _music;
	[Export] private Texture2D _rejectButtonImg;
	private Dictionary<int, PackedScene> _enemyTypesByUID;

	private Node2D _rewardsNode;
	private bool _isOver = false;
	public bool IsFightOver => _isOver;

	private CombatRewardState _rewardState = CombatRewardState.NOT_OFFERED;
	private Card _selectedRewardCard;
	private Card _rejectCard;

	public Vector2[] EnemySpawnPoints =
	{        
		new Vector2(500, -40),
		new Vector2(200, -100),
		new Vector2(-100, 0),
		new Vector2(-300, 40)
	};

	private List<Drop> _loot;

	private void PopulateEnemyArray(ref ICombatant[] enemies)
	{

		_enemyTypesByUID = new();

		// This is stupid, but that's Godot's fault.
		for (int i = 0; i < _enemyTypePaths.Count; i++)
		{
			PackedScene converted = GD.Load<PackedScene>(_enemyTypePaths[i]);
			if (converted == null)
				continue;

			Enemy tempInstance = (Enemy)converted.Instantiate();

			if (_enemyTypesByUID.ContainsKey(tempInstance.UID))
				throw new Exception("Two enemies conflict with shared UID " + tempInstance.UID);

			_enemyTypesByUID.Add(tempInstance.UID, converted);
			tempInstance.QueueFree();
		}
		Node enemiesParentNode = GetNode("Enemies");

		int[] enemyUIDs;
		int[] loaded = null;
		loaded = ((MasterScene)GetTree().Root.GetChild(0)).LoadEnemyIDs()?.ToArray();

		if (loaded != null)
			enemyUIDs = loaded;
		else
		{
			GD.Print("No enemy data received from overworld.");
			enemyUIDs = new int[] { 1, 2, 1, 1 };
		}

		enemies = new ICombatant[enemyUIDs.Length];

		int count = 0;
		Enemy newEnemy;
		foreach (int e in enemyUIDs)
		{
			newEnemy = (Enemy)_enemyTypesByUID[e].Instantiate();
			
			//RemoveChild(newEnemy);
			enemiesParentNode.AddChild(newEnemy);
			newEnemy.GlobalPosition = EnemySpawnPoints[count];
			enemies[count] = newEnemy;
			count++;
		}

	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 mousePos = GetGlobalMousePosition();
		PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
		PhysicsPointQueryParameters2D query = new();
		query.CollideWithAreas = true;
		query.Position = mousePos;
		query.CollisionMask = 0b0010;
		var hits = spaceState.IntersectPoint(query);

		if (_rewardState == CombatRewardState.PLAYER_CHOOSING)
			_selectedRewardCard = null;
		if (_rewardState == CombatRewardState.AWAITING_CONFIRMATION)
		{
			if (Input.IsActionJustPressed("Select"))
				_rewardState = CombatRewardState.CHOICE_MADE;  
		}

		foreach (var h in hits)
		{
			if (((Card)h["collider"].Obj).GetParent() == _rewardsNode)
			{
				_selectedRewardCard = (Card)h["collider"];
				break;
			}
		}

		if (_selectedRewardCard != null)
		{
			if (Input.IsActionJustPressed("Select"))
				_rewardState = CombatRewardState.CHOICE_MADE;            
		}

		if (_rewardState == CombatRewardState.PLAYER_CHOOSING)
			UpdateCardProtrusion();
	}

	private void UpdateCardProtrusion()
	{
		Godot.Collections.Array<Node> children = _rewardsNode.GetChildren();

		for (int i = 0; i < children.Count; i++)
		{
			if (children[i] is not Card)
				continue;
			Card c = (Card)children[i];
			
			c.SetMouseOverStatus(c == _selectedRewardCard);
			c.Scale = new Vector2(0.4f, 0.4f) * ((400f + c.Offset) / 475f);

		}
	}

	/*
	public void OnTreeEntered()
	{
		GD.Print("Entering tree");
		ICombatant[] enemies = null;
		PopulateEnemyArray(ref enemies);
	}
	*/

	public override void _Ready()
	{
		int combatBGID = MasterScene.GetInstance().LoadCombatBackground();
		GD.Print("Background ID: " + combatBGID);
		GetNode<Background>("Background").SetBG(EnemyAssetLookup.GetInstance().GetCombatBackground(combatBGID));
		MasterAudio.GetInstance().ClearQueue();
		MasterAudio.GetInstance().PlaySong(_music);
		
		_rewardsNode = GetNode<Node2D>("Rewards");

		ICombatant[] enemies = null;
		PopulateEnemyArray(ref enemies);

		_loot = new();

		foreach (Enemy e in enemies)
		{
			_loot.Add(e.GetDrop());
		}

		Player player = (Player)GetNode("Player");
		CombatManager.GetInstance().NewFight(player, enemies);
	}

	public void SetPlayerHP(int hp)
	{
		((Player)GetNode("Player")).SetHP(hp);
	}

	public int GetPlayerHP()
	{
		return ((Player)GetNode("Player")).CurrentHealth;
	}

	public async void EndFight(EndState result)
	{
		_isOver = true;
		GD.Print("Is over - " + result);

		
		MasterDeck.PlayerDeck.ForceFullReshuffle();

		if (result == EndState.VICTORY)
		{
			int goldReward_ref = 0;

			InitReward(ref goldReward_ref);

			await Task.Delay(500); // Provide a pleasant pause after combat ends

			while (_rewardState != CombatRewardState.CHOICE_MADE)
			{
				await Task.Delay(100);
			}

			GD.Print("Reward chosen: " + _selectedRewardCard);
			GD.Print("Gold found: " + goldReward_ref);

			if (IsInstanceValid(_selectedRewardCard) && _selectedRewardCard != _rejectCard)
			{
				GD.Print(MasterDeck.PlayerDeck.GetCardCount());
				MasterDeck.PlayerDeck.AddCard(_selectedRewardCard.Data);
				GD.Print(MasterDeck.PlayerDeck.GetCardCount());
			}

			AnimateRewardCards(_selectedRewardCard); // TODO: Make do something or remove

			_rewardState = CombatRewardState.NOT_OFFERED;
			_selectedRewardCard = null;

			MasterScene.GetInstance().AddCoins(goldReward_ref);        
		}
		else
			await Task.Delay(2000);

		MasterScene.GetInstance().SetPlayerHP(GetPlayerHP());
		MasterScene.GetInstance().CallDeferred("ActivatePreviousScene", true);
	}

	private void InitReward(ref int gold_ref)
	{        
		List<CardData> cardOptions = new();
		GD.Print("Drops:");
		foreach (Drop d in _loot)
		{
			GD.Print("Gold: " + d.Gold);
			GD.Print("Card: " + d.Card);

			if (cardOptions.Count < 3 && d.Card != null)
				cardOptions.Add(d.Card);

			gold_ref += d.Gold;
		}

		Price price = (Price)_coinValueResource.Instantiate();
		_rewardsNode.AddChild(price);
		price.SetPrice(gold_ref);
		price.Position = new Vector2(0, 100);

		if (cardOptions.Count == 0)
		{
			_rewardState = CombatRewardState.AWAITING_CONFIRMATION;
			return;
		}

		int i = 0;
		foreach (CardData c in cardOptions)
		{
			Card card = (Card)_cardResource.Instantiate();
			card.UpdateData(c);
			_rewardsNode.AddChild(card);
			card.ApplyScale(new Vector2(0.3f, 0.3f));

			card.Position = new Vector2(i * 400 - ((cardOptions.Count)/2f) * 400f, 0);

			i++;
		}            

		if (cardOptions.Count > 0)
		{
			_rejectCard = (Card)_nullCardResource.Instantiate();
			_rewardsNode.AddChild(_rejectCard);
			_rejectCard.ApplyScale(new Vector2(0.3f, 0.3f));

			_rejectCard.Position = new Vector2(i * 400 - ((cardOptions.Count)/2f) * 400f, 0);
		}

		_rewardState = CombatRewardState.PLAYER_CHOOSING;

	}

	private async void AnimateRewardCards(Card chosen)
	{
		foreach (Node2D card in _rewardsNode.GetChildren())
		{
			if (card != chosen)
				card.QueueFree();
			else
			{

			}
		}
	}

}
