using Godot;
using Overworld;
using System;

public partial class LevelSelectOption : Button
{
    [Export] private string _uid;
    [Export] private bool _returnToPurgatory;
    [Export] private bool _lockedUntilTutorialComplete;
    ILevel _level;
    ILevel _purgatory;

    public bool Enabled;

    public override void _Ready()
    {
        Enabled = true;
        _level = GetOwnerOrNull<ILevel>();
        _purgatory = _level;
        (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);
    }
    public void OnMouseEntered()
    {
        if ((_lockedUntilTutorialComplete && !QuestManager.GetInstance().FLAG_ACQUIRED_CROWN) || !Enabled)
            (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);
        else
            (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 1f);
    }

    public void OnMouseExited()
    {

        (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);
    }

    public void OnPressed()
    {
        if ((_lockedUntilTutorialComplete && !QuestManager.GetInstance().FLAG_ACQUIRED_CROWN) || !Enabled)
            return;

        _level = GetOwnerOrNull<ILevel>();
        
        if (_level == null) // Middle Management only
        {
            // Ugly but whatever
            QuestManager.GetInstance().FLAG_TALKED_TO_MANAGER = true;
            MasterScene.GetInstance().CallDeferred("ActivatePreviousScene", true);
            return;
        }
        
        if (!_returnToPurgatory)
            GetParent().GetParent<Control>().Visible = false;
        else
            GetParent().GetParent<Control>().Visible = false;

        _level.UseElevator(_uid);
    }
}
