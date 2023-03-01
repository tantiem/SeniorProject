using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFightingInterface : MonoBehaviour
{
    public HitBox attackBox;
    public AttackData slashAttackData;
    public AttackData stabAttackData;

    public void GenerateSlash(Vector2 position,Vector2 rightAlign,Vector2 speed)
    {
        GenerateAttack(slashAttackData,position,rightAlign,speed);
    }

    public void GenerateStab(Vector2 position, Vector2 rightAlign,Vector2 speed)
    {
        GenerateAttack(stabAttackData,position,rightAlign,speed);
    }

    public void GenerateLowStab(Vector2 position, Vector2 rightAlign,Vector2 speed)
    {
        GenerateAttack(stabAttackData,position-Vector2.up,rightAlign,speed);
    }

    public void GenerateLowSlash(Vector2 position, Vector2 rightAlign,Vector2 speed)
    {
        GenerateAttack(slashAttackData,position-Vector2.up,rightAlign,speed);
    }

    void GenerateAttack(AttackData data, Vector2 pos, Vector2 rightAlign, Vector2 speed)
    {
        attackBox.Generate(data,pos,rightAlign,speed);
    }

    
}
[System.Serializable]
public class AttackData
{
    public Vector2 attackSize;
    public float attackTime;
    public float attackDamage;

    public float preTime;
    public float postTime;

    public AttackData()
    {
        attackDamage = 1f;
        attackTime = 1f;
        attackSize = Vector2.one;
        preTime = .1f;
        postTime = .3f;
    }
}
