using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Control;
using RPG.Attributes;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0f;
        [SerializeField] float hideTime = 5f;

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                PickUp(other.gameObject);
            }
        }

        private void PickUp(GameObject player)
        {
            if (weapon != null) {
                player.GetComponent<Fighter>().EquipWeapon(weapon);
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
            if (Input.GetMouseButtonDown(1)) {
                PickUp(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursor()
        {
            return CursorType.Pickup;
        }
    }
}
