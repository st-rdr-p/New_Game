using System;
using System.Collections.Generic;
using System.IO;

namespace GameCore
{
    /// <summary>
    /// Example of loading dialogue data from JSON and integrating into game.
    /// </summary>
    public class DialogueLoadingExample
    {
        /// <summary>
        /// Setup game with dialogue loaded from JSON file.
        /// </summary>
        public static void SetupGameWithDialogueData(Game game, DialogueSystem dialogueSystem, IEngineAudio audio)
        {
            // Create loader
            var loader = new DialogueDataLoader(audio);

            // Load dialogue from JSON file
            // Path depends on where your project is deployed - adjust as needed
            string dialoguePath = FindDialogueFile();
            
            if (dialoguePath == null)
            {
                Console.WriteLine("Warning: Could not find dialogue_data.json");
                return;
            }

            Console.WriteLine($"Loading dialogue from: {dialoguePath}");

            // Load all entities with dialogue from JSON
            var dialogueEntities = loader.LoadFromFile(dialoguePath);

            // Add to game
            foreach (var entity in dialogueEntities)
            {
                game.AddEntity(entity);
                
                if (entity.TryGetComponent<DialogueComponent>(out var dialogue))
                {
                    Console.WriteLine($"✓ Loaded dialogue: {dialogue.EntityName}");
                }
            }

            Console.WriteLine($"Total dialogue entities loaded: {dialogueEntities.Count}");
        }

        /// <summary>
        /// Find the dialogue_data.json file in the project.
        /// </summary>
        private static string? FindDialogueFile()
        {
            // Try multiple common paths
            string[] searchPaths = new[]
            {
                "src/GameCore/dialogue_data.json",           // From project root
                "dialogue_data.json",                         // Current directory
                "../../../src/GameCore/dialogue_data.json",   // From build output
                "Resources/dialogue_data.json",               // Unity resources
                "Assets/Resources/dialogue_data.json"         // Unity full path
            };

            foreach (var path in searchPaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        /// <summary>
        /// Example: Manually add a custom dialogue entity to game.
        /// </summary>
        public static Entity CreateCustomDialogueEntity()
        {
            var entity = new Entity(500);
            entity.AddComponent(new Transform3D { Position = new Vector3(-5, 0, -5) });
            entity.AddComponent(new Tag("NPC"));

            // Create dialogue programmatically (alternative to JSON)
            var dialogue = new DialogueBuilder("Mysterious Stranger")
                .Sequence()
                    .Line("Stranger", "Why do we exist? What is our purpose?", 3f)
                    .Line("Stranger", "These are questions I ponder daily.", 2.5f)
                    .Choice("Philosophy interests me too.", 1)
                    .Choice("I don't have time for this.", 2)
                .Sequence()
                    .Line("Stranger", "Ah, a kindred spirit! Perhaps we shall meet again.")
                .Sequence()
                    .Line("Stranger", "A pity. Another opportunity lost.")
                .BuildFor(entity);

            var trigger = new EncounterTrigger(EncounterTrigger.EncounterType.NPC, dialogue)
            {
                TriggerRange = 8f,
                CanTriggerMultipleTimes = true
            };
            entity.AddComponent(trigger);

            return entity;
        }

        /// <summary>
        /// Example: Load dialogue from JSON and set up UI callbacks.
        /// </summary>
        public static void SetupDialogueUI(DialogueSystem dialogueSystem)
        {
            // Setup display callbacks for dialogue UI
            dialogueSystem.OnDisplayDialogueLine += line =>
            {
                // In a real game, pass this to your UI system
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n[{line.Speaker}] {line.Text}");
                Console.ResetColor();
            };

            dialogueSystem.OnDisplayChoices += (choices, onChoose) =>
            {
                // Display choices
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nChoices:");
                for (int i = 0; i < choices.Count; i++)
                {
                    Console.WriteLine($"  ({i + 1}) {choices[i].ChoiceText}");
                }
                Console.ResetColor();

                // In a real game, wait for player input
                // For this example, just pick the first choice
                // onChoose(0);
            };

            dialogueSystem.OnHideDialogue += () =>
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n[Dialogue ended]\n");
                Console.ResetColor();
            };
        }

        /// <summary>
        /// Example: How to manually trigger a specific dialogue.
        /// </summary>
        public static void TriggerDialogueByName(IEnumerable<Entity> entities, string entityName, DialogueSystem dialogueSystem)
        {
            // Find entity by dialogue component name
            foreach (var entity in entities)
            {
                if (entity.TryGetComponent<DialogueComponent>(out var dialogue) && dialogue.EntityName == entityName)
                {
                    dialogueSystem.StartDialogue(dialogue, 0);
                    return;
                }
            }

            Console.WriteLine($"Entity with dialogue '{entityName}' not found");
        }

        /// <summary>
        /// Example: Complete game setup with dialogue.
        /// NOTE: This example requires AudioBridge which is not available in GameCore.
        /// In a real implementation, provide an IEngineAudio implementation.
        /// </summary>
        public static void CompleteGameSetupExample()
        {
            // Example code (requires IEngineAudio implementation to run):
            // var input = new InputImplementation();
            // var audio = new AudioImplementation();
            // var game = new Game(input, audio);
            // var dialogueSystem = new DialogueSystem(audio);
            //
            // game.AddSystem(dialogueSystem);
            // Console.WriteLine("Game ready with dialogue!");
        }
    }

    /// <summary>
    /// Unity-specific integration example.
    /// </summary>
    public class DialogueManagerUnity
    {
        /// <summary>
        /// Example: How to use DialogueDataLoader in Unity.
        /// Place this on a prefab/game object to load dialogue.
        /// </summary>
        public class DialogueManagerComponent
        {
            private DialogueSystem? _dialogueSystem;
            private List<Entity>? _dialogueEntities;

            public void Initialize(/*MonoBehaviour context*/)
            {
                // Create dialogue system
                // var audioManager = GetComponent<AudioManager>();
                // _dialogueSystem = new DialogueSystem(audioManager.AudioBridge);

                // Setup UI callbacks
                // _dialogueSystem.OnDisplayDialogueLine += DisplayLine;
                // _dialogueSystem.OnDisplayChoices += DisplayChoices;
                // _dialogueSystem.OnHideDialogue += HideLine;

                // Load dialogue data
                // string jsonPath = Path.Combine(Application.persistentDataPath, "dialogue_data.json");
                // if (!File.Exists(jsonPath))
                // {
                //     jsonPath = "dialogue_data.json"; // Try local path
                // }

                // var loader = new DialogueDataLoader(audioManager.AudioBridge);
                // _dialogueEntities = loader.LoadFromFile(jsonPath);

                // Debug.Log($"Loaded {_dialogueEntities.Count} dialogue entities");
            }

            // private void DisplayLine(DialogueLine line)
            // {
            //     // Update UI text
            //     _dialogueText.text = line.Text;
            //     _speakerName.text = line.Speaker;
            //     
            //     // Play audio
            //     if (!string.IsNullOrEmpty(line.AudioClip))
            //     {
            //         var audioClip = Resources.Load<AudioClip>(line.AudioClip);
            //         if (audioClip) _audioSource.PlayOneShot(audioClip);
            //     }
            // }

            // private void DisplayChoices(List<DialogueChoice> choices, Action<int> onChoose)
            // {
            //     // Create choice buttons
            //     foreach (var choice in choices)
            //     {
            //         // Create button UI
            //     }
            // }

            // private void HideLine()
            // {
            //     _dialoguePanel.SetActive(false);
            // }
        }
    }
}
