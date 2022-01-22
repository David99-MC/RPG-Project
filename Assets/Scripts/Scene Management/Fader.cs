using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActiveFader = null;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate() {
            canvasGroup.alpha = 1 ;
        }

        // this prevent the Coroutines to overlap each other (kinda similar to a dead lock)
        public Coroutine FadeOut(float time) {
            return Fade(1, time);
        }
        
        public Coroutine FadeIn(float time) {
            return Fade(0, time);
        }

        public Coroutine Fade(float target, float time) {
            if (currentActiveFader != null)
            {
                StopCoroutine(currentActiveFader);
            }
            currentActiveFader = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFader;
        }

        IEnumerator FadeRoutine(float target, float time) {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                yield return null; // skip the first frame (fps) 
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
            }
        }

    }
}
