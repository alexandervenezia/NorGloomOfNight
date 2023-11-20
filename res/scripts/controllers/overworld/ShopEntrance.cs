using Data;
using Godot;
using Overworld;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShopEntrance : Area2D
{
    [Export] private Godot.Collections.Array<CardData> _startingStock;
    [Export] public bool Enabled;
    [Export] private string _shopUID;
    private ILevel _level;
    private bool _playerInside;

    private static Dictionary<string, List<CardData>> _stock;

    public override void _Ready()
    {
        _level = GetOwnerOrNull<ILevel>();
        _playerInside = false;

        if (_stock == null)
        {
            _stock = new();
            if (!_stock.ContainsKey(_shopUID))
            {
                List<CardData> cardList = new();
                foreach (CardData c in _startingStock)
                    cardList.Add(c);
                _stock.Add(_shopUID, cardList);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (!Enabled)
        {
            Visible = false;         
            return;   
        }
        Visible = true;
        if (_playerInside)
        {
            if (Input.IsActionJustPressed("ui_interact"))
            {
                Shop shop = (Shop)_level.UseElevator(_shopUID);
                shop.SetStock(_stock[_shopUID]);
                shop.CardBought += OnCardBought;
            }
        }
    }

    private void OnCardBought(CardData card)
    {
        _stock[_shopUID].Remove(card);
    }

    public void Enter()
    {
        _playerInside = true;
            
    }

    public void Exit()
    {
        _playerInside = false;
    }
}
