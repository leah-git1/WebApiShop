## PART 1: CURRENT STATE ANALYSIS

### Current Architecture
- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server (single shared database)
- **Controllers**: 5 (Users, Products, Orders, Categories, Password)
- **Services**: 6 (ProductService, OrderService, UsersServices, CategoryService, RatingService, PasswordService)
- **Entities**: 7 (User, ProductTbl, CategoriesTbl, OrdersTbl, OrderItemsTbl, Rating, CheckPassword)

### Problems with Current Monolith
❌ Can't scale Products independently
❌ Can't scale Orders independently
❌ Database lock contention
❌ One change requires redeploying entire application
❌ Hard to test individual services
❌ No clear separation of concerns
❌ Orders directly queries Products (tight coupling)


---

## PART 2: TARGET MICROSERVICES ARCHITECTURE

### Service Decomposition
BEFORE (Monolith):
┌─────────────────────────────────┐
│ WebApiShop (All-in-One) │
│ Users + Products + Orders │
│ Single SQL Server DB │
└─────────────────────────────────┘

AFTER (Microservices):
┌──────────────────┬────────────────┬──────────────────┬──────────────┐
│ USER SERVICE │ PRODUCT SERVICE│ ORDER SERVICE │AUTH SERVICE │
│ │ │ │ │
│ C# ASP.NET Core │C# ASP.NET Core │ C# ASP.NET Core │Node.js/Auth0 │
│ Port: 5001 │ Port: 5002 │ Port: 5003 │Port: 5004 │
│ PostgreSQL │PostgreSQL │ PostgreSQL │External/JWT │
│ 2 replicas │ 3 replicas │ 3 replicas │ 1 replica │
└──────────────────┴────────────────┴──────────────────┴──────────────┘
↓ ↓ ↓
┌────────────────────────────────────────────┐
│ API GATEWAY (Kong/AWS) │
│ Routes: /api/users → User Service │
│ /api/products → Product Service │
│ /api/orders → Order Service │
└────────────────────────────────────────────┘
↓
┌────────────────────────────────────────┐
│ RabbitMQ Message Broker │
│ (Event-driven communication) │
└────────────────────────────────────────┘


---

## PART 3: MICROSERVICES SPECIFICATION

### SERVICE 1: PRODUCT SERVICE

**Responsibility**: Product & Category Management

**Technology Stack**:
- **Framework**: C# + ASP.NET Core 9.0
- **Database**: PostgreSQL
- **Why**: Complex queries with filtering, JOINs on categories, strong consistency for pricing
- **Language**: C# (consistency with existing code)

**Scope** (Extract these from monolith):
- ProductTbl entity
- CategoriesTbl entity  
- ProductController endpoints
- ProductRepository
- ProductService
- CategoryRepository
- CategoryService

**Database Tables** (PostgreSQL):
```sql
products (
  id INT PRIMARY KEY,
  name VARCHAR(255),
  description TEXT,
  price DECIMAL(10,2),
  category_id INT,
  available BOOLEAN,
  created_at TIMESTAMP
)

categories (
  id INT PRIMARY KEY,
  name VARCHAR(100),
  description TEXT
)

GET    /api/products?category=X&min_price=Y    (filtered list)
GET    /api/products/:id                        (get single)
POST   /api/products                            (create)
PUT    /api/products/:id                        (update)
DELETE /api/products/:id                        (delete)
GET    /api/categories                          (list)
POST   /api/categories                          (create)

Caching: Redis for product details (TTL: 5 min)

Scaling: 3 replicas (HPA for CPU > 70%)

Port: 5002

SERVICE 2: ORDER SERVICE
Responsibility: Order Management & Processing

Technology Stack:

Framework: C# + ASP.NET Core 9.0
Database: PostgreSQL
Why: Complex transactions, ACID guarantees needed for orders
Language: C# (strong type safety for financial data)
Scope (Extract these from monolith):

OrdersTbl entity
OrderItemsTbl entity
OrderController endpoints
OrderRepository
OrderService
Database Tables (PostgreSQL):

orders (
  id INT PRIMARY KEY,
  user_id INT NOT NULL,
  created_at TIMESTAMP,
  total_amount DECIMAL(10,2),
  status VARCHAR(50),
  FOREIGN KEY (user_id) REFERENCES users(id)
)

order_items (
  id INT PRIMARY KEY,
  order_id INT NOT NULL,
  product_id INT NOT NULL,
  price_at_purchase DECIMAL(10,2),
  quantity INT,
  FOREIGN KEY (order_id) REFERENCES orders(id)
)
POST   /api/orders                      (create)
GET    /api/orders/:id                  (get single)
GET    /api/orders?user_id=X            (list by user)
PUT    /api/orders/:id/status           (update status)
GET    /api/users/:id/orders            (order history)

POST   /api/orders                      (create)
GET    /api/orders/:id                  (get single)
GET    /api/orders?user_id=X            (list by user)
PUT    /api/orders/:id/status           (update status)
GET    /api/users/:id/orders            (order history)

1. User calls POST /api/orders { product_ids: [1,2,3] }
2. Order Service validates products exist (HTTP GET to Product Service)
3. Order Service creates order record
4. Order Service publishes OrderCreated event (to RabbitMQ)
5. Product Service receives event, updates availability cache

Caching: Redis for recent orders (TTL: 30 min)

Scaling: 3 replicas (HPA for CPU > 70%, or message queue depth)

Port: 5003

SERVICE 3: USER SERVICE
Responsibility: User Authentication & Profiles

Technology Stack:

Framework: C# + ASP.NET Core 9.0
Database: PostgreSQL
Why: Credential security, consistency with other services
Alternative: Node.js for faster auth (optional upgrade later)
Scope (Extract these from monolith):

User entity
UsersController endpoints (login, register, profile)
PasswordService (validation logic)
UsersRepository
UsersServices
Database Tables (PostgreSQL):
users (
  id INT PRIMARY KEY,
  first_name VARCHAR(100),
  last_name VARCHAR(100),
  email VARCHAR(100) UNIQUE,
  password_hash VARCHAR(255),
  username VARCHAR(100) UNIQUE,
  is_admin BOOLEAN DEFAULT FALSE,
  created_at TIMESTAMP
)
POST   /api/auth/register              (register)
POST   /api/auth/login                 (login)
GET    /api/users/:id                  (get profile)
PUT    /api/users/:id                  (update profile)
DELETE /api/users/:id                  (delete account)
POST   /api/auth/validate-token        (used by API Gateway)

Token Strategy:

Type: JWT Bearer Token
Expiry: 24 hours (access), 30 days (refresh)
Validation: API Gateway calls /api/auth/validate-token endpoint
Caching: Redis for token validation (TTL: 5 min)

Scaling: 2 replicas (auth is lightweight)

Port: 5001

SERVICE 4: RATING SERVICE (Optional, can stay in monolith initially)
Responsibility: Product Reviews & Ratings

Current State: Simple Read/Write, Rating entity exists

Decision: Keep in monolith OR extract later

If kept in monolith: Simpler, no cross-service calls
If extracted: Independent scaling if needed
Future Option: Extract to separate service if volume grows

PART 4: IMPLEMENTATION PHASES
PHASE 0: Setup Infrastructure (Weeks 1-2)
Tasks:

 Deploy PostgreSQL (3 instances: prod, staging, dev)
 Deploy RabbitMQ (message broker)
 Deploy Redis (caching)
 Setup API Gateway (Kong or AWS API Gateway)
 Setup Kubernetes cluster (locally: Docker Desktop, or cloud)
 Create Docker registry (Docker Hub or private)
 Setup CI/CD (GitHub Actions)
 Setup monitoring (Prometheus, Grafana, ELK)
Output: Infrastructure ready, monolith still running, no code changes

PHASE 1: Extract User Service (Weeks 3-5)
Tasks:

1. CREATE NEW PROJECT
   - New C# ASP.NET Core 9.0 project: "WebApiShop.UserService"
   - Add to solution alongside monolith

2. MIGRATE CODE
   - Copy User entity → User.cs
   - Copy UsersRepository → IUsersRepository + UsersRepository
   - Copy UsersServices → IUsersServices + UsersServices
   - Copy PasswordService → IPasswordService + PasswordService
   - Copy UsersController (rename to AuthController)
   - Copy AutoMapper configs for User

3. SETUP DATABASE
   - Create new PostgreSQL database: "webapishop_users_db"
   - Run migrations: dotnet ef database update
   - Test connection string in appsettings.json

4. SETUP MESSAGE BROKER
   - Add RabbitMQ NuGet package: RabbitMQ.Client
   - Implement IEventPublisher for UserRegistered, UserDeleted events
   - Add event publisher injection to UserService

5. SETUP JWT TOKENS
   - Install: Microsoft.IdentityModel.Tokens
   - Implement JWT generation in login endpoint
   - Create token validation endpoint (GET /api/auth/validate-token)

6. DOCKERFILE & DEPLOYMENT
   - Create Dockerfile (multi-stage build)
   - Deploy to Kubernetes
   - Update Kong routes: /api/users/* → http://user-service:5001
   - Update Kong routes: /api/auth/* → http://user-service:5001   

7. TESTING
   - Unit tests for PasswordService
   - Integration tests with mock PostgreSQL
   - End-to-end tests via Postman

8. CUTOVER (BlueGreen)
   - Run monolith + User Service in parallel (2 weeks)
   - Monitor errors/logs
   - Migrate all auth traffic: kubectl set traffic
   - Remove auth endpoints from monolith

Code Changes Required:

New file: Program.cs

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ShopContext>(opts => 
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories & Services
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersServices, UsersServices>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts => {
        opts.Authority = builder.Configuration["Jwt:Authority"];
        opts.Audience = builder.Configuration["Jwt:Audience"];
    });

// Message Broker
builder.Services.AddScoped<IEventPublisher, RabbitMQEventPublisher>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

Result: User Service running independently, can deploy separately

PHASE 2: Extract Product & Category Service (Weeks 6-9)
Tasks:
1. CREATE NEW PROJECT
   - New C# ASP.NET Core 9.0: "WebApiShop.ProductService"

2. MIGRATE CODE
   - Copy ProductTbl, CategoriesTbl entities
   - Copy ProductRepository, CategoryRepository
   - Copy ProductService, CategoryService
   - Copy ProductController, CategoryController
   - Copy AutoMapper configs

3. SETUP DATABASE
   - Create PostgreSQL database: "webapishop_products_db"
   - Migrate entities

4. IMPLEMENT CACHING
   - Redis for product details (TTL 5 min)
   - Redis for categories (TTL 1 hour)

5. API GATEWAY ROUTES
   - /api/products/* → Product Service (5002)
   - /api/categories/* → Product Service (5002)

6. TESTING & CUTOVER
   - Unit tests
   - Integration tests
   - Blue-green deployment (parallel 2 weeks)

Result: Product Service running independently with caching, scales separately

PHASE 3: Extract Order Service (Weeks 10-13)
Tasks:

Result: Order Service running independently, handles failures gracefully

PHASE 4: Stabilization & Optimization (Weeks 14-16)
Tasks:

 Load testing: Simulate 100 concurrent users
 Performance tuning: Database indexes, query optimization
 Monitoring: Setup dashboards for each service
 Alerting: CPU > 80%, Error rate > 1%, Latency p95 > 500ms
 Documentation: How to deploy, scale, troubleshoot each service
 Team training: How to develop/test services independently
PART 5: TECHNOLOGY STACK DECISIONS
Language Choice Per Service
Service	Language	Framework	Database	Port	Why
User	C#	ASP.NET Core 9	PostgreSQL	5001	Auth simple logic, keep .NET
Product	C#	ASP.NET Core 9	PostgreSQL	5002	Complex queries, JOINs
Order	C#	ASP.NET Core 9	PostgreSQL	5003	Financial data, ACID important
API Gateway	Go/Rust	Kong	N/A	80/443	Ultra-fast routing
Why all C# .NET?

Existing codebase in C#
Strong type safety (financial/user data)
Excellent EF Core ORM
Team knows .NET
Easy migration
PART 6: MESSAGING & EVENTS
RabbitMQ Configuration
PART 7: API GATEWAY CONFIGURATION
Kong Routes
PART 8: TESTING STRATEGY
Unit Tests
Integration Tests
PART 9: MONITORING & ALERTS
Key Metrics
PART 10: TIMELINE
PART 11: RISKS & MITIGATION
Risk	Mitigation
Data loss	Dual-write, verification before cutover
Service down	Circuit breaker, fallback data
Performance regression	Load testing, metric comparison
Operational complexity	Monitoring, runbooks, training
CONCLUSION
This plan transforms WebApiShop from monolith into 3 independent microservices over 16 weeks.

Expected Effort:

Infrastructure: 40 person-days
Development: 120 person-days
Testing: 60 person-days
Documentation: 20 person-days
Total: ~240 person-days (5 weeks full team)
Key Benefits:
✓ Independent deployment
✓ Independent scaling
✓ Fault isolation
✓ Team autonomy

Plan Version: 1.0
Date: February 24, 2026
Status: READY FOR IMPLEMENTATION

Similar code found with 1 license type - View matches
Claude Haiku 4.5 • 1x
1. CREATE NEW PROJECT
   - New C# ASP.NET Core 9.0: "WebApiShop.OrderService"

2. MIGRATE CODE
   - Copy OrdersTbl, OrderItemsTbl entities
   - Copy OrderRepository
   - Copy OrderService
   - Copy OrderController

3. SETUP DATABASE
   - Create PostgreSQL database: "webapishop_orders_db"

4. IMPLEMENT CIRCUIT BREAKER
   - If Product Service fails 5x in 30sec → Open circuit
   - Fallback: Use cached prices
   - Retry: Exponential backoff

5. MESSAGE EVENTS
   - OrderService publishes: OrderCreated event
   - ProductService consumes: Updates cache (eventual consistency)

6. TESTING
   - Test Order creation with valid product
   - Test Order creation when product unavailable
   - Test when Product Service is down

   Result: Order Service running independently, handles failures gracefully

PHASE 4: Stabilization & Optimization (Weeks 14-16)
Tasks:

 Load testing: Simulate 100 concurrent users
 Performance tuning: Database indexes, query optimization
 Monitoring: Setup dashboards for each service
 Alerting: CPU > 80%, Error rate > 1%, Latency p95 > 500ms
 Documentation: How to deploy, scale, troubleshoot each service
 Team training: How to develop/test services independently
PART 5: TECHNOLOGY STACK DECISIONS
Language Choice Per Service
Service	Language	Framework	Database	Port	Why
User	C#	ASP.NET Core 9	PostgreSQL	5001	Auth simple logic, keep .NET
Product	C#	ASP.NET Core 9	PostgreSQL	5002	Complex queries, JOINs
Order	C#	ASP.NET Core 9	PostgreSQL	5003	Financial data, ACID important
API Gateway	Go/Rust	Kong	N/A	80/443	Ultra-fast routing
Why all C# .NET?

Existing codebase in C#
Strong type safety (financial/user data)
Excellent EF Core ORM
Team knows .NET
Easy migration
PART 6: MESSAGING & EVENTS
RabbitMQ Configuration
Exchange: webapishop.events

Queues:
  - user.events → UserRegistered, UserDeleted
  - product.events → ProductCreated, ProductUpdated
  - order.events → OrderCreated, OrderUpdated
  - notification.events → Consumes all, sends emails

Example Event:
{
  "event_id": "uuid",
  "event_type": "user.registered",
  "timestamp": "2026-02-24T10:30:00Z",
  "data": {
    "user_id": 123,
    "email": "user@example.com"
  }
}

PART 7: API GATEWAY CONFIGURATION
Kong Routes

services:
  - name: user-service
    url: http://user-service:5001
    routes:
      - /api/auth
      - /api/users

  - name: product-service
    url: http://product-service:5002
    routes:
      - /api/products
      - /api/categories

  - name: order-service
    url: http://order-service:5003
    routes:
      - /api/orders

plugins:
  - cors: Enable for Angular frontend
  - rate-limiting: 100 req/min per user
  - logging: All requests
  - jwt-validation: Check Authorization header

  
101 x 13
I apologize! The file creation tool is currently disabled on your system. However, I can provide you with the complete content that you can copy directly into the file you opened.

Here's the complete WebApiShop Microservices Implementation Plan. Copy everything from the line below to the end:

❌ Can't scale Products independently
❌ Can't scale Orders independently
❌ Database lock contention
❌ One change requires redeploying entire application
❌ Hard to test individual services
❌ No clear separation of concerns
❌ Orders directly queries Products (tight coupling)

BEFORE (Monolith):
┌─────────────────────────────────┐
│ WebApiShop (All-in-One) │
│ Users + Products + Orders │
│ Single SQL Server DB │
└─────────────────────────────────┘

AFTER (Microservices):
┌──────────────────┬────────────────┬──────────────────┬──────────────┐
│ USER SERVICE │ PRODUCT SERVICE│ ORDER SERVICE │AUTH SERVICE │
│ │ │ │ │
│ C# ASP.NET Core │C# ASP.NET Core │ C# ASP.NET Core │Node.js/Auth0 │
│ Port: 5001 │ Port: 5002 │ Port: 5003 │Port: 5004 │
│ PostgreSQL │PostgreSQL │ PostgreSQL │External/JWT │
│ 2 replicas │ 3 replicas │ 3 replicas │ 1 replica │
└──────────────────┴────────────────┴──────────────────┴──────────────┘
↓ ↓ ↓
┌────────────────────────────────────────────┐
│ API GATEWAY (Kong/AWS) │
│ Routes: /api/users → User Service │
│ /api/products → Product Service │
│ /api/orders → Order Service │
└────────────────────────────────────────────┘
↓
┌────────────────────────────────────────┐
│ RabbitMQ Message Broker │
│ (Event-driven communication) │
└────────────────────────────────────────┘

API Endpoints:

Caching: Redis for product details (TTL: 5 min)

Scaling: 3 replicas (HPA for CPU > 70%)

Port: 5002

SERVICE 2: ORDER SERVICE
Responsibility: Order Management & Processing

Technology Stack:

Framework: C# + ASP.NET Core 9.0
Database: PostgreSQL
Why: Complex transactions, ACID guarantees needed for orders
Language: C# (strong type safety for financial data)
Scope (Extract these from monolith):

OrdersTbl entity
OrderItemsTbl entity
OrderController endpoints
OrderRepository
OrderService
Database Tables (PostgreSQL):

API Endpoints:

Key Constraint: When creating order, Order Service calls Product Service API

Caching: Redis for recent orders (TTL: 30 min)

Scaling: 3 replicas (HPA for CPU > 70%, or message queue depth)

Port: 5003

SERVICE 3: USER SERVICE
Responsibility: User Authentication & Profiles

Technology Stack:

Framework: C# + ASP.NET Core 9.0
Database: PostgreSQL
Why: Credential security, consistency with other services
Alternative: Node.js for faster auth (optional upgrade later)
Scope (Extract these from monolith):

User entity
UsersController endpoints (login, register, profile)
PasswordService (validation logic)
UsersRepository
UsersServices
Database Tables (PostgreSQL):

API Endpoints:

Token Strategy:

Type: JWT Bearer Token
Expiry: 24 hours (access), 30 days (refresh)
Validation: API Gateway calls /api/auth/validate-token endpoint
Caching: Redis for token validation (TTL: 5 min)

Scaling: 2 replicas (auth is lightweight)

Port: 5001

SERVICE 4: RATING SERVICE (Optional, can stay in monolith initially)
Responsibility: Product Reviews & Ratings

Current State: Simple Read/Write, Rating entity exists

Decision: Keep in monolith OR extract later

If kept in monolith: Simpler, no cross-service calls
If extracted: Independent scaling if needed
Future Option: Extract to separate service if volume grows

PART 4: IMPLEMENTATION PHASES
PHASE 0: Setup Infrastructure (Weeks 1-2)
Tasks:

 Deploy PostgreSQL (3 instances: prod, staging, dev)
 Deploy RabbitMQ (message broker)
 Deploy Redis (caching)
 Setup API Gateway (Kong or AWS API Gateway)
 Setup Kubernetes cluster (locally: Docker Desktop, or cloud)
 Create Docker registry (Docker Hub or private)
 Setup CI/CD (GitHub Actions)
 Setup monitoring (Prometheus, Grafana, ELK)
Output: Infrastructure ready, monolith still running, no code changes

PHASE 1: Extract User Service (Weeks 3-5)
Tasks:

Code Changes Required:

New file: Program.cs

Result: User Service running independently, can deploy separately

PHASE 2: Extract Product & Category Service (Weeks 6-9)
Tasks:

Result: Product Service running independently with caching, scales separately

PHASE 3: Extract Order Service (Weeks 10-13)
Tasks:

Result: Order Service running independently, handles failures gracefully

PHASE 4: Stabilization & Optimization (Weeks 14-16)
Tasks:

 Load testing: Simulate 100 concurrent users
 Performance tuning: Database indexes, query optimization
 Monitoring: Setup dashboards for each service
 Alerting: CPU > 80%, Error rate > 1%, Latency p95 > 500ms
 Documentation: How to deploy, scale, troubleshoot each service
 Team training: How to develop/test services independently
PART 5: TECHNOLOGY STACK DECISIONS
Language Choice Per Service
Service	Language	Framework	Database	Port	Why
User	C#	ASP.NET Core 9	PostgreSQL	5001	Auth simple logic, keep .NET
Product	C#	ASP.NET Core 9	PostgreSQL	5002	Complex queries, JOINs
Order	C#	ASP.NET Core 9	PostgreSQL	5003	Financial data, ACID important
API Gateway	Go/Rust	Kong	N/A	80/443	Ultra-fast routing
Why all C# .NET?

Existing codebase in C#
Strong type safety (financial/user data)
Excellent EF Core ORM
Team knows .NET
Easy migration
PART 6: MESSAGING & EVENTS
RabbitMQ Configuration
PART 7: API GATEWAY CONFIGURATION
Kong Routes
PART 8: TESTING STRATEGY
Unit Tests
[TestClass]
public class UsersServiceTests
{
    [TestMethod]
    public async Task RegisterUser_WithValidEmail_ReturnedUserDTO()
    {
        var request = new UserToRegisterDTO { 
            Email = "test@example.com",
            Password = "SecurePass123!"
        };
        var result = await _service.RegisterUser(request);
        Assert.IsNotNull(result);
    }
}
[TestClass]
public class UsersRepositoryIntegrationTests
{
    [TestMethod]
    public async Task CreateUser_InsertedCorrectly()
    {
        var user = new User { Email = "test@example.com" };
        await _repo.AddUser(user);
        var retrieved = await _repo.GetUserById(user.Id);
        Assert.AreEqual("test@example.com", retrieved.Email);
    }
}


101 x 13
I apologize! The file creation tool is currently disabled on your system. However, I can provide you with the complete content that you can copy directly into the file you opened.

Here's the complete WebApiShop Microservices Implementation Plan. Copy everything from the line below to the end:

❌ Can't scale Products independently
❌ Can't scale Orders independently
❌ Database lock contention
❌ One change requires redeploying entire application
❌ Hard to test individual services
❌ No clear separation of concerns
❌ Orders directly queries Products (tight coupling)

BEFORE (Monolith):
┌─────────────────────────────────┐
│ WebApiShop (All-in-One) │
│ Users + Products + Orders │
│ Single SQL Server DB │
└─────────────────────────────────┘

AFTER (Microservices):
┌──────────────────┬────────────────┬──────────────────┬──────────────┐
│ USER SERVICE │ PRODUCT SERVICE│ ORDER SERVICE │AUTH SERVICE │
│ │ │ │ │
│ C# ASP.NET Core │C# ASP.NET Core │ C# ASP.NET Core │Node.js/Auth0 │
│ Port: 5001 │ Port: 5002 │ Port: 5003 │Port: 5004 │
│ PostgreSQL │PostgreSQL │ PostgreSQL │External/JWT │
│ 2 replicas │ 3 replicas │ 3 replicas │ 1 replica │
└──────────────────┴────────────────┴──────────────────┴──────────────┘
↓ ↓ ↓
┌────────────────────────────────────────────┐
│ API GATEWAY (Kong/AWS) │
│ Routes: /api/users → User Service │
│ /api/products → Product Service │
│ /api/orders → Order Service │
└────────────────────────────────────────────┘
↓
┌────────────────────────────────────────┐
│ RabbitMQ Message Broker │
│ (Event-driven communication) │
└────────────────────────────────────────┘

API Endpoints:

Caching: Redis for product details (TTL: 5 min)

Scaling: 3 replicas (HPA for CPU > 70%)

Port: 5002

SERVICE 2: ORDER SERVICE
Responsibility: Order Management & Processing

Technology Stack:

Framework: C# + ASP.NET Core 9.0
Database: PostgreSQL
Why: Complex transactions, ACID guarantees needed for orders
Language: C# (strong type safety for financial data)
Scope (Extract these from monolith):

OrdersTbl entity
OrderItemsTbl entity
OrderController endpoints
OrderRepository
OrderService
Database Tables (PostgreSQL):

API Endpoints:

Key Constraint: When creating order, Order Service calls Product Service API

Caching: Redis for recent orders (TTL: 30 min)

Scaling: 3 replicas (HPA for CPU > 70%, or message queue depth)

Port: 5003

SERVICE 3: USER SERVICE
Responsibility: User Authentication & Profiles

Technology Stack:

Framework: C# + ASP.NET Core 9.0
Database: PostgreSQL
Why: Credential security, consistency with other services
Alternative: Node.js for faster auth (optional upgrade later)
Scope (Extract these from monolith):

User entity
UsersController endpoints (login, register, profile)
PasswordService (validation logic)
UsersRepository
UsersServices
Database Tables (PostgreSQL):

API Endpoints:

Token Strategy:

Type: JWT Bearer Token
Expiry: 24 hours (access), 30 days (refresh)
Validation: API Gateway calls /api/auth/validate-token endpoint
Caching: Redis for token validation (TTL: 5 min)

Scaling: 2 replicas (auth is lightweight)

Port: 5001

SERVICE 4: RATING SERVICE (Optional, can stay in monolith initially)
Responsibility: Product Reviews & Ratings

Current State: Simple Read/Write, Rating entity exists

Decision: Keep in monolith OR extract later

If kept in monolith: Simpler, no cross-service calls
If extracted: Independent scaling if needed
Future Option: Extract to separate service if volume grows

PART 4: IMPLEMENTATION PHASES
PHASE 0: Setup Infrastructure (Weeks 1-2)
Tasks:

 Deploy PostgreSQL (3 instances: prod, staging, dev)
 Deploy RabbitMQ (message broker)
 Deploy Redis (caching)
 Setup API Gateway (Kong or AWS API Gateway)
 Setup Kubernetes cluster (locally: Docker Desktop, or cloud)
 Create Docker registry (Docker Hub or private)
 Setup CI/CD (GitHub Actions)
 Setup monitoring (Prometheus, Grafana, ELK)
Output: Infrastructure ready, monolith still running, no code changes

PHASE 1: Extract User Service (Weeks 3-5)
Tasks:

Code Changes Required:

New file: Program.cs

Result: User Service running independently, can deploy separately

PHASE 2: Extract Product & Category Service (Weeks 6-9)
Tasks:

Result: Product Service running independently with caching, scales separately

PHASE 3: Extract Order Service (Weeks 10-13)
Tasks:

Result: Order Service running independently, handles failures gracefully

PHASE 4: Stabilization & Optimization (Weeks 14-16)
Tasks:

 Load testing: Simulate 100 concurrent users
 Performance tuning: Database indexes, query optimization
 Monitoring: Setup dashboards for each service
 Alerting: CPU > 80%, Error rate > 1%, Latency p95 > 500ms
 Documentation: How to deploy, scale, troubleshoot each service
 Team training: How to develop/test services independently
PART 5: TECHNOLOGY STACK DECISIONS
Language Choice Per Service
Service	Language	Framework	Database	Port	Why
User	C#	ASP.NET Core 9	PostgreSQL	5001	Auth simple logic, keep .NET
Product	C#	ASP.NET Core 9	PostgreSQL	5002	Complex queries, JOINs
Order	C#	ASP.NET Core 9	PostgreSQL	5003	Financial data, ACID important
API Gateway	Go/Rust	Kong	N/A	80/443	Ultra-fast routing
Why all C# .NET?

Existing codebase in C#
Strong type safety (financial/user data)
Excellent EF Core ORM
Team knows .NET
Easy migration
PART 6: MESSAGING & EVENTS
RabbitMQ Configuration
PART 7: API GATEWAY CONFIGURATION
Kong Routes
PART 8: TESTING STRATEGY
Unit Tests
Integration Tests
PART 9: MONITORING & ALERTS
Key Metrics

User Service:
  - Login success rate: > 99.5%
  - Auth latency: < 100ms

Product Service:
  - Search latency p95: < 200ms
  - Cache hit ratio: > 80%

Order Service:
  - Creation success rate: > 99.9%
  - Creation latency p95: < 2s

  PART 10: TIMELINE
  Week 1-2:   ✓ Infrastructure setup
Week 3-5:   ✓ User Service extracted
Week 6-9:   ✓ Product Service extracted
Week 10-13: ✓ Order Service extracted
Week 14-16: ✓ Testing & optimization

PART 11: RISKS & MITIGATION
Risk	Mitigation
Data loss	Dual-write, verification before cutover
Service down	Circuit breaker, fallback data
Performance regression	Load testing, metric comparison
Operational complexity	Monitoring, runbooks, training
CONCLUSION
This plan transforms WebApiShop from monolith into 3 independent microservices over 16 weeks.

Expected Effort:

Infrastructure: 40 person-days
Development: 120 person-days
Testing: 60 person-days
Documentation: 20 person-days
Total: ~240 person-days (5 weeks full team)
Key Benefits:
✓ Independent deployment
✓ Independent scaling
✓ Fault isolation
✓ Team autonomy

Plan Version: 1.0
Date: February 24, 2026
Status: READY FOR IMPLEMENTATION
