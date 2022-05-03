using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MyScripts
{
    public class Chase : State
    {
        

        private Transform target;
        private NavMeshAgent agent;
        private float chaseDistance = 10f;
        private float waitTime;
        private float chaseSpeed;
        private bool hasTarget = false;
        private Vector3 lastKnownTargetPos;

        private float timer;
        private float rotationWaitTimer = 0f;
        private bool walkPointSet = false;
        private LayerMask playerMask;
        bool isChasing = false;
        float lengthCheck = 0.5f;
        private float attackRange;
        private float rotationSpeed = 20f;


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
            attackRange = enemyFsm.AttackDistance;
        }

        public override void OnUpdate()
        {
            if(enemyFsm.CurrentState is Chase)
            {
                target = GameManager.Instance.GetPlayerTransform();
            }
            
            CheckTransition();
           
            if(target == null) return;
            
            if (enemyFsm.isInRange(target.transform.position, chaseDistance))
            {
                // check if target is in Range, then for View - after target is set, enemy chase target also in range check, otherwise only view check
                CheckTargetDistance(); 
            }
            else
            {
                // if target is out of range, go to last knwon position
                LastPlayerPosCheck();
                
                // wait on last known position, then go back to start Pos -> back to Patrol state in Check transition
                WaitOnLastPlayerPos();
            }
        }

        private void CheckTargetDistance()
        {
            // after the enemy found a target follow it until it is out range
            if (isChasing)
            {
                agent.SetDestination(target.transform.position);
                return;
            }

            // if the enemy is in FoV and not chasing, start chasing - for the first time if there is no target
            if (enemyFsm.IsInView(target.transform.position))
            {
                agent.SetDestination(target.transform.position);
                hasTarget = true;
                isChasing = true;
            }
        }

        /// <summary>
        /// wait on last known target pos, if waittime is over, go back to start pos
        /// </summary>
        private void WaitOnLastPlayerPos()
        {
            if (!hasTarget && DistanceCheck(lastKnownTargetPos))
            {
                Debug.Log("Distance check true - now waiting...");
                LookoutForPlayer(2f);
                timer += Time.deltaTime;
                if (timer >= waitTime)
                {
                    agent.SetDestination(enemyFsm.StartPoint);
                    timer = 0f;
                    walkPointSet = true;
                }
            }
        }

        private void LastPlayerPosCheck()
        {
            RaycastHit hit;
            Vector3 rayDirection = target.transform.position - enemyFsm.transform.position;
            if (Physics.Raycast(enemyFsm.transform.position, rayDirection, out hit, chaseDistance, playerMask))
            {
                lastKnownTargetPos = hit.point;
                agent.SetDestination(lastKnownTargetPos);
                hasTarget = false;
                isChasing = false;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("Exit Chase State");
            target = null;
        }

        public override void CheckTransition()
        {
            
            if (walkPointSet && DistanceCheck(enemyFsm.StartPoint))
            {
                enemyFsm.ChangeState(new Patrol(enemyFsm, agent, enemyFsm.PatrolSpeed, enemyFsm.PointWaitTime, 120f,
                    enemyFsm.ViewDistance, enemyFsm.ViewAngle, playerMask));
                
            }

            if (enemyFsm.isInRange(target.transform.position, attackRange))
            {
                enemyFsm.ChangeState(new Attack(enemyFsm, agent, target, enemyFsm.AttackSpeed, enemyFsm.AttackDistance, enemyFsm.AttackWaitTime ));
            }
            
        }

        private bool DistanceCheck(Vector3 targetPos)
        {
            Vector3 offset = (enemyFsm.transform.position - targetPos);
            if (offset.magnitude <= lengthCheck)
            {
                Debug.Log("reached roaming pos");
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Rotate on last known target pos in a random angle, to find player
        /// </summary>
        /// <param name="waitTime"></param>
        private void LookoutForPlayer(float waitTime)
        {
            rotationWaitTimer += Time.deltaTime;
            if (rotationWaitTimer >= waitTime)
            {
                Debug.Log("Rotate");
                float randomAngle = Random.Range(enemyFsm.MinAngle, enemyFsm.MaxAngle);
                // TODO: - Rotate on last known target pos in a random angle, to find player
                
                rotationWaitTimer = 0f;
                
                
            }
        }

        private void OnDestroy()
        {
            Destroy(this);
        }
    }
}