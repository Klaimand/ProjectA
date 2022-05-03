using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Character
{
    BLAST,
    SIMO,
    SAYURI
};

public enum Weapon
{
    OLD_RELIABLE,
    THE_CLASSIC,
    SERVICE_CARABINE,
    CQC_SWEEPER,
    RANGER
};

public enum Map
{
    MALL,
    PARK,
    MARINA
}

public enum Difficulty
{
    EASY,
    MEDIUM,
    HARD
}


public class KLD_MenuData
{
    public Character character = Character.BLAST;

    public Weapon weapon = Weapon.THE_CLASSIC;

    public Map map = Map.MALL;

    public Difficulty difficulty = Difficulty.EASY;
}