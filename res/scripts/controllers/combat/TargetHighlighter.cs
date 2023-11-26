using Godot;
using System;

public partial class TargetHighlighter : ColorRect
{
    private ShaderMaterial _mat;

    public override void _Ready()
    {
        _mat = Material as ShaderMaterial;
        Visible = false;
    }

    public void SetHue(Vector4 color)
    {
        _mat.SetShaderParameter("HUE", color);
    }
}
