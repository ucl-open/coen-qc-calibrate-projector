# Backlog

History is in `git log`.

## Open

- Rig check on one dome before relying on the lib window (`RawProjectorDisplay`, which replaced the hand-built `Display`; ProjectorWindowTests open a window, so CI skips them). Before updating, run Calibrate with the old window and keep `gamma-ramp.csv` and `gammalut.bmp`. After: black at Start, each level fills the image, Finished; the ramps agree within photodiode noise and the LUTs within 1-2 values; Stop mid-ramp closes the window and exits; `DisplayDevice=Sixth` stops at Start naming `RawProjectorDisplay`.
- Run it on a dome: it builds, and app-rig-control's fit is tested on synthetic ramps; the first run needs someone at the dome.
- Mesh mapping joins it here; until then it is done by hand with av-dome's `src\MeshMapping`.

## Decided

- The board and clock are lib's pieces, never a board opened by hand (Pip, 16 Nov 2026); no experiment folder (`NoExperimentFolder`); `PortName` is no launch property.
- Bonsai measures, app-rig-control computes: the workflow writes `gamma-ramp.csv` (OpenRigs.Logging `CsvWriter`) and nothing else.
