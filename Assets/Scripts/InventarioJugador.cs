using UnityEngine;

public class InventarioJugador : MonoBehaviour
{
    public bool tieneHielo = false;

    [Header("Referencia donde aparece el hielo en la mano")]
    public Transform posicionHieloEnMano;

    private GameObject hieloInstanciado;

    // TOMAR HIELO
    public void TomarHielo(GameObject prefabHielo)
    {
        if (tieneHielo)
            return;

        tieneHielo = true;

        if (posicionHieloEnMano != null && prefabHielo != null)
        {
            hieloInstanciado = GameObject.Instantiate(prefabHielo, posicionHieloEnMano);
            hieloInstanciado.transform.localPosition = Vector3.zero;
            hieloInstanciado.transform.localRotation = Quaternion.identity;
        }

        Debug.Log("Jugador tomó un cubo de hielo.");
    }

    // COLOCAR HIELO EN LA OLLA
    public void ColocarHieloEnOlla(ControlFusion controlFusion)
    {
        if (!tieneHielo)
            return;

        tieneHielo = false;

        if (hieloInstanciado != null)
            Destroy(hieloInstanciado);

        Debug.Log("Jugador colocó el hielo en la olla.");
    }
}
