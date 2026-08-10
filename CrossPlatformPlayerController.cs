using UnityEngine;

// Yêu cầu Game Object phải có component CharacterController.
[RequireComponent(typeof(CharacterController))]
public class CrossPlatformPlayerController : MonoBehaviour
{
    #region Biến (Variables)

    [Header("Thiết lập Đa Nền Tảng")]
    [Tooltip("Tick vào đây để thử nghiệm giao diện mobile trên PC.")]
    public bool testMobileOnPC = false;
    [Tooltip("Kéo Canvas chứa các nút bấm điều khiển cho mobile vào đây.")]
    public GameObject mobileTouchCanvas;

    [Header("Di Chuyển")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [Tooltip("Kéo Fixed Joystick từ asset vào đây.")]
    [SerializeField] private FixedJoystick joystick;

    [Header("Góc Nhìn Camera")]
    [SerializeField] private float lookSensitivity = 100f;
    [Tooltip("Transform chứa camera để xoay lên/xuống.")]
    [SerializeField] private Transform cameraHolder;
    [Tooltip("Điểm đặt camera cho góc nhìn thứ nhất (First Person).")]
    [SerializeField] private Transform fpsCameraPoint;
    [Tooltip("Điểm đặt camera cho góc nhìn thứ ba (Third Person).")]
    [SerializeField] private Transform tpsCameraPoint;

    // Biến private để xử lý logic nội bộ
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool isMobilePlatform;
    private Camera mainCamera;
    private bool isFPS = true; // Bắt đầu với góc nhìn FPS
    private float xRotation = 0f;

    #endregion

    #region Vòng Đời Unity (Unity Lifecycle)

    private void Start()
    {
        // Lấy các component cần thiết
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        // --- 3. Tự động bật/tắt UI theo nền tảng ---
        isMobilePlatform = Application.isMobilePlatform || testMobileOnPC;

        if (mobileTouchCanvas != null)
        {
            mobileTouchCanvas.SetActive(isMobilePlatform);
        }

        if (!isMobilePlatform)
        {
            // Trên PC, khóa con trỏ chuột vào giữa màn hình
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Bắt đầu game với góc nhìn đã chọn
        SwitchToView(isFPS);
    }

    private void Update()
    {
        // Xử lý các input và di chuyển trong mỗi frame
        HandleMovement();
        HandleLookRotation();
        HandlePCInputs();

        // Cập nhật vị trí và góc xoay của camera chính
        UpdateCameraTransform();
    }

    #endregion

    #region Xử Lý Logic (Handlers)

    /// <summary>
    /// Xử lý di chuyển của nhân vật (bàn phím và joystick).
    /// </summary>
    private void HandleMovement()
    {
        // --- 1. Xử lý Di chuyển đa nền tảng ---
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Một lực nhỏ để giữ nhân vật trên mặt đất
        }

        // Lấy input từ bàn phím (PC) hoặc joystick (Mobile)
        float moveX = isMobilePlatform ? joystick.Horizontal : Input.GetAxis("Horizontal");
        float moveZ = isMobilePlatform ? joystick.Vertical : Input.GetAxis("Vertical");

        // Tạo vector di chuyển dựa trên hướng của nhân vật
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        // Xác định tốc độ hiện tại (chạy hoặc đi bộ)
        float currentSpeed = (isMobilePlatform || !Input.GetKey(KeyCode.LeftShift)) ? speed : runSpeed;

        // Áp dụng di chuyển
        controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);

        // Áp dụng trọng lực
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Xử lý xoay camera (chuột và cảm ứng).
    /// </summary>
    private void HandleLookRotation()
    {
        // --- 2. Xử lý Quay góc nhìn (Camera Look) ---
        float lookX = 0f;
        float lookY = 0f;

        if (isMobilePlatform)
        {
            // Dùng cảm ứng vuốt ở nửa phải màn hình trên Mobile
            foreach (Touch touch in Input.touches)
            {
                if (touch.position.x > Screen.width / 2 && touch.phase == TouchPhase.Moved)
                {
                    lookX = touch.deltaPosition.x * lookSensitivity * 0.05f; // Giảm độ nhạy cho mobile
                    lookY = touch.deltaPosition.y * lookSensitivity * 0.05f;
                }
            }
        }
        else
        {
            // Dùng chuột trên PC
            lookX = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
            lookY = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;
        }

        // Tính toán góc xoay lên/xuống (pitch)
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Giới hạn góc nhìn từ -90 đến 90 độ

        // Áp dụng góc xoay lên/xuống cho cameraHolder
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Áp dụng góc xoay trái/phải cho toàn bộ nhân vật
        transform.Rotate(Vector3.up * lookX);
    }

    /// <summary>
    /// Xử lý các input chỉ dành cho PC (phím, chuột).
    /// </summary>
    private void HandlePCInputs()
    {
        if (isMobilePlatform) return; // Không chạy code này trên mobile

        // Gọi hàm Jump() khi bấm phím Space
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Gọi hàm ToggleCameraView() khi bấm phím 'V'
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleCameraView();
        }

        // Gọi các hàm bắn khi bấm/thả chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            StartFire();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopFire();
        }
    }

    /// <summary>
    /// Cập nhật vị trí của Main Camera theo điểm nhìn đang được chọn.
    /// </summary>
    private void UpdateCameraTransform()
    {
        Transform targetPoint = isFPS ? fpsCameraPoint : tpsCameraPoint;
        // Di chuyển mượt mà camera đến vị trí và góc xoay của điểm target
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPoint.position, Time.deltaTime * 20f);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetPoint.rotation, Time.deltaTime * 20f);
    }

    /// <summary>
    /// Chuyển camera tới một góc nhìn cụ thể.
    /// </summary>
    private void SwitchToView(bool isFirstPerson)
    {
        isFPS = isFirstPerson;
    }

    #endregion

    #region Hàm Công Khai (Public Methods)

    // --- 5. Các hàm public để gán vào EventTrigger của các nút bấm UI ---

    /// <summary>
    /// Thực hiện hành động nhảy. Có thể gọi từ phím hoặc nút UI.
    /// </summary>
    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    /// <summary>
    /// Chuyển đổi qua lại giữa góc nhìn FPS và TPS. Có thể gọi từ phím hoặc nút UI.
    /// </summary>
    public void ToggleCameraView()
    {
        isFPS = !isFPS;
        SwitchToView(isFPS);
    }

    /// <summary>
    /// Bắt đầu hành động bắn. Có thể gọi từ chuột hoặc nút UI.
    /// </summary>
    public void StartFire()
    {
        Debug.Log("Bắt đầu bắn!");
        // Thêm logic bắn của bạn ở đây (ví dụ: tạo tia raycast, tạo hiệu ứng,...)
    }

    /// <summary>
    /// Dừng hành động bắn. Có thể gọi từ chuột hoặc nút UI.
    /// </summary>
    public void StopFire()
    {
        Debug.Log("Ngừng bắn!");
        // Thêm logic ngừng bắn của bạn ở đây
    }

    #endregion
}
