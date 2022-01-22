using UnityEngine;

using RPG.Attributes;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded= true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        //spawn the weapon base on which hand should use it (i.e. swords, bows, etc.)
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator) {

            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;
            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null) {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null) {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }

        void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            //check whether the weapon is on right or left hand
            Transform oldWeapon = rightHand.Find(weaponName);  
            if (oldWeapon == null) {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return; // currently have no weapon on both hands
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, GameObject instigator, Health target, float calculatedDamage) {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand,leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
            // (comment for line above): also set who is firing the projectile to the target
        }

        public bool HasProjectile() {
            if (projectile != null) return true;
            else return false;
        }

        public float GetRange() {
            return weaponRange;
        }

        public float GetDamage() {
            return weaponDamage;
        }

        public float GetPercentageBonus() {
            return percentageBonus;
        }
    }
}
