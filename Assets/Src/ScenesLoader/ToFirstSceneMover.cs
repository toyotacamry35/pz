using UnityEngine;
using UnityEngine.SceneManagement;

public class ToFirstSceneMover : MonoBehaviour
{
    public bool IsEnable = true;
    public bool IsDestroyObjectAtWorkingTime = true;

    //=== Unity ===============================================================

    private void Awake()
    {
        if (Application.isEditor)
        {
            if (!IsEnable)
                return;

            if (!IsFirstBuildIndexScene())
            {
                if (IsDestroyObjectAtWorkingTime)
                    Destroy(gameObject);
            }
            else
            {
                Debug.Log("ToFirstSceneMover: inpropriate scene detected. LoadScene(0)");
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            if (IsDestroyObjectAtWorkingTime)
                Destroy(gameObject);
        }

    }


    //=== Public ==============================================================

    public static bool IsFirstBuildIndexScene()
    {
        var activeSceneName = SceneManager.GetActiveScene().name;
        var zeroSceneName = SceneManager.GetSceneByBuildIndex(0).name;
        var res = activeSceneName != zeroSceneName;
        //Logs.Log("activeSceneName='{0}'  zeroSceneName='{1}'  res{2}", activeSceneName, zeroSceneName, res.AsSign()); //DEBUG
        return res;
    }
}