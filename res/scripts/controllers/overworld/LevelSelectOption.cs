using Godot;
using System;

public partial class LevelSelectOption : Button
{
    [Export] private string _uid;
    [Export] private bool _returnToPurgatory;
    ILevel _level;
    ILevel _purgatory;

    public override void _Ready()
    {
        _level = GetOwnerOrNull<ILevel>();
        _purgatory = _level;
    }
    public void OnMouseEntered()
    {
        (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 1f);
    }

    public void OnMouseExited()
    {

        (Material as ShaderMaterial).SetShaderParameter("isHighlighted", 0f);
    }

    public void OnPressed()
    {
        _level = GetOwnerOrNull<ILevel>();
        
        if (_level == null) // Middle Management only
        {
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
