# Architecture Index

## Implementation Guide

**This index is not the full contract. Do not implement from this file alone; follow the artifact paths below.**

These artifacts describe the existing application. No rewrite/migration request, declared cut, or target stack was supplied; consequently no `migration_boundary.yaml` or `seams.yaml` is emitted. `source_anchors` identify evidence and are not an implementation-scope directive.

### Global artifacts

- `unit_graph.yaml`
  - Who reads: all workers.
  - Use for: route-triggered units, source anchors, DI entry points, and shared references.
  - How to consume: find the assigned unit; treat shared modules separately from unit-owned page/component files.
- `shared_modules.yaml`
  - Who reads: workers touching shared routing/layout/bootstrap.
  - Use for: shared shell surfaces and fields.
  - How to consume: filter `used_by_units` for the assigned unit; only the listed `fields` are recorded.
- `wire_contracts.yaml`
  - Who reads: workers touching browser persistence or the static weather response.
  - Use for: the local-storage record and static JSON asset contract.
  - How to consume: filter by `unit`.
- `cross_unit_state.yaml`
  - Who reads: all workers handling persisted or cross-page state.
  - Use for: state that is implicitly passed between externally triggered units.
  - How to consume: filter by `writer.unit` or `reader.unit`; the current flow list is empty because no cross-unit flow was found.
- `project-structure.md`
  - Who reads: all workers.
  - Use for: application layers, route entry points, shared shell, and file counts.
- `tech-stack.md`
  - Who reads: all workers.
  - Use for: declared frameworks, package versions, browser requirements, and launch configuration.
- `data-model.md`
  - Who reads: workers changing game or weather data.
  - Use for: model fields, relationships, and browser persistence shape.

### Unit: `bingo-home`

- External trigger: Blazor route `/`.
- Must read:
  - `units/bingo-home/behavior.yaml` — selection, win, persistence, restoration, and failure behavior.
  - `units/bingo-home/bindings.yaml` — route, DI, component event, and browser API bindings.
  - `units/bingo-home/unit_decomposition.yaml` — advisory helper-split candidates only (`commit: false`).
- Relevant global rows:
  - `wire_contracts.yaml`: filter `unit: bingo-home`.
  - `shared_modules.yaml`: filter `used_by_units` containing `bingo-home`.
  - `cross_unit_state.yaml`: filter flows whose writer or reader is `bingo-home`.
- Before DONE report: artifact paths read, preserved state/win behavior, unresolved contracts, and build/runtime evidence.

### Unit: `counter-page`

- External trigger: Blazor route `/counter`.
- Must read:
  - `units/counter-page/behavior.yaml` — initial state and click increment.
  - `units/counter-page/bindings.yaml` — route and click binding.
  - `units/counter-page/unit_decomposition.yaml` — advisory split candidates only (`commit: false`).
- Relevant global rows:
  - `shared_modules.yaml`: filter `used_by_units` containing `counter-page`.
  - `cross_unit_state.yaml`: filter flows whose writer or reader is `counter-page`.
- Before DONE report: artifact paths read, preserved count behavior, unresolved contracts, and verification evidence.

### Unit: `weather-page`

- External trigger: Blazor route `/weather`.
- Must read:
  - `units/weather-page/behavior.yaml` — loading and rendering branches plus request failures.
  - `units/weather-page/bindings.yaml` — route, HttpClient injection, lifecycle request, and model binding.
  - `units/weather-page/unit_decomposition.yaml` — advisory split candidates only (`commit: false`).
- Relevant global rows:
  - `wire_contracts.yaml`: filter `unit: weather-page`.
  - `shared_modules.yaml`: filter `used_by_units` containing `weather-page`.
  - `cross_unit_state.yaml`: filter flows whose writer or reader is `weather-page`.
- Before DONE report: artifact paths read, preserved static JSON contract, unresolved error handling, and verification evidence.

### Unit: `not-found-page`

- External trigger: unmatched Blazor route; also direct route `/not-found`.
- Must read:
  - `units/not-found-page/behavior.yaml` — fallback text and rendering.
  - `units/not-found-page/bindings.yaml` — direct route and router fallback binding.
  - `units/not-found-page/unit_decomposition.yaml` — advisory split candidates only (`commit: false`).
- Relevant global rows:
  - `shared_modules.yaml`: filter `used_by_units` containing `not-found-page`.
  - `cross_unit_state.yaml`: filter flows whose writer or reader is `not-found-page`.
- Before DONE report: artifact paths read, preserved fallback behavior, unresolved contracts, and verification evidence.
