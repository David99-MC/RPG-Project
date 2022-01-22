using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using RPG.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        // Action (keyword) is a type of delegation that return nothing (void)
        public event Action OnExperienceGained; // list of pointers to functions

        [SerializeField] float experiencePoint = 0;

        public void GainExperience(float xp)
        {
            experiencePoint += xp;
            OnExperienceGained(); // call all the function in this delegation list
        }

        public float GetExperiencePoint() {
            return experiencePoint;
        }

        public object CaptureState()
        {
            return experiencePoint;
        }

        public void RestoreState(object state)
        {
            float xp = (float)state;
            experiencePoint = xp;
        }
    }
}
