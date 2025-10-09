using System;
using UnityEngine;

public class TutorialTileTrigger : TutorialBase
{
    private BoardSlot[] boardSlot;
    private TutorialController _controller;

    private void Start()
    {
        boardSlot = FindObjectsByType<BoardSlot>(FindObjectsSortMode.None);
    }

    public override void Enter(TutorialController controller)
    {
        for (int i=0;i<boardSlot.Length;i++)
        {
            boardSlot[i].OnPlaced += DetectPlacement;
            _controller = controller;
        }
    }

    public override void Execute(TutorialController controller)
    {
    }

    public override void Exit()
    {
        for (int i=0;i<boardSlot.Length;i++)
        {
            boardSlot[i].OnPlaced -= DetectPlacement;
        }
    }

    private void DetectPlacement()
    {
        _controller.SetNextTutorial();
    }
}
