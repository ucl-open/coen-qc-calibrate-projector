# Backlog

History is in `git log`.

## Open

- Rig check on one dome before relying on the lib window (`RawProjectorDisplay`; ProjectorWindowTests open a window, so CI skips them). Before updating, run Calibrate with the old window and keep `Projector-GammaRamp.csv` and `Projector-Gamma.bmp`. After: black at Start, each level fills the image, Finished; the ramps agree within photodiode noise and the LUTs within 1-2 values; Stop mid-ramp closes the window and exits; `DisplayDevice=Sixth` stops at Start naming `RawProjectorDisplay`.
- Run it on a dome: it builds, and app-rig-control's fit is tested on synthetic ramps; the first run needs someone at the dome.
- Mesh mapping joins it here; until then it is done by hand with av-dome's `src\MeshMapping`.

## Decided

- The board and clock are lib's pieces, never a board opened by hand (Pip, 16 Nov 2026); no experiment folder (`NoExperimentFolder`); `PortName` is no launch property.
- Bonsai measures, app-rig-control computes: the workflow writes `Projector-GammaRamp.csv` (OpenRigs.Logging `CsvWriter`) and nothing else.

## Not recommended

- `rx:Average` error not named (`src\main.bonsai:278`): naming it needs a lib operator.

## Audit 2026-11-22 (/audit-rigs, pipeline)

Changed (7a0be73): CI runs `scripts\check-workflow.ps1` in place of the C# 5 and parse steps, and the header no longer calls the check private (P6S-L1).

Deferred:

- P3Q-I3: whether the workflow finishes needs a dome run (Finished should arrive about 41 s after Go).
- A manual Checks run of the new step.

Not changed, do not raise again without new information:

- None in this repo this round.
