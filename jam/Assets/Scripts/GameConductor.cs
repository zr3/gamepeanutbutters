using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameConductor : MonoBehaviour
{
    [Header("Configuration")]
    public bool LoadTitleScene = false;
    public bool ShowHud
    {
        get => HUD.activeInHierarchy;
        set => HUD.SetActive(value);
    }
    public static void SetShowHud(bool value) => _instance.ShowHud = value;
    public ScriptableObject[] ScriptableObjectGameStates;

    [Header("References")]
    public GameObject HUD;
    public Animator CameraAnimator;

    private static GameConductor _instance;
    protected enum Hook {
        OnMainMenuStart,
        OnGameStart
    }
    private void CallHook(Hook hook) => gameObject.SendMessage(hook.ToString());

    private void Awake()
    {
        _instance = this.CheckSingleton(_instance);
        ShowHud = false;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnLevelLoad;
        if (LoadTitleScene)
        {
            SceneManager.LoadScene("Title", LoadSceneMode.Additive);
            CallHook(Hook.OnMainMenuStart);
        } else
        {
            CallHook(Hook.OnGameStart);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Title")
        {
            var gameStartButton = GameObject.FindGameObjectWithTag("GameStartButton")?.GetComponent<Button>();
            if (gameStartButton)
            {
                gameStartButton.onClick.AddListener(() => {
                    gameStartButton.interactable = false;
                    ScreenFader.FadeOutThen(() => {
                        SceneManager.UnloadSceneAsync("Title");
                        CallHook(Hook.OnGameStart);
                    });
                });
            }
            else
            {
                Debug.LogError($"{nameof(GameConductor)} could not find GameStartButton");
            }
        } else
        {
            Debug.Log($"Loaded level {scene.name} with no {nameof(GameConductor)} event handler.");
        }
    }

    public static void CameraStateTrigger(string trigger)
    {
        _instance.CameraAnimator.SetTrigger(trigger);
    }

    public static T GetScriptableGameStateOfType<T>() where T : ScriptableObject, IState
    {
        return _instance.ScriptableObjectGameStates.FirstOrDefault(gs => gs.GetType().Equals(typeof(T))) as T;
    }
}
