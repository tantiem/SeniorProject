using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFightingInterface : MonoBehaviour
{
    public HitBox attackBox;
    public AttackData slashAttackData;
    public AttackData stabAttackData;

    public void GenerateSlash(Vector2 position,float speed,Vector2 rightAlign)
    {
        GenerateAttack(slashAttackData,position,speed,rightAlign);
    }

    public void GenerateStab(Vector2 position, float speed,Vector2 rightAlign)
    {
        GenerateAttack(stabAttackData,position,speed,rightAlign);
    }

    public void GenerateLowStab(Vector2 position, float speed,Vector2 rightAlign)
    {
        GenerateAttack(stabAttackData,position-Vector2.up,speed,rightAlign);
    }

    public void GenerateLowSlash(Vector2 position, float speed,Vector2 rightAlign)
    {
        GenerateAttack(slashAttackData,position-Vector2.up,speed,rightAlign);
    }

    void GenerateAttack(AttackData data, Vector2 pos, float speed, Vector2 rightAlign)
    {
        attackBox.Generate(data.attackSize,data.attackDamage,pos,data.attackTime,rightAlign);
    }

    
}
[System.Serializable]
public class AttackData
{
    public Vector2 attackSize;
    public float attackTime;
    public float attackDamage;

    public AttackData()
    {
        attackDamage = 1f;
        attackTime = 1f;
        attackSize = Vector2.one;
    }
}
