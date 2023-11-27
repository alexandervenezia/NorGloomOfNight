using Combat;
using Godot;
using System;

public partial class Health : Sprite2D
{
    private Combatant _parent;
    private Label _label;

    [Export] private bool _overworld;

    public override void _Ready()
    {
        if (!_overworld)
            _parent = (Combatant)GetParent().GetParent();

        _label = (Label)GetChild(0);
        Visible = false;
    }
    public override void _Process(double delta)
    {
        Visible = true;
        int health;

        if (!_overworld)
            health = _parent.GetHealth();
        else
            health = MasterScene.GetInstance().GetActiveScene<ILevel>().GetPlayer().CurrentHealth;

        if (health <= 0)
            Visible = false;
        
        _label.Text = health.ToString();
        
    }
}
