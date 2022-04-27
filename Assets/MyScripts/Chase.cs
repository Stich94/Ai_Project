using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace MyScripts
{
    public class Chase : State
    {
        public Chase(EnemyFSM enemyFsm) : base(enemyFsm)
        {
        }

        private Transform target;
        private NavMeshAgent agent;
        private float chaseDistance = 10f;
        private float waitTime;
        private float chaseSpeed;
        private Transform roamingStartingPos;
        private bool hasTarget = false;
        private Vector3 lastKnownTargetPos;

        private float timer;
        private Vector3 distanceToRoamingPos;
        private bool walkPointSet = false;
        private LayerMask playerMask;


        public Chase(EnemyFSM enemyFsm, NavMeshAgent agent, Transform target, float chaseDistance, float waitTime, float chaseSpeed, LayerMask playerMask) : base(enemyFsm)
        {
            this.agent = agent;
            this.target = target;
            this.chaseDistance = chaseDistance;
            this.waitTime = waitTime;
            this.chaseSpeed = chaseSpeed;
            this.playerMask = playerMask;
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            agent.speed = chaseSpeed;
           
        }

        public override void OnUpdate()
        {
            target = GameManager.Instance.GetPlayerTransform();
            CheckTransition();
            
            // if (target == null) return;
            
            // if(!hasTarget) // TODO: if no target return to pos

            
            
            // move towards target



            if (enemyFsm.isInRange(target.transform.position, chaseDistance))
            {
                // if (!agent.hasPath && !agent.pathPending)
                // {
                //     if (!agent.SetDestination(target.transform.position))
                //     {
                //         agent.SetDestination(target.transform.position);
                //     }
                // }
                agent.SetDestination(target.transform.position);
                hasTarget = true;
            }
            else
            {
                RaycastHit hit;
                Vector3 rayDirection = target.transform.position - enemyFsm.transform.position;
                if (Physics.Raycast(enemyFsm.transform.position, rayDirection, out hit, chaseDistance + 3f, playerMask))
                {
                    lastKnownTargetPos = hit.point;
                    agent.SetDestination(lastKnownTargetPos);
                    hasTarget = false;
                    
                    
                }

                if (!hasTarget && DistanceCheck(lastKnownTargetPos))
                {
                    Debug.Log("Distance check true - now waiting...");
                    timer += Time.deltaTime;
                    if (timer >= waitTime)
                    {
                        agent.SetDestination(enemyFsm.StartPoint);
                    }

                }
                
                
            }
            
            

           
        }
        
        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("Exit Chase State");
            
        }

        public override void CheckTransition()
        {
            
        }

        private IEnumerator LookOutForPlayer()
        {
            
            yield return new WaitForSeconds(6f);
            Debug.Log("Finished Routine");
            agent.SetDestination(enemyFsm.StartPoint);
        }
        
        private bool DistanceCheck(Vector3 targetPos)
        {
            // agent.SetDestination(enemyFsm.StartPoint);
            Vector3 offset = (enemyFsm.transform.position - targetPos);
            float length = offset.sqrMagnitude;
            float lengthCheck = 0.5f;

            if (offset.magnitude <= lengthCheck)
            {
                Debug.Log("reached roaming pos");
                return true;
            }

             return false;
        }
        
    }
}