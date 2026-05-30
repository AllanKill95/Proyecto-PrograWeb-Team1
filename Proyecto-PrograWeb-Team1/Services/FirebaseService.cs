using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace Proyecto_PrograWeb_Team1.Services;

public class FirebaseService
{
    // Este servicio es el puente entre nuestra app y FB
    // Lo que vamos a hablar con FS pasa por aqui
    
    private readonly FirestoreDb _firestoreDb;

    public FirebaseService()
    {
        // Decirle a FB donde esta el archivo con las credenciales
        // Usar la ruta relativa 
        var credentialPath = Path.Combine(AppContext.BaseDirectory, "Config", "firebase-credentials.json");
        
        // Esta es una variable de entorno que usa el SDK de G, para autenticarse
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
        
        // Ahora, aqui colocamos el project id 
        _firestoreDb = FirestoreDb.Create("proyecto-847");
    }
    
    // Devuelve una referencia de una coleccion
    public CollectionReference GetCollection(string collectionName)
    {
        return _firestoreDb.Collection(collectionName);
    }
}