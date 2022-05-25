using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class XL_MainMenu : MonoBehaviour
{
    public static XL_MainMenu instance;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject characterSelectMenu;
    [SerializeField] private GameObject characterDetailsMenu;
    [SerializeField] private GameObject weaponSelectMenu;
    [SerializeField] private GameObject weaponDetailsMenu;
    [SerializeField] private GameObject mapSelectMenu;
    [SerializeField] private GameObject shopMenu;

    [Header("Save Characters and Weapons")]
    [SerializeField] private XL_CharacterAttributesSO[] characterAttributes;
    [SerializeField] private KLD_WeaponSO[] weaponAttributes;

    [Header("Energy")]
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text energyMaxText;
    [SerializeField] private int energyMax;

    [Header("Soft Currency")]
    [SerializeField] private TMP_Text softCurrencyText;

    [Header("Hard Currency")]
    [SerializeField] private TMP_Text hardCurrencyText;

    [Header("Mission Type")]
    [SerializeField] private GameObject[] missionTypes;

    [Header("Loader Bars")]
    [SerializeField] private TMP_Text characterXP;
    [SerializeField] private TMP_Text weaponXP;

    [Header("Go Button")]
    [SerializeField] private Button goButton;
    [SerializeField] private Image goImage;
    [SerializeField] private Image arrowGoImage;

    //[Header("Loadout")]


    private void Awake()
    {
        instance = this;

        InitPlayerPrefs();
        RefreshMainMenuUI();
        RefreshTopOverlay();
    }

    private void InitPlayerPrefs()
    {
        //---DELETE PLAYERPREFS---
        PlayerPrefs.DeleteAll();

        if (KLD_MissionInfos.instance != null)
        {
            PlayerPrefs.SetInt("Energy", KLD_MissionInfos.instance.missionData.GetEnergy() + PlayerPrefs.GetInt("Energy"));
            PlayerPrefs.SetInt("SoftCurrency", KLD_MissionInfos.instance.missionData.GetSoftCurrency() + PlayerPrefs.GetInt("SoftCurrency"));
            PlayerPrefs.SetInt("HardCurrency", KLD_MissionInfos.instance.missionData.GetHardCurrency() + PlayerPrefs.GetInt("HardCurrency"));
        }

        #region playerPrefs

        foreach (XL_CharacterAttributesSO ca in characterAttributes)
        {
            if (!PlayerPrefs.HasKey(ca.characterName))
            {
                PlayerPrefs.SetInt(ca.characterName, 1);
            }
            if (!PlayerPrefs.HasKey(ca.characterName + "Unlocked"))
            {
                PlayerPrefs.SetInt(ca.characterName, 0);
            }
            ca.level = PlayerPrefs.GetInt(ca.characterName);
        }
        PlayerPrefs.SetInt("BlastUnlocked", 1);
        foreach (KLD_WeaponSO wa in weaponAttributes)
        {
            if (!PlayerPrefs.HasKey(wa.weaponName))
            {
                PlayerPrefs.SetInt(wa.weaponName, 1);
            }
            wa.level = PlayerPrefs.GetInt(wa.weaponName);
        }
        if (!PlayerPrefs.HasKey("Energy"))
        {
            PlayerPrefs.SetInt("Energy", 100);
        }
        if (!PlayerPrefs.HasKey("SoftCurrency"))
        {
            PlayerPrefs.SetInt("SoftCurrency", 20000);
        }
        if (!PlayerPrefs.HasKey("HardCurrency"))
        {
            PlayerPrefs.SetInt("HardCurrency", 1000);
        }
        if (!PlayerPrefs.HasKey("SelectedHero"))
        {
            PlayerPrefs.SetInt("SelectedHero", 0);
        }

        PlayerPrefs.Save();

        #endregion

        RefreshMainMenuUI();
    }

    private void RefreshMainMenuUI()
    {
        characterXP.text = (characterAttributes[(int)XL_PlayerInfo.instance.menuData.character].level + 1).ToString();
        weaponXP.text = (weaponAttributes[(int)XL_PlayerInfo.instance.menuData.weapon].level + 1).ToString();

        RefreshGOButton();
    }

    public void RefreshGOButton()
    {
        if (XL_PlayerInfo.instance.menuData.missionEnergyCost > PlayerPrefs.GetInt("Energy"))
        {
            goButton.interactable = false;
            goImage.color = Color.red;
            arrowGoImage.color = Color.red;
        }
        else
        {
            goButton.interactable = true;
            goImage.color = Color.white;
            arrowGoImage.color = Color.white;
        }
    }

    public void StartMission()
    {
        if (energyMax <= PlayerPrefs.GetInt("Energy")) XL_PlayerSession.instance.StartCoroutine(XL_PlayerSession.instance.EnergyCoroutine());
        PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") - XL_PlayerInfo.instance.menuData.missionEnergyCost);

        if (KLD_LoadingScreen.instance != null)
        {
            KLD_LoadingScreen.instance.ShowLoadingScreen();
        }
        SceneManager.LoadScene(1);
    }

    public void SwitchMainMenu()
    {
        RefreshMainMenuUI();
        mainMenu.SetActive(true);

        shopMenu.SetActive(false);
        characterSelectMenu.SetActive(false);
        characterDetailsMenu.SetActive(false);
        weaponSelectMenu.SetActive(false);
        weaponDetailsMenu.SetActive(false);
        mapSelectMenu.SetActive(false);
    }

    public void SwitchShopMenu()
    {
        shopMenu.SetActive(true);

        mainMenu.SetActive(false);
        characterSelectMenu.SetActive(false);
        characterDetailsMenu.SetActive(false);
        weaponSelectMenu.SetActive(false);
        weaponDetailsMenu.SetActive(false);
        mapSelectMenu.SetActive(false);
    }

    public void SwitchCharaSelectMenu()
    {
        characterSelectMenu.SetActive(true);

        mainMenu.SetActive(false);
        characterDetailsMenu.SetActive(false);
    }

    public void SwitchCharacterDetailsMenu()
    {
        characterDetailsMenu.SetActive(true);

        characterSelectMenu.SetActive(false);
    }

    public void SwitchWeaponSelectMenu()
    {
        weaponSelectMenu.SetActive(true);

        mainMenu.SetActive(false);
        weaponDetailsMenu.SetActive(false);
    }

    public void SwitchWeaponDetailsMenu()
    {
        weaponDetailsMenu.SetActive(true);

        weaponSelectMenu.SetActive(false);
    }

    public void SwitchMapSelectMenu()
    {
        mapSelectMenu.SetActive(true);

        mainMenu.SetActive(false);
    }

    public void SelectMissionType(int idx)
    {
        foreach (GameObject go in missionTypes)
        {
            go.SetActive(false);
        }

        missionTypes[idx].SetActive(true);
    }

    public void RefreshTopOverlay()
    {
        //Energy
        energyText.text = PlayerPrefs.GetInt("Energy").ToString();
        energyMaxText.text = energyMax.ToString();

        //SoftCurrency
        softCurrencyText.text = PlayerPrefs.GetInt("SoftCurrency").ToString();

        //HardCurrency
        hardCurrencyText.text = PlayerPrefs.GetInt("HardCurrency").ToString();
    }

    public KLD_WeaponSO GetWeaponSO(Weapon _weapon)
    {
        return weaponAttributes[(int)_weapon];
    }

    public int GetEnergyMaxAmount()
    {
        return energyMax;
    }
}
