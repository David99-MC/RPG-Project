using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using RPG.Saving;
using RPG.Stats;
using RPG.Core;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] UnityEvent OnDie;
        [SerializeField] TakeDamageEvent takeDamage;
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> 
        {}
        //the lines above will now allow me to use a dynamic float which is passed in as an argument

        float healthPoint = -1f;

        bool isDead = false;

        void Start() {
            if (healthPoint < 0) { // to avoid race condition with restoring the state
                healthPoint = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
        }

        private void OnEnable() {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            GetComponent<BaseStats>().OnLevelUp -= RegenerateHealth;
        }

        public bool IsDead() {
            return isDead;
        }

        // param: the experience will go to whoever (instigator below) is dealing the damage
        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoint = Mathf.Max(healthPoint - damage, 0);
            if (healthPoint <= 0)
            {
                OnDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else {
                takeDamage.Invoke(damage);
            }
        }

        public void Heal(float healthToRestore)
        {
            healthPoint = Mathf.Min(healthPoint+healthToRestore, GetMaxHealthPoints());
        }

        public int GetPercentage()
        {
            return (int)(GetFraction() * 100);
        }

        public float GetFraction()
        {
            return (healthPoint / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public float GetHealthPoints() {
            return healthPoint;
        }

        public float GetMaxHealthPoints() {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die()
        {
            if (isDead) { return; }

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            float xp = GetComponent<BaseStats>().GetStat(Stat.ExperienceReward);
            instigator.GetComponent<Experience>().GainExperience(xp);
        }

        private void RegenerateHealth()
        {
            healthPoint = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public object CaptureState()
        {
            return healthPoint;
        }

        public void RestoreState(object state)
        {
            float HP = (float)state;
            healthPoint = HP;

            if (healthPoint <= 0)
            {
                Die();
            }
        }
    }
}
