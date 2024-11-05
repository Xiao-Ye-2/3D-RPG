using System;
using System.Diagnostics;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;
    public event Action<Vector3> onMouseClickGround;
    public event Action<GameObject> onMouseClickEnemy;
    public Texture2D point, doorway, attack, target, arrow;
    private RaycastHit hitInfo;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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
            switch(hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(0, 0), CursorMode.Auto);
                    break;
            }
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
            else if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                onMouseClickEnemy?.Invoke(hitInfo.collider.gameObject);
            }
        }
    }
}
