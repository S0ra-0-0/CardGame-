using UnityEngine;

public class AiPlayer : MonoBehaviour
{
    public Deck EnemyDeck;
    public Hand EnemyHand;
    public Mana EnemyMana;
    public Health EnemyHealth;
    public bool IsStunned = false;

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
        if (IsStunned)
        {
            IsStunned = false;
            Debug.Log($"{gameObject.name} is stunned and skips their turn!");
            GameManager.Instance.EndTurn();
            return;
        }

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
        GameManager.Instance.EndTurn();
    }

    public void OnClick()
    {
        if (GameManager.Instance.IsPlayerTurn)
        {
            GameManager.Instance.AttackEnemyHero(this);
        }
    }
}
