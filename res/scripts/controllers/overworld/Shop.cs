namespace Overworld;

using Combat;
using Data;
using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


public enum ShopState
{
    PURCHASING,
    MODIFYING
}

public partial class Shop : Node2D
{
    private const int MAX_PER_ROW = 4;
    private ColorRect _rect;
    private Node2D _cardNode;
    private Node2D _priceNode;
    private LevelSelectOption _modifyDeckButton;
    [Export] private PackedScene _cardResource;
    [Export] private PackedScene _coinResource;
    private List<CardData> _stock;
    private Card _selectedCard;
    private ShopState _state;
    public delegate void CardBoughtInformer(CardData card);
    public event CardBoughtInformer CardBought;

    private int _storedZIndex;

    public override void _Ready()
    {
        _rect = GetNode<ColorRect>("ColorRect");
        _cardNode = _rect.GetNode<Node2D>("Cards");
        _priceNode = _rect.GetNode<Node2D>("Prices");
        _modifyDeckButton = GetNode<LevelSelectOption>("ModifyDeck");
        _modifyDeckButton.OnButtonClicked += OnModifyCardButtonPressed;
        _state = ShopState.PURCHASING;
    }

    private void OnModifyCardButtonPressed()
    {
        if (_state == ShopState.PURCHASING)
        {
            _state = ShopState.MODIFYING;
            _modifyDeckButton.GetNode<RichTextLabel>("RichTextLabel").Text = "[center][font_size=50]View Stock";
        }
        else
        {
            _state = ShopState.PURCHASING;
            _modifyDeckButton.GetNode<RichTextLabel>("RichTextLabel").Text = "[center][font_size=50]Modify Deck";
        }
        UpdateSelection();
    }

    public void SetStock(List<CardData> cards)
    {
        _stock = new();
        foreach (CardData c in cards)
        {
            _stock.Add(c);
        }
        UpdateSelection();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 mousePos = GetGlobalMousePosition();
        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        PhysicsPointQueryParameters2D query = new();
        query.CollideWithAreas = true;
        query.Position = mousePos;
        query.CollisionMask = 0b0010;
        var hits = spaceState.IntersectPoint(query);

        Card nextCard = null;
        Card highestZ = null;

        if (_selectedCard != null && IsInstanceValid(_selectedCard))
        {
            _selectedCard.ZIndex = _storedZIndex;
        }

        _selectedCard = null;
        foreach (var h in hits)
        {
            //nextCard = h["collider"] as Card;
            if (h["collider"].Obj is not Card)
            {
                highestZ = null;
                break;
            }
            nextCard = (Card)(h["collider"]);
  
            if (highestZ == null || nextCard.ZIndex > highestZ.ZIndex)
                highestZ = nextCard;
        }

        _selectedCard = highestZ;

        if (_selectedCard != null)
        {
            _storedZIndex = _selectedCard.ZIndex;
            _selectedCard.ZIndex = 91;
        }

        if (Input.IsActionJustReleased("Select"))
        {
            if (_selectedCard != null)
            {                
                if (_state == ShopState.PURCHASING)
                {
                    _stock.Remove(_selectedCard.Data);
                    CardBought?.Invoke(_selectedCard.Data);
                    BuyCard(_selectedCard.Data);
                    UpdateSelection();
                    //_selectedCard = null;
                }
                else if (_state == ShopState.MODIFYING)
                {
                    // TODO:
                }
            }
        }

        UpdateCardProtrusion();
    }

    private void BuyCard(CardData card)
    {
        MasterDeck.PlayerDeck.AddCard(card);
    }

    private void UpdateCardProtrusion()
    {
        foreach (Card c in _cardNode.GetChildren())
        {
            c.SetMouseOverStatus(c == _selectedCard);
            if (c != _selectedCard)
            {
                c.SetMouseOverStatus(false);
            }

            c.Scale = new Vector2(0.4f, 0.4f) * ((400f+c.Offset) / 475f);
        }
    }

    private void UpdateSelection()
    {      
        foreach (var child in _cardNode.GetChildren())
            child.QueueFree();
        foreach (var child in _priceNode.GetChildren())
            child.QueueFree();
        
        int x = 0, y = 0;
        float width = _rect.Size.X;
        float height = _rect.Size.Y;
        int index = 0;

        List<CardData> active = _stock;
        if (_state == ShopState.MODIFYING)
            active = MasterDeck.PlayerDeck.GetCards();

        foreach (CardData c in active)
        {                        
            x = (int)((0.5 + index%Math.Min(active.Count(), MAX_PER_ROW)) * (width / Math.Min(active.Count(), MAX_PER_ROW)));
            y = 200 + (int)( (1 + index/MAX_PER_ROW) * (height / 5f) );    

            Card card = (Card)_cardResource.Instantiate();
            card.UpdateData(c);
            card.ZIndex = (1 + index/MAX_PER_ROW);

            if (_state == ShopState.PURCHASING)
            {
                Node2D price = (Node2D)_coinResource.Instantiate();
                _priceNode.AddChild(price);
                price.Position = new Vector2(x, y + 75);
                price.GlobalScale = Vector2.One * 0.5f;
            }

            _cardNode.AddChild(card);
            card.Position = new Vector2(x, y);
            card.ApplyScale(new Vector2(0.25f, 0.25f));

            index++;
        }
    }
}
