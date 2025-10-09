using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class MultiSOViewerWindow : EditorWindow
{
    // [SerializeField] 변수들은 사용하지 않으므로 제거하거나 주석 처리해도 됩니다.
    [SerializeField] private VisualTreeAsset visualTree;
    [SerializeField] private StyleSheet styleSheet;

    private VisualElement dropArea;
    private ScrollView contentContainer;

    [MenuItem("Window/Custom Tools/Multi SO Viewer")]
    public static void ShowWindow()
    {
        MultiSOViewerWindow wnd = GetWindow<MultiSOViewerWindow>();
        wnd.titleContent = new GUIContent("Multi SO Viewer");
    }

    public void CreateGUI()
    {
        // 1. UXML 로드 및 복제
        if (visualTree != null)
            visualTree.CloneTree(rootVisualElement);
        else
        {
            string visualTreeAssetPath = AssetDatabase.GUIDToAssetPath("e0431c2f2aad437c94b21ae27fd0d58b");
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreeAssetPath);
            if (uxml)
            {
                visualTree = uxml;
                visualTree.CloneTree(rootVisualElement);
            }
            else
            {
                rootVisualElement.Add(new Label("UXML 파일을 찾을 수 없습니다. GUID를 확인하세요."));
                return;
            }
        }

        // 2. USS 스타일시트 로드 및 적용
        if (styleSheet != null)
        {
            rootVisualElement.styleSheets.Add(styleSheet);
        }
        else
        {
            string styleSheetPath = AssetDatabase.GUIDToAssetPath("29c9dcb331264331a8e4d5d2d56b4c63");
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
            if (uss)
            {
                styleSheet = uss;
                rootVisualElement.styleSheets.Add(styleSheet);
            }
            else
            {
                rootVisualElement.Add(new Label("USS 파일을 찾을 수 없습니다. GUID를 확인하세요."));
                return;
            }
        }
        
        // 3. UI 요소 참조 가져오기
        dropArea = rootVisualElement.Q<VisualElement>("drop-area");
        contentContainer = rootVisualElement.Q<ScrollView>("content-container");

        // 4. 드래그 앤 드롭 이벤트 콜백 등록
        dropArea.RegisterCallback<DragEnterEvent>(OnDragEnter);
        dropArea.RegisterCallback<DragLeaveEvent>(OnDragLeave);
        dropArea.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        dropArea.RegisterCallback<DragPerformEvent>(OnDragPerform);
    }
    
    private void OnDragEnter(DragEnterEvent evt)
    {
        if (DragAndDrop.objectReferences.Any(obj => obj is ScriptableObject))
        {
            dropArea.AddToClassList("drag-over");
        }
    }
    
    private void OnDragLeave(DragLeaveEvent evt)
    {
        dropArea.RemoveFromClassList("drag-over");
    }

    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        bool hasScriptableObject = DragAndDrop.objectReferences.Any(obj => obj is ScriptableObject);
        DragAndDrop.visualMode = hasScriptableObject ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
    }

    private void OnDragPerform(DragPerformEvent evt)
    {
        dropArea.RemoveFromClassList("drag-over");
        DragAndDrop.AcceptDrag(); 

        var scriptableObjects = DragAndDrop.objectReferences.OfType<ScriptableObject>();

        foreach (var so in scriptableObjects)
        {
            var soContainer = new VisualElement();
            soContainer.AddToClassList("so-container");

            // --- 여기가 수정된 부분입니다 ---
            var header = new Label(); // 1. 빈 Label 생성
            header.enableRichText = true; // 2. 리치 텍스트 활성화
            // 3. 텍스트 할당 (이 시점에 태그가 해석됨)
            header.text = $"<b>{so.name}</b>";
            header.AddToClassList("so-header");
            soContainer.Add(header);
            // -----------------------------

            Editor editor = Editor.CreateEditor(so);
            var imguiContainer = new IMGUIContainer(() => editor.OnInspectorGUI());
            soContainer.Add(imguiContainer);

            contentContainer.Add(soContainer);
        }
    }
}