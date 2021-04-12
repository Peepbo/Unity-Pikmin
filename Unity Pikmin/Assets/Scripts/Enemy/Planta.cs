using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planta : EnemyManager
{
    [Header("Bee Settings")]
    public int prefabIndex;

    private void Attack()
    {
        throw new NotImplementedException();
    }

    protected override void Animation()
    {
        switch (state)
        {
            case EnemyState.STAY:
                anim.SetInteger("animation", 1);
                base.Stay();
                
                break;
            case EnemyState.MOVE:
                anim.SetInteger("animation", 2);
                base.Move();

                break;
            case EnemyState.ATTACK:
                anim.SetInteger("animation", 3);
                Attack();

                break;
        }
    }
}