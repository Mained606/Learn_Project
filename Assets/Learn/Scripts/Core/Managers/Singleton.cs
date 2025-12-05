using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬에 존재하는지 먼저 확인
                instance = FindFirstObjectByType<T>();

                if (instance == null)
                {
                    Debug.LogError($"[Singleton] {typeof(T)} Instance를 씬에서 찾지 못 했습니다.");
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this as T;
        DontDestroyOnLoad(gameObject); // 전역 유지
    }
}