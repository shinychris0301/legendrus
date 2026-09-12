import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { execFileSync } from "node:child_process";

const root = process.cwd();
const baselineFlag = process.argv.indexOf("--baseline");
const baseline = baselineFlag >= 0 ? path.resolve(process.argv[baselineFlag + 1]) : null;
const failures = [];

function files(glob) {
  const output = execFileSync("rg", ["--files", "-g", glob], {
    cwd: root,
    encoding: "utf8"
  }).trim();
  return output ? output.split(/\r?\n/).sort() : [];
}

function fail(message) {
  failures.push(message);
}

function read(relative) {
  return fs.readFileSync(path.join(root, relative), "utf8");
}

function sha256(file) {
  return crypto.createHash("sha256").update(fs.readFileSync(file)).digest("hex");
}

const jsonFiles = files("*.json");
for (const file of jsonFiles) {
  try {
    JSON.parse(read(file));
  } catch (error) {
    fail(`JSON ${file}: ${error.message}`);
  }
}

const story = JSON.parse(read("Assets/LegendsOfRus/Resources/Config/chapter01_story.json"));
const russian = JSON.parse(read("Assets/LegendsOfRus/Resources/Localization/story_ru.json"));
const levels = JSON.parse(read("Assets/LegendsOfRus/Resources/Config/chapter01_levels.json"));
const validRoutes = new Set(["PreLevel", "ChapterMap", "ChapterComplete"]);
const localization = new Map();
const sceneIds = new Set();
const triggers = new Set();

if (story.schemaVersion !== 1) fail("Story catalog schemaVersion must be 1.");
if (story.chapterId !== 1) fail("Story catalog chapterId must be 1.");
if (russian.schemaVersion !== 1 || russian.locale !== "ru")
  fail("Russian localization must use schemaVersion 1 and locale ru.");

for (const entry of russian.entries ?? []) {
  if (!entry.key || !entry.value) fail("Localization entries require key and value.");
  if (localization.has(entry.key)) fail(`Localization key ${entry.key} is duplicated.`);
  localization.set(entry.key, entry.value);
}

for (const scene of story.scenes ?? []) {
  if (!scene.id) fail("Every story scene requires an id.");
  if (sceneIds.has(scene.id)) fail(`Story scene ${scene.id} is duplicated.`);
  sceneIds.add(scene.id);
  if (scene.trigger) {
    if (triggers.has(scene.trigger)) fail(`Story trigger ${scene.trigger} is duplicated.`);
    triggers.add(scene.trigger);
  }
  if (!validRoutes.has(scene.returnRoute)) fail(`${scene.id} has an invalid return route.`);
  if (!(scene.lines?.length > 0)) fail(`${scene.id} has no story lines.`);
  for (const line of scene.lines ?? []) {
    if (!localization.has(line.speakerKey)) fail(`${scene.id} misses ${line.speakerKey}.`);
    if (!localization.has(line.textKey)) fail(`${scene.id} misses ${line.textKey}.`);
  }
}

if (!sceneIds.has(story.introSceneId)) fail("Intro scene id does not resolve.");
const intro = story.scenes.find((scene) => scene.id === story.introSceneId);
if (intro?.returnRoute !== "PreLevel") fail("Intro must return to PreLevel.");
if (intro?.lines?.length !== 3) fail("Intro must contain three lines.");
const finale = story.scenes.find((scene) => scene.trigger === "dialogue.chapter_finale");
if (finale?.returnRoute !== "ChapterComplete") fail("Finale must return to ChapterComplete.");

const releasedDialogueTriggers = [];
for (const level of levels.levels ?? []) {
  for (const trigger of level.storyTriggers ?? []) {
    if (!trigger.startsWith("dialogue.")) continue;
    releasedDialogueTriggers.push(trigger);
    if (!triggers.has(trigger)) fail(`Released trigger ${trigger} has no story scene.`);
  }
}
if (releasedDialogueTriggers.length !== 7) fail("Expected seven released dialogue triggers.");

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

const csharpFiles = files("*.cs");
for (const file of csharpFiles) {
  const source = read(file);
  const scrubbed = scrubCSharp(source);
  for (const [open, close] of [["{", "}"], ["(", ")"], ["[", "]"]]) {
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
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /saveVersion = 5/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /completedStoryScenes/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /CompleteStoryScene/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /class StorySequenceRuntime/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /dialogue trigger.*has no story scene/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /TryBeginChapterIntro/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /TryBeginPostVictoryDialogue/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /ShouldShowDialogueAfterVictory/],
  ["Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs", /reloaded\.saveVersion, Is\.EqualTo\(5\)/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /BuildStoryDialogueOverlay/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /StoryAdvanceResult\.PreLevel/],
  ["Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs", /StoryCatalog\.Validate/],
  ["Assets/LegendsOfRus/Scripts/Editor/Step13StoryDataSelfTest.cs", /Run v1\.13 Step 1 Self-Tests/],
  ["Assets/LegendsOfRus/Tests/PlayMode/StoryDataRuntimePlayModeTests.cs", /FirstLaunchIntro_CompletesToPreLevelAndDoesNotRepeat/]
];
for (const [file, pattern] of invariants) {
  if (!pattern.test(read(file))) fail(`${file} lacks invariant ${pattern}.`);
}
if (/reloaded\.saveVersion, Is\.EqualTo\(4\)/.test(
  read("Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs")
)) fail("Retained PlayMode coverage still expects stale save version 4.");

const required = [
  "README_V1_13_STEP01.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_13_STEP01.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_13_STEP02.json",
  "Assets/LegendsOfRus/Documentation/V1_13_STEP01_CHANGED_FILES.txt",
  "Assets/LegendsOfRus/Documentation/V1_13_STEP01_STORY_DATA_CHAPTER_INTRO.md",
  "Assets/LegendsOfRus/Resources/Config/chapter01_story.json",
  "Assets/LegendsOfRus/Resources/Localization/story_ru.json",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs",
  "Assets/LegendsOfRus/Scripts/Editor/Step13StoryDataSelfTest.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/StoryDataRuntimePlayModeTests.cs"
];
for (const file of required)
  if (!fs.existsSync(path.join(root, file))) fail(`Required file ${file} is missing.`);

const pngFiles = files("*.png");
let identicalPngs = 0;
if (baseline) {
  const baselinePngs = execFileSync("rg", ["--files", "-g", "*.png"], {
    cwd: baseline,
    encoding: "utf8"
  }).trim().split(/\r?\n/).filter(Boolean).sort();
  if (baselinePngs.length !== pngFiles.length) fail("PNG file count differs from baseline.");
  for (const file of pngFiles) {
    const current = path.join(root, file);
    const original = path.join(baseline, file);
    if (!fs.existsSync(original)) fail(`PNG ${file} is new relative to baseline.`);
    else if (sha256(current) !== sha256(original)) fail(`PNG ${file} differs from baseline.`);
    else identicalPngs++;
  }
}

const testPattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
let discoveredTests = 0;
for (const file of csharpFiles.filter((file) => file.includes("/Tests/")))
  discoveredTests += read(file).match(testPattern)?.length ?? 0;
const targetedTests = read(
  "Assets/LegendsOfRus/Tests/PlayMode/StoryDataRuntimePlayModeTests.cs"
).match(testPattern)?.length ?? 0;
const selfTestSource = read("Assets/LegendsOfRus/Scripts/Editor/Step13StoryDataSelfTest.cs");
const selfTestChecks = (selfTestSource.match(/\bCheck\(/g)?.length ?? 1) - 1;

const result = {
  version: "v1.13-step01",
  all_project_json_parses: !failures.some((failure) => failure.startsWith("JSON ")),
  project_json_files: jsonFiles.length,
  story_schema_version: story.schemaVersion,
  story_scenes: story.scenes?.length ?? 0,
  intro_lines: intro?.lines?.length ?? 0,
  russian_localization_entries: russian.entries?.length ?? 0,
  released_dialogue_triggers_covered: releasedDialogueTriggers.length,
  csharp_structural_scan_files: csharpFiles.length,
  csharp_delimiters_balanced: !failures.some((failure) => failure.includes("delimiters")),
  preprocessor_directives_balanced: !failures.some((failure) => failure.includes("#if") || failure.includes("#endif")),
  step01_source_invariants_checked: invariants.length,
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_step01_targeted_tests: targetedTests,
  step01_editor_self_test_checks: selfTestChecks,
  project_png_files_compared: baseline ? pngFiles.length : 0,
  project_png_files_byte_identical_to_step03: baseline ? identicalPngs === pngFiles.length : null,
  required_step01_files_present: required.every((file) => fs.existsSync(path.join(root, file))),
  failures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (failures.length > 0) process.exitCode = 1;
