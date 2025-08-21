# Solution Overview: GolfAll Aspire Solution

## Overview
GolfAll es una solución basada en .NET Aspire que orquesta un frontend Razor Pages y una API REST mediante un AppHost. La arquitectura prioriza modularidad, telemetría y resiliencia compartidas.

## Arquitectura (ASCII)

+------------------------+
|   GolfAllWeb.AppHost   |
|     (Aspire Host)      |
+-----------+------------+
            |
            | orchestrates & waits
            v
   +--------+--------+               +-----------------+
   |   GolfAllWeb    |<------------->|   GolfAllApi    |
   | (Razor Frontend)|   calls API   |  (REST + Swagger)
   +--------+--------+               +-----------------+
            |                                  ^
            | uses ServiceDefaults             |
            v                                  |
     +------+----------------+-----------------+
     |       GolfAllWeb.ServiceDefaults        |
     | (telemetry, health, defaults, etc.)     |
     +-----------------------------------------+

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
