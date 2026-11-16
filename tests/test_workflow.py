"""src\\main.bonsai against the lab's rules for a workflow's hardware (open_rigs.rigs.workflow_lint): the lab's device
pieces, never a board opened by hand (Pip, 16 Nov 2026), and the ramp's output as before."""

from pathlib import Path

import pytest
from open_rigs.rigs import workflow_lint as lint

ROOT = Path(__file__).parents[1]
W, XSI = lint.W, lint.XSI


@pytest.fixture(scope="module")
def wf():
    return lint.Workflow(ROOT)


def test_the_hardware_is_the_lab_pieces(wf):
    # Ready also waits for the photodiode's first reading, the workflow's own stage (ReadyGate's WorkflowStages).
    assert lint.hardware_problems(wf) == []
    assert dict(lint.hardware(wf))[lint.READY_GATE]["WorkflowStages"] == "PhotodiodeReading"


def test_the_devices_it_drives(wf):
    assert lint.required_devices(wf) == [("TimestampGeneratorGen3-Main", "timestampGeneratorGen3"), ("BehaviorBoard-Main", "behaviorBoard")]


def test_no_board_opened_by_hand(wf):
    assert not [n for n in wf.root.iter(W + "Expression") if lint.kind(n).endswith(":Device")]


def test_no_shared_mapping(wf):
    # The ramp's level reached DrawQuad's colour through an Item1 that Format and Measure shared.
    assert lint.shared_mapping_problems(wf.root) == []


def test_every_announcer_never_completes(wf):
    assert lint.ready_announcer_problems(wf.root) == []


def test_every_command_is_declared(wf):
    assert lint.undeclared_commands(wf) == [] and lint.declared_twice(wf) == []


def test_no_experiment_folder(wf):
    # NoExperimentFolder's PathPrefix is never published, so no piece's log starts.
    assert lint.no_experiment_folder_problems(wf) == []


def test_no_launch_property_but_the_lab_ones(wf):
    # PortName came from rig-control until 16 Nov 2026; the board piece reads rig.yml's port itself.
    top = lint.nodes_and_edges(lint.child(wf.root, W + "Workflow"))[0]
    names = {p.get("DisplayName") or p.get("Name") for n in top if n.get(XSI) == "ExternalizedMapping" for p in n.iter(W + "Property")}
    assert names == {"ConfigRoot", "SubjectId", "SessionId", "DisplayDevice", "Levels"}


def test_the_ramp_file_keeps_its_columns(wf):
    assert "new(Item1 as level, Item2 as reading)" in wf.text
    assert "{0}\\calibration\\gamma-ramp.csv" in wf.text
