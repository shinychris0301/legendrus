import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { spawnSync } from "node:child_process";

const VERSION = "v1.16-step02";
const MILESTONE_ID =
  "v1.16-step02-provider-integration-runtime-wiring-activation-review";
const BASELINE_MILESTONE_ID =
  "v1.16-step01-commercial-adapters-sandbox-foundation";
const IMPLEMENTATION_STATUS =
  "IMPLEMENTED_IN_SOURCE_PENDING_PROVIDER_LEGAL_AND_ACTIVATION";
const RUNTIME_CANDIDATE_STATUS = "PREACTIVATION_DISABLED";
const UNITY_VERSION = "6000.5.9f1";
const NOT_RETAINED = "NOT_RETAINED";
const NOT_RECORDED = "NOT_RECORDED";

const ECONOMY_SHA256 =
  "ba66d7104c3d0a33ea4e0575b9965325cf9f90a8177c4e089c638d202267c42c";
const V115_ACTIVATION_SHA256 =
  "4c4a3d062257addf9e3bd50008a5bde281aa3e227bffe097982b7890bd597248";
const PACKAGE_MANIFEST_SHA256 =
  "f534b3f48018d7c02ddfb1b59588813562837db8167dda7395956065bf4f6713";
const REMOTE_CONFIG_SHA256 =
  "3d8c8c676217bd87e884fdeaa5663871cf54cf6daac23b6584fcbd62f44a8536";
const REMOTE_CONFIG_CONTENT_SHA256 =
  "3f4229158b3f5516a536ccf31954f417f8be45a64edd2e4cc33946c2d42e07eb";
const COMMERCIAL_CONFIG_SHA256 =
  "d5e7705c47f0a7368fa618db0740703e4e0628a2b99ab5318681933f00866c02";
const COMMERCIAL_ACTIVATION_SHA256 =
  "67f31982d6eafcc54f644f8ad1e81f672ddeefe3856da4eaae6b71af3bb753e9";
const BASELINE_VERIFIER_SHA256 =
  "14e40de7232b8b89ee757cac36a1d65b2514d1407b4b6aa9e9497fe7fc86c032";
const GAME_STATE_SHA256 =
  "94d3ac2c1e8fc432d8ba96b230b7498310a2b7752b1bfa3420beb907639a821f";
const SAVE_SERVICE_SHA256 =
  "3e8988e3a071fd7d96d9d73cc503bd38baa76c442c5a95043cea861df25a6d21";
const BASELINE_TREE_SHA256 =
  "4ec116b6e8cbf5327da431997de359b59bcd99e9f9806df8867a6ed5e2f138bd";
const BASELINE_FILE_COUNT = 396;

const root = process.cwd();
const failures = [];
const staticFailures = [];

function fail(message, isStructural = false) {
  if (!failures.includes(message)) failures.push(message);
  if (isStructural && !staticFailures.includes(message))
    staticFailures.push(message);
}

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

const sourceOnly = process.argv.includes("--source-only");
const baseline = option("--baseline");
const knownArguments = new Set(["--source-only", "--baseline"]);
for (let index = 2; index < process.argv.length; index++) {
  const argument = process.argv[index];
  if (argument === "--baseline") {
    index++;
    continue;
  }
  if (!knownArguments.has(argument)) fail(`Unknown argument '${argument}'.`);
}
if (sourceOnly && baseline)
  fail("Choose either --source-only or --baseline, not both.");
if (!sourceOnly && !baseline)
  fail("Release-boundary verification requires --baseline <v1.16-step01-project>.");

const generatedRootNames = new Set([
  ".git", "Library", "Temp", "Logs", "obj", "Build", "Builds", "UserSettings"
]);

function walkProject(directory, label, recordFailure = true) {
  const output = [];
  const caseFolded = new Map();
  function report(message) {
    if (recordFailure) fail(`${label}: ${message}`);
  }
  function visit(relative) {
    const full = path.join(directory, relative);
    let entries;
    try {
      entries = fs.readdirSync(full, { withFileTypes: true })
        .sort((left, right) => left.name.localeCompare(right.name));
    } catch (error) {
      report(`cannot enumerate '${relative || "."}': ${error.message}`);
      return;
    }
    for (const entry of entries) {
      const child = relative
        ? path.posix.join(relative, entry.name)
        : entry.name;
      if (!relative && generatedRootNames.has(entry.name)) {
        report(`generated or repository directory '${entry.name}' is not allowed.`);
        continue;
      }
      const childFull = path.join(directory, child);
      let stat;
      try {
        stat = fs.lstatSync(childFull);
      } catch (error) {
        report(`cannot inspect '${child}': ${error.message}`);
        continue;
      }
      if (stat.isSymbolicLink()) {
        report(`symbolic link '${child}' is not allowed.`);
        continue;
      }
      if (stat.isDirectory()) {
        visit(child);
        continue;
      }
      if (!stat.isFile()) {
        report(`special filesystem entry '${child}' is not allowed.`);
        continue;
      }
      const folded = child.toLocaleLowerCase("en-US");
      if (caseFolded.has(folded) && caseFolded.get(folded) !== child) {
        report(
          `case-fold collision '${caseFolded.get(folded)}' and '${child}'.`
        );
      } else {
        caseFolded.set(folded, child);
      }
      output.push(child);
    }
  }
  visit("");
  return output.sort();
}

function absolute(relative, directory = root) {
  return path.join(directory, relative);
}

function exists(relative, directory = root) {
  return fs.existsSync(absolute(relative, directory));
}

function read(relative, directory = root) {
  return fs.readFileSync(absolute(relative, directory), "utf8");
}

function sha256Buffer(buffer) {
  return crypto.createHash("sha256").update(buffer).digest("hex");
}

function sha256File(relative, directory = root) {
  return sha256Buffer(fs.readFileSync(absolute(relative, directory)));
}

function treeFingerprint(directory, projectFiles) {
  const hash = crypto.createHash("sha256");
  for (const file of projectFiles) {
    hash.update(file);
    hash.update("\0");
    hash.update(sha256File(file, directory));
    hash.update("\n");
  }
  return hash.digest("hex");
}

function sameArray(left, right) {
  return Array.isArray(left) &&
    left.length === right.length &&
    left.every((item, index) => item === right[index]);
}

function sameStringSet(left, right) {
  return Array.isArray(left) &&
    left.every((item) => typeof item === "string") &&
    sameArray([...left].sort(), [...right].sort());
}

function isObject(value) {
  return value !== null && typeof value === "object" && !Array.isArray(value);
}

function hasOwn(value, key) {
  return Object.prototype.hasOwnProperty.call(value, key);
}

function exactKeys(value, expected, description) {
  if (!isObject(value)) {
    fail(`${description} must be a JSON object.`);
    return false;
  }
  const actual = Object.keys(value).sort();
  const wanted = [...expected].sort();
  if (!sameArray(actual, wanted)) {
    const missing = wanted.filter((key) => !actual.includes(key));
    const extra = actual.filter((key) => !wanted.includes(key));
    fail(
      `${description} keys differ; missing=[${missing.join(",")}], ` +
      `extra=[${extra.join(",")}].`
    );
    return false;
  }
  return true;
}

// JSON.parse accepts duplicate object keys. This small recursive scanner records
// them before JSON.parse so an activation or evidence field cannot be shadowed.
function duplicateJsonKeys(source) {
  let index = 0;
  const duplicates = [];
  function whitespace() {
    while (/\s/.test(source[index] ?? "")) index++;
  }
  function stringToken() {
    const start = index;
    if (source[index] !== '"') throw new Error(`expected string at ${index}`);
    index++;
    while (index < source.length) {
      if (source[index] === "\\") {
        index += 2;
      } else if (source[index] === '"') {
        index++;
        return JSON.parse(source.slice(start, index));
      } else {
        index++;
      }
    }
    throw new Error(`unterminated string at ${start}`);
  }
  function value(location) {
    whitespace();
    const char = source[index];
    if (char === "{") return object(location);
    if (char === "[") return array(location);
    if (char === '"') {
      stringToken();
      return;
    }
    const match = source.slice(index).match(
      /^(?:-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?|true|false|null)/
    );
    if (!match) throw new Error(`invalid token at ${index}`);
    index += match[0].length;
  }
  function object(location) {
    index++;
    whitespace();
    const seen = new Set();
    if (source[index] === "}") {
      index++;
      return;
    }
    while (index < source.length) {
      whitespace();
      const key = stringToken();
      const keyLocation = location ? `${location}.${key}` : key;
      if (seen.has(key)) duplicates.push(keyLocation);
      seen.add(key);
      whitespace();
      if (source[index] !== ":") throw new Error(`expected ':' at ${index}`);
      index++;
      value(keyLocation);
      whitespace();
      if (source[index] === "}") {
        index++;
        return;
      }
      if (source[index] !== ",") throw new Error(`expected ',' at ${index}`);
      index++;
    }
    throw new Error("unterminated object");
  }
  function array(location) {
    index++;
    whitespace();
    let item = 0;
    if (source[index] === "]") {
      index++;
      return;
    }
    while (index < source.length) {
      value(`${location}[${item}]`);
      item++;
      whitespace();
      if (source[index] === "]") {
        index++;
        return;
      }
      if (source[index] !== ",") throw new Error(`expected ',' at ${index}`);
      index++;
    }
    throw new Error("unterminated array");
  }
  value("$");
  whitespace();
  if (index !== source.length) throw new Error(`trailing data at ${index}`);
  return duplicates;
}

function parseJson(relative, directory = root, recordFailure = true) {
  try {
    const source = read(relative, directory);
    const duplicates = duplicateJsonKeys(source);
    if (duplicates.length > 0) {
      if (recordFailure)
        fail(`JSON ${relative} has duplicate key(s): ${duplicates.join(", ")}.`);
      return null;
    }
    return JSON.parse(source);
  } catch (error) {
    if (recordFailure) fail(`JSON ${relative}: ${error.message}`);
    return null;
  }
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
    } else {
      output += char === "\n" ? "\n" : " ";
    }
  }
  return output;
}

function methodBlock(source, methodName) {
  const scrubbed = scrubCSharp(source);
  const nameIndex = scrubbed.search(new RegExp(`\\b${methodName}\\s*\\(`));
  if (nameIndex < 0) return "";
  const brace = scrubbed.indexOf("{", nameIndex);
  if (brace < 0) return "";
  let depth = 0;
  for (let index = brace; index < scrubbed.length; index++) {
    if (scrubbed[index] === "{") depth++;
    else if (scrubbed[index] === "}") {
      depth--;
      if (depth === 0) return scrubbed.slice(brace, index + 1);
    }
  }
  return "";
}

const milestonePath = "MILESTONE_VERSION.txt";
const projectVersionPath = "ProjectSettings/ProjectVersion.txt";
const packagesPath = "Packages/manifest.json";
const economyPath = "Assets/LegendsOfRus/Resources/Config/economy.json";
const v115ActivationPath =
  "Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json";
const remoteConfigPath =
  "Assets/LegendsOfRus/Resources/Config/remote_config_local.json";
const commercialConfigPath =
  "Assets/LegendsOfRus/Resources/Config/commercial_runtime_config.json";
const commercialActivationPath =
  "Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json";
const commercialCoreRoot = "Assets/LegendsOfRus/Scripts/CommercialCore";
const commercialRuntimeRoot = "Assets/LegendsOfRus/Scripts/CommercialRuntime";
const runtimeRoot = "Assets/LegendsOfRus/Scripts/Runtime";
const commercialRuntimeAsmdefPath =
  `${commercialRuntimeRoot}/LegendsOfRus.CommercialRuntime.asmdef`;
const runtimeAsmdefPath = `${runtimeRoot}/LegendsOfRus.Runtime.asmdef`;
const editTestRoot = "Assets/LegendsOfRus/Tests/CommercialRuntimeEditMode";
const playTestRoot = "Assets/LegendsOfRus/Tests/CommercialRuntimePlayMode";
const editTestAsmdefPath =
  `${editTestRoot}/LegendsOfRus.CommercialRuntime.Tests.asmdef`;
const playTestAsmdefPath =
  `${playTestRoot}/LegendsOfRus.CommercialRuntime.PlayModeTests.asmdef`;
const selfTestPath =
  "Assets/LegendsOfRus/Scripts/Editor/Step16ProviderIntegrationRuntimeWiringSelfTest.cs";
const changedManifestPath =
  "Assets/LegendsOfRus/Documentation/V1_16_STEP02_CHANGED_FILES.txt";
const staticReportPath =
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_16_STEP02.json";
const evidencePath =
  "Assets/LegendsOfRus/Documentation/V1_16_STEP02_ACTIVATION_EVIDENCE.json";
const implementationStatusPath =
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_16_STEP02.json";

const required = [
  "README.md",
  "README_V1_16_STEP02.md",
  milestonePath,
  projectVersionPath,
  packagesPath,
  economyPath,
  v115ActivationPath,
  remoteConfigPath,
  commercialConfigPath,
  commercialActivationPath,
  commercialRuntimeAsmdefPath,
  runtimeAsmdefPath,
  editTestAsmdefPath,
  playTestAsmdefPath,
  selfTestPath,
  "Assets/LegendsOfRus/Documentation/V1_16_STEP02_PROVIDER_INTEGRATION_RUNTIME_WIRING_ACTIVATION_REVIEW.md",
  implementationStatusPath,
  evidencePath,
  changedManifestPath,
  staticReportPath,
  "Tools/verify_v1_16_step01.mjs",
  "Tools/verify_v1_16_step02.mjs"
];

const projectFiles = walkProject(root, "current project");
const projectFileSet = new Set(projectFiles);
for (const file of required)
  if (!projectFileSet.has(file)) fail(`Required Step 2 artifact is missing: ${file}.`);

if (exists(milestonePath) && read(milestonePath).trim() !== MILESTONE_ID)
  fail(`${milestonePath} must equal '${MILESTONE_ID}'.`);
const exactUnityVersion = exists(projectVersionPath) &&
  read(projectVersionPath).trim() === `m_EditorVersion: ${UNITY_VERSION}`;
if (!exactUnityVersion)
  fail(`${projectVersionPath} must contain exactly 'm_EditorVersion: ${UNITY_VERSION}'.`);

const jsonFiles = projectFiles.filter((file) => file.endsWith(".json"));
const parsedJson = new Map();
for (const file of jsonFiles) parsedJson.set(file, parseJson(file));
const asmdefFiles = projectFiles.filter((file) => file.endsWith(".asmdef"));
const parsedAsmdefs = new Map();
for (const file of asmdefFiles) parsedAsmdefs.set(file, parseJson(file));

function pinnedHash(relative, expected, description) {
  const actual = exists(relative) ? sha256File(relative) : null;
  if (actual !== expected)
    fail(`${description} must retain SHA-256 ${expected}; found ${actual}.`);
  return actual;
}

const economyHash = pinnedHash(
  economyPath,
  ECONOMY_SHA256,
  "Economy catalog"
);
const v115ActivationHash = pinnedHash(
  v115ActivationPath,
  V115_ACTIVATION_SHA256,
  "v1.15 runtime-economy activation manifest"
);
const packagesHash = pinnedHash(
  packagesPath,
  PACKAGE_MANIFEST_SHA256,
  "Package manifest"
);
const remoteConfigHash = pinnedHash(
  remoteConfigPath,
  REMOTE_CONFIG_SHA256,
  "Bundled remote-config source"
);
const commercialConfigHash = pinnedHash(
  commercialConfigPath,
  COMMERCIAL_CONFIG_SHA256,
  "Commercial runtime config"
);
const commercialActivationHash = pinnedHash(
  commercialActivationPath,
  COMMERCIAL_ACTIVATION_SHA256,
  "Commercial runtime activation manifest"
);
const inheritedStep1VerifierHash = pinnedHash(
  "Tools/verify_v1_16_step01.mjs",
  BASELINE_VERIFIER_SHA256,
  "Inherited Step 1 verifier"
);

const economy = parsedJson.get(economyPath);
if (
  !economy ||
  economy.catalogId !== "v1.15-step01-proposal" ||
  economy.proposalOnly !== true ||
  economy.authoritative !== false
) {
  fail("The inherited economy catalog must remain proposal-only and non-authoritative.");
}

const v115Activation = parsedJson.get(v115ActivationPath);
const v115PendingKeys = [
  "compilationVerified",
  "editorSelfTestsPassed",
  "editModeTestsPassed",
  "playModeTestsPassed",
  "runtimeActivationApproved"
];
const v115ActivationInactive = Boolean(v115Activation) &&
  v115Activation.schemaVersion === 1 &&
  v115Activation.manifestId === "v1.15-step02-runtime-economy-activation" &&
  v115Activation.productApproval === true &&
  v115PendingKeys.every((key) => v115Activation[key] === false);
if (!v115ActivationInactive)
  fail("The v1.15 runtime economy must remain unverified and inactive.");

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
const remoteConfig = parsedJson.get(remoteConfigPath);
const remoteConfigAllDisabled = Boolean(remoteConfig) &&
  exactKeys(
    remoteConfig,
    [
      "schemaVersion", "snapshotId", "revision", "environment",
      "proposalOnly", "authoritative", "economyCatalogId",
      "economyCatalogSourceSha256", "entries"
    ],
    "Bundled remote config"
  ) &&
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
  remoteConfig.entries.every((entry, index) =>
    exactKeys(entry, ["key", "type", "value"], `Remote config entry ${index}`) &&
    entry.type === "bool" && entry.value === "false"
  );
if (!remoteConfigAllDisabled)
  fail("The bundled remote config must remain exact, proposal-only, and all-disabled.");

const commercialConfigKeys = [
  "schemaVersion", "configId", "environment", "consentProviderId",
  "analyticsProviderId", "iapProviderId", "rewardedAdProviderId",
  "analyticsCaptureAllowed", "analyticsUploadAllowed", "iapAllowed",
  "rewardedAdsAllowed", "developmentSandboxAllowed"
];
const commercialConfigFlags = [
  "analyticsCaptureAllowed", "analyticsUploadAllowed", "iapAllowed",
  "rewardedAdsAllowed", "developmentSandboxAllowed"
];
const commercialProviderKeys = [
  "consentProviderId", "analyticsProviderId", "iapProviderId",
  "rewardedAdProviderId"
];
const commercialConfig = parsedJson.get(commercialConfigPath);
const commercialConfigAllDisabled = Boolean(commercialConfig) &&
  exactKeys(commercialConfig, commercialConfigKeys, "Commercial runtime config") &&
  commercialConfig.schemaVersion === 1 &&
  commercialConfig.configId === "v1.16-step02-disabled-composition" &&
  commercialConfig.environment === "disabled" &&
  commercialProviderKeys.every((key) => commercialConfig[key] === "disabled") &&
  commercialConfigFlags.every((key) => commercialConfig[key] === false);
if (!commercialConfigAllDisabled)
  fail("The commercial runtime config must be exact and all-disabled.");

const commercialActivationKeys = [
  "schemaVersion", "manifestId", "unityVersion", "environment",
  "runtimeConfigId", "runtimeConfigSourceSha256", "economyCatalogId",
  "economyCatalogSourceSha256", "remoteConfigSnapshotId",
  "remoteConfigRevision", "remoteConfigSourceSha256",
  "remoteConfigContentSha256", "v115RuntimeEconomyApproved",
  "v116Step1CompilationVerified", "v116Step1ProjectValidatorPassed",
  "v116Step1EditorSelfTestsPassed", "v116Step1EditModeTestsPassed",
  "v116Step1PlayModeTestsPassed", "providerSelectionApproved",
  "legalPrivacyApproved", "consentFlowApproved",
  "productIdentifiersApproved", "rewardedPlacementsApproved",
  "regionalBehaviorApproved", "securityReviewPassed",
  "representativeDeviceFailurePathsPassed", "saveMigrationApproved",
  "durableFulfillmentApproved", "analyticsActivationApproved",
  "iapActivationApproved", "rewardedAdsActivationApproved",
  "finalRuntimeActivationApproved"
];
const commercialActivationFlagKeys = commercialActivationKeys.filter((key) =>
  /(?:Approved|Verified|Passed)$/.test(key)
);
const commercialActivation = parsedJson.get(commercialActivationPath);
const commercialActivationAllFalse = Boolean(commercialActivation) &&
  exactKeys(
    commercialActivation,
    commercialActivationKeys,
    "Commercial runtime activation manifest"
  ) &&
  commercialActivation.schemaVersion === 1 &&
  commercialActivation.manifestId ===
    "v1.16-step02-commercial-runtime-preactivation" &&
  commercialActivation.unityVersion === UNITY_VERSION &&
  commercialActivation.environment === "disabled" &&
  commercialActivation.runtimeConfigId ===
    "v1.16-step02-disabled-composition" &&
  commercialActivation.runtimeConfigSourceSha256 ===
    COMMERCIAL_CONFIG_SHA256 &&
  commercialActivation.economyCatalogId === "v1.15-step01-proposal" &&
  commercialActivation.economyCatalogSourceSha256 === ECONOMY_SHA256 &&
  commercialActivation.remoteConfigSnapshotId ===
    "v1.16-step01-local-defaults" &&
  commercialActivation.remoteConfigRevision === 1 &&
  commercialActivation.remoteConfigSourceSha256 === REMOTE_CONFIG_SHA256 &&
  commercialActivation.remoteConfigContentSha256 ===
    REMOTE_CONFIG_CONTENT_SHA256 &&
  commercialActivationFlagKeys.every((key) =>
    commercialActivation[key] === false
  );
if (!commercialActivationAllFalse)
  fail("Every Step 2 commercial verification, approval, capability, and activation flag must remain false.");

const nativeExtensions = new Set([
  ".dll", ".aar", ".jar", ".so", ".dylib", ".bundle", ".framework",
  ".a", ".lib", ".winmd", ".wasm"
]);
const nativeOrSdkArtifacts = projectFiles.filter((file) =>
  nativeExtensions.has(path.posix.extname(file).toLowerCase()) ||
  file.startsWith("Assets/Plugins/") ||
  file.endsWith(".asmref") ||
  (file.startsWith("Packages/") && file !== packagesPath)
);
if (nativeOrSdkArtifacts.length > 0)
  fail(`SDK, native, asmref, or embedded-package artifacts are forbidden: ${nativeOrSdkArtifacts.join(", ")}.`);

function validateAsmdef(
  relative,
  name,
  namespaceName,
  references,
  includePlatforms,
  noEngineReferences,
  description
) {
  const asmdef = parsedAsmdefs.get(relative);
  const keys = [
    "name", "rootNamespace", "references", "includePlatforms",
    "excludePlatforms", "allowUnsafeCode", "overrideReferences",
    "precompiledReferences", "autoReferenced", "defineConstraints",
    "versionDefines", "noEngineReferences"
  ];
  if (relative.includes("/Tests/")) keys.push("optionalUnityReferences");
  const valid = Boolean(asmdef) &&
    exactKeys(asmdef, keys, description) &&
    asmdef.name === name &&
    asmdef.rootNamespace === namespaceName &&
    sameStringSet(asmdef.references, references) &&
    sameArray(asmdef.includePlatforms, includePlatforms) &&
    sameArray(asmdef.excludePlatforms, []) &&
    asmdef.allowUnsafeCode === false &&
    asmdef.overrideReferences === false &&
    sameArray(asmdef.precompiledReferences, []) &&
    asmdef.autoReferenced === true &&
    sameArray(asmdef.defineConstraints, []) &&
    sameArray(asmdef.versionDefines, []) &&
    asmdef.noEngineReferences === noEngineReferences &&
    (!relative.includes("/Tests/") ||
      sameArray(asmdef.optionalUnityReferences, ["TestAssemblies"]));
  if (!valid) fail(`${description} does not match its reviewed assembly boundary.`);
  return valid;
}

const commercialRuntimeAsmdefValid = validateAsmdef(
  commercialRuntimeAsmdefPath,
  "LegendsOfRus.CommercialRuntime",
  "LegendsOfRus.CommercialRuntime",
  [
    "LegendsOfRus.CommercialCore", "LegendsOfRus.EconomyCore",
    "LegendsOfRus.Runtime", "LegendsOfRus.RuntimeEconomy"
  ],
  [],
  false,
  "CommercialRuntime asmdef"
);
const runtimeAsmdefValid = validateAsmdef(
  runtimeAsmdefPath,
  "LegendsOfRus.Runtime",
  "LegendsOfRus",
  [
    "LegendsOfRus.GameplayCore", "LegendsOfRus.EconomyCore",
    "LegendsOfRus.CommercialCore", "Unity.InputSystem", "Unity.ugui"
  ],
  [],
  false,
  "Runtime asmdef"
);
const editTestAsmdefValid = validateAsmdef(
  editTestAsmdefPath,
  "LegendsOfRus.CommercialRuntime.Tests",
  "LegendsOfRus.Tests.CommercialRuntime",
  [
    "LegendsOfRus.CommercialRuntime", "LegendsOfRus.CommercialCore",
    "LegendsOfRus.Runtime", "LegendsOfRus.RuntimeEconomy",
    "LegendsOfRus.EconomyCore"
  ],
  ["Editor"],
  false,
  "CommercialRuntime EditMode test asmdef"
);
const playTestAsmdefValid = validateAsmdef(
  playTestAsmdefPath,
  "LegendsOfRus.CommercialRuntime.PlayModeTests",
  "LegendsOfRus.Tests.CommercialRuntime.PlayMode",
  [
    "LegendsOfRus.CommercialRuntime", "LegendsOfRus.CommercialCore",
    "LegendsOfRus.Runtime", "LegendsOfRus.RuntimeEconomy",
    "LegendsOfRus.EconomyCore", "Unity.ugui"
  ],
  [],
  false,
  "CommercialRuntime PlayMode test asmdef"
);

const assemblyNames = new Map();
for (const [file, asmdef] of parsedAsmdefs) {
  if (!asmdef || typeof asmdef.name !== "string") continue;
  if (assemblyNames.has(asmdef.name))
    fail(`Duplicate assembly name '${asmdef.name}' in ${assemblyNames.get(asmdef.name)} and ${file}.`);
  assemblyNames.set(asmdef.name, file);
}
const assemblyEdges = new Map();
for (const [file, asmdef] of parsedAsmdefs) {
  if (!asmdef || typeof asmdef.name !== "string") continue;
  assemblyEdges.set(
    asmdef.name,
    (Array.isArray(asmdef.references) ? asmdef.references : [])
      .filter((reference) => assemblyNames.has(reference))
  );
  if (Array.isArray(asmdef.precompiledReferences) &&
      asmdef.precompiledReferences.length > 0)
    fail(`${file} must not add precompiled references.`);
}
const assemblyState = new Map();
let assemblyGraphAcyclic = true;
function visitAssembly(name, stack) {
  if (assemblyState.get(name) === 2) return;
  if (assemblyState.get(name) === 1) {
    assemblyGraphAcyclic = false;
    fail(`Assembly dependency cycle: ${[...stack, name].join(" -> ")}.`);
    return;
  }
  assemblyState.set(name, 1);
  for (const dependency of assemblyEdges.get(name) ?? [])
    visitAssembly(dependency, [...stack, name]);
  assemblyState.set(name, 2);
}
for (const name of assemblyEdges.keys()) visitAssembly(name, []);

const csharpFiles = projectFiles.filter((file) => file.endsWith(".cs"));
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
    if (depth !== 0)
      fail(`${file} has unbalanced ${open}${close} delimiters.`, true);
  }
  let preprocessorDepth = 0;
  for (const line of source.split(/\r?\n/)) {
    if (/^\s*#if\b/.test(line)) preprocessorDepth++;
    if (/^\s*#endif\b/.test(line)) preprocessorDepth--;
    if (preprocessorDepth < 0) break;
  }
  if (preprocessorDepth !== 0)
    fail(`${file} has unmatched #if/#endif directives.`, true);
}

const commercialRuntimeSources = csharpFiles.filter((file) =>
  file.startsWith(`${commercialRuntimeRoot}/`)
);
if (commercialRuntimeSources.length < 10)
  fail("CommercialRuntime source boundary is incomplete.");

const combinedCommercialRuntimeSource = commercialRuntimeSources
  .map((file) => read(file))
  .join("\n");
const combinedCommercialRuntimeCode = commercialRuntimeSources
  .map((file) => scrubCSharp(read(file)))
  .join("\n");
const requiredCommercialRuntimeTypes = [
  "CommercialRuntimeActivationGate",
  "CommercialRuntimeComposition",
  "CommercialRuntimeBootstrap",
  "CommercialRuntimeConfigLoader",
  "RuntimeRemoteConfigCache",
  "CommercialFulfillmentCoordinator",
  "CommercialFulfillmentCandidate",
  "UnavailableCommercialAtomicFulfillmentStore"
];
for (const typeName of requiredCommercialRuntimeTypes) {
  if (!new RegExp(`\\b${typeName}\\b`).test(combinedCommercialRuntimeCode))
    fail(`CommercialRuntime is missing required type ${typeName}.`);
}

const forbiddenCommercialRuntimeCode = [
  ["live GameState access", /\bGameState\b/],
  ["SaveService access", /\bSaveService\b/],
  ["PlayerPrefs access", /\bPlayerPrefs\b/],
  ["file persistence", /\bSystem\s*\.\s*IO\b|\b(?:File|Directory|FileStream|StreamReader|StreamWriter)\s*\./],
  ["network API", /\bSystem\s*\.\s*Net\b|\b(?:HttpClient|UnityWebRequest|Socket|WebRequest|Dns)\b/],
  ["entitlement completion", /\.\s*Complete\s*\(/],
  ["direct live economy grant", /\b(?:AddGold|AddGems|AddEnergy|RefillEnergy|ConsumeBooster)\s*\(/],
  ["development adapter binding", /\b(?:ScriptedDevelopment\w*|InMemoryDevelopment\w*|DevelopmentPurchaseVerifier|DevelopmentRewardedAdVerifier)\b/],
  ["production SDK namespace", /\b(?:Firebase|AppsFlyer|AppLovin|IronSource|LevelPlay|AdMob|UnityAds|GooglePlay|AppleStore|FacebookSDK|Unity\.Services)\b/]
];
for (const [description, pattern] of forbiddenCommercialRuntimeCode) {
  if (pattern.test(combinedCommercialRuntimeCode))
    fail(`CommercialRuntime contains forbidden ${description}.`);
}
if (/https?:\/\//i.test(combinedCommercialRuntimeSource))
  fail("CommercialRuntime contains an embedded network endpoint.");
if (/\b(?:api[_-]?key|client[_-]?secret|private[_-]?key|ad[_-]?unit[_-]?id|store[_-]?credential)\b/i.test(
  combinedCommercialRuntimeSource
)) {
  fail("CommercialRuntime contains a credential or live-provider identifier token.");
}
if (/\.\s*(?:ReceiptPayload|ProofPayload)\b/.test(combinedCommercialRuntimeCode))
  fail("CommercialRuntime reads raw receipt or rewarded-proof payload data.");
if (/\b(?:50|120|250|500|3000|10000)\b/.test(combinedCommercialRuntimeCode))
  fail("CommercialRuntime contains an inherited development grant amount.");

const activationSourcePath =
  `${commercialRuntimeRoot}/Activation/CommercialRuntimeActivation.cs`;
const activationRawSource = exists(activationSourcePath)
  ? read(activationSourcePath)
  : "";
const activationSource = exists(activationSourcePath)
  ? scrubCSharp(activationRawSource)
  : "";
const externalProviderActivationCompiledFalse =
  /private\s+const\s+bool\s+ExternalProviderActivationCompiled\s*=\s*false\s*;/.test(
    activationSource
  ) &&
  new RegExp(
    `ExpectedManifestSourceSha256\\s*=\\s*[^;]*${COMMERCIAL_ACTIVATION_SHA256}`
  ).test(activationRawSource);
if (!externalProviderActivationCompiledFalse)
  fail("The external-provider activation compile boundary must remain private const false and hash-bound.");

const configSourcePath =
  `${commercialRuntimeRoot}/Config/CommercialRuntimeConfig.cs`;
const remoteCacheSourcePath =
  `${commercialRuntimeRoot}/Config/RuntimeRemoteConfigCache.cs`;
if (
  !exists(configSourcePath) ||
  !read(configSourcePath).includes(COMMERCIAL_CONFIG_SHA256)
) fail("CommercialRuntimeConfigLoader is not bound to the reviewed config hash.");
if (
  !exists(remoteCacheSourcePath) ||
  !read(remoteCacheSourcePath).includes(REMOTE_CONFIG_CONTENT_SHA256)
) fail("RuntimeRemoteConfigCache is not bound to the reviewed content hash.");

const fulfillmentCoordinatorPath =
  `${commercialRuntimeRoot}/Fulfillment/CommercialFulfillmentCoordinator.cs`;
const fulfillmentContractsPath =
  `${commercialRuntimeRoot}/Fulfillment/CommercialFulfillmentContracts.cs`;
const recipeCatalogPath =
  `${commercialRuntimeRoot}/Fulfillment/CommercialEntitlementRecipeCatalog.cs`;
const fulfillmentCoordinatorCode = exists(fulfillmentCoordinatorPath)
  ? scrubCSharp(read(fulfillmentCoordinatorPath))
  : "";
const fulfillmentContractsCode = exists(fulfillmentContractsPath)
  ? scrubCSharp(read(fulfillmentContractsPath))
  : "";
const recipeCatalogCode = exists(recipeCatalogPath)
  ? scrubCSharp(read(recipeCatalogPath))
  : "";
const candidateOnlyFulfillmentBoundaryPresent =
  /public\s+bool\s+IsPersisted\s*\{\s*get\s*\{\s*return\s+false\s*;/.test(
    fulfillmentContractsCode
  ) &&
  /public\s+bool\s+IsEntitlementCompleted\s*\{\s*get\s*\{\s*return\s+false\s*;/.test(
    fulfillmentContractsCode
  ) &&
  /\bCommitted\s*=\s*false\s*;/.test(fulfillmentContractsCode) &&
  /CommercialAtomicCommitResult\.Rejected\s*\(/.test(
    fulfillmentContractsCode
  ) &&
  /TryDoubleLastReward\s*\(/.test(fulfillmentCoordinatorCode) &&
  !/\.\s*Complete\s*\(/.test(fulfillmentCoordinatorCode) &&
  /RecipeUnavailable/.test(recipeCatalogCode) &&
  /entitlementGate\.Release\s*\(/.test(fulfillmentCoordinatorCode);
if (!candidateOnlyFulfillmentBoundaryPresent)
  fail("Candidate-only fulfillment and unavailable atomic-store boundaries are incomplete.");

const monetizationServicePath =
  `${runtimeRoot}/Services/MonetizationService.cs`;
const monetizationSource = exists(monetizationServicePath)
  ? read(monetizationServicePath)
  : "";
const monetizationCode = scrubCSharp(monetizationSource);
const trustedMonetizationBoundaryPresent =
  /internal\s+interface\s+IRuntimeMonetizationPort\b/.test(monetizationCode) &&
  /internal\s+bool\s+TryBindRuntimePort\s*\(/.test(monetizationCode) &&
  !/public\s+[^;{}]*TryBindRuntimePort\s*\(/.test(monetizationCode) &&
  /internal\s+RuntimeMonetizationActionResult\s*\(/.test(monetizationCode) &&
  /internal\s+static\s+RuntimeMonetizationActionResult\s+Committed\s*\(/.test(
    monetizationCode
  ) &&
  /internal\s+static\s+RuntimeMonetizationActionResult\s+CandidateOnly\s*\(/.test(
    monetizationCode
  );
if (!trustedMonetizationBoundaryPresent)
  fail("Runtime monetization binding or committed-result construction is publicly forgeable.");
const forbiddenMonetizationMutations =
  /\b(?:GameState|SaveService|PlayerPrefs)\b|\b(?:AddGold|AddGems|AddEnergy|RefillEnergy|TryDoubleLastLevelGoldReward)\s*\(/;
if (forbiddenMonetizationMutations.test(monetizationCode))
  fail("MonetizationService contains a direct live-state or economy mutation path.");
if (/\b(?:50|120|250|500|3000|10000)\b/.test(monetizationCode))
  fail("MonetizationService retains an inherited development grant amount.");

const productionCsharpFiles = csharpFiles.filter((file) =>
  file.startsWith(`${runtimeRoot}/`) ||
  file.startsWith(`${commercialRuntimeRoot}/`)
);
const productionCsharpCode = productionCsharpFiles
  .map((file) => scrubCSharp(read(file)))
  .join("\n");
const monetizationBindCalls =
  productionCsharpCode.match(/\.\s*TryBindRuntimePort\s*\(/g)?.length ?? 0;
const committedFactoryCalls =
  productionCsharpCode.match(
    /RuntimeMonetizationActionResult\s*\.\s*Committed\s*\(/g
  )?.length ?? 0;
const runtimePortImplementations = productionCsharpFiles.filter((file) =>
  /:\s*IRuntimeMonetizationPort\b/.test(scrubCSharp(read(file)))
);
const noRuntimeCommercialPortBound =
  monetizationBindCalls === 0 &&
  committedFactoryCalls === 0 &&
  sameArray(runtimePortImplementations, [monetizationServicePath]);
if (!noRuntimeCommercialPortBound) {
  fail(
    "No production source may bind the internal monetization port, call the " +
    "committed-result factory, or add a second port implementation in this preactivation build."
  );
}
if (/\.\s*(?:ReceiptPayload|ProofPayload)\b/.test(productionCsharpCode))
  fail("Production runtime source reads a raw receipt or rewarded-proof payload.");

const screenRouterPath = `${runtimeRoot}/UI/ScreenRouter.cs`;
const screenRouterSource = exists(screenRouterPath) ? read(screenRouterPath) : "";
for (const methodName of [
  "RequestPurchase", "RequestRewardedDouble", "RequestRewardedEnergy"
]) {
  const block = methodBlock(screenRouterSource, methodName);
  if (!block) {
    fail(`ScreenRouter is missing reviewed commercial handler ${methodName}.`);
  } else if (forbiddenMonetizationMutations.test(block)) {
    fail(`ScreenRouter.${methodName} directly mutates live economy or save state.`);
  }
}

const analyticsServicePath = `${runtimeRoot}/Services/AnalyticsService.cs`;
const analyticsBridgePath = `${runtimeRoot}/Commercial/RuntimeAnalyticsBridge.cs`;
const analyticsFunnelPath =
  `${commercialCoreRoot}/Analytics/AnalyticsFunnelTracker.cs`;
const analyticsRuntimeSource = [analyticsServicePath, analyticsBridgePath]
  .filter((file) => exists(file))
  .map((file) => read(file))
  .join("\n");
const analyticsRuntimeCode = scrubCSharp(analyticsRuntimeSource);
if (/\bAnalyticsService\s*\.\s*Event\s*\(/.test(analyticsRuntimeCode))
  fail("The runtime analytics facade exposes a free-form event path.");
if (/\[ANALYTICS\]/.test(analyticsRuntimeSource))
  fail("The runtime analytics path contains the inherited free-form analytics log.");
if (/\b(?:System\s*\.\s*Net|UnityWebRequest|HttpClient|PlayerPrefs|SaveService)\b/.test(
  analyticsRuntimeCode
)) fail("Runtime analytics contains upload or persistence access.");
const funnelCode = exists(analyticsFunnelPath)
  ? scrubCSharp(read(analyticsFunnelPath))
  : "";
const atomicAnalyticsFunnelPresent =
  /TryBeginLevelAttempt\s*\(\s*string\s+attemptKey\s*,\s*Func<AnalyticsRecordResult>\s+tryRecord\s*\)/.test(
    funnelCode
  ) &&
  /TryAcceptTerminal\s*\(\s*string\s+attemptKey\s*,[\s\S]{0,180}Func<AnalyticsRecordResult>\s+tryRecord\s*\)/.test(
    funnelCode
  ) &&
  /funnel\.TryBeginLevelAttempt\s*\([\s\S]{0,180}\(\)\s*=>\s*pipeline\.TryRecord/.test(
    analyticsRuntimeCode
  ) &&
  /funnel\.TryAcceptTerminal\s*\([\s\S]{0,260}\(\)\s*=>\s*pipeline\.TryRecord/.test(
    analyticsRuntimeCode
  );
if (!atomicAnalyticsFunnelPresent)
  fail("Runtime analytics does not use callback-atomic funnel recording.");

const runtimeBootstrapPath = `${runtimeRoot}/Core/RuntimeBootstrap.cs`;
const runtimeBootstrapCode = exists(runtimeBootstrapPath)
  ? scrubCSharp(read(runtimeBootstrapPath))
  : "";
const sessionIndex = runtimeBootstrapCode.indexOf("AnalyticsService.TryBeginSession");
const earlyReturnIndex = runtimeBootstrapCode.indexOf("if (existingRouter != null)");
if (sessionIndex < 0 || earlyReturnIndex < 0 || sessionIndex > earlyReturnIndex)
  fail("Runtime bootstrap must begin the typed analytics session before its existing-router early return.");

const gameStatePath = `${runtimeRoot}/Core/GameState.cs`;
const saveServicePath = `${runtimeRoot}/Services/SaveService.cs`;
const gameStateHash = pinnedHash(
  gameStatePath,
  GAME_STATE_SHA256,
  "GameState source"
);
const saveServiceHash = pinnedHash(
  saveServicePath,
  SAVE_SERVICE_SHA256,
  "SaveService source"
);
const saveVersionPreserved = exists(gameStatePath) &&
  /public\s+int\s+saveVersion\s*=\s*6\s*;/.test(read(gameStatePath));
const saveKeyPreserved = exists(saveServicePath) &&
  /const\s+string\s+Key\s*=\s*"LegendsOfRus_Save_v1"\s*;/.test(
    read(saveServicePath)
  );
if (!saveVersionPreserved) fail("PlayerSaveData.saveVersion must remain 6.");
if (!saveKeyPreserved) fail("SaveService key must remain LegendsOfRus_Save_v1.");

const testAttributePattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
function countTestAttributes(file, directory = root) {
  if (!exists(file, directory)) return 0;
  return scrubCSharp(read(file, directory)).match(testAttributePattern)?.length ?? 0;
}
const allTestFiles = csharpFiles.filter((file) => file.includes("/Tests/"));
const sourceTestAttributes = allTestFiles.reduce(
  (total, file) => total + countTestAttributes(file),
  0
);
const editTestFiles = allTestFiles.filter((file) =>
  file.startsWith(`${editTestRoot}/`)
);
const playTestFiles = allTestFiles.filter((file) =>
  file.startsWith(`${playTestRoot}/`)
);
const editTestAttributes = editTestFiles.reduce(
  (total, file) => total + countTestAttributes(file),
  0
);
const playTestAttributes = playTestFiles.reduce(
  (total, file) => total + countTestAttributes(file),
  0
);
if (editTestAttributes < 1 || playTestAttributes < 1)
  fail("Both isolated Step 2 EditMode and PlayMode suites need source test attributes.");
for (const file of [...editTestFiles, ...playTestFiles]) {
  if (/\[\s*(?:NonParallelizable|Ignore|Explicit)\b/.test(read(file)))
    fail(`${file} contains a forbidden NonParallelizable, Ignore, or Explicit annotation.`);
}
const selfTestCheckSites = exists(selfTestPath)
  ? Math.max(
      0,
      (scrubCSharp(read(selfTestPath)).match(/\bCheck\s*\(/g)?.length ?? 0) - 1
    )
  : 0;
if (selfTestCheckSites < 1)
  fail("The Step 2 Editor self-test needs at least one source Check site.");

const currentPngs = projectFiles.filter((file) =>
  file.toLowerCase().endsWith(".png")
);
if (currentPngs.length !== 65)
  fail(`Expected exactly 65 approved PNGs, found ${currentPngs.length}.`);

const allowedExactChanges = new Set([
  milestonePath,
  "README.md",
  "README_V1_16_STEP02.md",
  "Assets/LegendsOfRus/Documentation/README_FIRST.md",
  "Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_16_STEP02.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_16_STEP02.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_16_STEP03.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_17_STEP01.json",
  "Assets/LegendsOfRus/Documentation/NEXT_CHECKPOINT_V1_16_STEP02_ACTIVATION.json",
  "Assets/LegendsOfRus/Documentation/V1_16_STEP02_PROVIDER_INTEGRATION_RUNTIME_WIRING_ACTIVATION_REVIEW.md",
  "Assets/LegendsOfRus/Documentation/V1_16_STEP02_UNITY_VERIFICATION_CHECKLIST.md",
  implementationStatusPath,
  evidencePath,
  changedManifestPath,
  staticReportPath,
  commercialConfigPath,
  commercialActivationPath,
  `${commercialCoreRoot}/Analytics/AnalyticsFunnelTracker.cs`,
  runtimeAsmdefPath,
  `${runtimeRoot}/Core/RuntimeBootstrap.cs`,
  `${runtimeRoot}/Gameplay/Match3Board.cs`,
  `${runtimeRoot}/Services/AnalyticsService.cs`,
  `${runtimeRoot}/Services/MonetizationService.cs`,
  `${runtimeRoot}/Services/StoryService.cs`,
  `${runtimeRoot}/UI/LevelTutorialPresenter.cs`,
  `${runtimeRoot}/UI/ScreenRouter.cs`,
  selfTestPath,
  "Tools/verify_v1_16_step02.mjs"
]);
function isAllowedChangedPath(file) {
  if (allowedExactChanges.has(file)) return true;
  if (file.startsWith(`${commercialRuntimeRoot}/`))
    return file.endsWith(".cs") || file.endsWith(".asmdef");
  if (file.startsWith(`${runtimeRoot}/Commercial/`)) return file.endsWith(".cs");
  if (file.startsWith(`${editTestRoot}/`) || file.startsWith(`${playTestRoot}/`))
    return file.endsWith(".cs") || file.endsWith(".asmdef");
  return false;
}

let changedManifestLines = null;
if (exists(changedManifestPath)) {
  changedManifestLines = read(changedManifestPath)
    .split(/\r?\n/)
    .map((line) => line.trim())
    .filter((line) =>
      line && !/^(?:Added|Modified|Changed files|Files)\s*:?\s*$/i.test(line)
    );
  const duplicateManifestLines = changedManifestLines.filter(
    (line, index) => changedManifestLines.indexOf(line) !== index
  );
  if (duplicateManifestLines.length > 0) {
    fail(
      `Changed-file manifest has duplicate paths: ` +
      `${[...new Set(duplicateManifestLines)].join(", ")}.`
    );
  }
  const malformedManifestLines = changedManifestLines.filter((line) =>
    line.startsWith("/") || line.includes("\\") ||
    line.split("/").includes("..") || !isAllowedChangedPath(line)
  );
  if (malformedManifestLines.length > 0)
    fail(`Changed-file manifest contains malformed or unauthorized paths: ${malformedManifestLines.join(", ")}.`);
  if (!changedManifestLines.includes(changedManifestPath))
    fail("Changed-file manifest must inventory its own changed path.");
}

let baselineTreeValid = null;
let baselineVerifierPassed = null;
let protectedUnionIdentical = null;
let approvedPngsIdentical = null;
let inheritedTestsUnchanged = null;
let changedManifestMatchesBaseline = null;
let actualChangedFiles = null;
let baselineFileCount = null;
let baselineTreeHash = null;

const protectedPrefixes = [
  "Assets/Scenes/",
  "ProjectSettings/",
  "Assets/LegendsOfRus/Scripts/EconomyCore/",
  "Assets/LegendsOfRus/Scripts/GameplayCore/",
  "Assets/LegendsOfRus/Scripts/RuntimeEconomy/"
];
const protectedExact = new Set([
  packagesPath,
  economyPath,
  v115ActivationPath,
  remoteConfigPath,
  gameStatePath,
  saveServicePath
]);
function isProtected(file) {
  if (protectedExact.has(file)) return true;
  if (protectedPrefixes.some((prefix) => file.startsWith(prefix))) return true;
  if (
    file.startsWith(`${commercialCoreRoot}/`) &&
    file !== `${commercialCoreRoot}/Analytics/AnalyticsFunnelTracker.cs`
  ) return true;
  return file.toLowerCase().endsWith(".png");
}

if (baseline) {
  if (!fs.existsSync(baseline) || !fs.statSync(baseline).isDirectory()) {
    fail(`Step 1 baseline directory does not exist: ${baseline}.`);
  } else if (fs.realpathSync(baseline) === fs.realpathSync(root)) {
    fail("The Step 2 project cannot be its own Step 1 baseline.");
  } else {
    const baselineFiles = walkProject(baseline, "Step 1 baseline");
    const baselineSet = new Set(baselineFiles);
    baselineFileCount = baselineFiles.length;
    baselineTreeHash = treeFingerprint(baseline, baselineFiles);
    baselineTreeValid =
      baselineFileCount === BASELINE_FILE_COUNT &&
      baselineTreeHash === BASELINE_TREE_SHA256 &&
      exists(milestonePath, baseline) &&
      read(milestonePath, baseline).trim() === BASELINE_MILESTONE_ID &&
      exists("Tools/verify_v1_16_step01.mjs", baseline) &&
      sha256File("Tools/verify_v1_16_step01.mjs", baseline) ===
        BASELINE_VERIFIER_SHA256;
    if (!baselineTreeValid) {
      fail(
        "The supplied Step 1 baseline is not the pinned immutable 396-file tree " +
        `${BASELINE_TREE_SHA256}; found ${baselineFileCount} files and ${baselineTreeHash}.`
      );
    }

    if (baselineTreeValid) {
      const execution = spawnSync(
        process.execPath,
        [absolute("Tools/verify_v1_16_step01.mjs", baseline), "--source-only"],
        { cwd: baseline, encoding: "utf8", stdio: ["ignore", "pipe", "pipe"] }
      );
      let report = null;
      try {
        report = JSON.parse(execution.stdout || "null");
      } catch (error) {
        fail(`Pinned Step 1 verifier did not emit JSON: ${error.message}`);
      }
      baselineVerifierPassed =
        !execution.error &&
        execution.status === 0 &&
        report?.version === "v1.16-step01" &&
        report?.status === "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION" &&
        report?.passed === true &&
        Array.isArray(report?.failures) &&
        report.failures.length === 0;
      if (!baselineVerifierPassed) {
        const detail = Array.isArray(report?.failures)
          ? report.failures.join(" | ")
          : (execution.stderr || execution.error?.message || "unknown failure").trim();
        fail(`Pinned Step 1 source verifier failed: ${detail}`);
      }
    } else {
      baselineVerifierPassed = false;
    }

    const union = [...new Set([...projectFiles, ...baselineFiles])].sort();
    actualChangedFiles = union.filter((file) => {
      if (!projectFileSet.has(file) || !baselineSet.has(file)) return true;
      return sha256File(file) !== sha256File(file, baseline);
    });
    const deleted = actualChangedFiles.filter((file) => !projectFileSet.has(file));
    if (deleted.length > 0)
      fail(`Step 2 may not delete baseline files: ${deleted.join(", ")}.`);
    const unauthorized = actualChangedFiles.filter((file) =>
      !isAllowedChangedPath(file)
    );
    if (unauthorized.length > 0)
      fail(`Changed paths are outside the reviewed Step 2 allowlist: ${unauthorized.join(", ")}.`);

    const protectedUnion = union.filter(isProtected);
    protectedUnionIdentical = protectedUnion.every((file) =>
      projectFileSet.has(file) && baselineSet.has(file) &&
      sha256File(file) === sha256File(file, baseline)
    );
    if (!protectedUnionIdentical)
      fail("A protected path was added, removed, or modified across the full current/baseline union.");

    const baselinePngs = baselineFiles.filter((file) =>
      file.toLowerCase().endsWith(".png")
    );
    approvedPngsIdentical =
      sameArray(currentPngs, baselinePngs) &&
      currentPngs.every((file) =>
        sha256File(file) === sha256File(file, baseline)
      );
    if (!approvedPngsIdentical)
      fail("Approved PNG paths or bytes differ from the pinned Step 1 baseline.");

    const baselineTestFiles = baselineFiles.filter((file) =>
      file.endsWith(".cs") && file.includes("/Tests/")
    );
    const baselineTestAttributes = baselineTestFiles.reduce(
      (total, file) => total + countTestAttributes(file, baseline),
      0
    );
    inheritedTestsUnchanged =
      sourceTestAttributes ===
        baselineTestAttributes + editTestAttributes + playTestAttributes;
    if (!inheritedTestsUnchanged)
      fail("Inherited source test attributes changed outside the isolated Step 2 suites.");

    const baselineAsmdefs = baselineFiles.filter((file) => file.endsWith(".asmdef"));
    const expectedNewAsmdefs = [
      commercialRuntimeAsmdefPath,
      editTestAsmdefPath,
      playTestAsmdefPath
    ].sort();
    const newAsmdefs = asmdefFiles.filter((file) => !baselineAsmdefs.includes(file));
    if (!sameArray(newAsmdefs, expectedNewAsmdefs))
      fail(`Unexpected new asmdef inventory: ${newAsmdefs.join(", ")}.`);
    const changedInheritedAsmdefs = baselineAsmdefs.filter((file) =>
      !projectFileSet.has(file) || sha256File(file) !== sha256File(file, baseline)
    );
    if (!sameArray(changedInheritedAsmdefs, [runtimeAsmdefPath]))
      fail(`Only the Runtime asmdef may change; found ${changedInheritedAsmdefs.join(", ")}.`);

    if (exists(changedManifestPath)) {
      const sortedManifest = [...(changedManifestLines ?? [])].sort();
      changedManifestMatchesBaseline = sameArray(
        sortedManifest,
        actualChangedFiles
      );
      if (!changedManifestMatchesBaseline) {
        const missing = actualChangedFiles.filter((file) =>
          !sortedManifest.includes(file)
        );
        const extra = sortedManifest.filter((file) =>
          !actualChangedFiles.includes(file)
        );
        fail(`Changed-file manifest mismatch; missing=[${missing.join(",")}], extra=[${extra.join(",")}].`);
      }
    }
  }
}

const productionSdkOrNetworkProviderAbsent =
  nativeOrSdkArtifacts.length === 0 &&
  !forbiddenCommercialRuntimeCode
    .filter(([description]) =>
      description === "network API" ||
      description === "production SDK namespace" ||
      description === "development adapter binding"
    )
    .some(([, pattern]) => pattern.test(combinedCommercialRuntimeCode)) &&
  !/https?:\/\//i.test(combinedCommercialRuntimeSource) &&
  packagesHash === PACKAGE_MANIFEST_SHA256;
const commercialRuntimeLiveStateOrPersistenceAbsent =
  !forbiddenCommercialRuntimeCode
    .filter(([description]) =>
      [
        "live GameState access", "SaveService access", "PlayerPrefs access",
        "file persistence", "entitlement completion", "direct live economy grant"
      ].includes(description)
    )
    .some(([, pattern]) => pattern.test(combinedCommercialRuntimeCode));
const effectiveCapabilitiesAllFalse =
  commercialConfigAllDisabled &&
  commercialActivationAllFalse &&
  externalProviderActivationCompiledFalse;

const implementationStatus = parsedJson.get(implementationStatusPath);
if (
  !implementationStatus ||
  implementationStatus.version !== VERSION ||
  implementationStatus.status !== IMPLEMENTATION_STATUS ||
  implementationStatus.runtime_candidate_status !== RUNTIME_CANDIDATE_STATUS ||
  implementationStatus.unity_version !== UNITY_VERSION ||
  implementationStatus.unity_execution_evidence !== NOT_RETAINED ||
  implementationStatus.v1_15_runtime_economy_approved !== false ||
  implementationStatus.external_provider_activation_compiled !== false ||
  implementationStatus.production_providers_bound !== false ||
  implementationStatus.durable_fulfillment_available !== false ||
  implementationStatus.activation_authorized !== false ||
  !isObject(implementationStatus.effective_capabilities) ||
  Object.values(implementationStatus.effective_capabilities).some(
    (value) => value !== false
  )
) {
  fail(
    `${implementationStatusPath} must identify ${VERSION}, ${UNITY_VERSION}, ` +
    `and status ${IMPLEMENTATION_STATUS}.`
  );
}

const evidenceTopKeys = [
  "schemaVersion", "milestone", "status", "implementationStatus",
  "unityVersion", "evidenceRetention", "v115RuntimeEconomyApproved",
  "effectiveCapabilities", "approvalDecisions", "activationAuthorized",
  "blockerCodes"
];
const evidenceRetentionKeys = [
  "v116Step1Compilation",
  "v116Step1ProjectValidator",
  "v116Step1EditorSelfTests",
  "v116Step1EditModeTests",
  "v116Step1PlayModeTests",
  "v116Step2Compilation",
  "v116Step2ProjectValidator",
  "v116Step2EditorSelfTests",
  "v116Step2EditModeTests",
  "v116Step2PlayModeTests",
  "providerIntegration",
  "legalPrivacyReview",
  "representativeDeviceFailurePaths",
  "durableFulfillment"
];
const effectiveCapabilityKeys = [
  "analyticsCapture", "analyticsUpload", "iap", "rewardedAds",
  "durableFulfillment"
];
const approvalDecisionKeys = [
  "providerSelection", "legalPrivacy", "consentFlow", "productIdentifiers",
  "rewardedPlacements", "regionalBehavior", "securityReview",
  "representativeDeviceFailurePaths", "saveMigration", "durableFulfillment",
  "analyticsActivation", "iapActivation", "rewardedAdsActivation",
  "finalRuntimeActivation"
];
const expectedBlockerCodes = [
  "DURABLE_FULFILLMENT_NOT_APPROVED",
  "EXTERNAL_PROVIDER_ACTIVATION_NOT_COMPILED",
  "FINAL_RUNTIME_ACTIVATION_NOT_APPROVED",
  "LEGAL_PRIVACY_NOT_APPROVED",
  "PROVIDER_SELECTION_NOT_APPROVED",
  "V115_RUNTIME_ECONOMY_NOT_APPROVED",
  "V116_STEP1_EVIDENCE_NOT_RETAINED",
  "V116_STEP2_UNITY_EVIDENCE_NOT_RETAINED"
];
const evidence = parsedJson.get(evidencePath);
const evidenceValid = Boolean(evidence) &&
  exactKeys(evidence, evidenceTopKeys, "Step 2 activation evidence") &&
  evidence.schemaVersion === 1 &&
  evidence.milestone === VERSION &&
  evidence.status === "SOURCE_CANDIDATE_ONLY_BLOCKED_FROM_ACTIVATION" &&
  evidence.implementationStatus === IMPLEMENTATION_STATUS &&
  evidence.unityVersion === UNITY_VERSION &&
  evidence.v115RuntimeEconomyApproved === false &&
  evidence.activationAuthorized === false &&
  exactKeys(
    evidence.evidenceRetention,
    evidenceRetentionKeys,
    "Step 2 evidence retention"
  ) &&
  evidenceRetentionKeys.every((key) =>
    evidence.evidenceRetention[key] === NOT_RETAINED
  ) &&
  exactKeys(
    evidence.effectiveCapabilities,
    effectiveCapabilityKeys,
    "Step 2 effective capabilities"
  ) &&
  effectiveCapabilityKeys.every((key) =>
    evidence.effectiveCapabilities[key] === false
  ) &&
  exactKeys(
    evidence.approvalDecisions,
    approvalDecisionKeys,
    "Step 2 approval decisions"
  ) &&
  approvalDecisionKeys.every((key) =>
    evidence.approvalDecisions[key] === NOT_RECORDED
  ) &&
  sameArray(evidence.blockerCodes, expectedBlockerCodes);
if (!evidenceValid)
  fail("The Step 2 activation evidence must be exact, all-disabled, and entirely NOT_RETAINED/NOT_RECORDED.");

const staticReportKeys = [
  "version", "status", "implementation_status", "runtime_candidate_status",
  "unity_execution_evidence", "activation_authorized", "exact_unity_version",
  "commercial_runtime_config_sha256", "commercial_runtime_activation_sha256",
  "economy_catalog_sha256", "v1_15_activation_manifest_sha256",
  "packages_manifest_sha256", "remote_config_local_sha256",
  "remote_config_content_sha256", "v1_15_runtime_activation_approved",
  "commercial_activation_flags_all_false", "effective_capabilities_all_false",
  "external_provider_activation_compiled",
  "production_sdk_or_network_provider_absent",
  "commercial_runtime_live_state_or_persistence_access_absent",
  "candidate_only_fulfillment_boundary_present", "durable_fulfillment_available",
  "unity_compile_status", "editor_self_test_execution_status",
  "editmode_execution_status", "playmode_execution_status",
  "source_test_attributes", "step16_step02_editor_check_sites",
  "approved_png_files", "failures", "static_failures"
];
const staticReport = parsedJson.get(staticReportPath);
const expectedStaticFacts = new Map([
  ["version", VERSION],
  ["status", "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION"],
  ["implementation_status", IMPLEMENTATION_STATUS],
  ["runtime_candidate_status", RUNTIME_CANDIDATE_STATUS],
  ["unity_execution_evidence", NOT_RETAINED],
  ["activation_authorized", false],
  ["exact_unity_version", true],
  ["commercial_runtime_config_sha256", COMMERCIAL_CONFIG_SHA256],
  ["commercial_runtime_activation_sha256", COMMERCIAL_ACTIVATION_SHA256],
  ["economy_catalog_sha256", ECONOMY_SHA256],
  ["v1_15_activation_manifest_sha256", V115_ACTIVATION_SHA256],
  ["packages_manifest_sha256", PACKAGE_MANIFEST_SHA256],
  ["remote_config_local_sha256", REMOTE_CONFIG_SHA256],
  ["remote_config_content_sha256", REMOTE_CONFIG_CONTENT_SHA256],
  ["v1_15_runtime_activation_approved", false],
  ["commercial_activation_flags_all_false", true],
  ["effective_capabilities_all_false", true],
  ["external_provider_activation_compiled", false],
  ["production_sdk_or_network_provider_absent", true],
  ["commercial_runtime_live_state_or_persistence_access_absent", true],
  ["candidate_only_fulfillment_boundary_present", true],
  ["durable_fulfillment_available", false],
  ["unity_compile_status", NOT_RETAINED],
  ["editor_self_test_execution_status", NOT_RETAINED],
  ["editmode_execution_status", NOT_RETAINED],
  ["playmode_execution_status", NOT_RETAINED],
  ["source_test_attributes", sourceTestAttributes],
  ["step16_step02_editor_check_sites", selfTestCheckSites],
  ["approved_png_files", currentPngs.length]
]);
if (staticReport) {
  exactKeys(staticReport, staticReportKeys, "Step 2 static report");
  for (const [key, expected] of expectedStaticFacts) {
    if (!hasOwn(staticReport, key))
      fail(`${staticReportPath} is missing '${key}'.`);
    else if (staticReport[key] !== expected)
      fail(`${staticReportPath} ${key} must equal ${JSON.stringify(expected)}.`);
  }
  if (!Array.isArray(staticReport.failures) || staticReport.failures.length !== 0)
    fail(`${staticReportPath} failures must be an empty array.`);
  if (!Array.isArray(staticReport.static_failures) ||
      staticReport.static_failures.length !== 0)
    fail(`${staticReportPath} static_failures must be an empty array.`);
}

const passed = failures.length === 0;
const result = {
  version: VERSION,
  status: passed
    ? (sourceOnly
      ? "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION"
      : "PASS_STATIC_AND_PROTECTED_BASELINE_VERIFICATION")
    : "FAIL_SOURCE_OR_BOUNDARY_VERIFICATION",
  passed,
  implementation_status: IMPLEMENTATION_STATUS,
  runtime_candidate_status: RUNTIME_CANDIDATE_STATUS,
  unity_execution_evidence: NOT_RETAINED,
  unity_compile_status: NOT_RETAINED,
  editor_self_test_execution_status: NOT_RETAINED,
  editmode_execution_status: NOT_RETAINED,
  playmode_execution_status: NOT_RETAINED,
  activation_authorized: false,
  exact_unity_version: exactUnityVersion,
  source_only: sourceOnly,
  release_boundary_verified: !sourceOnly && passed,
  all_project_json_parses:
    !failures.some((failure) => failure.startsWith("JSON ")),
  project_files: projectFiles.length,
  project_json_files: jsonFiles.length,
  project_asmdef_files: asmdefFiles.length,
  csharp_structural_scan_files: csharpFiles.length,
  csharp_delimiters_balanced:
    !staticFailures.some((failure) => failure.includes("delimiters")),
  preprocessor_directives_balanced:
    !staticFailures.some((failure) => failure.includes("#if/#endif")),
  economy_catalog_sha256: economyHash,
  v1_15_activation_manifest_sha256: v115ActivationHash,
  packages_manifest_sha256: packagesHash,
  remote_config_local_sha256: remoteConfigHash,
  remote_config_content_sha256: REMOTE_CONFIG_CONTENT_SHA256,
  commercial_runtime_config_sha256: commercialConfigHash,
  commercial_runtime_activation_sha256: commercialActivationHash,
  inherited_step1_verifier_sha256: inheritedStep1VerifierHash,
  game_state_source_sha256: gameStateHash,
  save_service_source_sha256: saveServiceHash,
  v1_15_runtime_activation_approved: false,
  v1_15_runtime_activation_inactive: v115ActivationInactive,
  commercial_runtime_config_all_disabled: commercialConfigAllDisabled,
  commercial_activation_flags_all_false: commercialActivationAllFalse,
  effective_capabilities_all_false: effectiveCapabilitiesAllFalse,
  external_provider_activation_compiled: false,
  production_sdk_or_network_provider_absent:
    productionSdkOrNetworkProviderAbsent,
  commercial_runtime_live_state_or_persistence_access_absent:
    commercialRuntimeLiveStateOrPersistenceAbsent,
  candidate_only_fulfillment_boundary_present:
    candidateOnlyFulfillmentBoundaryPresent,
  durable_fulfillment_available: false,
  trusted_monetization_boundary_present: trustedMonetizationBoundaryPresent,
  runtime_commercial_port_bound: false,
  committed_runtime_result_factory_called: false,
  atomic_analytics_funnel_present: atomicAnalyticsFunnelPresent,
  commercial_runtime_asmdef_valid: commercialRuntimeAsmdefValid,
  runtime_asmdef_valid: runtimeAsmdefValid,
  commercial_editmode_test_asmdef_valid: editTestAsmdefValid,
  commercial_playmode_test_asmdef_valid: playTestAsmdefValid,
  assembly_graph_acyclic: assemblyGraphAcyclic,
  source_test_attributes_informational: sourceTestAttributes,
  new_editmode_source_test_attributes_informational: editTestAttributes,
  new_playmode_source_test_attributes_informational: playTestAttributes,
  editor_self_test_check_sites_informational: selfTestCheckSites,
  source_counts_are_execution_evidence: false,
  approved_png_files: currentPngs.length,
  static_report_present: exists(staticReportPath),
  activation_evidence_present: exists(evidencePath),
  evidence_record_valid: evidenceValid,
  baseline_supplied: baseline !== null,
  baseline_file_count: baseline ? baselineFileCount : null,
  baseline_tree_sha256: baseline ? baselineTreeHash : null,
  baseline_tree_valid: baseline ? baselineTreeValid : null,
  baseline_source_verifier_passed: baseline ? baselineVerifierPassed : null,
  protected_union_identical: baseline ? protectedUnionIdentical : null,
  approved_pngs_identical_to_step01: baseline ? approvedPngsIdentical : null,
  inherited_source_test_attributes_unchanged:
    baseline ? inheritedTestsUnchanged : null,
  actual_changed_paths: baseline ? actualChangedFiles?.length ?? null : null,
  changed_file_manifest_present: exists(changedManifestPath),
  changed_file_manifest_matches_step01_baseline:
    baseline ? changedManifestMatchesBaseline : null,
  required_final_artifacts_present: required.every((file) => exists(file)),
  failures,
  static_failures: staticFailures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (!passed) process.exitCode = 1;
