using UnityEngine;
using UnityEngine.AI;

namespace MyScripts
{
    public class Attack : State
    {
        private GameObject target;
        private EnemyFSM enemyFSM;
        private NavMeshAgent agent;
        
        public Attack(EnemyFSM enemyFsm, GameObject target) : base(enemyFsm)
        {
            this.enemyFsm = enemyFsm;
            this.target = target;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
        }

        public override void OnUpdate()
        {
            // move towards target
            
            // if target is out of range or out of FoV, go to last target pos
            
            // wait there for a time
            
            // go back to roaming pos
        }
        
        public override void OnExit()
        {
            base.OnExit();
            target = null;
        }

        public override void CheckTransition()
        {
            
        }
    }
}