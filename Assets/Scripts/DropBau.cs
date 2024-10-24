using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropBau : MonoBehaviour
{
    public GameObject[] drops;
    public GameObject spawnDrop;
    public bool functionActivated = true;
    public GameObject tampaBau;
   
    public void SpawnItemBau()
    {
        if(functionActivated == true)
        {
           tampaBau.transform.Rotate(0, 0, -120);

            spawnDrop.SetActive(true);

            int chance = Random.Range(1, 100);

            int opcoes = Random.Range(0, drops.Length);
            GameObject novoPrefab = Instantiate(drops[opcoes], spawnDrop.transform.position, transform.rotation);

            Rigidbody rb = novoPrefab.GetComponent<Rigidbody>();
            Vector3 direction = spawnDrop.transform.forward;
            rb.AddForce(direction * 6, ForceMode.Impulse);

            Destroy(spawnDrop.gameObject);
            functionActivated = false;
           
        }
    }
}
