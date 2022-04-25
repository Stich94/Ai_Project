using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


namespace MyScripts
{
    public class Patrol : State
    {
        private EnemyFSM enemyFsm;
        private NavMeshAgent agent;
        private Vector3 walkPoint;
        private float pointWaitTime;
        private float rotationTime;
        private float viewAngle;
        private  float viewRadius;
        private LayerMask playerMask;
        private LayerMask obstacleMask;
        private bool walkPointSet = false;
        private float walkPointRange = 10f;
        private GameObject target;

        private bool playerIsInView;
        private bool playerIsInRange;
        private float currentWaitTime = 0f;

        private Vector3 playerPos;
        Vector3 distanceToWalkPoint;
        private Vector3 startingPos;


        private void Start()
        {
            startingPos = enemyFsm.transform.position;
        }

        public Patrol(EnemyFSM enemyFsm, NavMeshAgent agent, float pointWaitTime, float rotationTime, float viewRadius,
            float viewAngle, LayerMask playerMask, LayerMask obstacleMask) : base(enemyFsm)
        {
            this.enemyFsm = enemyFsm;
            this.agent = agent;
            this.pointWaitTime = pointWaitTime;
            this.rotationTime = rotationTime;
            this.viewRadius = viewRadius;
            this.viewAngle = viewAngle;
            this.playerMask = playerMask;
            this.obstacleMask = obstacleMask;
        }

        public override void OnUpdate()
        {
            currentWaitTime += Time.deltaTime;
            // playerPos = GameManager.Instance.GetPlayerPos();

            CheckTransition();
            Roaming();
          
            // float distance = Vector3.Distance(playerPos, enemyFsm.transform.position);
            // if (distance < viewRadius)
            // {
            //     Debug.Log("Player is in ViewRange");
            //     enemyFsm.transform.LookAt(playerPos);
            // }

            // Debug.Log("dist :" + distanceToWalkPoint.magnitude);
        }

        private bool WaitOnPosition(float time)
        {
            if (distanceToWalkPoint.magnitude <= 1f)
            {
                currentWaitTime = 0f;
                walkPointSet = false;
                return true;
            }

            return time >= pointWaitTime;
        }

        private void Roaming()
        {
            if (!WaitOnPosition(currentWaitTime)) return;

            if (!walkPointSet)
            {
                // walkPoint = enemyFsm.transform.position + Random.insideUnitSphere * walkPointRange;
                SearchWayPoint();

                Instantiate(new GameObject("Destination"), walkPoint, Quaternion.identity);
                
            }
            
            distanceToWalkPoint = enemyFsm.transform.position - walkPoint;

            if (walkPointSet)
                agent.SetDestination(walkPoint);

            if(distanceToWalkPoint.magnitude <= 1f)
                walkPointSet = false;
            

        }

        private Vector3 GetRandomDir()
        {
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            walkPoint = new Vector3(enemyFsm.transform.position.x + randomX, enemyFsm.transform.position.y,
                enemyFsm.transform.position.z + randomZ).normalized;
            return walkPoint;
        }

        private void SearchWayPoint()
        {
            float randomX = Random.Range(-walkPointRange, walkPointRange);
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            walkPoint = new Vector3(enemyFsm.transform.position.x + randomX, enemyFsm.transform.position.y,
                enemyFsm.transform.position.z + randomZ);
            
            walkPointSet = true;
        }

        private Vector3 GetRoamingPosition()
        {
            return startingPos + GetRandomDir() * Random.Range(10f, 70f);
        }

        public override void CheckTransition()
        {

            if (enemyFsm.IsInCalculatePlayerPosRange(enemyFsm.transform.position))
            {
                playerPos = GameManager.Instance.GetPlayerPos();
            }
            
            
        }

        
        
    }
}