using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Control;
using RPG.Attributes;
using RPG.Movement;
using UnityEngine.AI;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weaponConfig = null;
        [SerializeField] float healthToRestore = 0f;
        [SerializeField] float hideTime = 5f;
        [SerializeField] float interactRadius = .5f;

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                PickUp(other.gameObject);
            }
        }

        private void PickUp(GameObject player)
        {
            if (weaponConfig != null) {
                player.GetComponent<Fighter>().EquipWeapon(weaponConfig);
            }
            else {
                player.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(hideTime));
        }

        IEnumerator HideForSeconds(float seconds) {
            ShowPickups(false);
            yield return new WaitForSeconds(seconds);
            ShowPickups(true);
        }
        
        private void ShowPickups(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform) { //get all the children in this gameObject
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            float distance = Vector3.Distance(callingController.transform.position, transform.position);
            NavMeshAgent player = callingController.GetComponent<NavMeshAgent>();
            if (Input.GetMouseButton(1)) {
                if (distance < interactRadius) {
                    PickUp(callingController.gameObject);
                }
                player.SetDestination(transform.position);
                player.stoppingDistance = interactRadius;
            }
            return true;
        }

        public CursorType GetCursor()
        {
            return CursorType.Pickup;
        }
    }
}
