using Cinemachine;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MainGameStateState.asset", menuName = "peanutbutters/MainGameState", order = 20)]
public class MainGameState : ScriptableObject, IState
{
    public GameObject PlayerPrototype;
    public Vector3 PlayerSpawnLocation;

    private GameObject player;

    public IState NextState { get; private set; }

    public void OnEnter() {
        player = GameObject.Instantiate(PlayerPrototype);
        player.transform.position = PlayerSpawnLocation;
        Juicer.CreateFx(0, PlayerSpawnLocation);
        MusicBox.ChangeMusic((int)Song.Boss);
        DataDump.Set("HP", 130);
        var cam = GameObject.Find("CinemachineStateCamera/GameCam").GetComponent<CinemachineVirtualCamera>();
        cam.Follow = player.transform;
        cam.LookAt = player.transform;
    }

    public IEnumerator OnUpdate()
    {
        do
        {
            DataDump.Set("HP", DataDump.Get<int>("HP") - 1);
            yield return new WaitForSeconds(1);
        } while (DataDump.Get<int>("HP") > 0);
        Juicer.CreateFx(0, player.transform.position);
        GameObject.Destroy(player);
        bool readyToMoveOn = false;
        MessageController.AddMessage("butterboi is dead now.", postAction: () => readyToMoveOn = true);
        while (!readyToMoveOn)
        {
            yield return null;
        }
    }

    public void OnExit()
    {
        // set next state
    }
}
