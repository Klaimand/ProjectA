using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class XL_PlayerInfo : MonoBehaviour
{
    public static XL_PlayerInfo instance;

    public KLD_MenuData menuData;
    [SerializeField] private XL_CharacterDetailsMenu characterMenu;
    [SerializeField] private XL_WeaponDetailsMenu weaponMenu;

    public Action<Weapon> onWeaponChange;
    public Action<Character> onCharacterChange;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (menuData == null)
        {
            menuData = new KLD_MenuData();
        }

        InitialiseMenuData();
    }

    private void InitialiseMenuData()
    {
        menuData.character = 0;
        menuData.weapon = 0;
        menuData.map = 0;
        menuData.difficulty = 0;
        menuData.missionEnergyCost = 0;
    }

    public void SelectPlayer()
    {
        menuData.character = (Character)characterMenu.selectedPlayer;
        onCharacterChange?.Invoke(menuData.character);
    }

    public void SelectWeapon()
    {
        menuData.weapon = (Weapon)weaponMenu.selectedWeapon;
        onWeaponChange?.Invoke(menuData.weapon);
    }

    public void SelectMap(int idx)
    {
        menuData.map = (Map)idx;
    }

    public void SelectDifficulty(int idx)
    {
        menuData.difficulty = (Difficulty)idx;
    }
}
