namespace Proyecto_PrograWeb_Team1.DTOs;

public class CrearDenunciaDto
{
    // Lo que el frontend manda cuando alguien crea un experimento
    // No vamos a incluir el userId porque lo vamos a sacar del token
    
    public string Title { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public bool Success { get; set; } = false;
}