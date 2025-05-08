using UnityEngine;
using UnityEngine.SceneManagement;

public class Common : MonoBehaviour
{
    public static Common Instance;

    public string OpenScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    private void Start()
    {
        SingletonSaveData.Instance.SaveManager = GetSaveManager();
        SingletonSaveData.Instance.Load();
        SceneManager.LoadScene(OpenScene);
        //if (LoadingSceneIntegration.otherScene == -2)
        //{
        //}
        //else
        //{
        //    SceneManager.LoadScene(LoadingSceneIntegration.otherScene);
        //}
    }

    public static ISaveManager GetSaveManager()
    {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_WEBGL
        return new PlayerPrefSaveManager();
#elif UNITY_STANDALONE_WIN
        return new SteamSaveManager();
#else
        Debug.LogWarning("No save manager implemented for this platform!");
        return null;
#endif
    }
}
