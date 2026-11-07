# coen-calibrate-projector

Calibrates a Coen Lab dome projector. Today it does the gamma: it shows a ramp of grey levels full screen and reads each one's brightness with a photodiode. rig-control fits the ramp into the lookup table that BonVision's `GammaCorrection` reads, where the rig's experiment workflows find it. Mesh mapping will join it here; until then it is done by hand with av-dome's `src\MeshMapping`.

Run it from rig-control's Calibrations page. It is an open-rigs project under `ExperimentControl`, which the page launches, watches and stops.

**Not yet run on a dome.** It builds, and rig-control's fit is tested on synthetic ramps; the first hardware run needs someone at the dome.

## Bonsai Measures, rig-control Computes

This workflow drives the display and the board and writes the raw ramp, nothing more. It has no C# of its own. The lookup, its record and its history are written by rig-control's Python (`calibrations.py`, `fit_run` and `write_gamma`), with tests, once the run has completed. The lab's rule (Pip, 7 Nov 2026) is that fits and derived files are tested Python, never C# in a workflow.

## What It Takes

- **A photodiode on the behavior board's ADC0**, facing the screen. The workflow reads `AnalogInput0` (1 kHz, ADC counts). Nothing else on the domes uses ADC0 today. The dome's sync-square photodiode is on a digital input (DIPort0), which says only light or dark, so it cannot measure brightness.
- **Launch properties, from rig-control**, read from the rig's own rig.yml (`<ConfigRoot>\<MACHINE-NAME>\rig.yml`, the file av-dome reads):
  - `PortName`: `behaviorBoard: main: portName`.
  - `DisplayDevice`: the display the projector is on, as av-dome is launched with it.
- **`Levels`** (default 80, the width of av-dome's lookup) and **`Period`** (default 0.5 s per level). Each level is read from 0.2 s to 0.45 s after it appears, so keep `Period` at 0.5 s or more.

## A Run

1. Put the photodiode on the screen and into the board's ADC0. Keep the room dark.
2. On the Calibrations page, press Projector Gamma's **Calibrate** and give your Lab PIN. The projector goes black: the window is full screen with no mesh mapping and no gamma correction, so the ramp drives the projector's raw levels.
3. **Ready** once the board has answered. Press **Start Calibration**.
4. After 1 s of black, the levels from black to white, one every `Period` (80 levels take about 41 s). The `level` metric is the one shown.
5. **Finished** once every level is written; the `points` metric says how many. rig-control then fits the ramp and the page shows the result: the levels, the gamma that fits the ramp (brightness as level to that power), and the dark and white readings. Or why nothing was written.

## What It Writes

The workflow writes `calibration\gamma-ramp.csv` beside rig.yml: columns `level` (0 to 1) and `reading` (the photodiode's mean, ADC counts), one row per level as it is read, replaced each run.

Once the run completes, rig-control writes, into the same `calibration\`, where rig.yml's `projectionCalibration: gammaLutPath` points:

- `gammalut.bmp`: one row, `Levels` pixels wide, 24-bit grey. Pixel *i* is the drive level that gives brightness *i* / (`Levels` - 1) of full white, above the dark screen. Each comes from the measured ramp, interpolated between the two levels around it, never falling, with black and white pinned to 0 and 1.
- `gammalut.json`: when, who (whoever launched it, by Lab PIN), the machine, and the ramp: each level, its reading and its drive.
- `history\`: the run's `gamma-ramp.csv` under its date (kept whether or not it could be fitted), a dated copy of the lookup and its record, and the lookup being replaced, under its own date, if it is not there already.

A ramp whose white is no brighter than its black is refused and no lookup is written: the photodiode is not facing the screen or not on ADC0. A run that is stopped is not fitted.

## Setup

`scripts\deploy.ps1` (or `.bonsai\Setup.ps1`) installs Bonsai and the packages. The lab's packages come from lib-open-rigs; never bump them here by hand.

Check a change without launching Bonsai: `C:\Github\Coen-Lab\lib-open-rigs\scripts\check-workflow-builds.ps1 -Repo <this repo>`.
