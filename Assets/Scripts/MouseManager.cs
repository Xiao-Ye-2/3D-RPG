using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EventVector3: UnityEvent<Vector3> {}
public class MouseManager : MonoBehaviour
{
    public EventVector3 onMouseClickGround;
    private RaycastHit hitInfo;

    void Start()
    {

    }

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    private void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            // if (hitInfo.collider.gameObject.tag == "Floor")
            // {
            //     Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Target"), new Vector2(16, 16), CursorMode.Auto);
            // }
            // else
            // {
            //     Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Attack"), new Vector2(16, 16), CursorMode.Auto);
            // }
        }
    }

    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                onMouseClickGround?.Invoke(hitInfo.point);
            }
        }
    }
}
