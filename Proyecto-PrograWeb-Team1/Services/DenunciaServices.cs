using Proyecto_PrograWeb_Team1.Models;
using Proyecto_PrograWeb_Team1.DTOs;
using Proyecto_PrograWeb_Team1.Services;


namespace Proyecto_PrograWeb_Team1.Services;

    public class DenunciaServices
{
    //Manejamos lo relacionado con los experimentos del usuario
    
    private readonly FirebaseService _firebaseService;
    private readonly UserServices _userServices;

    public DenunciaServices(FirebaseService firebaseService, UserServices userServices)
    {
        _firebaseService = firebaseService;
        _userServices =  userServices;
        
    }
    
    

    public async Task<Denuncia> Create(CrearDenunciaDto dto, string userId)
    {
        var denunciaExistente = await _firebaseService.GetCollection("denuncia").WhereEqualTo("UserId", userId)
            .WhereEqualTo("User-report", dto.Userreport) .WhereEqualTo("Status", dto.Status) .GetSnapshotAsync();
        if (denunciaExistente.Documents.Count != 0)
        {
            throw new Exception ("Ya existe una denuncia entre estos usuarios");
        }
            
        //VALIDACIONES PARA QUE ESTE CORRECTO (LA VERDAD NO SE SI IR ACA SEA UNA BUENA PRACTICA 
        
        if (!Enum.TryParse<Categoria>(dto.Categoria, true, out Categoria categoria))
        {
            throw new Exception ("Categoria invalida");
        }
        if (!Enum.TryParse<Status>(dto.Status, true, out Status status))
        {
            throw new Exception ("Status invalido");
        }
        if(!await _userServices.ConfirmUsuario(dto.Userreport))
        {
            throw new Exception ("Usuario reportado no existe");
        }
        

        var denuncia = new Denuncia
        {
            Id = Guid.NewGuid().ToString(),
            Title = dto.Title,
            Status = dto.Status,
            Success = dto.Success,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Categoria =  dto.Categoria,
            Comment = dto.Comment,
            Userreport = dto.Userreport,
        };
        
        
        
        // Guardamos con Dictionary

        await _firebaseService.GetCollection("denuncia")
            .Document(denuncia.Id)
            .SetAsync(new Dictionary<string, object>()
            {
                { "Id", denuncia.Id },
                { "Title", denuncia.Title },
                { "Status", denuncia.Status },
                { "Comment", denuncia.Comment },
                { "Success", denuncia.Success },
                { "UserId", denuncia.UserId },
                {"Categoria", denuncia.Categoria},
                { "CreatedAt", denuncia.CreatedAt },
                {"User-report", denuncia.Userreport },
            });
        return denuncia;
    }

    public async Task<List<Denuncia>> GetByUser(string userId)
    {
        // Solo van a extraer las denuncias del usuarios que hace login
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
                Status = data["Status"].ToString()!,
                Comment = data["Comment"].ToString()!,
                Success = (bool)data["Success"],
                UserId = data["UserId"].ToString()!,
                Categoria = data["Categoria"].ToString()!,
                CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime(),
                Userreport = data["User-report"].ToString()!
            });
        }
        return denuncia;
    }
    
    public async Task<List<Denuncia>> GetAll()
    {
        // Solo van a extraer los experimentos del usuarios que hace login
        var snapshot = await _firebaseService.GetCollection("denuncia")
            .GetSnapshotAsync();
        
        var denuncia = new List<Denuncia>();
        
        foreach (var doc in snapshot.Documents)
        {
            var data = doc.ToDictionary();

            denuncia.Add(new Denuncia
            {
                Id = data["Id"].ToString()!,
                Title = data["Title"].ToString()!,
                Status = data["Status"].ToString()!,
                Comment = data["Comment"].ToString()!,
                Success = (bool)data["Success"],
                UserId = data["UserId"].ToString()!,
                Categoria = data["Categoria"].ToString()!,
                CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime(),
                Userreport = data ["User-report"].ToString()!
            });
        }
        return denuncia;
    }
    
}
