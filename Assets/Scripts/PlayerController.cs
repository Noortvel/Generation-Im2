using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Character controlledCharacter
    {
        get;
        set;
    }
    void Awake()
    {
        controlledCharacter = GetComponent<Character>();
        if(controlledCharacter == null)
        {
            print("Player Controller need Character component");
        }
    }
    void Update()
    {
        InputControllUpdate();
    }
    private void InputControllUpdate()
    {
        if (Input.GetKey(KeyCode.S))
        {
            controlledCharacter.MoveDown();
        }
        if (Input.GetKey(KeyCode.W) )
        {
            controlledCharacter.MoveUp();
        }
        if (Input.GetKey(KeyCode.D) )
        {
            controlledCharacter.MoveRight();
        }
        if (Input.GetKey(KeyCode.A) )
        {
            controlledCharacter.MoveLeft();
        }
    }
}
