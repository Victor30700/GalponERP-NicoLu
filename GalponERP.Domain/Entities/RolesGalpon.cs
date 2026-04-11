namespace GalponERP.Domain.Entities;

public static class RolesGalpon
{
    public const string Admin = "Admin";
    public const string Operario = "Operario";
    public const string Veterinario = "Veterinario";

    public static readonly IReadOnlyCollection<string> All = new[] { Admin, Operario, Veterinario };
}
