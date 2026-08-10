using UnityEngine;

/// <summary>
/// Quản lý việc chuyển đổi giữa các vũ khí của người chơi.
/// Gắn script này vào một GameObject rỗng là con của Player (ví dụ: WeaponHolder).
/// </summary>
public class WeaponSwitcher : MonoBehaviour
{
    [Tooltip("Danh sách các vũ khí mà người chơi sở hữu. Kéo các GameObject vũ khí vào đây.")]
    [SerializeField] private GameObject[] weapons;

    // Biến lưu trữ vũ khí đang được chọn
    private int selectedWeaponIndex = 0;

    void Start()
    {
        // Khi bắt đầu game, chỉ bật vũ khí đầu tiên và tắt các vũ khí khác
        SelectWeapon(selectedWeaponIndex);
    }

    void Update()
    {
        int previousSelectedWeaponIndex = selectedWeaponIndex;

        // --- Xử lý input từ PC ---
        // 1. Lăn chuột
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // Lăn lên - Chuyển đến vũ khí tiếp theo
            if (selectedWeaponIndex >= weapons.Length - 1)
                selectedWeaponIndex = 0;
            else
                selectedWeaponIndex++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            // Lăn xuống - Chuyển đến vũ khí phía trước
            if (selectedWeaponIndex <= 0)
                selectedWeaponIndex = weapons.Length - 1;
            else
                selectedWeaponIndex--;
        }

        // 2. Phím số
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeaponIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Length >= 2)
        {
            selectedWeaponIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Length >= 3)
        {
            selectedWeaponIndex = 2;
        }
        // (Thêm các phím khác nếu cần)

        // Nếu có sự thay đổi vũ khí, gọi hàm SelectWeapon
        if (previousSelectedWeaponIndex != selectedWeaponIndex)
        {
            SelectWeapon(selectedWeaponIndex);
        }
    }

    /// <summary>
    /// Bật vũ khí được chọn và tắt tất cả các vũ khí còn lại.
    /// </summary>
    /// <param name="index">Index của vũ khí trong mảng.</param>
    public void SelectWeapon(int index)
    {
        // Đảm bảo index hợp lệ
        if (index < 0 || index >= weapons.Length) return;

        selectedWeaponIndex = index;
        int i = 0;
        foreach (GameObject weapon in weapons)
        {
            // Nếu là vũ khí được chọn thì bật, ngược lại thì tắt
            weapon.SetActive(i == selectedWeaponIndex);
            i++;
        }
    }

    /// <summary>
    /// Chuyển sang vũ khí tiếp theo trong danh sách. Dùng cho nút bấm UI.
    /// </summary>
    public void NextWeapon()
    {
        int nextIndex = selectedWeaponIndex + 1;
        if (nextIndex >= weapons.Length)
        {
            nextIndex = 0;
        }
        SelectWeapon(nextIndex);
    }

    /// <summary>
    /// Quay về vũ khí phía trước trong danh sách. Dùng cho nút bấm UI.
    /// </summary>
    public void PreviousWeapon()
    {
        int prevIndex = selectedWeaponIndex - 1;
        if (prevIndex < 0)
        {
            prevIndex = weapons.Length - 1;
        }
        SelectWeapon(prevIndex);
    }
}
