using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        public event Action OnLevelUp;
        [SerializeField] UnityEvent sfx;
        [SerializeField] Progression progression = null;
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] GameObject levelUpFX;
        [SerializeField] bool shouldUseModifiers = false;

        Experience experience;

        private void Awake() {
            experience = GetComponent<Experience>();
        }

        int currentLevel = 0;

        void Start() {
            currentLevel = CalculateLevel();
        }

        private void OnEnable() {
            if (experience != null) {
                experience.OnExperienceGained += UpdateLevel;
                // add the function to this delegation list (subscribe to the list)
            }
        }

        private void OnDisable() {
            if (experience != null) {
                experience.OnExperienceGained -= UpdateLevel;
                // add the function to this delegation list (subscribe to the list)
            }
        }

        void UpdateLevel() {
            int newLevel = CalculateLevel(); // checking if the player has leveled
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUpEffect();
                OnLevelUp();
            }
        }

        void LevelUpEffect()
        {
            sfx.Invoke();
            Instantiate(levelUpFX, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat)/100);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        float GetAdditiveModifier(Stat stat) { // this will differ depends on the function called in other classes
            if (!shouldUseModifiers) return 0; // only Player will have the modifiers
            float total = 0;
            // there can be multiple IModifierProvider. Each for BaseDamage, Health, etc.
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in provider.GetAdditiveModifiers(stat)) {
                    total += modifier;
                }
            }
            return total;
        }

        float GetPercentageModifier(Stat stat) {
            if (!shouldUseModifiers) return 0; // only Player will have the modifiers
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in provider.GetPercentageModifiers(stat)) {
                    total += modifier;
                }
            }
            return total;
        }

        public int GetLevel() {
            if (currentLevel < 1) {
                currentLevel = CalculateLevel();
            }
            return currentLevel;
        }

        int CalculateLevel() {

            Experience experience = GetComponent<Experience>();

            if (experience == null) return startingLevel;

            float currentXP = experience.GetExperiencePoint();

            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            // go through the elements in Progression
            for (int level = 1; level <= penultimateLevel; level++) {
                // get the next Level which has the higher limit of XP
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentXP) {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }

    }

}