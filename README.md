## Ava Trading News. Architecture Proposal (Dev Home Assignment)

## Business use case (recap)
Centralize and enrich trading news (today used in AvaTradeGo/WebTrader) to serve them on marketing websites/landing pages:

1. **Every hour** fetch news from a provider (initially: Polygon `/v2/reference/news`).
2. For **each news item**:  
   a) **enrich** (e.g., mini ticker chart, price change %),  
   b) **persist**.
3. WebAPI for clients:  
   a) `[Authorize]` Get all news,  
   b) `[Authorize]` News from “today − {n} days”,  
   c) `[Authorize]` News by instrument (default limit = 10),  
   d) `[Authorize]` News that contains {text},  
   e) `[Authorize]` Subscribe,  
   f) `[Public]` Latest N (top distinct instruments) - for conversion.

---

## Solution Options

### Option A. **Modular Monolith on .NET 8** (selected & implemented)
**Idea:** a single service split into clear layers (Domain, Application, Infrastructure, API) in one deployment. Ingestion runs on a schedule in the same process.

**Components (one-liners):**
- **API (ASP.NET Core 8)** - handles requests, validates JWT, maps DTO↔Domain, returns `ProblemDetails`.
- **Auth (JwtBearer via external IdP)** - validates tokens from SSO/IdP; globally protects endpoints with `[Authorize]`, except the public one.
- **Ingestion (Quartz.NET job)** - hourly pull from the provider, paginate, run enrichment and persist to DB.
- **Application Services** - orchestrate use cases (search/filters/subscriptions/ingestion/enrichment) via ports/interfaces.
- **Provider Adapter (Polygon) [inside Infrastructure]** - HTTP to `/v2/reference/news`, map responses to provider-agnostic models.
- **Enrichment (Pipeline + MarketData)** - compute price/change/sparkline/sentiment and attach to news.
- **Persistence (EF Core 8 + PostgreSQL)** - store news/subscriptions; indexes/FTS; dedup on `ProviderId`.
- **Caching (Redis, optional)** - speeds up “latest N distinct by ticker” and hot reads.
- **Observability (Serilog/OTel, HealthChecks)** - logs, metrics, dependency checks.

**API endpoints:**
- `[Authorize] GET /api/news` - all news (filters/cursor pagination)
- `[Authorize] GET /api/news/range?days=n`
- `[Authorize] GET /api/news/instrument/{symbol}?limit=10`
- `[Authorize] GET /api/news/search?q=...`
- `[Authorize] POST /api/subscriptions`
- `[AllowAnonymous] GET /api/news/latest?limit=5` - latest N **distinct by ticker**

---

### Option B. **Event-Driven / Cloud (microservices/serverless)**
**Idea:** split ingestion/enrichment into functions/workers, connect them via queues, store in a document DB, index in a search engine. API is a separate layer.

**Components (one-liners):**
- **Event Scheduler (EventBridge/Cloud Scheduler)** - triggers ingestion hourly.
- **Ingestion Function (AWS Lambda, .NET 8)** - pulls from provider, pushes raw events to a queue.
- **Queue/Topic (AWS SQS/SNS | Service Bus)** - buffer between ingestion and enrichment.
- **Enrichment Worker (AWS Lambda)** - enriches, stores document, indexes for search.
- **Storage (MongoDB / DynamoDB)** - document store for news/subscriptions.
- **Search (Search-oriented databases)** - full-text search/highlighting/relevance.
- **Files (AWS S3)** - store sparklines/images.
- **API (ASP.NET Core 8 on API Gateway/ECS)** - Auth via Cognito/Auth0; reads from DB/search.
- **Caching (Redis)** - hot sets/public feeds.
- **Observability (CloudWatch/X-Ray)** - tracing, metrics, alerts.

---

## Comparison

### 1) Technical properties
| Criterion | Option A (Monolith) | Option B (Event-Driven) |
|---|---|---|
| Complexity | Low/medium (single service) | Medium/high (multiple components, IAM, queues) |
| Scalability | Scale the whole service | Independently scale ingestion/enrichment/API |
| Read latency | Minimal (single DB) | Low, depends on indexing/consistency |
| Ingestion spikes | Limited by service resources | Excellent via queues/autoscaling |
| Consistency | ACID in a single DB | Eventual consistency (store/search) |
| Observability | Simpler (one process) | Harder (cross-service tracing) |
| Local dev | Simple | Harder |
| Cost at low load | Minimal | Serverless ok, but higher baseline complexity |
| Cost at high load | Scale entire app | Pay/scale only bottlenecks |
| Vendor lock-in | Low | Higher (SQS/SNS/Lambda, etc.) |
| Time-to-Market | Faster | Slower (infra & glue) |

### 2) Data & search
| Aspect | Option A | Option B |
|---|---|---|
| DB | PostgreSQL | MongoDB / DynamoDB |
| Full-text search | PostgreSQL | Search-oriented databases |
| News schema | More rigid (JSONB available) | Flexible document model |

---

## Recommended Solution

**Option A - Modular Monolith on .NET 8.**  
**Why:**
1. Fast **MVP/Time-to-Market** and simpler operations.  
2. Clean modularity: easy to swap provider and extend enrichment.  
3. PostgreSQL + FTS/JSONB covers search/filters.  
4. Clear **growth path**: later extract ingestion to a separate worker, add a queue and/or external search - without rewriting domain or API.

> **Status:** this option is implemented in code (single service with Api, Application, Domain, Infrastructure).

---

## Mapping of the current implementation to Option A

- **Ava.News.Api** → API layer + Quartz job (`FetchNewsJob`)  
- **Ava.News.Application** → Contracts/Use-cases (`INewsProviderClient`, `INewsRepository`, `INewsIngestionService`, `IQueryNewsService`, `ISubscriptionService`, etc.)  
- **Ava.News.Domain** → Anemic domain models (`NewsItem`, `Subscription`, `PublisherInfo`, `NewsEnrichment`)  
- **Ava.News.Infrastructure** → EF Core + PostgreSQL, repositories, **Providers/Polygon** (adapter), MarketData (stub), DI registration

**Endpoints (as implemented):**
- `[Authorize] GET /api/news`  
- `[Authorize] GET /api/news/range?days=n`  
- `[Authorize] GET /api/news/instrument/{symbol}?limit=10`  
- `[Authorize] GET /api/news/search?q=...`  
- `[Authorize] POST /api/subscriptions`  
- `[AllowAnonymous] GET /api/news/latest?limit=5`

**Auth:** external IdP (SSO), `JwtBearer` validation, `FallbackPolicy`: everything protected except `[AllowAnonymous]`.

### For more information, see this file: [`TaskListForOption1.md`](TaskListForOption1.md) and [`TaskListForOption2.md`](TaskListForOption2.md)
