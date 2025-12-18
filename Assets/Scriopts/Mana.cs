using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    public int CurrentMana { get; private set; }
    public int MaxMana = 10;
    public int TurnCount = 0;
    public Image[] ManaCrystals;
    public TextMeshProUGUI ManaText;


    private void Start()
    {
        CurrentMana = 1;
        TurnCount = 1;
        ManaText.text = CurrentMana + "/" + MaxMana;
    }

    public void StartTurn()
    {
        TurnCount++;
        foreach (Image crystal in ManaCrystals)
        {
            crystal.enabled = false;
        }
        for (int i = 0; i < Mathf.Min(TurnCount, MaxMana); i++)
        {
            ManaCrystals[i].enabled = true;
        }
        CurrentMana = Mathf.Min(TurnCount, MaxMana);
        ManaText.text = CurrentMana + "/" + MaxMana;
    }

    public bool SpendMana(int cost)
    {
        if (CurrentMana >= cost)
        {
            CurrentMana -= cost;
            ManaText.text = CurrentMana + "/" + MaxMana;
            return true;
        }
        return false;
    }



}
