using System;
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

        [Header("FoV")] [SerializeField] private float viewDistance = 6f;
        [SerializeField] private float viewAngle = 60f;
        
        [Header("Idle")] [SerializeField] private float rotationSpeed;
        
        
        [Header("Patrol")] [SerializeField] private float patrolSpeed;
        
        
        

        public float Speed => speed;


        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            // currentState = new Idle(this, agent, 20f, 5f, 3f);
            currentState = new Patrol(this, agent, 4f, 2f, 5f, 90f, playerMask, obstacleMask);
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

        private void FoV()
        {
            
        }

        private bool isInRange(Vector3 position)
        {
            return Vector3.Distance(transform.position, position) <= viewDistance;
        }


        private bool IsInView(Vector3 position)
        {
            if (!isInRange(position)) return false;
            
            // get the direction towards target
            Vector3 direction = (position - transform.position).normalized;
            // get the angle between forward and direction
            float angletemp = Vector3.Angle(transform.forward, direction);
            return angletemp < viewAngle / 2;
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
            }
    }
}