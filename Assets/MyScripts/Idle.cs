using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MyScripts
{
    public class Idle : State
    {
        private float rotationSpeed;
        private float walkpointRange;
        float waitTime;
        private Vector3 walkPoint;
        private NavMeshAgent agent;
        private float currentWaitTime = 0f;
        private bool walkPointSet = false;

        public Idle(EnemyFSM enemyFsm, NavMeshAgent agent, float rotationSpeed, float walkpointRange, float waitTime) : base(enemyFsm)
        {
            this.agent = agent;
            this.rotationSpeed = rotationSpeed;
            this.walkpointRange = walkpointRange;
            this.waitTime = waitTime;
        }

        public override void OnUpdate()
        {
            currentWaitTime += Time.deltaTime;
            if (currentWaitTime > waitTime)
            {
                SearchWalkPoint();
            }

            Debug.Log("current secs: " + currentWaitTime);
            
        }

        public override void CheckTransition()
        {
            
        }

        private void SearchWalkPoint()
        {
            float randomZ = Random.Range(-walkpointRange, walkpointRange);
            float randomX = Random.Range(-walkpointRange, walkpointRange);
            
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
            agent.SetDestination(walkPoint);
            currentWaitTime = 0f;

        }

        IEnumerator IWait()
        {
            yield return new WaitForSeconds(waitTime);
            SearchWalkPoint();
        }
    }
}