using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileDestroy : MonoBehaviour
{
    private Transform canvas;
    Transform[] childList;

    private void Awake()
    {
        canvas = FindFirstObjectByType<Canvas>().transform;
        childList = gameObject.GetComponentsInChildren<Transform>();
    }

    public void StartBreak()
    {
        for (int i = 1; i < childList.Length; i++)
        {
            childList[i].SetParent(canvas);
            childList[i].GetComponent<DestroyedTile>().enabled = true;
            childList[i].GetComponent<DestroyedTile>().StartDestroyEffect();
        }

        Destroy(childList[0].gameObject);
    }
}
