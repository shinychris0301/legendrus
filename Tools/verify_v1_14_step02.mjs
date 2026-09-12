import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { execFileSync } from "node:child_process";

const root = process.cwd();
const baselineFlag = process.argv.indexOf("--baseline");
const baseline = baselineFlag >= 0 && process.argv[baselineFlag + 1]
  ? path.resolve(process.argv[baselineFlag + 1])
  : null;
const sourceOnly = process.argv.includes("--source-only");
const failures = [];

function fail(message) {
  failures.push(message);
}

if (!baseline && !sourceOnly) {
  fail(
    "A v1.14 Step 1 baseline is required for release verification. " +
    "Pass --baseline <project> or explicitly choose --source-only."
  );
}

function projectFilesAt(directory) {
  try {
    const output = execFileSync("rg", ["--files"], {
      cwd: directory,
      encoding: "utf8"
    }).trim();
    return output ? output.split(/\r?\n/).sort() : [];
  } catch {
    return [];
  }
}

function files(glob) {
  try {
    const output = execFileSync("rg", ["--files", "-g", glob], {
      cwd: root,
      encoding: "utf8"
    }).trim();
    return output ? output.split(/\r?\n/).sort() : [];
  } catch {
    return [];
  }
}

function read(relative, directory = root) {
  return fs.readFileSync(path.join(directory, relative), "utf8");
}

function sha256(file) {
  return crypto.createHash("sha256").update(fs.readFileSync(file)).digest("hex");
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
    } else if (char === "\\") {
      output += "  ";
      index++;
    } else if ((state === "string" && char === '"') || (state === "char" && char === "'")) {
      state = "code";
      output += " ";
    } else {
      output += char === "\n" ? "\n" : " ";
    }
  }
  return output;
}

function methodBody(source, marker) {
  const start = source.indexOf(marker);
  if (start < 0) return "";
  const open = source.indexOf("{", start);
  if (open < 0) return "";
  let depth = 0;
  for (let index = open; index < source.length; index++) {
    if (source[index] === "{") depth++;
    if (source[index] === "}" && --depth === 0)
      return source.slice(open + 1, index);
  }
  return "";
}

const required = [
  "README_V1_14_STEP02.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_14_STEP02.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_14_STEP03.json",
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_14_STEP02.json",
  "Assets/LegendsOfRus/Documentation/V1_14_STEP02_CHANGED_FILES.txt",
  "Assets/LegendsOfRus/Documentation/V1_14_STEP02_CORE_LOOP_VFX_BOOSTER_FEEDBACK.md",
  "Assets/LegendsOfRus/Scripts/Editor/Step14CoreLoopVfxBoosterFeedbackSelfTest.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxProfile.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs",
  "Tools/verify_v1_14_step02.mjs"
];

const expectedChanged = [
  ...required,
  "Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
  "Assets/LegendsOfRus/Documentation/README_FIRST.md",
  "Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs",
  "MILESTONE_VERSION.txt",
  "README.md"
].sort();

for (const file of required)
  if (!fs.existsSync(path.join(root, file))) fail(`Required file ${file} is missing.`);

const jsonFiles = files("*.json");
for (const file of jsonFiles) {
  try {
    JSON.parse(read(file));
  } catch (error) {
    fail(`JSON ${file}: ${error.message}`);
  }
}

const story = JSON.parse(read("Assets/LegendsOfRus/Resources/Config/chapter01_story.json"));
const levels = JSON.parse(read("Assets/LegendsOfRus/Resources/Config/chapter01_levels.json"));
const russian = JSON.parse(read("Assets/LegendsOfRus/Resources/Localization/story_ru.json"));
if (story.schemaVersion !== 3 || story.scenes?.length !== 8 || story.milestones?.length !== 7)
  fail("Chapter 1 story schema, scenes, or milestones changed.");
if (story.reactions?.length !== 20 || story.completion?.finalLevelId !== 10)
  fail("Chapter 1 reactions or completion contract changed.");
if (levels.schemaVersion !== 3 || levels.levels?.length !== 10)
  fail("Chapter 1 level schema or authored level count changed.");
if (russian.schemaVersion !== 1 || russian.locale !== "ru" || russian.entries?.length !== 49)
  fail("Russian story localization contract changed.");

const csharpFiles = files("*.cs");
for (const file of csharpFiles) {
  const source = read(file);
  const scrubbed = scrubCSharp(source);
  for (const [open, close] of [["{", "}"], ["(", ")"], ["[", "]"]]) {
    let depth = 0;
    for (const char of scrubbed) {
      if (char === open) depth++;
      else if (char === close) depth--;
      if (depth < 0) break;
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
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /BoosterActivated/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /BoosterCompleted/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /BoosterRejected/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /enum GameplayBoosterKind/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /BogatyrMaceRow/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /BogatyrMaceColumn/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /Firebird/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /LivingLight/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /SeaPearl/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /SpecialCombinationKind CombinationKind/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs", /GameplayBoosterKind BoosterKind/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /enum CoreLoopVfxCue/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /readonly struct CoreLoopVfxStyle/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /readonly struct CoreLoopVfxDiagnostics/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /readonly struct CoreLoopVfxTrace/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /readonly struct CoreLoopVfxRequest/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /static class CoreLoopVfxMapper/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /TryCreateRequest/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /SpecialCombinationKind/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /GameplayBoosterKind/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /BoosterMaceRow/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /BoosterMaceColumn/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /BoosterFirebird/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /BoosterLivingLight/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /BoosterSeaPearl/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs", /BoosterRejected/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxProfile.cs", /class CoreLoopVfxProfile/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxProfile.cs", /DefaultPoolCapacity/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxProfile.cs", /MaximumPoolCapacity/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxProfile.cs", /MaximumBurstEffectsPerCue/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxProfile.cs", /MaximumCascadePulses/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /class PooledUiVfxLayer/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /void Initialize/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /PrewarmedObjectCount/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /raycastTarget\s*=\s*false/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /blocksRaycasts\s*=\s*false/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /interactable\s*=\s*false/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /RectMask2D/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /Active(?:Effect)?Count/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /Created(?:Effect)?Count/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /Dropped(?:Effect)?Count/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs", /ResetEffects/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /class CoreLoopVfxPresenter/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /static CoreLoopVfxCue ResolveCue/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /MotionEventPublished\s*\+=/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /MotionEventPublished\s*\-=/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /CommandRecovered/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /OnDisable/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /StableSeed/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /MajorCombinationPulse/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /TickForTesting/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs", /void ClearAll/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs", /CoreLoopVfxPresenter/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs", /GameplayMotionKind\.BoosterActivated/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs", /GameplayMotionKind\.BoosterCompleted/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs", /GameplayMotionKind\.BoosterRejected/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs", /ToGameplayBooster/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs", /wave\.Combination/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs", /vfxPresenter\?\.ResetEffects/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /CoreLoopFeedbackCue\.BoosterActivated/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /CoreLoopFeedbackCue\.BoosterCompleted/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /CoreLoopFeedbackCue\.BoosterRejected/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /motion\.BoosterKind\s*!=\s*GameplayBoosterKind\.None/],
  ["Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs", /Step14CoreLoopVfxBoosterFeedbackSelfTest/],
  ["Assets/LegendsOfRus/Scripts/Editor/Step14CoreLoopVfxBoosterFeedbackSelfTest.cs", /Run v1\.14 Step 2 Self-Tests/],
  ["Assets/LegendsOfRus/Scripts/Editor/Step14CoreLoopVfxBoosterFeedbackSelfTest.cs", /CoreLoopVfxMapper\.TryCreateRequest/],
  ["Assets/LegendsOfRus/Scripts/Editor/Step14CoreLoopVfxBoosterFeedbackSelfTest.cs", /presenter exposes idempotent cleanup/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /GameplayBoosterKind/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /BoosterRejected/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /raycastTarget/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /PresentationTrace_IsDeterministicForIdenticalMotionSequence/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /FixedPool_IsPrewarmedAndBoundedUnderBurstLoad/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /PoolReuse_DoesNotCreateObjectsAfterRelease/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /ActiveVfx_DoesNotInterceptBoardCellRaycasts/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /AllFiveBoosterVariants_ActivateCompleteAndSpendExactlyOnce/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /InvalidBoosterTarget_RejectsWithoutSpendOrActivation/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /SeaPearl_GrantsFiveMovesWithoutBoardOrUnrelatedSaveMutation/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /Recovery_ClearsBoosterVfxSuppressesCompletionAndRefundsOnce/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /PresentationOnlyVfx_DoesNotMutateSaveOrFireStateChanged/],
  ["Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs", /PersistedSaveKey/],
  ["MILESTONE_VERSION.txt", /^v1\.14-step02-core-loop-vfx-booster-feedback\s*$/]
];

for (const [file, pattern] of invariants) {
  if (!fs.existsSync(path.join(root, file))) continue;
  if (!pattern.test(read(file))) fail(`${file} lacks invariant ${pattern}.`);
}

const boardSource = read("Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs");
for (const legacy of [
  "IEnumerator LineSweep(",
  "IEnumerator BoardFlash(",
  "IEnumerator PulseArea(",
  'new GameObject("LineSweep"',
  'new GameObject("BoardFlash"'
]) {
  if (boardSource.includes(legacy)) fail(`Legacy runtime VFX path remains: ${legacy}`);
}

if (fs.existsSync(path.join(root, "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs"))) {
  const poolSource = read("Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs");
  if (/\bInstantiate\s*\(/.test(poolSource))
    fail("PooledUiVfxLayer must not instantiate effects while playing.");
  const playBody = methodBody(poolSource, "bool Emit(");
  if (/new\s+GameObject\s*\(/.test(playBody) || /\bInstantiate\s*\(/.test(playBody))
    fail("The VFX play path allocates a runtime GameObject.");
  if (/\b(?:StartCoroutine|WaitForSeconds|WaitForSecondsRealtime)\b/.test(poolSource))
    fail("PooledUiVfxLayer must use its single Tick clock, not per-effect coroutines.");
}

if (fs.existsSync(path.join(root, "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs"))) {
  const presenterSource = read("Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs");
  if (/UnityEngine\.Random|\bRandom\.(?:value|Range)/.test(presenterSource))
    fail("Core-loop VFX placement must be deterministic and must not use Unity random state.");
  if (/\b(?:GameState|TryUseBooster|AddBooster|RestoreSnapshot|PlanBooster)\b/.test(presenterSource))
    fail("The VFX presenter references mutable gameplay, inventory, or planning state.");
  if (/\b(?:StartCoroutine|WaitForSeconds|WaitForSecondsRealtime)\b/.test(presenterSource))
    fail("CoreLoopVfxPresenter must use one Update clock, not per-effect coroutines.");
}

const feedbackSource = read("Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs");
if (/\b(?:AudioSource|AudioClip|Handheld|AndroidJava(?:Class|Object)?)\b/.test(feedbackSource))
  fail("Core-loop feedback must remain provider- and audio-module-neutral.");

const manifestLines = read("Assets/LegendsOfRus/Documentation/V1_14_STEP02_CHANGED_FILES.txt")
  .split(/\r?\n/).map((line) => line.trim()).filter((line) =>
    line && line !== "Added" && line !== "Modified"
  ).sort();
if (manifestLines.length !== expectedChanged.length ||
    !expectedChanged.every((file, index) => file === manifestLines[index])) {
  fail("Step 2 changed-file manifest does not match the 22-path release contract.");
}

let pngFilesCompared = 0;
let pngFilesIdentical = null;
let protectedFilesCompared = 0;
let protectedFilesIdentical = null;
let changedManifestMatchesBaseline = null;

if (baseline) {
  if (!fs.existsSync(baseline)) {
    fail(`Baseline directory does not exist: ${baseline}`);
  } else {
    const currentFiles = projectFilesAt(root);
    const baselineFiles = projectFilesAt(baseline);
    const currentPngs = currentFiles.filter((file) => file.endsWith(".png"));
    const baselinePngs = baselineFiles.filter((file) => file.endsWith(".png"));
    pngFilesCompared = currentPngs.length;
    pngFilesIdentical = currentPngs.length === baselinePngs.length;
    if (!pngFilesIdentical) fail("PNG file count differs from the v1.14 Step 1 baseline.");
    for (const file of currentPngs) {
      const current = path.join(root, file);
      const original = path.join(baseline, file);
      if (!fs.existsSync(original) || sha256(current) !== sha256(original)) {
        pngFilesIdentical = false;
        fail(`Approved PNG differs from v1.14 Step 1: ${file}`);
      }
    }

    const mutableGameplay = new Set([
      "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs",
      "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs",
      "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs",
      "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs",
      "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxProfile.cs",
      "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs"
    ]);
    const protectedFiles = currentFiles.filter((file) => {
      if (file.startsWith("Assets/LegendsOfRus/Scripts/GameplayCore/")) return true;
      if (file.startsWith("Assets/LegendsOfRus/Resources/Config/")) return true;
      if (file.startsWith("Assets/LegendsOfRus/Resources/Localization/")) return true;
      if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Gameplay/"))
        return !mutableGameplay.has(file);
      if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Services/"))
        return file !== "Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs";
      if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Core/")) return true;
      if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/UI/")) return true;
      return new Set([
        "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef",
        "Packages/manifest.json",
        "ProjectSettings/ProjectVersion.txt"
      ]).has(file);
    });
    protectedFilesCompared = protectedFiles.length;
    protectedFilesIdentical = protectedFiles.length === 49;
    if (protectedFiles.length !== 49)
      fail(`Expected 49 protected boundary files, found ${protectedFiles.length}.`);
    for (const file of protectedFiles) {
      const current = path.join(root, file);
      const original = path.join(baseline, file);
      if (!fs.existsSync(original) || sha256(current) !== sha256(original)) {
        protectedFilesIdentical = false;
        fail(`Protected boundary file differs from v1.14 Step 1: ${file}`);
      }
    }

    const union = new Set([...currentFiles, ...baselineFiles]);
    const actualChanged = [...union].filter((file) => {
      const current = path.join(root, file);
      const original = path.join(baseline, file);
      if (!fs.existsSync(current) || !fs.existsSync(original)) return true;
      return sha256(current) !== sha256(original);
    }).sort();
    changedManifestMatchesBaseline = actualChanged.length === manifestLines.length &&
      actualChanged.every((file, index) => file === manifestLines[index]);
    if (!changedManifestMatchesBaseline) {
      const missing = actualChanged.filter((file) => !manifestLines.includes(file));
      const extra = manifestLines.filter((file) => !actualChanged.includes(file));
      fail(`Changed-file manifest mismatch; missing=[${missing.join(",")}], extra=[${extra.join(",")}].`);
    }
  }
}

const testPattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
let discoveredTests = 0;
for (const file of csharpFiles.filter((file) => file.includes("/Tests/")))
  discoveredTests += read(file).match(testPattern)?.length ?? 0;
const step2TestFile = "Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs";
const targetedTests = fs.existsSync(path.join(root, step2TestFile))
  ? read(step2TestFile).match(testPattern)?.length ?? 0
  : 0;
const selfTestFile = "Assets/LegendsOfRus/Scripts/Editor/Step14CoreLoopVfxBoosterFeedbackSelfTest.cs";
const selfTestChecks = fs.existsSync(path.join(root, selfTestFile))
  ? (read(selfTestFile).match(/\bCheck\s*\(/g)?.length ?? 1) - 1
  : 0;

const result = {
  version: "v1.14-step02",
  all_project_json_parses: !failures.some((failure) => failure.startsWith("JSON ")),
  project_json_files: jsonFiles.length,
  story_schema_version: story.schemaVersion,
  persistent_story_scenes: story.scenes?.length ?? 0,
  post_victory_mappings: story.milestones?.length ?? 0,
  outcome_reactions: story.reactions?.length ?? 0,
  chapter_levels: levels.levels?.length ?? 0,
  russian_localization_entries: russian.entries?.length ?? 0,
  csharp_structural_scan_files: csharpFiles.length,
  csharp_delimiters_balanced: !failures.some((failure) => failure.includes("delimiters")),
  preprocessor_directives_balanced: !failures.some((failure) => failure.includes("#if") || failure.includes("#endif")),
  step02_source_invariants_checked: invariants.length,
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_step02_targeted_tests: targetedTests,
  step02_editor_self_test_checks: selfTestChecks,
  project_png_files_compared: baseline ? pngFilesCompared : 0,
  project_png_files_byte_identical_to_v1_14_step01: baseline ? pngFilesIdentical : null,
  protected_boundary_files_compared: baseline ? protectedFilesCompared : 0,
  protected_boundary_files_byte_identical_to_v1_14_step01: baseline ? protectedFilesIdentical : null,
  changed_file_manifest_matches_v1_14_step01_baseline: changedManifestMatchesBaseline,
  required_v1_14_step02_files_present: required.every((file) => fs.existsSync(path.join(root, file))),
  fixed_prewarmed_non_raycast_pool_contract: !failures.some((failure) => /pool|raycast|play path/i.test(failure)),
  no_runtime_vfx_gameobject_allocation_after_initialization: !failures.some((failure) => /allocates a runtime GameObject|instantiate effects/i.test(failure)),
  deterministic_vfx_placement_contract: !failures.some((failure) => /Unity random|placement must be deterministic/i.test(failure)),
  presentation_isolated_from_gameplay_mutation: !failures.some((failure) => /mutable gameplay|inventory|planning state/i.test(failure)),
  legacy_runtime_vfx_paths_removed: !failures.some((failure) => failure.startsWith("Legacy runtime VFX")),
  booster_lifecycle_contract_present: !failures.some((failure) => /Booster(?:Activated|Completed|Rejected)|GameplayBoosterKind/.test(failure)),
  feedback_service_provider_neutral: !/\b(?:AudioSource|AudioClip|Handheld|AndroidJava(?:Class|Object)?)\b/.test(feedbackSource),
  failures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (failures.length > 0) process.exitCode = 1;
