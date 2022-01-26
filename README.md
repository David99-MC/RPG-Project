# Have a look at my game here: https://sharemygame.com/@Eru1k99/rpg-demo
# Welcome to my RPG game!
This is a Diablo-inspired RPG with my takes on the features. A nice place where I will reinforce my game programming experience as well as satisfy my imagination.
The game is still a demo and under development, which means it will get more and more updates in near future (as I learn new things to apply)!

# A glance over the architecture of the code
In order to optimize the efficiency and flexibility, the player will have their personal PlayerController and enemies will have their own AIController, both of which will share mutual scripts such as moving and fighting. Thanks to C#'s OOP feature, this is achieved easily as one class will call other public methods from other classes.

# The weapon system
Currently, each weapon is described by a scriptable object. By doing this, it is easier to modify the weapons' stats, decide if it is right or left-handed, and their corresponding animations. Moreover, using Delegates help each weapon expresses unique SFX as well as Interface to integrate any bonuses such as elemental factors or rarity.

# Stat system
There is one scriptable object which is responsible for organizing all the stats from player to the AI enemies. I use a Dictionary to store all the information and again, thanks to Delegates feature, my code doesn't have to check for level up every single frame, but only check for it when the player gained experience (i.e kill an enemy).
