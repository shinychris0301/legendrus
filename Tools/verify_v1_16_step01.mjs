import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { execFileSync, spawnSync } from "node:child_process";

const VERSION = "v1.16-step01";
const MILESTONE_ID = "v1.16-step01-commercial-adapters-sandbox-foundation";
const BASELINE_MILESTONE_ID =
  "v1.15-step02-runtime-economy-transactions-energy-integration";
const UNITY_VERSION = "6000.5.9f1";
const ECONOMY_SHA256 =
  "ba66d7104c3d0a33ea4e0575b9965325cf9f90a8177c4e089c638d202267c42c";
const ACTIVATION_SHA256 =
  "4c4a3d062257addf9e3bd50008a5bde281aa3e227bffe097982b7890bd597248";
const PACKAGE_MANIFEST_SHA256 =
  "f534b3f48018d7c02ddfb1b59588813562837db8167dda7395956065bf4f6713";
const BASELINE_VERIFIER_SHA256 =
  "86c99a0f409e115109058d5e5b086b495ee3001282411b547aa71248a42c45b5";
const REMOTE_CONFIG_LOCAL_SHA256 =
  "3d8c8c676217bd87e884fdeaa5663871cf54cf6daac23b6584fcbd62f44a8536";

const root = process.cwd();
const failures = [];
const sourceOnly = process.argv.includes("--source-only");

function option(name) {
  const index = process.argv.indexOf(name);
  if (index < 0) return null;
  const value = process.argv[index + 1];
  if (!value || value.startsWith("--")) {
    fail(`${name} requires a directory argument.`);
    return null;
  }
  return path.resolve(value);
}

const baseline = option("--baseline");
const step01Baseline = option("--step01-baseline");

function fail(message) {
  failures.push(message);
}

if (sourceOnly && (baseline || step01Baseline))
  fail("Choose either --source-only or baseline verification, not both.");
if (!sourceOnly && !baseline)
  fail("Release verification requires --baseline <corrected-v1.15-step02-project>.");
if (!sourceOnly && !step01Baseline)
  fail("Release verification requires --step01-baseline <corrected-v1.15-step01-project>.");

function absolute(relative, directory = root) {
  return path.join(directory, relative);
}

function exists(relative, directory = root) {
  return fs.existsSync(absolute(relative, directory));
}

function read(relative, directory = root) {
  return fs.readFileSync(absolute(relative, directory), "utf8");
}

function sha256File(relative, directory = root) {
  return crypto
    .createHash("sha256")
    .update(fs.readFileSync(absolute(relative, directory)))
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
    fail(`Unable to enumerate '${glob}': ${error.message}`);
    return [];
  }
}

function parseJson(relative, directory = root, recordFailure = true) {
  try {
    return JSON.parse(read(relative, directory));
  } catch (error) {
    if (recordFailure) fail(`JSON ${relative}: ${error.message}`);
    return null;
  }
}

function isObject(value) {
  return value !== null && typeof value === "object" && !Array.isArray(value);
}

function sameArray(left, right) {
  return Array.isArray(left) &&
    left.length === right.length &&
    left.every((item, index) => item === right[index]);
}

function hasOwn(value, key) {
  return Object.prototype.hasOwnProperty.call(value, key);
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
      } else output += " ";
    } else if (state === "block") {
      if (char === "*" && next === "/") {
        state = "code";
        output += "  ";
        index++;
      } else output += char === "\n" ? "\n" : " ";
    } else if (state === "verbatim") {
      if (char === '"' && next === '"') {
        output += "  ";
        index++;
      } else if (char === '"') {
        state = "code";
        output += " ";
      } else output += char === "\n" ? "\n" : " ";
    } else if (char === "\\") {
      output += "  ";
      index++;
    } else if (
      (state === "string" && char === '"') ||
      (state === "char" && char === "'")
    ) {
      state = "code";
      output += " ";
    } else output += char === "\n" ? "\n" : " ";
  }
  return output;
}

const milestonePath = "MILESTONE_VERSION.txt";
const projectVersionPath = "ProjectSettings/ProjectVersion.txt";
const packagesPath = "Packages/manifest.json";
const economyPath = "Assets/LegendsOfRus/Resources/Config/economy.json";
const activationPath =
  "Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json";
const remoteConfigPath =
  "Assets/LegendsOfRus/Resources/Config/remote_config_local.json";
const commercialRoot = "Assets/LegendsOfRus/Scripts/CommercialCore";
const commercialAsmdefPath =
  `${commercialRoot}/LegendsOfRus.CommercialCore.asmdef`;
const testRoot = "Assets/LegendsOfRus/Tests/CommercialEditMode";
const testAsmdefPath =
  `${testRoot}/LegendsOfRus.CommercialCore.Tests.asmdef`;
const selfTestPath =
  "Assets/LegendsOfRus/Scripts/Editor/Step16CommercialAdaptersSandboxSelfTest.cs";
const changedManifestPath =
  "Assets/LegendsOfRus/Documentation/V1_16_STEP01_CHANGED_FILES.txt";
const staticReportPath =
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_16_STEP01.json";

const required = [
  "README.md",
  "README_V1_16_STEP01.md",
  milestonePath,
  projectVersionPath,
  packagesPath,
  economyPath,
  activationPath,
  remoteConfigPath,
  commercialAsmdefPath,
  selfTestPath,
  testAsmdefPath,
  "Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_16_STEP01.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_16_STEP02.json",
  "Assets/LegendsOfRus/Documentation/V1_16_STEP01_COMMERCIAL_ADAPTERS_SANDBOX_FOUNDATION.md",
  changedManifestPath,
  staticReportPath,
  "Tools/verify_v1_15_step02.mjs",
  "Tools/verify_v1_16_step01.mjs"
];
for (const file of required)
  if (!exists(file)) fail(`Required v1.16 Step 1 artifact is missing: ${file}.`);

if (exists(milestonePath) && read(milestonePath).trim() !== MILESTONE_ID)
  fail(`${milestonePath} must equal '${MILESTONE_ID}'.`);

const exactUnityVersion = exists(projectVersionPath) &&
  read(projectVersionPath).trim() === `m_EditorVersion: ${UNITY_VERSION}`;
if (!exactUnityVersion)
  fail(`${projectVersionPath} must contain exactly 'm_EditorVersion: ${UNITY_VERSION}'.`);

const jsonFiles = files("*.json");
const parsedJson = new Map();
for (const file of jsonFiles) parsedJson.set(file, parseJson(file));
const asmdefFiles = files("*.asmdef");
const parsedAsmdefs = new Map();
for (const file of asmdefFiles) parsedAsmdefs.set(file, parseJson(file));

const economyHash = exists(economyPath) ? sha256File(economyPath) : null;
if (economyHash !== ECONOMY_SHA256)
  fail(`${economyPath} must retain the reviewed SHA-256.`);
const economy = parsedJson.get(economyPath);
if (economy) {
  if (economy.catalogId !== "v1.15-step01-proposal")
    fail("The economy catalog id changed.");
  if (economy.proposalOnly !== true || economy.authoritative !== false)
    fail("The economy catalog must remain proposal-only and non-authoritative.");
}

const activationHash = exists(activationPath) ? sha256File(activationPath) : null;
if (activationHash !== ACTIVATION_SHA256)
  fail(`${activationPath} bytes changed from the v1.15 Step 2 baseline.`);
const activation = parsedJson.get(activationPath);
const pendingActivationKeys = [
  "compilationVerified",
  "editorSelfTestsPassed",
  "editModeTestsPassed",
  "playModeTestsPassed",
  "runtimeActivationApproved"
];
const activationFlagsPreserved = Boolean(activation) &&
  activation.productApproval === true &&
  pendingActivationKeys.every((key) => activation[key] === false);
if (!activationFlagsPreserved)
  fail("Every v1.15 Unity-verification and runtime-activation flag must remain false.");

const packagesHash = exists(packagesPath) ? sha256File(packagesPath) : null;
if (packagesHash !== PACKAGE_MANIFEST_SHA256)
  fail(`${packagesPath} must remain byte-identical; Step 1 adds no SDK package.`);

const remoteConfigHash = exists(remoteConfigPath)
  ? sha256File(remoteConfigPath)
  : null;
if (remoteConfigHash !== REMOTE_CONFIG_LOCAL_SHA256)
  fail(`${remoteConfigPath} must retain the reviewed all-disabled SHA-256.`);
const remoteConfig = parsedJson.get(remoteConfigPath);
const expectedRemoteConfigKeys = [
  "analytics.capture.enabled",
  "analytics.upload.enabled",
  "monetization.iap.enabled",
  "monetization.rewarded_ads.enabled",
  "monetization.rewarded_energy.enabled",
  "monetization.rewarded_continue.enabled",
  "monetization.rewarded_double.enabled",
  "monetization.starter_offer.enabled",
  "monetization.chapter_finale_offer.enabled",
  "monetization.development_sandbox.enabled"
].sort();
const remoteConfigAllDisabled = Boolean(remoteConfig) &&
  remoteConfig.schemaVersion === 1 &&
  remoteConfig.snapshotId === "v1.16-step01-local-defaults" &&
  remoteConfig.revision === 1 &&
  remoteConfig.environment === "development" &&
  remoteConfig.proposalOnly === true &&
  remoteConfig.authoritative === false &&
  remoteConfig.economyCatalogId === "v1.15-step01-proposal" &&
  remoteConfig.economyCatalogSourceSha256 === ECONOMY_SHA256 &&
  Array.isArray(remoteConfig.entries) &&
  sameArray(
    remoteConfig.entries.map((entry) => entry?.key).sort(),
    expectedRemoteConfigKeys
  ) &&
  remoteConfig.entries.every((entry) =>
    isObject(entry) &&
    entry.type === "bool" &&
    entry.value === "false" &&
    Object.keys(entry).sort().join("\n") === ["key", "type", "value"].sort().join("\n")
  );
if (!remoteConfigAllDisabled)
  fail("The bundled remote config must remain proposal-only, bound, exact-keyed, and all-disabled.");

const gameStatePath = "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs";
const saveServicePath = "Assets/LegendsOfRus/Scripts/Runtime/Services/SaveService.cs";
const saveVersionPreserved = exists(gameStatePath) &&
  /public\s+int\s+saveVersion\s*=\s*6\s*;/.test(read(gameStatePath));
const saveKeyPreserved = exists(saveServicePath) &&
  /const\s+string\s+Key\s*=\s*"LegendsOfRus_Save_v1"\s*;/.test(
    read(saveServicePath)
  );
if (!saveVersionPreserved) fail("PlayerSaveData.saveVersion must remain 6.");
if (!saveKeyPreserved) fail("SaveService key must remain LegendsOfRus_Save_v1.");

const commercialAsmdef = parsedAsmdefs.get(commercialAsmdefPath);
const expectedCommercialAsmdefKeys = [
  "name", "rootNamespace", "references", "includePlatforms",
  "excludePlatforms", "allowUnsafeCode", "overrideReferences",
  "precompiledReferences", "autoReferenced", "defineConstraints",
  "versionDefines", "noEngineReferences"
].sort();
const commercialAsmdefValid = Boolean(commercialAsmdef) &&
  Object.keys(commercialAsmdef).sort().join("\n") ===
    expectedCommercialAsmdefKeys.join("\n") &&
  commercialAsmdef.name === "LegendsOfRus.CommercialCore" &&
  commercialAsmdef.rootNamespace === "LegendsOfRus.CommercialCore" &&
  sameArray(commercialAsmdef.references, []) &&
  sameArray(commercialAsmdef.includePlatforms, []) &&
  sameArray(commercialAsmdef.excludePlatforms, []) &&
  commercialAsmdef.allowUnsafeCode === false &&
  commercialAsmdef.overrideReferences === false &&
  sameArray(commercialAsmdef.precompiledReferences, []) &&
  commercialAsmdef.autoReferenced === true &&
  sameArray(commercialAsmdef.defineConstraints, []) &&
  sameArray(commercialAsmdef.versionDefines, []) &&
  commercialAsmdef.noEngineReferences === true;
if (!commercialAsmdefValid)
  fail("CommercialCore asmdef must be the exact engine-free, zero-reference contract.");

const testAsmdef = parsedAsmdefs.get(testAsmdefPath);
const testAsmdefValid = Boolean(testAsmdef) &&
  testAsmdef.name === "LegendsOfRus.CommercialCore.Tests" &&
  sameArray(testAsmdef.references, ["LegendsOfRus.CommercialCore"]) &&
  sameArray(testAsmdef.includePlatforms, ["Editor"]) &&
  testAsmdef.noEngineReferences === false &&
  sameArray(testAsmdef.optionalUnityReferences, ["TestAssemblies"]) &&
  sameArray(testAsmdef.precompiledReferences, []);
if (!testAsmdefValid)
  fail("CommercialCore test asmdef must be Editor-only and reference only CommercialCore plus TestAssemblies.");

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
    if (/^\s*#endif\b/.test(line)) preprocessorDepth--;
    if (preprocessorDepth < 0) break;
  }
  if (preprocessorDepth !== 0)
    fail(`${file} has unmatched #if/#endif directives.`);
}

const commercialSourceFiles = csharpFiles.filter((file) =>
  file.startsWith(`${commercialRoot}/`)
);
const domainNames = ["Analytics", "RemoteConfig", "Monetization", "Consent"];
const commercialDomainSourceCounts = Object.fromEntries(domainNames.map((domain) => [
  domain,
  commercialSourceFiles.filter((file) => file.includes(`/${domain}/`)).length
]));
for (const domain of domainNames) {
  if (commercialDomainSourceCounts[domain] === 0)
    fail(`CommercialCore ${domain} source is missing.`);
}

const forbiddenCommercialCode = [
  ["Unity runtime/editor API", /\b(?:UnityEngine|UnityEditor)\b/],
  ["live player state", /\b(?:PlayerPrefs|SaveService|GameState|ScreenRouter|RuntimeBootstrap|MonetizationService|AnalyticsService)\b/],
  ["economy grant API", /\b(?:AddGold|AddGems|AddEnergy|RefillEnergy|ConsumeBooster|Persist)\s*\(/],
  ["file persistence", /\bSystem\s*\.\s*IO\b|\b(?:File|Directory|FileStream|StreamReader|StreamWriter)\s*\./],
  ["network API", /\bSystem\s*\.\s*Net\b|\b(?:HttpClient|UnityWebRequest|Socket|WebRequest)\b/],
  ["thread/timer API", /\bSystem\s*\.\s*Threading\b|\b(?:Thread|Timer|Task)\b/],
  ["ambient wall clock", /\bDateTime\s*\.\s*(?:UtcNow|Now)\b/],
  ["ambient identifier", /\bGuid\s*\.\s*NewGuid\s*\(/],
  ["ambient randomness", /\b(?:System\s*\.\s*)?Random\s*\(/]
];
for (const file of commercialSourceFiles) {
  const source = read(file);
  const code = scrubCSharp(source);
  for (const [label, pattern] of forbiddenCommercialCode) {
    if (pattern.test(code)) fail(`${file} contains forbidden ${label}.`);
  }
  if (/https?:\/\//i.test(source))
    fail(`${file} contains an embedded network endpoint.`);
  if (/\b(?:api[_-]?key|client[_-]?secret|private[_-]?key|ad[_-]?unit[_-]?id)\b/i.test(source))
    fail(`${file} contains a credential or production-provider identifier.`);
}

const combinedCommercialSource = commercialSourceFiles.map((file) => read(file)).join("\n");
const requiredTypeNames = [
  "IAnalyticsAdapter",
  "DisabledAnalyticsAdapter",
  "DeterministicOfflineAnalyticsAdapter",
  "AnalyticsPipeline",
  "AnalyticsFunnelTracker",
  "IRemoteConfigProvider",
  "DeterministicLocalRemoteConfigProvider",
  "RemoteConfigValidator",
  "RemoteConfigSnapshot",
  "IConsentProvider",
  "DisabledConsentProvider",
  "InMemoryDevelopmentConsentProvider",
  "ConsentEligibility",
  "IIapAdapter",
  "IPurchaseVerifier",
  "IRewardedAdAdapter",
  "IRewardedAdVerifier",
  "DefaultUnavailableIapAdapter",
  "DefaultUnavailableRewardedAdAdapter",
  "ScriptedDevelopmentIapAdapter",
  "ScriptedDevelopmentRewardedAdAdapter",
  "VerifiedEntitlementGate"
];
for (const name of requiredTypeNames) {
  if (!new RegExp(`\\b${name}\\b`).test(combinedCommercialSource))
    fail(`CommercialCore is missing required contract type ${name}.`);
}

const monetizationGatePath =
  `${commercialRoot}/Monetization/VerifiedEntitlementGate.cs`;
const monetizationContractsPath =
  `${commercialRoot}/Monetization/MonetizationContracts.cs`;
const monetizationGateSource = exists(monetizationGatePath)
  ? scrubCSharp(read(monetizationGatePath))
  : "";
const monetizationContractsSource = exists(monetizationContractsPath)
  ? scrubCSharp(read(monetizationContractsPath))
  : "";
function publicMethodParameterSets(source, methodName) {
  return [...source.matchAll(new RegExp(
    `public\\s+EntitlementEligibilityResult\\s+${methodName}\\s*\\(([^)]*)\\)`,
    "g"
  ))].map((match) => match[1]);
}
const gateConstructorParameters = [
  ...monetizationGateSource.matchAll(
    /public\s+VerifiedEntitlementGate\s*\(([^)]*)\)/g
  )
].map((match) => match[1]);
const purchaseReservationParameterSets = publicMethodParameterSets(
  monetizationGateSource,
  "TryReservePurchase"
);
const rewardedReservationParameterSets = publicMethodParameterSets(
  monetizationGateSource,
  "TryReserveRewardedAd"
);
const purchaseReservationParameters = purchaseReservationParameterSets[0] ?? "";
const rewardedReservationParameters = rewardedReservationParameterSets[0] ?? "";
const normalizedGateConstructorParameters =
  (gateConstructorParameters[0] ?? "").replace(/\s+/g, " ").trim();
const normalizedPurchaseReservationParameters =
  purchaseReservationParameters.replace(/\s+/g, " ").trim();
const normalizedRewardedReservationParameters =
  rewardedReservationParameters.replace(/\s+/g, " ").trim();
const monetizationVerifierProvenanceBoundary =
  gateConstructorParameters.length === 1 &&
  /^MonetizationEnvironment environment, IPurchaseVerifier purchaseVerifier, IRewardedAdVerifier rewardedAdVerifier, IEnumerable<string> durableCompletedFulfillmentKeys = null$/.test(
    normalizedGateConstructorParameters
  ) &&
  purchaseReservationParameterSets.length === 1 &&
  rewardedReservationParameterSets.length === 1 &&
  normalizedPurchaseReservationParameters ===
    "PurchaseRequest request, PurchaseResult purchase" &&
  normalizedRewardedReservationParameters ===
    "RewardedAdRequest request, RewardedAdResult result" &&
  /readonly\s+IPurchaseVerifier\s+purchaseVerifier\s*;/.test(monetizationGateSource) &&
  /readonly\s+IRewardedAdVerifier\s+rewardedAdVerifier\s*;/.test(monetizationGateSource) &&
  /this\.purchaseVerifier\s*=\s*purchaseVerifier\s*;/.test(monetizationGateSource) &&
  /this\.rewardedAdVerifier\s*=\s*rewardedAdVerifier\s*;/.test(monetizationGateSource) &&
  /verifierBindingsValid\s*=\s*purchaseVerifier\s*!=\s*null\s*&&\s*rewardedAdVerifier\s*!=\s*null\s*;/.test(
    monetizationGateSource
  ) &&
  /public\s+bool\s+IsOperational[\s\S]{0,400}\bverifierBindingsValid\b/.test(
    monetizationGateSource
  ) &&
  /purchaseVerifier\.Verify\s*\(\s*purchase\.Evidence\s*\)/.test(
    monetizationGateSource
  ) &&
  /rewardedAdVerifier\.Verify\s*\(\s*result\.Evidence\s*\)/.test(
    monetizationGateSource
  ) &&
  /if\s*\(\s*!verifierBindingsValid\s*\)\s*return\s+MonetizationReasonCodes\.VerifierUnavailable\s*;/.test(
    monetizationGateSource
  ) &&
  /internal\s+PurchaseVerificationResult\s*\(/.test(monetizationContractsSource) &&
  /internal\s+RewardedAdVerificationResult\s*\(/.test(monetizationContractsSource);
if (!monetizationVerifierProvenanceBoundary)
  fail("Verified entitlement reservation must use immutable constructor-bound verifiers and expose no per-call or result-based public bypass.");

const testPattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
function countTests(file, directory = root) {
  return exists(file, directory)
    ? scrubCSharp(read(file, directory)).match(testPattern)?.length ?? 0
    : 0;
}
const testFiles = csharpFiles.filter((file) => file.includes("/Tests/"));
const discoveredTests = testFiles.reduce((total, file) => total + countTests(file), 0);
const commercialTestFiles = testFiles.filter((file) => file.startsWith(`${testRoot}/`));
const commercialTests = commercialTestFiles.reduce(
  (total, file) => total + countTests(file),
  0
);
const commercialDomainTestCounts = Object.fromEntries(domainNames.map((domain) => {
  const matching = commercialTestFiles.filter((file) =>
    path.basename(file).toLowerCase().includes(domain.toLowerCase())
  );
  return [domain, matching.reduce((total, file) => total + countTests(file), 0)];
}));
for (const domain of domainNames) {
  if (commercialDomainTestCounts[domain] === 0)
    fail(`No ${domain} CommercialCore EditMode tests were discovered.`);
}
if (commercialTests === 0)
  fail("No v1.16 CommercialCore EditMode tests were discovered.");
if (commercialTestFiles.some((file) => /\[\s*NonParallelizable/.test(read(file))))
  fail("CommercialCore tests use unsupported NonParallelizable annotations.");

const selfTestFiles = csharpFiles.filter((file) =>
  file.includes("/Scripts/Editor/") &&
  /Step16.*Commercial.*SelfTest/i.test(path.basename(file))
);
const selfTestCheckSites = selfTestFiles.reduce((total, file) =>
  total + Math.max(0, (scrubCSharp(read(file)).match(/\bCheck\s*\(/g)?.length ?? 0) - 1),
  0
);
if (selfTestFiles.length !== 1 || selfTestCheckSites === 0)
  fail("Exactly one v1.16 Commercial self-test with nonzero Check sites is required.");

const currentPngs = files("*.png");
if (currentPngs.length !== 65)
  fail(`Expected exactly 65 approved PNGs, found ${currentPngs.length}.`);

let baselineVerifierPassed = null;
let baselineContractValid = null;
let baselineInheritedTests = null;
let approvedPngsIdentical = null;
let protectedFilesIdentical = null;
let existingAsmdefsIdentical = null;
let actualChangedFiles = null;
let changedManifestMatchesBaseline = null;

const protectedFiles = [
  economyPath,
  activationPath,
  packagesPath,
  projectVersionPath,
  gameStatePath,
  saveServicePath,
  "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/MonetizationService.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/AnalyticsService.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef",
  "Assets/LegendsOfRus/Scripts/RuntimeEconomy/LegendsOfRus.RuntimeEconomy.asmdef",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_15_STEP02_ACTIVATION.json"
];
const protectedPrefixes = [
  "Assets/LegendsOfRus/Scripts/EconomyCore/",
  "Assets/LegendsOfRus/Scripts/RuntimeEconomy/",
  "Assets/Scenes/",
  "ProjectSettings/"
];

if (baseline) {
  if (!fs.existsSync(baseline) || !fs.statSync(baseline).isDirectory()) {
    fail(`Step 2 baseline directory does not exist: ${baseline}.`);
  } else if (fs.realpathSync(baseline) === fs.realpathSync(root)) {
    fail("The v1.16 project cannot be its own v1.15 Step 2 baseline.");
  } else {
    const baselineVerifierPath = "Tools/verify_v1_15_step02.mjs";
    const baselineStaticPath =
      "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_15_STEP02.json";
    const baselineReport = exists(baselineStaticPath, baseline)
      ? parseJson(baselineStaticPath, baseline)
      : null;
    baselineContractValid =
      exists(milestonePath, baseline) &&
      read(milestonePath, baseline).trim() === BASELINE_MILESTONE_ID &&
      exists(projectVersionPath, baseline) &&
      read(projectVersionPath, baseline).trim() === `m_EditorVersion: ${UNITY_VERSION}` &&
      exists(economyPath, baseline) &&
      sha256File(economyPath, baseline) === ECONOMY_SHA256 &&
      exists(activationPath, baseline) &&
      sha256File(activationPath, baseline) === ACTIVATION_SHA256 &&
      exists(packagesPath, baseline) &&
      sha256File(packagesPath, baseline) === PACKAGE_MANIFEST_SHA256 &&
      exists(baselineVerifierPath, baseline) &&
      sha256File(baselineVerifierPath, baseline) === BASELINE_VERIFIER_SHA256 &&
      baselineReport?.version === "v1.15-step02" &&
      baselineReport?.status === "PASS_STATIC_AND_BOUNDARY_VERIFICATION" &&
      Array.isArray(baselineReport?.failures) &&
      baselineReport.failures.length === 0 &&
      (!Array.isArray(baselineReport?.static_failures) ||
        baselineReport.static_failures.length === 0);
    if (!baselineContractValid)
      fail("The supplied baseline is not the corrected immutable v1.15 Step 2 contract.");

    if (
      step01Baseline &&
      fs.existsSync(step01Baseline) &&
      fs.statSync(step01Baseline).isDirectory() &&
      exists(baselineVerifierPath, baseline)
    ) {
      const execution = spawnSync(
        process.execPath,
        [absolute(baselineVerifierPath, baseline), "--baseline", step01Baseline],
        { cwd: baseline, encoding: "utf8", stdio: ["ignore", "pipe", "pipe"] }
      );
      let report = null;
      try {
        report = JSON.parse(execution.stdout || "null");
      } catch (error) {
        fail(`Baseline Step 2 verifier did not emit valid JSON: ${error.message}`);
      }
      baselineVerifierPassed =
        !execution.error &&
        execution.status === 0 &&
        report?.version === "v1.15-step02" &&
        report?.status === "PASS_STATIC_AND_BOUNDARY_VERIFICATION" &&
        Array.isArray(report?.failures) &&
        report.failures.length === 0;
      if (!baselineVerifierPassed) {
        const details = Array.isArray(report?.failures)
          ? report.failures.join(" | ")
          : (execution.stderr || execution.error?.message || "unknown failure").trim();
        fail(`Corrected baseline Step 2 verification failed: ${details}`);
      }
    } else {
      baselineVerifierPassed = false;
      fail("A valid corrected Step 1 baseline is required to execute the Step 2 baseline verifier.");
    }

    const baselineFiles = projectFilesAt(baseline);
    const baselineCsharp = baselineFiles.filter((file) => file.endsWith(".cs"));
    const baselineTestFiles = baselineCsharp.filter((file) => file.includes("/Tests/"));
    baselineInheritedTests = baselineTestFiles.reduce(
      (total, file) => total + countTests(file, baseline),
      0
    );
    if (discoveredTests !== baselineInheritedTests + commercialTests)
      fail("Inherited test discovery changed outside the new CommercialEditMode suite.");

    const baselinePngs = baselineFiles.filter((file) => file.endsWith(".png"));
    approvedPngsIdentical =
      sameArray(currentPngs, baselinePngs) &&
      currentPngs.every((file) =>
        exists(file, baseline) && sha256File(file) === sha256File(file, baseline)
      );
    if (!approvedPngsIdentical)
      fail("Approved PNG paths or bytes differ from the v1.15 Step 2 baseline.");

    const prefixProtected = baselineFiles.filter((file) =>
      protectedPrefixes.some((prefix) => file.startsWith(prefix))
    );
    const allProtected = [...new Set([...protectedFiles, ...prefixProtected])].sort();
    protectedFilesIdentical = allProtected.every((file) =>
      exists(file) && exists(file, baseline) &&
      sha256File(file) === sha256File(file, baseline)
    );
    if (!protectedFilesIdentical)
      fail("A protected v1.15 runtime, economy, scene, or project-setting file changed.");

    const baselineAsmdefs = baselineFiles.filter((file) => file.endsWith(".asmdef"));
    existingAsmdefsIdentical = baselineAsmdefs.every((file) =>
      exists(file) && sha256File(file) === sha256File(file, baseline)
    );
    if (!existingAsmdefsIdentical)
      fail("An inherited assembly definition changed from the Step 2 baseline.");
    const newAsmdefs = asmdefFiles.filter((file) => !baselineAsmdefs.includes(file));
    if (!sameArray(newAsmdefs, [commercialAsmdefPath, testAsmdefPath].sort()))
      fail(`Unexpected new assembly definition(s): ${newAsmdefs.join(", ")}.`);

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
        .filter((line) => line && !/^(?:Added|Modified|Changed files|Files)\s*:?\s*$/i.test(line))
        .sort();
      changedManifestMatchesBaseline = sameArray(manifestLines, actualChangedFiles);
      if (!changedManifestMatchesBaseline) {
        const missing = actualChangedFiles.filter((file) => !manifestLines.includes(file));
        const extra = manifestLines.filter((file) => !actualChangedFiles.includes(file));
        fail(`Changed-file manifest mismatch; missing=[${missing.join(",")}], extra=[${extra.join(",")}].`);
      }
    }
  }
}

if (exists(staticReportPath)) {
  const report = parsedJson.get(staticReportPath);
  if (report) {
    const requiredFacts = new Map([
      ["version", VERSION],
      ["status", "PASS_STATIC_AND_BOUNDARY_VERIFICATION"],
      ["runtime_candidate_status", "DETACHED_PROVIDER_NEUTRAL_SANDBOX"],
      ["unity_execution", "PENDING_UNITY_EDITOR"],
      ["exact_unity_version", true],
      ["commercial_core_asmdef_valid", true],
      ["commercial_test_asmdef_valid", true],
      ["commercial_core_engine_runtime_save_network_access_absent", true],
      ["required_commercial_contract_types_present", true],
      ["monetization_verifier_provenance_boundary_present", true],
      ["economy_catalog_sha256", ECONOMY_SHA256],
      ["economy_catalog_bytes_unchanged", true],
      ["economy_proposal_only_non_authoritative", true],
      ["v1_15_activation_manifest_sha256", ACTIVATION_SHA256],
      ["v1_15_activation_manifest_bytes_unchanged", true],
      ["v1_15_activation_flags_preserved_false", true],
      ["packages_manifest_bytes_unchanged", true],
      ["remote_config_local_sha256", REMOTE_CONFIG_LOCAL_SHA256],
      ["remote_config_proposal_only_bound_and_all_disabled", true],
      ["save_version_unchanged", true],
      ["save_key_unchanged", true],
      ["csharp_delimiters_balanced", true],
      ["preprocessor_directives_balanced", true],
      ["required_final_artifacts_present", true]
    ]);
    for (const [key, expected] of requiredFacts) {
      if (!hasOwn(report, key))
        fail(`${staticReportPath} is missing required fact '${key}'.`);
      else if (report[key] !== expected)
        fail(`${staticReportPath} ${key} must equal ${JSON.stringify(expected)}.`);
    }
    const exactFields = new Map([
      ["project_json_files", jsonFiles.length],
      ["project_asmdef_files", asmdefFiles.length],
      ["csharp_structural_scan_files", csharpFiles.length],
      ["commercial_core_source_files", commercialSourceFiles.length],
      ["analytics_source_files", commercialDomainSourceCounts.Analytics],
      ["remote_config_source_files", commercialDomainSourceCounts.RemoteConfig],
      ["monetization_source_files", commercialDomainSourceCounts.Monetization],
      ["consent_source_files", commercialDomainSourceCounts.Consent],
      ["discovered_editmode_and_playmode_tests", discoveredTests],
      ["new_commercial_test_files", commercialTestFiles.length],
      ["new_commercial_tests", commercialTests],
      ["analytics_tests", commercialDomainTestCounts.Analytics],
      ["remote_config_tests", commercialDomainTestCounts.RemoteConfig],
      ["monetization_tests", commercialDomainTestCounts.Monetization],
      ["consent_tests", commercialDomainTestCounts.Consent],
      ["step16_editor_self_test_files", selfTestFiles.length],
      ["step16_editor_self_test_check_sites", selfTestCheckSites],
      ["current_project_png_files", currentPngs.length]
    ]);
    if (baseline) {
      exactFields.set("baseline_step02_contract_valid", baselineContractValid);
      exactFields.set("baseline_step02_verifier_passed", baselineVerifierPassed);
      exactFields.set("baseline_inherited_tests", baselineInheritedTests);
      exactFields.set("project_png_files_path_and_hash_identical_to_step02", approvedPngsIdentical);
      exactFields.set("protected_files_identical_to_step02", protectedFilesIdentical);
      exactFields.set("existing_asmdefs_identical_to_step02", existingAsmdefsIdentical);
      exactFields.set("actual_changed_paths", actualChangedFiles?.length ?? null);
      exactFields.set("changed_file_manifest_matches_step02_baseline", changedManifestMatchesBaseline);
    }
    for (const [key, expected] of exactFields) {
      if (!hasOwn(report, key))
        fail(`${staticReportPath} is missing exact field '${key}'.`);
      else if (report[key] !== expected)
        fail(`${staticReportPath} ${key} must equal ${JSON.stringify(expected)}.`);
    }
    if (!Array.isArray(report.failures) || report.failures.length !== 0)
      fail(`${staticReportPath} failures must be an empty array.`);
    if (!Array.isArray(report.static_failures) || report.static_failures.length !== 0)
      fail(`${staticReportPath} static_failures must be an empty array.`);
  }
}

const structuralFailures = failures.filter((failure) =>
  failure.includes("delimiters") || failure.includes("#if/#endif")
);
const commercialBoundaryFailures = failures.filter((failure) =>
  failure.includes("contains forbidden") ||
  failure.includes("embedded network endpoint") ||
  failure.includes("credential or production-provider identifier")
);
const requiredTypesPresent = !failures.some((failure) =>
  failure.includes("missing required contract type")
);
const passed = failures.length === 0;
const result = {
  version: VERSION,
  status: passed
    ? (sourceOnly
      ? "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION"
      : "PASS_STATIC_AND_BOUNDARY_VERIFICATION")
    : "FAIL_STATIC_AND_BOUNDARY_VERIFICATION",
  passed,
  runtime_candidate_status: "DETACHED_PROVIDER_NEUTRAL_SANDBOX",
  unity_execution: "PENDING_UNITY_EDITOR",
  exact_unity_version: exactUnityVersion,
  all_project_json_parses: !failures.some((failure) => failure.startsWith("JSON ")),
  project_json_files: jsonFiles.length,
  project_asmdef_files: asmdefFiles.length,
  csharp_structural_scan_files: csharpFiles.length,
  commercial_core_source_files: commercialSourceFiles.length,
  analytics_source_files: commercialDomainSourceCounts.Analytics,
  remote_config_source_files: commercialDomainSourceCounts.RemoteConfig,
  monetization_source_files: commercialDomainSourceCounts.Monetization,
  consent_source_files: commercialDomainSourceCounts.Consent,
  commercial_core_asmdef_valid: commercialAsmdefValid,
  commercial_test_asmdef_valid: testAsmdefValid,
  commercial_core_engine_runtime_save_network_access_absent:
    commercialBoundaryFailures.length === 0,
  required_commercial_contract_types_present: requiredTypesPresent,
  monetization_verifier_provenance_boundary_present:
    monetizationVerifierProvenanceBoundary,
  economy_catalog_sha256: economyHash,
  economy_catalog_bytes_unchanged: economyHash === ECONOMY_SHA256,
  economy_proposal_only_non_authoritative:
    economy?.proposalOnly === true && economy?.authoritative === false,
  v1_15_activation_manifest_sha256: activationHash,
  v1_15_activation_manifest_bytes_unchanged: activationHash === ACTIVATION_SHA256,
  v1_15_activation_flags_preserved_false: activationFlagsPreserved,
  packages_manifest_bytes_unchanged: packagesHash === PACKAGE_MANIFEST_SHA256,
  remote_config_local_sha256: remoteConfigHash,
  remote_config_proposal_only_bound_and_all_disabled: remoteConfigAllDisabled,
  save_version_unchanged: saveVersionPreserved,
  save_key_unchanged: saveKeyPreserved,
  csharp_delimiters_balanced:
    structuralFailures.every((failure) => !failure.includes("delimiters")),
  preprocessor_directives_balanced:
    structuralFailures.every((failure) => !failure.includes("#if/#endif")),
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_commercial_test_files: commercialTestFiles.length,
  new_commercial_tests: commercialTests,
  analytics_tests: commercialDomainTestCounts.Analytics,
  remote_config_tests: commercialDomainTestCounts.RemoteConfig,
  monetization_tests: commercialDomainTestCounts.Monetization,
  consent_tests: commercialDomainTestCounts.Consent,
  step16_editor_self_test_files: selfTestFiles.length,
  step16_editor_self_test_check_sites: selfTestCheckSites,
  current_project_png_files: currentPngs.length,
  baseline_supplied: baseline !== null,
  step01_baseline_supplied: step01Baseline !== null,
  baseline_step02_contract_valid: baseline ? baselineContractValid : null,
  baseline_step02_verifier_passed: baseline ? baselineVerifierPassed : null,
  baseline_inherited_tests: baseline ? baselineInheritedTests : null,
  project_png_files_path_and_hash_identical_to_step02:
    baseline ? approvedPngsIdentical : null,
  protected_files_identical_to_step02: baseline ? protectedFilesIdentical : null,
  existing_asmdefs_identical_to_step02: baseline ? existingAsmdefsIdentical : null,
  actual_changed_paths: baseline ? actualChangedFiles?.length ?? null : null,
  changed_file_manifest_present: exists(changedManifestPath),
  changed_file_manifest_matches_step02_baseline:
    baseline && exists(changedManifestPath) ? changedManifestMatchesBaseline : null,
  static_report_present: exists(staticReportPath),
  required_final_artifacts_present: required.every((file) => exists(file)),
  failures,
  static_failures: structuralFailures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (!passed) process.exitCode = 1;
