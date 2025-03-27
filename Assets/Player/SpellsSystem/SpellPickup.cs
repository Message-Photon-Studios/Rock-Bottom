using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEditor.Localization.Plugins.XLIFF.V20;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class SpellPickup : InteractionObject
{
    [SerializeField] int inspirationRequired = 0;
    [SerializeField] float spawnChance = 1f;
    [SerializeField] bool needsPayment;
    [SerializeField] bool lockBottleAfterSwap = false;
    [SerializeField] ColorSpell colorSpell;
    [SerializeField] PickUpCanvasController pickUpController;
    [SerializeField] Collider2D collider;
    Rigidbody2D body;
    SpriteRenderer spriteRenderer;
    ColorInventory inventory;
    ItemInventory itemInventory;
    bool bought = false;


    protected override void Start()
    {
        base.Start();
        foreach (Collider2D coll in Player.instance.gameObject.GetComponentsInChildren<Collider2D>())
        {
            Physics2D.IgnoreCollision(collider, coll);
        }
        //Physics2D.IgnoreCollision(collider, GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());

        body = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Sets the color spell for this spawnpoint
    /// </summary>
    /// <param name="setItem"></param>
    public void SetSpell(ColorSpell setSpell)
    {
        if (colorSpell == null || bought) this.colorSpell = setSpell;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = colorSpell.GetBottleSprite().smallSprite;

        inventory = Player.instance.colorInventory;
        itemInventory = Player.instance.playerInventory;
    }
    /// <summary>
    /// Randomly destroys the spawn point depending on the initial conditions
    /// </summary>
    public void RandomSpawnDestroy()
    {
        if(spawnChance < 1f)
        {
            float rng = UnityEngine.Random.Range(0,1f);
            if(rng > spawnChance)
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }
    }

    protected override void PlayerClose (bool isClose)
    {
        if (inventory == null) inventory = Player.instance.GetComponent<ColorInventory>();
        if(isClose)
        {
            if(bought && lockBottleAfterSwap) return;
            if(inspirationRequired > GameManager.instance.GetInspiration())
            {
                //TODO Add text about it being locked or something
                return;
            }

            pickUpController.SetBottle(this);
        } else pickUpController.CloseUi();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Physics2D.Raycast((Vector2)transform.position, Vector2.down, 0.36f, GameManager.instance.maskLibrary.onlyGround)) body.gravityScale = 0;
        
    }

    protected override void PlayerInteract()
    {
        if(needsPayment && !bought && !itemInventory.PayCost(colorSpell.spellCost)) return;

        bought = true;
        PlayerCombatSystem pcs = Player.instance.playerCombatSystem;
        
        if(!pcs.pickUpSpellMode)
        {
            pcs.SpellPickup(true, this);
            Player.instance.playerMovement.movementRoot.SetTotalRoot("pickUpSpell", true);
        } else if(!lockBottleAfterSwap)
        {
            pcs.SpellPickup(false, null);
            Player.instance.playerMovement.movementRoot.SetTotalRoot("pickUpSpell", false);
        }
    }

    /// <summary>
    /// Is called when this color spell is picked up
    /// </summary>
    public void PickedUp(int slotIndex)
    {
        Player.instance.playerMovement.movementRoot.SetTotalRoot("pickUpSpell", false);
        bought = true;
        ColorSpell tmp = inventory.GetColorSpell(slotIndex);
        inventory.ChangeColorSpell(slotIndex, colorSpell);
        needsPayment = false;
        SetSpell(tmp);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        transform.position = player.transform.position;
        body.velocity = new Vector2(0,0);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(player.GetComponent<PlayerMovement>().lookDir * 200, 500));
        body.gravityScale = 2;
        if(lockBottleAfterSwap) 
        {
            pickUpController.CloseUi();
        } else 
        {
            pickUpController.SetBottle(this);
        }
    }

    /// <summary>
    /// Returns the spawnpoints color spell;
    /// </summary>
    /// <returns></returns>
    public ColorSpell GetSpell()
    {
        return colorSpell;
    }

    /// <summary>
    /// Returns true if the spell needs payment
    /// </summary>
    /// <returns></returns>
    public bool GetNeedsPayement()
    {
        return needsPayment;
    }
}
