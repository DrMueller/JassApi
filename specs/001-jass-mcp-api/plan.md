# Implementation Plan: Jass MCP API

**Branch**: `001-jass-mcp-api` | **Date**: 2026-10-08 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-jass-mcp-api/spec.md`; existing SQL schema inspected in sibling source but not referenced by the API

## Summary

Expose existing Jass data through an Entra ID-protected MCP service. The API owns its EF Core mappings, read queries and required Jass scoring calculations, targets .NET 9, and maps to the already-existing SQL Server tables. It has no project/runtime dependency on the sibling `JassApp` or its Blazor UI.

## Technical Context

**Language/Version**: C# / .NET 9 (`net9.0`).

**Primary Dependencies**: ASP.NET Core Web API; Model Context Protocol C# SDK (`ModelContextProtocol.AspNetCore` 1.4.1); Microsoft Identity Web 3.12.0 for Entra ID JWT bearer authentication; EF Core SQL Server 9.0.7; API-owned domain/query and persistence types mapped to the existing SQL schema.

**Storage**: Existing SQL Server database accessed by API-owned EF Core 9 entities and read-only query service. Keep the existing database and table/column names authoritative; do not add migrations or require the sibling application at build/runtime.

**Testing**: xUnit, Moq, and FluentAssertions on `net9.0`; include domain parity, MCP contract, authorization, and data-source integration coverage.

**Target Platform**: ASP.NET Core server on .NET 9; deployment topology remains to be confirmed.

**Project Type**: ASP.NET Core web service exposing regular API capabilities and an MCP Streamable HTTP endpoint.

**Performance Goals**: At least 95% of successful acceptance questions complete within 3 seconds, excluding confirmed delays from the existing data source (spec SC-002).

**Constraints**: Read-only access to the existing database; Entra ID authentication and authorization before protected data is returned; reuse Jass domain rules; no browser UI dependency; do not invent data when there is no authoritative result.

**Scale/Scope**: Initial release supports an agreed set of Jass questions and one or more authorized MCP clients; traffic volume and deployment topology are not specified.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

The constitution file contains only template placeholders and defines no ratified project principles or governance constraints. No constitution-specific gate can be evaluated. This plan preserves the existing SQL Server database as authoritative, limits access to read operations, and requires Entra ID authorization.

**Pre-research gate**: Pass with no identifiable constitution violations. Repository inventory is a prerequisite to selecting .NET version, database integration, or source layout.

**Post-design gate**: Pass. The design uses an API-owned, read-only schema/query boundary, has no browser dependency, avoids database writes, and protects the MCP endpoint.

## Project Structure

### Documentation (this feature)

```text
specs/001-jass-mcp-api/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── implementation-inputs.md
└── contracts/
    └── mcp-tools.md
```

### Source Code (repository root)

The API owns its domain and persistence code and does not project-reference the sibling application.

```text
src/
└── JassApi/                  # API host, MCP transport, API auth and adapters

tests/
├── JassApi.UnitTests/
├── JassApi.IntegrationTests/
└── JassApi.ContractTests/
```

**Structure Decision**: Keep `JassApi` as a self-contained ASP.NET Core host with API-owned read-only persistence entities mapped to the existing table schema, narrow query service, DTOs and required Jass result calculations. No project reference to the sibling app, Blazor initialization, write repositories, or migration project is included.

## Implementation Sequence

1. Keep the API project independently buildable on .NET 9 with SQL Server/EF Core 9 persistence mapped to the existing table and column schema; do not reference or start the sibling application.
2. Implement API-owned player/round read queries and port only the required Jass calculations while preserving behavior through regression tests.
3. Configure Entra ID JWT bearer validation and explicit authorization for protected API/MCP operations. Confirm accepted token audience and delegated scope/app permission with the tenant/app registration owner before deployment.
4. Configure API-owned SQL Server/EF Core services with short-lived contexts and read-only endpoints; do not enable sensitive-data SQL logging or database writes.
5. Register the MCP server over Streamable HTTP and expose two documented structured data tools: player listing and filtered completed-round listing that includes existing team, result, and trump-round data. The MCP client interprets natural language.
6. Add tests following the existing xUnit/Moq/FluentAssertions conventions: domain behavior parity, MCP contract behavior, authorized/unauthorized requests, and read-only integration with SQL Server.
7. Validate the acceptance scenarios and measurable outcomes in `quickstart.md`; verify no database writes or browser-only dependencies are introduced.

## Complexity Tracking

No constitution violations identified. The API owns only the domain behavior needed by its tools; no separate shared project or sibling application reference is required.
