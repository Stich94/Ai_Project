using System;
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
        private bool hasTarget = false;
        private Vector3 lastKnownTargetPos;

        private float timer;
        private bool walkPointSet = false;
        private LayerMask playerMask;
        bool isChasing = false;
        float lengthCheck = 0.5f;


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

        private void WaitOnLastPlayerPos()
        {
            if (!hasTarget && DistanceCheck(lastKnownTargetPos))
            {
                Debug.Log("Distance check true - now waiting...");
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

        private void OnDestroy()
        {
            Destroy(this);
        }
    }
}