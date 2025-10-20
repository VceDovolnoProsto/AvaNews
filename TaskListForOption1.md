## Option A - Modular Monolith on .NET 8 (selected & implemented)

### Task List (hours only)

| #   | Task                                               | Deliverable                                                                                          | Est. (hours) |
|-----|----------------------------------------------------|------------------------------------------------------------------------------------------------------|--------------|
| A1  | Bootstrap, DI, config, Swagger, Auth wiring        | Solution skeleton, DI, config binding, Swagger UI, JwtBearer validation                              | **2–3**      |
| A2  | Domain & API contracts                             | Anemic models (`NewsItem`, `Subscription`, `PublisherInfo`, `NewsEnrichment`), DTOs                  | **2–3**      |
| A3  | EF Core setup                                      | `NewsDbContext`, migrations, indexes (unique `ProviderId`, `PublishedUtc`), JSONB collections        | **4–5**      |
| A4  | Repositories & queries                             | `INewsRepository`, `ISubscriptionRepository`, filters, cursor pagination, DISTINCT-by-ticker         | **5–6**      |
| A5  | Provider client (Polygon)                          | `INewsProviderClient` + HttpClientFactory + Polly (timeouts/retries) + mapping `/v2/reference/news`  | **3–4**      |
| A6  | Ingestion + Quartz                                 | `INewsIngestionService`, hourly `FetchNewsJob`, paging via `next_url`, dedup by `ProviderId`         | **3–4**      |
| A7  | Enrichment baseline                                | `IEnrichmentPipeline` + `IMarketDataProvider` (stub), price delta (sparkline optional)               | **2–3**      |
| A8  | Subscriptions logic                                | `ISubscriptionService` + controller `POST /api/subscriptions` + repo + validation                    | **3–4**      |
| A9  | Caching                                            | Redis cache for `/api/news/latest` (distinct by ticker) and hot queries                              | **2–3**      |
| A10 | API controllers & validation                       | Wire `IQueryNewsService` endpoints; model validation; ProblemDetails middleware                      | **2–3**      |
| A11 | AuthZ policies & scopes                            | Enforce `news.read` and `news.write` scopes; Swagger OAuth config                                    | **1–2**      |
| A12 | Health checks & logging                            | `/health` (DB + provider), Serilog structured logs                                                   | **1–2**      |
| A13 | Containerization & local env                       | Dockerfile; `docker-compose` (app + Postgres + optional Redis); local runbook                        | **2–3**      |
| A14 | CI/CD pipeline                                     | GitHub Actions (build/test/publish), env configs, basic secrets handling                             | **3–4**      |
| A15 | Tests (minimal)                                    | Unit (repo upsert/query, provider mapping), 1–2 API integration smokes                               | **3–4**      |
| A16 | Docs & ops notes                                   | README updates, short runbook (ingestion schedule, env vars, health endpoints)                       | **1–2**      |

**Total (Option A):** ~**45 hours**

### A1 - Bootstrap, DI, config, Swagger, Auth wiring
**As a feature owner I want** a clean solution skeleton with DI/config/Swagger and initial auth wiring **so that** the team can build & run locally with minimal friction.  
**Acceptance:**  
- Solution compiles; DI registered; Options bind `appsettings.*`.  
- Swagger UI at `/swagger`; XML comments visible.  
- JwtBearer added (placeholder IdP values acceptable).  
**Est.:** **2–3h**

### A2 - Domain & API contracts
**As a feature owner I want** anemic domain models and API DTOs **so that** our data model and HTTP contracts are clear and stable.  
**Acceptance:**  
- POCOs: `NewsItem`, `Subscription`, `PublisherInfo`, `NewsEnrichment`.  
- DTOs: `NewsItemDto`, `NewsListResponseDto`, `NewsQueryDto`, `CreateSubscriptionDto`.  
- Mapping rules (Domain ↔ DTO) documented.  
**Est.:** **2–3h**

### A3 - EF Core setup
**As a feature owner I want** a PostgreSQL schema with migrations and indices **so that** persistence is robust and queries are efficient.  
**Acceptance:**  
- `NewsDbContext` + migrations apply cleanly.  
- Indices: unique `ProviderId`, index on `PublishedUtc`.  
- Arrays persisted as JSONB; base FTS scaffolded (if used).  
**Est.:** **4–5h**

### A4 - Repositories & queries
**As a feature owner I want** filterable, cursor-paginated queries and a distinct-latest retrieval **so that** users browse efficiently and public feeds stay diverse.  
**Acceptance:**  
- Implement `INewsRepository`, `ISubscriptionRepository`.  
- `QueryAsync(NewsQuery)` supports `q/ticker/from/to/limit/cursor`.  
- `LatestDistinctByTickerAsync(limit)` returns ≤ limit unique tickers.  
**Est.:** **5–6h**

### A5 - Provider client (Polygon)
**As a feature owner I want** a resilient Polygon client behind a provider-agnostic port **so that** fetching is reliable and swappable.  
**Acceptance:**  
- `INewsProviderClient.FetchAsync` supports ticker, date range, limit, `next_url`.  
- `HttpClientFactory` + Polly timeouts/retries.  
- Maps to `ProviderArticle` / `ProviderPage`.  
**Est.:** **3–4h**

### A6 - Ingestion + Quartz
**As a feature owner I want** hourly ingestion **so that** news remain fresh without manual runs.  
**Acceptance:**  
- `INewsIngestionService` pages via `next_url`, maps to domain, dedups by `ProviderId`.  
- Quartz `FetchNewsJob` scheduled hourly; manual trigger possible.  
**Est.:** **3–4h**

### A7 - Enrichment baseline
**As a feature owner I want** baseline enrichment (price delta; optional sparkline) **so that** users see value beyond raw headlines.  
**Acceptance:**  
- `IEnrichmentPipeline` uses `IMarketDataProvider` (stub).  
- Computes `PriceChangePct`; applies enrichment before persistence.  
**Est.:** **2–3h**

### A8 - Subscriptions logic
**As a feature owner I want** a subscription creation flow **so that** customers can request notifications for new relevant news.  
**Acceptance:**  
- `ISubscriptionService` + repo create path.  
- `POST /api/subscriptions` (scope `news.write`) returns `{ id }`.  
- Validates: at least one filter; channel/target rules enforced.  
**Est.:** **3–4h**

### A9 - Caching
**As a feature owner I want** caching for hot reads **so that** `/api/news/latest` and similar queries are fast and cost-effective.  
**Acceptance:**  
- Redis (or in-memory fallback) caches `/api/news/latest` with short TTL (30–60s).  
- Keys include params; invalidation strategy documented.  
**Est.:** **2–3h**

### A10 - API controllers & validation
**As a feature owner I want** clean, validated endpoints **so that** client integrations are predictable.  
**Acceptance:**  
- Endpoints: `GET /api/news`, `/range`, `/instrument/{symbol}`, `/search`, `/latest`.  
- Model validation and `ProblemDetails` on errors.  
- Controllers call `IQueryNewsService` only.  
**Est.:** **2–3h**

### A11 - AuthZ policies & scopes
**As a feature owner I want** scope-based authorization **so that** read vs write access is enforced.  
**Acceptance:**  
- Fallback policy requires `news.read` except `/news/latest`.  
- `/api/subscriptions` requires `news.write`.  
- Swagger OAuth configured for secured calls.  
**Est.:** **1–2h**

### A12 - Health checks & logging
**As a feature owner I want** health endpoints and structured logs **so that** operability is straightforward.  
**Acceptance:**  
- `/health` checks DB + provider.  
- Serilog structured logs with correlation IDs; basic request/error logging.  
**Est.:** **1–2h**

### A13 - Containerization & local env
**As a feature owner I want** Docker and compose for local runs **so that** onboarding is quick and reproducible.  
**Acceptance:**  
- `Dockerfile` for API; `docker-compose` with Postgres (+ Redis optional).  
- `.env` and README with run instructions.  
**Est.:** **2–3h**

### A14 - CI/CD pipeline
**As a feature owner I want** a simple CI/CD pipeline **so that** builds/tests/publish are automated.  
**Acceptance:**  
- GitHub Actions (or similar): restore → build → test → publish container.  
- Secrets from CI store; artifacts retained.  
**Est.:** **3–4h**

### A15 - Tests (minimal)
**As a feature owner I want** minimal tests for critical paths **so that** regressions are caught early.  
**Acceptance:**  
- Unit: repo upsert/query; provider mapping.  
- Integration smoke: `GET /api/news/latest` returns 200 & expected shape.  
**Est.:** **3–4h**

### A16 - Docs & ops notes
**As a feature owner I want** clear docs and runbooks **so that** handover and operations are smooth.  
**Acceptance:**  
- README: setup, env vars, endpoints, auth, health, local run.  
- Short runbook: schedule, common failures, restart/rollback.  
**Est.:** **1–2h**