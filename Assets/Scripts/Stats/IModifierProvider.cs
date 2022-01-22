using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    // use a Modifier here as an interface to modify not just weapon, but also other stats
    // such as Health, Mana, Armor, Attack Speed, etc.
    public interface IModifierProvider  
    {
        IEnumerable<float> GetAdditiveModifiers(Stat stat);
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}
