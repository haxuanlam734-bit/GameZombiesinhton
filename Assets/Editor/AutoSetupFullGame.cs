using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AutoSetupFullGame
{
    [MenuItem("Tools/Auto Setup Full Game")]
    public static void SetupFullGame()
    {
        // --- BƯỚC 1: KHỞI TẠO CÁC MANAGER ---
        GameObject gameManager = FindOrCreateManager("GameManager", typeof(GameManager), typeof(ScoreManager));
        GameObject soundManager = FindOrCreateManager("SoundManager", typeof(SoundManager));
        SetupSoundManager(soundManager.GetComponent<SoundManager>());

        GameObject waveManager = FindOrCreateManager("WaveManager", typeof(ZombieSpawner));
        SetupWaveManager(waveManager.GetComponent<ZombieSpawner>());

        // --- BƯỚC 2: KHỞI TẠO NHÂN VẬT & CAMERA ---
        GameObject player = FindOrCreatePlayer();
        Camera mainCamera = FindOrCreateMainCamera();
        SetupPlayerAndCamera(player, mainCamera);

        // --- BƯỚC 3: KHỞI TẠO UI ---
        Canvas canvas = FindOrCreateCanvas();
        GameObject uiManagerObject = FindOrCreateUIManager(canvas);
        UIManager uiManager = uiManagerObject.GetComponent<UIManager>();
        DamageIndicator damageIndicator = uiManagerObject.GetComponent<DamageIndicator>();

        // --- BƯỚC 4: TỰ ĐỘNG KẾT NỐI THAM CHIẾU ---
        ConnectReferences(uiManager, player, waveManager.GetComponent<ZombieSpawner>());

        Debug.Log("✅ Đã tự động Setup và kết nối thành công 100% hệ thống Game Scene!");
    }

    #region Các hàm khởi tạo và thiết lập chi tiết

    private static GameObject FindOrCreateManager(string name, params System.Type[] components)
    {
        GameObject manager = GameObject.Find(name);
        if (manager == null)
        {
            manager = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(manager, $"Create {name}");
        }
        foreach (var component in components)
        {
            if (manager.GetComponent(component) == null)
            {
                Undo.AddComponent(manager, component);
            }
        }
        return manager;
    }

    private static void SetupSoundManager(SoundManager sm)
    {
        SerializedObject so = new SerializedObject(sm);
        if (sm.transform.Find("BGM_Source") == null) CreateAudioSource(sm.transform, "BGM_Source");
        if (sm.transform.Find("SFX_Source") == null) CreateAudioSource(sm.transform, "SFX_Source");

        so.FindProperty("bgmSource").objectReferenceValue = sm.transform.Find("BGM_Source").GetComponent<AudioSource>();
        so.FindProperty("sfxSource").objectReferenceValue = sm.transform.Find("SFX_Source").GetComponent<AudioSource>();
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(sm);
    }

    private static void CreateAudioSource(Transform parent, string name)
    {
        GameObject sourceObj = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(sourceObj, $"Create {name}");
        sourceObj.transform.SetParent(parent);
        AudioSource source = Undo.AddComponent<AudioSource>(sourceObj);
        source.playOnAwake = false;
    }

    private static void SetupWaveManager(ZombieSpawner spawner)
    {
        SerializedObject so = new SerializedObject(spawner);
        SerializedProperty spawnPointsProp = so.FindProperty("spawnPoints");
        if (spawnPointsProp.arraySize == 0)
        {
            GameObject holder = new GameObject("SpawnPoints_Holder");
            Undo.RegisterCreatedObjectUndo(holder, "Create SpawnPoints_Holder");
            holder.transform.SetParent(spawner.transform);

            for (int i = 0; i < 3; i++)
            {
                GameObject point = new GameObject($"SpawnPoint_{i + 1}");
                Undo.RegisterCreatedObjectUndo(point, $"Create SpawnPoint_{i + 1}");
                point.transform.SetParent(holder.transform);
                point.transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                spawnPointsProp.InsertArrayElementAtIndex(i);
                spawnPointsProp.GetArrayElementAtIndex(i).objectReferenceValue = point.transform;
            }
        }
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(spawner);
    }

    private static GameObject FindOrCreatePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            Object.DestroyImmediate(player.GetComponent<Collider>());
            Undo.RegisterCreatedObjectUndo(player, "Create Player");
        }

        if (player.GetComponent<CharacterController>() == null) Undo.AddComponent<CharacterController>(player);
        if (player.GetComponent<Health>() == null) Undo.AddComponent<Health>(player);
        if (player.GetComponent<CrossPlatformPlayerController>() == null) Undo.AddComponent<CrossPlatformPlayerController>(player);
        if (player.GetComponent<GrenadeSystem>() == null) Undo.AddComponent<GrenadeSystem>(player);

        return player;
    }

    private static Camera FindOrCreateMainCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = Undo.AddComponent<Camera>(camObj);
            cam.tag = "MainCamera";
            Undo.RegisterCreatedObjectUndo(camObj, "Create Main Camera");
        }
        if (cam.GetComponent<CameraShake>() == null) Undo.AddComponent<CameraShake>(cam);
        return cam;
    }

    private static void SetupPlayerAndCamera(GameObject player, Camera mainCamera)
    {
        // Setup Weapon Holder
        Transform weaponHolder = mainCamera.transform.Find("WeaponHolder");
        if (weaponHolder == null)
        {
            GameObject holderObj = new GameObject("WeaponHolder");
            Undo.RegisterCreatedObjectUndo(holderObj, "Create WeaponHolder");
            weaponHolder = holderObj.transform;
            weaponHolder.SetParent(mainCamera.transform);
            weaponHolder.localPosition = Vector3.zero;
        }
        if (weaponHolder.GetComponent<WeaponSwitcher>() == null) Undo.AddComponent<WeaponSwitcher>(weaponHolder.gameObject);
        if (weaponHolder.GetComponent<CrossPlatformGunController>() == null) Undo.AddComponent<CrossPlatformGunController>(weaponHolder.gameObject);
    }

    private static Canvas FindOrCreateCanvas()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = Undo.AddComponent<Canvas>(canvasObj);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Undo.AddComponent<CanvasScaler>(canvasObj);
            Undo.AddComponent<GraphicRaycaster>(canvasObj);
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
        }

        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            Undo.AddComponent<EventSystem>(esObj);
            Undo.AddComponent<StandaloneInputModule>(esObj);
            Undo.RegisterCreatedObjectUndo(esObj, "Create EventSystem");
        }
        return canvas;
    }

    private static GameObject FindOrCreateUIManager(Canvas canvas)
    {
        UIManager uiManager = canvas.GetComponentInChildren<UIManager>();
        GameObject uiManagerObj;
        if (uiManager == null)
        {
            uiManagerObj = new GameObject("UIManager");
            uiManagerObj.transform.SetParent(canvas.transform);
            uiManager = Undo.AddComponent<UIManager>(uiManagerObj);
            Undo.AddComponent<DamageIndicator>(uiManagerObj); // Gắn chung
            Undo.RegisterCreatedObjectUndo(uiManagerObj, "Create UIManager");
        }
        else
        {
            uiManagerObj = uiManager.gameObject;
        }
        return uiManagerObj;
    }

    private static void ConnectReferences(UIManager uiManager, GameObject player, ZombieSpawner spawner)
    {
        SerializedObject so = new SerializedObject(uiManager);

        // Nối các Manager và Player
        so.FindProperty("playerHealth").objectReferenceValue = player.GetComponent<Health>();
        so.FindProperty("gunController").objectReferenceValue = player.GetComponentInChildren<CrossPlatformGunController>();
        so.FindProperty("zombieSpawner").objectReferenceValue = spawner;

        // Tìm hoặc tạo và nối các UI elements
        ConnectUIElement<Slider>(so, "healthSlider", "HealthSlider");
        ConnectUIElement<TextMeshProUGUI>(so, "healthText", "HealthText");
        ConnectUIElement<TextMeshProUGUI>(so, "ammoText", "AmmoText");
        ConnectUIElement<TextMeshProUGUI>(so, "waveText", "WaveText");
        ConnectUIElement<TextMeshProUGUI>(so, "zombiesLeftText", "ZombiesLeftText");
        ConnectUIElement<TextMeshProUGUI>(so, "waveCountdownText", "WaveCountdownText");
        ConnectUIElement<GameObject>(so, "gameOverPanel", "GameOverPanel");

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(uiManager);
    }

    private static void ConnectUIElement<T>(SerializedObject so, string propName, string objName) where T : Component
    {
        UIManager uiManager = so.targetObject as UIManager;
        Transform elementTransform = uiManager.transform.parent.Find(objName); // Tìm con của Canvas
        if (elementTransform == null)
        {
            GameObject elementObj = new GameObject(objName);
            Undo.RegisterCreatedObjectUndo(elementObj, $"Create {objName}");
            elementObj.transform.SetParent(uiManager.transform.parent);
            T component = Undo.AddComponent<T>(elementObj);
            so.FindProperty(propName).objectReferenceValue = component;
        }
        else
        {
            so.FindProperty(propName).objectReferenceValue = elementTransform.GetComponent<T>();
        }
    }

    #endregion
}
