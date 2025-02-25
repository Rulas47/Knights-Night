using UnityEngine;
using UnityEngine.UI;


public class HealthController : MonoBehaviour
{
    public Image rellenoBarraVida;
    private KnightController knightController;
    private float vidaMaxima;

    void Start()
    {
        knightController = GameObject.Find("Knight").GetComponent<KnightController>();
        vidaMaxima = knightController.vida;
    }

    void Update()
    {
        rellenoBarraVida.fillAmount = knightController.vida / vidaMaxima;
    }
}
