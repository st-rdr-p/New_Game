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

    public void DrawHealthBar(Vector3 worldPosition, float currentHealth, float maxHealth, 
        float width, float height, string fillColor = "green", string backgroundColor = "gray", 
        float opacity = 1.0f)
    {
        // Convert world position to screen space
        if (_mainCamera == null)
            return;

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPosition);
        
        // Draw background bar
        DrawUIRectangle(screenPos, width, height, backgroundColor, opacity);
        
        // Draw fill bar
        float fillPercent = Mathf.Clamp01(currentHealth / maxHealth);
        float fillWidth = width * fillPercent;
        DrawUIRectangle(screenPos, fillWidth, height, fillColor, opacity);
    }

    public void DrawUIText(string text, UIElement.AnchorPosition anchor, Vector3 offset, 
        int fontSize, string color, float opacity)
    {
        // This would typically use Unity's TextMesh Pro or UI Text
        // For now, we'll use Debug.Log as a placeholder
        Debug.Log($"DrawUIText: {text} at {anchor} with color {color}");
        
        // In a real implementation:
        // 1. Create/reuse a Text element in the canvas
        // 2. Position it based on anchor and offset
        // 3. Set font size and color
        // 4. Apply opacity via alpha
    }

    public void DrawCrosshair(string style, float size, string color, float thickness, float opacity)
    {
        // Draw crosshair at screen center
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        DrawCrosshairAtPosition(screenCenter, style, size, color, thickness, opacity);
    }

    public void DrawMinimap(float size, float worldRange, UIElement.AnchorPosition anchor, 
        Vector3 offset, string backgroundColor, string borderColor, float opacity)
    {
        // Calculate minimap position based on anchor
        Vector2 minimapPos = CalculateAnchoredPosition(anchor, offset);
        
        // Draw minimap background
        DrawUIRectangle(minimapPos, size, size, backgroundColor, opacity);
        
        // Draw minimap border
        DrawUIBorder(minimapPos, size, size, borderColor, 2f, opacity);
    }

    private void DrawUIRectangle(Vector3 position, float width, float height, string color, float opacity)
    {
        // Placeholder for UI rectangle drawing
        // In a real implementation, this would draw a rectangle on the canvas
    }

    private void DrawUIBorder(Vector2 position, float width, float height, string color, float thickness, float opacity)
    {
        // Placeholder for UI border drawing
    }

    private void DrawCrosshairAtPosition(Vector3 screenPos, string style, float size, string color, float thickness, float opacity)
    {
        // Placeholder for crosshair drawing
        // Styles: "cross", "dot", "circle", "target"
    }

    private Vector2 CalculateAnchoredPosition(UIElement.AnchorPosition anchor, Vector3 offset)
    {
        int screenWidth = GetScreenWidth();
        int screenHeight = GetScreenHeight();
        Vector2 anchorPos = anchor switch
        {
            UIElement.AnchorPosition.TopLeft => new Vector2(0, screenHeight),
            UIElement.AnchorPosition.TopCenter => new Vector2(screenWidth / 2f, screenHeight),
            UIElement.AnchorPosition.TopRight => new Vector2(screenWidth, screenHeight),
            UIElement.AnchorPosition.MiddleLeft => new Vector2(0, screenHeight / 2f),
            UIElement.AnchorPosition.MiddleCenter => new Vector2(screenWidth / 2f, screenHeight / 2f),
            UIElement.AnchorPosition.MiddleRight => new Vector2(screenWidth, screenHeight / 2f),
            UIElement.AnchorPosition.BottomLeft => new Vector2(0, 0),
            UIElement.AnchorPosition.BottomCenter => new Vector2(screenWidth / 2f, 0),
            UIElement.AnchorPosition.BottomRight => new Vector2(screenWidth, 0),
            _ => new Vector2(0, screenHeight)
        };

        return anchorPos + new Vector2(offset.x, offset.y);
    }
}
