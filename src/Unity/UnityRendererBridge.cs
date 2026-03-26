using GameCore;
using UnityEngine;

/// <summary>
/// Unity renderer bridge - connects GameCore rendering to Unity's Graphics/UI systems.
/// </summary>
public class UnityRendererBridge : IEngineRenderer
{
    private Camera _mainCamera;
    private Canvas _uiCanvas;
    private Image _screenFlashImage;
    private Material _pixelationMaterial;
    private RenderTexture _pixelatedRT;

    public UnityRendererBridge()
    {
        _mainCamera = Camera.main;
        InitializeScreenFlash();
        InitializePixelationShader();
    }

    private void InitializeScreenFlash()
    {
        // Create canvas for screen flash overlay
        var canvasGO = new GameObject("ScreenFlashCanvas");
        _uiCanvas = canvasGO.AddComponent<Canvas>();
        _uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _uiCanvas.sortingOrder = 100;

        // Create image for flash effect
        var imageGO = new GameObject("FlashImage");
        imageGO.transform.SetParent(_uiCanvas.transform, false);
        _screenFlashImage = imageGO.AddComponent<Image>();
        _screenFlashImage.color = new Color(1, 0, 0, 0);

        // Stretch to full screen
        var rectTransform = imageGO.GetComponent<RectTransform>();
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void InitializePixelationShader()
    {
        // Create material for pixelation post-process
        _pixelationMaterial = new Material(Shader.Find("Hidden/RetroPixelate"));
        if (_pixelationMaterial.shader.name == "Hidden/RetroPixelate")
        {
            Debug.Log("Pixelation shader loaded");
        }
        else
        {
            Debug.LogWarning("RetroPixelate shader not found. Using default material.");
            _pixelationMaterial = new Material(Shader.Find("Standard"));
        }
    }

    public void DrawSprite(string spriteId, float x, float y, float rotation, 
        float scaleX = 1.0f, float scaleY = 1.0f, float opacity = 1.0f)
    {
        // Load mesh from Resources or asset bundle
        var prefab = Resources.Load<GameObject>($"Meshes/{spriteId}");
        if (prefab != null)
        {
            var instance = Object.Instantiate(prefab);
            instance.transform.position = new Vector3(x, y, 0);
            instance.transform.Rotate(0, 0, rotation * Mathf.Rad2Deg);
            instance.transform.localScale = new Vector3(scaleX, scaleY, 1);

            // Set opacity on material
            foreach (var renderer in instance.GetComponentsInChildren<Renderer>())
            {
                var mat = renderer.material;
                mat.SetFloat("_Opacity", opacity);
            }
        }
    }

    public void DrawScreenFlash(float intensity, string color = "red")
    {
        if (_screenFlashImage == null)
            return;

        Color flashColor = color.ToLower() switch
        {
            "red" => Color.red,
            "white" => Color.white,
            "yellow" => Color.yellow,
            "blue" => Color.blue,
            _ => Color.red
        };

        flashColor.a = intensity;
        _screenFlashImage.color = flashColor;
    }

    public void ApplyRetroGraphics(RetroGraphicsEffect effect)
    {
        if (!effect.Enabled || _pixelationMaterial == null || _mainCamera == null)
            return;

        // Set pixelation parameters on material
        _pixelationMaterial.SetFloat("_PixelSize", effect.PixelSize);
        _pixelationMaterial.SetFloat("_ColorBits", effect.ColorBitsPerChannel);
        _pixelationMaterial.SetFloat("_ScanlineOpacity", effect.EnableScanlines ? effect.ScanlineOpacity : 0);
        _pixelationMaterial.SetInt("_UseDithering", effect.EnableDithering ? 1 : 0);

        // Apply post-process effect
        Graphics.Blit(null, null, _pixelationMaterial, 0);
    }

    public int GetScreenWidth()
    {
        return Screen.width;
    }

    public int GetScreenHeight()
    {
        return Screen.height;
    }
}
