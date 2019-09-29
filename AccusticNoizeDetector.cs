using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccusticNoizeDetector : MonoBehaviour
{
    [SerializeField]
    private float addNoizePerSec;
    [SerializeField]
    private float subNoizePerSec;
    [SerializeField]
    private float detectionVal;

    [SerializeField]
    private Slider noizeSlider;

    private Character character;
    private float noize = 0;

    private GameField gameField;
    public void Initialize(GameField gameField)
    {
        this.gameField = gameField;
        character = gameField.characterInstance;
        character.MoveStartEvent += StartNoizing;
        character.MoveEndEvent += EndNoizing;
        noize = 0;
        isNoizing = false;
        noizeSlider.value = 0;

    }
    private bool isNoizing = false;
    private void StartNoizing()
    {
        isNoizing = true;
    }
    private void EndNoizing()
    {
        isNoizing = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (isNoizing && GameField.isGame)
        {
            noize += addNoizePerSec * Time.deltaTime;
           

            if (noize >= detectionVal)
            {
                gameField.enemyInstance1.PlayerDetected();
                gameField.enemyInstance2.PlayerDetected();
            }

        }
        else
        {
            if(noize > 0)
            {
                noize -= subNoizePerSec * Time.deltaTime;
            }
        }
        if(noize > 0)
        {
            noizeSlider.value = noize / detectionVal;
        }
    }
}
