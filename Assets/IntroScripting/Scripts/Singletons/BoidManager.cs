using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IntroScripting.Scripts.Singletons
{
    public class BoidManager : Singleton<BoidManager>
    {
        public Boid boidPrefab;

        public float spread;
        public float startSpeed;
        public float boidCount;
        

        public BoidSettings boidSettings;
        public BoidDebug debug;
        
        private List<Boid> boids = new List<Boid>();

        private void Awake()
        {
            SpawnBoids(boidCount, transform.position, spread, startSpeed);
        }

        private void SpawnBoids(float count, Vector3 position, float spread = 1, float speed = 4)
        {
            for (int i = 0; i <= count; i++)
            {
                // Créer un boid
                Boid boid = GameObject.Instantiate<Boid>(boidPrefab);

                // Générer une position aléatoire
                Vector3 boidPosition = (Random.insideUnitSphere * spread) + position;
                boidPosition.y = Mathf.Abs(boidPosition.y);
                boid.velocity = (boidPosition - transform.position).normalized * speed;
                boid.boidSettings = boidSettings;

                boids.Add(boid);
            }
        }

        private void OnDrawGizmosSelected()
        {
            var pos = transform.position;
            Gizmos.color = debug.repulsionDistanceColor;
            Gizmos.DrawWireSphere(pos, boidSettings.repulsionDistance);     
            DrawArrow.ForGizmo(pos, Vector3.forward * boidSettings.repulsionDistance - pos);
            
            Gizmos.color = debug.alignmentDistanceColor;
            Gizmos.DrawWireSphere(pos, boidSettings.alignmentDistance);
            DrawArrow.ForGizmo(pos, Vector3.forward * boidSettings.alignmentDistance - pos);
            
            Gizmos.color = debug.attractionDistanceColor;
            Gizmos.DrawWireSphere(pos, boidSettings.attractionDistance);
            DrawArrow.ForGizmo(pos, Vector3.forward * boidSettings.attractionDistance - pos);
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(pos, spread);
            
        }
    }
}