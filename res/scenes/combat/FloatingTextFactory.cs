using Data;
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class FloatingTextFactory : Node2D
{
    [Export] Theme _theme;
    [Export] int _fontSize = 30;
    private static FloatingTextFactory _activeInstance;
    private Queue<RichTextLabel> _waitingQueue;

    private Dictionary<Vector2, ulong> _spawnLocationDelays;

    public override void _Ready()
    {
        GD.Print("New instance of FLoatingText");
        _activeInstance = this;
        _waitingQueue = new();
        _spawnLocationDelays = new();

        TreeEntered += OnEnterTree;
    }

    private void OnEnterTree()
    {
        GD.Print("Entered tree - FloaingText");
        _activeInstance = this;        
    }


    public static FloatingTextFactory GetInstance()
    {
        return _activeInstance;
    }

    public void CreateFloatingCardText(bool isHeal, bool isCrit, bool isPoison, bool isResist, int armorAmount, int amount, Vector2 position, DamageType type)
    {
        string color = isHeal ? "#33FF33" : "FF3333";
        color = isPoison ? "#FFBB22" : color;
        string prefix = isCrit ? "Critical! " : "";
        if (isResist)
            prefix += "Resist! ";
        
        Texture2D icon = MasterDeck.GetDamageIcon(type);
		string path = icon.ResourcePath;
        string col = isHeal ? "green" : "red";
        
        string message = String.Format("{1}{2} [img color="+col+" width=65]{3}[/img]", color, prefix, amount, path);

        Vector2 offset = Vector2.Up * 100;

        CreateFloatingText(message, position  + offset, color:color);

        //if (armorAmount > 0)
            //CreateFloatingText(String.Format("[s]{0}[/s]", armorAmount), position + offset*1.5f, color:"#888888");
    }

    public async void CreateFloatingText(string message, Vector2 position, int lifetime=1000, int height=200, int fontSize=-1, string color="#111111", float sizeMult=1)
    {
        if (_spawnLocationDelays.ContainsKey(position))
        {
            if (_spawnLocationDelays[position] + 500 > Time.GetTicksMsec())
                await Task.Delay((int)(_spawnLocationDelays[position] + 500 - Time.GetTicksMsec()));
            
            _spawnLocationDelays[position] = Time.GetTicksMsec();
        }
        else
            _spawnLocationDelays.Add(position, Time.GetTicksMsec());

        
        if (fontSize == -1)
            fontSize = _fontSize;

        message = String.Format("[center][font_size={0}][color={1}]{2}[/color][/font_size][/center]", fontSize, color, message);
        GD.Print("Message", message);

        RichTextLabel floatingText;

        if (_waitingQueue.TryPeek(out RichTextLabel label))
        {
            Refresh(label, message, position - new Vector2(300, 75) * sizeMult);
            floatingText = label;
            _waitingQueue.Dequeue();
        }
        else
        {
            RichTextLabel newLabel = new();
            newLabel.Theme = _theme;
            newLabel.Text = message;
            newLabel.SetSize(new Vector2(600, 200) * sizeMult, false);
            newLabel.BbcodeEnabled = true;
            
            newLabel.Position = position - new Vector2(300, 75) * sizeMult;
            floatingText = newLabel;
        }

        AddChild(floatingText);
        TextLifetime(floatingText, lifetime, height);
    }

    private void Refresh(RichTextLabel label, string message, Vector2 position)
    {
        label.Text = message;
        label.Position = position;
        label.Modulate = new Color(1, 1, 1, 1);
    }

    private async void TextLifetime(RichTextLabel label, int wait, int height)
    {
        Tween positionTween = CreateTween();
        Tween alphaTween = CreateTween();
        
        float waitSeconds = wait * 0.001f;

        positionTween.TweenProperty(label, "position:y", label.Position.Y - height, waitSeconds);
        
        alphaTween.TweenProperty(label, "modulate:a", 0, waitSeconds);
        await Task.Delay(wait);
        
        RemoveChild(label);
        _waitingQueue.Enqueue(label);
    }
}
