using Godot;
using Overworld;
using System;

public partial class LevelSelectOption : Button
{
    [Export] private bool _useShader;
    [Export] private string _uid;
    [Export] private bool _returnToPurgatory;
    [Export] bool _middleManagement;
    [Export] private bool _lockedUntilTutorialComplete;
    [Export] private bool _externallyControlled;
    ILevel _level;
    ILevel _purgatory;

    public bool Enabled;

    public delegate void ButtonClickInformer();
    public event ButtonClickInformer OnButtonClicked;

    public override void _Ready()
    {
        Enabled = true;
        _level = GetOwnerOrNull<ILevel>();
        _purgatory = _level;
        
        if (_useShader)            
            (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);        
    }
    public void OnMouseEntered()
    {
        if (!_useShader)
            return;
        if ((_lockedUntilTutorialComplete && !QuestManager.GetInstance().FLAG_ACQUIRED_CROWN) || !Enabled)
            (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);
        else
            (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 1f);
    }

    public void OnMouseExited()
    {
        if (!_useShader)
            return;
        (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);
    }

    public void OnPressed()
    {
        OnButtonClicked?.Invoke();
        if ((_lockedUntilTutorialComplete && !QuestManager.GetInstance().FLAG_ACQUIRED_CROWN) || !Enabled || _externallyControlled)
            return;        

        _level = GetOwnerOrNull<ILevel>();
        
        if (_level == null)
        {
            // Ugly but whatever
                        
            if (_middleManagement)
                QuestManager.GetInstance().FLAG_TALKED_TO_MANAGER = true;

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
