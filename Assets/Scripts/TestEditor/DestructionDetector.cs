// --- START OF NEW FILE DestructionDetector.cs ---
using UnityEngine;
using Unity.Netcode;

public class DestructionDetector : NetworkBehaviour
{
    // OnDestroy는 Unity에 의해 오브젝트가 파괴될 때 호출되는 특별한 함수입니다.
    private void OnDestroy()
    {
        // 클라이언트에서만, 그리고 애플리케이션이 종료되는 상황이 아닐 때 로그를 남깁니다.
        // NetworkManager가 null이 아니라는 것은 게임이 아직 실행 중임을 의미합니다.
        if (!IsServer && NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
        {
            // 이 로그가 콘솔에 찍힌다면, 클라이언트에서 이 오브젝트가 파괴되었다는 확실한 증거입니다.
            Debug.LogError($"====== DESTRUCTION DETECTED on {gameObject.name}! NetworkObjectId: {NetworkObjectId} ======");
            Debug.LogError("오브젝트가 파괴되었습니다. 아래 'Call Stack'을 확인하여 어떤 스크립트가 Destroy를 호출했는지 추적하세요.");

            // 에디터에서 실행 중일 때, 이 코드는 파괴가 일어나는 바로 그 순간에 에디터를 일시정지 시킵니다.
            // 이를 통해 Call Stack을 분석할 수 있습니다.
#if UNITY_EDITOR
            Debug.Break();
#endif
        }
    }
}