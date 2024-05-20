# Research
AI is a very broad subject of computer science, and it has been portrayed in many different ways throughout its conception. 
In 1984, one of the first books to tackle the subject of AI, Neuromancer  was released. It explored the possibility of storing people's consciousness in computer storage and being able to create new ones, as well as putting those consciousnesses into robots. It invented the phrase "Matrix" to describe a simulated world that lives inside a computer. This being one of the first pieces of media to be in a Cyberpunk style, it influenced many future films, games, and other pieces of media such as Cyberpunk 2077, The Matrix and Ghost in the Shell.
Another example of an AI character is GLaDOS from the Portal games. She is the AI controller of Aperture Science, a large scientific research facility that the Portal games take place in. She oversees your gameplay, adding context and humour as you complete the game's tests, and by the end of Portal 2, becomes a character you can empathise with and understand.

However, in the context of games, AI can mean a different thing. The Non-Player Characters (NPCs) that fill up many games use code to make the player feel like they are interacting with a realistic character. This form of AI uses many different techniques to achieve this, for example pathfinding to make the NPC move to the correct position, or advanced algorithms to decide how an NPC may act in combat as well as choosing the right voice line to play when an event occurs.
#  F.E.A.R.
An early example of great AI is the first-person shooter F.E.A.R, in which the developers created enemy AI that will react to the environment it is in. For example, if it senses it is in danger, it may fall back to a further piece of cover, or if it detects that there is an opening, it may even flank you and attack from behind. 
![[Pasted image 20240229100026.png]]
This adds a sense of realism to all the fights, as the enemies are unpredictable and act like real soldiers might. It creates much more dynamic encounters and boosts the replay value. F.E.A.R. is the gold standard for enemy AI in shooters and its design decisions such as Goal Oriented Action Plans and Finite State Machines are still used in modern games such as Metro Exodus and Half-Life: Alyx.

## Goal Oriented Action Planning (GOAP)

<span style="color:#FF0000">
GOAP is an AI system that was first used in F.E.A.R. and has been used in many games since. It is a very robust and "smart" system, meaning that there is a lot of nuance in the ways that NPCs can act in response to the player, and they can even do things proactively, without any input from the player.
</span>
![[Pasted image 20240426152417.png]]
## Finite State Machines (FSM)
<span style="color:#FF0000">
An FSM is a simple bit of logic that is used in many different systems, including engineering, software development, and game development.<p>
It works by creating multiple "Nodes" or "States", with transitions between those states. For example, a few States for an enemy AI could be things such as "Attack," "Idle," "Patrol," "Push," or "Flee." They are essentially what the AI is currently doing. <p>
The AI starts in an "Idle" State, and once it spots a player, it may transition into the "Attack," State. Then, after it has fired a few shots it may transition into a "Flee," or "Push," State, depending on many different factors such as if it hit any shots, if it is damaged at all, if it has teammates nearby, etc. <p>
However, once you start adding many different States, it becomes more and more complicated to handle the transitions between every node. This means that it is a good system for fairly simple AI's, such as those in platformers like Mario, or casual shooters such as Risk of Rain 2, however since I want to create a more realistic and deep AI, an FSM is probably not the best option for me to use.
</span>
![[Pasted image 20240426152538.png]]

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

# Starfield
I decided to look at a game that I believe has a bad AI system for it's enemies, as many players have also spoken about. I looked online to see what people disliked about the AI.

"The ai literally is so bad. They would forget you are fighting them if you run away just 10 feet, they will run haywire everywhere when they are fighting in groups, failing to maintain any sort of battle cohesion."
-[DarthMaul628](https://www.reddit.com/user/DarthMaul628/)
