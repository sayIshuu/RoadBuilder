using UnityEngine;

public class TutorialBoardSetter : TutorialBase
{
    [SerializeField] private float _clickDelay = 1f;
    private float _enterTime;

    public int[] CustomSlotIndex = new int[3];
    public int[] CustomBoardIndex = new int[25];

    private TileGenerator _tileGenerator;

    public override void Enter(TutorialController controller)
    {
        _enterTime = Time.time;
        _tileGenerator = FindAnyObjectByType<TileGenerator>();
    }

    public override void Execute(TutorialController controller)
    {
        if (Time.time - _enterTime > _clickDelay) controller.SetNextTutorial();
    }

    public override void Exit()
    {
        _tileGenerator.DeleteAnyTiles();

        // Offer Slot, Board Slot 커스텀
        _tileGenerator.GenerateCustomTiles(CustomSlotIndex);
        _tileGenerator.GenerateCustomBoard(CustomBoardIndex);
    }
}
