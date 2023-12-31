using Combat;
using Data;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class HealthBar : Sprite2D
{
	private Combatant _parent;

	private RichTextLabel buffs;
	private RichTextLabel debuffs;
	private float _currentHealthTrack;

	[Export] private bool _overworld;

	public override void _Ready()
	{
		if (!_overworld)
			_parent = (Combatant)GetParent().GetParent();

		buffs = (RichTextLabel)GetNode("../Buffs/RichTextLabel");
		debuffs = (RichTextLabel)GetNode("../Debuffs/RichTextLabel");
		_currentHealthTrack = -1;
	}
	public override void _Process(double delta)
	{
		float cHealth = 0;
		float mHealth = 0;

		if (_overworld)
		{
			cHealth = MasterScene.GetInstance().GetActiveScene<ILevel>().GetPlayer().CurrentHealth;
			mHealth = MasterScene.GetInstance().GetActiveScene<ILevel>().GetPlayer().MaxHealth;
		}
		else
		{
			cHealth = (float)_parent.GetHealth();
			mHealth = (float)_parent.MaxHealth;
		}

		if (_currentHealthTrack < 0)
			_currentHealthTrack = cHealth / mHealth;

		float currentHealth = cHealth / mHealth;

		if (Mathf.Abs(currentHealth-_currentHealthTrack) < 0.01f)
		{
			_currentHealthTrack = currentHealth;
		}
		else if (_currentHealthTrack < currentHealth)
			_currentHealthTrack += (float)delta * 0.5f;
		else if (_currentHealthTrack > currentHealth)
			_currentHealthTrack -= (float)delta * 0.5f;

		float drain = Math.Max(_currentHealthTrack-currentHealth, 0);
		//drain = 0f;

		ShaderMaterial mat = (ShaderMaterial)Material;
		mat.SetShaderParameter("health_factor", currentHealth);
		mat.SetShaderParameter("draining_factor", drain);

		if (_overworld)
			return;

		string text = "[right][color=#22BB22]";

		foreach (BuffType b in _parent.Buffs.Keys)
		{
			if (_parent.Buffs[b] > 0)
			{
				text += Data.EnumStringWrapper.BuffToString(b) + " : " + _parent.Buffs[b] + "\n";
			}
		}

		Dictionary<DamageType, int> resistances = _parent.GetResistances();
		foreach (DamageType resistance in resistances.Keys)
		{
			if (resistances[resistance] >= 999) // permanent resistance
			{
				text += "Resist: " + Data.EnumStringWrapper.DamageToString(resistance) + "\n";
			}
			else if (resistances[resistance] > 0)
			{
				text += "Resist: " + Data.EnumStringWrapper.DamageToString(resistance) + 
					" : " + resistances[resistance] + "\n";
			}
		}
		text += "[/color]";
		buffs.Text = text;

		text = "[left][color=#BB2222]";

		foreach (DebuffType d in _parent.Debuffs.Keys)
		{
			if (_parent.Debuffs[d] > 0)
				text += Data.EnumStringWrapper.DebuffToString(d) + " : " + _parent.Debuffs[d] + "\n";
		}
		text += "[/color]";
		debuffs.Text = text;

	}
}
