using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, ICharacter
{
    public MovementController MovementController;
    public RockBendingController RockBendingController;
    public int Health {get;set;} = 100;
    public bool IsAlive {get;set;} = true;

    public void ApplyDamage(int damage)
    {
        Health -= damage;
        if(Health <= 0 && IsAlive)
            Die();
        else
            Debug.Log("Health: " + Health);
    }
    
    virtual public void Die(){}
}