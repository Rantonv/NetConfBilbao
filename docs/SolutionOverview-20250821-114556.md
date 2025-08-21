# Solution Overview: GolfAll Aspire Solution

## Overview
GolfAll es una solución basada en .NET Aspire que orquesta un frontend Razor Pages y una API REST mediante un AppHost. La arquitectura prioriza modularidad, telemetría y resiliencia compartidas.

## Arquitectura
- GolfAllWeb.AppHost (Aspire Host): orquesta los servicios y expone el dashboard.
- GolfAllApi: API REST con Swagger y CORS para el frontend.
- GolfAllWeb: Frontend Razor Pages.
- GolfAllWeb.ServiceDefaults: configuración común (telemetría, healthchecks, etc.).

### Diagrama de Arquitectura (Mermaid)
```mermaid
graph TD;
    A[GolfAllWeb.AppHost (Aspire Host)] --> B[GolfAllApi (API Service)];
    A --> C[GolfAllWeb (Frontend)];
    B --> D[GolfAllWeb.ServiceDefaults];
    C --> D[GolfAllWeb.ServiceDefaults];
    B -- Exposes REST API --> C;
    C -- Calls API --> B;
```

## Componentes
- AppHost: define proyectos "apiservice" y "webfrontend", hace referencia de web a api y espera su disponibilidad.
- Api: Endpoints REST, Swagger y CORS permitiendo https://localhost:7206 y http://localhost:5274.
- Web: Razor Pages, integra ServiceDefaults.
- ServiceDefaults: instrumentación y defaults compartidos.

## Screenshots
- Aspire Dashboard:
  ![Aspire Dashboard](screenshots/aspire-dashboard.png)
- Frontend principal:
  ![Frontend Principal](screenshots/frontend-main.png)

---
Generado automáticamente; capturas vía Playwright.
