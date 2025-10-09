using UnityEngine;

public abstract class TutorialBase : MonoBehaviour
{
    public abstract void Enter(TutorialController controller);

    public abstract void Execute(TutorialController controller);

    public abstract void Exit();
}
