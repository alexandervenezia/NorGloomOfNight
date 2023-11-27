 /*
 @author Alexander Venezia (Blunderguy)
*/

namespace Combat;

using Data;
using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

public partial class Card : Area2D
{
	private static Dictionary<CardRarity, Texture2D> _borderLookup;
	private readonly Color COMMON_COL = new Color("#FFFFD9");//new Color(1, 1, 1, 1);
	private readonly Color UNCOMMON_COL = new Color("#49E362"); //new Color(0.11f, 1, 0, 1);
	private readonly Color RARE_COL = new Color("#49ACE3"); //new Color(0, 0.44f, 0.86f, 1);
	private readonly Color EPIC_COL = new Color("#971A95"); //new Color(0.64f, 0.21f, 0.93f, 1);
	private readonly Color LEGENDARY_COL = new Color("#FEEE7D"); //new Color(1, 0.5f, 0, 1);

	[Export] private float _maxOffset = 75f;

	private Hand _handOwner;

	private float _offset;
	public float Offset => _offset;

	private Tween _tween;

	private bool _isMoused;

	public float AngleOffset;
	public Tween AngleTween;
	private int _currentAngleGoal;

	private float _rotationFactor = 0;
	public Tween _rotationFactorTween;

	private CardData _data;
	public CardData Data => _data;

	private ShaderMaterial _playerStampMat;

	private static void InitializeBorderLookup()
	{
		GD.Print("Loading card borders");
		_borderLookup = new();

		string path = "res://res/scenes/combat/subscenes/Card/images/transparent_backgrounds/";
		
		_borderLookup.Add(CardRarity.COMMON, GD.Load<Texture2D>(path + "COMMON.png"));
		_borderLookup.Add(CardRarity.UNCOMMON, GD.Load<Texture2D>(path + "UNCOMMON.png"));
		_borderLookup.Add(CardRarity.RARE, GD.Load<Texture2D>(path + "RARE.png"));
		_borderLookup.Add(CardRarity.EPIC, GD.Load<Texture2D>(path + "EPIC.png"));
		_borderLookup.Add(CardRarity.LEGENDARY, GD.Load<Texture2D>(path + "LEGENDARY.png"));
	}
	
	public void UpdateData(CardData data)
	{
		if (_borderLookup == null)
			Card.InitializeBorderLookup();

		_data = data;
		((Sprite2D)GetNode("Art")).Texture = data.Artwork;
		((RichTextLabel)GetNode("NameLabel")).Text = "[center][font_size=70]" + data.Name + "[/font_size][/center]";
		((RichTextLabel)GetNode("DescriptionLabel")).Text = "[center][font_size=45]" + data.Description + "[/font_size][/center]";

		string diceText = "";

		foreach (DamageType dt in data.Damage.Keys)
		{
			Texture2D icon = MasterDeck.GetDamageIcon(dt);
			string path = icon.ResourcePath;
			diceText += data.Damage[dt].ToString() + " " + dt + " [img color=black width=65]" + path + "[/img]\n";
		}
		foreach (DrawEffect de in data.DrawEffects.Keys)
		{
			diceText += de + " " + data.DrawEffects[de] + "\n";
		}
		foreach (Buff b in Data.Buffs)
		{
			string type = (b.Type == BuffType.RESISTANCE) ? Enum.GetName(b.ResistanceType) : "";
			diceText += "[color=#227733]" + ((b.Type == BuffType.RESISTANCE) ? "RES" : b.Type) + " " + type + ": " + b.Value + "[/color]\n";
		}
		foreach (Debuff d in Data.Debuffs)
		{
			diceText += "[color=#772233]" + d.Type + ": " + d.Duration + "[/color]\n";
 		}
		((RichTextLabel)GetNode("DamageLabel")).Text = "[font_size=50]" + diceText + "[/font_size]";

		((RichTextLabel)GetNode("TargetsLabel")).Text = ""; // "[right][font_size=50]" + data.Target + "[/font_size][/right]";

		Node2D targetingStamp = null;

		_playerStampMat = null;

		switch (data.Target)
		{
			case TargetType.SELF:
			targetingStamp = ((Node2D)GetNode("SelfStamp")); //.Visible = true;
			_playerStampMat = (((Sprite2D)targetingStamp.GetNode("PlayerStamp")).Material as ShaderMaterial);
			break;
			case TargetType.SINGLE:
			targetingStamp = ((Node2D)GetNode("SingleStamp"));
			break;
			case TargetType.MULTI_TWO:
			targetingStamp = ((Node2D)GetNode("DualStamp"));
			break;
			case TargetType.MULTI_THREE:
			targetingStamp = ((Node2D)GetNode("TripleStamp"));
			break;
			case TargetType.MULTI_FOUR:
			targetingStamp = ((Node2D)GetNode("QuadStamp"));
			break;
			case TargetType.ALL:
			targetingStamp = ((Node2D)GetNode("AllStamp"));
			_playerStampMat = (((Sprite2D)targetingStamp.GetNode("PlayerStamp")).Material as ShaderMaterial);
			break;
		}


		targetingStamp.Visible = true;

		Color rarityColor = new Color(1, 0, 1, 1);
		
		
		switch (data.Rarity)
		{
			case CardRarity.COMMON:
			rarityColor = COMMON_COL;
			// (((Sprite2D)targetingStamp.GetNode("PlayerStamp")).Material as ShaderMaterial).SetShaderParameter("tint", rarityColor);
			
			break;
			case CardRarity.UNCOMMON:
			rarityColor = UNCOMMON_COL;
			break;
			case CardRarity.RARE:
			rarityColor = RARE_COL;
			break;
			case CardRarity.EPIC:
			rarityColor = EPIC_COL;
			break;
			case CardRarity.LEGENDARY:
			rarityColor = LEGENDARY_COL;
			break;
		}

		if (_playerStampMat != null)
			_playerStampMat.SetShaderParameter("tint", rarityColor);

		targetingStamp.Modulate = rarityColor;
		//((Node2D)GetNode("Background")).Modulate = rarityColor;
		((Sprite2D)GetNode("Background")).Texture = _borderLookup[data.Rarity]; // GD.Load<Texture2D>("res://res/scenes/combat/subscenes/Card/images/transparent_backgrounds/RARE.png");

		((Label)GetNode("ActionPointIcon/ActionPointNumber")).Text = data.ActionPointCost.ToString();

		if (data.ActionPointCost == 0)
			((Node2D)GetNode("ActionPointIcon")).Hide();

		if (data.MovementPointCost == 0)
			((Node2D)GetNode("MovementPointIcon")).Hide();
		
	}

	private float _playAnimationVerticalPosition;
	private Vector2 _playAnimationPosition;
	private bool _animating = false;
	public async Task BeginPlayAnimation(Node2D discard)
	{
		((CollisionShape2D)GetNode("CollisionShape2D")).Disabled = true;
		_animating = true;
		_playAnimationPosition = Position;
		_tween = CreateTween();
		_tween.TweenProperty(this, "_playAnimationPosition", Vector2.Zero, 0.25f);		
		Tween rotTween = CreateTween();
		rotTween.TweenProperty(this, "rotation_degrees", 0, 0.25f);
		
		await Task.Delay(150);

		// Play animation/shader here
		// ((AnimatedSprite2D)GetNode("PlayAnimation")).Play();

		await Task.Delay(750);
		
		GetParent().RemoveChild(this);
		AddToDiscard(discard);

	}

	public void AddToDiscard(Node2D discard)
	{
		Position = Vector2.Zero;		

		if (discard.GetNodeOrNull("Top") == null)		
			discard.AddChild(this);
		else
		{
			Card temp = (Card)discard.GetNode("Top");
			discard.RemoveChild(temp);
			temp.QueueFree();
			discard.AddChild(this);
		}

		Name = "Top";

		//Position = discard.Position;
		Scale *= 0.8f;

		_animating = false;
	}

    public override void _Process(double delta)
    {
        if (_animating)
		{
			Position = _playAnimationPosition;
		}
    }

    public void SetMouseOverStatus(bool moused)
	{		
		if (moused == _isMoused)
			return;

		_isMoused = moused;

		if (moused)
		{
			_tween = CreateTween();
			_tween.TweenProperty(this, "_offset", _maxOffset, 0.25);
			_rotationFactorTween = CreateTween();
			_rotationFactorTween.TweenProperty(this, "_rotationFactor", 1.0f, 0.25f);
		}
		else
		{
			_tween = CreateTween();
			_tween.TweenProperty(this, "_offset", 0f, 0.25);
			_rotationFactorTween = CreateTween();
			_rotationFactorTween.TweenProperty(this, "_rotationFactor", 0f, 0.25f);
		}
	}

	public void SetHandOwner(Hand hand)
	{
		_handOwner = hand;
	}

	public void StartAngleTween(int goal, float duration)
	{
		if (_currentAngleGoal == goal)
			return;

		_currentAngleGoal = goal;

		AngleTween = CreateTween();
		AngleTween.TweenProperty(this, "AngleOffset", goal, duration);
	}

	public float GetRotationFactor()
	{
		return _rotationFactor;
	}

	public bool FullyExtended()
	{
		return _rotationFactor >= 0.95f;
	}

}
