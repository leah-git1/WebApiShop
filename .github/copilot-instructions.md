# WebApiShop - Agent Onboarding Instructions

## Project Overview
WebApiShop is an ASP.NET Core Web API backend for a shopping/e-commerce platform. It provides REST endpoints for user authentication, product catalog management, category organization, order processing, and product ratings. The API supports both customer and admin operations with centralized error handling and rating middleware.

## Tech Stack

### Backend
- **Framework**: ASP.NET Core Web API (.NET 9.0)
- **Database**: SQL Server (snake_case naming convention for tables)
- **ORM**: Entity Framework Core (6 entities generated via EF Core Power Tools)
- **Mapping**: AutoMapper (DTO ↔ Entity conversion)
- **Logging**: NLog (currently commented out in Program.cs - enable if needed)
- **API Documentation**: Swagger/OpenAPI
- **Architecture**: Layered (Controllers → Services → Repository → Entities)
- **Middleware**: Custom error handling and rating middleware

## Project Structure

```
WebApiShop/
├── WebApiShop/                   # Main ASP.NET Core project
│   ├── Controllers/              # 5 API controllers
│   │   ├── UsersController       # Authentication & user management
│   │   ├── ProductController     # Product CRUD & listings
│   │   ├── CategoryController    # Category management
│   │   ├── OrderController       # Order processing
│   │   └── PasswordController    # Password reset
│   ├── Middleware/               # Custom middleware
│   │   ├── ErrorHandlingMiddleware
│   │   └── RatingMiddleware
│   ├── Program.cs                # DI & configuration
│   ├── appsettings.json          # Shared config
│   ├── appsettings.Development.json  # DB connection string
│   ├── nlog.config               # Logging (disabled)
│   └── WebApiShop.http           # REST client file for testing
├── Services/                     # Business logic (7 services)
│   ├── AutoMapping.cs            # AutoMapper profiles
│   └── *Service.cs               # Service implementations
├── Repository/                   # Data access layer
│   ├── ShopContext.cs            # EF Core DbContext
│   └── *Repository.cs            # Repository pattern
├── Entities/                     # Domain models (6 entities)
│   ├── User.cs                   # User entity
│   ├── ProductTbl.cs             # Product (snake_case table)
│   ├── CategoriesTbl.cs          # Category
│   ├── OrdersTbl.cs              # Order aggregate
│   ├── OrderItemsTbl.cs          # Order line items
│   ├── Rating.cs                 # Product ratings
│   └── CheckPassword.cs          # Password utility
├── DTOs/                         # Data Transfer Objects (10 DTOs)
│   └── *DTO.cs                   # Various response/request models
└── TestProject/                  # xUnit test fixtures
```

## Core Entities

- **User**: Authentication, registration, profile management
- **ProductTbl**: Products with categories, pricing, availability (`Products_tbl` in DB)
- **CategoriesTbl**: Product categorization (`Categories_tbl` in DB)
- **OrdersTbl**: Customer orders (`Orders_tbl` in DB)
- **OrderItemsTbl**: Order line items mapping products to orders (`Order_items_tbl`)
- **Rating**: Product ratings/reviews with user references

## Coding Guidelines

### Backend (.NET/C#)
1. **Async/Await**: All service/repository methods use `async Task`/`async Task<T>`
2. **Exception Handling**: Global error handling via `ErrorHandlingMiddleware`
3. **Dependency Injection**: Constructor injection; scoped services registered in `Program.cs`
4. **AutoMapper**: All Entity ↔ DTO mappings defined in `Services/AutoMapping.cs`
5. **Naming Conventions**: 
   - Entities: PascalCase with "Tbl" suffix (e.g., `ProductTbl`, `OrdersTbl`)
   - Database Tables: snake_case with `_tbl` suffix (e.g., `Products_tbl`)
6. **Middleware**: Register in `Program.cs` before routing; execution order matters
7. **Controllers**: RESTful conventions; return appropriate HTTP status codes (200, 201, 400, 404, 500)

## Build & Run Instructions

### Prerequisites
- .NET 9.0 SDK installed
- SQL Server instance running
- Connection string in `appsettings.Development.json`

### Build & Run
```bash
cd WebApiShop
dotnet restore
dotnet build
dotnet run --project WebApiShop/WebApiShop.csproj
```

### Access API
- **Base URL**: `http://localhost:5202` (HTTPS may be enabled in production)
- **Swagger UI**: `https://localhost:<port>/swagger` (in Development environment)
- **REST Client**: Use `WebApiShop.http` in VS Code with REST Client extension

### Database Setup
1. Update `appsettings.Development.json` with your SQL Server connection string:
   ```json
   "DefaultConnection": "Data Source=<server>\\<instance>;Initial Catalog=<database>;Integrated Security=True;Trust Server Certificate=True"
   ```
2. Run EF Core migrations (if available) or database initialization code

## Key Tools & Resources

### Code Generation
- **EF Core Power Tools**: Used to auto-generate entities and `ShopContext.cs`
- **AutoMapper**: Pre-configured in `Services/AutoMapping.cs` with 8+ mappings
- **Swagger**: Auto-generated from XML comments and controller definitions

### Testing
- `WebApiShop.http` file contains sample HTTP requests for manual testing
- TestProject contains xUnit test structure; extend with service/repository tests
- Use VS Code REST Client extension to test endpoints from HTTP file

### Middleware Pipeline
Middleware registration order in `Program.cs` (top-to-bottom):
```
HttpsRedirection → ErrorHandling → Rating → StaticFiles → Authorization → Controllers
```

## Common Implementation Patterns

### Adding a New Feature (Example: GetMostOrderedProducts Endpoint)
1. **Repository Interface** (`IProductRepository.cs`):
   ```csharp
   Task<List<ProductTbl>> GetMostOrderedProducts(int count = 5);
   ```

2. **Repository Implementation** (`ProductRepository.cs`):
   ```csharp
   public async Task<List<ProductTbl>> GetMostOrderedProducts(int count = 5)
   {
       return await _ShopContext.ProductTbls
           .Include(p => p.OrderItemsTbls)
           .Include(p => p.Category)
           .OrderByDescending(p => p.OrderItemsTbls.Count)
           .Take(count)
           .ToListAsync();
   }
   ```

3. **Service Interface** (`IProductService.cs`):
   ```csharp
   Task<List<MoreInfoProductDTO>> GetMostOrderedProducts(int count = 5);
   ```

4. **Service Implementation** (`ProductService.cs`):
   ```csharp
   public async Task<List<MoreInfoProductDTO>> GetMostOrderedProducts(int count = 5)
   {
       List<ProductTbl> mostOrdered = await _iProductRepository.GetMostOrderedProducts(count);
       return _mapper.Map<List<ProductTbl>, List<MoreInfoProductDTO>>(mostOrdered);
   }
   ```

5. **Controller Endpoint** (`ProductController.cs`):
   ```csharp
   [HttpGet("most-ordered")]
   public async Task<ActionResult<List<MoreInfoProductDTO>>> GetMostOrderedProducts([FromQuery] int count = 5)
   {
       try
       {
           List<MoreInfoProductDTO> products = await _iProductService.GetMostOrderedProducts(count);
           return Ok(products);
       }
       catch (Exception ex)
       {
           return StatusCode(500, new { error = ex.Message });
       }
   }
   ```

6. **Verify AutoMapper** (`AutoMapping.cs`): Ensure mapping exists:
   ```csharp
   CreateMap<ProductTbl, MoreInfoProductDTO>();  // Already defined
   ```

### Database Connection
- Connection string location: `appsettings.Development.json` → `DefaultConnection`
- Used by `ShopContext` in `Program.cs` via `UseSqlServer()`
- Tables use snake_case with `_tbl` suffix (e.g., `Products_tbl`, `Users`)

## Important Notes

### NLog Logging
- Currently commented out in `Program.cs`
- To enable: uncomment `builder.Host.UseNLog();` and ensure `nlog.config` is configured
- Useful for debugging and production monitoring

### CORS & Security
- No CORS policy configured yet (may need to enable for frontend integration)
- No JWT/OAuth visible; add authentication as needed
- `AllowedHosts: "*"` in appsettings - restrict in production

### Port Configuration
- Default: `http://localhost:5202` (see `WebApiShop.http`)
- HTTPS redirect enabled: requests to HTTP redirect to HTTPS
- Configurable via launchSettings.json in Properties folder

## Troubleshooting

1. **Database Connection Errors**: Verify connection string, SQL Server running, credentials valid
2. **Port Conflicts**: Check 5202 availability; change in `launchSettings.json`
3. **Entity Model Mismatches**: Regenerate entities using EF Core Power Tools if schema changes
4. **AutoMapper Configuration**: Test mappings with `IMapper` in services; verify DTO coverage
5. **Middleware Issues**: Check execution order in `Program.cs`; error handling must be early

---

**Last Updated**: February 2026 | **Version**: 1.0
