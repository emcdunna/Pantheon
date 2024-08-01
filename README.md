# Battle Deck Prototype
## Overview
I created this game as a prototype to test a "battle" system I plan on integrating with a wider grand strategy game's "campaign" mode. The primary focus is a turn-based deck-building game where two armies fight by playing cards that signify tactical maneuvers like "advance," "cavalry charge," or "retreat."

## Gameplay Mechanics
### Card System
Each card has symbols that correlate to specific attack or defense types that the units possess. The game operates on a rock-paper-scissors mechanic where:

- Shields block Arrows
- Spears block Cavalry
- Retreat blocks Swords
Any unblocked attack reduces the morale (health) of the opposing team in that sector. If a sector's health reaches 0, that sector flees (the on screen units run away).

### Victory Conditions
The objective is to either:
- Defeat both flank sectors (left and right)
- Defeat the middle sector

If either condition is satisfied then the entire army runs away and a Victory/Defeat screen is shown.

## Code Structure

### Suggested areas to review
The primary gameplay loop can be found in: https://github.com/emcdunna/Pantheon/blob/main/Scripts/BattleDeck/_MB_BattleRunner.cs
Player input is handled in: https://github.com/emcdunna/Pantheon/blob/main/Scripts/BattleDeck/Player.cs
A deck of battle cards is controlled by: https://github.com/emcdunna/Pantheon/blob/main/Scripts/BattleDeck/Deck.cs
The animation states and movements of units is controlled by: https://github.com/emcdunna/Pantheon/blob/main/Scripts/BattleDeck/UnitAnimation.cs
Here is how I handled Card UI hints: https://github.com/emcdunna/Pantheon/blob/main/Scripts/BattleDeck/_MB_CardUI.cs

### Architecture
The game uses a Model/View/Controller (MVC) architecture:

- Model: "Card" and "Deck" are instantiated as models.
- View: UI elements are generated or redistributed on the screen through "UI..." code.
- Controller: "Player" handles keyboard and mouse input as controls.

## Legacy Code
### Real-Time Strategy Prototype
Before developing the card-based battle prototype, I designed a real-time strategy battle game similar to the Total War series or Age of Empires. This included features like:

- Minimap
- Bounding box unit selection
- Grouped pathfinding
This code is available under the "Battle" and "Legacy battle system code" folders. Some libraries from this version were reused for the "BattleDeck" version. The code for this version is fully functional but was not engaging enough, leading to the shift to the deck-building concept.

## Additional Scripts
### Campaign Mode Data Generation
There is some Python code under the GameData folder used to auto-generate world data in CSV formats for the campaign mode of the game. While it is not used in the Battle Deck simulation, it serves as an example of some scripting I have done.

I hope this README provides a clear overview of the Battle Deck prototype and its development.
