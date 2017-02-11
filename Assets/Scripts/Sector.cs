using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector {

    private const float orbitRadius = 6f; //size of a planets backstop orbit pull radius so we can stay within the sector bounds 

    public Vector2 position;
    public Planet attachedPlanet;
    public float sectorRadius;

    public Sector(Vector2 position, Planet attachedPlanet, float radius)
    {
        this.position = position;
        this.attachedPlanet = attachedPlanet;
        this.sectorRadius = radius;

        if (attachedPlanet != null)
        {
            PositionPlanet();
        }
    }

    private void PositionPlanet()
    {
        float planetRadius = attachedPlanet.transform.localScale.x * orbitRadius;
        float placeableRadius = sectorRadius - planetRadius;

        float xOffset = Random.Range(0f, placeableRadius);
        float yOffset = Random.Range(0f, placeableRadius);
        attachedPlanet.transform.position = new Vector3(position.x + xOffset, position.y + yOffset, 0f);
    }


}
