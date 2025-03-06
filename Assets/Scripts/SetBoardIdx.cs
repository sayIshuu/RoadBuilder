using UnityEngine;

public class SetBoardIdx : MonoBehaviour
{
    private BoardSlot[] boardSlots;
    private void Awake()
    {
        boardSlots = GetComponentsInChildren<BoardSlot>();

        for(int i = 0; i < boardSlots.Length; i++)
        {
            boardSlots[i].SetIdx(i);
        }
    }
}
