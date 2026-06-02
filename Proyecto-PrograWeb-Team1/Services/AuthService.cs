using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Google.Cloud.Firestore;
using Proyecto_PrograWeb_Team1.DTOs;
using Proyecto_PrograWeb_Team1.Services;
using Proyecto_PrograWeb_Team1.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Convert = System.Convert;

namespace Proyecto_PrograWeb_Team1.Services;

public class AuthService
{
    // Maneja lo relacionado a registro e inicio de sesion
    private readonly FirebaseService _firebaseService;
    private readonly IConfiguration _configuration;

    public AuthService(FirebaseService firebaseService, IConfiguration configuration)
    {
        _firebaseService = firebaseService;
        _configuration = configuration;
    }

    public async Task<User> Register(RegisterDto dto)
    {
        // Primero verificamos que no existe un usuario con ese correo
        var collection = _firebaseService.GetCollection("users");
        var existing = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();

        if (existing.Count > 0)
            throw new Exception("Ya existe un usuario con ese correo");
        
        // Creamos el objeto con la contraseña hasheada

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = "user",
            CreatedAt =  DateTime.UtcNow
        };
        
        // Guardamos En FS usando el Id como nombre del documento
        await collection.Document(user.Id).SetAsync(new Dictionary<string, object>
        {
            { "Id", user.Id },
            { "FullName", user.FullName },
            { "Email", user.Email },
            { "PasswordHash", user.PasswordHash },
            { "Role", user.Role },
            { "CreatedAt", user.CreatedAt }
        });
        return user;
    }

    public async Task<string> Login(LoginDto dto)
    {
        // Buscar al usuario por correo en FS
        var collection = _firebaseService.GetCollection("users");
        var snapshot = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();

        if(snapshot.Count == 0)
            throw new Exception("Credenciales inválidas");

        // Si lo encontramos mapeamos manualmente el documento a nuestro objeto
        // Usamos ToDictionary()
        var doc = snapshot.Documents[0];
        var data = doc.ToDictionary();

        var user = new User
        {
            Id = data["Id"].ToString()!,
            FullName = data["FullName"].ToString()!,
            Email = data["Email"].ToString()!,
            PasswordHash = data["PasswordHash"].ToString()!,
            Role = data["Role"].ToString()!,
            // Int64, necesitamos convertirlo
            CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime()
        };

        // Verificar si la contraseña esta hasheada
        if(!VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Credenciales inválidas");

        // Se completo exitosamente, generamos un token JWT
        return GenerateToken(user);

    }

    private string GenerateToken(User user)
    {
        // El token lleva cierta informacion, Id, Email y Role del usuario que hizo login
        // Para proteccion de los endpoints, se sabe quien los esta llamando
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                
                var token = new JwtSecurityToken(

                        issuer: _configuration["Jwt:Issuer"], //Quien lo genera, nuestro token lo genera la app
                        audience: _configuration["Jwt:Audience"], // Para quien lo genera, clientes / front-end
                        claims: claims, // Estos son los datos del usuario
                        expires: DateTime.UtcNow.AddHours(8), //Tiempo de vida del token
                        signingCredentials: creds // Firma de seguridad
                        );
                    return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string password, string hash)
    {
        try
        {
            var parts = hash.Split(':');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                var computedHash = Convert.ToBase64String(pbkdf2.GetBytes(256 / 8));
                return computedHash == storedHash;
            }
        }
        catch
        {
            return false;
        }
    }

    // Encriptar contraseña con PBKDF2 + salt
    private string HashPassword(string password)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                var hash = pbkdf2.GetBytes(256 / 8);
                var saltB64 = Convert.ToBase64String(salt);
                var hashB64 = Convert.ToBase64String(hash);
                return $"{saltB64}:{hashB64}";
            }
        }
    }
}