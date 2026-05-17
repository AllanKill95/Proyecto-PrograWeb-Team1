using System.Runtime;
using Google.Cloud.Firestore;

namespace Proyecto_PrograWeb_Team1.Services;

public class FirebseServices
{
    //Este servicio es el puente entre nuestra app y FB
    //Lo que vamos a hablar con FS pasa por aqui
    
    private readonly FirestoreDb _firestoreDb;

    public FirebseServices()
    {
        //Decirle a FB donde esta el archivo con las credenciales
        //Usar la ruta relativa
        var credentialsPath = Path.Combine(AppContext.BaseDirectory, "Config", "firebase-credentials.json");
        
     
        
    }
}