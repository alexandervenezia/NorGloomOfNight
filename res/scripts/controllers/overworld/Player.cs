namespace Overworld;

using Godot;
using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;


public enum State
{
	GROUNDED,
	AIRBORNE
}

public partial class Player : CharacterBody2D
{
	private readonly IQuest[] _quests = {
		new TalkToManager(),
		new CollectCrown(),
		new DeliverCrown(),
		new EmptyQuest()
	};
	private IQuest _quest;
	[Export] private Node2D _questUI;
	[Export] private int _maxHealth = 25;
	public int MaxHealth => _maxHealth;
	private int _currentHealth;
	public int CurrentHealth => _currentHealth;
	private int _coins; // TODO: Set to zero
	public int Coins => _coins;
	[Export] private float _walkSpeed = 500f;
	[Export] private float _sprintSpeed = 1700f;
	[Export] private float _jumpTime; // Time spent in upward acceleration
	[Export] private float _jumpSpeed = 500f; // Speed player moves upward while jumping
	[Export] private float _gravityFallMultiplier = 2f;
	[Export] private float _accelerationStrength = 0.09f;
	[Export] private float _coyoteBuffer = 0.5f; // In seconds
	[Export] private float _gravityMult = 3;

	[Export] private string _gameOverScene;
	[Export] private string _creditsScene;

	[Export] private Sprite2D _balancedScale;
	[Export] private Sprite2D _unbalancedScale;

	[Export] private Cutscene _cutsceneCreditsTrigger;
	

	private float _coyoteTimer;
	private float _gravityDefault;
	private State _state;
	private AnimatedSprite2D _playerSprite;
	private CharacterBody2D _charcterBody;
	private CutscenePlayer _cutscenePlayer;
	private Sprite2D _interactionIcon;
	private Vector2 _interactionIconBasePos;
	private float _interactionIconBob;

	public delegate void CombatStart(ICombatable enemy);
	public event CombatStart EnemyAggroed;

	private AudioStreamPlayer _walkSound;
	private AudioStreamPlayer _runSound;
	private AudioStreamPlayer _jumpSound;
	private AudioStreamPlayer _landSound;
	private AudioStreamPlayer _hurtSound;
	private AudioStreamPlayer _encounterSound;
	public AudioStreamPlayer EncounterSong => _encounterSound;

	private bool _cutsceneFinished = false;
	private bool _glideState = false;
	private bool _playingCutscene = false;
	public bool PlayingCutscene => _playingCutscene;

	private bool _interactableOnForThisFrame;

	private CutsceneZone _cutsceneZoneWithin;

	public override void _Ready()
	{
		_gravityDefault = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		_gravityDefault *= _gravityMult;
		_state = State.GROUNDED;
		_coyoteTimer = _coyoteBuffer;
				
		_playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_cutscenePlayer = GetNode<CutscenePlayer>("Camera2D/CutscenePlayer");
		_interactionIcon = GetNode<Sprite2D>("E");
		_interactionIcon.Visible = false;

		_walkSound = (AudioStreamPlayer)GetNode("WalkSound");
		_runSound = (AudioStreamPlayer)GetNode("RunSound");
		_jumpSound = (AudioStreamPlayer)GetNode("JumpSound");
		_landSound = (AudioStreamPlayer)GetNode("LandSound");
		_hurtSound = GetNode<AudioStreamPlayer>("DmgPhysical");
		_encounterSound = GetNode<AudioStreamPlayer>("EncounterSound");

		_quest = _quests[0];
		UpdateQuestUI();
		QuestManager.GetInstance().FlagChanged += QuestFlagUpdated;
		_cutscenePlayer.OnEnd += OnCutsceneEnd;

		// PlayFootsteps();
		_interactableOnForThisFrame = false;
		_interactionIconBasePos = _interactionIcon.Position;
	}

	private void QuestFlagUpdated()
	{
		if (_quest.IsFinished())
		{
			_quest = _quests[Array.IndexOf(_quests, _quest) + 1];
		}
		UpdateQuestUI();

		if (QuestManager.GetInstance().FLAG_ACQUIRED_CROWN)
		{
			_balancedScale.Visible = false;
			_unbalancedScale.Visible = true;
		}

	}

	private void UpdateQuestUI()
	{
		if (_quest == null)
		{
			_questUI.GetNode<RichTextLabel>("QuestName").Text = "";
			_questUI.GetNode<RichTextLabel>("QuestCommand").Text = "[right][font_size=125]Your work is done . . . for now.";
		}

		_questUI.GetNode<RichTextLabel>("QuestName").Text = "[right][font_size=200]" + _quest.GetName();
		_questUI.GetNode<RichTextLabel>("QuestCommand").Text = "[right][font_size=125]" + _quest.GetNextStep();
	}

	public void FrameChange()
	{
		if (_playerSprite.Animation == "jump" && _playerSprite.Frame == _playerSprite.SpriteFrames.GetFrameCount("jump")-1)
			_glideState = true;
	}

	public override void _Process(double delta)
	{
		if (Engine.TimeScale < 0.01f)
		{
			return;
		}

		ChangePlayerOrientation();

		if (IsIdle())
		{
			_playerSprite.Play("idle");

		}
		else if (IsSprinting())
		{
			_playerSprite.Play("sprint");
			if (!_runSound.Playing)
				_runSound.Play();
		}
		else if (IsJumping())
		{
			if (!_glideState)
			{
				_playerSprite.Play("jump");
			}
			else
			{
				if (Input.IsActionPressed("Jump"))
					_playerSprite.Play("glide");
				else
					_playerSprite.Play("idle");
			}
		}
		else if (IsWalking())
		{
			_playerSprite.Play("walk");
			if (!_walkSound.Playing)
				_walkSound.Play();
		}

		

		//Shop shop = (Shop)_level.UseElevator(_shopUID);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.TimeScale < 0.01f && _playingCutscene)
		{
			_playerSprite.Play("idle");
			Velocity = new Vector2(0, Velocity.Y + _gravityDefault);
			MoveAndCollide(Velocity * 0.01f);
			return;
		}
		else if (Engine.TimeScale < 0.01f)
		{
			return;
		}

		float fDelta = (float)delta;
		Godot.Vector2 movementInput = GetMovementInput();

		Godot.Vector2 vel = Velocity;

		if (IsOnFloor())
		{
			_state = State.GROUNDED;
		}
		else
		{

			_state = State.AIRBORNE;
		}

		float accelerationRate;
		// Continuing in current direction
		if ((movementInput.X > 0) == (Velocity.X > 0))
		{
			accelerationRate = (_walkSpeed - Mathf.Abs(Velocity.X)) / _walkSpeed;
		}
		else
		{
			accelerationRate = 2f;
		}

		switch (_state)
		{
			case State.GROUNDED:
				_coyoteTimer = _coyoteBuffer;
				vel.X = (float)Mathf.Lerp(vel.X, 0, 0.157);
				if (GetSprintInput())
				{
					vel += movementInput * accelerationRate * _sprintSpeed * _accelerationStrength;
				}
				else
				{
					vel += movementInput * accelerationRate * _walkSpeed * _accelerationStrength;
				}
				if (GetJumpInput())
				{
					vel.Y = -_jumpSpeed;
					_coyoteTimer = 0f;
					_glideState = false;
					_jumpSound.Play();
				}
				break;

			case State.AIRBORNE:
				_coyoteTimer -= fDelta;

				if (GetJumpInput() && _coyoteTimer >= 0)
				{
					vel.Y = -_jumpSpeed;
					_coyoteTimer = 0f;
					_glideState = false;
					_jumpSound.Play();
				}

				vel.X = (float)Mathf.Lerp(vel.X, 0, 0.075);
				vel.X += movementInput.X * accelerationRate * _walkSpeed * _accelerationStrength;

				if (vel.Y < 0)
				{
					vel.Y += _gravityDefault * fDelta;

				}
				else
				{
					float gravM = _gravityFallMultiplier;

					if (Input.IsActionPressed("Jump"))
						gravM = 0.25f;

					vel.Y += _gravityDefault * gravM * fDelta;
				}

				break;
		}

		Velocity = vel;

		MoveAndSlide();

		_interactionIcon.Visible = _interactableOnForThisFrame || (_cutsceneZoneWithin != null && _cutsceneZoneWithin.RequireInteraction);
		_interactableOnForThisFrame = false;
		UpdateInteractionIcon(fDelta);

		if (IsInstanceValid(_cutsceneZoneWithin))
		{
			if (Input.IsActionJustReleased("ui_interact"))
				_ = PlayCutscene(_cutsceneZoneWithin.Cutscene);
		}
	}

	private void UpdateInteractionIcon(float delta)
	{
		_interactionIcon.Position = _interactionIconBasePos + new Vector2(0, Mathf.Sin(_interactionIconBob) * 20f);
		_interactionIconBob += delta * 4f;
	}

	private Godot.Vector2 GetMovementInput()
	{
		Godot.Vector2 movement = new();

		movement.X = Input.GetAxis("ui_left", "ui_right");

		return movement;
	}

	private bool GetJumpInput()
	{
		return Input.IsActionJustPressed("Jump");
	}

	private bool GetSprintInput()
	{
		return Input.IsActionPressed("ui_sprint");
	}

	private bool IsSprinting()
	{

		if (IsOnFloor() && GetSprintInput())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool IsWalking()
	{
		if (IsOnFloor() && !IsSprinting())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool IsIdle()
	{
		Godot.Vector2 movementInput = GetMovementInput();
		if (movementInput.X == 0 && IsOnFloor())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool IsJumping()
	{
		if (!IsOnFloor())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private void ChangePlayerOrientation()
	{
		Godot.Vector2 movementInput = GetMovementInput();
		if (movementInput.X > 0)
		{
			_playerSprite.FlipH = true;
		}
		else if (movementInput.X < 0)
		{
			_playerSprite.FlipH = false;
		}
	}

	private void OnArea2DEntered(Area2D area)
	{
		if (area.GetOwnerOrNull<ICombatable>() is ICombatable)
		{
			if (area.GetOwner<ICombatable>().IsEnabled())
				EnemyAggroed?.Invoke(area.GetOwner<ICombatable>());
		}
		if (area is ShopEntrance)
		{			
			GD.Print("Enter");
			((ShopEntrance)area).Enter();
		}
		if (area is HealArea)
		{
			GD.Print("HealEnter");			
			((HealArea)area).Enter();
		}
		if (area is Spike)
		{
			GD.Print("SpikeEnter");
			FloatingTextFactory.GetInstance().CreateFloatingText(((Spike)area).Damage.ToString(), Position-Godot.Vector2.Left*50 + Godot.Vector2.Up * 100, fontSize:150, color:"red", sizeMult:5f);
			HandleSpikeHit(((Spike)area).Damage);
		}
		if (area is CutsceneZone)
		{
			GD.Print("Cutscene zone");
			if ((area as CutsceneZone).ShouldRun())
			{
				_ = PlayCutscene((area as CutsceneZone).Cutscene);
				(area as CutsceneZone).Burned = true;
			}
			
			else if ((area as CutsceneZone).RequireInteraction)
			{
				_cutsceneZoneWithin = ((CutsceneZone)area);
			}
		}
	}

	private void OnArea2DExited(Area2D area)
	{
		if (area is ShopEntrance)
		{			
			GD.Print("Exit");
			((ShopEntrance)area).Exit();
		}
		if (area is HealArea)
		{
			GD.Print("HealExit");
			((HealArea)area).Exit();
		}
		if (area == _cutsceneZoneWithin)
		{
			_cutsceneZoneWithin = null;
		}

	}

	public async void TakeDamage(int dmg, bool spawnText=true)
	{
		_hurtSound.Play();

		if (spawnText)
			FloatingTextFactory.GetInstance().CreateFloatingText(dmg.ToString(), Position-Godot.Vector2.Left*50 + Godot.Vector2.Up * 100, fontSize:150, color:"red", sizeMult:5f);
		_currentHealth -= dmg;
		if (_currentHealth <= 0)
			Die();
		
		_playerSprite.SelfModulate = Colors.Red;
		await Task.Delay(250);
		_playerSprite.SelfModulate = Colors.White;
	}

	private void OnCutsceneEnd()
	{
		GD.Print("OnEnd");
		_cutsceneFinished = true;
	}

	public async Task PlayCutscene(Cutscene cutscene)
	{
		_playingCutscene = true;
		_cutscenePlayer.Visible = true;
		_cutsceneFinished = false;
		_cutscenePlayer.SetCutscene(cutscene);

		await Task.Run(() => {
			_cutsceneFinished = false;

			while (!_cutsceneFinished)
			{
				Thread.Sleep(100);
			}		

			});

		_cutscenePlayer.Visible = false;
		_playingCutscene = false;

		if (cutscene == _cutsceneCreditsTrigger)
		{
			GD.Print("Roll credits");

			MasterAudio.GetInstance().ClearQueue();
			MasterScene.GetInstance().ResetVars();
			MasterScene.GetInstance().CallDeferred("ActivateScene", _creditsScene, true, true);
		}
		
		
	}

	public void SetHealth(int hp)
	{
		_currentHealth = hp;
	}

	public void AddCoins(int coins)
	{
		_coins += coins;
		GD.Print(_coins, " coins");
	}

	public void RemoveCoins(int coins)
	{
		_coins -= coins;
	}

	public void FullHeal(bool msg=false)
	{
		if (msg)
			FloatingTextFactory.GetInstance().CreateFloatingText("Full Heal!", Position-Godot.Vector2.Left*50, fontSize:150, color:"green", sizeMult:5f);

		_currentHealth = MaxHealth;
		GD.Print("Healed! Health " + _currentHealth);
	}

	public void HandleSpikeHit(int dmg)
	{
		GD.Print("Hit spike.");
		TakeDamage(dmg, false);
		Velocity = new Godot.Vector2(-Velocity.X * 5, -Velocity.Y);
	}

	public void DebugTeleport(Vector2 dest)
	{
		Position += dest;
	}

	public void Die()
	{
		GD.Print("Player died - Die() stub called");
		MasterAudio.GetInstance().ClearQueue();
		MasterScene.GetInstance().ResetVars();
		//MasterScene.GetInstance().GetActiveScene<ILevel>().UseElevator(_gameOverScene);
		MasterScene.GetInstance().CallDeferred("ActivateScene", _gameOverScene, true, true);
		// TODO: Implement death screen
	}

	public void SetInteractable(bool canInteract)
	{
		_interactableOnForThisFrame = _interactableOnForThisFrame || canInteract;
		// _interactionIndicator.Visible = canInteract;
	}
}



