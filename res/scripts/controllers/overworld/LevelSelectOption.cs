using Godot;
using Overworld;
using System;

public partial class LevelSelectOption : Button
{
    [Export] private bool _decorationOnly;
    [Export] private bool _useShader;
    [Export] private string _uid;
    [Export] private bool _returnToPurgatory;
    [Export] bool _middleManagement;
    [Export] private bool _lockedUntilTutorialComplete;
    [Export] private bool _lockAfterTutorialComplete;
    [Export] private bool _externallyControlled;
    ILevel _level;
    ILevel _purgatory;

    public bool Enabled;

    public delegate void ButtonClickInformer();
    public event ButtonClickInformer OnButtonClicked;

    private Color _eventualColor;
    private string _eventualText;

    public override void _Ready()
    {
        


        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        Pressed += OnPressed;

        Enabled = true;
        _level = GetOwnerOrNull<ILevel>();
        _purgatory = _level;

        if (_lockedUntilTutorialComplete)
        {
            _eventualColor = ((StyleBoxFlat)GetThemeStylebox("normal")).BgColor;
            _eventualText = GetNode<RichTextLabel>("RichTextLabel").Text;
            GetNode<RichTextLabel>("RichTextLabel").Text = "[center][font_size=50]? ? ?";

            StyleBoxFlat newBox = new StyleBoxFlat();
            // AddThemeStyleboxOverride("Normal", null);
            RemoveThemeStyleboxOverride("normal");
            GD.Print("Test");

            QuestManager.GetInstance().FlagChanged += () =>
            {
                if (QuestManager.GetInstance().FLAG_ACQUIRED_CROWN)
                {
                    StyleBoxFlat newBox = new StyleBoxFlat();
                    newBox.BgColor = _eventualColor;
                    AddThemeStyleboxOverride("normal", newBox);
                    GetNode<RichTextLabel>("RichTextLabel").Text = _eventualText;
                }
            };
        }
        if (_lockAfterTutorialComplete)
        {
            QuestManager.GetInstance().FlagChanged += () =>
            {
                if (QuestManager.GetInstance().FLAG_ACQUIRED_CROWN)
                {
                    _decorationOnly = true;
                }
            };
        }
        
        if (_useShader)            
            (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);        
    }
    public void OnMouseEntered()
    {
        if (_decorationOnly)
            return;
        if (!_useShader)
            return;
        if ((_lockedUntilTutorialComplete && !QuestManager.GetInstance().FLAG_ACQUIRED_CROWN) || !Enabled)
            (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);
        else
            (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 1f);
    }

    public void OnMouseExited()
    {
        if (_decorationOnly)
            return;
        if (!_useShader)
            return;
        (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);
    }

    public void OnPressed()
    {
        GD.Print("PressedEarly");
        OnButtonClicked?.Invoke();
        if ((_lockedUntilTutorialComplete && !QuestManager.GetInstance().FLAG_ACQUIRED_CROWN) || !Enabled || _externallyControlled)
            return;        
        if (_decorationOnly)
            return;

        GD.Print("Pressed");

        _level = GetOwnerOrNull<ILevel>();
        
        if (_level == null)
        {
            GD.Print("RETURN SCENE");
            // Ugly but whatever
                        
            if (_middleManagement)
            {
                GD.Print("BUTTON IN MANAGEMENT PRESSED");
                QuestManager.GetInstance().FLAG_TALKED_TO_MANAGER = true;
            }

            MasterScene.GetInstance().CallDeferred("ActivatePreviousScene", true);
            return;
        }

        GD.Print(_level.GetPlayer().CurrentHealth);
        MasterScene.GetInstance().SetPlayerHP(_level.GetPlayer().CurrentHealth);
        
        if (!_returnToPurgatory)
            GetParent().GetParent<Control>().Visible = false;
        else
            GetParent().GetParent<Control>().Visible = false;

        _level.UseElevator(_uid);
    }
}
