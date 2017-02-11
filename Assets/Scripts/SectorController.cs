using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SectorController : MonoBehaviour
{

    public float sectorRadius = 15f;
    public float spawnHorizon = 45f;

    public PlanetPool planets;
    public CadetController cadet;

    private List<Sector> sectors = new List<Sector>();
    private float distancePopulated = 0f;
    private float distanceBetweenCols;

    private bool triTopFilled = false;
    private bool triMidFilled = false;
    private bool triBotFilled = false;

    private bool quadTopFilled = false;
    private bool quadTopMidFilled = false;
    private bool quadBotMidFilled = false;
    private bool quadBotFilled = false;

    private bool spawnQuadNext = false;

    // Use this for initialization
    void Start()
    {
        distanceBetweenCols = Mathf.Sqrt(((sectorRadius * 2) * (sectorRadius * 2)) - (sectorRadius * sectorRadius)); //Pythagorus
        SpawnStartSectors();
        SpawnCol();
        SpawnCol();
    }

    void Update()
    {
        //Spawn in planets as required
        while ((cadet.transform.position.x + spawnHorizon) > distancePopulated)
        {
            SpawnCol();
        }

        //TODO cleanup old planets?

        //Check for death
        if (Mathf.Abs(cadet.transform.position.y) > sectorRadius * 5f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (cadet.transform.position.x < (distancePopulated - (3f * spawnHorizon)))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void SpawnCol()
    {
        if (spawnQuadNext)
        {
            SpawnColOfFour();
        } else
        {
            SpawnColOfThree();
        }
        spawnQuadNext = !spawnQuadNext;
    }

    private void SpawnStartSectors()
    {
        quadTopMidFilled = true;
        quadBotMidFilled = true;

        SpawnSector(sectorRadius, true);
        SpawnSector(-sectorRadius, true);

        //Line the cadet up towards the first planet
        cadet.transform.position += new Vector3(0, sectors[0].attachedPlanet.transform.position.y);
    }

    private void SpawnColOfThree()
    {
        distancePopulated += distanceBetweenCols;

        int planetsToSpawn = Random.Range(MinPlanetsInThreeCol(), 3);
        if (planetsToSpawn == 1)
        {
            SpawnOneInColOfThree();
        }
        else if (planetsToSpawn == 2)
        {
            SpawnTwoInColOfThree();
        }
        else
        {
            SpawnThreeInColOfThree();
        }
    }

    private int MinPlanetsInThreeCol()
    {
        if (quadTopFilled && (quadBotMidFilled || quadBotFilled))
        {
            return 2;
        }
        else if (quadBotFilled && (quadTopMidFilled || quadTopFilled))
        {
            return 2;
        }
        return 1;
    }

    private void SpawnOneInColOfThree()
    {
        ClearThreeRow();
        if (quadTopFilled)
        {
            SpawnTopRowInThree();
        }
        else if (quadBotFilled)
        {
            SpawnBotRowInThree();
        }
        else
        {
            SpawnMidRowInThree();
        }

    }

    private void SpawnTwoInColOfThree()
    {
        int spawned = 0;
        ClearThreeRow();

        if (quadTopFilled)
        {
            SpawnTopRowInThree();
            spawned++;
        }
        if (quadBotFilled)
        {
            SpawnBotRowInThree();
            spawned++;
        }
        if (spawned < 2)
        {
            SpawnMidRowInThree();
            spawned++;
        }
        if (spawned < 2)
        {
            if (Random.Range(0, 2) == 1)
            {
                SpawnTopRowInThree();
            }
            else
            {
                SpawnBotRowInThree();
            }
        }

    }

    private void SpawnThreeInColOfThree()
    {
        ClearThreeRow();
        SpawnTopRowInThree();
        SpawnMidRowInThree();
        SpawnBotRowInThree();
    }

    private void ClearThreeRow()
    {
        triTopFilled = false;
        triMidFilled = false;
        triBotFilled = false;
    }

    private void SpawnTopRowInThree()
    {
        SpawnSector(sectorRadius * 2, true);
        triTopFilled = true;
    }

    private void SpawnMidRowInThree()
    {
        SpawnSector(0f, true);
        triMidFilled = true;
    }

    private void SpawnBotRowInThree()
    {
        SpawnSector(-sectorRadius * 2, true);
        triBotFilled = true;
    }

    private void SpawnColOfFour()
    {
        distancePopulated += distanceBetweenCols;
        int planetsToSpawn = Random.Range(MinPlanetsInFourCol(), 4); //Intentionally changed to spawn 1, 2 or 3, never 4

        if (planetsToSpawn == 1)
        {
            SpawnOneInColOfFour();
        }
        else if (planetsToSpawn == 2)
        {
            SpawnTwoInColOfFour();
        }
        else if (planetsToSpawn == 3)
        {
            SpawnThreeInColOfFour();
        }
        else
        {
            SpawnFourInColOfFour();
        }
    }

    private int MinPlanetsInFourCol()
    {
        if (triTopFilled && triBotFilled)
        {
            return 2;
        }
        return 1;
    }

    private void ClearFourRow()
    {
        quadTopFilled = false;
        quadTopMidFilled = false;
        quadBotMidFilled = false;
        quadBotFilled = false;
    }

    private void SpawnOneInColOfFour()
    {
        ClearFourRow();
        if (triTopFilled)
        {
            SpawnTopMidRowInFour();
        }
        else
        {
            SpawnBotMidRowInFour();
        }
    }

    private void SpawnTwoInColOfFour()
    {
        ClearFourRow();
        if (Random.Range(0, 2) == 1)
        {
            SpawnTopRowInFour();
            SpawnBotMidRowInFour();
        }
        else
        {
            SpawnTopMidRowInFour();
            SpawnBotRowInFour();
        }       
    }

    private void SpawnThreeInColOfFour()
    {
        ClearFourRow();
        int rand = Random.Range(0, 4);
        if (rand == 0)
        {
            SpawnTopRowInFour();
            SpawnTopMidRowInFour();
            SpawnBotMidRowInFour();
        }
        else if (rand == 1)
        {
            SpawnTopRowInFour();
            SpawnTopMidRowInFour();
            SpawnBotRowInFour();
        }
        else if (rand == 2)
        {
            SpawnTopRowInFour();
            SpawnBotMidRowInFour();
            SpawnBotRowInFour();
        }
        else
        {
            SpawnBotMidRowInFour();
            SpawnTopMidRowInFour();
            SpawnBotRowInFour();
        }
    }

    private void SpawnFourInColOfFour()
    {
        ClearFourRow();
        SpawnTopRowInFour();
        SpawnTopMidRowInFour();
        SpawnBotMidRowInFour();
        SpawnBotRowInFour();
    }

    private void SpawnTopRowInFour()
    {
        SpawnSector(sectorRadius * 3, true);
        quadTopFilled = true;
    }

    private void SpawnTopMidRowInFour()
    {
        SpawnSector(sectorRadius, true);
        quadTopMidFilled = true;
    }

    private void SpawnBotMidRowInFour()
    {
        SpawnSector(-sectorRadius, true);
        quadBotMidFilled = true;
    }

    private void SpawnBotRowInFour()
    {
        SpawnSector(-sectorRadius * 3, true);
        quadBotFilled = true;
    }

    private void SpawnSector(float height, bool hasPlanet)
    {
        sectors.Add(new Sector(new Vector2(distancePopulated, height), hasPlanet ? planets.TakeRandomPlanet() : null, sectorRadius));
    }
}
