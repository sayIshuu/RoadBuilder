using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileBreaker : MonoBehaviour
{
    private Transform canvas;
    Transform[] childList;

    public void Awake()
    {
        canvas = FindFirstObjectByType<Canvas>().transform;
        childList = gameObject.GetComponentsInChildren<Transform>();
    }

    public void Start()
    {
        //StartCoroutine(DestroyEffect());
    }

    public void StartBreak()
    {
        for (int i = 1; i < childList.Length; i++)
        {
            childList[i].SetParent(canvas);
            childList[i].GetComponent<TileDestroyCheck>().StartDestroyEffect();
        }

        Destroy(childList[0].gameObject);
    }
}
