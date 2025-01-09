using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseStudio.Database;
using UniverseStudio.Services;

namespace UniverseStudio.Controllers;

[Route("/api/v1/auth")]
public class AuthenticationController : Controller
{
    protected readonly TokensCacheService TokensCache;
    protected readonly IDbContextFactory<MainDbContext> Factory;

    public AuthenticationController(TokensCacheService tokensCacheService, IDbContextFactory<MainDbContext> factory)
    {
        TokensCache = tokensCacheService;
        Factory = factory;
    }
}