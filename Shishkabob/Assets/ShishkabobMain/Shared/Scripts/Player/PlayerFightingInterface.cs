using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFightingInterface : MonoBehaviour
{
    public HitBox attackBox;
    public AttackData slashAttackData;

    public void GenerateSlash(Vector2 position,float speed)
    {
        GenerateAttack(slashAttackData,position,speed);
    }

    void GenerateAttack(AttackData data, Vector2 pos, float speed)
    {
        attackBox.Generate(data.attackSize,data.attackDamage,pos,data.attackTime);
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
