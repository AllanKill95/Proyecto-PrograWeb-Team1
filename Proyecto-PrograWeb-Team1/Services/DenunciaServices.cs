using Proyecto_PrograWeb_Team1.Models;
using Proyecto_PrograWeb_Team1.DTOs;

namespace Proyecto_PrograWeb_Team1.Services;

    public class DenunciaServices
{
    //Manejamos lo relacionado con los experimentos del usuario
    
    private readonly FirebaseService _firebaseService;

    public DenunciaServices(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    public async Task<Denuncia> Create(CrearDenunciaDto dto, string userId)
    {
        var denuncia = new Denuncia
        {
            Id = Guid.NewGuid().ToString(),
            Title = dto.Title,
            Result = dto.Result,
            Success = dto.Success,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Comment = dto.Comment
        };
        
        // Guardamos con Dictionary

        await _firebaseService.GetCollection("denuncia")
            .Document(denuncia.Id)
            .SetAsync(new Dictionary<string, object>()
            {
                { "Id", denuncia.Id },
                { "Title", denuncia.Title },
                { "Result", denuncia.Result },
                { "Comment", denuncia.Comment },
                { "Success", denuncia.Success },
                { "UserId", denuncia.UserId },
                { "CreatedAt", denuncia.CreatedAt },
            });
        return denuncia;
    }

    public async Task<List<Denuncia>> GetByUser(string userId)
    {
        // Solo van a extraer los experimentos del usuarios que hace login
        var snapshot = await _firebaseService.GetCollection("denuncia")
            .WhereEqualTo("UserId", userId)
            .GetSnapshotAsync();
        
        var denuncia = new List<Denuncia>();
        
        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ToDictionary();

            denuncia.Add(new Denuncia
            {
                Id = data["Id"].ToString()!,
                Title = data["Title"].ToString()!,
                Result = data["Result"].ToString()!,
                Comment = data["Comment"].ToString()!,
                Success = (bool)data["Success"],
                UserId = data["UserId"].ToString()!,
                CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime()
            });
        }
        return denuncia;
    }
}
