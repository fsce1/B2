# Ideation
I knew that I wanted to create a first-person shooter, as it is a great way to experiment with hostile AI and a lot of my favourite games are FPS's. However, FPS is a very broad genre and has many different styles and speeds of gameplay. For example, Escape from Tarkov is a very slow, tactical and realistic shooter with accurate bullet and armour physics, meaning one shot is enough to kill in many circumstances. In comparison, a game like Titanfall 2 is very movement-based and fast-paced, with a long time-to-kill (TTK). Titanfall 2 has advanced movement mechanics such as double-jumping, sliding, wall-running and bunny-hopping which pushes the skill ceiling very high.
## Idea 1: Tactical Shooter
My Initial Idea was to create a Tactical Shooter in the style of Escape from Tarkov and Counter-Strike 2. These games have a very high skill ceiling and play quite slowly, as they have slow movement speed and increased inaccuracy when walking. This allows for a very tactical game with a lot of high-importance decision making and many ways to approach a given situation. In order to fit the brief, I would implement enemy AI and potentially a friendly AI that you can direct to open doors, throw flashbangs, as well as covering your rear for any flankers. I would need to code a custom AI to follow your commands, as well as staying out of your way and not blocking any doorways/passages.
As this game would be single-player, I think a good game loop could be inspired by the Armed Assault (ArmA) series. These games have multiple "scenarios," which you can complete in any order you wish. An example of a scenario may be travelling to a town that is enemy occupied, attacking the town, destroying radio equipment or vehicles etc, and then exfiltrating the area. I think this is a good game loop to bring back, as it is very open and allows the player to approach the current situation in many different ways. As well as this, it may be possible to randomise enemy spawn locations and let you infiltrate the area from multiple entry points. I think this would increase the replayability of each scenario, and encourage speedrunners to play the game.
I am inspired by Eastern European shooters such as Tarkov and S.T.A.L.K.E.R. for the visual style, as they are both quite realistic and grounded.
## Idea 2: Movement Shooter
Another idea I had was a very fast, movement-based shooter with grappling hooks. I was inspired by the game Titanfall 2, as well as the Mirror's Edge games to create a shooter with a lot of map traversal and verticality, as well as being able to hit people without needing to aim down sights, or stay still to maintain accuracy. It could also score you based on time as well as bullets used, allowing sections to be replayed in hopes of "speedrunning" the areas. This would increase the replayability of the game and might attract speedrunners. Large maps with semi-non-linear pathways could make it more interesting to find new paths through the map for a faster time.
The visual style for this game would likely be futuristic/sci-fi, as advanced movement mechanics in a realistic or historical setting may be jarring or out of place.
<span style="color:#FF0000">The narrative is that you are part of a group of the last humans in the world, after the rest have been killed by a rogue AI. The story of the game is being hunted by a powerful AI that sends robots out to kill you.</span>
## Idea 3: Realistic Sniper Game

Another game idea I had was a 
- Realistic Zeroing and Bullet physics
- Bipod stabilisation
- Bolt action rifles
- Simpler AI, will try to locate you and fire back as well as running to cover if they spot you.
## Idea 4: Base-builder
Inspired by Rimworld, Foxhole and Dwarf Fortress
Set in the post-apocalypse, you are tasked with rebuilding a city using many AI NPCs.
Top-down perspective, you micromanage hundreds of people with their own jobs such as collecting wood, mining resources, building structures, etc.
You also need to balance the number of people in your military. More people in your army means fights will be easier, but there are less people to do normal jobs.
Maybe an XCOM or Red Alert-style military system, with turn based combat and managing the entire army.

# Idea 5:
## Chosen Idea
I decided that I would choose the first idea, a tactical shooter. I chose this because I have more experience in similar games and they are some of my favourites to play.
<span style="color:#FF0000">Realistic games have also always been very interesting to me, as they have the ability to immerse you further in the world through mechanics such as permadeath, realistic health and bullet mechanics and punishing AI.
</span>
I created a moodboard to help pin down the visual style and setting of my game. I want to have a modern, Eastern-European setting, with a large military presence and realistic area maps.
![[SilentStrikeMoodboard.png]]
<span style="color:#FF0000">
I then started to think of the narrative and setting of the game. 
I want a realistic conflict that could happen in real life, as well as taking place in a fictional country to avoid real life parallels.
I named the country Sujusterea, and it is located along the Baltic sea, neighbouring Latvia and Lithuania. A civil war has been brewing for a long time, since the previous one in 1968 and the government's increasing allyship with Russia has angered the citizens.
</span>

As well as this, I considered which game engine to use for this project. The main 2 I considered were Unreal Engine 5 and Unity.
Each engine has its upsides and downsides, for example, Unreal Engine 5 has access to Quixel Megascans. This is one of the largest 3D asset databases in the world, including over 18,000 photorealistic and photo-scanned assets for completely free, as long as you use them inside Unreal Engine 5. In comparison, Unity has a few free assets on the Unity Asset Store, however they are made by the community and have varying levels of quality.
Unity uses C# as a scripting language, compared to C++ in Unreal 5. Both have visual scripting solutions, however I prefer to use code to node-based scripting. I am much more familiar with Unity, and have been using it for around 3 years now, and have only slightly dabbled with using Unreal 5. For this reason, I will use Unity as it allows me to realise my ideas much more quickly as I have a better idea of how to use the program.