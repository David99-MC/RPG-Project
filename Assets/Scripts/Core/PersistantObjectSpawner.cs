using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistantObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistantObjectPrefab;
        
        static bool hasSpawn = false;

        private void Awake() {
            if (hasSpawn) return;

            SpawnPersistantObject();

            hasSpawn = true;
        }

        void SpawnPersistantObject()
        {
            GameObject persistantObject = GameObject.Instantiate(persistantObjectPrefab);

            DontDestroyOnLoad(persistantObject);
        }
    }
}
