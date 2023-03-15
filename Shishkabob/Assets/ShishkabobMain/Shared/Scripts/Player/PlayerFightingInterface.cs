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
        GenerateAttack(stabAttackData,position,rightAlign,speed,true);
    }

    public void GenerateLowStab(Vector2 position, Vector2 rightAlign,Vector2 speed)
    {
        GenerateAttack(lowStabAttackData,position,rightAlign,speed,true);
    }

    public void GenerateLowSlash(Vector2 position, Vector2 rightAlign,Vector2 speed)
    {
        GenerateAttack(lowSlashAttackData,position,rightAlign,speed);
    }

    void GenerateAttack(AttackData data, Vector2 pos, Vector2 rightAlign, Vector2 speed, bool isStab = false)
    {
        Vector2 alignedPos = AlignAttackDataPosToAim(rightAlign,data);
        
        attackBox.Generate(data,pos + alignedPos,rightAlign,speed,isStab);
        
    }
    /// <summary>
    /// Attack data position is an offset based on the right. Use this to convert it to whatever aimed direction.
    /// </summary>
    /// <param name="rightAlign"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    Vector2 AlignAttackDataPosToAim(Vector2 rightAlign, AttackData data)
    {
        if(rightAlign.x > 0)
        {
            return new Vector2(data.pos.x, data.pos.y);
        }
        else if(rightAlign.x < 0)
        {
            return new Vector2(-data.pos.x, data.pos.y);
        }
        else if(rightAlign.y > 0)
        {
            return new Vector2(0,data.pos.x);
        }
        else
        {
            return new Vector2(0, -data.pos.x);
        }
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
