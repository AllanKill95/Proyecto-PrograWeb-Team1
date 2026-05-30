using Proyecto_PrograWeb_Team1.Models;
using Proyecto_PrograWeb_Team1.DTOs;

namespace Proyecto_PrograWeb_Team1.Services;

    public class NotaService
{
    //Manejamos lo relacionado con los experimentos del usuario
    
    private readonly FirebaseService _firebaseService;

    public NotaService(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<Nota> Create(CrearNotaDto dto, string userId)
    {
        var nota = new Nota
        {
            Id = Guid.NewGuid().ToString(),
            NotaId = dto.NotaId,
            Nombre = dto.Nombre,
            CreatedAt = DateTime.UtcNow,
        };
        
        // Guardamos con Dictionary

        await _firebaseService.GetCollection("nota")
            .Document(nota.Id)
            .SetAsync(new Dictionary<string, object>()
            {
                { "Id", nota.Id },
                { "UserId", userId },
                { "Nombre", nota.Nombre },
                { "Nota", nota.NotaId },
                { "CreatedAt", nota.CreatedAt },
            });
        return nota;
    }

    public async Task<List<Nota>> GetByUser(string userId)
    {
        // Solo van a extraer los experimentos del usuarios que hace login
        var snapshot = await _firebaseService.GetCollection("nota")
            .WhereEqualTo("Id", userId)
            .GetSnapshotAsync();
        
        var nota = new List<Nota>();
        
        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ToDictionary();

            nota.Add(new Nota
            {
                Id = data["Id"].ToString()!,
                Nombre = data["Nombre"].ToString()!,
                NotaId = Convert.ToInt32(data["Nota"].ToString()!),
                CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime()
            });
        }
        return nota;
    }
}
