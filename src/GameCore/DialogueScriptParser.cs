using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GameCore
{
    /// <summary>
    /// Parses simple dialogue scripts into DialogueSequence objects.
    /// 
    /// Script Format:
    /// 
    /// NPC: OldSage
    ///   "Welcome, traveler!"
    ///   "I seek your help."
    /// 
    /// CHOICE
    ///   [Yes, help me] -> SEQUENCE: quest_line
    ///   [No thanks] -> SEQUENCE: rejection_line
    /// 
    /// SEQ quest_line
    ///   NPC: OldSage
    ///     "Great! Go north to find the crystal."
    ///     (voice_quest.mp3, 3.0)
    /// 
    /// SEQ rejection_line
    ///   NPC: OldSage
    ///     "Perhaps another time..."
    /// </summary>
    public class DialogueScriptParser
    {
        private List<string> _lines;
        private int _currentLine;
        private Dictionary<string, DialogueSequence> _sequences;

        public DialogueScriptParser()
        {
            _sequences = new Dictionary<string, DialogueSequence>();
        }

        /// <summary>Parse dialogue script string into sequences.</summary>
        public Dictionary<string, DialogueSequence> Parse(string script)
        {
            _lines = script.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            _currentLine = 0;
            _sequences.Clear();

            while (_currentLine < _lines.Count)
            {
                string line = GetTrimmedLine();
                if (string.IsNullOrEmpty(line)) continue;

                if (line.StartsWith("SEQ "))
                {
                    ParseSequence(line.Substring(4).Trim());
                }
                else
                {
                    _currentLine++;
                }
            }

            return _sequences;
        }

        /// <summary>Parse a single sequence definition.</summary>
        private void ParseSequence(string sequenceName)
        {
            var sequence = new DialogueSequence();
            _currentLine++;

            while (_currentLine < _lines.Count)
            {
                string line = GetTrimmedLine();
                if (string.IsNullOrEmpty(line))
                {
                    _currentLine++;
                    continue;
                }

                // Check for end of sequence (next SEQ or EOF)
                if (line.StartsWith("SEQ "))
                    break;

                // Parse NPC/Player lines
                if (line.StartsWith("NPC:") || line.StartsWith("PLAYER:"))
                {
                    string speaker = line.Split(':')[0];
                    string speakerName = line.Substring(line.IndexOf(':') + 1).Trim();
                    _currentLine++;

                    // Parse dialogue lines for this speaker
                    while (_currentLine < _lines.Count)
                    {
                        string dialogueLine = GetTrimmedLine();
                        if (string.IsNullOrEmpty(dialogueLine)) break;

                        // Parse quoted dialogue
                        if (dialogueLine.StartsWith("\""))
                        {
                            var (text, audio, duration) = ParseDialogueLine(dialogueLine);
                            sequence.Lines.Add(new DialogueLine(speakerName, text, duration)
                            {
                                AudioClip = audio
                            });
                            _currentLine++;
                        }
                        else if (dialogueLine.StartsWith("CHOICE"))
                        {
                            _currentLine--;
                            break;
                        }
                        else
                        {
                            _currentLine++;
                            break;
                        }
                    }
                }
                // Parse CHOICE branches
                else if (line.StartsWith("CHOICE"))
                {
                    _currentLine++;
                    var choices = new List<DialogueChoice>();

                    while (_currentLine < _lines.Count)
                    {
                        string choiceLine = GetTrimmedLine();
                        if (string.IsNullOrEmpty(choiceLine)) break;

                        if (choiceLine.StartsWith("["))
                        {
                            var (text, targetSeq) = ParseChoice(choiceLine);
                            choices.Add(new DialogueChoice { Text = text, ChoiceIndex = choices.Count });
                            _currentLine++;
                        }
                        else if (!choiceLine.StartsWith("["))
                        {
                            break;
                        }
                    }

                    // Store choices in sequence metadata
                    if (choices.Count > 0)
                    {
                        sequence.Choices = choices;
                    }
                }
                else
                {
                    _currentLine++;
                }
            }

            _sequences[sequenceName] = sequence;
        }

        /// <summary>Parse a dialogue line with optional audio and duration.</summary>
        private (string text, string audio, float duration) ParseDialogueLine(string line)
        {
            // Format: "Text here" or "Text here" (audio.mp3, 2.5)
            string text = "";
            string audio = "";
            float duration = 0f;

            // Extract quoted text
            int firstQuote = line.IndexOf('"');
            int lastQuote = line.LastIndexOf('"');
            if (firstQuote >= 0 && lastQuote > firstQuote)
            {
                text = line.Substring(firstQuote + 1, lastQuote - firstQuote - 1);
            }

            // Extract audio and duration if present
            if (line.Contains("(") && line.Contains(")"))
            {
                int openParen = line.IndexOf('(');
                int closeParen = line.LastIndexOf(')');
                string metadata = line.Substring(openParen + 1, closeParen - openParen - 1);

                // Parse "audio.mp3, 2.5"
                string[] parts = metadata.Split(',');
                if (parts.Length > 0)
                    audio = parts[0].Trim().Trim('"').Trim('\'');

                if (parts.Length > 1 && float.TryParse(parts[1].Trim(), out float dur))
                    duration = dur;
            }

            // Auto-calculate duration from text if not specified
            if (duration == 0f && !string.IsNullOrEmpty(text))
            {
                duration = MathF.Max(2f, text.Length / 15f); // ~15 chars per second
            }

            return (text, audio, duration);
        }

        /// <summary>Parse a choice line.</summary>
        private (string text, string targetSequence) ParseChoice(string line)
        {
            // Format: [Choice text] -> SEQUENCE: seq_name
            string text = "";
            string targetSeq = "";

            int openBracket = line.IndexOf('[');
            int closeBracket = line.IndexOf(']');
            if (openBracket >= 0 && closeBracket > openBracket)
            {
                text = line.Substring(openBracket + 1, closeBracket - openBracket - 1);
            }

            if (line.Contains("->"))
            {
                int arrow = line.IndexOf("->");
                string target = line.Substring(arrow + 2).Trim();
                if (target.StartsWith("SEQUENCE:"))
                {
                    targetSeq = target.Substring(9).Trim();
                }
            }

            return (text, targetSeq);
        }

        private string GetTrimmedLine()
        {
            if (_currentLine >= _lines.Count)
                return "";
            return _lines[_currentLine].Trim();
        }

        /// <summary>Parse dialogue from a file-like string with multiple dialogues.</summary>
        public static Dictionary<string, DialogueSequence> ParseDialogueFile(string fileContent)
        {
            var parser = new DialogueScriptParser();
            return parser.Parse(fileContent);
        }
    }

    /// <summary>Simple dialogue builder for fluent API.</summary>
}
