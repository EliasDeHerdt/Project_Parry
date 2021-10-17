# Project_Parry
 Project_Parry is a game I made for the course Game Mechanics at Howest university of applied sciences. In this repo, you can find the code for this project.

## Important code:
 - [void HandleCameraLockOn()](https://github.com/EliasDeHerdt/Project_Parry/blob/b09ea6c1aa90a486af1b50ee9837da105eaf7848/Character/PlayerCharacter.cs#L106) Handles the code for the camera lock-on system.
 - [void MakeSwing(float delta)](https://github.com/EliasDeHerdt/Project_Parry/blob/b09ea6c1aa90a486af1b50ee9837da105eaf7848/Weapon/BasicWeapon.cs#L155) Handles the weapons swinging animations.
 - [BasicWeapon.OnTriggerEnter(Collider other)](https://github.com/EliasDeHerdt/Project_Parry/blob/b09ea6c1aa90a486af1b50ee9837da105eaf7848/Weapon/BasicWeapon.cs#L74) The code that checks when to deal damage to an opponent.
 - [public void ParryHit()](https://github.com/EliasDeHerdt/Project_Parry/blob/b09ea6c1aa90a486af1b50ee9837da105eaf7848/Character/PlayerCharacter.cs#L208) Gets called on the player whenever he hits a parry. Since the player can not use abilities without hitting parries, this is very important for the game.
