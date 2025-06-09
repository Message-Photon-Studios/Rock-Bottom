using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapMask : MonoBehaviour
{
    //-- UnityTilemapMask - Open source repo hosted on github
    //-- License: MIT

    public GameObject maskCell;
    [HideInInspector]
    public GameObject[] maskParentsObj; // Must be public for correct handling of game/editor destroying

    public void GenerateMask()
    {
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
        if (maskParentsObj != null)
        {
            for (int i = 0; i < maskParentsObj.Length; i++)
            {
                if(maskParentsObj[i] != null)
                    DestroyImmediate(maskParentsObj[i]);
            }
        }

        maskParentsObj = new GameObject[tilemaps.Length];

        for (int i = 0; i < tilemaps.Length; i++)
        {
            Tilemap tilemap = tilemaps[i];

            if (tilemap.GetComponent<TilemapRenderer>().sortingLayerName != "Default" && tilemap.GetComponent<TilemapRenderer>().sortingLayerName != "Front Decoration") continue;

            Vector3Int startCoord = tilemap.origin;
            Vector3Int size = tilemap.size;

            maskParentsObj[i] = new GameObject("TilemapMask");
            maskParentsObj[i].transform.parent = tilemap.transform;

            //Iterate over each cell
            for (int x = startCoord.x; x < startCoord.x + size.x; x++)
            {
                for (int y = startCoord.y; y < startCoord.y + size.y; y++)
                {
                    //Check if cell isn't empty
                    if (tilemap.GetTile(new Vector3Int(x, y, startCoord.z)) != null)
                    {
                        //Create maskCell on the cell coords
                        Vector3 coord = tilemap.CellToWorld(new Vector3Int(x, y, startCoord.z)) + new Vector3(0.5f, 0.5f, 0) - new Vector3(0.05f, 0.05f, 0f);
                        Sprite sprite = tilemap.GetSprite(new Vector3Int(x, y, startCoord.z));
                        if (sprite == null) continue;
                        GameObject cell = Instantiate(maskCell, coord, Quaternion.identity, maskParentsObj[i].transform);
                        cell.GetComponent<SpriteMask>().sprite = sprite;
                    }
                }
            }
        }
    }
}

