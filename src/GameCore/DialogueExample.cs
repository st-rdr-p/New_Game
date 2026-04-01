using System;
using System.Collections.Generic;

namespace GameCore
{
    /// <summary>
    /// Example showing how to set up entities with dialogue and encounters.
    /// This demonstrates integrating the dialogue system with your game.
    /// </summary>
    public class DialogueExample
    {
        /// <summary>
        /// Example 1: Create a simple NPC merchant with greeting dialogue.
        /// </summary>
        public static Entity CreateMerchantNPC(int entityId)
        {
            var merchant = new Entity(entityId);
            merchant.AddComponent(new Transform3D { Position = new Vector3(5, 0, 10) });
            merchant.AddComponent(new Tag("NPC"));

            // Build merchant dialogue
            var dialogue = new DialogueBuilder("Eldra, the Merchant")
                .Sequence()
                    .Line("Eldra", "Welcome to my shop, traveler!", 2f)
                    .Line("Eldra", "I have many fine wares for sale.", 2f)
                    .Choice("What can you sell me?", 1)
                    .Choice("Just passing through.", 2)
                .Sequence()
                    .Line("Eldra", "I sell potions, weapons, and rare items.", 2f)
                    .Line("Eldra", "Browse to your heart's content!", 2f)
                .Sequence()
                    .Line("Eldra", "Safe travels then, friend.", 1f)
                .BuildFor(merchant);

            // Add encounter trigger
            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue)
            {
                TriggerRange = 8f,
                CanTriggerMultipleTimes = true,
                InitialDialogueSequence = 0
            };
            merchant.AddComponent(trigger);

            return merchant;
        }

        /// <summary>
        /// Example 2: Create a hostile enemy with combat dialogue.
        /// </summary>
        public static Entity CreateHostileEnemy(int entityId, Vector3 position)
        {
            var enemy = new Entity(entityId);
            enemy.AddComponent(new Transform3D { Position = position });
            enemy.AddComponent(new Tag("Enemy"));
            enemy.AddComponent(new Health(50f));

            // Create enemy AI
            var ai = new EnemyAI
            {
                CurrentState = EnemyAI.State.Idle,
                DetectionRange = 25f,
                AttackRange = 5f
            };
            enemy.AddComponent(ai);

            // Build hostile dialogue
            var dialogue = new DialogueBuilder("Brutal Orc")
                .Sequence()
                    .Line("Brutal Orc", "HRAAAGHH! Found you!", 1.5f)
                    .Line("Brutal Orc", "Prepare to taste blade!", 1.5f)
                .BuildFor(enemy);

            // Create encounter that auto-starts combat
            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.Enemy, dialogue)
            {
                TriggerRange = 15f,
                CanTriggerMultipleTimes = false,
                StartCombatAfter = true,
                InitialDialogueSequence = 0
            };

            // Trigger combat when encounter ends
            trigger.OnEncounterEnd += () =>
            {
                ai.CurrentState = EnemyAI.State.Chase;
            };

            enemy.AddComponent(trigger);
            return enemy;
        }

        /// <summary>
        /// Example 3: Create a wise NPC offering quests.
        /// </summary>
        public static Entity CreateQuestGiver(int entityId)
        {
            var questGiver = new Entity(entityId);
            questGiver.AddComponent(new Transform3D { Position = new Vector3(0, 0, 20) });
            questGiver.AddComponent(new Tag("NPC"));

            var dialogue = new DialogueBuilder("Wise Sage")
                .Sequence()
                    .Line("Wise Sage", "Greetings, adventurer.", 2f)
                    .Line("Wise Sage", "The land is in great peril.", 2f)
                    .Choice("I can help!", 1)
                    .Choice("Perhaps later.", 3)
                .Sequence()
                    .Line("Wise Sage", "Excellent! Dark forces have awakened in the North.", 3f)
                    .Line("Wise Sage", "Will you journey there and stop them?", 2f)
                    .Choice("I accept your quest!", 2)
                    .Choice("That sounds too dangerous.", 3)
                .Sequence()
                    .Line("Wise Sage", "You are most brave. May fortune favor you.", 2f)
                    .Line("Wise Sage", "Seek the Tower of Shadows in the North.", 2f)
                .Sequence()
                    .Line("Wise Sage", "Very well. The choice is yours.", 2f)
                .BuildFor(questGiver);

            // Add encounter
            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue)
            {
                TriggerRange = 10f,
                CanTriggerMultipleTimes = false,
                InitialDialogueSequence = 0
            };
            questGiver.AddComponent(trigger);

            return questGiver;
        }

        /// <summary>
        /// Example 4: Create a boss with complex dialogue tree.
        /// </summary>
        public static Entity CreateBossEncounter(int entityId)
        {
            var boss = new Entity(entityId);
            boss.AddComponent(new Transform3D { Position = new Vector3(30, 0, 30) });
            boss.AddComponent(new Tag("Boss"));
            boss.AddComponent(new Health(200f));

            var bossMesh = new MeshRenderer { MeshId = "boss_model", MaterialId = "boss_material" };
            boss.AddComponent(bossMesh);

            // Complex boss dialogue
            var dialogue = new DialogueBuilder("Shadow King")
                .Sequence()
                    .Line("Shadow King", "You dare enter my throne room?", 2f)
                    .Line("Shadow King", "I am the eternal darkness, the void itself!", 2f)
                    .Choice("I challenge you in combat!", 1)
                    .Choice("Who are you really?", 2)
                    .Choice("I'm running away!", 3)
                .Sequence()
                    .Line("Shadow King", "A warrior's heart! I respect that.", 2f)
                    .Line("Shadow King", "Then let us dance with death!", 2f)
                    .Line("Shadow King", "COME! Show me your strength!", 2f)
                .Sequence()
                    .Line("Shadow King", "I was once like you, filled with hope and light.", 3f)
                    .Line("Shadow King", "But the world showed me only betrayal and pain.", 3f)
                    .Line("Shadow King", "So I embraced the darkness instead.", 2f)
                    .Line("Shadow King", "Now you must too!", 2f)
                .Sequence()
                    .Line("Shadow King", "Fool! There is no escape!", 2f)
                    .Line("Shadow King", "My power is absolute here!", 2f)
                .BuildFor(boss);

            // Boss AI
            var bossAI = new EnemyAI
            {
                CurrentState = EnemyAI.State.Idle,
                DetectionRange = 30f,
                AttackRange = 8f
            };
            boss.AddComponent(bossAI);

            // Boss encounter
            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.Boss, dialogue)
            {
                TriggerRange = 20f,
                CanTriggerMultipleTimes = false,
                StartCombatAfter = true,
                InitialDialogueSequence = 0
            };

            trigger.OnEncounterEnd += () =>
            {
                bossAI.CurrentState = EnemyAI.State.Chase;
            };

            boss.AddComponent(trigger);
            return boss;
        }

        /// <summary>
        /// Example 5: Friendly NPC with multiple-visit dialogue.
        /// </summary>
        public static Entity CreateFriendlyNPC(int entityId)
        {
            var friend = new Entity(entityId);
            friend.AddComponent(new Transform3D { Position = new Vector3(-10, 0, 0) });
            friend.AddComponent(new Tag("NPC"));

            var dialogue = new DialogueBuilder("Sofia, the Ranger")
                .Sequence()
                    .Line("Sofia", "Oh! I haven't seen you in ages!", 2f)
                    .Line("Sofia", "I've been training in the forest.", 2f)
                    .Line("Sofia", "How have you been?", 1f)
                .BuildFor(friend);

            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue)
            {
                TriggerRange = 6f,
                CanTriggerMultipleTimes = true,  // Can chat multiple times
                InitialDialogueSequence = 0
            };
            friend.AddComponent(trigger);

            return friend;
        }

        /// <summary>
        /// Example 6: Set up a complete game scene with dialogue.
        /// </summary>
        public static void SetupGameWithDialogue(Game game, DialogueSystem dialogueSystem)
        {
            // Create entities
            var merchant = CreateMerchantNPC(1);
            var questGiver = CreateQuestGiver(2);
            var enemy = CreateHostileEnemy(3, new Vector3(15, 0, 15));
            var boss = CreateBossEncounter(100);
            var friend = CreateFriendlyNPC(5);

            // Add to game
            game.AddEntity(merchant);
            game.AddEntity(questGiver);
            game.AddEntity(enemy);
            game.AddEntity(boss);
            game.AddEntity(friend);

            // Setup dialogue system UI callbacks
            dialogueSystem.OnDisplayDialogueLine += (line) =>
            {
                Console.WriteLine($"\n>>> {line.Speaker}: {line.Text}");
            };

            dialogueSystem.OnDisplayChoices += (choices, onChoose) =>
            {
                Console.WriteLine("\nChoices:");
                for (int i = 0; i < choices.Count; i++)
                {
                    Console.WriteLine($"  [{i}] {choices[i].ChoiceText}");
                }
                // In real implementation, wait for player input and call onChoose
            };

            dialogueSystem.OnHideDialogue += () =>
            {
                Console.WriteLine("\n*** Dialogue ended ***\n");
            };
        }
    }
}
