# Backlog

## 2026-11-15 Rig hardware as device pieces (lib-open-rigs)

- lib-open-rigs' `BehaviorBoard` include became a device piece that takes its port from rig.yml and logs into the experiment folder, which a calibration does not have (no `PathPrefix`, no `HarpClockSet`). The board is opened directly instead (`Hardware > BehaviorBoard`: Harp.Behavior's `Device` on `PortName`, behind `BehaviorBoard-Main.Commands` and `.Data`), so `PortName` stays rig-control's launch property and nothing on the rig or in rig-control changes. update-workflows had refused the packages here ("The specified property 'PortName' was not found in the nested workflow").

## Audit round 3 of 3 (15 Nov 2026)

- `scripts\deploy.ps1` exits 1 naming `.bonsai\Setup.ps1` when it is missing (a missing script left the last exit code, 0).

## Audit round 2 of 3 (15 Nov 2026, stage C)

- **Done 15 Nov 2026 (d1e6cc1): one Bonsai setup.** `.bonsai\Setup.ps1` runs lib-open-rigs' `setup-bonsai.ps1` from the open-rigs `uv.lock` pins, as every lab workflow does. Was: a hand copy of it, updated by hand.

## 2026-11-14 Dome-task alignment audit

- **Done 15 Nov 2026: log through the lab's loggers.** `gamma-ramp.csv` is written by OpenRigs.Logging's `CsvWriter` (columns `level`, `reading`). Was: written by Bonsai's own `CsvWriter` (dome-task alignment audit, 14 Nov 2026).
