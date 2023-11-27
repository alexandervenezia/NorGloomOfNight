namespace Overworld;

using Godot;
using Overworld;
using System;

public partial class CoinUICounter : Sprite2D
{
    private Label _text;

    public override void _Ready()
    {
        _text = GetNode<Label>("Label");
    }

    public override void _Process(double delta)
    {
        _text.Text = MasterScene.GetInstance().GetActiveScene<ILevel>().GetPlayer().Coins.ToString();
    }
}
