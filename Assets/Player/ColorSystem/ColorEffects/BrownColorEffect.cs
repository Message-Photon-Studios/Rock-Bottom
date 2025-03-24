using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The brown color effect
/// </summary>
[CreateAssetMenu( menuName = "Gameplay Color/Color Effect/BrownColorEffect")]
public class BrownColorEffect : ColorEffect
{
    [SerializeField] int uncoloredDamage;
    public override void Apply(GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power, bool forcePerspectivePlayer, int extraDamage)
    {

        EnemyStats enemy = enemyObj.GetComponent<EnemyStats>();
        if(enemy.GetColor() == null || enemy.GetColorAmmount() <= 0)
        {
            enemy.DamageEnemy(Mathf.RoundToInt(uncoloredDamage*power)+extraDamage);
            return;
        }

        GameObject instantiatedParticles = GameObject.Instantiate(particles, enemyObj.transform.position, enemyObj.transform.rotation);
        instantiatedParticles.GetComponent<ParticleSystem>().Play();
        Destroy(instantiatedParticles, instantiatedParticles.GetComponent<ParticleSystem>().main.duration*2);
        // Set enemy as parent of the particle system
        instantiatedParticles.transform.parent = enemyObj.transform;
        enemy.DamageEnemy(Mathf.RoundToInt(damage*power)+extraDamage);

        if (playerObj.GetComponent<ColorInventory>().shatteredPrism)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Cannot run coroutine outside of play mode");
                return;
            }
            RainbowCoWorker coWorker = new GameObject().AddComponent<RainbowCoWorker>();
            List<GameColor> colors = playerObj.GetComponent<ColorInventory>().GetColorsInInv();
            ColorLibrary colorLib = GameManager.instance.GetComponent<ColorLibrary>();
            foreach (GameColor color in colors.ToArray())
            {
                if (color == colorLib.rainbow) colors.Remove(color);
            }
            if (colors.Count <= 0) return;
            int spot = Random.Range(0, colors.Count);
            float colorPower = playerObj.GetComponent<ColorInventory>().GetColorBuff(colors[spot]);
            coWorker.Work(ApplyBonusEffect(colors[spot], enemyObj, impactPoint, playerObj, Mathf.Max(0.5f + colorPower * 0.5f, 0.1f) , forcePerspectivePlayer, extraDamage));
        }
    }

    private IEnumerator ApplyBonusEffect(GameColor color, GameObject enemyObj, Vector2 impactPoint, GameObject playerObj, float power, bool forcePerspectivePlayer, int extraDamage)
    {
        yield return new WaitForSeconds(0.2f);
        color.GetColorEffect().Apply(enemyObj, impactPoint, playerObj, power, forcePerspectivePlayer, extraDamage);
    }
}
