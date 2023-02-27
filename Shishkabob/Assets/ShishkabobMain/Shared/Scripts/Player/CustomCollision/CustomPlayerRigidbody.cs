using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///A custom rigidbody implementation for the player character
///</summary>
public class CustomPlayerRigidbody : CustomRB
{
    //nice control game plan:
    //If about to collide 
    public delegate void Signal();
    public event Signal OnGrounded;
    public event Signal OnUnGrounded;
    public event Signal OnCollideWall;
    public float walkAngleLimit = 0.6f;
    public ContactFilter2D filter;

    float defaultFriction = 1.05f;

    bool goingToCollideNextFrame;
    private void Awake() 
    {
        defaultFriction = frictionScale;
        this.Initialize();
    }

    private void FixedUpdate() 
    {

        float dt = Time.fixedDeltaTime;
        
        DoBackgroundForces(dt);

        float distance = 0f;
        Vector2 direction = new Vector2();
        int hits = 0;
        if(CheckAboutToCollide(dt,out distance,out direction,out hits))
        {
            MoveCollide(dt,futureColResults[0]);
        }
        else
        {
            MoveNoCollide(dt);
        }
        CheckGrounded();
    }

    public void SetFrictionScaleToPercentDefault(float amt)
    {
        frictionScale = defaultFriction * amt;
    }
    public void ResetFrictionScale()
    {
        SetFrictionScaleToPercentDefault(1);
    }

    bool CheckAboutToCollide(float dt, out float distanceToCheck, out Vector2 direction, out int hits) //Valid, makes sense
    {
        //Vector2 movement = this.velocity * dt;
        //that is what movenocollide does, adds velocity * dt to current position. So, distance is the magnitude of velocity * dt
        distanceToCheck = (this.velocity * dt).magnitude;
        direction = this.velocity.normalized;

        //weed out not environment colliders
        hits = col.Cast(direction,filter,futureColResults,distanceToCheck);
        RaycastHit2D firstHit = futureColResults[0];
        
        //if there were hits and at least one was the environment
        if(hits > 0)
        {
            return true;
        }
        return false;
    }
    Vector2 ResolveAboutToCollideVelocity(RaycastHit2D hit)
    {
        //Projection of U onto V = ((U * V) / V.sqrmag) * V
        //We want to project our current velocity onto the surface we are about to collide with.
        //U = rb.velocity V = hit.normal.right
        Vector2 normalRHS = new Vector2(hit.normal.y,-hit.normal.x);
        float UdotV = Vector2.Dot(this.velocity,normalRHS);
        //divide that by v sqrmag
        float remainder = UdotV / normalRHS.sqrMagnitude;
        return remainder * normalRHS;

    }
    void ResolveCurrentlyInGeometry(float dt, RaycastHit2D hit)
    {
        //This is not a great solution, pretty sure it is the cause of the pushback issue.
        //A better solution is to move the character that is in the wall, out of the wall lol
        int hits = col.Cast(Vector2.zero,futureColResults,0f);
        for(int i = 0; i < hits;i++)
        {
            //Cast a new cast per original hit, slower but more exact, i just want it to work
            if(col.Cast(Vector2.zero,futureColResults,0f) > 0)
            {
                RaycastHit2D curHit = futureColResults[i];
                //get the distance between
                ColliderDistance2D distance = col.Distance(curHit.collider);
                transform.position += (Vector3)curHit.normal * (distance.distance ) * -1f;
            }
        }
        
    }
    Vector2 ResolveCollisionVelocity(RaycastHit2D hit)
    {
        Vector2 resolvedVelocity = ResolveAboutToCollideVelocity(hit);
        return resolvedVelocity;
    }
    Vector2 GetAlignToWalkableSurface(RaycastHit2D[] hits,int hitCount)
    {
        Vector2 averageNormal = new Vector2();
    
        for(int i = 0; i < hitCount; i++)
        {
            if(CheckCollideWalkable(hits[i]))
            {
                averageNormal+=hits[i].normal;
            }
        }
        averageNormal /= hitCount;
    
        
        return averageNormal;
    }
    bool CheckCollideWalkable(RaycastHit2D hit)
    {
        if(!CheckCollideCeiling(hit))
        {
            if(hit.normal.x >= -walkAngleLimit && hit.normal.x <= walkAngleLimit)
            {return true;}
        
            return false;
        }
        return false;
    }
    void CheckGrounded()
    {
        if(col.Cast(Vector2.down,filter,futureColResults,0.1f) > 0)
        {
            if(CheckCollideWalkable(futureColResults[0]))
            {
                OnGrounded?.Invoke();
            }
            else
            {
                OnUnGrounded?.Invoke();
            }
        }
        else
        {
            OnUnGrounded?.Invoke();
        }
    }
    bool CheckCloseToWalkable()
    {
        //something here
        return Physics2D.Raycast(transform.position,Vector2.down,10f);
    }
    bool CheckCollideCeiling(RaycastHit2D hit)
    {
        return hit.normal.y < 0;
    }

    void MoveNoCollide(float dt)
    {
        Vector2 movement = this.velocity * dt;
        transform.position += new Vector3(movement.x,movement.y,0);
    }
    void MoveCollide(float dt, RaycastHit2D hit)
    {
        Vector2 movement = this.velocity * dt;
        Vector2 preCollideDir = movement;
        //move distance to collision
        Vector2 preCollideMovement = preCollideDir * hit.fraction * 0.8f;
        transform.position += new Vector3(preCollideMovement.x, preCollideMovement.y,0);
        //find remaining magnitude and move than much on the new surface, as well as setting our new velocity
        this.velocity = ResolveCollisionVelocity(hit);
        this.velocity/= frictionScale;
        float remainingMagnitude = movement.magnitude - preCollideMovement.magnitude;
        Vector2 postCollideMovement = remainingMagnitude * this.velocity.normalized;
        transform.position += new Vector3(postCollideMovement.x,postCollideMovement.y,0);
        //at this point we moved to the collision, and then changed the velocity due to our collision, and then moved a bit more with that new velocity.
        ResolveCurrentlyInGeometry(dt,hit);
    }
    

    void DoGravity(float dt)
    {
        this.velocity += Vector2.down * dt * gravity * mass;
    }

    void DoDrag(float dt)
    {

    }

    void DoBackgroundForces(float dt)
    {
        DoGravity(dt);
    }


    private void OnDrawGizmos() 
    {
        if(futureColResults.Length > 0)
        {
            Gizmos.color = new Color(.01f,.98f,.01f,0.8f);
            Gizmos.DrawCube(futureColResults[0].centroid,col.bounds.size);
        }
    }
}
