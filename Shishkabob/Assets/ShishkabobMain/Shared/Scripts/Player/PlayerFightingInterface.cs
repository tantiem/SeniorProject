using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFightingInterface : MonoBehaviour
{
    public HitBox attackBox;
    public AttackData slashAttackData;
    public AttackData stabAttackData;
    public AttackData lowSlashAttackData;
    public AttackData lowStabAttackData;

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
        GenerateAttack(lowStabAttackData,position,rightAlign,speed);
    }

    public void GenerateLowSlash(Vector2 position, Vector2 rightAlign,Vector2 speed)
    {
        GenerateAttack(lowSlashAttackData,position,rightAlign,speed);
    }

    void GenerateAttack(AttackData data, Vector2 pos, Vector2 rightAlign, Vector2 speed)
    {
        Vector2 alignedPos = Vector2.one;
        if(rightAlign.x != 0)
        {
            //align on x
            alignedPos = new Vector2(rightAlign.x,alignedPos.y);
        }
        if(rightAlign.y != 0)
        {
            alignedPos = new Vector2(alignedPos.x, rightAlign.y);
        }
        alignedPos *= data.pos; // align data input pos to the aim direction
        attackBox.Generate(data,pos + alignedPos,rightAlign,speed);
        
    }

    
}
[System.Serializable]
public class AttackData
{
    public Vector2 pos;
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
