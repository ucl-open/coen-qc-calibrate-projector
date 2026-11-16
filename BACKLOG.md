# Backlog

History is in `git log`.

## Open

- Run it on a dome: it builds, and app-rig-control's fit is tested on synthetic ramps; the first run needs someone at the dome.
- Mesh mapping joins it here; until then it is done by hand with av-dome's `src\MeshMapping`.
- Its window is its own (`Display`: CreateWindow, OrthographicView) because lib's `ProjectorDisplay` always applies mesh mapping and the gamma table, which a gamma calibration must not. A raw display in lib (the window, view and render-thread disposal without the two corrections) would let it use the lab's window and its start-up deadlock fix.

## Decided

- The board and clock are lib's pieces, never a board opened by hand (Pip, 16 Nov 2026); no experiment folder (`NoExperimentFolder`); `PortName` is no launch property.
- Bonsai measures, app-rig-control computes: the workflow writes `gamma-ramp.csv` (OpenRigs.Logging `CsvWriter`) and nothing else.
