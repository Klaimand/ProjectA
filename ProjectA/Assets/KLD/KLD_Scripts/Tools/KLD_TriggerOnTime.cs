using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KLD_TriggerOnTime : MonoBehaviour
{
    [SerializeField] TriggerOnTimeStep[] timeTriggers;

    float timeOffset;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < timeTriggers.Length; i++)
        {
            if (!timeTriggers[i].triggered && KLD_SpawnersManager.Instance.levelTime + timeOffset >= timeTriggers[i].triggerAt)
            {
                timeTriggers[i].triggered = true;
                timeTriggers[i].onTime.Invoke();
            }
        }
    }

    public void Retrigger()
    {
        timeOffset = -KLD_SpawnersManager.Instance.levelTime;
    }
}

[System.Serializable]
public class TriggerOnTimeStep
{
    public float triggerAt = 0f;
    [HideInInspector] public bool triggered = false;
    public UnityEvent onTime;
}