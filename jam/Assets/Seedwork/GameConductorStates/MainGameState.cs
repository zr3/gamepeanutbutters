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
        Juicer.ShakeCamera(0.5f);
        MusicBox.ChangeMusic((int)Song.Boss);
        DataDump.Set("HP", 20);
        var cam = GameObject.Find("CinemachineStateCamera/GameCam").GetComponent<CinemachineVirtualCamera>();
        cam.Follow = player.transform;
        cam.LookAt = player.transform;
        GameConductor.SetShowHud(true);
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
        Juicer.ShakeCamera(1.5f);
        bool readyToMoveOn = false;
        MessageController.AddMessage("butterboi is dead now.", postAction: () => readyToMoveOn = true);
        while (!readyToMoveOn)
        {
            yield return null;
        }
    }

    public void OnExit()
    {
        GameConductor.SetShowHud(false);
        ScreenFader.FadeOut();
        NextState = new CreditsState();
    }
}
