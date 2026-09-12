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

function allFilesAt(directory) {
  const output = execFileSync("rg", ["--files"], {
    cwd: directory,
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

if (story.schemaVersion !== 3) fail("Story catalog schemaVersion must be 3.");
if (story.chapterId !== 1) fail("Story catalog chapterId must be 1.");
if (levels.chapterId !== story.chapterId)
  fail("Story and level catalogs must use the same chapterId.");
if (story.scenes?.length !== 8) fail("Story catalog must retain eight persistent scenes.");
if (story.milestones?.length !== 7) fail("Story catalog must contain seven milestone mappings.");
if (story.reactions?.length !== 20) fail("Story catalog must contain twenty outcome reactions.");
if (!story.completion) fail("Story catalog must contain Chapter Complete presentation data.");
if (russian.schemaVersion !== 1 || russian.locale !== "ru")
  fail("Russian localization must use schemaVersion 1 and locale ru.");
if (russian.entries?.length !== 49)
  fail("Russian story localization must contain 49 entries.");

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

const completion = story.completion ?? {};
const finalLevel = levelById.get(completion.finalLevelId);
const finaleScene = scenesById.get(completion.finaleSceneId);
const finalMilestone = mappedLevels.get(completion.finalLevelId);
if (completion.finalLevelId !== 10) fail("Chapter Complete finalLevelId must be 10.");
if (completion.finaleSceneId !== "chapter01.finale")
  fail("Chapter Complete finaleSceneId must be chapter01.finale.");
if (completion.completionTrigger !== "chapter.complete")
  fail("Chapter Complete trigger must be chapter.complete.");
if (!finalLevel) fail("Chapter Complete final level is missing.");
if (!(finalLevel?.storyTriggers ?? []).includes(completion.completionTrigger))
  fail("Final level lacks chapter.complete.");
if (!(finalLevel?.storyTriggers ?? []).includes(finaleScene?.trigger))
  fail("Final level lacks its finale dialogue trigger.");
if (!finaleScene || finaleScene.returnRoute !== "ChapterComplete")
  fail("Finale scene must return to ChapterComplete.");
if (finaleScene?.lines?.length !== 5) fail("Finale presentation must contain five lines.");
if (!finalMilestone || !finalMilestone.major || finalMilestone.sceneId !== completion.finaleSceneId)
  fail("Final level must map a major milestone to the finale scene.");
if ((levels.levels ?? []).filter((level) =>
  (level.storyTriggers ?? []).includes(completion.completionTrigger)).length !== 1)
  fail("Exactly one level must own chapter.complete.");
if ((story.scenes ?? []).filter((scene) => scene.returnRoute === "ChapterComplete").length !== 1)
  fail("Exactly one scene must route to ChapterComplete.");
for (const key of [completion.summaryTitleKey, completion.summaryFormatKey, completion.rewardNoteKey])
  if (!localizedKeys.has(key)) fail(`Chapter Complete misses localization key ${key}.`);
const summaryFormat = (russian.entries ?? []).find((entry) =>
  entry.key === completion.summaryFormatKey)?.value ?? "";
for (const placeholder of ["{0}", "{1}", "{2}", "{3}"])
  if (!summaryFormat.includes(placeholder))
    fail(`Chapter Complete summary format misses ${placeholder}.`);
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
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /schemaVersion = 3/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /class StoryChapterCompletionData/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /class LocalizedChapterCompletion/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /LocalizeCompletion/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /ValidateCompletion/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /terminal released level/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /Exactly one story scene must route to ChapterComplete/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs", /same chapterId/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /GetLocalizedReaction/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /GetLocalizedChapterCompletion/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /GetChapterCompletionSummary/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /ShouldShowDialogueAfterVictory/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /ShouldShowChapterComplete/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /AcknowledgeChapterComplete/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /activeSequence != null && !activeSequence\.IsComplete/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs", /Rejected ChapterComplete route/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /saveVersion = 6/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /acknowledgedChapterCompletions/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /Save\.saveVersion < 6/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /NormalizeChapterCompletionProgress/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /enum ChapterCompletionPhase/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /GetChapterCompletionPhase/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /ChapterCompletionPhase\.FinalePending/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /ChapterCompletionPhase\.PresentationPending/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /AcknowledgeChapterCompletion/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", /class ChapterCompletionSummary/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /TryShowPendingChapterFlow/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /void OpenChapterMap/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /ShouldShowChapterIntro/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /BuildChapterCompleteOverlay/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /ChapterCompletionSummary/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /ContinueAfterChapterComplete/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /if \(current == ScreenId\.ChapterComplete\)/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /LevelData outcomeLevel = activeBoard\.CurrentLevel/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /background\.raycastTarget = false/],
  ["Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs", /Step13ChapterFinaleCompletionSelfTest/],
  ["Assets/LegendsOfRus/Scripts/Editor/Step13ChapterFinaleCompletionSelfTest.cs", /Run v1\.13 Step 3 Self-Tests/],
  ["Assets/LegendsOfRus/Tests/PlayMode/ChapterFinaleCompletionPlayModeTests.cs", /FullFirstLaunchRoute_EndsAtAcknowledgedChapterComplete/],
  ["Assets/LegendsOfRus/Tests/PlayMode/ChapterFinaleCompletionPlayModeTests.cs", /ReplayedLevel10_DoesNotRepeatFinaleCompletionOrRewards/],
  ["MILESTONE_VERSION.txt", /v1\.13-step03-chapter-finale-completion-flow/]
];
for (const [file, pattern] of invariants)
  if (!pattern.test(read(file))) fail(`${file} lacks invariant ${pattern}.`);
const routerSource = read("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
if ((routerSource.match(/LevelData outcomeLevel = activeBoard\.CurrentLevel/g)?.length ?? 0) !== 2)
  fail("ScreenRouter must capture the exact outcome level in both callbacks.");
const validatorSource = read("Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs");
if (/"README_V1_13_STEP0[123]\.md"/.test(validatorSource) || /"Tools\//.test(validatorSource))
  fail("ProjectValidator sends a root or Tools path through AssetDatabase.");
if ((routerSource.match(/ContinueAfterChapterComplete\(ScreenId\.WorldMap\)/g)?.length ?? 0) < 3)
  fail("Chapter Complete buttons and Back must acknowledge before routing to World Map.");

const required = [
  "README_V1_13_STEP03.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_13_STEP03.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_14_STEP01.json",
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_13_STEP03.json",
  "Assets/LegendsOfRus/Documentation/V1_13_STEP03_CHANGED_FILES.txt",
  "Assets/LegendsOfRus/Documentation/V1_13_STEP03_CHAPTER_FINALE_COMPLETION_FLOW.md",
  "Assets/LegendsOfRus/Scripts/Editor/Step13ChapterFinaleCompletionSelfTest.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/ChapterFinaleCompletionPlayModeTests.cs"
];
for (const file of required)
  if (!fs.existsSync(path.join(root, file))) fail(`Required file ${file} is missing.`);

const pngFiles = files("*.png");
let identicalPngs = 0;
let immutableFilesCompared = 0;
let immutableFilesIdentical = null;
let changedManifestMatchesBaseline = null;
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

  const currentFiles = allFilesAt(root);
  const baselineFiles = allFilesAt(baseline);
  const immutableFiles = currentFiles.filter((file) =>
    file === "Assets/LegendsOfRus/Resources/Config/chapter01_levels.json" ||
    file.startsWith("Assets/LegendsOfRus/Scripts/GameplayCore/") ||
    file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Gameplay/")
  );
  immutableFilesIdentical = true;
  for (const file of immutableFiles) {
    immutableFilesCompared++;
    const original = path.join(baseline, file);
    if (!fs.existsSync(original) || sha256(path.join(root, file)) !== sha256(original)) {
      immutableFilesIdentical = false;
      fail(`Gameplay/balance boundary file ${file} differs from Step 2.`);
    }
  }

  const union = new Set([...currentFiles, ...baselineFiles]);
  const actualChanged = [...union].filter((file) => {
    const current = path.join(root, file);
    const original = path.join(baseline, file);
    if (!fs.existsSync(current) || !fs.existsSync(original)) return true;
    return sha256(current) !== sha256(original);
  }).sort();
  const manifestChanged = read(
    "Assets/LegendsOfRus/Documentation/V1_13_STEP03_CHANGED_FILES.txt"
  ).split(/\r?\n/).map((line) => line.trim()).filter((line) =>
    line && line !== "Added" && line !== "Modified"
  ).sort();
  changedManifestMatchesBaseline =
    actualChanged.length === manifestChanged.length &&
    actualChanged.every((file, index) => file === manifestChanged[index]);
  if (!changedManifestMatchesBaseline) {
    const missing = actualChanged.filter((file) => !manifestChanged.includes(file));
    const extra = manifestChanged.filter((file) => !actualChanged.includes(file));
    fail(`Changed-file manifest mismatch; missing=[${missing.join(",")}], extra=[${extra.join(",")}].`);
  }
}

const testPattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
let discoveredTests = 0;
for (const file of csharpFiles.filter((file) => file.includes("/Tests/")))
  discoveredTests += read(file).match(testPattern)?.length ?? 0;
const targetedTests = read(
  "Assets/LegendsOfRus/Tests/PlayMode/ChapterFinaleCompletionPlayModeTests.cs"
).match(testPattern)?.length ?? 0;
const selfTestSource = read("Assets/LegendsOfRus/Scripts/Editor/Step13ChapterFinaleCompletionSelfTest.cs");
const selfTestChecks = (selfTestSource.match(/\bCheck\(/g)?.length ?? 1) - 1;

const result = {
  version: "v1.13-step03",
  all_project_json_parses: !failures.some((failure) => failure.startsWith("JSON ")),
  project_json_files: jsonFiles.length,
  story_schema_version: story.schemaVersion,
  persistent_story_scenes: story.scenes?.length ?? 0,
  post_victory_mappings: story.milestones?.length ?? 0,
  major_milestone_levels: (story.milestones ?? []).filter((item) => item.major).map((item) => item.afterLevelId),
  outcome_reactions: story.reactions?.length ?? 0,
  chapter_final_level: completion.finalLevelId ?? 0,
  chapter_finale_scene: completion.finaleSceneId ?? "",
  chapter_finale_lines: finaleScene?.lines?.length ?? 0,
  russian_localization_entries: russian.entries?.length ?? 0,
  csharp_structural_scan_files: csharpFiles.length,
  csharp_delimiters_balanced: !failures.some((failure) => failure.includes("delimiters")),
  preprocessor_directives_balanced: !failures.some((failure) => failure.includes("#if") || failure.includes("#endif")),
  step03_source_invariants_checked: invariants.length,
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_step03_targeted_tests: targetedTests,
  step03_editor_self_test_checks: selfTestChecks,
  project_png_files_compared: baseline ? pngFiles.length : 0,
  project_png_files_byte_identical_to_step02: baseline ? identicalPngs === pngFiles.length : null,
  gameplay_and_level_balance_boundary_files_compared: immutableFilesCompared,
  gameplay_and_level_balance_boundary_files_byte_identical_to_step02: immutableFilesIdentical,
  changed_file_manifest_matches_step02_baseline: changedManifestMatchesBaseline,
  required_v1_13_step03_files_present: required.every((file) => fs.existsSync(path.join(root, file))),
  failures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (failures.length > 0) process.exitCode = 1;
