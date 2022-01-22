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
