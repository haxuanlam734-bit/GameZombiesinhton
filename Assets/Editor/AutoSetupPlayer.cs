using UnityEngine;
using UnityEditor;

public class AutoSetupPlayer
{
    // 1. Tạo 1 Menu Item trên thanh công cụ Unity: `Tools` -> `Auto Setup Player`.
    [MenuItem("Tools/Auto Setup Player")]
    public static void SetupPlayer()
    {
        // 2. Khi bấm vào menu này, code sẽ tự động:
        
        // Tìm GameObject tên "Player" trong Scene (nếu chưa có thì tự tạo 1 Capsule 3D tên "Player").
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            // Điều chỉnh vị trí capsule để chân chạm đất tại y=0
            player.transform.position = new Vector3(0, 1, 0); 
            Debug.Log("Không tìm thấy 'Player', đã tạo một Capsule mới.");
        }

        // Tự động gắn Component `CharacterController` và `CrossPlatformPlayerController` vào Player nếu chưa có.
        CharacterController charController = player.GetComponent<CharacterController>() ?? player.AddComponent<CharacterController>();
        CrossPlatformPlayerController playerController = player.GetComponent<CrossPlatformPlayerController>() ?? player.AddComponent<CrossPlatformPlayerController>();
        
        // Điều chỉnh CharacterController cho hợp lý với Capsule
        charController.center = new Vector3(0, 1, 0);
        charController.height = 2;
        charController.radius = 0.5f;

        // Tự động tạo con `CameraHolder` (nằm ở tọa độ local 0,0,0).
        Transform cameraHolder = player.transform.Find("CameraHolder");
        if (cameraHolder == null)
        {
            GameObject holderObj = new GameObject("CameraHolder");
            cameraHolder = holderObj.transform;
            cameraHolder.SetParent(player.transform);
            cameraHolder.localPosition = Vector3.zero;
        }

        // Tự động tạo 2 con của CameraHolder là `FPS_Point` và `TPS_Point`.
        // FPS_Point (tọa độ local 0, 0.6f, 0 - ngang tầm mắt)
        Transform fpsPoint = cameraHolder.Find("FPS_Point");
        if (fpsPoint == null)
        {
            GameObject pointObj = new GameObject("FPS_Point");
            fpsPoint = pointObj.transform;
            fpsPoint.SetParent(cameraHolder);
            fpsPoint.localPosition = new Vector3(0, 0.6f, 0);
        }

        // TPS_Point (tọa độ local 0.5f, 1.2f, -2.5f - đằng sau vai).
        Transform tpsPoint = cameraHolder.Find("TPS_Point");
        if (tpsPoint == null)
        {
            GameObject pointObj = new GameObject("TPS_Point");
            tpsPoint = pointObj.transform;
            tpsPoint.SetParent(cameraHolder);
            tpsPoint.localPosition = new Vector3(0.5f, 1.2f, -2.5f);
        }

        // Tự động tìm `Main Camera` trong Scene và kéo nó làm con của `CameraHolder`.
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(cameraHolder.transform);
            mainCamera.transform.localPosition = Vector3.zero; // Camera sẽ tự di chuyển đến FPS/TPS point
            mainCamera.transform.localRotation = Quaternion.identity;
            Debug.Log("Đã di chuyển Main Camera vào làm con của CameraHolder.");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy 'Main Camera' trong Scene! Vui lòng tạo một camera.");
        }

        // Tự động gán (assign) các biến vào đúng các ô trong Inspector.
        // Dùng SerializedObject để thay đổi giá trị trong Inspector một cách an toàn.
        SerializedObject so = new SerializedObject(playerController);
        
        so.FindProperty("cameraHolder").objectReferenceValue = cameraHolder;
        so.FindProperty("fpsCameraPoint").objectReferenceValue = fpsPoint;
        so.FindProperty("tpsCameraPoint").objectReferenceValue = tpsPoint;

        so.ApplyModifiedProperties(); // Lưu lại các thay đổi

        // 3. Hiện thông báo khi hoàn tất.
        Debug.Log("Dựng cấu trúc Player thành công!");
        
        // Chọn GameObject Player trong Hierarchy để người dùng thấy kết quả ngay
        Selection.activeGameObject = player;
    }
}
