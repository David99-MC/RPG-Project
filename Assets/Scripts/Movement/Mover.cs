using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 5.66f;
        [SerializeField] float maxPathLength = 40f;

        NavMeshAgent navMesh;
        Animator animator;
        ActionScheduler actionScheduler;
        Health health;

        // Start is called before the first frame update
        void Awake()
        {
            navMesh = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            navMesh.enabled = !health.IsDead(); // disable collider when dead

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction) {
            actionScheduler.StartAction(this); //this class
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMesh.destination = destination; // where the movement happens
            navMesh.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMesh.isStopped = false;
        }

        public bool CanMoveTo(Vector3 destination) {
            NavMeshPath path = new NavMeshPath(); // *
            // the line below will store info into the path* variable
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false; //can't calculate path
            if (path.status != NavMeshPathStatus.PathComplete) return false; // can't complete the path
            if (GetPathLength(path) > maxPathLength) return false; // limit how far the NavMeshAgent can cover;
            return true;

        }

        public void Cancel() {
            navMesh.isStopped = true;
        }

        void UpdateAnimator()
        {
            Vector3 velocity = navMesh.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            animator.SetFloat("forwardSpeed", speed);
        }

        float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length <= 2) return 0;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state) // can store multiple variables using a Struct (preferred) 
                                               // or a Dictionary
        {
            SerializableVector3 position = (SerializableVector3)state;
            navMesh.enabled = false;
            transform.position = position.ToVector();
            navMesh.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
