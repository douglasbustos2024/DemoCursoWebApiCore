using System.Security.Claims;

namespace WebAppApiArq.Contract.Dtos
{
    public class UserDTO
    {          
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Roles { get; set; }
                                                        
        
    }
}
