using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class CintaController : MonoBehaviour
{
    [Header("Configuración de animaciones")]
    [SerializeField] private List<AnimatorController> animations1;
    [SerializeField] private List<AnimatorController> animations2;

    [Header("Lista de objetos controlados (cintas)")]
    [SerializeField] private List<GameObject> objetos; // varios objeto1

    [Header("Otros")]
    [SerializeField] public float time = 2f;

    private List<ConveyorBelt> conveyorBelts = new List<ConveyorBelt>();
    private List<List<Animator>> animatorsList = new List<List<Animator>>();

    public bool firstAnim = true;
    private bool isChanging = false;

    private void Awake()
    {
        // Recolectamos todos los belts y sus animators
        foreach (var obj in objetos)
        {
            if (obj == null) continue;

            var belt = obj.GetComponent<ConveyorBelt>();
            if (belt != null)
                conveyorBelts.Add(belt);

            var animators = new List<Animator>(obj.GetComponentsInChildren<Animator>());
            animatorsList.Add(animators);
        }
    }

    public void ChangeStatus()
    {
        if (isChanging) return;
        StartCoroutine(ChangeSequence());
    }

    private IEnumerator ChangeSequence()
    {
        isChanging = true;

        bool initialState = firstAnim;

        Change();
        yield return new WaitForSeconds(time);

        firstAnim = initialState;
        ApplyAnimationState(firstAnim);

        isChanging = false;
    }

    private void ApplyAnimationState(bool useFirstAnim)
    {
        var changed = useFirstAnim ? animations1 : animations2;

        for (int i = 0; i < animatorsList.Count; i++)
        {
            var animators = animatorsList[i];

            if (animators.Count == 0) continue;

            // Aplicar las animaciones correspondientes
            animators[0].runtimeAnimatorController = changed[0];
            for (int j = 1; j < animators.Count - 1; j++)
                animators[j].runtimeAnimatorController = changed[1];
            animators[animators.Count - 1].runtimeAnimatorController = changed[2];
        }

        // Cambiar dirección de todas las cintas
        foreach (var belt in conveyorBelts)
            belt.moveRight = useFirstAnim;
    }

    public void Change()
    {
        firstAnim = !firstAnim;
        ApplyAnimationState(firstAnim);
    }
}
