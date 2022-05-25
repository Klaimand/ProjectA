using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XL_GameManager : MonoBehaviour
{
    public static XL_GameManager instance;
    public List<GameObject> players = new List<GameObject>();
    //public List<XL_Enemy> enemies = new List<XL_Enemy>();
    //public KLD_ZombieList zombieList;
    [SerializeField] float timeBetweenInitAndShowStats = 1.5f;


    [SerializeField] KLD_EndingScreen endingScreen;
    [SerializeField] KLD_TouchInputs inputs;
    [SerializeField] KLD_PlayerController controller;
    [SerializeField] KLD_PlayerAim playerAim;
    [SerializeField] ParticleSystem spawnFX;

    bool gameEnded = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inputs.disableInputs = true;
        KLD_LoadingScreen.instance.ShowLoadingScreen();
        StartGame();
    }

    void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(2f);

        KLD_LoadingScreen.instance.HideLoadingScreen();

        yield return new WaitForSeconds(1f);

        KLD_AudioManager.Instance.PlaySound("SpawnHelico");
        spawnFX.Play();

        yield return new WaitForSeconds(2f);

        controller.DoSpawnAnimation();

        yield return new WaitForSeconds(1.5f);

        KLD_SpawnersManager.Instance.StartTimer();
        XL_GameModeManager.instance.StartTimer();
        inputs.disableInputs = false;
    }

    public void WinGame()
    {
        if (!gameEnded)
        {

            gameEnded = true;

            controller.SetPlayerState(PlayerState.WIN);

            KLD_MissionInfos.instance.RefreshMissionInfos(true);

            StartCoroutine(ChangeSceneCoroutine());
        }
    }

    public void LoseGame(bool _dead)
    {
        if (!gameEnded)
        {
            gameEnded = true;

            controller.SetPlayerState(_dead ? PlayerState.DEAD : PlayerState.LOOSE);

            KLD_MissionInfos.instance.RefreshMissionInfos(false);

            StartCoroutine(ChangeSceneCoroutine());
        }
    }

    IEnumerator ChangeSceneCoroutine()
    {
        playerAim.Die();
        inputs.disableInputs = true;

        yield return new WaitForSeconds(2f);

        KLD_AudioManager.Instance.PlayCharacterSound("Victory", 8);

        Time.timeScale = 0f;

        endingScreen.gameObject.SetActive(true);

        endingScreen.Initialize();

        yield return new WaitForSecondsRealtime(timeBetweenInitAndShowStats);

        endingScreen.ShowStats();

    }

    //triggered by ending screen button
    public void ChangeScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
