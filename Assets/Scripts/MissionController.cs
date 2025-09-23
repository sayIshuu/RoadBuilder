using System.Reflection;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    public Mission[] missions; // 모든 업적 리스트

    private void Start()
    {
        //missions = GetComponentsInChildren<Mission>(); // 업적들 자동 등록
    }

    // 적 처치 이벤트 발생 시 업적 체크
    public void CheckAllLengthMission(int len)
    {
        foreach (Mission mission in missions)
        {
            mission.CheckLengthMisson(len);
        }
    }
}
