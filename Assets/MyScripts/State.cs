using System.Collections;
using System.Collections.Generic;
using MyScripts;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected EnemyFSM enemyFsm;
    
    // constructor
    public State(EnemyFSM enemyFsm)
    {
        this.enemyFsm = enemyFsm;
    }

    public virtual void OnEnter()
    {
        
    }
    
    public abstract void OnUpdate();

    public virtual void OnExit()
    {
        
    }

    public abstract void CheckTransition();
}
