using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public int Health {get;set;}
    public bool IsAlive {get;set;}

    public void ApplyDamage(int damage);
    public void Die();
}