namespace GalponERP.Application.Interfaces;

/// <summary>
/// Define el servicio de voz que actúa como puente para la orquestación de IA en n8n.
/// </summary>
public interface IVoiceService
{
    /// <summary>
    /// Transcribe un flujo de audio delegando el procesamiento a n8n.
    /// </summary>
    Task<string> TranscribirAudioAsync(Stream audioStream, string fileName);

    /// <summary>
    /// Sintetiza voz a partir de texto delegando la generación a n8n.
    /// </summary>
    Task<byte[]> SintetizarVozAsync(string texto);
}
