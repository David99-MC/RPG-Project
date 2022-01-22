using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{   // THIS SCRIPT IS PUT ON THE ENEMIES
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursor()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            Fighter fighter = callingController.GetComponent<Fighter>();
            if (!fighter.CanAttack(gameObject)) return false;

            if (Input.GetMouseButton(1))
            {
                fighter.Attack(gameObject);
            }
            return true;
        }

    }
}
