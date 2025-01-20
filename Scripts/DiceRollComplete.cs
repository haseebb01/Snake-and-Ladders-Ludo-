using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DiceRollComplete : MonoBehaviour
{
    public float rollPerMilliSec = 0.2f;
    public Sprite[] Dices;
    private void OnEnable()
    {
        // StartCoroutine(RollDice());
    }
    public IEnumerator RollDice(int rollValue)
    {
        int count = 6;
        LeanTween.scale(gameObject, new Vector3(1.25f, 1.25f, 1f), (rollPerMilliSec - 0.02f) * 6);

        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(rollPerMilliSec);
            gameObject.GetComponent<Image>().sprite = Dices[i];
        }

        StartCoroutine(RollDiceBacksword());
        FinishAnim(rollValue);
        gameObject.SetActive(false);
    }

    public IEnumerator RollDiceOnNetwork(int value)
    {
        int count = 6;
        LeanTween.scale(gameObject, new Vector3(1.25f, 1.25f, 1f), (rollPerMilliSec - 0.02f) * 6);
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(rollPerMilliSec);
            gameObject.GetComponent<Image>().sprite = Dices[i];
        }
        StartCoroutine(RollDiceBacksword());
        DiceController.Instance.DiceValueOnNetwork(value);
        gameObject.SetActive(false);
    }

    IEnumerator RollDiceBacksword()
    {
        int count = 6;
        LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f, 1f), (rollPerMilliSec - 0.02f) * 6);

        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(rollPerMilliSec);
            gameObject.GetComponent<Image>().sprite = Dices[i];
        }
    }
    public void FinishAnim(int value)
    {
        DiceController.Instance.RollAnimComplete(value);
    }
}
