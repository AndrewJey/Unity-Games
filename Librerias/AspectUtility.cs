using UnityEngine;

// Preserves the serialized field and the public static API used by the original project.
public class AspectUtility : MonoBehaviour
{
    public float _wantedAspectRatio = 1.3333333f;
    private static AspectUtility active;
    private Camera targetCamera;
    private Camera backgroundCamera;
    private Rect originalRect;
    private bool hasOriginalRect;
    private int lastWidth = -1;
    private int lastHeight = -1;
    private float lastAspect;

    void Awake()
    {
        targetCamera = GetComponent<Camera>();
        if (!targetCamera) targetCamera = Camera.main;
        if (!targetCamera)
        {
            Debug.LogError("AspectUtility requires a camera on this object or a MainCamera.", this);
            enabled = false;
            return;
        }
        originalRect = targetCamera.rect;
        hasOriginalRect = true;
    }

    void OnEnable()
    {
        if (!targetCamera) return;
        active = this;
        RefreshCamera();
    }

    void LateUpdate()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight || !_wantedAspectRatio.Equals(lastAspect))
            RefreshCamera();
        if (backgroundCamera && targetCamera) backgroundCamera.enabled = targetCamera.enabled;
    }

    private void RefreshCamera()
    {
        if (!targetCamera) return;
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        lastAspect = _wantedAspectRatio;
        // Minimized/uninitialized windows must not cause division by zero.
        if (lastWidth <= 0 || lastHeight <= 0) return;
        float aspect = _wantedAspectRatio;
        if (aspect <= 0 || float.IsNaN(aspect) || float.IsInfinity(aspect))
        {
            Debug.LogWarning("AspectUtility: invalid aspect ratio; using 4:3.", this);
            aspect = 4f / 3f;
        }
        AspectViewport viewport = AspectViewport.Calculate(lastWidth, lastHeight, aspect);
        targetCamera.rect = new Rect(viewport.X, viewport.Y, viewport.Width, viewport.Height);
        if (viewport.Width == 1 && viewport.Height == 1)
        {
            RemoveBackground();
            return;
        }
        if (!backgroundCamera)
        {
            GameObject background = new GameObject("AspectUtility Background", typeof(Camera));
            background.hideFlags = HideFlags.HideAndDontSave;
            backgroundCamera = background.GetComponent<Camera>();
            backgroundCamera.depth = int.MinValue;
            backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
            backgroundCamera.backgroundColor = Color.black;
            backgroundCamera.cullingMask = 0;
        }
        backgroundCamera.enabled = targetCamera.enabled;
    }

    private void RemoveBackground()
    {
        if (!backgroundCamera) return;
        // Destroy is deferred; disable first so it cannot render another frame.
        backgroundCamera.enabled = false;
        Destroy(backgroundCamera.gameObject);
        backgroundCamera = null;
    }

    void OnDisable()
    {
        RemoveBackground();
        if (targetCamera && hasOriginalRect) targetCamera.rect = originalRect;
        if (active == this) active = null;
    }

    public static void SetCamera()
    {
        if (active) active.RefreshCamera();
    }

    public static Rect screenRect
    {
        get
        {
            Rect viewport = active && active.targetCamera ? active.targetCamera.rect : new Rect(0, 0, 1, 1);
            return new Rect(viewport.x * Screen.width, viewport.y * Screen.height,
                viewport.width * Screen.width, viewport.height * Screen.height);
        }
    }
    public static int screenHeight { get { return (int)screenRect.height; } }
    public static int screenWidth { get { return (int)screenRect.width; } }
    public static int xOffset { get { return (int)screenRect.x; } }
    public static int yOffset { get { return (int)screenRect.y; } }

    public static Vector3 mousePosition
    {
        get
        {
            Vector3 position = Input.mousePosition;
            Rect area = screenRect;
            position.x -= (int)area.x;
            position.y -= (int)area.y;
            return position;
        }
    }
    public static Vector2 guiMousePosition
    {
        get
        {
            // Event.current is null outside OnGUI. Preserve global GUI coordinates.
            Vector2 position = Event.current != null ? Event.current.mousePosition :
                new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            Rect area = screenRect;
            position.x = Mathf.Clamp(position.x, area.x, area.x + area.width);
            position.y = Mathf.Clamp(position.y, area.y, area.y + area.height);
            return position;
        }
    }
}
