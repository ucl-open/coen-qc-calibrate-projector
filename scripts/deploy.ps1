# Installs Bonsai and the workflow's packages into .bonsai\. This project has no rig schema of its
# own: rig-control passes the board's port from the rig's <ConfigRoot>\<MACHINE-NAME>\rig.yml; the workflow reads no rig.yml
# itself and writes its artefacts into calibration\ beside it.
$repo = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
& (Join-Path $repo '.bonsai\Setup.ps1')
exit $LASTEXITCODE
