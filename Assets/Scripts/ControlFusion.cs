using UnityEngine;
using TMPro;

/// <summary>
/// Control del proceso de fusión: hielo -> agua.
/// Requiere que el jugador coloque el hielo en la olla.
/// </summary>
public class ControlFusion : MonoBehaviour
{
    [Header("Referencias de escena")]
    public GameObject hielo;                // Cubo de hielo dentro de la olla
    public GameObject agua;                 // Agua que aparece al derretirse
    public Light luzEstufa;                 // Luz indicadora de estufa encendida
    public TextMeshProUGUI textoUI;

    [Header("Ajustes")]
    public float velocidadAumentoTemp = 10f;
    public float velocidadDerretir = 0.005f;
    public float velocidadSubidaAgua = 0.05f;

    // -----------------------------
    // ESTADOS INTERNOS
    // -----------------------------
    private bool estufaEncendida = false;
    private bool hayHieloEnOlla = false;
    private bool transicionHieloAguaCompleta = false;

    private float temperatura = 0f;

    // ⬇️ ADICIÓN: límite máximo de temperatura
    private const float TEMPERATURA_MAXIMA = 150f;

    private Vector3 escalaInicialAgua;
    private Vector3 posicionInicialAgua;

    void Start()
    {
        if (agua != null)
        {
            escalaInicialAgua = agua.transform.localScale;
            posicionInicialAgua = agua.transform.position;
            agua.SetActive(false);
        }

        if (hielo != null)
            hielo.SetActive(false);

        if (luzEstufa != null)
            luzEstufa.enabled = false;

        if (textoUI != null)
            textoUI.text = "Temperatura: 0 °C";
    }

    void Update()
    {
        if (!hayHieloEnOlla)
            return;

        if (!estufaEncendida)
            return;

        // ⬇️ MODIFICACIÓN CONTROLADA (sin borrar nada):
        // Se limita la temperatura a 150 °C
        temperatura += Time.deltaTime * velocidadAumentoTemp;
        temperatura = Mathf.Min(temperatura, TEMPERATURA_MAXIMA);

        if (textoUI != null)
            textoUI.text = "Temperatura: " + (int)temperatura + " °C";

        // -----------------------------
        // DERRETIMIENTO DEL HIELO
        // -----------------------------
        if (hielo != null && hielo.activeSelf && temperatura >= 0f)
        {
            hielo.transform.localScale -= Vector3.one * (velocidadDerretir * Time.deltaTime);

            if (hielo.transform.localScale.x <= 0.05f && !transicionHieloAguaCompleta)
            {
                hielo.transform.localScale = Vector3.zero;
                hielo.SetActive(false);

                if (agua != null)
                {
                    agua.SetActive(true);
                    agua.transform.localScale = new Vector3(escalaInicialAgua.x, 0.01f, escalaInicialAgua.z);
                    agua.transform.position = posicionInicialAgua;
                }

                transicionHieloAguaCompleta = true;

                GestorSimulacionResultado.RegistrarResultado(
                    GestorSimulacion.idSimulacionActual,
                    "Temperatura final",
                    temperatura.ToString("F1"),
                    "°C"
                );

                int duracionReal = (int)(Time.time - tiempoInicioSimulacion);

                    GestorSimulacionFinalizar.FinalizarSimulacion(
                    GestorSimulacion.idSimulacionActual,
                    duracionReal
                );

                if (textoUI != null)
                {
                    textoUI.text = "¡EXPERIMENTO COMPLETADO!";
                    textoUI.color = Color.green; // Si usas TMP, puedes cambiar el color
                }

                Debug.Log("Simulación completada con exito. Datos enviados.");

                Invoke("CerrarAplicacion", 5f);
            }
        }

        // -----------------------------
        // SUBIDA DEL AGUA
        // -----------------------------
        if (agua != null && agua.activeSelf)
        {
            Vector3 esc = agua.transform.localScale;

            if (esc.y < escalaInicialAgua.y)
            {
                esc.y += Time.deltaTime * velocidadSubidaAgua;
                agua.transform.localScale = esc;

                Vector3 pos = agua.transform.position;
                pos.y += Time.deltaTime * velocidadSubidaAgua * 0.5f;
                agua.transform.position = pos;
            }
        }
    }

    // ----------------------------------------------------
    // EL JUGADOR COLOCA EL HIELO
    // ----------------------------------------------------

    private float tiempoInicioSimulacion;

    public void RecibirHielo()
    {
        
        if (GestorSimulacion.idSimulacionActual != -1)
        {
            Debug.Log("La simulación ya está activa (ID: " + GestorSimulacion.idSimulacionActual + "). No se creará otro registro.");
            return;
        }

        
        hayHieloEnOlla = true;

        if (hielo != null)
        {
            hielo.SetActive(true);
            hielo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        temperatura = 0f;
        transicionHieloAguaCompleta = false;

        if (agua != null)
            agua.SetActive(false);

        if (textoUI != null)
            textoUI.text = "Temperatura: 0 °C";

        
        tiempoInicioSimulacion = Time.time;

        
        GestorSimulacion.IniciarSimulacion(
            SesionUsuario.IdUsuario,
            "Fusion del hielo",
            "Proceso de cambio de estado solido a liquido",
            "VR"
        );

        Debug.Log("Enviando petición de inicio para el Usuario ID: " + SesionUsuario.IdUsuario);
    }

    // ----------------------------------------------------
    // ENCENDER / APAGAR LA ESTUFA
    // ----------------------------------------------------
    public void ToggleEstufa()
    {
        estufaEncendida = !estufaEncendida;

        if (luzEstufa != null)
            luzEstufa.enabled = estufaEncendida;

        GestorSimulacionEvento.RegistrarEvento(
            GestorSimulacion.idSimulacionActual,
            estufaEncendida ? "Estufa encendida" : "Estufa apagada",
            "El usuario interactuo con la estufa",
            (int)Time.time
        );

        Debug.Log("Estufa: " + (estufaEncendida ? "ENCENDIDA" : "APAGADA"));
    }

    public bool EstufaEncendida => estufaEncendida;

    // ----------------------------------------------------
    // RESET DEL PROCESO
    // ----------------------------------------------------
    public void ResetProceso()
    {
        hayHieloEnOlla = false;
        estufaEncendida = false;
        transicionHieloAguaCompleta = false;
        temperatura = 0;

        if (hielo != null)
        {
            hielo.SetActive(false);
            hielo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        if (agua != null)
            agua.SetActive(false);

        if (luzEstufa != null)
            luzEstufa.enabled = false;

        if (textoUI != null)
            textoUI.text = "Temperatura: 0 °C";

        int duracionReal = (int)(Time.time - tiempoInicioSimulacion);

        GestorSimulacionFinalizar.FinalizarSimulacion(
            GestorSimulacion.idSimulacionActual,
            (int)Time.time
        );
    }

    void CerrarAplicacion()
    {
        Application.Quit();
    }
}