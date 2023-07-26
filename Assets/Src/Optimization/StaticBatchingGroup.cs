using System.Linq;
using UnityEngine;

public class StaticBatchingGroup : MonoBehaviour
{
    public enum BatchingMode
    {
        Children,
        List
    }

    public BatchingMode Mode;
    public GameObject[] list = new GameObject[0];

    // Start is called before the first frame update
    void Start()
    {
        switch (Mode)
        {
            case BatchingMode.Children:
                BatchByChildren();
                break;
            case BatchingMode.List:
                BatchByList();
                break;
        }
    }

    private void BatchByList()
    {
        var cleanedList = list.Where(o => o != null).ToArray();
        if (cleanedList.Length > 0)
            StaticBatchingUtility.Combine(cleanedList, cleanedList[0]);
    }

    private void BatchByChildren()
    {
        StaticBatchingUtility.Combine(gameObject);
    }
}