using UnityEngine;


/// <summary>
/// Implementing this manages the creating of a MonoBehaviour Singleton 
/// It uses lazy initialization, so the singleton is initialized on first call to it
/// Also prevents the Monobehaviour from having duplicates in the scene.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<T>();
                if (!instance)

                {
                    instance = new GameObject(typeof(T) + "").AddComponent<T>();
                }

            }

            return instance;
        }

    }

    private void Awake()
    {
        //To prevent duplicates
        if (instance)
            Destroy(this.gameObject);
    }
}
