using UnityEngine;
using UnityEngine.EventSystems; // UI 이벤트를 사용하기 위해 필요
using UnityEngine.UI;           // GraphicRaycaster를 사용하기 위해 필요
using System.Collections.Generic; // List를 사용하기 위해 필요

public class ClickDetector : MonoBehaviour
{
    // 인스펙터 창에서 Canvas를 연결해줘야 합니다.
    public Canvas canvas; 

    private GraphicRaycaster graphicRaycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    void Start()
    {
        // Canvas에 붙어있는 GraphicRaycaster 컴포넌트를 가져옵니다.
        if (canvas != null)
        {
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        }
        else
        {
            Debug.LogError("ClickDetector: Canvas가 할당되지 않았습니다! 인스펙터에서 Canvas를 연결해주세요.");
        }
        
        // 씬에 있는 EventSystem을 가져옵니다.
        eventSystem = EventSystem.current; 
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 클릭했을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 1. UI를 먼저 체크합니다.
            if (IsPointerOverUIObject())
            {
                // UI가 클릭되었으므로 여기서 함수를 종료합니다.
                // 이렇게 해야 UI 뒤에 있는 3D/2D 오브젝트가 클릭되지 않습니다.
                return;
            }

            // 2. UI가 클릭되지 않았다면 3D 오브젝트를 체크합니다.
            if (CheckFor3DObject())
            {
                return; // 3D 오브젝트가 클릭되었으므로 종료
            }

            // 3. 3D 오브젝트도 없었다면 2D 오브젝트를 체크합니다.
            CheckFor2DObject();
        }
    }

    /// <summary>
    /// 마우스 포인터 아래에 UI 요소가 있는지 확인하고, 있다면 어떤 UI인지 출력합니다.
    /// </summary>
    private bool IsPointerOverUIObject()
    {
        if (graphicRaycaster == null || eventSystem == null) return false;

        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            // 가장 위에 있는 UI 요소의 이름을 출력합니다.
            Debug.Log("UI 클릭: " + results[0].gameObject.name);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 마우스 클릭 위치에 3D 오브젝트가 있는지 확인하고, 있다면 어떤 오브젝트인지 출력합니다.
    /// </summary>
    private bool CheckFor3DObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Raycast에 맞은 3D 오브젝트의 이름을 출력합니다.
            Debug.Log("3D 오브젝트 클릭: " + hit.collider.gameObject.name);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 마우스 클릭 위치에 2D 오브젝트가 있는지 확인하고, 있다면 어떤 오브젝트인지 출력합니다.
    /// </summary>
    private void CheckFor2DObject()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit2D.collider != null)
        {
            // Raycast에 맞은 2D 오브젝트의 이름을 출력합니다.
            Debug.Log("2D 오브젝트 클릭: " + hit2D.collider.gameObject.name);
        }
    }
}