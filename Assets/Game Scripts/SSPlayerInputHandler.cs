using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SSPlayerInputHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Vector3 GetMoveInput()
    {
        //in pratica qui stiamo settando l'input che dobbiamo dare quando ci volgiamo muovere tramite la script salvata SSGameConstants
        Vector3 move = new Vector3(Input.GetAxisRaw(SSGameConstants.k_AxisNameHorizontal), 0f, Input.GetAxisRaw(SSGameConstants.k_AxisNameVertical));
        move = Vector3.ClampMagnitude(move, 1);

        return move;
    }

    public bool GetJumpInputDown()
    {

        if (CanProcessInput())
        {
            return Input.GetButtonDown(SSGameConstants.k_ButtonNameJump);
        }

        return false;
    }

    public bool GetCrouchingInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown(SSGameConstants.k_ButtonNameCrouch);
        }

        return false;
    }

    
    public float GetLookInputsHorizontal()
    {
        return GetMouseOrStickLookAxis(SSGameConstants.k_MouseAxisNameHorizontal, SSGameConstants.k_AxisNameJoystickLookHorizontal);
    }

    public float GetLookInputsVertical()
    {
        return GetMouseOrStickLookAxis(SSGameConstants.k_MouseAxisNameVertical, SSGameConstants.k_AxisNameJoystickLookHorizontal);
    }

    public bool CanProcessInput()//** da capire
    {
        //return Cursor.lockState == CursorLockMode.Locked && !m_GameFlowManager.gameIsEnding;
        return true;//per bloccare l'input che si da durante la pausa del gioco ad esempio si si pare l'inventario e necessario bloccare il giocatore per eseguire altre azioni

    }


    float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName)//** da capire
    {
        if (CanProcessInput())
        {
            float i = Input.GetAxisRaw(mouseInputName);

            //// handle inverting vertical input
            //if (invertYAxis)
            //    i *= -1f;

            //// apply sensitivity multiplier
            //i *= lookSensitivity;

            i *= 0.01f;

            return i;
        }

        return 0f;
    }
}
