# Artifact Unity

Max Garber

Used [How to Build a Multiplayer Card Game with Unity 2D and Mirror by M. S. Farzan](https://www.freecodecamp.org/news/how-to-build-a-multiplayer-card-game-with-unity-2d-and-mirror/) as a starting point for the project:

## Running the Game

The game is currently Multiplayer only so you need 2 instances running to test most things. Build and Run the game (Ctrl-B) as one instance and run the game in Unity as the other instance. Running 1 instances of the game as host can sometime be helpfull just to make sure not everything crashes.

## How To Add Cards
Check [Cards.md](Cards.md) to see what cards have been implemented.

<details>
   <summary>Click to expand!</summary>
   If you are making an Enchantment Card start making the Enchantment/ability first.   

   **Do not change any of the existing scripts. If functionality is missing and not yet implemented either work on a different card or message me.**

   1. Create a Prefab Variant of the type of card you are making
   2. Replace the *Card* script if needed. The *Card* script is the last script in the prefab, it will not be the Base *Card* script but is Derived from it. **The new script must be Derived from the one you are replacing**. Replace the script by removing the existing one and adding the new one.
       * All Spell need their own script, here is where you program its functionality.
       * Enchantment scripts need to be replace with either the *Enchant Tower* or *Enchant Unit* script
       * Creeps and Heros most likely do not need a new script. Their abilities and effect should be added via an ability
   4. Fill out the attributes on the *Card* script in the inspector
   5. If you are making a Creep or hero with an ability **make the ability** then open up the prefab and add it as a child of the Abilities GameObject
        * you can skip this and comeback to it later
   6. Add your new card to *Registered Spawnable Prefabs* attribute in the NetworkManager
   7. Add your new card to the *Player Manager* Prefab either as a Card or a Heros.
        * The last card in the Cards list will alway be drawn in your starting hand for testing purposes
</details>

## How to Make an Enchantment or Ability

<details>
   <summary>Click to expand!</summary>

   Similar to making a card... (*will finnish this later*)  
   Unit Enchantments are the same thing as Abilities  
   While testing you can add tower enchantments directly to *Board/XXXXLane/XXXXSide/Enchantments* or Enchantments/abilities to an existing creep or hero (step 5 of adding a card). Just rember to remove them when you are done.
   </details>

### TaskList
- [x] Discard pile instead of destroying card
- [X] Play History (a history is required to handle cards such as Spell Steal)
- [x] Add hero deployments to history
- [ ] Draw from deck instead of spawning cards as needed (required to handle eclipse)
- [ ] OnCardPlayed trigger (ex. Zues's Static Field)
- [ ] OnUnitKillled tigger (ex. Kanna's Prey On The Weak)
- [ ] BeforeYourTurn tigger (ex. Kanna's Prey On The Weak)
- [ ] Make global auras linger during combat ( Drow's PRECISION AURA)
- [ ] Casting rules, require a caster of the correct color
---
- [x] Devour
- [x] Scheme (ex. Oglodi Catapult)
- [x] Death Shield (ex. Rumusque Redeemer)
- [ ] Damage Immunity (ex. Divine Purpose)
- [ ] Purge
- [ ] Untargetable
- [ ] Retaliate
- [ ] Card Back / unrevealed cards
---
- [ ] Items
- [ ] Shop
---
- [ ] Start Screen
- [ ] Set Deck via deck code
- [ ] Deck Builder
- [ ] Networking over the internet
- [x] Cards linger after played / discarded
---
- [ ] Make Game Look Good
