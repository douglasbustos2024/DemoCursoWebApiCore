 
using Jose;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using WebAppApiArq.Contract;
using WebAppApiArq.Entities;
using WebAppApiArq.Models;
using WebAppApiArq.Authorization;
using WebAppApiArq.Contract.Dtos;
 

public class JwtTokenService
{
    private readonly JwtSettingsFile _jwtSettings;
    private RSA _privateKey;
    private RSA _publicKey;

    private readonly IRepository<RefreshToken> _refreshTokenRepository;
                                                               
    public JwtTokenService(IOptions<JwtSettingsFile> jwtSettings, IRepository<RefreshToken> refreshTokenRepository)
    {

        _refreshTokenRepository = refreshTokenRepository;

        _jwtSettings = jwtSettings.Value;
        LoadKeys();
    }

    private void LoadKeys()
    {
        var privateKeyPem = File.ReadAllText(_jwtSettings.PrivateKeyPath);
        var publicKeyPem = File.ReadAllText(_jwtSettings.PublicKeyPath);

        _privateKey = RSA.Create();
        _privateKey.ImportFromPem(privateKeyPem.ToCharArray());

        _publicKey = RSA.Create();
        _publicKey.ImportFromPem(publicKeyPem.ToCharArray());
    }

    public async Task<TokenResponseDTO> GenerateToken(UserDTO user, string clientType )
    {

        var claims = new List<Claim>
            {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                     new Claim("client_type", clientType) 
                    //new Claim(ClaimTypes.Role, user.Role)
                };


        // Dividir los roles separados por comas
        var rolesList = user.Roles.Split(',').Select(r => r.Trim()).ToList();

        // Agregar los roles como claims
        foreach (var role in rolesList)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));

            // Obtener permisos desde la clase estática para cada rol
            var permissions = RolePermissionsStore.GetPermissionsByRole(role);
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }
        }


        var tokenHandler = new JwtSecurityTokenHandler();
                        

        var privateKeyPem = File.ReadAllText(_jwtSettings.PrivateKeyPath);
        var publicKeyPem = File.ReadAllText(_jwtSettings.PublicKeyPath);

        RSA rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem.ToCharArray());
        var key = new RsaSecurityKey(rsa);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
        };
                                                                        

        var accessToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessTokenString = tokenHandler.WriteToken(accessToken);


        var encryptedToken = 
            JWT.Encode(accessTokenString, _publicKey, JweAlgorithm.RSA_OAEP, JweEncryption.A256GCM);



        // Generar Refresh Token 
        var refreshToken = await GenerateRefreshToken(user.Id);
                     

        // Retornar el DTO con Access Token y Refresh Token
        return new TokenResponseDTO
        {
            AccessToken = encryptedToken,
            RefreshToken = refreshToken
        };


    }


    public string DecryptToken(string encryptedToken)
    {
        // Desencriptar el token utilizando la clave privada
        try
        {
            var decryptedToken = JWT.Decode(encryptedToken, _privateKey);
            return decryptedToken;
        }
        catch (Exception ex)
        {
            // Manejar errores en caso de token inválido o problemas con el descifrado
            throw new SecurityTokenException("Token inválido o no pudo ser desencriptado", ex);
        }
    }

    // Métodos para obtener el emisor, audiencia y clave pública
    public string GetIssuer()
    {
        return _jwtSettings.Issuer; // Obtener del archivo de configuración
    }

    public string GetAudience()
    {
        return _jwtSettings.Audience; // Obtener del archivo de configuración
    }

    public RsaSecurityKey GetPublicKey()
    {
        return new RsaSecurityKey(_publicKey); // Convertir la clave pública a RsaSecurityKey
    }

    public ClaimsPrincipal ValidateToken(string encryptedToken)
    {
        // Desencriptar el token
        var jwt = DecryptToken(encryptedToken);

        // Aquí puedes validar el token desencriptado como cualquier JWT normal
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new RsaSecurityKey(_privateKey) // Clave privada para verificar la firma
        };

        SecurityToken validatedToken;
        var principal = tokenHandler.ValidateToken(jwt, validationParameters, out validatedToken);

        return principal;

        // El token es válido, puedes continuar con la lógica de autenticación/autorización
    }
    private async Task<RefreshTokenDTO> GenerateRefreshToken(int userId)
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        var refreshToken = Convert.ToBase64String(randomBytes);

                      

        var rt = new RefreshTokenDTO
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays), // Expiración del Refresh Token
        };


        var newRefreshToken = new RefreshToken
        {
            Token = rt.Token,
            Expires = rt.Expires,
            UserId = userId,
            Created = DateTime.UtcNow
        };

        // Guardar en el repositorio
        await _refreshTokenRepository.AddAsync(newRefreshToken);
                                

        return rt;
    }
    

}

