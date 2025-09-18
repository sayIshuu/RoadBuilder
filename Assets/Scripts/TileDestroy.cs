using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileDestroy : MonoBehaviour
{
    private Transform canvas;
    Transform[] childList;

    private void Awake()
    {
        canvas = FindFirstObjectByType<GameCanvas>().GetComponent<Canvas>().transform;
        childList = gameObject.GetComponentsInChildren<Transform>();
    }

    public void StartBreak()
    {
        foreach (var child in childList.Skip(1))
        {
            child.SetParent(canvas);
            child.GetComponent<DestroyedTile>().enabled = true;
            child.GetComponent<DestroyedTile>().StartDestroyEffect();
        }

        Destroy(childList[0].gameObject);
    }
}
