namespace Proyecto_PrograWeb_Team1.Models;

public class Nota
{
    //Id del estudiante
    public string Id { get; set; }
    
    public string Nombre { get; set; } 
    
    //Hora
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    //Nota solo valores enteros
    public int NotaId { get; set; }
    
    
}