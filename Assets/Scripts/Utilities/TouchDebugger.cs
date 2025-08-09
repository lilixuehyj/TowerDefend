using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;
            Debug.Log("点击屏幕位置：" + pos);

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = pos;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count == 0)
            {
                Debug.Log("没有命中任何 UI");
            }
            else
            {
                foreach (var result in results)
                {
                    Debug.Log("命中：" + result.gameObject.name);
                }
            }
        }
    }
}
