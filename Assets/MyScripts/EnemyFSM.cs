using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace MyScripts
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyFSM : MonoBehaviour
    {
        [Header("General")] [SerializeField] private float speed;
        private State currentState;
        private NavMeshAgent agent;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField, Range(15f, 30f)] private float playerCalcDistance;
        [SerializeField] private float angularSpeed = 120f;
        
        private Collider[] player;
        

        [Header("FoV")] [SerializeField] private float viewDistance = 6f;
        [SerializeField] private float viewAngle = 60f;
        public float ViewDistance => viewDistance;

        
        [Header("Patrol")] [SerializeField] private float patrolSpeed = 3.5f;
        [SerializeField] private float pointWaitTime = 6f;
        [SerializeField] private Vector3 startPoint;
        public float PatrolSpeed => patrolSpeed;
        public float PointWaitTime => pointWaitTime;
        public Vector3 StartPoint => startPoint;
        
        
        [Header("Chase")] [SerializeField] private float chaseSpeed;
        [SerializeField] private float chaseWaitTime = 6f;
        [SerializeField] private float chaseDistance = 20f;
        public float ChaseDistance => chaseDistance;
        public float ChaseWaitTime => chaseWaitTime;
        public float ChaseSpeed => chaseSpeed;
        
        private GameObject target = null;

        
        // TODO: - Add Boid
        public static List<EnemyFSM> enemies = new List<EnemyFSM>();


        


        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            startPoint = transform.position;
            // currentState = new Idle(this, agent, 20f, 5f, 3f);
            currentState = new Patrol(this,agent,patrolSpeed, pointWaitTime, angularSpeed, viewDistance, viewAngle, playerMask);
        }

        private void Update()
        {
            currentState.OnUpdate();
        }
        
        public void ChangeState(State newState)
        {
            currentState.OnExit();
            currentState = newState;
            currentState.OnEnter();
        }

        public GameObject GetObject()
        {
            return this.gameObject;
        }


        private bool isInRange(Vector3 position)
        {
            return Vector3.Distance(transform.position, position) <= viewDistance;
        }
        
        /// <summary>
        /// Check if other object is in range with distance
        /// </summary>
        /// <param name="other_position"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public bool isInRange(Vector3 otherPos, float distance)
        {
            return Vector3.Distance(this.transform.position, otherPos) <= distance;
        }
        
        
        public bool IsInView(Vector3 position)
        {
            if (!isInRange(position)) return false;
            
            // get the direction towards target
            Vector3 direction = (position - transform.position).normalized;
            // get the angle between forward and direction
            float angletemp = Vector3.Angle(transform.forward, direction);
            return angletemp < viewAngle / 2;
        }

        public bool PlayerDistanceRangeCheck(Vector3 enemyPosition) 
        {
             player = Physics.OverlapSphere(enemyPosition, playerCalcDistance, playerMask);
             
             if(player != null && player.Length > 0)
             {
                 if (CalculateDistanceToPlayer(player[0].transform.position, playerCalcDistance))
                 {
                    this.GetComponent<MeshRenderer>().material.color = Color.magenta;
                    target = player[0].gameObject;
                    return true;

                 }
                 this.GetComponent<MeshRenderer>().material.color = Color.red;
                 return false;
             }
             return false;
        }

        // TODO: change calc from viewDistance to render distance
        private bool CalculateDistanceToPlayer(Vector3 position, float getDistance)
        {
            float distance = Vector3.Distance(position, this.transform.position);
            if (distance < getDistance)
            {
                return true;
            }

            return false;
        }


        public Vector3 GetPos()
        {
            return this.transform.position;
        }


        private void OnDrawGizmosSelected()
        {
            // Gizmos.color = Color.green;
            // Gizmos.DrawWireSphere(transform.position, 5f);
            
            
        }
        
        private void OnDrawGizmos()
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, viewDistance);
                if (GameManager.Instance == null) return;
                
                
                Vector3 pos = transform.position;
                Vector3 forward = transform.forward;
                
                Gizmos.color = IsInView(GameManager.instance.GetPlayerPos()) ? Color.green : Color.red; // visual for enemy check
        
                // Gizmos.color = Color.green;
                Gizmos.DrawRay(pos, transform.forward * viewDistance); // hat ne standard länge von 1

                Vector3 right = Quaternion.AngleAxis(viewAngle / 2, transform.up) * transform.forward;
                Vector3 left = Quaternion.AngleAxis(-viewAngle / 2, transform.up) * transform.forward;
        
                Gizmos.DrawRay(pos, right * viewDistance);
                Gizmos.DrawRay(pos, left * viewDistance);
                
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.position, playerCalcDistance);
                
                
            }
            
            
    }
}