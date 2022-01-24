using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        Health target;
        Mover mover;
        ActionScheduler actionScheduler;
        Animator animator;

        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] float timeBetweenAttack = 1f;
        [SerializeField] WeaponConfig defaultWeapon = null;

        WeaponConfig currentWeaponConfig = null;
        Weapon currentWeapon = null;

        float timeSinceLastAttack = 100;

        private void Awake() {
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            mover = GetComponent<Mover>();
            currentWeaponConfig = defaultWeapon;
        }
        
        private void Start()
        {
            if (currentWeapon == null) {
                EquipWeapon(defaultWeapon);
            }
        }

        void Update() {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            bool isInRange = Vector3.Distance(transform.position, target.transform.position) <= currentWeaponConfig.GetRange();
            
            if (!isInRange) { // this {if else} is now having a target
                mover.MoveTo(target.transform.position, 1f);
            }
            else { // in range
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator newAnimator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, newAnimator);
        }

        public Health GetTarget()
        {
            return target;
        }

        void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttack)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        // Animation Event (this method is called from the Animator)
        void Hit()
        {
            if (target == null) return;
            float calculatedDamage = GetComponent<BaseStats>().GetStat(Stat.BaseDamage) + currentWeaponConfig.GetDamage();
            if (currentWeapon != null) {
                currentWeapon.OnHit();
            }
            if (currentWeaponConfig.HasProjectile()) {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, gameObject, target, calculatedDamage);
            }
            else { // this gameObject, is dealing damage so it gets the experience
                target.TakeDamage(gameObject, calculatedDamage);
            }
        }

        void Shoot() {
            Hit();
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            if (!mover.CanMoveTo(combatTarget.transform.position)) { return false; }

            Health target = combatTarget.GetComponent<Health>();
            if (target != null && !target.IsDead())
            {
                return true;
            }
            return false;
        }

        public void Attack(GameObject CombatTarget) {
            actionScheduler.StartAction(this); //this class
            target = CombatTarget.GetComponent<Health>();    
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.BaseDamage) {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat) {
            if (stat == Stat.BaseDamage) {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        
    }
}
