namespace Proyecto_PrograWeb_Team1.Models;

public class User
{
    // Representar un usuario en el sistema 
    // Esta clase es lo que vamos a guardar / leer en FS
    
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    //La contraseña siempre va a ir hasheada, nunca en texto plano
    public String PasswordHash { get; set; } = string.Empty;
    
    //Colocar un rol por defecto
    public string Role { get; set; } = "User";
    
    //Para saber cuando se creo el registro
    public DateTime Created { get; set; } = DateTime.UtcNow;
    
    





}