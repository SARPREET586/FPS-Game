using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
// quando dobbiamo usare il vector 3 e quaternion bisogna eliminare system.numeric(e di c#) perche le due classi si sovrappongono
public class SSWeapon : MonoBehaviour
{
    private bool isFiring = false;
    private bool isReloading = false;
    public float reloadTime = 2;
    private float reloadCounter = 0;
    private float counter = 0;
    public float firingTime = 1;


    public Transform muzzlePoint;
    public GameObject muzzleEffectPrefab;
    public GameObject gunHitEffectPrefab;
    public GameObject gunDecal;
    public AudioSource gunSound;
    public Transform raycastStartSpot;

    // Range
    public float range = 500f;
    public float damage = 50;

    public Animator anim;


    public bool constantFire = false; // if constant fire is on then it is an automatic gun, otherwise it is a single fire gun


    public int bulletInClip = 40;
    public int bulletsToReload = 40;
    void Start()
    {
        
    }

    void doConstantFire()
    {
        if (isFiring == false)
        {
            if (Input.GetMouseButtonDown(0))// only the fire mode will be activated
            {
                if (bulletInClip <= 0)
                {
                    Debug.Log("Magazine empty");
                    return;
                }

                Debug.Log("Fire");
                isFiring = true;// to get the trigger  on
            }

            if (isReloading == false)
            {// per ricaricare sempre anche se hai dei colpi rimasti
                if (Input.GetKeyDown(KeyCode.R))
                {
                    isReloading = true;
                }
            }

        }

        else
        {   // here the trigger is on
            // this section is when firing is on
            if (bulletInClip <= 0)
            {
                counter = 0;
                isFiring = false;
                return;
            }


            counter += Time.deltaTime;
            if (counter > firingTime)
            {
                counter = 0;
                FireOneShot();// questo e il counter per esmpio ogni 0.5 secondi spara un colpo e dopo si resetta
            }

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Fire");
                isFiring = false;
            }


        }
    }

    void doSingleFire()
    {
        if (isFiring == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Fire");
                if (bulletInClip <= 0)
                {
                    Debug.Log("Magazine empty");
                    return;
                }

                isFiring = true;
                FireOneShot();
            }


            if (isReloading == false)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    isReloading = true;
                    //anim2.Play("Reload");
                }
            }

        }
        else
        {
            counter += Time.deltaTime;
            if (counter > firingTime)
            {
                counter = 0;
                isFiring = false;

                if (anim != null)
                {
                    anim.SetInteger("flag", 0);
                }

            }
        }
    }

    void Update()
    {
       if( constantFire)
        {
            doConstantFire();

        }

        else
        {
            doSingleFire();
        }


        if (isReloading == true)
        {
            reloadCounter += Time.deltaTime;
            if (reloadCounter > reloadTime)
            {
                reloadCounter = 0;


                bulletInClip = bulletsToReload;
                isReloading = false;


            }
        }




    }

    private void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle(GUI.skin.GetStyle("label"));
        myStyle.fontSize = 32;

        GUI.Label(new Rect(20, 20, 600, 40), "BulletsInClip: " + bulletInClip, myStyle);
    }


    void FireOneShot()
    {
        Instantiate(muzzleEffectPrefab, muzzlePoint.position, muzzlePoint.rotation);// instantiate prende direttamente il prefab e lo mette sul object in questo caso sul vettore z della pistola

        if (gunSound != null)
        {
            gunSound.Play();
        }
        bulletInClip--;

        if (anim != null)// per non far sovvrascrivere le animazioni di oggetti diversi ad esempio la stick animation
        {
            anim.SetInteger("flag", 1);
        }

        // The ray that will be used for this shot
        Vector3 direction = raycastStartSpot.forward;// ci dara la z posizione in avanti
        Ray ray = new Ray(raycastStartSpot.position, direction);// da dove e in quale direzioni inizia il raycast
        RaycastHit hit;// dove si e stata la collisione dove si spara

        if (Physics.Raycast(ray, out hit, range))// nella funzione ci va sempre la copia,out non manda la copia della variabile ma l'originale e ritorna anche l'originale//in questo caso usiamo out per cambiare continuamente l informazione
        {

            // Damage
            //hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -damage, SendMessageOptions.DontRequireReceiver);
            Instantiate(gunHitEffectPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

            GameObject g = Instantiate(gunDecal, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            g.transform.parent = hit.collider.transform;

            SSHealth h = hit.collider.gameObject.GetComponent<SSHealth>();
            if (h != null)
            {
                h.ApplyDamage(damage);
            }

        }

    }
}
