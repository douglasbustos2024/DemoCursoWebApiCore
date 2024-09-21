using Jose;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAppApiArq.Entities;
using WebAppApiArq.Models;
using WebAppApiArq.Authorization;
using WebAppApiArq.Contract.Dtos;

namespace WebAppApiArq.Services
{
    public class LoginServices
    {             
        public UserDTO AuthenticateUser(LoginModel login)
        {
            // Aquí deberías autenticar al usuario con tu lógica de negocio
            // Este es solo un ejemplo de usuario
            var user = new UserDTO
            {
                Id = 1,
                UserName = "exampleUser",
                Roles = "operador"
            };

                                        
            // Aquí puedes devolver el usuario con sus claims, por ejemplo, en un token JWT
            return user; // Asegúrate de incluir los claims en el token si es necesario


        }
                  
    }



}
