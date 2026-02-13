using Microsoft.AspNetCore.Identity;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.Repositories;

public class UniversiteUserRepository(UniversiteDbContext context, 
    UserManager<UniversiteUser> userManager, 
    RoleManager<UniversiteRole> roleManager) : 
    Repository<IUniversiteUser>(context), IUniversiteUserRepository
{
    public async Task<IUniversiteUser?> AddUserAsync(string login, string email, string password, string role, Etudiant? etudiant)
    {
        var user = new UniversiteUser { UserName = login, Email = email, Etudiant = etudiant, EtudiantId = etudiant?.Id };
        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, role);

        await context.SaveChangesAsync();
        return result.Succeeded ? user : null;
    }

    public async Task<IUniversiteUser?> FindByEmailAsync(string email)
        => await userManager.FindByEmailAsync(email);

    public async Task UpdateAsync(IUniversiteUser entity, string userName, string email)
    {
        var user = (UniversiteUser)entity;
        user.UserName = userName;
        user.Email = email;
        await userManager.UpdateAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<List<string>> GetRolesAsync(IUniversiteUser user)
    {
        var u = (UniversiteUser)user;
        var roles = await userManager.GetRolesAsync(u);
        return roles.ToList();
    }

    // Cette signature "override DeleteAsync(long id)" dépend de ton Repository<T>.
    // Si ton Repository<T> a DeleteAsync(long id) qui retourne Task, garde Task.
    public async Task DeleteAsync(long id)
    {
        var etud = await context.Etudiants.FindAsync(id)
                   ?? throw new InvalidOperationException($"Etudiant {id} introuvable.");

        var user = await userManager.FindByEmailAsync(etud.Email);
        if (user != null)
        {
            await userManager.DeleteAsync(user);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsInRoleAsync(string email, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user != null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> CheckPasswordAsync(IUniversiteUser user, string password)
    {
        var u = (UniversiteUser)user;
        return await userManager.CheckPasswordAsync(u, password);
    }
}
