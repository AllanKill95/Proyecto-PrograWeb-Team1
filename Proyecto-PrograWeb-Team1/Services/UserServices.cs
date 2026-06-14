namespace Proyecto_PrograWeb_Team1.Services;
using Proyecto_PrograWeb_Team1.DTOs;
using Google.Cloud.Firestore;

public class UserServices
{

    private readonly FirebaseService _firebaseService; 
    public UserServices(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    
    }
    

//CONFIRMACION SI EL USUARIO EXISTE
    public async Task<bool> ConfirmUsuario(String Username)
    {
        // Busca que exista el usuario
        var collection = _firebaseService.GetCollection("users");
        var existing = await collection
            .WhereEqualTo("FullName", Username)
            .GetSnapshotAsync();

        return existing.Any();
        
    }

}