using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameCore
{
    /// <summary>
    /// Load dialogue data from JSON files and create game entities with dialogue.
    /// </summary>
    public class DialogueDataLoader
    {
        private readonly IEngineAudio _audio;

        // JSON model classes for deserialization
        public class DialogueData
        {
            [JsonPropertyName("entities")]
            public List<DialogueEntityData> Entities { get; set; } = new();
        }

        public class DialogueEntityData
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }  // "NPC", "Enemy", "Boss"

            [JsonPropertyName("triggerRange")]
            public float TriggerRange { get; set; } = 10f;

            [JsonPropertyName("canRepeat")]
            public bool CanRepeat { get; set; } = true;

            [JsonPropertyName("startCombatAfter")]
            public bool StartCombatAfter { get; set; } = false;

            [JsonPropertyName("sequences")]
            public List<DialogueSequenceData> Sequences { get; set; } = new();
        }

        public class DialogueSequenceData
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("lines")]
            public List<DialogueLineData> Lines { get; set; } = new();

            [JsonPropertyName("choices")]
            public List<DialogueChoiceData> Choices { get; set; } = new();

            [JsonPropertyName("nextSequence")]
            public int NextSequence { get; set; } = -1;
        }

        public class DialogueLineData
        {
            [JsonPropertyName("speaker")]
            public string Speaker { get; set; }

            [JsonPropertyName("text")]
            public string Text { get; set; }

            [JsonPropertyName("duration")]
            public float Duration { get; set; } = 0;

            [JsonPropertyName("audioFile")]
            public string AudioFile { get; set; }
        }

        public class DialogueChoiceData
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }

            [JsonPropertyName("nextSequence")]
            public int NextSequence { get; set; } = -1;
        }

        public DialogueDataLoader(IEngineAudio audio)
        {
            _audio = audio;
        }

        /// <summary>
        /// Load dialogue from a JSON file and create entities.
        /// </summary>
        public List<Entity> LoadFromFile(string filePath)
        {
            var entities = new List<Entity>();

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Dialogue file not found: {filePath}");
                    return entities;
                }

                var json = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<DialogueData>(json, options);

                if (data?.Entities != null)
                {
                    int entityIdCounter = 1000; // Start from higher ID to avoid conflicts
                    foreach (var entityData in data.Entities)
                    {
                        var entity = CreateEntityFromData(entityData, entityIdCounter++);
                        if (entity != null)
                        {
                            entities.Add(entity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dialogue file: {ex.Message}");
            }

            return entities;
        }

        /// <summary>
        /// Create a single entity from dialogue data.
        /// </summary>
        private Entity CreateEntityFromData(DialogueEntityData data, int entityId)
        {
            var entity = new Entity(entityId);

            // Add transform (default position, can be overridden)
            entity.AddComponent(new Transform3D { Position = Vector3.Zero });

            // Add tag for entity type
            entity.AddComponent(new Tag(data.Type));

            // Create dialogue component
            var dialogue = new DialogueComponent(data.Name);

            // Add sequences
            foreach (var seqData in data.Sequences)
            {
                var sequence = new DialogueSequence(seqData.Id) { NextSequenceIndex = seqData.NextSequence };

                // Add lines
                foreach (var lineData in seqData.Lines)
                {
                    var line = new DialogueLine(lineData.Speaker, lineData.Text, lineData.Duration)
                    {
                        AudioClip = lineData.AudioFile
                    };
                    sequence.AddLine(line);
                }

                // Add choices
                foreach (var choiceData in seqData.Choices)
                {
                    var choice = new DialogueChoice(choiceData.Text, choiceData.NextSequence);
                    sequence.AddChoice(choice);
                }

                dialogue.Sequences[sequence.Id] = sequence;
            }

            entity.AddComponent(dialogue);

            // Add encounter trigger
            EncounterTrigger.EncounterType encounterType = data.Type switch
            {
                "NPC" => EncounterTrigger.EncounterType.NPC,
                "Enemy" => EncounterTrigger.EncounterType.Enemy,
                "Boss" => EncounterTrigger.EncounterType.Boss,
                _ => EncounterTrigger.EncounterType.NPC
            };

            var trigger = new EncounterTrigger(encounterType, dialogue)
            {
                TriggerRange = data.TriggerRange,
                CanTriggerMultipleTimes = data.CanRepeat,
                StartCombatAfter = data.StartCombatAfter,
                InitialDialogueSequence = 0
            };

            entity.AddComponent(trigger);

            return entity;
        }

        /// <summary>
        /// Generate a template JSON file with example structure.
        /// </summary>
        public static string GenerateTemplate()
        {
            return @"{
  ""entities"": [
    {
      ""id"": ""your_entity_id"",
      ""name"": ""Character Name"",
      ""type"": ""NPC"",
      ""triggerRange"": 10.0,
      ""canRepeat"": true,
      ""startCombatAfter"": false,
      ""sequences"": [
        {
          ""id"": 0,
          ""name"": ""sequence_name"",
          ""lines"": [
            {
              ""speaker"": ""Character Name"",
              ""text"": ""This is the first line of dialogue."",
              ""duration"": 2.5,
              ""audioFile"": ""dialogue/character_line_01.wav""
            }
          ],
          ""choices"": [
            {
              ""text"": ""Choice text shown to player"",
              ""nextSequence"": 1
            }
          ],
          ""nextSequence"": -1
        }
      ]
    }
  ]
}";
        }
    }

    /// <summary>
    /// Helper for working with dialogue data programmatically.
    /// </summary>
    public class DialogueDataHelper
    {
        /// <summary>
        /// Create a simple dialogue data structure for an NPC.
        /// </summary>
        public static DialogueDataLoader.DialogueEntityData CreateNPCData(
            string id,
            string name,
            List<(string speaker, string text, float duration, string audioFile)> lines,
            float triggerRange = 10f)
        {
            var entity = new DialogueDataLoader.DialogueEntityData
            {
                Id = id,
                Name = name,
                Type = "NPC",
                TriggerRange = triggerRange,
                CanRepeat = true
            };

            var sequence = new DialogueDataLoader.DialogueSequenceData { Id = 0, Name = "greeting" };

            foreach (var (speaker, text, duration, audioFile) in lines)
            {
                sequence.Lines.Add(new()
                {
                    Speaker = speaker,
                    Text = text,
                    Duration = duration,
                    AudioFile = audioFile
                });
            }

            entity.Sequences.Add(sequence);
            return entity;
        }

        /// <summary>
        /// Create dialogue data for an enemy encounter.
        /// </summary>
        public static DialogueDataLoader.DialogueEntityData CreateEnemyData(
            string id,
            string name,
            List<(string speaker, string text, float duration, string audioFile)> lines,
            float triggerRange = 15f)
        {
            var entity = new DialogueDataLoader.DialogueEntityData
            {
                Id = id,
                Name = name,
                Type = "Enemy",
                TriggerRange = triggerRange,
                CanRepeat = false,
                StartCombatAfter = true
            };

            var sequence = new DialogueDataLoader.DialogueSequenceData { Id = 0, Name = "encounter" };

            foreach (var (speaker, text, duration, audioFile) in lines)
            {
                sequence.Lines.Add(new()
                {
                    Speaker = speaker,
                    Text = text,
                    Duration = duration,
                    AudioFile = audioFile
                });
            }

            entity.Sequences.Add(sequence);
            return entity;
        }
    }
}
