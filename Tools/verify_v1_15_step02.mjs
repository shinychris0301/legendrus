import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { execFileSync, spawnSync } from "node:child_process";

const VERSION = "v1.15-step02";
const UNITY_VERSION = "6000.5.9f1";
const CATALOG_ID = "v1.15-step01-proposal";
const CATALOG_SHA256 =
  "ba66d7104c3d0a33ea4e0575b9965325cf9f90a8177c4e089c638d202267c42c";
const CORRECTED_STEP01_VALIDATOR_SHA256 =
  "71baa2343f0f3041795135d3ce1123d317b70ce30d646ecbee80f81a9f54e608";
const ACTIVATION_MANIFEST_ID = "v1.15-step02-runtime-economy-activation";
const STEP02_MILESTONE_ID =
  "v1.15-step02-runtime-economy-transactions-energy-integration";
const STEP01_MILESTONE_ID =
  "v1.15-step01-economy-configuration-simulation-foundation";

const root = process.cwd();
const baselineFlag = process.argv.indexOf("--baseline");
const baselineValue = baselineFlag >= 0 ? process.argv[baselineFlag + 1] : null;
const baselineArgumentValid = baselineFlag < 0 ||
  (typeof baselineValue === "string" && !baselineValue.startsWith("--"));
const baseline = baselineArgumentValid && baselineValue
  ? path.resolve(baselineValue)
  : null;
const sourceOnly = process.argv.includes("--source-only");
const failures = [];

function fail(message) {
  failures.push(message);
}

if (!baselineArgumentValid)
  fail("--baseline requires a directory argument that is not another option.");
if (baseline && sourceOnly)
  fail("Choose either --baseline <project> or --source-only, not both.");

if (!baseline && !sourceOnly) {
  fail(
    "A corrected v1.15 Step 1 baseline is required for release verification. " +
    "Pass --baseline <project> or explicitly choose --source-only."
  );
}

function read(relative, directory = root) {
  return fs.readFileSync(path.join(directory, relative), "utf8");
}

function exists(relative, directory = root) {
  return fs.existsSync(path.join(directory, relative));
}

function sha256File(relative, directory = root) {
  return crypto
    .createHash("sha256")
    .update(fs.readFileSync(path.join(directory, relative)))
    .digest("hex");
}

function projectFilesAt(directory) {
  try {
    const output = execFileSync("rg", ["--files"], {
      cwd: directory,
      encoding: "utf8",
      stdio: ["ignore", "pipe", "pipe"]
    }).trim();
    return output
      ? output.split(/\r?\n/).map((file) => file.replace(/\\/g, "/")).sort()
      : [];
  } catch (error) {
    fail(`Unable to enumerate files under ${directory}: ${error.message}`);
    return [];
  }
}

function files(glob) {
  try {
    const output = execFileSync("rg", ["--files", "-g", glob], {
      cwd: root,
      encoding: "utf8",
      stdio: ["ignore", "pipe", "pipe"]
    }).trim();
    return output
      ? output.split(/\r?\n/).map((file) => file.replace(/\\/g, "/")).sort()
      : [];
  } catch (error) {
    fail(`Unable to enumerate '${glob}' with rg: ${error.message}`);
    return [];
  }
}

function hasOwn(value, key) {
  return Object.prototype.hasOwnProperty.call(value, key);
}

function escapeRegExp(value) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function parseJson(relative, directory = root, recordFailure = true) {
  try {
    return JSON.parse(read(relative, directory));
  } catch (error) {
    if (recordFailure) fail(`JSON ${relative}: ${error.message}`);
    return null;
  }
}

function isPlainObject(value) {
  return value !== null && typeof value === "object" && !Array.isArray(value);
}

function exactKeys(value, expected, pointer) {
  if (!isPlainObject(value)) {
    fail(`${pointer} must be an object.`);
    return false;
  }
  const actual = Object.keys(value).sort();
  const wanted = [...expected].sort();
  const unknown = actual.filter((key) => !wanted.includes(key));
  const missing = wanted.filter((key) => !actual.includes(key));
  if (unknown.length > 0)
    fail(`${pointer} has unknown key(s): ${unknown.join(", ")}.`);
  if (missing.length > 0)
    fail(`${pointer} is missing key(s): ${missing.join(", ")}.`);
  return unknown.length === 0 && missing.length === 0;
}

function expectType(value, type, pointer) {
  if (typeof value !== type)
    fail(`${pointer} must be a ${type}.`);
}

function scrubCSharp(source) {
  let output = "";
  let state = "code";
  for (let index = 0; index < source.length; index++) {
    const char = source[index];
    const next = source[index + 1];
    if (state === "code") {
      if (char === "/" && next === "/") {
        state = "line";
        output += "  ";
        index++;
      } else if (char === "/" && next === "*") {
        state = "block";
        output += "  ";
        index++;
      } else if (char === "@" && next === '"') {
        state = "verbatim";
        output += "  ";
        index++;
      } else if (char === '"') {
        state = "string";
        output += " ";
      } else if (char === "'") {
        state = "char";
        output += " ";
      } else {
        output += char;
      }
    } else if (state === "line") {
      if (char === "\n") {
        state = "code";
        output += "\n";
      } else {
        output += " ";
      }
    } else if (state === "block") {
      if (char === "*" && next === "/") {
        state = "code";
        output += "  ";
        index++;
      } else {
        output += char === "\n" ? "\n" : " ";
      }
    } else if (state === "verbatim") {
      if (char === '"' && next === '"') {
        output += "  ";
        index++;
      } else if (char === '"') {
        state = "code";
        output += " ";
      } else {
        output += char === "\n" ? "\n" : " ";
      }
    } else if (char === "\\") {
      output += "  ";
      index++;
    } else if (
      (state === "string" && char === '"') ||
      (state === "char" && char === "'")
    ) {
      state = "code";
      output += " ";
    } else {
      output += char === "\n" ? "\n" : " ";
    }
  }
  return output;
}

const projectVersionPath = "ProjectSettings/ProjectVersion.txt";
const milestoneVersionPath = "MILESTONE_VERSION.txt";
const packageManifestPath = "Packages/manifest.json";
const economyPath = "Assets/LegendsOfRus/Resources/Config/economy.json";
const activationPath =
  "Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json";
const gameStatePath = "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs";
const saveServicePath = "Assets/LegendsOfRus/Scripts/Runtime/Services/SaveService.cs";
const catalogValidatorPath =
  "Assets/LegendsOfRus/Scripts/EconomyCore/EconomyCatalogValidator.cs";
const coordinatorPath =
  "Assets/LegendsOfRus/Scripts/RuntimeEconomy/DetachedRuntimeEconomyCoordinator.cs";
const gatePath =
  "Assets/LegendsOfRus/Scripts/RuntimeEconomy/RuntimeEconomyActivationGate.cs";
const engineTestPath =
  "Assets/LegendsOfRus/Tests/EconomyEditMode/RuntimeEconomyTransactionEngineEditModeTests.cs";
const coordinatorTestPath =
  "Assets/LegendsOfRus/Tests/PlayMode/DetachedRuntimeEconomyCoordinatorPlayModeTests.cs";
const step02SelfTestPath =
  "Assets/LegendsOfRus/Scripts/Editor/Step15RuntimeEconomyTransactionsEnergySelfTest.cs";
const changedManifestPath =
  "Assets/LegendsOfRus/Documentation/V1_15_STEP02_CHANGED_FILES.txt";
const staticReportPath =
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_15_STEP02.json";

const required = [
  "README_V1_15_STEP02.md",
  milestoneVersionPath,
  projectVersionPath,
  packageManifestPath,
  economyPath,
  activationPath,
  "Assets/LegendsOfRus/Scripts/EconomyCore/RuntimeEconomyTransactionEngine.cs",
  "Assets/LegendsOfRus/Scripts/RuntimeEconomy/RuntimeEconomyActivationManifestData.cs",
  gatePath,
  "Assets/LegendsOfRus/Scripts/RuntimeEconomy/RuntimeEconomyPolicy.cs",
  coordinatorPath,
  "Assets/LegendsOfRus/Scripts/RuntimeEconomy/LegendsOfRus.RuntimeEconomy.asmdef",
  engineTestPath,
  coordinatorTestPath,
  step02SelfTestPath,
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_15_STEP02.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_15_STEP02_ACTIVATION.json",
  "Assets/LegendsOfRus/Documentation/V1_15_STEP02_RUNTIME_ECONOMY_TRANSACTIONS_ENERGY_INTEGRATION.md",
  "Tools/verify_v1_15_step01.mjs",
  "Tools/verify_v1_15_step02.mjs"
];

for (const file of required)
  if (!exists(file)) fail(`Required Step 2 artifact ${file} is missing.`);
if (exists(milestoneVersionPath) && read(milestoneVersionPath).trim() !== STEP02_MILESTONE_ID)
  fail(`${milestoneVersionPath} must equal '${STEP02_MILESTONE_ID}'.`);

let step01Verification = null;
let step01VerificationPassed = false;
const step01VerifierPath = path.join(root, "Tools/verify_v1_15_step01.mjs");
if (fs.existsSync(step01VerifierPath)) {
  const execution = spawnSync(
    process.execPath,
    [step01VerifierPath, "--source-only"],
    { cwd: root, encoding: "utf8", stdio: ["ignore", "pipe", "pipe"] }
  );
  try {
    step01Verification = JSON.parse(execution.stdout || "null");
  } catch (error) {
    fail(`Step 1 verifier did not emit valid JSON: ${error.message}`);
  }
  if (execution.error) fail(`Step 1 verifier could not run: ${execution.error.message}`);
  step01VerificationPassed =
    !execution.error &&
    execution.status === 0 &&
    step01Verification?.version === "v1.15-step01" &&
    Array.isArray(step01Verification?.failures) &&
    step01Verification.failures.length === 0;
  if (!step01VerificationPassed) {
    const inherited = Array.isArray(step01Verification?.failures)
      ? step01Verification.failures.join(" | ")
      : (execution.stderr || "unknown failure").trim();
    fail(`Step 1 source verification failed: ${inherited}`);
  }
  if (step01Verification && step01Verification.version !== "v1.15-step01")
    fail("Step 1 verifier returned the wrong version contract.");
}

const jsonFiles = [...new Set([
  ...files("*.json"),
  ...required.filter((file) => file.endsWith(".json") && exists(file)),
  ...(exists(staticReportPath) ? [staticReportPath] : [])
])].sort();
const parsedJson = new Map();
for (const file of jsonFiles) parsedJson.set(file, parseJson(file));
const playModeAsmdefPath =
  "Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef";
const asmdefFiles = [...new Set([
  ...files("*.asmdef"),
  ...required.filter((file) => file.endsWith(".asmdef") && exists(file)),
  ...(exists(playModeAsmdefPath) ? [playModeAsmdefPath] : [])
])].sort();
const parsedAsmdefs = new Map();
for (const file of asmdefFiles) parsedAsmdefs.set(file, parseJson(file));

const projectVersion = exists(projectVersionPath) ? read(projectVersionPath).trim() : "";
const exactProjectVersion = projectVersion === `m_EditorVersion: ${UNITY_VERSION}`;
if (!exactProjectVersion)
  fail(`${projectVersionPath} must contain exactly 'm_EditorVersion: ${UNITY_VERSION}'.`);

const economy = parsedJson.get(economyPath);
let economyHash = null;
if (exists(economyPath)) {
  economyHash = sha256File(economyPath);
  if (economyHash !== CATALOG_SHA256)
    fail(`${economyPath} SHA-256 changed: ${economyHash}.`);
}
if (economy) {
  if (economy.catalogId !== CATALOG_ID)
    fail(`economy.catalogId must remain '${CATALOG_ID}'.`);
  if (economy.schemaVersion !== 1)
    fail("economy.schemaVersion must remain 1.");
  if (economy.proposalOnly !== true)
    fail("economy.proposalOnly must remain true.");
  if (economy.authoritative !== false)
    fail("economy.authoritative must remain false.");
}

const activation = parsedJson.get(activationPath);
const activationKeys = [
  "schemaVersion",
  "manifestId",
  "catalogSchemaVersion",
  "catalogId",
  "catalogSourceSha256",
  "unityVersion",
  "productApproval",
  "compilationVerified",
  "editorSelfTestsPassed",
  "editModeTestsPassed",
  "playModeTestsPassed",
  "runtimeActivationApproved"
];
if (activation) {
  exactKeys(activation, activationKeys, "runtime activation manifest");
  for (const key of ["schemaVersion", "catalogSchemaVersion"])
    expectType(activation[key], "number", `runtime activation manifest.${key}`);
  for (const key of [
    "manifestId", "catalogId", "catalogSourceSha256", "unityVersion"
  ]) expectType(activation[key], "string", `runtime activation manifest.${key}`);
  for (const key of [
    "productApproval", "compilationVerified", "editorSelfTestsPassed",
    "editModeTestsPassed", "playModeTestsPassed", "runtimeActivationApproved"
  ]) expectType(activation[key], "boolean", `runtime activation manifest.${key}`);

  if (!Number.isSafeInteger(activation.schemaVersion) || activation.schemaVersion !== 1)
    fail("runtime activation manifest.schemaVersion must equal integer 1.");
  if (!Number.isSafeInteger(activation.catalogSchemaVersion) || activation.catalogSchemaVersion !== 1)
    fail("runtime activation manifest.catalogSchemaVersion must equal integer 1.");
  if (activation.manifestId !== ACTIVATION_MANIFEST_ID)
    fail(`runtime activation manifest.manifestId must equal '${ACTIVATION_MANIFEST_ID}'.`);
  if (activation.catalogId !== CATALOG_ID)
    fail(`runtime activation manifest.catalogId must bind to '${CATALOG_ID}'.`);
  if (activation.catalogSourceSha256 !== CATALOG_SHA256)
    fail("runtime activation manifest.catalogSourceSha256 does not bind to the approved catalog bytes.");
  if (activation.unityVersion !== UNITY_VERSION)
    fail(`runtime activation manifest.unityVersion must equal '${UNITY_VERSION}'.`);
  if (activation.productApproval !== true)
    fail("runtime activation manifest.productApproval must be true.");
  for (const key of [
    "compilationVerified", "editorSelfTestsPassed", "editModeTestsPassed",
    "playModeTestsPassed", "runtimeActivationApproved"
  ]) {
    if (activation[key] !== false)
      fail(`runtime activation manifest.${key} must remain false before Unity approval.`);
  }
}

const saveVersionUnchanged = exists(gameStatePath) &&
  /public\s+int\s+saveVersion\s*=\s*6\s*;/.test(read(gameStatePath));
if (!saveVersionUnchanged) fail("PlayerSaveData.saveVersion must remain 6.");
const saveKeyUnchanged = exists(saveServicePath) &&
  /const\s+string\s+Key\s*=\s*"LegendsOfRus_Save_v1"\s*;/.test(read(saveServicePath));
if (!saveKeyUnchanged)
  fail("SaveService key must remain 'LegendsOfRus_Save_v1'.");

const runtimeEconomyAsmdefPath =
  "Assets/LegendsOfRus/Scripts/RuntimeEconomy/LegendsOfRus.RuntimeEconomy.asmdef";
const runtimeEconomyAsmdef = parsedAsmdefs.get(runtimeEconomyAsmdefPath) ?? null;
const expectedRuntimeEconomyReferences = [
  "LegendsOfRus.EconomyCore",
  "LegendsOfRus.Runtime"
];
const runtimeEconomyReferencesExact =
  Array.isArray(runtimeEconomyAsmdef?.references) &&
  [...runtimeEconomyAsmdef.references].sort().join("\n") ===
    [...expectedRuntimeEconomyReferences].sort().join("\n");
if (runtimeEconomyAsmdef) {
  if (runtimeEconomyAsmdef.name !== "LegendsOfRus.RuntimeEconomy")
    fail("RuntimeEconomy asmdef name must equal 'LegendsOfRus.RuntimeEconomy'.");
  if (!runtimeEconomyReferencesExact)
    fail("RuntimeEconomy asmdef must reference exactly EconomyCore and Runtime.");
  if (runtimeEconomyAsmdef.noEngineReferences !== false)
    fail("RuntimeEconomy asmdef must allow Unity engine references.");
}
const playModeAsmdef = parsedAsmdefs.get(playModeAsmdefPath) ?? null;
const playModeAsmdefReferencesRuntimeEconomy =
  Array.isArray(playModeAsmdef?.references) &&
  playModeAsmdef.references.includes("LegendsOfRus.RuntimeEconomy");
if (!playModeAsmdefReferencesRuntimeEconomy)
  fail(`${playModeAsmdefPath} must reference 'LegendsOfRus.RuntimeEconomy'.`);

const forbiddenBoundaryPatterns = [
  ["PlayerPrefs", /\bPlayerPrefs\b/],
  ["SaveService", /\bSaveService\b/],
  ["GameState.Instance", /\bGameState\s*\.\s*Instance\b/],
  ["Persist", /\bPersist\s*\(/],
  ["RuntimeBootstrap registration", /\bRuntimeBootstrap\b/]
];
for (const file of [gatePath, coordinatorPath]) {
  if (!exists(file)) continue;
  const source = scrubCSharp(read(file));
  for (const [label, pattern] of forbiddenBoundaryPatterns)
    if (pattern.test(source)) fail(`${file} contains forbidden boundary access '${label}'.`);
}

if (exists(gatePath)) {
  const gateSource = read(gatePath);
  const gateCode = scrubCSharp(gateSource);
  const requiredGateConstants = [
    ["manifest id", "ManifestId", ACTIVATION_MANIFEST_ID],
    ["catalog id", "CatalogId", CATALOG_ID],
    ["Unity version", "RequiredUnityVersion", UNITY_VERSION],
    ["catalog SHA-256", "CatalogSourceSha256", CATALOG_SHA256]
  ];
  for (const [label, name, value] of requiredGateConstants) {
    const declaration = new RegExp(
      `^\\s*public\\s+const\\s+string\\s+${name}\\s*=\\s*"${escapeRegExp(value)}"\\s*;`,
      "m"
    );
    if (!declaration.test(gateSource))
      fail(`${gatePath} is missing the required ${label} binding.`);
  }
  const requiredGateProperties = [
    ["product approval", "productApproval"],
    ["compilation verification", "compilationVerified"],
    ["Editor self-test verification", "editorSelfTestsPassed"],
    ["EditMode verification", "editModeTestsPassed"],
    ["PlayMode verification", "playModeTestsPassed"],
    ["runtime activation approval", "runtimeActivationApproved"]
  ];
  for (const [label, property] of requiredGateProperties) {
    const use = new RegExp(`\\bmanifest\\s*\\.\\s*${property}\\b`);
    if (!use.test(gateCode))
      fail(`${gatePath} is missing the required ${label} decision input.`);
  }
}

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
  let preprocessorDepth = 0;
  for (const line of source.split(/\r?\n/)) {
    if (/^\s*#if\b/.test(line)) preprocessorDepth++;
    if (/^\s*#endif\b/.test(line)) {
      preprocessorDepth--;
      if (preprocessorDepth < 0) break;
    }
  }
  if (preprocessorDepth !== 0)
    fail(`${file} has unmatched #if/#endif directives.`);
}

const testPattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
function countTestAttributes(file) {
  return exists(file) ? scrubCSharp(read(file)).match(testPattern)?.length ?? 0 : 0;
}
const testFiles = csharpFiles.filter((file) => file.includes("/Tests/"));
let discoveredTests = 0;
for (const file of testFiles)
  discoveredTests += countTestAttributes(file);
const step02TestFiles = testFiles.filter((file) =>
  /RuntimeEconomy|EconomyTransaction|EnergyIntegration|Step15.*(?:Runtime|Transaction|Energy)/i
    .test(path.basename(file))
);
let step02Tests = 0;
for (const file of step02TestFiles)
  step02Tests += countTestAttributes(file);
if (step02Tests === 0)
  fail("No v1.15 Step 2 runtime economy transaction/energy tests were discovered.");
const engineEditModeTests = countTestAttributes(engineTestPath);
const coordinatorPlayModeTests = countTestAttributes(coordinatorTestPath);
if (engineEditModeTests === 0)
  fail(`${engineTestPath} must contain at least one EditMode test.`);
if (coordinatorPlayModeTests === 0)
  fail(`${coordinatorTestPath} must contain at least one PlayMode test.`);

const step02SelfTestFiles = csharpFiles.filter((file) =>
  file.includes("/Scripts/Editor/") &&
  /Step15.*(?:RuntimeEconomy|Transaction|Energy).*SelfTest/i.test(path.basename(file))
);
let step02EditorChecks = 0;
for (const file of step02SelfTestFiles)
  step02EditorChecks += Math.max(
    0,
    (scrubCSharp(read(file)).match(/\bCheck\s*\(/g)?.length ?? 0) - 1
  );
if (step02EditorChecks === 0)
  fail("No v1.15 Step 2 Editor self-test Check sites were discovered.");

const currentPngs = files("*.png");
if (currentPngs.length !== 65)
  fail(`Expected exactly 65 approved PNGs, found ${currentPngs.length}.`);

const protectedLiveFiles = [
  gameStatePath,
  "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/MonetizationService.cs"
];
let pngFilesIdentical = null;
let protectedLiveFilesIdentical = null;
let forbiddenPackageAdditionsAbsent = null;
let actualChangedFiles = null;
let changedManifestMatchesBaseline = null;
let baselineStep01ContractValid = null;
let baselineStep01VerifierPassed = null;

function dependencyKeys(manifest) {
  return isPlainObject(manifest?.dependencies)
    ? Object.keys(manifest.dependencies).sort()
    : [];
}

if (baseline) {
  if (!fs.existsSync(baseline) || !fs.statSync(baseline).isDirectory()) {
    fail(`Baseline directory does not exist: ${baseline}`);
  } else {
    if (fs.realpathSync(baseline) === fs.realpathSync(root))
      fail("The Step 2 project cannot be used as its own Step 1 baseline.");

    const baselineStaticReportPath =
      "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_15_STEP01.json";
    const baselineStaticReport = exists(baselineStaticReportPath, baseline)
      ? parseJson(baselineStaticReportPath, baseline)
      : null;
    const baselineStaticReportValid = Boolean(baselineStaticReport) &&
      baselineStaticReport.version === "v1.15-step01" &&
      Array.isArray(baselineStaticReport.failures) &&
      baselineStaticReport.failures.length === 0 &&
      (!Array.isArray(baselineStaticReport.static_failures) ||
        baselineStaticReport.static_failures.length === 0);

    const baselineVerifierPath = path.join(baseline, "Tools/verify_v1_15_step01.mjs");
    if (!fs.existsSync(baselineVerifierPath)) {
      fail("The supplied baseline is missing Tools/verify_v1_15_step01.mjs.");
      baselineStep01VerifierPassed = false;
    } else {
      const execution = spawnSync(
        process.execPath,
        [baselineVerifierPath, "--source-only"],
        { cwd: baseline, encoding: "utf8", stdio: ["ignore", "pipe", "pipe"] }
      );
      let report = null;
      try {
        report = JSON.parse(execution.stdout || "null");
      } catch (error) {
        fail(`Baseline Step 1 verifier did not emit valid JSON: ${error.message}`);
      }
      baselineStep01VerifierPassed =
        !execution.error &&
        execution.status === 0 &&
        report?.version === "v1.15-step01" &&
        Array.isArray(report?.failures) &&
        report.failures.length === 0;
      if (!baselineStep01VerifierPassed) {
        const inherited = Array.isArray(report?.failures)
          ? report.failures.join(" | ")
          : (execution.stderr || execution.error?.message || "unknown failure").trim();
        fail(`Corrected baseline Step 1 source verification failed: ${inherited}`);
      }
    }

    baselineStep01ContractValid =
      exists(milestoneVersionPath, baseline) &&
      read(milestoneVersionPath, baseline).trim() === STEP01_MILESTONE_ID &&
      exists(economyPath, baseline) &&
      sha256File(economyPath, baseline) === CATALOG_SHA256 &&
      exists(catalogValidatorPath, baseline) &&
      sha256File(catalogValidatorPath, baseline) === CORRECTED_STEP01_VALIDATOR_SHA256 &&
      exists(projectVersionPath, baseline) &&
      read(projectVersionPath, baseline).trim() === `m_EditorVersion: ${UNITY_VERSION}` &&
      exists(gameStatePath, baseline) &&
      /public\s+int\s+saveVersion\s*=\s*6\s*;/.test(read(gameStatePath, baseline)) &&
      exists(saveServicePath, baseline) &&
      /const\s+string\s+Key\s*=\s*"LegendsOfRus_Save_v1"\s*;/.test(read(saveServicePath, baseline)) &&
      baselineStaticReportValid &&
      baselineStep01VerifierPassed;
    if (!baselineStep01ContractValid)
      fail("The supplied baseline is not the corrected immutable v1.15 Step 1 contract.");

    const baselineFiles = projectFilesAt(baseline);
    const baselinePngs = baselineFiles.filter((file) => file.endsWith(".png"));
    pngFilesIdentical =
      baselinePngs.length === 65 &&
      currentPngs.length === baselinePngs.length &&
      currentPngs.every((file, index) => file === baselinePngs[index]);
    if (!pngFilesIdentical)
      fail("Approved PNG path list differs from the corrected Step 1 baseline.");
    for (const file of currentPngs) {
      if (!exists(file, baseline) || sha256File(file) !== sha256File(file, baseline)) {
        pngFilesIdentical = false;
        fail(`Approved PNG differs from the corrected Step 1 baseline: ${file}`);
      }
    }

    protectedLiveFilesIdentical = true;
    for (const file of protectedLiveFiles) {
      if (!exists(file) || !exists(file, baseline) ||
          sha256File(file) !== sha256File(file, baseline)) {
        protectedLiveFilesIdentical = false;
        fail(`Existing live Runtime callsite differs from the corrected Step 1 baseline: ${file}`);
      }
    }

    const currentPackages = parsedJson.get(packageManifestPath);
    const baselinePackages = exists(packageManifestPath, baseline)
      ? parseJson(packageManifestPath, baseline)
      : null;
    const currentDependencies = dependencyKeys(currentPackages);
    const baselineDependencies = new Set(dependencyKeys(baselinePackages));
    const packageAdditions = currentDependencies.filter((id) => !baselineDependencies.has(id));
    const forbiddenPackagePattern =
      /(?:^|[._-])(?:ads?|advertis(?:ement|ing)?|iap|purchas(?:e|ing)|store|backend|services|remote[._-]?config|firebase|playfab|cloud|azure|aws|supabase|appwrite)(?:$|[._-])/i;
    const forbiddenAdditions = packageAdditions.filter((id) => forbiddenPackagePattern.test(id));
    forbiddenPackageAdditionsAbsent = forbiddenAdditions.length === 0;
    if (!forbiddenPackageAdditionsAbsent)
      fail(`Forbidden store/ad/backend/remote-config package addition(s): ${forbiddenAdditions.join(", ")}.`);

    const currentFiles = projectFilesAt(root);
    const union = new Set([...currentFiles, ...baselineFiles]);
    actualChangedFiles = [...union].filter((file) => {
      if (!exists(file) || !exists(file, baseline)) return true;
      return sha256File(file) !== sha256File(file, baseline);
    }).sort();

    if (exists(changedManifestPath)) {
      const manifestLines = read(changedManifestPath)
        .split(/\r?\n/)
        .map((line) => line.trim())
        .filter((line) => line && !/^(?:Added|Modified|Changed files|Files)\s*:?[\s]*$/i.test(line))
        .sort();
      changedManifestMatchesBaseline =
        manifestLines.length === actualChangedFiles.length &&
        actualChangedFiles.every((file, index) => file === manifestLines[index]);
      if (!changedManifestMatchesBaseline) {
        const missing = actualChangedFiles.filter((file) => !manifestLines.includes(file));
        const extra = manifestLines.filter((file) => !actualChangedFiles.includes(file));
        fail(`Changed-file manifest mismatch; missing=[${missing.join(",")}], extra=[${extra.join(",")}].`);
      }
    }
  }
}

const runtimeEconomyAsmdefValid = Boolean(runtimeEconomyAsmdef) &&
  runtimeEconomyAsmdef.name === "LegendsOfRus.RuntimeEconomy" &&
  runtimeEconomyReferencesExact &&
  runtimeEconomyAsmdef.noEngineReferences === false;

if (exists(staticReportPath)) {
  const report = parsedJson.get(staticReportPath);
  if (report) {
    if (report.version !== VERSION)
      fail(`${staticReportPath} version must equal '${VERSION}'.`);
    if (Array.isArray(report.failures) && report.failures.length > 0)
      fail(`${staticReportPath} records ${report.failures.length} unresolved failure(s).`);
    if (Array.isArray(report.static_failures) && report.static_failures.length > 0)
      fail(`${staticReportPath} records ${report.static_failures.length} static failure(s).`);
    const requiredReportFacts = new Map([
      ["status", "PASS_STATIC_AND_BOUNDARY_VERIFICATION"],
      ["runtime_candidate_status", "PRE_ACTIVATION_DETACHED"],
      ["exact_unity_version", true],
      ["economy_catalog_sha256", CATALOG_SHA256],
      ["economy_catalog_bytes_unchanged", true],
      ["proposal_only", true],
      ["authoritative", false],
      ["activation_manifest_exact_keys", true],
      ["activation_manifest_exact_types", true],
      ["activation_catalog_binding_valid", true],
      ["product_approval_recorded", true],
      ["compilation_editor_edit_play_runtime_flags_pending", true],
      ["save_version_unchanged", true],
      ["save_key_unchanged", true],
      ["runtime_economy_boundary_access_absent", true],
      ["runtime_economy_asmdef_valid", true],
      ["playmode_asmdef_references_runtime_economy", true],
      ["csharp_delimiters_balanced", true],
      ["preprocessor_directives_balanced", true],
      ["required_final_artifacts_present", true]
    ]);
    for (const [key, expected] of requiredReportFacts) {
      if (!hasOwn(report, key))
        fail(`${staticReportPath} is missing required fact '${key}'.`);
      else if (report[key] !== expected)
        fail(`${staticReportPath} ${key} must equal ${JSON.stringify(expected)}.`);
    }
    if (!Array.isArray(report.failures))
      fail(`${staticReportPath} failures must be an array.`);
    if (!Array.isArray(report.static_failures))
      fail(`${staticReportPath} static_failures must be an array.`);
    const exactReportFields = new Map([
      ["project_json_files", jsonFiles.length],
      ["project_asmdef_files", asmdefFiles.length],
      ["csharp_structural_scan_files", csharpFiles.length],
      ["discovered_editmode_and_playmode_tests", discoveredTests],
      ["new_step02_targeted_test_files", step02TestFiles.length],
      ["new_step02_targeted_tests", step02Tests],
      ["step02_engine_editmode_tests", engineEditModeTests],
      ["step02_coordinator_playmode_tests", coordinatorPlayModeTests],
      ["step02_editor_self_test_files", step02SelfTestFiles.length],
      ["step02_editor_self_test_check_sites", step02EditorChecks],
      ["current_project_png_files", currentPngs.length]
    ]);
    if (baseline) {
      exactReportFields.set("baseline_step01_contract_valid", baselineStep01ContractValid);
      exactReportFields.set("baseline_step01_verifier_passed", baselineStep01VerifierPassed);
      exactReportFields.set("project_png_files_compared", currentPngs.length);
      exactReportFields.set("project_png_files_path_and_hash_identical_to_step01", pngFilesIdentical);
      exactReportFields.set("protected_live_files_compared", protectedLiveFiles.length);
      exactReportFields.set("protected_live_files_identical_to_step01", protectedLiveFilesIdentical);
      exactReportFields.set("forbidden_package_additions_absent", forbiddenPackageAdditionsAbsent);
      exactReportFields.set("actual_changed_paths", actualChangedFiles?.length ?? null);
    }
    for (const [key, expected] of exactReportFields) {
      if (!hasOwn(report, key))
        fail(`${staticReportPath} is missing exact field '${key}'.`);
      else if (report[key] !== expected)
        fail(`${staticReportPath} ${key} must equal ${JSON.stringify(expected)}.`);
    }
  }
}

const activationFlagsArePending = Boolean(activation) &&
  activation.productApproval === true &&
  activation.compilationVerified === false &&
  activation.editorSelfTestsPassed === false &&
  activation.editModeTestsPassed === false &&
  activation.playModeTestsPassed === false &&
  activation.runtimeActivationApproved === false;
const activationTypesAreExact = Boolean(activation) &&
  Number.isSafeInteger(activation.schemaVersion) &&
  Number.isSafeInteger(activation.catalogSchemaVersion) &&
  ["manifestId", "catalogId", "catalogSourceSha256", "unityVersion"]
    .every((key) => typeof activation[key] === "string") &&
  [
    "productApproval", "compilationVerified", "editorSelfTestsPassed",
    "editModeTestsPassed", "playModeTestsPassed", "runtimeActivationApproved"
  ].every((key) => typeof activation[key] === "boolean");

const passed = failures.length === 0;

const result = {
  version: VERSION,
  status: passed
    ? "PASS_STATIC_AND_BOUNDARY_VERIFICATION"
    : "FAIL_STATIC_AND_BOUNDARY_VERIFICATION",
  passed,
  runtime_candidate_status: "PRE_ACTIVATION_DETACHED",
  step01_source_verification_passed: step01VerificationPassed,
  exact_unity_version: exactProjectVersion,
  all_project_json_parses: !failures.some((failure) => failure.startsWith("JSON ")),
  project_json_files: jsonFiles.length,
  project_asmdef_files: asmdefFiles.length,
  economy_catalog_id: economy?.catalogId ?? null,
  economy_catalog_sha256: economyHash,
  economy_catalog_bytes_unchanged: economyHash === CATALOG_SHA256,
  proposal_only: economy?.proposalOnly ?? null,
  authoritative: economy?.authoritative ?? null,
  activation_manifest_exact_keys:
    Boolean(activation) &&
    Object.keys(activation).sort().join("\n") === [...activationKeys].sort().join("\n"),
  activation_manifest_exact_types: activationTypesAreExact,
  activation_catalog_binding_valid:
    Boolean(activation) &&
    activation.catalogSchemaVersion === economy?.schemaVersion &&
    activation.catalogId === economy?.catalogId &&
    activation.catalogSourceSha256 === economyHash,
  product_approval_recorded: activation?.productApproval ?? null,
  compilation_editor_edit_play_runtime_flags_pending: activationFlagsArePending,
  save_version_unchanged: saveVersionUnchanged,
  save_key_unchanged: saveKeyUnchanged,
  runtime_economy_boundary_access_absent:
    !failures.some((failure) => failure.includes("forbidden boundary access")),
  runtime_economy_asmdef_valid: runtimeEconomyAsmdefValid,
  playmode_asmdef_references_runtime_economy: playModeAsmdefReferencesRuntimeEconomy,
  csharp_structural_scan_files: csharpFiles.length,
  csharp_delimiters_balanced:
    !failures.some((failure) => failure.includes("delimiters")),
  preprocessor_directives_balanced:
    !failures.some((failure) => failure.includes("#if/#endif")),
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_step02_targeted_test_files: step02TestFiles.length,
  new_step02_targeted_tests: step02Tests,
  step02_engine_editmode_tests: engineEditModeTests,
  step02_coordinator_playmode_tests: coordinatorPlayModeTests,
  step02_editor_self_test_files: step02SelfTestFiles.length,
  step02_editor_self_test_check_sites: step02EditorChecks,
  current_project_png_files: currentPngs.length,
  baseline_supplied: baseline !== null,
  baseline_step01_contract_valid: baseline ? baselineStep01ContractValid : null,
  baseline_step01_verifier_passed: baseline ? baselineStep01VerifierPassed : null,
  project_png_files_compared: baseline ? currentPngs.length : 0,
  project_png_files_path_and_hash_identical_to_step01: baseline ? pngFilesIdentical : null,
  protected_live_files_compared: baseline ? protectedLiveFiles.length : 0,
  protected_live_files_identical_to_step01: baseline ? protectedLiveFilesIdentical : null,
  forbidden_package_additions_absent: baseline ? forbiddenPackageAdditionsAbsent : null,
  actual_changed_paths: baseline ? actualChangedFiles?.length ?? null : null,
  changed_file_manifest_present: exists(changedManifestPath),
  changed_file_manifest_matches_step01_baseline:
    baseline && exists(changedManifestPath) ? changedManifestMatchesBaseline : null,
  static_report_present: exists(staticReportPath),
  required_final_artifacts_present: required.every((file) => exists(file)),
  unity_execution: "PENDING_UNITY_EDITOR",
  failures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (failures.length > 0) process.exitCode = 1;
