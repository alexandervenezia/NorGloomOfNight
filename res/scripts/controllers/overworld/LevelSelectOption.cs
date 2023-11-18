using Godot;
using System;

public partial class LevelSelectOption : Button
{
    ILevel _level;

    public override void _Ready()
    {
        _level = GetOwner<ILevel>();
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
        GetParent().GetParent<Control>().Visible = false;
        _level.UseElevator();
    }
}
