using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AiPlayer : MonoBehaviour
{
    public Deck EnemyDeck;
    public Hand EnemyHand;
    public Mana EnemyMana;
    public Health EnemyHealth;
    public bool IsStunned = false;
    [SerializeField] private Button enemyHeroButton;
    [SerializeField] private float turnTime = 2f;
    private void Start()
    {
        SetHeroButtonUse();
        EnemyHealth = GetComponent<Health>();
        EnemyDeck.Shuffle();
        for (int i = 0; i < 3; i++)
        {
            EnemyHand.AddCard(EnemyDeck.Draw());
        }
    }

    public void StartTurn()
    {

        EnemyMana.StartTurn();
        EnemyHand.AddCard(EnemyDeck.Draw());

        foreach (CardScriptable card in EnemyHand.CardsInHand)
        {
            if (EnemyMana.SpendMana(card.ManaCost))
            {
                GameObject target = gameObject;
                foreach (CardEffect effect in card.Effects)
                {
                    if (effect.Type == CardEffect.EffectType.Summon)
                    {
                        target = gameObject;
                        break;
                    }
                    else if (effect.Type == CardEffect.EffectType.Damage)
                    {
                        target = GameManager.Instance.Player.gameObject;
                        break;
                    }
                }
                card.PlayCard(target);
                EnemyHand.RemoveCard(card);
                break;
            }
        }
        
        // Attack with minions after playing cards
        AttackWithMinions();
        
        StartCoroutine(endTurn());//wait so it doesn't mess with targeting and to later implement animations or effects
    }

    public void AttackWithMinions()
    {
        StartCoroutine(PerformMinionAttacksWithDelay());
    }
    
    private IEnumerator PerformMinionAttacksWithDelay()
    {
        List<GameObject> availableMinions = new List<GameObject>();
        
        // Get all enemy minions that can attack
        foreach (GameObject minionObj in GameManager.Instance.EnemyMinions)
        {
            Minion minion = minionObj.GetComponent<Minion>();
            if (minion != null && CanMinionAttack(minion))
            {
                availableMinions.Add(minionObj);
            }
        }
        
        // Attack with each available minion with delay between attacks
        foreach (GameObject minionObj in availableMinions)
        {
            Minion attackingMinion = minionObj.GetComponent<Minion>();
            
            // Try to attack player minions first, then hero
            GameObject target = FindBestTarget(attackingMinion);
            
            if (target != null)
            {
                // Highlight attacking minion briefly
                Material originalMaterial = attackingMinion.minionImage.material;
                Material highlightMaterial = new Material(originalMaterial);
                highlightMaterial.color = Color.red;
                attackingMinion.minionImage.material = highlightMaterial;
                
                yield return new WaitForSeconds(0.5f);
                
                if (target.GetComponent<Minion>() != null)
                {
                    // Attack minion
                    PerformMinionAttack(attackingMinion, target.GetComponent<Minion>());
                }
                else if (target == GameManager.Instance.Player.gameObject)
                {
                    // Attack hero
                    PerformHeroAttack(attackingMinion);
                }
                
                // Restore original color
                attackingMinion.minionImage.material = originalMaterial;
                
                // Delay between attacks
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
    
    private bool CanMinionAttack(Minion minion)
    {
        // Check if minion can attack this turn
        if (minion.HasStatus(Minion.MinionStatus.HasAttackedThisTurn))
            return false;
            
        // Check for summoning sickness (unless has Rush or Charge)
        if (minion.HasStatus(Minion.MinionStatus.JustSummoned) && 
            !minion.HasStatus(Minion.MinionStatus.HasRush) && 
            !minion.HasStatus(Minion.MinionStatus.CanAttackHero))
            return false;
            
        // Check if stunned
        if (minion.HasStatus(Minion.MinionStatus.IsStunned))
            return false;
            
        return true;
    }
    
    private GameObject FindBestTarget(Minion attackingMinion)
    {
        // First, check if we can attack player minions (prioritize Taunt minions)
        List<GameObject> playerMinions = new List<GameObject>(GameManager.Instance.Minions);
        
        // Check for Taunt minions first
        foreach (GameObject minionObj in playerMinions)
        {
            Minion minion = minionObj.GetComponent<Minion>();
            if (minion != null && minion.HasStatus(Minion.MinionStatus.HasTaunt) && !minion.HasStatus(Minion.MinionStatus.HasStealth))
            {
                return minionObj;
            }
        }
        
        // If no Taunt minions, attack the weakest non-stealth minion
        GameObject weakestMinion = null;
        int lowestHealth = int.MaxValue;
        
        foreach (GameObject minionObj in playerMinions)
        {
            Minion minion = minionObj.GetComponent<Minion>();
            if (minion != null && !minion.HasStatus(Minion.MinionStatus.HasStealth))
            {
                if (minion.Health < lowestHealth)
                {
                    lowestHealth = minion.Health;
                    weakestMinion = minionObj;
                }
            }
        }
        
        if (weakestMinion != null)
            return weakestMinion;
        
        // If no valid minion targets, check if we can attack the hero
        if (CanAttackHero(attackingMinion))
        {
            return GameManager.Instance.Player.gameObject;
        }
        
        return null;
    }
    
    private bool CanAttackHero(Minion minion)
    {
        // Check if minion can attack hero
        if (!minion.HasStatus(Minion.MinionStatus.CanAttackHero))
            return false;
            
        // Check if there are any Taunt minions on player's side
        foreach (GameObject minionObj in GameManager.Instance.Minions)
        {
            Minion playerMinion = minionObj.GetComponent<Minion>();
            if (playerMinion != null && playerMinion.HasStatus(Minion.MinionStatus.HasTaunt))
            {
                return false; // Can't attack hero while Taunt minions exist
            }
        }
        
        return true;
    }
    
    private void PerformMinionAttack(Minion attacker, Minion defender)
    {
        Debug.Log($"{attacker.name} attacks {defender.name}!");
        
        // Remove Stealth from attacker when it attacks
        if (attacker.HasStatus(Minion.MinionStatus.HasStealth))
        {
            attacker.SetStatus(Minion.MinionStatus.HasStealth, false);
            Debug.Log($"{attacker.name} loses Stealth after attacking!");
        }
        
        // Highlight the target briefly
        Material defenderOriginalMaterial = defender.minionImage.material;
        Material defenderHighlightMaterial = new Material(defenderOriginalMaterial);
        defenderHighlightMaterial.color = Color.yellow;
        defender.minionImage.material = defenderHighlightMaterial;
        
        int attackerAttack = attacker.Attack;
        int defenderAttack = defender.Attack;
        
        // Apply attacker damage to defender
        if (defender.HasStatus(Minion.MinionStatus.HasDivineShield))
        {
            defender.SetStatus(Minion.MinionStatus.HasDivineShield, false);
            Debug.Log($"{defender.name} lost its Divine Shield!");
        }
        else if (attacker.HasStatus(Minion.MinionStatus.HasPoisonous))
        {
            defender.Health = 0;
            Debug.Log($"{defender.name} was destroyed by Poisonous!");
        }
        else
        {
            defender.Health -= attackerAttack;
            Debug.Log($"{defender.name} takes {attackerAttack} damage!");
        }
        
        // Apply defender damage to attacker
        if (attacker.HasStatus(Minion.MinionStatus.HasDivineShield))
        {
            attacker.SetStatus(Minion.MinionStatus.HasDivineShield, false);
            Debug.Log($"{attacker.name} lost its Divine Shield!");
        }
        else if (defender.HasStatus(Minion.MinionStatus.HasPoisonous))
        {
            attacker.Health = 0;
            Debug.Log($"{attacker.name} was destroyed by Poisonous!");
        }
        else
        {
            attacker.Health -= defenderAttack;
            Debug.Log($"{attacker.name} takes {defenderAttack} damage!");
        }
        
        // Apply Life Drain effects
        if (attacker.HasStatus(Minion.MinionStatus.HasLifeDrain))
        {
            EnemyHealth.Heal(attackerAttack);
            Debug.Log($"{attacker.name} drains {attackerAttack} health to the enemy!");
        }
        
        if (defender.HasStatus(Minion.MinionStatus.HasLifeDrain))
        {
            GameManager.Instance.Player.PlayerHealth.Heal(defenderAttack);
            Debug.Log($"{defender.name} drains {defenderAttack} health to the player!");
        }
        
        // Mark attacker as having attacked
        attacker.SetStatus(Minion.MinionStatus.HasAttackedThisTurn, true);
        
        // Restore defender's original color after a short delay
        StartCoroutine(RestoreDefenderColor(defender, defenderOriginalMaterial));
    }
    
    private IEnumerator RestoreDefenderColor(Minion defender, Material originalMaterial)
    {
        yield return new WaitForSeconds(0.3f);
        if (defender != null && defender.minionImage != null)
        {
            defender.minionImage.material = originalMaterial;
        }
    }
    
    private void PerformHeroAttack(Minion attacker)
    {
        Debug.Log($"{attacker.name} attacks the player!");
        
        // Remove Stealth from attacker when it attacks
        if (attacker.HasStatus(Minion.MinionStatus.HasStealth))
        {
            attacker.SetStatus(Minion.MinionStatus.HasStealth, false);
            Debug.Log($"{attacker.name} loses Stealth after attacking!");
        }
        
        int attackerAttack = attacker.Attack;
        GameManager.Instance.Player.PlayerHealth.TakeDamage(attackerAttack);
        
        // Apply Life Drain effect
        if (attacker.HasStatus(Minion.MinionStatus.HasLifeDrain))
        {
            EnemyHealth.Heal(attackerAttack);
            Debug.Log($"{attacker.name} drains {attackerAttack} health to the enemy!");
        }
        
        // Mark attacker as having attacked
        attacker.SetStatus(Minion.MinionStatus.HasAttackedThisTurn, true);
    }

    public IEnumerator endTurn()
    {
        yield return new WaitForSeconds(turnTime);
        GameManager.Instance.EndTurnAi();
    }

    public void OnClick()
    {
        if (GameManager.Instance.IsPlayerTurn)
        {
            GameManager.Instance.AttackEnemyHero(this);
        }
    }

    public void SetHeroButtonUse()
    {

        enemyHeroButton.onClick.AddListener(OnClick);

    }
}
