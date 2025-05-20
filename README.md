# 🕹️ Unseen Ancients: Top-Down 2D Adventure

**Unseen Ancients** is a 2D top-down action-adventure game built with **Unity**, combining dark fantasy aesthetics with rich gameplay systems. The game is designed with modular, expandable systems and hand-crafted visuals, including custom enemy animations and interactive mechanics.

## 🛠️ Tech Stack

- **Engine**: Unity 2022+
- **Language**: C#
- **Animation**: Unity Animator & custom 2D sprite sheets
- **UI**: Unity UI Toolkit (or legacy UI)

## ⚙️ Core Features

### 🎒 Inventory System
- Fully dynamic drag-and-drop inventory UI
- Item pickup, drop, stack, equip, and use functionality
- tooltips, and quickslot hotkeys

### ✨ Ability System
- Abilities are bindable to hotkeys
- Cooldowns, charges, and unique effects per skill
- Passive and active abilities integration

### 💰 Looting System
- Dynamic loot generation
- World drops, loot chests, and enemy drops
- Proximity-based or interact-based looting

### 📜 Quest System
- Modular quest definitions via ScriptableObjects
- Multi-stage quests with objectives and rewards
- Journal UI and quest tracking

### 🤖 Enemy AI
- Finite State Machines (FSM) for behavior control
- Patrol, chase, flee, and attack states
- Enemies react to visibility

## 🎨 Art & Animation

All enemy sprites and animations are drawn by hand. Below are samples of animations implemented in the game:

### 🧟‍♂️ Marauder Walk Animation

![Marauder Walk](Animations/Marauder/Marauder_Move..gif)

### ⚔️ Big Marauder All Animations

![Marauder Attack](Animations/BigMarauder/GiantMarauder_DEMO.gif)

☠️ Skeleton — Walk & Attack

![Aracher](Animations/Skeletons/SkeletonArcher_Death_Right.gif)

![Bomber](Animations/Skeletons/Bomber_Demonsttrative.gif)

![Summon](Animations/Skeletons/SkeletonCreature_right+awake.gif)

👑 Boss — Move & Attack

![Boss Run](Animations/DeathGod/Death%20God_Run.gif)

![Boss Attack](Animations/DeathGod/Death%20God_Front_Sweeping_Poke.gif)

## 🚧 In Development

This project is still under development. Upcoming features include:
- Save/load system
- Boss fights
- More quests and biomes


## 🧑‍🎨 Author

Created by [AplyQ8]   
Animations, code, and design are all original work.
