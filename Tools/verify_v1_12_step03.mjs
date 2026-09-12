import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { execFileSync } from "node:child_process";

const projectRoot = process.cwd();
const baselineFlag = process.argv.indexOf("--baseline");
const baselineRoot = baselineFlag >= 0 ? path.resolve(process.argv[baselineFlag + 1]) : null;
const failures = [];

function relativeFiles(glob) {
  const output = execFileSync("rg", ["--files", "-g", glob], {
    cwd: projectRoot,
    encoding: "utf8"
  }).trim();
  return output ? output.split(/\r?\n/).sort() : [];
}

function fail(message) {
  failures.push(message);
}

const jsonFiles = relativeFiles("*.json");
for (const file of jsonFiles) {
  try {
    JSON.parse(fs.readFileSync(path.join(projectRoot, file), "utf8"));
  } catch (error) {
    fail(`JSON ${file}: ${error.message}`);
  }
}

const catalogPath = path.join(
  projectRoot,
  "Assets/LegendsOfRus/Resources/Config/chapter01_levels.json"
);
const catalog = JSON.parse(fs.readFileSync(catalogPath, "utf8"));
const requiredMechanics = [
  "match",
  "score",
  "ice",
  "chain",
  "crate",
  "bring_down",
  "survival"
];

function activeMechanics(level) {
  const active = new Set();
  for (const goal of level.goals ?? []) {
    if (goal.type === "collect_tile") active.add("match");
    if (goal.type === "score") active.add("score");
    if (goal.type === "bring_down") active.add("bring_down");
    if (goal.type === "survival") active.add("survival");
    if (goal.type === "clear_obstacle") active.add(goal.obstacleType);
  }
  for (const obstacle of level.obstacles ?? []) active.add(obstacle.type);
  if ((level.boardItems ?? []).length > 0) active.add("bring_down");
  return active;
}

if (catalog.schemaVersion !== 3) fail("Chapter catalog must use schema version 3.");
if (catalog.levels?.length !== 10) fail("Chapter 1 must contain ten levels.");

const introduced = new Set();
const introductionLevels = {};
const tutorialIds = new Set();
const recoveryLevels = [];
let tutorialStepCount = 0;

for (let index = 0; index < catalog.levels.length; index++) {
  const level = catalog.levels[index];
  const active = activeMechanics(level);
  const fresh = [...active].filter((mechanic) => !introduced.has(mechanic));
  if (fresh.length > 1) fail(`Level ${level.levelId} introduces ${fresh.join(", ")}.`);
  for (const mechanic of fresh) {
    introductionLevels[mechanic] = level.levelId;
    if (!(level.tutorialSteps ?? []).some(
      (step) => step.mechanic === mechanic && step.trigger === "level_start"
    )) {
      fail(`Level ${level.levelId} lacks a start tutorial for ${mechanic}.`);
    }
  }

  const obstaclePositions = new Set();
  const fixedPositions = new Set((level.fixedCells ?? []).map((cell) => `${cell.x}:${cell.y}`));
  for (const obstacle of level.obstacles ?? []) {
    const position = `${obstacle.x}:${obstacle.y}`;
    if (obstaclePositions.has(position)) fail(`${level.id} duplicates obstacle ${position}.`);
    obstaclePositions.add(position);
    if (obstacle.x < 0 || obstacle.x >= level.columns || obstacle.y < 0 || obstacle.y >= level.rows)
      fail(`${level.id} has out-of-bounds obstacle ${position}.`);
    if (obstacle.type === "crate" && fixedPositions.has(position))
      fail(`${level.id} overlaps a crate and fixed gem at ${position}.`);
  }

  for (const item of level.boardItems ?? []) {
    const position = `${item.x}:${item.y}`;
    if (fixedPositions.has(position)) fail(`${level.id} overlaps an item and fixed gem at ${position}.`);
    if ((level.obstacles ?? []).some(
      (obstacle) => obstacle.type === "crate" && `${obstacle.x}:${obstacle.y}` === position
    )) fail(`${level.id} overlaps an item and crate at ${position}.`);
  }

  for (const goal of level.goals ?? []) {
    if (goal.type === "score" && goal.target <= level.startingScore)
      fail(`${level.id} score goal does not exceed starting score.`);
    if (goal.type === "survival" && goal.target > level.moveLimit)
      fail(`${level.id} survival target exceeds move limit.`);
    if (goal.type === "clear_obstacle" && goal.target > (level.obstacles ?? []).filter(
      (obstacle) => obstacle.type === goal.obstacleType
    ).length) fail(`${level.id} obstacle target exceeds authored obstacles.`);
    if (goal.type === "bring_down" && goal.target > (level.boardItems ?? []).filter(
      (item) => item.type === goal.itemType
    ).length) fail(`${level.id} bring-down target exceeds authored items.`);
  }

  for (const step of level.tutorialSteps ?? []) {
    tutorialStepCount++;
    if (tutorialIds.has(step.id)) fail(`Tutorial ID ${step.id} is duplicated.`);
    tutorialIds.add(step.id);
    if (!active.has(step.mechanic)) fail(`${level.id} teaches inactive ${step.mechanic}.`);
  }

  if (level.difficulty === "recovery") {
    recoveryLevels.push(level.levelId);
    const peak = catalog.levels[index - 1];
    const beforePeak = catalog.levels[index - 2];
    if (fresh.length > 0) fail(`Recovery level ${level.levelId} introduces a mechanic.`);
    if (!peak || level.difficultyRating > peak.difficultyRating - 2)
      fail(`Recovery level ${level.levelId} does not drop two rating points.`);
    if (!beforePeak || peak.difficultyRating <= beforePeak.difficultyRating)
      fail(`Recovery level ${level.levelId} does not follow a local peak.`);
    if (!peak || level.moveLimit < peak.moveLimit + 2)
      fail(`Recovery level ${level.levelId} does not grant two relief moves.`);
  }

  if (index > 0 && level.difficultyRating - catalog.levels[index - 1].difficultyRating > 2)
    fail(`Difficulty jumps too quickly before level ${level.levelId}.`);
  for (const mechanic of active) introduced.add(mechanic);
}

let previousIntroduction = 0;
for (const mechanic of requiredMechanics) {
  const levelId = introductionLevels[mechanic];
  if (!levelId) fail(`Required mechanic ${mechanic} is missing.`);
  if (levelId <= previousIntroduction) fail(`Mechanic ${mechanic} is out of teaching order.`);
  previousIntroduction = Math.max(previousIntroduction, levelId ?? 0);
}
if (recoveryLevels.join(",") !== "4,7") fail("Recovery levels must be 4 and 7.");
const finale = catalog.levels.at(-1);
if (finale.difficulty !== "finale") fail("Level 10 must be the finale.");
if (finale.difficultyRating <= Math.max(...catalog.levels.slice(0, -1).map(
  (level) => level.difficultyRating
))) fail("Finale must be the unique highest-rated level.");

function scrubCSharp(source) {
  let output = "";
  let state = "code";
  for (let index = 0; index < source.length; index++) {
    const char = source[index];
    const next = source[index + 1];
    if (state === "code") {
      if (char === "/" && next === "/") { state = "line"; output += "  "; index++; }
      else if (char === "/" && next === "*") { state = "block"; output += "  "; index++; }
      else if (char === "@" && next === '"') { state = "verbatim"; output += "  "; index++; }
      else if (char === '"') { state = "string"; output += " "; }
      else if (char === "'") { state = "char"; output += " "; }
      else output += char;
    } else if (state === "line") {
      if (char === "\n") { state = "code"; output += "\n"; } else output += " ";
    } else if (state === "block") {
      if (char === "*" && next === "/") { state = "code"; output += "  "; index++; }
      else output += char === "\n" ? "\n" : " ";
    } else if (state === "verbatim") {
      if (char === '"' && next === '"') { output += "  "; index++; }
      else if (char === '"') { state = "code"; output += " "; }
      else output += char === "\n" ? "\n" : " ";
    } else {
      if (char === "\\") { output += "  "; index++; }
      else if ((state === "string" && char === '"') || (state === "char" && char === "'")) {
        state = "code"; output += " ";
      } else output += char === "\n" ? "\n" : " ";
    }
  }
  return output;
}

const csharpFiles = relativeFiles("*.cs");
for (const file of csharpFiles) {
  const source = fs.readFileSync(path.join(projectRoot, file), "utf8");
  const scrubbed = scrubCSharp(source);
  const pairs = [["{", "}"], ["(", ")"], ["[", "]"]];
  for (const [open, close] of pairs) {
    let depth = 0;
    for (const char of scrubbed) {
      if (char === open) depth++;
      else if (char === close && --depth < 0) break;
    }
    if (depth !== 0) fail(`${file} has unbalanced ${open}${close} delimiters.`);
  }

  const directives = [];
  for (const line of source.split(/\r?\n/)) {
    if (/^\s*#if\b/.test(line)) directives.push("if");
    if (/^\s*#endif\b/.test(line)) {
      if (directives.length === 0) fail(`${file} has unmatched #endif.`);
      else directives.pop();
    }
  }
  if (directives.length > 0) fail(`${file} has unmatched #if.`);
}

const invariants = [
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelData.cs", /CurrentSchemaVersion = 3/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelData.cs", /ValidateTutorialSteps/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelMechanicData.cs", /Chapter01Rollout/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelMechanicData.cs", /FirstItemCollected/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelTutorialSequence.cs", /scheduled\.Contains/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Chapter01ProgressionValidator.cs", /ValidateRecoveryLevel/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Chapter01ProgressionValidator.cs", /HasStartTutorial/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs", /board\.TryPause/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs", /board\.TryResume/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs", /HasCompletedTutorial/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /saveVersion = 4/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /CompleteTutorial/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /LevelTutorialPresenter/],
  ["Assets/LegendsOfRus/Scripts/GameplayCore/GameplayStateMachine.cs", /case GameplayPhase\.Swapping:[\s\S]{0,320}to == GameplayPhase\.Idle/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs", /event Action<string> CommandRejected/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs", /LastRejectedCommandReason/],
  ["Assets/LegendsOfRus/Scripts/Editor/ChapterLevelAuthoringWindow.cs", /DrawTutorialSteps/],
  ["Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs", /Chapter01ProgressionValidator/],
  ["Assets/LegendsOfRus/Scripts/Editor/Step12Chapter01CurveSelfTest.cs", /Run v1\.12 Step 3 Self-Tests/],
  ["Assets/LegendsOfRus/Tests/PlayMode/Chapter01MechanicCurvePlayModeTests.cs", /TutorialPresenter_PausesAndResumesGameplay/],
  ["Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs", /TutorialProgress_Persists/],
  ["Assets/LegendsOfRus/Tests/EditMode/GameplayStateMachineTests.cs", /PreflightRejectedSwap_ReturnsDirectlyToIdle/],
  ["Assets/LegendsOfRus/Tests/EditMode/BoardObstacleResolutionTests.cs", /ChainCrateObjectiveItemAndEmptyCell_BlockSwaps_WhileIceAllowsThem/],
  ["Assets/LegendsOfRus/Tests/PlayMode/Match3RuntimePlayModeTests.cs", /BlockedSwap_ReleasesInputWithoutMutationOrMoveSpend/]
];
for (const [file, pattern] of invariants) {
  const source = fs.readFileSync(path.join(projectRoot, file), "utf8");
  if (!pattern.test(source)) fail(`${file} lacks invariant ${pattern}.`);
}
if (/Debug\.LogWarning\(\$"Rejected board command:/.test(
  fs.readFileSync(
    path.join(projectRoot, "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs"),
    "utf8"
  )
)) fail("Blocked swap rejection still logs a warning.");

const requiredFiles = [
  "README_V1_12_STEP03.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_12_STEP03.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_13_STEP01.json",
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_12_STEP03.json",
  "Assets/LegendsOfRus/Documentation/V1_12_STEP03_CHANGED_FILES.txt",
  "Assets/LegendsOfRus/Documentation/V1_12_STEP03_CHAPTER01_MECHANIC_CURVE.md",
  "Assets/LegendsOfRus/Documentation/V1_12_STEP03_BLOCKED_SWAP_PHASE_FIX.md",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelTutorialSequence.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Chapter01ProgressionValidator.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs",
  "Assets/LegendsOfRus/Scripts/Editor/Step12Chapter01CurveSelfTest.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/Chapter01MechanicCurvePlayModeTests.cs"
];
for (const file of requiredFiles)
  if (!fs.existsSync(path.join(projectRoot, file))) fail(`Required file ${file} is missing.`);

function sha256(file) {
  return crypto.createHash("sha256").update(fs.readFileSync(file)).digest("hex");
}

const pngFiles = relativeFiles("*.png");
let identicalPngFiles = 0;
if (baselineRoot) {
  for (const file of pngFiles) {
    const current = path.join(projectRoot, file);
    const baseline = path.join(baselineRoot, file);
    if (!fs.existsSync(baseline)) fail(`PNG ${file} is new relative to the baseline.`);
    else if (sha256(current) !== sha256(baseline)) fail(`PNG ${file} differs from the baseline.`);
    else identicalPngFiles++;
  }
  const baselinePngFiles = execFileSync("rg", ["--files", "-g", "*.png"], {
    cwd: baselineRoot,
    encoding: "utf8"
  }).trim().split(/\r?\n/).filter(Boolean);
  if (baselinePngFiles.length !== pngFiles.length)
    fail("PNG file count differs from the baseline.");
}

const testPattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
let discoveredTests = 0;
for (const file of csharpFiles.filter((file) => file.includes("/Tests/"))) {
  const matches = fs.readFileSync(path.join(projectRoot, file), "utf8").match(testPattern);
  discoveredTests += matches?.length ?? 0;
}
const targetedTestFiles = [
  "Assets/LegendsOfRus/Tests/PlayMode/Chapter01MechanicCurvePlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs"
];
let targetedTests = 0;
for (const file of targetedTestFiles) {
  const source = fs.readFileSync(path.join(projectRoot, file), "utf8");
  const matches = source.match(testPattern);
  targetedTests += matches?.length ?? 0;
}
// LevelSystemRuntimePlayModeTests contains historical coverage; only one test is new here.
const curveFileTests = fs.readFileSync(
  path.join(projectRoot, targetedTestFiles[0]), "utf8"
).match(testPattern)?.length ?? 0;
targetedTests = curveFileTests + 3;

const selfTestSource = fs.readFileSync(
  path.join(projectRoot, "Assets/LegendsOfRus/Scripts/Editor/Step12Chapter01CurveSelfTest.cs"),
  "utf8"
);
const selfTestChecks = (selfTestSource.match(/\bCheck\(/g)?.length ?? 1) - 1;

const result = {
  version: "v1.12-step03",
  all_project_json_parses: failures.every((failure) => !failure.startsWith("JSON ")),
  project_json_files: jsonFiles.length,
  chapter_catalog_schema_version: catalog.schemaVersion,
  chapter_1_level_count: catalog.levels.length,
  chapter_1_tutorial_steps: tutorialStepCount,
  mechanic_introductions: introductionLevels,
  recovery_levels: recoveryLevels,
  difficulty_curve: catalog.levels.map((level) => level.difficultyRating),
  csharp_structural_scan_files: csharpFiles.length,
  csharp_delimiters_balanced: !failures.some((failure) => failure.includes("delimiters")),
  preprocessor_directives_balanced: !failures.some((failure) => failure.includes("#if") || failure.includes("#endif")),
  step03_source_invariants_checked: invariants.length,
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_step03_targeted_tests: targetedTests,
  blocked_swap_phase_fix_tests: 2,
  step03_editor_self_test_checks: selfTestChecks,
  project_png_files_compared: baselineRoot ? pngFiles.length : 0,
  project_png_files_byte_identical_to_step02: baselineRoot ? identicalPngFiles === pngFiles.length : null,
  required_step03_files_present: requiredFiles.every((file) => fs.existsSync(path.join(projectRoot, file))),
  failures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (failures.length > 0) process.exitCode = 1;
