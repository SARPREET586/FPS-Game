using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSWeaponManager : MonoBehaviour
{
    public GameObject[] weapons;
    int selectIndex = 0;

  
    void Start()
    {
        SelectWeapon(0);
    }

    void SelectWeapon(int index)
    {
        // new index = 1, si = 0
        weapons[selectIndex].SetActive(false);
        Debug.Log("hidden array value : " + selectIndex);

        selectIndex = index;

        weapons[selectIndex].SetActive(true);
        Debug.Log("show array value: " + selectIndex);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectWeapon(1);
        }

    }
}
