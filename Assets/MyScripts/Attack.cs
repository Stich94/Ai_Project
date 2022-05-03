using UnityEngine;
using UnityEngine.AI;

namespace MyScripts
{
    public class Attack : State
    {
        private Transform target;
        private NavMeshAgent agent;
        private float attackSpeed;
        private float attackRange;
        private bool canAttack = false;
        private float attackDelay;
        private float timer;
        
        public Attack(EnemyFSM enemyFsm, NavMeshAgent agent, Transform target, float attackSpeed, float attackRange, float attackDelay) : base(enemyFsm)
        {
            this.agent = agent;
            this.target = target;
            this.attackSpeed = attackSpeed;
            this.attackRange = attackRange;
            this.attackDelay = attackDelay;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Entered Attack State");
            canAttack = true;
            

        }

        public override void OnUpdate()
        {
            target = GameManager.Instance.GetPlayerTransform();
            
            timer += Time.deltaTime;
            
            CheckTransition();
            
        }
        
        public override void OnExit()
        {
            base.OnExit();
            target = null;
        }

        public override void CheckTransition()
        {
            // if in Range attack
            if (enemyFsm.isInRange(target.transform.position, attackRange))
            {
                StartAttack(target);
            }
            // if not in Range change State to chase
            else if (!enemyFsm.isInRange(target.transform.position, attackRange))
            {
                Debug.Log("Change from Attack to Chase State");
                enemyFsm.ChangeState(new Chase(enemyFsm, agent, target.transform, enemyFsm.ChaseDistance,
                    enemyFsm.ChaseWaitTime, enemyFsm.ChaseSpeed, enemyFsm.PlayerMask));
            }

           
        }

        private void StartAttack(Transform target)
        {
            if(timer >= attackDelay)
            {
                timer = 0;
                Debug.Log("is hitting player");
                
            }
        }
    }
}