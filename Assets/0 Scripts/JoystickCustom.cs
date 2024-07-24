using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickCustom : MonoBehaviour {
    public static Vector2 posInput;
    public Camera cam;
    public Vector2 pointBegin, pointEnd;

    void Update() {
        if (Input.GetMouseButtonDown(0))
            pointBegin = Input.mousePosition;
        if (Input.GetMouseButton(0))
            pointEnd = Input.mousePosition;
        if (Input.GetMouseButtonUp(0))
            posInput = pointBegin = pointEnd = Vector2.zero;
        posInput = (pointEnd - pointBegin).normalized;
    }

    public static float Horizontal() {
        return posInput.x;
    }

    public static float Vertical() {
        return posInput.y;
    }
}
