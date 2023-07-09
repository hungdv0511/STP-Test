using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{

    #region Simulations
    public enum SimDevice
    {
        None,
        AdSpace_Bottom,
        AdSpace_Top
    }

    public static SimDevice Sim = SimDevice.None;
    Rect[] NSA_AdSpace_Bottom = new Rect[]
    {
            //new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
            new Rect (0f, 150f / 1080f, 0f, 1 - (150f / 1080f))  // Landscape
    };

    Rect[] NSA_AdSpace_Top = new Rect[]
    {
            new Rect (0f, 0, 0f, 1 - (150f / 1080f))  // Landscape
    };
    #endregion

    RectTransform Panel;
    Rect LastSafeArea = new Rect(0, 0, 0, 0);
    Vector2Int LastScreenSize = new Vector2Int(0, 0);
    //ScreenOrientation LastOrientation = ScreenOrientation.AutoRotation;
    [SerializeField] bool ConformX = true;  // Conform to screen safe area on X-axis (default true, disable to ignore)
    [SerializeField] bool ConformY = true;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
    [SerializeField] bool Logging = false;  // Conform to screen safe area on Y-axis (default true, disable to ignore)

    void Awake()
    {
        Panel = GetComponent<RectTransform>();
        //if (Panel == null)
        //{
        //    Debug.LogError ("Cannot apply safe area - no RectTransform found on " + name);
        //    Destroy (gameObject);
        //}
        //Refresh ();
    }

    private void OnEnable()
    {
        Refresh();
    }
    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        Rect safeArea = GetSafeArea();
        if (safeArea != LastSafeArea
            || Screen.width != LastScreenSize.x
            || Screen.height != LastScreenSize.y) //|| Screen.orientation != LastOrientation)
        {
            // Fix for having auto-rotate off and manually forcing a screen orientation.
            // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
            LastScreenSize.x = Screen.width;
            LastScreenSize.y = Screen.height;
            //LastOrientation = Screen.orientation;
            ApplySafeArea(safeArea);
        }
    }
    Rect safeArea;
    Rect GetSafeArea()
    {
        safeArea = Screen.safeArea;
        if (Sim != SimDevice.None)// && Application.isEditor)
        {
            Rect nsa = new Rect(0, 0, Screen.width, Screen.height);
            switch (Sim)
            {
                case SimDevice.AdSpace_Bottom:
                    if (Screen.height <= Screen.width)  // Landscape
                        nsa = NSA_AdSpace_Bottom[0];
                    break;

                case SimDevice.AdSpace_Top:
                    if (Screen.height <= Screen.width)  // Landscape
                        nsa = NSA_AdSpace_Top[0];
                    break;

                default:
                    break;
            }
            safeArea = new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);
        }
        return safeArea;
    }

    void ApplySafeArea(Rect r)
    {
        LastSafeArea = r;

        // Ignore x-axis?
        if (!ConformX)
        {
            r.x = 0;
            r.width = Screen.width;
        }

        // Ignore y-axis?
        if (!ConformY)
        {
            r.y = 0;
            r.height = Screen.height;
        }

        // Check for invalid screen startup state on some Samsung devices (see below)
        if (Screen.width > 0 && Screen.height > 0)
        {
            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            // Fix for some Samsung devices (e.g. Note 10+, A71, S20) where Refresh gets called twice and the first time returns NaN anchor coordinates
            // See https://forum.unity.com/threads/569236/page-2#post-6199352
            if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
            {
                Panel.anchorMin = anchorMin;
                Panel.anchorMax = anchorMax;
            }
        }

        if (Logging)
        {
            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }
}
