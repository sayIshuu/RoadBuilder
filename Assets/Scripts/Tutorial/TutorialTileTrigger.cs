
using UnityEngine;

public class TutorialTileTrigger : TutorialBase
{
    private TileDraggable[] tileDraggables;
    private TutorialController _controller;
    private TutorialRerollButton _rerollBtn;
    private bool _completed = false;

    public override void Enter(TutorialController controller)
    {
        _controller = controller;
        _rerollBtn = FindAnyObjectByType<TutorialRerollButton>();
        _rerollBtn.OnClicked += AddDetectPlacementEvent;

        AddDetectPlacementEvent();
    }

    public override void Execute(TutorialController controller)
    {
    }

    public override void Exit()
    {
        UnsubscribeTileEvents();
        _rerollBtn.OnClicked -= AddDetectPlacementEvent;
    }

    private void AddDetectPlacementEvent()
    {
        UnsubscribeTileEvents();

        tileDraggables = FindObjectsByType<TileDraggable>(FindObjectsSortMode.None);

        for (int i = 0; i < tileDraggables.Length; i++)
        {
            tileDraggables[i].OnPlaced += DetectPlacement;
        }
    }

    private void UnsubscribeTileEvents()
    {
        if (tileDraggables == null) return;

        for (int i = 0; i < tileDraggables.Length; i++)
        {
            if (tileDraggables[i] != null)
            {
                tileDraggables[i].OnPlaced -= DetectPlacement;
            }
        }

        tileDraggables = null;
    }

    private void DetectPlacement()
    {
        if (_completed) return;
        _completed = true;

        _controller.SetNextTutorial();
    }
}
