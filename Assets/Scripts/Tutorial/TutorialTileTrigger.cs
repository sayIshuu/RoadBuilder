
using UnityEngine;

public class TutorialTileTrigger : TutorialBase
{
    private TileDraggable[] tileDraggables;
    private TutorialController _controller;
    private bool _isSet;

    public override void Enter(TutorialController controller)
    {
        _controller = controller;

        tileDraggables = FindObjectsByType<TileDraggable>(FindObjectsSortMode.None);

        for (int i = 0; i < tileDraggables.Length; i++)
        {
            tileDraggables[i].OnPlaced += DetectPlacement;
        }
    }

    public override void Execute(TutorialController controller)
    {
    }

    public override void Exit()
    {
        for (int i = 0; i < tileDraggables.Length; i++)
        {
            tileDraggables[i].OnPlaced -= DetectPlacement;
        }
    }

    private void DetectPlacement()
    {
        _controller.SetNextTutorial();
    }
}
