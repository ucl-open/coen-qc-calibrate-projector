# Backlog

## 2026-11-16 Onto the lab's device pieces (lib-open-rigs 5ea5729)

- Done: the board is lib's `BehaviorBoard` piece, with `ReadLabRig`, `ReadyGate` and the dome's clock `ClockFromTimestampGenerator` (Pip, 16 Nov 2026: QC workflows use the same pieces as the tasks, never a device opened by hand). `PathPrefix` declared and never published, so no piece logs. Replaces e647052's board opened by hand. The ramp, its timing, `gamma-ramp.csv` (`level`, `reading`) and the metrics are unchanged.
- Changed: `PortName` is no longer a launch property; the board's port comes from rig.yml. app-rig-control's `calibrations.toml` `[gamma.rig] PortName` must go in the same sitting, or the agent refuses the launch (an unknown launch property).
- Changed: the board's clock is now set (the generator's UTC clock, as in a session), and Ready also waits for it; a generator that does not answer stops the workflow, naming it.
- Done: the ramp's level reaches DrawQuad's colour through its own `Item1` (`shared_mapping_problems`). `tests\test_workflow.py` added, with pytest.
- Suggestion for lib: `hardware_problems` knows only the pieces' Ready stages; this workflow announces its own (`PhotodiodeReading`), so the test expects that one problem.

## 2026-11-15 Rig hardware as device pieces (lib-open-rigs)

- lib-open-rigs' `BehaviorBoard` include became a device piece that takes its port from rig.yml and logs into the experiment folder, which a calibration does not have (no `PathPrefix`, no `HarpClockSet`). The board is opened directly instead (`Hardware > BehaviorBoard`: Harp.Behavior's `Device` on `PortName`, behind `BehaviorBoard-Main.Commands` and `.Data`), so `PortName` stays rig-control's launch property and nothing on the rig or in rig-control changes. update-workflows had refused the packages here ("The specified property 'PortName' was not found in the nested workflow").

## Audit round 3 of 3 (15 Nov 2026)

- `scripts\deploy.ps1` exits 1 naming `.bonsai\Setup.ps1` when it is missing (a missing script left the last exit code, 0).

## Audit round 2 of 3 (15 Nov 2026, stage C)

- **Done 15 Nov 2026 (d1e6cc1): one Bonsai setup.** `.bonsai\Setup.ps1` runs lib-open-rigs' `setup-bonsai.ps1` from the open-rigs `uv.lock` pins, as every lab workflow does. Was: a hand copy of it, updated by hand.

## 2026-11-14 Dome-task alignment audit

- **Done 15 Nov 2026: log through the lab's loggers.** `gamma-ramp.csv` is written by OpenRigs.Logging's `CsvWriter` (columns `level`, `reading`). Was: written by Bonsai's own `CsvWriter` (dome-task alignment audit, 14 Nov 2026).
