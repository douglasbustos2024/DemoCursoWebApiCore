namespace WebAppApiArq.Authorization
{
    public static class RolePermissionsStore
    {
        public static readonly Dictionary<string, List<string>> RolePermissions = new Dictionary<string, List<string>>
        {
         { "usuario",  new List<string> { AppPermissions.Pages_General_Data } },
         { "operador", new List<string> { AppPermissions.Pages_General_Data } },
         { "admin",    new List<string> { AppPermissions.Pages_General_Data, 
                                            AppPermissions.Pages_Product_Create } }
                            
        // Agrega otros roles y permisos según sea necesario
        };

        // Método para obtener los permisos por rol
        public static List<string> GetPermissionsByRole(string role)
        {
            return RolePermissions.ContainsKey(role) ? RolePermissions[role] : new List<string>();
        }
    }

}
