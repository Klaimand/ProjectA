using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KLD_ZombieList : MonoBehaviour
{
    public static KLD_ZombieList Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField, ReadOnly] List<KLD_ZombieAttributes> zombies = new List<KLD_ZombieAttributes>();

    public void AddZombie(KLD_ZombieAttributes _zombieAttributes)
    {
        zombies.Add(_zombieAttributes);
    }

    public void RemoveZombie(KLD_ZombieAttributes _zombieAttributes)
    {
        //if (!zombies.Remove(_zombieAttributes))
        //{
        //    Debug.LogError("Wanted to remove zombie that is not in list");
        //}
        zombies.Remove(_zombieAttributes);
    }

    public List<KLD_ZombieAttributes> GetZombies()
    {
        return zombies;
    }

}
