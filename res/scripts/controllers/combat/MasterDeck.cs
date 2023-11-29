/*
 @author Alexander Venezia (Blunderguy)
*/

// This class manages all card types in the game
namespace Data;

using System.Collections.Generic;
using System.Linq;
using Data;
using Godot;

public partial class MasterDeck : Node
{
	[Export] private Godot.Collections.Array<CardData> _cardTypes = new();
	public static List<CardData> CardTypes = new();

	[ExportGroup("Player Starter Deck")]
	[Export] private Godot.Collections.Array<CardData> _playerStarterDeck;

	[ExportGroup("Card Appearance")]
	
	//[Export] private Godot.Collections.Array<string> _damageIconTypes;
	[Export] private Godot.Collections.Dictionary<DamageType, string> _damageIcons;
	private static Dictionary<DamageType, string> _damageIconsStatic; // This is very dumb.
	public static Dictionary<DamageType, Texture2D> DamageIcons;

	public delegate void OnMasterDeckLoad();
	public static event OnMasterDeckLoad OnLoad;

	private static Systems.Combat.Deck _playerDeck;
	public static Systems.Combat.Deck PlayerDeck => _playerDeck;

	private static List<CardData> _playerStarterDeckStatic; // This is dumb. Don't use static classes. Singletons work way better with Godot export fields.

	public override void _Ready()
	{
		_playerDeck = new();
		DamageIcons = new();
		_damageIconsStatic = new();
		_playerStarterDeckStatic = new();

		foreach (DamageType type in _damageIcons.Keys)
		{
			ResourceLoader.LoadThreadedRequest(_damageIcons[type]);
			DamageIcons.Add(type, null);
			_damageIconsStatic.Add(type, _damageIcons[type]);
		}

		foreach (CardData c in _cardTypes)
		{
			c.Activate();
			CardTypes.Add(c);
		}

		List<CardData> starterDeck;

		if (_playerStarterDeck == null || _playerStarterDeck.Count == 0)
		{
			starterDeck = _cardTypes.ToList<CardData>();
		}
		else
			starterDeck = _playerStarterDeck.ToList<CardData>();

		foreach (CardData c in starterDeck)
		{			
			_playerDeck.AddCard(c);
			_playerStarterDeckStatic.Add(c);
		}

		// _cardTypes.Clear();
		OnLoad?.Invoke();
	}

	public static Texture2D GetDamageIcon(DamageType type)
	{
		if (DamageIcons[type] == null)
		{
			DamageIcons[type] = (Texture2D)ResourceLoader.LoadThreadedGet(_damageIconsStatic[type]);
		}

		return DamageIcons[type];
	}

	public static void ResetPlayerDeck()
	{
		_playerDeck = new();

		foreach (CardData c in _playerStarterDeckStatic)
		{			
			_playerDeck.AddCard(c);
		}
	}

}