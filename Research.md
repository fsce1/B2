# Game Research
# F.E.A.R.
An early example of great AI is the first-person shooter F.E.A.R, in which the developers created enemy AI NPCs that will react to the environment it is in. For example, if it senses it is in danger, it may fall back to a further piece of cover, or if it detects that there is an opening, it may even flank the player and attack from behind. 
![[Pasted image 20240229100026.png]]
This adds a sense of realism to all the fights, as the enemies are unpredictable and act like real soldiers might. It creates much more dynamic encounters and boosts the replay value. F.E.A.R. is the gold standard for enemy AI in shooters and its design decisions such as Goal Oriented Action Plans and Finite State Machines are still used in modern games such as Metro Exodus (2019) and Half-Life: Alyx (2020).
# Rimworld
Another example of good NPC AI in games is Rimworld. Rimworld is quite different to F.E.A.R., as it is a top-down 2D base building game in which you partially control multiple characters at once. At the beginning of the game, you choose 3 characters from a preset list, and each of these characters has unique skills and traits. 
![[Pasted image 20240229094850.png]]
For example, a character could be very good at cooking, but bad at construction and shooting. They can also have traits such as "Smoker", which means they need to have access to cigarettes, or "Hates Animals," meaning they receive a mood debuff when around animals. In-game, your characters also have needs such as food, sleep, recreation and mood. Depending on the events in the game, their mood may decrease, for example if they are sleeping in a cold room or get injured. Mood can also increase with events such as being social, eating a good meal, or winning a fight.
![[Pasted image 20240229095309.png]]
Since you can't control multiple characters at once easily, the characters in Rimworld have a large degree of automation and AI control. You can manually set the order in which different people complete tasks by using the "Work" screen.
![[Pasted image 20240229095605.png]]
For example, if a character spots a fire, and it is currently doing a task with a priority of 2, it will quit that task and automatically put out the fire, as firefighting has a higher priority of 1. This allows you to automate the game much more and create an efficient colony with the help of AI. This is similar to F.E.A.R.'s Goal Oriented Action Plan, as it selects a goal for the character based on the current environment and conditions. For example, if the food in storage is low, the person with the highest priority for cooking will start cooking meals automatically.
# S.T.A.L.K.E.R.
The S.T.A.L.K.E.R. series has a very interesting approach to AI enemies, as it has 10 different factions that inhabit the game's open world. For example, if you are part of the Military faction, the Clear Sky faction will be hostile, but you are friendly with the Ecologists. You can start as any faction, meaning every playthrough is unique as you will be fighting a different set of enemies.
![[Pasted image 20240322152352.png]]
The AI system in S.T.A.L.K.E.R. is called A-Life, and it is designed using GOAP as well. Any AI that is within 150m of you is considered "Online," meaning that it is active and can move around/shoot, etc. Each "Online" AI has its own goals and routines for the day. For example, someone in the Loner faction may wake up at 8am, hang around their camp for a while, then squad up with some friends to go to to a mutant hideout and hunt some mutants for food. Then they may cook and eat, and go to an Anomalous Zone to hunt valuable Artefacts before returning to camp to relax by the campfire until night. Even enemies engage in this behaviour, meaning its possible to find an enemy squad roaming the open world, not just at their bases. This is called Emergent Gameplay, where multiple procedural systems interact with each other in different ways and give way to unique gameplay that is different for every playthrough.
You can also interact with every friendly AI by walking up to them. You can usually get a quest from them, such as finding a specific gun and giving it to them, or clearing out a mutant/enemy camp. Some AI's are also dealers, meaning you can buy and sell items from them. 
For a game released in 2009, A-Life was quite an ambitious AI system. It had quite a few glitches, for example the AI characters could actually complete the main quest before the player and finish the game.
In S.T.A.L.K.E.R. Anomaly, a fan-made mod pack that expands the game, these relationships are also dynamic. For example, if you finish a gunfight and there is an enemy lying on the ground that isn't dead, you can choose to either heal them, leave them to die, or kill them. All of these choices will impact your standing with that faction in different ways.
# AI Research

AI is a very broad subject of computer science, and it has been portrayed in many different ways throughout its conception. 
In 1984, one of the first books to tackle the subject of AI, Neuromancer  was released. It explored the possibility of storing people's consciousness in computer storage and being able to create new ones, as well as putting those consciousnesses into robots. It invented the phrase "Matrix" to describe a simulated world that lives inside a computer. This being one of the first pieces of media to be in a Cyberpunk style, it influenced many future films, games, and other pieces of media such as Cyberpunk 2077, The Matrix and Ghost in the Shell.
An example of an AI character is GLaDOS from the Portal games. She is the AI controller of Aperture Science, a large scientific research facility that the Portal games take place in. She oversees your gameplay, adding context and humour as you complete the game's tests, and by the end of Portal 2, becomes a character you can empathise with and understand.

These are examples of AI as a narrative concept, however in the context of games, AI can mean a different thing. The Non-Player Characters (NPCs) that fill up many games use code to make the player feel like they are interacting with a realistic character. This form of AI uses many different techniques to achieve this, for example pathfinding to make the NPC move to the correct position, or advanced algorithms to decide how an NPC may act in combat as well as choosing the right voice line to play when an event occurs.
## Goal Oriented Action Planning (GOAP)
https://www.youtube.com/watch?v=nEnNtiumgII
https://www.youtube.com/watch?v=gm7K68663rA
<span style="color:#FF0000">
GOAP is an AI system that was first used in F.E.A.R. and has been used in many games since. It is a very robust and "smart" system, meaning that there is a lot of nuance in the ways that NPCs can act in response to the player, and they can even do things proactively, without any input from the player. <br>
It works by setting an end goal, usually something like "Gain Money," or "Kill Player," and then there are multiple actions that can lead to that outcome. Each of these actions has pre-conditions (for example, you need a weapon in order to hunt for food). <br>
For example, in a game where the AI NPC's goal is to eat food, it may be able to choose between foraging for berries or go hunting for an animal. <br>
You can also provide costs for each action plan, meaning that if an NPC already has a weapon to hunt with, it may prefer to go hunting, however if it needs to create a weapon and then go hunting, it may decide to forage for berries instead, as it is a simpler action plan. <br>
This system is very good and allows NPCs to act in a much more immersive and realistic way, properly choosing the correct action in order to achieve their goals. However, it is mainly useful in games such as Rimworld and other survival games. This is because the system is quite overkill for FPS enemies, as they don't have many actions that they can perform aside from shooting, reloading and moving.
</span>
![[Pasted image 20240426152417.png]]
## Finite State Machines (FSM)
https://www.youtube.com/watch?v=JyF0oyarz4U
<span style="color:#FF0000">
An FSM is a simple bit of logic that is used in many different systems, including engineering, software development, and game development.<br>
It works by creating multiple "Nodes" or "States", with transitions between those states. For example, a few States for an enemy AI could be things such as "Attack," "Idle," "Patrol," "Push," or "Flee." They are essentially what the AI is currently doing. <br>
The AI starts in an "Idle" State, and once it spots a player, it may transition into the "Attack," State. Then, after it has fired a few shots it may transition into a "Flee," or "Push," State, depending on many different factors such as if it hit any shots, if it is damaged at all, if it has teammates nearby, etc. <br>
However, FSMs are not very scalable. Once you start adding many different States, it becomes more and more complicated to handle the transitions between every node, increasing development and debugging time for more complex State Machines. This means that it is a good system for fairly simple AI's, such as those in platformers like Mario, or casual shooters such as Risk of Rain 2.
</span>
![[Pasted image 20240426152538.png]]
# Behaviour Trees
https://www.youtube.com/watch?v=6VBCXvfNlCM
<span style="color:#FF0000">
Behaviour trees are quickly becoming one of the most used AI systems in AAA games. They work in a similar way to GOAP, however different in a few very important ways. Instead of having an end goal, the flow of the tree starts from the root node at the top, and the current behaviour is decided by moving down through "decision" or "selector" nodes. A selector node could be something like "Is player within a radius," or "Is out of ammo." <br>
Depending on the result of these selector nodes, the current state of the AI NPC will be changed. For example, if the magazine is below 5 rounds, the selector node may change the state to reloading instead of shooting.<br>
Behaviour trees have many upsides when compared to an FSM, specifically you don't have to individually handle every transition between states as the tree does that automatically. As well as this, it is much easier to debug as you can view the current path that is being taken in a simple graphical display. This makes it much more scalable. <br>
This is also the only system that doesn't require any coding experience, so it is much more accessible to designers.
</span>
![[Pasted image 20240524142229.png]]