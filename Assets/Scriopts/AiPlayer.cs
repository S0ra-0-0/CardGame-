using System.Collections;
using UnityEngine;

public class AiPlayer : MonoBehaviour
{
    public Deck EnemyDeck;
    public Hand EnemyHand;
    public Mana EnemyMana;
    public Health EnemyHealth;
    public bool IsStunned = false;
    [SerializeField] private float turnTime = 2f;
    private void Start()
    {
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
        StartCoroutine(endTurn());//wait so it doesn't mess with targeting and to later implement animations or effects
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
}
