using UnityEngine;

public class IMGUI : MonoBehaviour
{
    public float health = 0.5f;

    void OnGUI()
    {
        //计算血条位置
        Vector3 worldPos = new Vector3(transform.position.x, transform.position.y -0.2f, transform.position.z);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        //生成HorizontalScrollbar
        GUI.HorizontalScrollbar(new Rect(new Rect(screenPos.x - 100, screenPos.y, 200, 20)), 0.0f, health, 0.0f, 1.0f);
    }
}