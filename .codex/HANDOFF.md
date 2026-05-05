# Codex Handoff

Last updated: 2026-05-05

## Current State

The first usable slice for investigation telemetry is tightened enough to continue from here.

Implemented:

- `InsightTelemetryCore` defines the common investigation model: correlation, causation, session, trace, diagnostics, CDC, event envelope, and sinks.
- `MyGateKeeper` references `InsightTelemetryCore`.
- `AuthServer`, `AuthorizationServer`, and `ResourceServer` reference `InsightTelemetryCore`, so the shared model is available across the flow.
- `MyGateKeeper` owns the filesystem sink.
- Dashboard context flow writes structured investigation events.
- `X-Correlation-Id` is propagated from gateway to Auth/AuthZ/Resource calls.
- Investigation files are written under `MyGateKeeper/logs/investigations/{correlationId}.jsonl`.

## Validation Already Run

```powershell
dotnet build MyGateKeeper\MyGateKeeper.csproj
dotnet build AuthorizationServer\AuthorizationServer.csproj
dotnet build ResourceServer\ResourceServer.csproj
dotnet build AuthServer\AuthenticationServer.csproj
```

## Next Useful Slice

- Re-run the builds after any further changes.
- Add or verify an end-to-end request path that creates one correlation ID and produces a matching `.jsonl` investigation file.
- Inspect one generated investigation file for event shape consistency across gateway, auth, authorization, and resource calls.
- Decide whether investigation logs should remain local-only or get a configurable sink abstraction for future storage backends.

## Notes For Future Sessions

Start by reading this file, then inspect current git status before making changes. Do not assume prior chat/session memory is available.
