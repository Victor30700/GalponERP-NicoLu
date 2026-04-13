namespace GalponERP.Application.Interfaces;

public interface IVoiceService
{
    Task<string> TranscribirAudioAsync(Stream audioStream, string fileName);
    Task<byte[]> SintetizarVozAsync(string texto);
}
