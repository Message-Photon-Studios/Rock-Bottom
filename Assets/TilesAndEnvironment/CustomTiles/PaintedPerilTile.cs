using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/PP Tile")]
public class PaintedPerilTile : RuleTile<PaintedPerilTile.Neighbor> {

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        //public const int Null = 3;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.This: return tile != null;
            case Neighbor.NotThis: {
                if(tile != null) return false;
                
                break;
            }
        }
        return base.RuleMatch(neighbor, tile);
    }
}