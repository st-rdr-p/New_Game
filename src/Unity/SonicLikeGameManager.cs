using GameCore;
using UnityEngine;

/// <summary>
/// Main game manager for Unity - handles GameCore initialization and update loop.
/// </summary>
public class SonicLikeGameManager : MonoBehaviour
{
    [SerializeField] private bool autoInitialize = true;
    
    private Game _game;
    private UnityInputBridge _inputBridge;
    private UnityRendererBridge _rendererBridge;
    private UnityAudioBridge _audioBridge;

    private void Start()
    {
        if (autoInitialize)
        {
            InitializeGame();
        }
    }

    public void InitializeGame()
    {
        if (_game != null)
        {
            Debug.LogWarning("Game already initialized!");
            return;
        }

        Debug.Log("Initializing Sonic-like game...");

        // Create engine bridges
        _inputBridge = new UnityInputBridge();
        _rendererBridge = new UnityRendererBridge();
        _audioBridge = new UnityAudioBridge();

        // Preload common sounds
        _audioBridge.PreloadSounds(
            "collect_ring", "collect_coin", "collect_healthpickup",
            "hazard_spike", "hazard_lava", "hazard_pit",
            "player_hit", "enemy_defeat"
        );

        // Create game instance
        _game = new Game(_inputBridge);

        // Setup the game with all entities and systems
        GameSetup.SetupSonicLikeGame(_game, _inputBridge, _rendererBridge, _audioBridge);

        Debug.Log("Game initialized successfully!");
    }

    private void Update()
    {
        if (_game == null)
            return;

        // Run one frame of the game simulation
        _game.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (_game == null)
            return;

        // Post-update cleanup or final rendering
        // (Can add additional rendering or cleanup here)
    }

    public Game GetGame()
    {
        return _game;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        _inputBridge?.LockMouse(false);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        _inputBridge?.LockMouse(true);
    }

    private void OnDestroy()
    {
        // Cleanup
        _game = null;
    }
}
