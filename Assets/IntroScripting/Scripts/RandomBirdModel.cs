
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomBirdModel : MonoBehaviour
{
    public List<GameObject> birdsModels;

    private void Awake()
    {
        // Créer un modèle random d'oiseau
        var rnd = (int) Mathf.Floor(Random.Range(0, birdsModels.Count));
        var birdModel = GameObject.Instantiate<GameObject>(birdsModels[rnd], transform, false);
    }
}
