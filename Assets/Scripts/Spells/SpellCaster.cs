using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpellCaster 
{
    public int mana;
    public int max_mana;
    public int mana_reg;
    public int spellPower;
    public Hittable.Team team;
    public Spell spell;
    public Spell spell2;
    public Spell spell3;
    public Spell spell4;

    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            mana += mana_reg;
            mana = Mathf.Min(mana, max_mana);
            yield return new WaitForSeconds(1);
        }
    }

    public SpellCaster(int mana, int mana_reg, Hittable.Team team, int spellPower = 0)
    {
        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.team = team;
        this.spellPower = spellPower;
        spell = new SpellBuilder().Build(this, "arcane_bolt");
    }

    public IEnumerator Cast(Vector3 where, Vector3 target)
    {        
        if (mana >= spell.GetManaCost() && spell.IsReady())
        {
            mana -= spell.GetManaCost();
            yield return spell.Cast(where, target, team);
        }
        if (mana >= spell2.GetManaCost() && spell2.IsReady())
        {
            mana -= spell2.GetManaCost();
            yield return spell2.Cast(where, target, team);
        }
        if (mana >= spell3.GetManaCost() && spell3.IsReady())
        {
            mana -= spell3.GetManaCost();
            yield return spell3.Cast(where, target, team);
        }
        if (mana >= spell4.GetManaCost() && spell4.IsReady())
        {
            mana -= spell4.GetManaCost();
            yield return spell4.Cast(where, target, team);
        }
        yield break;
    }

}
