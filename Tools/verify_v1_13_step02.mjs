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

function read(relative) {
  return fs.readFileSync(path.join(root, relative), "utf8");
}

function fail(message) {
  failures.push(message);
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
const levelById = new Map((levels.levels ?? []).map((level) => [level.levelId, level]));
const localizedKeys = new Set();

if (story.schemaVersion !== 2) fail("Story catalog schemaVersion must be 2.");
if (story.chapterId !== 1) fail("Story catalog chapterId must be 1.");
if (story.scenes?.length !== 8) fail("Story catalog must retain eight persistent scenes.");
if (story.milestones?.length !== 7) fail("Story catalog must contain seven milestone mappings.");
if (story.reactions?.length !== 20) fail("Story catalog must contain twenty outcome reactions.");
if (russian.schemaVersion !== 1 || russian.locale !== "ru")
  fail("Russian localization must use schemaVersion 1 and locale ru.");

for (const entry of russian.entries ?? []) {
  if (!entry.key || !entry.value) fail("Every localization entry requires key and value.");
  if (localizedKeys.has(entry.key)) fail(`Localization key ${entry.key} is duplicated.`);
  localizedKeys.add(entry.key);
}

const scenesById = new Map();
const scenesByTrigger = new Map();
for (const scene of story.scenes ?? []) {
  if (!scene.id) fail("Every story scene requires an id.");
  if (scenesById.has(scene.id)) fail(`Story scene ${scene.id} is duplicated.`);
  scenesById.set(scene.id, scene);
  if (scene.trigger) {
    if (scenesByTrigger.has(scene.trigger)) fail(`Story trigger ${scene.trigger} is duplicated.`);
    scenesByTrigger.set(scene.trigger, scene);
  }
  if (!new Set(["PreLevel", "ChapterMap", "ChapterComplete"]).has(scene.returnRoute))
    fail(`${scene.id} has invalid return route ${scene.returnRoute}.`);
  if (!(scene.lines?.length > 0)) fail(`${scene.id} has no lines.`);
  for (const line of scene.lines ?? []) {
    if (!localizedKeys.has(line.speakerKey)) fail(`${scene.id} misses ${line.speakerKey}.`);
    if (!localizedKeys.has(line.textKey)) fail(`${scene.id} misses ${line.textKey}.`);
  }
}

const mappedLevels = new Map();
const mappedSceneIds = new Set();
const milestoneIds = new Set();
let previousLevel = 0;
for (const milestone of story.milestones ?? []) {
  if (!milestone.id) fail("Every story milestone requires an id.");
  if (milestoneIds.has(milestone.id)) fail(`Milestone ${milestone.id} is duplicated.`);
  milestoneIds.add(milestone.id);
  if (milestone.afterLevelId <= previousLevel) fail("Milestones are not strictly ordered.");
  previousLevel = milestone.afterLevelId;
  if (mappedLevels.has(milestone.afterLevelId))
    fail(`Level ${milestone.afterLevelId} has multiple milestone mappings.`);
  mappedLevels.set(milestone.afterLevelId, milestone);
  if (mappedSceneIds.has(milestone.sceneId))
    fail(`Scene ${milestone.sceneId} has multiple milestone mappings.`);
  mappedSceneIds.add(milestone.sceneId);

  const scene = scenesById.get(milestone.sceneId);
  const level = levelById.get(milestone.afterLevelId);
  if (!scene) fail(`${milestone.id} references a missing scene.`);
  if (!level) fail(`${milestone.id} references a missing level.`);
  if (scene && level && !(level.storyTriggers ?? []).includes(scene.trigger))
    fail(`${milestone.id} scene trigger is not authored on its level.`);
}

if ([...mappedLevels.keys()].join(",") !== "1,3,4,6,8,9,10")
  fail("Post-victory mappings must be levels 1, 3, 4, 6, 8, 9, and 10.");
if ((story.milestones ?? []).filter((item) => item.major).map((item) => item.afterLevelId).join(",") !== "1,3,6,10")
  fail("Major milestones must be levels 1, 3, 6, and 10.");

const releasedDialogueTriggers = [];
for (const level of levels.levels ?? []) {
  for (const trigger of level.storyTriggers ?? []) {
    if (!trigger.startsWith("dialogue.")) continue;
    releasedDialogueTriggers.push(trigger);
    const scene = scenesByTrigger.get(trigger);
    const milestone = mappedLevels.get(level.levelId);
    if (!scene) fail(`Released trigger ${trigger} has no scene.`);
    if (!milestone || milestone.sceneId !== scene?.id)
      fail(`Released trigger ${trigger} has no matching level mapping.`);
  }
}
if (releasedDialogueTriggers.length !== 7) fail("Expected seven released dialogue triggers.");
for (const [trigger, scene] of scenesByTrigger) {
  if (trigger.startsWith("dialogue.") && !mappedSceneIds.has(scene.id))
    fail(`Dialogue scene ${scene.id} has no milestone mapping.`);
}

const reactionIds = new Set();
const reactionPairs = new Set();
for (const reaction of story.reactions ?? []) {
  if (!reaction.id) fail("Every reaction requires an id.");
  if (reactionIds.has(reaction.id)) fail(`Reaction id ${reaction.id} is duplicated.`);
  reactionIds.add(reaction.id);
  if (!levelById.has(reaction.levelId)) fail(`${reaction.id} references a missing level.`);
  if (!new Set(["victory", "failure"]).has(reaction.outcome))
    fail(`${reaction.id} has invalid outcome ${reaction.outcome}.`);
  const pair = `${reaction.levelId}:${reaction.outcome}`;
  if (reactionPairs.has(pair)) fail(`Reaction pair ${pair} is duplicated.`);
  reactionPairs.add(pair);
  if (!localizedKeys.has(reaction.speakerKey)) fail(`${reaction.id} misses ${reaction.speakerKey}.`);
  if (!localizedKeys.has(reaction.textKey)) fail(`${reaction.id} misses ${reaction.textKey}.`);
}
for (const level of levels.levels ?? []) {
  for (const outcome of ["victory", "failure"])
    if (!reactionPairs.has(`${level.levelId}:${outcome}`))
      fail(`Level ${level.levelId} misses its ${outcome} reaction.`);
}

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
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /schemaVersion = 2/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /class StoryMilestoneData/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /class StoryReactionData/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /class LocalizedStoryReaction/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /class StoryReactionOutcomes/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /FindMilestoneAfterLevel/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /ValidateMilestones/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /ValidateReactions/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /GetLocalizedReaction/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /static StorySceneData FindPostVictoryScene/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /ShouldShowDialogueAfterVictory/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /LevelProgressData progress = state\.GetLevelProgress/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /activeSequence != null && !activeSequence\.IsComplete/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /LevelData outcomeLevel = activeBoard\.CurrentLevel/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /ShowOutcome/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /BuildOutcomeReactionOverlay/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /background\.raycastTarget = false/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /if \(current == ScreenId\.Dialogue\)\s*return;/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /void OpenSelectedLevel\(\)[\s\S]*?StoryService\.TryBeginPostVictoryDialogue/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /has no sceneId/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /saveVersion = 5/],
  ["Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs", /Step13StoryMilestoneReactionSelfTest/],
  ["Assets/LegendsOfRus/Scripts/Editor/Step13StoryMilestoneReactionSelfTest.cs", /Run v1\.13 Step 2 Self-Tests/],
  ["Assets/LegendsOfRus/Tests/PlayMode/StoryMilestoneReactionPlayModeTests.cs", /CompletedMilestone_IsSuppressedWhileCompatibilityPredicateStaysPure/],
  ["MILESTONE_VERSION.txt", /v1\.13-step02-story-milestones-character-reactions/]
];
for (const [file, pattern] of invariants)
  if (!pattern.test(read(file))) fail(`${file} lacks invariant ${pattern}.`);
const routerSource = read("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
if ((routerSource.match(/LevelData outcomeLevel = activeBoard\.CurrentLevel/g)?.length ?? 0) !== 2)
  fail("ScreenRouter must capture the exact outcome level in both callbacks.");
const validatorSource = read("Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs");
if (/"README_V1_13_STEP01\.md"/.test(validatorSource))
  fail("ProjectValidator still sends a root README through AssetDatabase.");

const required = [
  "README_V1_13_STEP02.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_13_STEP02.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_13_STEP03.json",
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_13_STEP02.json",
  "Assets/LegendsOfRus/Documentation/V1_13_STEP02_CHANGED_FILES.txt",
  "Assets/LegendsOfRus/Documentation/V1_13_STEP02_STORY_MILESTONES_CHARACTER_REACTIONS.md",
  "Assets/LegendsOfRus/Scripts/Editor/Step13StoryMilestoneReactionSelfTest.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/StoryMilestoneReactionPlayModeTests.cs"
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
  "Assets/LegendsOfRus/Tests/PlayMode/StoryMilestoneReactionPlayModeTests.cs"
).match(testPattern)?.length ?? 0;
const selfTestSource = read("Assets/LegendsOfRus/Scripts/Editor/Step13StoryMilestoneReactionSelfTest.cs");
const selfTestChecks = (selfTestSource.match(/\bCheck\(/g)?.length ?? 1) - 1;

const result = {
  version: "v1.13-step02",
  all_project_json_parses: !failures.some((failure) => failure.startsWith("JSON ")),
  project_json_files: jsonFiles.length,
  story_schema_version: story.schemaVersion,
  persistent_story_scenes: story.scenes?.length ?? 0,
  post_victory_mappings: story.milestones?.length ?? 0,
  major_milestone_levels: (story.milestones ?? []).filter((item) => item.major).map((item) => item.afterLevelId),
  outcome_reactions: story.reactions?.length ?? 0,
  russian_localization_entries: russian.entries?.length ?? 0,
  csharp_structural_scan_files: csharpFiles.length,
  csharp_delimiters_balanced: !failures.some((failure) => failure.includes("delimiters")),
  preprocessor_directives_balanced: !failures.some((failure) => failure.includes("#if") || failure.includes("#endif")),
  step02_source_invariants_checked: invariants.length,
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_step02_targeted_tests: targetedTests,
  step02_editor_self_test_checks: selfTestChecks,
  project_png_files_compared: baseline ? pngFiles.length : 0,
  project_png_files_byte_identical_to_step01: baseline ? identicalPngs === pngFiles.length : null,
  required_v1_13_step02_files_present: required.every((file) => fs.existsSync(path.join(root, file))),
  failures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (failures.length > 0) process.exitCode = 1;
