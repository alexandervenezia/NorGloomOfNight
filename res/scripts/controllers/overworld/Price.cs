using Godot;
using System;

public partial class Price : Node2D
{
    private Label _cost;

    public override void _Ready()
    {
        _cost = GetNode<Label>("Label");
    }

    public void SetPrice(int price)
    {
        _cost.Text = price.ToString();
    }
}
