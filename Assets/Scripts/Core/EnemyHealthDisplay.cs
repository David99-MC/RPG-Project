using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RPG.Attributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter = null;

        // Get the current target that the Player is focusing on  
        // (the Fighter component on PLayer has the reference of the current enemy)
        private void Awake() {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        // Update is called once per frame
        void Update()
        {
            if (fighter.GetTarget() == null) {
                GetComponent<Text>().text = "N/A";
                return;
            }
            else {
                Health health = fighter.GetTarget();
                GetComponent<Text>().text = health.GetHealthPoints() + "/" + health.GetMaxHealthPoints();
            }
            
        }
    }
}
