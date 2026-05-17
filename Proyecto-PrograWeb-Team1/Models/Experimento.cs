namespace Proyecto_PrograWeb_Team1.Models;

public class Experimento
{
    //Representa un experimento o pruebas que un usuario esta realizando 
    //Funcionalidad principal depues de hacer el login
    
    public string Id { get; set; }
    
    //Titulo de lo que se intenta hacer
    public string Tittle { get; set; } = string.Empty;
    
    //El resultado de si funciona o no
    public string Result { get; set; } = string.Empty;
    
    //Usuario que creo el experimento / Pruebas
    public string UserId { get; set; } = string.Empty;
    
    //Exito o fracasp
    public bool Success { get; set; } = false;
    
    public DateTime Created { get; set; } = DateTime.UtcNow;
    
    
    
    
    
    
}