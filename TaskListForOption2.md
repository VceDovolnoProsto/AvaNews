
## Option B - Event-Driven / Cloud (microservices/serverless) - **Revised Estimates**

### Task List (hours only)

| #   | Task                                        | Deliverable                                                                                 | Est. (hours) |
|-----|---------------------------------------------|---------------------------------------------------------------------------------------------|--------------|
| B1  | IaC & Cloud Bootstrap (minimal)             | Base IaC stack (network/tags), secrets/KMS, core IAM for functions & API                    | **12–16**    |
| B2  | Scheduler (hourly)                          | EventBridge/Cloud Scheduler rule that triggers ingestion hourly                             | **2–4**      |
| B3  | Ingestion Function                          | Lambda/.NET 8: fetch Polygon `/v2/reference/news`, page via `next_url`, emit messages       | **8–12**     |
| B4  | Queue/Topic + DLQ + IAM                     | SQS/SNS (or Service Bus) with retries/backoff, DLQ, least-privilege IAM                     | **6–8**      |
| B5  | Enrichment Worker                           | Lambda/.NET 8: consume queue, enrich (market deltas, optional sparkline), persist              | **12–18**    |
| B6  | Storage Layer                               | MongoDB/Dynamo upsert + dedup by `ProviderId`, secondary indexes                            | **8–12**     |
| B7  | Search Layer                                | Search-oriented database. Search index, mappings, ingest/update pipeline                             | **12–18**    |
| B8  | Files Bucket                                 | S3 for sparkline/images, optional signed URL helper                                     | **2–4**      |
| B9  | API (Gateway + ASP.NET Core)                | Read/search endpoints, Gateway routing, consistent JSON, ProblemDetails                     | **8–12**     |
| B10 | Auth Integration (IdP & Scopes)             | OIDC config, JWT validation, scopes `news.read`/`news.write`, Swagger OAuth                 | **4–6**      |
| B11 | Caching                                     | Redis for `/news/latest` & hot queries (short TTL)                              | **3–5**      |
| B12 | Observability                               | Centralized logs, traces across function→queue→worker→API, alarms                           | **6–10**     |
| B13 | CI/CD (multi-artifact)                      | Pipelines for functions, API, and IaC (build/test/deploy per artifact)                      | **12–16**    |
| B14 | Tests (unit/integration/smoke)              | Function unit tests, worker integration (mock queue/store), API smoke                       | **6–10**     |
| B15 | Docs & Runbooks                             | README, env setup, on-call (retry/DLQ, replays), cost notes                                 | **3–5**      |
| B16 | Cost Guardrails                             | Budget alerts, tags, throttles/quotas                                                       | **2–4**      |

**Total (Option B):** **106–160 hours**

---

### User Stories (hours only, revised)

**B1 - IaC & Cloud Bootstrap (minimal)**  
**As a feature owner I want** a reproducible cloud baseline via IaC **so that** environments are provisioned consistently and securely.  
**Acceptance:** Base stack applied; VPC/subnets (or equivalent), secrets in KMS/KeyVault/Parameter Store, IAM roles for functions/API, tagging policy.  
**Est.:** **12–16h**

### B2 - Hourly Scheduler
**As a feature owner I want** an hourly trigger for ingestion **so that** news are fetched regularly without manual actions.  
**Acceptance:** EventBridge/Cloud Scheduler invokes ingestion on schedule; on-demand trigger supported.  
**Est.:** **2–4h**

### B3 - Ingestion Function
**As a feature owner I want** a function that fetches pages from Polygon and publishes messages **so that** enrichment is decoupled from provider rate/latency.  
**Acceptance:** Lambda (.NET 8) calls `/v2/reference/news`, honors filters/limit, follows `next_url`, publishes messages; Polly retries/timeouts.  
**Est.:** **8–12h**

### B4 - Queue/Topic with DLQ & IAM
**As a feature owner I want** resilient queuing with backoff and dead-lettering **so that** poison messages don’t block the flow.  
**Acceptance:** Queue + DLQ configured; backoff/retries; least-privilege IAM; queue metrics visible.  
**Est.:** **6–8h**

### B5 - Enrichment Worker
**As a feature owner I want** a worker that enriches news and persists them **so that** consumers get value-added data.  
**Acceptance:** Consumes queue; computes price delta via `IMarketDataProvider`; (optional) sparkline; builds `NewsEnrichment`; idempotent upsert by `ProviderId`.  
**Est.:** **12–18h**

### B6 - Storage Layer
**As a feature owner I want** an upsertable document model with dedup **so that** reprocessing doesn’t create duplicates.  
**Acceptance:** Mongo/Dynamo with unique key on `ProviderId`; secondary indexes for `PublishedUtc`/`Tickers`; upsert implemented.  
**Est.:** **8–12h**

### B7 - Search Layer
**As a feature owner I want** fast full-text search **so that** users can find relevant news by text/time/ticker.  
**Acceptance:** Search index + mappings; searchable fields; update pipeline; relevance ranking; cursor pagination.  
**Est.:** **12–18h**

### B8 - Files Bucket
**As a feature owner I want** storage for sparklines/images **so that** the API can reference stable assets.  
**Acceptance:** S3 container created; folder convention; optional signed URL helper.  
**Est.:** **2–4h**

### B9 - API (Gateway + ASP.NET Core)
**As a feature owner I want** consistent, documented endpoints **so that** integrations are straightforward.  
**Acceptance:** Gateway routes to ASP.NET Core; endpoints mirror Option A (list/range/instrument/search/latest); ProblemDetails; Swagger docs.  
**Est.:** **8–12h**

### B10 - Auth Integration (IdP & Scopes)
**As a feature owner I want** OIDC/JWT with scope checks **so that** access is controlled per contract.  
**Acceptance:** `news.read` required for all but `/news/latest`; `news.write` for `/subscriptions`; Swagger OAuth wired.  
**Est.:** **4–6h**

### B11 - Caching
**As a feature owner I want** Redis caching for hot reads **so that** latency and cloud cost are reduced.  
**Acceptance:** `/api/news/latest` cached with short TTL; param-aware keys; basic cache metrics.  
**Est.:** **3–5h**

### B12 - Observability
**As a feature owner I want** logs/traces/metrics and alarms **so that** the team can operate & debug the pipeline.  
**Acceptance:** Structured logs; distributed traces across function→queue→worker→API; alarms on queue depth, DLQ count, error rate.  
**Est.:** **6–10h**

### B13 - CI/CD (multi-artifact)
**As a feature owner I want** build/test/deploy pipelines per artifact **so that** changes ship safely.  
**Acceptance:** Pipelines for functions, API, and IaC; environment promotion; rollback guidance; basic approvals.  
**Est.:** **12–16h**

### B14 - Tests (unit/integration/smoke)
**As a feature owner I want** minimal tests for critical paths **so that** regressions are caught early.  
**Acceptance:** Unit tests (ingestion mapping, worker logic); integration (mock queue/store); API smoke (200 OK + shape).  
**Est.:** **6–10h**

### B15 - Docs & Runbooks
**As a feature owner I want** clear docs and runbooks **so that** onboarding and on-call are smooth.  
**Acceptance:** README with setup; runbooks for DLQ handling, retries, replays, cost notes; endpoint catalog.  
**Est.:** **3–5h**

### B16 - Cost Guardrails
**As a feature owner I want** budget alerts and basic quotas **so that** spend remains predictable.  
**Acceptance:** Budget alarms; per-resource tags; concurrency/throughput caps where applicable.  
**Est.:** **2–4h**