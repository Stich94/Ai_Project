using UnityEngine;
using UnityEngine.AI;

namespace MyScripts
{
    public class Chase : State
    {
        public Chase(EnemyFSM enemyFsm) : base(enemyFsm)
        {
        }
        private GameObject target = null;
        private EnemyFSM enemyFSM;
        private NavMeshAgent agent;
        
        public Chase(EnemyFSM enemyFsm, NavMeshAgent agent, GameObject target) : base(enemyFsm)
        {
            this.enemyFsm = enemyFsm;
            this.agent = agent;
            this.target = target;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (target != null)
            {
                agent.SetDestination(target.transform.position);
            }
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