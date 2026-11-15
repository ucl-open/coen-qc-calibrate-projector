# Installs Bonsai and the workflow's packages into .bonsai\. This project has no rig schema of its
# own: rig-control passes the board's port from the rig's <ConfigRoot>\<MACHINE-NAME>\rig.yml; the workflow reads no rig.yml
# itself and writes its artefacts into calibration\ beside it.
$repo = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$setup = Join-Path $repo '.bonsai\Setup.ps1'
# A missing script is a non-terminating error and leaves $LASTEXITCODE as it was, so check first.
if (-not (Test-Path -Path $setup)) { Write-Error ".bonsai\Setup.ps1 was not found in $repo."; exit 1 }
& $setup
exit $LASTEXITCODE
