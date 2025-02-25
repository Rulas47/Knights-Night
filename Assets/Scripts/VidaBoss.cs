using UnityEngine;
using UnityEngine.UI;

public class VidaBoss : MonoBehaviour
{
    public Image rellenoVida;
    private GolemController golemController;
    private float vidaMaxima;
    
    void Start()
    {
        golemController = GameObject.Find("Golem").GetComponent<GolemController>();
        vidaMaxima = golemController.vida;
    }

    void Update()
    {
        rellenoVida.fillAmount = golemController.vida / vidaMaxima;
    }
}
