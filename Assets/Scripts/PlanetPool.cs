using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPool : MonoBehaviour {

    public CadetController cadet;
    public List<Planet> planetPrefabs;

    private List<Planet> activePlanets = new List<Planet>();
    private List<Planet> pool = new List<Planet>();
    	
    public Planet TakeRandomPlanet()
    {
        Planet planet = FindRandomPlanet();
        pool.Remove(planet);
        activePlanets.Add(planet);
        return planet;
    }

    private Planet FindRandomPlanet()
    {
        //Pick a random prefab
        int randomIndex = Random.Range(0, planetPrefabs.Count);
        Planet prefab = planetPrefabs[randomIndex];

        //Check if the pool contains an available instance for that prefab
        Planet planet = TakeFromPoolIfAvailable(prefab.id);
        if (planet == null)
        {
            planet = InstantiatePlanetFromPrefab(prefab);
        }

        RandomisePlanetAttributes(planet);

        return planet;
    }

    private Planet TakeFromPoolIfAvailable(int id)
    {
        foreach (Planet planet in pool)
        {
            if (planet.id == id)
            {
                return planet;
            }
        }
        return null;
    }
    
    private Planet InstantiatePlanetFromPrefab(Planet planet)
    {
        Planet newPlanet = Instantiate(planet, transform);
        foreach (Orbit orbit in newPlanet.GetComponentsInChildren<Orbit>())
        {
            orbit.cadet = cadet;
        }
        return newPlanet;
    }
    
    private void RandomisePlanetAttributes(Planet planet)
    {
        float planetScale = Random.Range(planet.minScale, planet.maxScale);
        planet.transform.localScale = new Vector3(planetScale, planetScale);

        float orbitRotationsPerSecond = Random.Range(planet.minRotationsPerSecond, planet.maxRotationsPerSecond);
        foreach (Orbit orbit in planet.GetComponentsInChildren<Orbit>())
        {
            orbit.SetTargetRotationsPerSecond(orbitRotationsPerSecond * planetScale);
        }

        planet.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(0f, 360f)));
    } 
}
