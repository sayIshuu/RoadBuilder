using System.Reflection;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    public Mission[] missions; // ��� ���� ����Ʈ

    private void Start()
    {
        //missions = GetComponentsInChildren<Mission>(); // ������ �ڵ� ���
    }

    // �� óġ �̺�Ʈ �߻� �� ���� üũ
    public void CheckAllLengthMission(int len)
    {
        foreach (Mission mission in missions)
        {
            mission.CheckLengthMisson(len);
        }
    }
}
