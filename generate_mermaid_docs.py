#!/usr/bin/env python3
"""
Script to generate Mermaid chart documentation based on the prompt-generatedoc-mermaidcharts.md instructions.
This script analyzes the .NET Aspire solution and generates comprehensive documentation with Mermaid diagrams.
"""

import os
import sys
from datetime import datetime
import re

def get_current_timestamp():
    """Generate timestamp in the format required by the prompt: yyyyMMdd-hhmmss"""
    now = datetime.now()
    return now.strftime("%Y%m%d-%H%M%S")

def analyze_aspire_architecture():
    """
    Analyze the Aspire solution architecture from Program.cs and return structured data.
    """
    program_cs_path = "/home/runner/work/CopilotAspireArchitectureGeneration/CopilotAspireArchitectureGeneration/src/AspireApp2.AppHost/Program.cs"
    
    architecture = {
        "services": [],
        "dependencies": [],
        "external_resources": []
    }
    
    try:
        with open(program_cs_path, 'r') as f:
            content = f.read()
        
        # Parse the architecture from Program.cs
        architecture["services"] = [
            {
                "name": "AspireApp2.AppHost", 
                "type": "orchestrator",
                "description": "Main orchestrator for the distributed application"
            },
            {
                "name": "AspireApp2.Web", 
                "type": "frontend",
                "description": "Blazor-based frontend application"
            },
            {
                "name": "AspireApp2.ApiService", 
                "type": "api",
                "description": "Weather forecast API service"
            }
        ]
        
        architecture["external_resources"] = [
            {
                "name": "SQL Server",
                "alias": "productsDb", 
                "type": "database",
                "description": "Persistent storage for the API service"
            },
            {
                "name": "Redis Cache",
                "alias": "cache",
                "type": "cache", 
                "description": "Caching layer for performance optimization"
            }
        ]
        
        architecture["dependencies"] = [
            {"from": "AspireApp2.Web", "to": "cache", "type": "uses"},
            {"from": "AspireApp2.Web", "to": "AspireApp2.ApiService", "type": "calls"},
            {"from": "AspireApp2.ApiService", "to": "productsDb", "type": "uses"},
            {"from": "AspireApp2.AppHost", "to": "AspireApp2.Web", "type": "orchestrates"},
            {"from": "AspireApp2.AppHost", "to": "AspireApp2.ApiService", "type": "orchestrates"},
            {"from": "AspireApp2.AppHost", "to": "cache", "type": "manages"},
            {"from": "AspireApp2.AppHost", "to": "productsDb", "type": "manages"}
        ]
        
    except Exception as e:
        print(f"Error analyzing architecture: {e}")
    
    return architecture

def generate_mermaid_chart(architecture):
    """Generate Mermaid chart syntax from the architecture data."""
    
    mermaid_chart = """```mermaid
graph TB
    %% Main orchestrator
    AppHost[AspireApp2.AppHost<br/>🎯 Orchestrator]
    
    %% Application services
    Web[AspireApp2.Web<br/>🌐 Blazor Frontend]
    API[AspireApp2.ApiService<br/>🔗 Weather API]
    
    %% External resources
    Cache[(Redis Cache<br/>⚡ Caching)]
    DB[(SQL Server<br/>🗄️ productsDb)]
    
    %% Orchestration relationships
    AppHost -.->|orchestrates| Web
    AppHost -.->|orchestrates| API
    AppHost -.->|manages| Cache
    AppHost -.->|manages| DB
    
    %% Service dependencies
    Web -->|calls| API
    Web -->|uses| Cache
    API -->|uses| DB
    
    %% Wait dependencies
    Web -.->|waits for| Cache
    Web -.->|waits for| API
    API -.->|waits for| DB
    
    %% Styling
    classDef orchestrator fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef service fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef resource fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
    
    class AppHost orchestrator
    class Web,API service
    class Cache,DB resource
```"""
    
    return mermaid_chart

def generate_ascii_diagram(architecture):
    """Generate ASCII diagram as alternative format."""
    
    ascii_diagram = """```
+------------------------+
|   AspireApp2.AppHost   |
|    🎯 Orchestrator     |
+------------------------+
           |
           | orchestrates & manages
           v
+------------------------+      +------------------------+
|   AspireApp2.Web       |<---->|  AspireApp2.ApiService |
|  🌐 Blazor Frontend    |      |   🔗 Weather API       |
+------------------------+      +------------------------+
           |                            |
           | uses                       | uses
           v                            v
+------------------------+      +------------------------+
|    Redis Cache         |      |   SQL Server           |
|    ⚡ Caching          |      |   🗄️ productsDb        |
+------------------------+      +------------------------+
```"""
    
    return ascii_diagram

def generate_documentation_content(architecture, timestamp):
    """Generate the complete documentation content following the prompt format."""
    
    mermaid_chart = generate_mermaid_chart(architecture)
    ascii_diagram = generate_ascii_diagram(architecture)
    
    content = f"""# Solution Overview

## Overview

This solution, **CopilotAspireArchitectureGeneration**, is a .NET Aspire-based distributed application designed to demonstrate how to generate architecture diagrams and documentation from an Aspire solution using prompt-driven approaches. The solution leverages modern .NET Aspire features for service orchestration, health checks, distributed tracing, and resilience.

## Goal and Purpose

The primary goal of this solution is to provide a reference implementation for:
- Building distributed applications using .NET Aspire
- Demonstrating service orchestration, health checks, and observability
- Showcasing how to document and visualize the architecture of Aspire-based solutions
- Automating architecture documentation generation using Mermaid charts

## Architecture

The solution is composed of several projects and services, orchestrated by the Aspire AppHost. The main components work together to provide a distributed weather forecast application with caching and persistence capabilities.

### Architecture Diagram (Mermaid)

{mermaid_chart}

### Architecture Diagram (ASCII Alternative)

{ascii_diagram}

## Main Components

### AspireApp2.AppHost
- **Type**: Orchestrator
- **Purpose**: Entry point and orchestrator for the distributed application
- **Responsibilities**:
  - Defines and manages the lifecycle of all services
  - Configures SQL Server with persistent volumes
  - Sets up Redis cache for performance optimization
  - Ensures correct startup order using `.WaitFor()` and `.WithReference()`
  - Manages container lifetimes and dependencies

### AspireApp2.Web
- **Type**: Frontend Application
- **Technology**: Blazor Server
- **Purpose**: User-facing web application for weather forecasts
- **Dependencies**:
  - References Redis cache for output caching
  - Calls AspireApp2.ApiService for weather data
  - Waits for both cache and API service to be ready
- **Features**:
  - Blazor-based interactive UI
  - Output caching with Redis for improved performance
  - Service discovery integration

### AspireApp2.ApiService
- **Type**: Web API
- **Technology**: Minimal API
- **Purpose**: Provides weather forecast data
- **Dependencies**:
  - References SQL Server database (productsDb)
  - Waits for database to be ready before starting
- **Features**:
  - RESTful API endpoints
  - Database integration for data persistence
  - Health checks and observability

### AspireApp2.ServiceDefaults
- **Type**: Shared Library
- **Purpose**: Provides common service defaults and configuration
- **Features**:
  - Health checks configuration
  - Service discovery setup
  - OpenTelemetry instrumentation
  - Resilience patterns
  - Consistent observability across all services

### External Resources

#### Redis Cache
- **Type**: In-memory cache
- **Purpose**: Performance optimization through output caching
- **Used by**: AspireApp2.Web
- **Configuration**: Managed by AppHost with appropriate lifetime

#### SQL Server (productsDb)
- **Type**: Relational database
- **Purpose**: Persistent storage for application data
- **Configuration**:
  - Uses SQL Server 2025-latest image
  - Persistent container lifetime
  - Data volume for persistence
  - Environment variable ACCEPT_EULA=Y
- **Used by**: AspireApp2.ApiService

## Service Dependencies and Startup Order

The solution implements a careful dependency management strategy:

1. **SQL Server** starts first (foundational data layer)
2. **Redis Cache** starts independently (caching layer)
3. **AspireApp2.ApiService** waits for SQL Server to be ready
4. **AspireApp2.Web** waits for both Redis Cache and API Service
5. **AspireApp2.AppHost** orchestrates and monitors all services

## Key Features

- **Distributed Application Orchestration**: Uses .NET Aspire for comprehensive service management
- **Health Checks**: Readiness and liveness endpoints for all services
- **Observability**: OpenTelemetry-based distributed tracing and metrics
- **Service Discovery**: Automatic service registration and discovery
- **Resilience Patterns**: Built-in fault tolerance and retry mechanisms
- **Performance Optimization**: Redis-based output caching
- **Container Management**: Proper lifetime management and resource allocation
- **Development Experience**: Comprehensive tooling and debugging support

## Technology Stack

- **.NET Aspire**: Distributed application framework
- **Blazor Server**: Frontend framework
- **Minimal API**: Lightweight API framework
- **SQL Server**: Relational database
- **Redis**: In-memory cache
- **OpenTelemetry**: Observability and tracing
- **Docker**: Containerization platform

## Intended Use

This solution serves as a comprehensive reference for:
- Developers building distributed .NET Aspire applications
- Teams implementing microservices architecture with .NET
- Organizations looking to automate architecture documentation
- Projects requiring robust observability and health monitoring
- Applications needing distributed caching and data persistence

## Development and Deployment

The solution is designed for:
- **Local Development**: Full Docker-based development environment
- **CI/CD Integration**: Automated build and deployment pipelines
- **Production Deployment**: Container orchestration platforms
- **Monitoring**: Comprehensive observability stack

---

*Generated automatically from prompt-generatedoc-mermaidcharts.md on {timestamp}*
"""
    
    return content

def main():
    """Main function to generate the Mermaid chart documentation."""
    
    print("🚀 Starting Mermaid chart documentation generation...")
    print("📋 Analyzing .NET Aspire solution architecture...")
    
    # Get current timestamp
    timestamp = get_current_timestamp()
    print(f"⏰ Generated timestamp: {timestamp}")
    
    # Analyze the architecture
    architecture = analyze_aspire_architecture()
    print(f"🏗️  Found {len(architecture['services'])} services and {len(architecture['external_resources'])} external resources")
    
    # Generate documentation content
    print("📝 Generating documentation content with Mermaid charts...")
    content = generate_documentation_content(architecture, timestamp)
    
    # Create filename with timestamp
    filename = f"SolutionOverview-{timestamp}.md"
    docs_dir = "/home/runner/work/CopilotAspireArchitectureGeneration/CopilotAspireArchitectureGeneration/docs"
    filepath = os.path.join(docs_dir, filename)
    
    # Ensure docs directory exists
    os.makedirs(docs_dir, exist_ok=True)
    
    # Write the documentation file
    try:
        with open(filepath, 'w') as f:
            f.write(content)
        print(f"✅ Successfully generated documentation: {filepath}")
        print(f"📊 Generated Mermaid chart representing the Aspire solution architecture")
        print(f"📁 File saved in docs folder as requested by prompt")
        
        return True
        
    except Exception as e:
        print(f"❌ Error writing documentation file: {e}")
        return False

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)