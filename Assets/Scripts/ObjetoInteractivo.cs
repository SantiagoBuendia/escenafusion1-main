using UnityEngine;

/// <summary>
/// Objeto interactivo: nevera, olla, botón estufa.
/// Maneja mensajes e interacción.
/// </summary>
public class ObjetoInteractivo : MonoBehaviour
{
    [TextArea]
    public string mensajeExplicacion;

    [Header("Tipo de Objeto")]
    public bool esNevera;
    public bool esOlla;
    public bool esBotonEstufa;

    [Header("Mensajes al interactuar")]
    [TextArea] public string mensajeAccionNevera = "Has tomado un cubo de hielo.";
    [TextArea] public string mensajeAccionOlla = "Has colocado el hielo en la olla.";
    [TextArea] public string mensajeAccionEstufaEncendida = "La estufa está encendida.";
    [TextArea] public string mensajeAccionEstufaApagada = "La estufa se ha apagado.";
    [TextArea] public string mensajeSinAccion = "No hay acción para este objeto.";

    [Header("Referencias")]
    public GameObject prefabHielo;
    public ControlFusion controlFusion; // referencia directa a la olla

    MensajeVRPro mensajeVR;
    InventarioJugador jugador;
    UIExplicacionLaboratorio uiExplicacion;

    void Awake()
    {
        mensajeVR = FindObjectOfType<MensajeVRPro>();
        jugador = FindObjectOfType<InventarioJugador>();
        uiExplicacion = FindObjectOfType<UIExplicacionLaboratorio>();
    }

    // HOVER
    public void OnHoverEnter()
    {
        if (!string.IsNullOrEmpty(mensajeExplicacion))
        {
            mensajeVR?.MostrarMensaje(mensajeExplicacion, 999f);
            uiExplicacion?.MostrarExplicacion(mensajeExplicacion);
        }
    }

    public void OnHoverExit()
    {
        mensajeVR?.OcultarAhora();
    }

    // INTERACTUAR
    public void Interactuar()
    {
        Debug.Log("Interacción con " + gameObject.name);

        if (esNevera)
        {
            jugador.TomarHielo(prefabHielo);
            mensajeVR?.MostrarMensaje(mensajeAccionNevera);
            GestorSimulacionEvento.RegistrarEvento(
                GestorSimulacion.idSimulacionActual,
                "Hielo en olla",
                "El usuario coloco el hielo sobre la estufa/olla",
                (int)Time.time
            );

            return;

        }

        if (esOlla && controlFusion != null)
        {
            jugador.ColocarHieloEnOlla(controlFusion);
            controlFusion.RecibirHielo();
            mensajeVR?.MostrarMensaje(mensajeAccionOlla);

            GestorSimulacionEvento.RegistrarEvento(
                GestorSimulacion.idSimulacionActual,
                "Hielo en olla",
                "El usuario coloco el hielo sobre la estufa/olla",
                (int)Time.time
            );


            return;

        }

        if (esBotonEstufa)
        {
            controlFusion.ToggleEstufa();

            if (controlFusion.EstufaEncendida)
            {
                mensajeVR?.MostrarMensaje(mensajeAccionEstufaEncendida);

                GestorSimulacionEvento.RegistrarEvento(GestorSimulacion.idSimulacionActual, "Encendido estufa", "El usuario pulso el boton de encendido", (int)Time.time);
            }
            else
            {

                GestorSimulacionEvento.RegistrarEvento(GestorSimulacion.idSimulacionActual, "Apagado estufa", "El usuario apago la estufa", (int)Time.time);
                mensajeVR?.MostrarMensaje(mensajeAccionEstufaApagada);
            }
            return;
        }

        mensajeVR?.MostrarMensaje(mensajeSinAccion);
    }
}
