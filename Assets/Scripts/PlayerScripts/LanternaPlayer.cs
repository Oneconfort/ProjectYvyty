using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternaPlayer : MonoBehaviour
{
    public static LanternaPlayer lanternaPlayer;
    public GameObject lanterna, luzSpot, luzPoint, bauInteracao, caixa;
    Rigidbody rb;
    public bool ligar = false;
    float maxBateria = 100f, drenagemBateria = 1f;
    public float bateriaAtual = 0;

    enum DetectedType { None, Caixa, Plataforma }
    DetectedType detectedInIteration = DetectedType.None;

    public bool caixaDetectada = false, plataformaDetectada = false, paredeDetectada = false, naPosicao = false;
    public List<GameObject> detectedPlataformas = new List<GameObject>(); 



    void Awake()
    {
        if (lanternaPlayer == null)
        {
            lanternaPlayer = this;
        }

        bateriaAtual = SaveGame.data.flashlightBattery;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (GameController.controller.uiController.batterySlider != null)
        {
            GameController.controller.uiController.batterySlider.maxValue = maxBateria;
            GameController.controller.uiController.batterySlider.value = bateriaAtual;
        }
    }

    private void Update()
    {
        if (GameController.controller.uiController.visivelpause == true) return;
        InteragirLanterna();
        Bateria();
    }

    public void Lanterna()
    {
        if (InteracaoComItem.interacaoComItem.pegouLanterna == true)
        {
            ligar = !ligar;
            luzSpot.SetActive(ligar);
        }
    }

    public void LigarLuminaria()
    {
        luzSpot.SetActive(false);
        luzPoint.SetActive(ligar);
    }

    public void LigarLanterna()
    {
        luzSpot.SetActive(ligar);
        luzPoint.SetActive(false);
    }

    void InteragirLanterna()
    {
        string[] tags = { "Inimigo1", "Bau", "Caixa", "ParedeFalsa", "Plataforma" };
        List<GameObject> alvos = new List<GameObject>();

        foreach (string tag in tags)
        {
            GameObject[] objetosComTag = GameObject.FindGameObjectsWithTag(tag);
            alvos.AddRange(objetosComTag);
        }

        detectedInIteration = DetectedType.None;
        detectedPlataformas.Clear(); // Clear previous detections

        foreach (var alvoLanterna in alvos)
        {
            if (EstaNaFrenteEProximo(alvoLanterna.transform.position, alvoLanterna))
            {
                ProcessarAlvo(alvoLanterna);
            }
        }
        caixaDetectada = detectedInIteration == DetectedType.Caixa;
        plataformaDetectada = detectedInIteration == DetectedType.Plataforma;
    }

    void ProcessarAlvo(GameObject alvo)
    {
        switch (alvo.tag)
        {
            case "Inimigo1":
                Destroy(alvo, 0.5f);
                break;
            case "Bau":
                bauInteracao = alvo;
                if (bauInteracao != null)
                {
                    bauInteracao.SendMessage("SpawnItemBau", SendMessageOptions.DontRequireReceiver);
                    bauInteracao = null;
                }
                break;
            case "Caixa":
                detectedInIteration = DetectedType.Caixa;
                break;
            case "Plataforma":
                detectedInIteration = DetectedType.Plataforma;
                detectedPlataformas.Add(alvo);
                break;
            case "ParedeFalsa":
                paredeDetectada = true;
                break;
        }
    }

    bool EstaNaFrenteEProximo(Vector3 posicaoAlvo, GameObject alvo)
    {
        if (ligar && InteracaoComItem.interacaoComItem.pegouLanterna)
        {
            float lightRange = 1f; // How far the light should detect for colliders
            Collider[] colliders = new Collider[2]; // We probably should cache this rather than creating it every time it enters the 'if' statement

            int hits = Physics.OverlapSphereNonAlloc(transform.position + transform.forward * lightRange, lightRange, colliders);
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++) // Checking all game objects hit by the OverlapSphere. We could remove this 'for' if we add a layer for objects that can be catched by the light - such as the wall that hides a treasure. But for now this is a work around.
                {
                    if (colliders[i].gameObject == alvo) // Checking if the OverlapSphere hit the game object we want
                    {
                        return true;
                    }
                }
            }
        }
        if (ligar && InteracaoComItem.interacaoComItem.pegouLanterna == false)
        {
            Vector3 dir = posicaoAlvo - transform.position;
            bool isInSphere = dir.magnitude < 3.5f;
            return isInSphere;
        }
        return false;
    }

    void Bateria()
    {
        if (ligar)
        {
            DrenagemBateria();
        }
        if (GameController.controller.uiController.batterySlider != null)
        {
            GameController.controller.uiController.batterySlider.value = bateriaAtual;
        }

        if (bateriaAtual <= 0)
        {
            ligar = false;
            luzSpot.SetActive(false);
            luzSpot.SetActive(false);
        }
    }

    void DrenagemBateria()
    {
        bateriaAtual -= drenagemBateria * Time.deltaTime;
        bateriaAtual = Mathf.Clamp(bateriaAtual, 0, maxBateria);
    }

    public void RecargaBateria(float quantidade)
    {
        if (InteracaoComItem.interacaoComItem.pegouLanterna == true)
        {
            bateriaAtual += quantidade;
            bateriaAtual = Mathf.Clamp(bateriaAtual, 0, maxBateria);
        }
    }

    public void ReposicionarLanterna(GameObject alvo)
    {
        if (InteracaoComItem.interacaoComItem.pegouLanterna == true)
        {
            InteracaoComItem.interacaoComItem.DerrubarItem();
            naPosicao = true;
            rb.isKinematic = true;
            lanterna.transform.SetParent(alvo.transform);
            if (detectedInIteration != DetectedType.Caixa)
            {
                lanterna.transform.localPosition = new Vector3(-0.046f, 0.266f, 0f); 
            }
            else
            {
                lanterna.transform.localPosition = new Vector3(0, 2, 0.5F);
            }
            lanterna.transform.localRotation = Quaternion.identity;
        }

    }

    private void OnDrawGizmos()
    {
        float lightRange = 1f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * lightRange, lightRange);
    }
}

