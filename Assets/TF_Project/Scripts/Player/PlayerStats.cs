using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelSystem _playerLevel;
    [SerializeField] private BaseStatsSO playerBaseStats;

    public float CurrentAttack => playerBaseStats.Attack * _playerLevel.Level;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
