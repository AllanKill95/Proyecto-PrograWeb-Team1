namespace Proyecto_PrograWeb_Team1.Models;

public class Denuncia
{
    
    //Representa un las denuncias que se realizan
    //Funcionalidad principal depues de hacer el login
    
    public string Id { get; set; }
    
    //Titulo de lo que se intenta hacer
    public string Title { get; set; } = string.Empty;
    
    //Comentaarios de la denuncia
    public string Comment { get; set; } = string.Empty;
    
    //El resultado de si funciona o no
    public string Result { get; set; } = string.Empty;
    
    //Usuario que creo el experimento / Pruebas
    public string UserId { get; set; } = string.Empty;
    
    //Exito o fracasp
    public bool Success { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    
    
    
    
    
}
