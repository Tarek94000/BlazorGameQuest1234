using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace BlazorGame.Client.Security;

public class CustomUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    private readonly IAccessTokenProviderAccessor _accessor;

    public CustomUserFactory(IAccessTokenProviderAccessor accessor)
        : base(accessor)
    {
        _accessor = accessor;
    }

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);
        var identity = (ClaimsIdentity)user.Identity!;

        if (user.Identity?.IsAuthenticated ?? false)
        {
            // 1. Essayer depuis l'ID Token (déjà fait par défaut ou via AdditionalProperties)
            MapRolesFromProperties(account.AdditionalProperties, identity, options.RoleClaim ?? "role");

            // 2. Essayer depuis l'Access Token (souvent là que Keycloak met les rôles)
            try
            {
                var tokenResult = await _accessor.TokenProvider.RequestAccessToken();
                if (tokenResult.TryGetToken(out var token))
                {
                    var payload = token.Value.Split('.')[1];
                    var jsonBytes = ParseBase64WithoutPadding(payload);
                    var jsonDocument = JsonDocument.Parse(jsonBytes);
                    var additionalProps = new Dictionary<string, object>();
                    
                    // On extrait les propriétés pertinentes du JSON du token
                    foreach (var prop in jsonDocument.RootElement.EnumerateObject())
                    {
                        additionalProps[prop.Name] = prop.Value;
                    }
                    
                    Console.WriteLine("[CustomUserFactory] Mapping roles from Access Token...");
                    MapRolesFromElement(jsonDocument.RootElement, identity, options.RoleClaim ?? "role");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CustomUserFactory] Error parsing access token: {ex.Message}");
            }
        }

        return user;
    }

    private void MapRolesFromProperties(IDictionary<string, object> properties, ClaimsIdentity identity, string roleClaimType)
    {
        if (properties.TryGetValue("realm_access", out var realmAccess) && realmAccess is JsonElement raElement)
        {
            ExtractRoles(raElement, identity, roleClaimType);
        }

        if (properties.TryGetValue("resource_access", out var resourceAccess) && resourceAccess is JsonElement resElement)
        {
             foreach (var client in resElement.EnumerateObject())
             {
                 if (client.Value.TryGetProperty("roles", out var roles))
                 {
                     ExtractRoles(roles, identity, roleClaimType); // Passage direct du tableau de rôles
                 }
             }
        }
    }

    private void MapRolesFromElement(JsonElement root, ClaimsIdentity identity, string roleClaimType)
    {
         if (root.TryGetProperty("realm_access", out var realmAccess))
        {
            ExtractRoles(realmAccess, identity, roleClaimType);
        }

        if (root.TryGetProperty("resource_access", out var resourceAccess))
        {
             foreach (var client in resourceAccess.EnumerateObject())
             {
                 if (client.Value.TryGetProperty("roles", out var roles))
                 {
                     ExtractRoles(roles, identity, roleClaimType);
                 }
             }
        }
    }

    private void ExtractRoles(JsonElement element, ClaimsIdentity identity, string roleClaimType)
    {
        // Si l'élément est directement le tableau (cas resource_access)
        if (element.ValueKind == JsonValueKind.Array)
        {
             foreach (var role in element.EnumerateArray())
            {
                var val = role.GetString();
                if (!string.IsNullOrEmpty(val) && !identity.HasClaim(c => c.Type == roleClaimType && c.Value == val))
                {
                    identity.AddClaim(new Claim(roleClaimType, val));
                    identity.AddClaim(new Claim(ClaimTypes.Role, val));
                }
            }
            return;
        }

        // Si l'élément est l'objet wrapper (cas realm_access: { roles: [...] })
        if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("roles", out var rolesElement))
        {
            foreach (var role in rolesElement.EnumerateArray())
            {
                var val = role.GetString();
                if (!string.IsNullOrEmpty(val) && !identity.HasClaim(c => c.Type == roleClaimType && c.Value == val))
                {
                    identity.AddClaim(new Claim(roleClaimType, val));
                    identity.AddClaim(new Claim(ClaimTypes.Role, val));
                }
            }
        }
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
