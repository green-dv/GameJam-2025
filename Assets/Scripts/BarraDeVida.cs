using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    [Header("Referencia UI")]
    public Slider slider;
    public Gradient gradient;
    public Image fill; 

    public void InicializarBarradeVida(float vidaMax)
    {
        slider.maxValue = vidaMax;
        slider.value = vidaMax;

        if (fill != null && gradient != null)
            fill.color = gradient.Evaluate(1f);
    }

    public void CambiarVidaActual(float vidaActual)
    {
        slider.value = vidaActual;

        if (fill != null && gradient != null)
            fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
