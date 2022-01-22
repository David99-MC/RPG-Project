using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;



namespace RPG.Cinematic
{
    public class CInematicTrigger : MonoBehaviour
    {
        bool hasPlayed = false;

        void OnTriggerEnter(Collider other) {
            
            if (other.gameObject.tag == "Player") {
                if (!hasPlayed) {
                    GetComponent<PlayableDirector>().Play();
                    hasPlayed = true;
                }
            }    
        }

    }
}
