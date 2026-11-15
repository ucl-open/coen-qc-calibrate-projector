# Backlog

## Audit round 3 of 3 (15 Nov 2026)

- `scripts\deploy.ps1` exits 1 naming `.bonsai\Setup.ps1` when it is missing (a missing script left the last exit code, 0).

## Audit round 2 of 3 (15 Nov 2026, stage C)

- `.bonsai\Setup.ps1` is a hand copy of lib-open-rigs' `setup-bonsai.ps1` (no pyproject pins it), so update it by hand when lib's changes.

## 2026-11-14 Dome-task alignment audit

- **Done 15 Nov 2026: log through the lab's loggers.** `gamma-ramp.csv` is written by OpenRigs.Logging's `CsvWriter` (columns `level`, `reading`). Was: written by Bonsai's own `CsvWriter` (dome-task alignment audit, 14 Nov 2026).
