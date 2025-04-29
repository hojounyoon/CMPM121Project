using UnityEngine;
using System.Collections;

public class ArcaneBlast : Spell
{

    public ArcaneBlast(SpellCaster owner) : base(owner)
    {
    }
    private int secondaryDamage;
    private int numProjectiles;

    protected override void InitializeSpell()
    {
        spellName = "Arcane Blast";
        description = "A bolt that explodes into smaller bolts on impact.";
        icon = 2;
        cooldown = 1.5f;
        manaCost = 15;
        baseDamage = 20;
        secondaryDamage = 5;
        numProjectiles = 8;
        projectileTrajectory = "straight";
        projectileSpeed = 12f;
        projectileSprite = 0;
    }

    public override int GetDamage()
    {
        return (int)(baseDamage * (1 + owner.spellPower / 3f));
    }

    public int GetSecondaryDamage()
    {
        return (int)(secondaryDamage * (1 + owner.spellPower / 10f));
    }

    public int GetNumProjectiles()
    {
        return (int)(numProjectiles * (1 + owner.spellPower / 5f));
    }

    protected override void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != team)
        {
            other.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
            
            // Create secondary projectiles
            float angleStep = 360f / GetNumProjectiles();
            for (int i = 0; i < GetNumProjectiles(); i++)
            {
                float angle = i * angleStep;
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                
                GameManager.Instance.projectileManager.CreateProjectile(
                    projectileSprite,
                    "straight",
                    impact,
                    direction,
                    20f,
                    (h, pos) => {
                        if (h.team != team)
                        {
                            h.Damage(new Damage(GetSecondaryDamage(), Damage.Type.ARCANE));
                        }
                    },
                    0.1f
                );
            }
        }
    }
} 