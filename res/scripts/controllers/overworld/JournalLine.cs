using Godot;
using System;

public partial class JournalLine : Area2D
{
    private Sprite2D _xButton;

    public delegate void LineClickWatcher();
    public event LineClickWatcher OnLineClicked;

    public override void _Ready()
    {
        _xButton = GetNode<Sprite2D>("X");
        _xButton.Visible = false;

        MouseEntered += OnMouseEnter;
        MouseExited += OnMouseExit;
    }

    private void OnMouseEnter()
    {
        _xButton.Visible = true;
    }

    private void OnMouseExit()
    {
        _xButton.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Select"))
        {
            if (_xButton.Visible)
                OnLineClicked?.Invoke();
        }
    }

}
