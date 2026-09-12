import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { spawnSync } from "node:child_process";

const VERSION = "v1.17-step02";
const MILESTONE_ID = "v1.17-step02-save-migration-recovery-foundation";
const IMPLEMENTATION_STATUS =
  "IMPLEMENTED_IN_SOURCE_PENDING_UNITY_SAVE_RECOVERY_VERIFICATION";
const CANDIDATE_STATUS =
  "SAVE_V6_ENVELOPE_V1_RECOVERY_FOUNDATION_PRODUCTION_ACTIVATION_BLOCKED";
const UNITY_VERSION = "6000.5.9f1";
const PAYLOAD_VERSION = 6;
const ENVELOPE_VERSION = 1;

const BASELINE_FILE_COUNT = 457;
const BASELINE_TREE_SHA256 =
  "780e70e665604a414cd0c1887f223134a8c7aaff7cf648d81d3bdae25b007739";
const BASELINE_VERIFIER_SHA256 =
  "9b99862be8e73e304edbc6a4203a207c5989839344282b4f04fea8bea1e77ee0";
const BASELINE_MILESTONE_ID =
  "v1.17-step01-android-build-configuration-signing-foundation";
const BASELINE_VERIFIER_PATH = "Tools/verify_v1_17_step01.mjs";

const PNG_FILE_COUNT = 65;
const PNG_INVENTORY_SHA256 =
  "f45fa37b4e19a68a85317d508cc0364ac45bbe183e227cd83c3e17c41c3ff0fe";
const PACKAGE_MANIFEST_SHA256 =
  "f534b3f48018d7c02ddfb1b59588813562837db8167dda7395956065bf4f6713";
const PROJECT_VERSION_SHA256 =
  "55ad613d4cefdd466d10e22bb2885c8a0993cebfe7b01544d843430e36102784";
const EDITOR_BUILD_SETTINGS_SHA256 =
  "19d5bf4b4915efa6ca636161ee67bb21033b6a9fc919bc9ccdc22ecca0f71162";
const ECONOMY_ACTIVATION_SHA256 =
  "4c4a3d062257addf9e3bd50008a5bde281aa3e227bffe097982b7890bd597248";
const COMMERCIAL_ACTIVATION_SHA256 =
  "67f31982d6eafcc54f644f8ad1e81f672ddeefe3856da4eaae6b71af3bb753e9";
const COMMERCIAL_CONFIG_SHA256 =
  "d5e7705c47f0a7368fa618db0740703e4e0628a2b99ab5318681933f00866c02";

const REVIEWED_PROJECT_FILE_COUNT = 487;
const REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256 =
  "3a9860614cfcea7661a79af9c67dd72167e6b4fbcfb65317e5c4cae4b663d2ed";

const VERIFIER_PATH = "Tools/verify_v1_17_step02.mjs";
const changedManifestPath =
  "Assets/LegendsOfRus/Documentation/V1_17_STEP02_CHANGED_FILES.txt";
const staticReportPath =
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_17_STEP02.json";
const implementationStatusPath =
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_17_STEP02.json";
const activationEvidencePath =
  "Assets/LegendsOfRus/Documentation/V1_17_STEP02_ACTIVATION_EVIDENCE.json";
const economyActivationPath =
  "Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json";
const commercialActivationPath =
  "Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json";
const commercialConfigPath =
  "Assets/LegendsOfRus/Resources/Config/commercial_runtime_config.json";
const androidStateReaderPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildStateReader.cs";
const androidGuardPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildGuard.cs";
const androidEvaluatorPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Core/AndroidBuildPolicyEvaluator.cs";
const commercialActivationSourcePath =
  "Assets/LegendsOfRus/Scripts/CommercialRuntime/Activation/CommercialRuntimeActivation.cs";
const saveServicePath =
  "Assets/LegendsOfRus/Scripts/Runtime/Services/SaveService.cs";
const gameStatePath =
  "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs";
const migrationRegistryPath =
  "Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs";
const envelopeCodecPath =
  "Assets/LegendsOfRus/Scripts/SaveCore/SaveEnvelopeCodec.cs";
const envelopeTypesPath =
  "Assets/LegendsOfRus/Scripts/SaveCore/SaveEnvelopeTypes.cs";
const journalRepositoryPath =
  "Assets/LegendsOfRus/Scripts/SaveCore/SaveJournalRepository.cs";
const payloadCodecPath =
  "Assets/LegendsOfRus/Scripts/Runtime/Services/PlayerSavePayloadCodec.cs";
const runtimeAsmdefPath =
  "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef";
const saveCoreAsmdefPath =
  "Assets/LegendsOfRus/Scripts/SaveCore/LegendsOfRus.SaveCore.asmdef";
const saveCoreTestsAsmdefPath =
  "Assets/LegendsOfRus/Tests/SaveRecoveryEditMode/LegendsOfRus.SaveCore.Tests.asmdef";
const migrationTestsAsmdefPath =
  "Assets/LegendsOfRus/Tests/SaveMigrationEditMode/LegendsOfRus.SaveMigration.Tests.asmdef";
const selfTestPath =
  "Assets/LegendsOfRus/Scripts/Editor/Step17SaveMigrationRecoveryFoundationSelfTest.cs";

const inheritedSaveIsolationFixturePaths = [
  "Assets/LegendsOfRus/Tests/CommercialRuntimePlayMode/CommercialRuntimeWiringPlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/AudioHapticsFidelityVerificationPlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/ChapterFinaleCompletionPlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/CoreLoopMotionFeedbackPlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/EconomyCatalogLoaderPlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/Match3RuntimePlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/MetaUiMotionTransitionPolishPlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/RuntimeEconomyActivationGatePlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/StoryDataRuntimePlayModeTests.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/StoryMilestoneReactionPlayModeTests.cs"
].sort();
const inheritedSaveIsolationFixtureSet =
  new Set(inheritedSaveIsolationFixturePaths);

const allowedChangedPaths = [
  "Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
  implementationStatusPath,
  "Assets/LegendsOfRus/Documentation/NEXT_CHECKPOINT_V1_17_STEP02_UNITY_VERIFICATION.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_17_STEP03.json",
  "Assets/LegendsOfRus/Documentation/README_FIRST.md",
  staticReportPath,
  activationEvidencePath,
  changedManifestPath,
  "Assets/LegendsOfRus/Documentation/V1_17_STEP02_SAVE_MIGRATION_RECOVERY_FOUNDATION.md",
  selfTestPath,
  gameStatePath,
  migrationRegistryPath,
  "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeSaveAssemblyAccess.cs",
  runtimeAsmdefPath,
  "Assets/LegendsOfRus/Scripts/Runtime/Services/PlayerPrefsSaveJournalStore.cs",
  payloadCodecPath,
  "Assets/LegendsOfRus/Scripts/Runtime/Services/PlayerSavePersistenceResults.cs",
  saveServicePath,
  saveCoreAsmdefPath,
  "Assets/LegendsOfRus/Scripts/SaveCore/SaveCoreAssemblyAccess.cs",
  envelopeCodecPath,
  envelopeTypesPath,
  journalRepositoryPath,
  "Assets/LegendsOfRus/Scripts/SaveCore/SaveJournalStore.cs",
  "Assets/LegendsOfRus/Scripts/SaveCore/SaveJournalTypes.cs",
  ...inheritedSaveIsolationFixturePaths,
  "Assets/LegendsOfRus/Tests/PlayMode/SaveMigrationRecoveryPlayModeTests.cs",
  migrationTestsAsmdefPath,
  "Assets/LegendsOfRus/Tests/SaveMigrationEditMode/PlayerSaveMigrationRegistryEditModeTests.cs",
  "Assets/LegendsOfRus/Tests/SaveRecoveryEditMode/DurablePendingFakeStore.cs",
  saveCoreTestsAsmdefPath,
  "Assets/LegendsOfRus/Tests/SaveRecoveryEditMode/SaveEnvelopeCodecEditModeTests.cs",
  "Assets/LegendsOfRus/Tests/SaveRecoveryEditMode/SaveJournalLoadEditModeTests.cs",
  "Assets/LegendsOfRus/Tests/SaveRecoveryEditMode/SaveJournalWriteEditModeTests.cs",
  "MILESTONE_VERSION.txt",
  "README.md",
  "README_V1_17_STEP02.md",
  VERIFIER_PATH
].sort();

const protectedPrefixPins = [
  {
    prefix: "Assets/LegendsOfRus/Resources",
    files: 56,
    sha256: "afc6f3bfad1acd64c02537fcf0e0d0ef0db049b5e7b953247c75e6bf6cce8669"
  },
  {
    prefix: "Assets/LegendsOfRus/Resources/Config",
    files: 8,
    sha256: "4ab534bd45fd178b233d8d4a8367313a62ee1830033bf66128b5f5180c1f3f95"
  },
  {
    prefix: "Assets/Scenes",
    files: 2,
    sha256: "0940801e6d1174dab075d0f3671ef72a7c5d26624a368209fc7f0d1a5348172a"
  },
  {
    prefix: "Packages",
    files: 1,
    sha256: "8ea8fe4b3512dab71a526fb3d76cea7e07709725d36a6ff270660c2b181b2ce6"
  },
  {
    prefix: "ProjectSettings",
    files: 2,
    sha256: "9d1402e81b07019f5f41afb56d4765e6851ccfda08221943d16d78edd61e5a4f"
  },
  {
    prefix: "Assets/LegendsOfRus/Editor/AndroidBuild",
    files: 13,
    sha256: "53c750bc6a9db555238b58e88b82d138c86bc170f449460151bc652303071e53"
  },
  {
    prefix: "Assets/LegendsOfRus/Scripts/EconomyCore",
    files: 9,
    sha256: "cd1303ed19a5a80c457035a387f77ea80f733f99a89f783396208ea5e877853f"
  },
  {
    prefix: "Assets/LegendsOfRus/Scripts/RuntimeEconomy",
    files: 5,
    sha256: "956f5595c78fbbb29977e734c7a58156e0a59271732bd39b1ca6edb38c8a6a2c"
  },
  {
    prefix: "Assets/LegendsOfRus/Scripts/CommercialCore",
    files: 27,
    sha256: "40731664c8acb17b0524dd4a96c54087082d0cfaf99c81562975970bde88eff4"
  },
  {
    prefix: "Assets/LegendsOfRus/Scripts/CommercialRuntime",
    files: 14,
    sha256: "cf0b7a9225163f041c2636ab5f1db3bcab2a76f44b374b0e7ffff990e36f56f3"
  }
];

const root = process.cwd();
const failures = [];
const staticFailures = [];

function fail(message, structural = false) {
  if (!failures.includes(message)) failures.push(message);
  if (structural && !staticFailures.includes(message))
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
  fail("Release-boundary verification requires --baseline <v1.17-step01-project>.");

const generatedRootNames = new Set([
  ".git", ".gradle", "Library", "Temp", "Logs", "obj", "Build", "Builds",
  "UserSettings"
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
    } catch {
      report(`cannot enumerate '${relative || "."}'.`);
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
      } catch {
        report(`cannot inspect '${child}'.`);
        continue;
      }
      if (stat.isSymbolicLink()) {
        report(`symbolic link '${child}' is not allowed.`);
        continue;
      }
      if (stat.isDirectory()) {
        const lower = child.toLocaleLowerCase("en-US");
        if (lower === "assets/plugins" || lower.startsWith("assets/plugins/"))
          report(`plugin directory '${child}' is not allowed.`);
        if (child.startsWith("Assets/") &&
            entry.name.toLowerCase().endsWith(".androidlib"))
          report(`Android library project directory '${child}' is not allowed.`);
        if (relative === "Packages")
          report(`embedded package directory '${child}' is not allowed.`);
        visit(child);
        continue;
      }
      if (!stat.isFile()) {
        report(`special filesystem entry '${child}' is not allowed.`);
        continue;
      }
      const folded = child.toLocaleLowerCase("en-US");
      if (caseFolded.has(folded) && caseFolded.get(folded) !== child) {
        report(`case-fold collision '${caseFolded.get(folded)}' and '${child}'.`);
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

function inventoryFingerprint(directory, files) {
  const hash = crypto.createHash("sha256");
  for (const file of [...files].sort()) {
    hash.update(file);
    hash.update("\0");
    hash.update(sha256File(file, directory));
    hash.update("\n");
  }
  return hash.digest("hex");
}

function treeFingerprint(directory, files) {
  return inventoryFingerprint(directory, files);
}

function sameArray(left, right) {
  return Array.isArray(left) && left.length === right.length &&
    left.every((item, index) => item === right[index]);
}

function sameSet(left, right) {
  return sameArray([...left].sort(), [...right].sort());
}

function isObject(value) {
  return value !== null && typeof value === "object" && !Array.isArray(value);
}

// JSON.parse accepts duplicate object keys. Reject them before parsing.
function duplicateJsonKeys(source) {
  let index = 0;
  const duplicates = [];
  function whitespace() {
    while (/\s/.test(source[index] ?? "")) index++;
  }
  function stringToken() {
    const start = index;
    if (source[index] !== '"') throw new Error("expected string");
    index++;
    while (index < source.length) {
      if (source[index] === "\\") index += 2;
      else if (source[index] === '"') {
        index++;
        return JSON.parse(source.slice(start, index));
      } else index++;
    }
    throw new Error("unterminated string");
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
    if (!match) throw new Error("invalid token");
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
      if (seen.has(key)) duplicates.push(`${location}.${key}`);
      seen.add(key);
      whitespace();
      if (source[index] !== ":") throw new Error("expected colon");
      index++;
      value(`${location}.${key}`);
      whitespace();
      if (source[index] === "}") {
        index++;
        return;
      }
      if (source[index] !== ",") throw new Error("expected comma");
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
      value(`${location}[${item++}]`);
      whitespace();
      if (source[index] === "]") {
        index++;
        return;
      }
      if (source[index] !== ",") throw new Error("expected comma");
      index++;
    }
    throw new Error("unterminated array");
  }
  whitespace();
  value("$");
  whitespace();
  if (index !== source.length) throw new Error("trailing content");
  return duplicates;
}

function parseJson(relative, directory = root) {
  if (!exists(relative, directory)) {
    fail(`${relative} is missing.`);
    return null;
  }
  const source = read(relative, directory);
  try {
    const duplicates = duplicateJsonKeys(source);
    if (duplicates.length > 0) {
      fail(`${relative} contains duplicate JSON keys.`);
      return null;
    }
    return JSON.parse(source);
  } catch {
    fail(`${relative} is not strict valid JSON.`);
    return null;
  }
}

function scrubCSharp(source) {
  let output = "";
  let index = 0;
  let state = "code";
  while (index < source.length) {
    const char = source[index];
    const next = source[index + 1] ?? "";
    if (state === "line") {
      if (char === "\n") {
        output += "\n";
        state = "code";
      } else output += " ";
      index++;
      continue;
    }
    if (state === "block") {
      if (char === "*" && next === "/") {
        output += "  ";
        index += 2;
        state = "code";
      } else {
        output += char === "\n" ? "\n" : " ";
        index++;
      }
      continue;
    }
    if (state === "string" || state === "char") {
      const quote = state === "string" ? '"' : "'";
      if (char === "\\") {
        output += "  ";
        index += 2;
      } else if (char === quote) {
        output += " ";
        index++;
        state = "code";
      } else {
        output += char === "\n" ? "\n" : " ";
        index++;
      }
      continue;
    }
    if (state === "verbatim") {
      if (char === '"' && next === '"') {
        output += "  ";
        index += 2;
      } else if (char === '"') {
        output += " ";
        index++;
        state = "code";
      } else {
        output += char === "\n" ? "\n" : " ";
        index++;
      }
      continue;
    }
    if (char === "/" && next === "/") {
      output += "  ";
      index += 2;
      state = "line";
    } else if (char === "/" && next === "*") {
      output += "  ";
      index += 2;
      state = "block";
    } else if (char === "@" && next === '"') {
      output += "  ";
      index += 2;
      state = "verbatim";
    } else if (char === '"') {
      output += " ";
      index++;
      state = "string";
    } else if (char === "'") {
      output += " ";
      index++;
      state = "char";
    } else {
      output += char;
      index++;
    }
  }
  return output;
}

function structuralCSharpCheck(relative) {
  const source = read(relative);
  const scrubbed = scrubCSharp(source);
  const pairs = new Map([["}", "{"], [")", "("], ["]", "["]]);
  const openings = new Set(["{", "(", "["]);
  const stack = [];
  for (const char of scrubbed) {
    if (openings.has(char)) stack.push(char);
    else if (pairs.has(char) && stack.pop() !== pairs.get(char)) {
      fail(`${relative} has unbalanced C# delimiters.`, true);
      break;
    }
  }
  if (stack.length > 0)
    fail(`${relative} has unbalanced C# delimiters.`, true);

  let preprocessorDepth = 0;
  for (const line of source.split(/\r?\n/)) {
    if (/^\s*#if\b/.test(line)) preprocessorDepth++;
    if (/^\s*#endif\b/.test(line)) preprocessorDepth--;
    if (preprocessorDepth < 0) {
      fail(`${relative} has unbalanced preprocessor directives.`, true);
      return;
    }
  }
  if (preprocessorDepth !== 0)
    fail(`${relative} has unbalanced preprocessor directives.`, true);
}

function extractCSharpMethodBody(source, signaturePattern) {
  const scrubbed = scrubCSharp(source);
  const match = signaturePattern.exec(scrubbed);
  if (!match) return null;
  const opening = scrubbed.indexOf("{", match.index + match[0].length);
  if (opening < 0) return null;
  let depth = 0;
  for (let index = opening; index < scrubbed.length; index++) {
    if (scrubbed[index] === "{") depth++;
    else if (scrubbed[index] === "}") {
      depth--;
      if (depth === 0) return source.slice(opening, index + 1);
    }
  }
  return null;
}

function filesUnder(files, prefix) {
  return files.filter((file) => file === prefix || file.startsWith(`${prefix}/`));
}

function isProtectedPath(file) {
  return file.toLowerCase().endsWith(".png") ||
    protectedPrefixPins.some(({ prefix }) =>
      file === prefix || file.startsWith(`${prefix}/`));
}

const projectFiles = walkProject(root, "candidate");
const projectFileSet = new Set(projectFiles);
const requiredArtifacts = new Set(allowedChangedPaths);
for (const relative of requiredArtifacts) {
  if (!projectFileSet.has(relative)) fail(`Required artifact '${relative}' is missing.`);
}

const finalPinsReady =
  Number.isInteger(REVIEWED_PROJECT_FILE_COUNT) &&
  REVIEWED_PROJECT_FILE_COUNT > BASELINE_FILE_COUNT &&
  /^[0-9a-f]{64}$/.test(REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256);
if (!finalPinsReady) {
  fail(
    "Final reviewed candidate file-count/hash pins are unset; freeze all other Step 2 bytes and repin the FINALIZATION PINS.",
    true
  );
}
const reviewedCandidateFiles = projectFiles.filter((file) => file !== VERIFIER_PATH);
const reviewedCandidateBytesExact = finalPinsReady &&
  projectFiles.length === REVIEWED_PROJECT_FILE_COUNT &&
  reviewedCandidateFiles.length === REVIEWED_PROJECT_FILE_COUNT - 1 &&
  treeFingerprint(root, reviewedCandidateFiles) ===
    REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256;
if (finalPinsReady && !reviewedCandidateBytesExact)
  fail("Candidate files outside the verifier differ from the exact reviewed source boundary.", true);

const milestone = exists("MILESTONE_VERSION.txt")
  ? read("MILESTONE_VERSION.txt").trim()
  : null;
if (milestone !== MILESTONE_ID)
  fail(`MILESTONE_VERSION.txt must equal '${MILESTONE_ID}'.`);
if (!exists("ProjectSettings/ProjectVersion.txt") ||
    read("ProjectSettings/ProjectVersion.txt").trim() !==
      `m_EditorVersion: ${UNITY_VERSION}`)
  fail(`ProjectVersion.txt must pin exactly Unity ${UNITY_VERSION}.`);
if (exists("ProjectSettings/ProjectSettings.asset"))
  fail("ProjectSettings/ProjectSettings.asset must remain absent from the source candidate.");
if (exists("Packages/packages-lock.json"))
  fail("Packages/packages-lock.json remains unreviewed and must be absent from this source candidate.");

const directProtectedHashes = new Map([
  ["Packages/manifest.json", PACKAGE_MANIFEST_SHA256],
  ["ProjectSettings/ProjectVersion.txt", PROJECT_VERSION_SHA256],
  ["ProjectSettings/EditorBuildSettings.asset", EDITOR_BUILD_SETTINGS_SHA256],
  [economyActivationPath, ECONOMY_ACTIVATION_SHA256],
  [commercialActivationPath, COMMERCIAL_ACTIVATION_SHA256],
  [commercialConfigPath, COMMERCIAL_CONFIG_SHA256]
]);
for (const [relative, expected] of directProtectedHashes) {
  if (!exists(relative)) fail(`Protected file '${relative}' is missing.`);
  else if (sha256File(relative) !== expected)
    fail(`Protected file '${relative}' changed from the reviewed bytes.`);
}

const protectedPrefixResults = {};
for (const pin of protectedPrefixPins) {
  const files = filesUnder(projectFiles, pin.prefix);
  const fingerprint = inventoryFingerprint(root, files);
  const exact = files.length === pin.files && fingerprint === pin.sha256;
  protectedPrefixResults[pin.prefix] = {
    files: files.length,
    sha256: fingerprint,
    exact
  };
  if (!exact)
    fail(`Protected inventory '${pin.prefix}' differs from the reviewed Step 1 bytes.`);
}

const allJsonFiles = projectFiles.filter((file) => file.endsWith(".json"));
for (const file of allJsonFiles) parseJson(file);

const economyActivation = parseJson(economyActivationPath);
const economyActivationDisabled = Boolean(
  economyActivation &&
  economyActivation.compilationVerified === false &&
  economyActivation.editorSelfTestsPassed === false &&
  economyActivation.editModeTestsPassed === false &&
  economyActivation.playModeTestsPassed === false &&
  economyActivation.runtimeActivationApproved === false
);
if (!economyActivationDisabled)
  fail("Runtime economy activation evidence must remain incomplete and activation must remain false.");

const commercialActivation = parseJson(commercialActivationPath);
const commercialFalseFields = [
  "v115RuntimeEconomyApproved",
  "v116Step1CompilationVerified",
  "v116Step1ProjectValidatorPassed",
  "v116Step1EditorSelfTestsPassed",
  "v116Step1EditModeTestsPassed",
  "v116Step1PlayModeTestsPassed",
  "providerSelectionApproved",
  "legalPrivacyApproved",
  "consentFlowApproved",
  "productIdentifiersApproved",
  "rewardedPlacementsApproved",
  "regionalBehaviorApproved",
  "securityReviewPassed",
  "representativeDeviceFailurePathsPassed",
  "saveMigrationApproved",
  "durableFulfillmentApproved",
  "analyticsActivationApproved",
  "iapActivationApproved",
  "rewardedAdsActivationApproved",
  "finalRuntimeActivationApproved"
];
const commercialActivationDisabled = Boolean(
  commercialActivation &&
  commercialActivation.environment === "disabled" &&
  commercialFalseFields.every((field) => commercialActivation[field] === false)
);
if (!commercialActivationDisabled)
  fail("Commercial activation approvals and provider environment must remain disabled.");

const commercialConfig = parseJson(commercialConfigPath);
const commercialConfigDisabled = Boolean(
  commercialConfig &&
  commercialConfig.environment === "disabled" &&
  commercialConfig.consentProviderId === "disabled" &&
  commercialConfig.analyticsProviderId === "disabled" &&
  commercialConfig.iapProviderId === "disabled" &&
  commercialConfig.rewardedAdProviderId === "disabled" &&
  commercialConfig.analyticsCaptureAllowed === false &&
  commercialConfig.analyticsUploadAllowed === false &&
  commercialConfig.iapAllowed === false &&
  commercialConfig.rewardedAdsAllowed === false &&
  commercialConfig.developmentSandboxAllowed === false
);
if (!commercialConfigDisabled)
  fail("Commercial runtime configuration must remain the exact disabled composition.");

const androidStateReaderSource = exists(androidStateReaderPath)
  ? read(androidStateReaderPath) : "";
const androidGuardSource = exists(androidGuardPath) ? read(androidGuardPath) : "";
const androidEvaluatorSource = exists(androidEvaluatorPath)
  ? read(androidEvaluatorPath) : "";
const commercialActivationSource = exists(commercialActivationSourcePath)
  ? read(commercialActivationSourcePath) : "";
const resolvedPackageLockConstants =
  androidStateReaderSource.match(
    /private\s+const\s+bool\s+ResolvedPackageLockApproved\s*=\s*false\s*;/g
  )?.length ?? 0;
const packageLockApprovalBlocked =
  resolvedPackageLockConstants === 1 &&
  androidStateReaderSource.includes(
    "facts.PackageLockApproved = ResolvedPackageLockApproved;"
  ) &&
  /if\s*\(\s*!facts\.PackageLockApproved\s*\)/.test(androidEvaluatorSource) &&
  androidEvaluatorSource.includes("AndroidBuildReasonCodes.PackageLockUnapproved");
if (!packageLockApprovalBlocked)
  fail("ResolvedPackageLockApproved must remain a compiled false fail-closed Android gate.");

const productionAndroidBuildCompiled =
  /private\s+const\s+bool\s+ProductionAndroidBuildCompiled\s*=\s*false\s*;/.test(
    androidGuardSource
  );
if (!productionAndroidBuildCompiled)
  fail("ProductionAndroidBuildCompiled must remain false.");
const externalProviderActivationCompiled =
  /private\s+const\s+bool\s+ExternalProviderActivationCompiled\s*=\s*false\s*;/.test(
    commercialActivationSource
  );
if (!externalProviderActivationCompiled)
  fail("ExternalProviderActivationCompiled must remain false.");

const prohibitedExtensions = new Set([
  ".aar", ".aab", ".apk", ".apks", ".dll", ".jar", ".java", ".jks",
  ".key", ".keystore", ".kt", ".p12", ".pem", ".pfx", ".rsp", ".so",
  ".unitypackage", ".xapk"
]);
for (const file of projectFiles) {
  const lower = file.toLowerCase();
  if (prohibitedExtensions.has(path.extname(lower)))
    fail(`Prohibited provider, signing, plugin, or build artifact '${file}' is present.`);
  if (lower === ".env" || lower.startsWith(".env.") || lower.includes("/.env"))
    fail(`Environment secret file '${file}' is not allowed.`);
  if (lower.endsWith(".asmref"))
    fail(`Assembly reference '${file}' is not allowed in this milestone.`);
}

const changedTextExtensions = new Set([
  ".cs", ".json", ".md", ".mjs", ".txt", ".asmdef"
]);
const highConfidenceSecretPatterns = [
  /-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----/,
  /\bAKIA[0-9A-Z]{16}\b/,
  /\bAIza[0-9A-Za-z_-]{30,}\b/,
  /\bgh[pousr]_[0-9A-Za-z]{30,}\b/,
  /\bxox[baprs]-[0-9A-Za-z-]{10,}\b/,
  /\bsk_live_[0-9A-Za-z]{16,}\b/,
  /\beyJ[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{10,}\b/,
  /(?:password|secret|token|private[_-]?key|keyalias|keystorepass)\s*[:=]\s*["'][^"']{4,}["']/i
];
for (const file of allowedChangedPaths) {
  if (!exists(file) || !changedTextExtensions.has(path.extname(file).toLowerCase()))
    continue;
  const source = read(file);
  if (highConfidenceSecretPatterns.some((pattern) => pattern.test(source)))
    fail(`${file} contains a prohibited credential or high-confidence secret pattern.`);
}

const allCSharpFiles = projectFiles.filter((file) => file.endsWith(".cs"));
const changedCSharpFiles = allowedChangedPaths.filter(
  (file) => file.endsWith(".cs") && exists(file)
);
for (const file of changedCSharpFiles) structuralCSharpCheck(file);

let playerPrefsDeleteAllAbsent = true;
for (const file of allCSharpFiles) {
  if (/\bPlayerPrefs\s*\.\s*DeleteAll\s*\(/.test(scrubCSharp(read(file)))) {
    fail(`${file} invokes forbidden PlayerPrefs.DeleteAll().`);
    playerPrefsDeleteAllAbsent = false;
  }
}

const saveNamespaceKeyLiterals = [
  '"LegendsOfRus_Save_v1"',
  '"LegendsOfRus_Save_v1_Backup"',
  '"LegendsOfRus_Save_v1_Staging"'
];
let inheritedSaveIsolationFixturesValid = true;
for (const file of inheritedSaveIsolationFixturePaths) {
  if (!exists(file)) {
    fail(`Required inherited save-isolation fixture '${file}' is missing.`);
    inheritedSaveIsolationFixturesValid = false;
    continue;
  }
  const source = read(file);
  const scrubbed = scrubCSharp(source);
  const keysPresent = saveNamespaceKeyLiterals.every((key) =>
    source.includes(key)
  );
  const capturesNamespace =
    /static\s+readonly\s+string\s*\[\s*\]/.test(scrubbed) &&
    /\bPlayerPrefs\s*\.\s*HasKey\s*\(/.test(scrubbed) &&
    /\bPlayerPrefs\s*\.\s*GetString\s*\(/.test(scrubbed);
  const clearsAndRestoresNamespace =
    (scrubbed.match(/\bPlayerPrefs\s*\.\s*DeleteKey\s*\(/g)?.length ?? 0) >= 2 &&
    /\bPlayerPrefs\s*\.\s*SetString\s*\(/.test(scrubbed) &&
    (scrubbed.match(/\bPlayerPrefs\s*\.\s*Save\s*\(/g)?.length ?? 0) >= 2;
  const deleteAllAbsent =
    !/\bPlayerPrefs\s*\.\s*DeleteAll\s*\(/.test(scrubbed);
  if (!keysPresent || !capturesNamespace || !clearsAndRestoresNamespace ||
      !deleteAllAbsent) {
    fail(`${file} does not preserve and isolate the complete three-key save namespace.`);
    inheritedSaveIsolationFixturesValid = false;
  }
}

const productionDeltaFiles = changedCSharpFiles.filter((file) =>
  file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/") ||
  file.startsWith("Assets/LegendsOfRus/Scripts/SaveCore/")
);
const productionDeltaSource = productionDeltaFiles
  .map((file) => scrubCSharp(read(file))).join("\n");
const forbiddenProductionPatterns = [
  { name: "System.Net", pattern: /\busing\s+System\.Net(?:\.|\s*;)/ },
  { name: "UnityWebRequest", pattern: /\bUnityWebRequest\b/ },
  { name: "HttpClient", pattern: /\bHttpClient\b/ },
  { name: "WebClient", pattern: /\bWebClient\b/ },
  { name: "Firebase", pattern: /\bFirebase(?:\.|\w)/ },
  { name: "PlayFab", pattern: /\bPlayFab(?:\.|\w)/ },
  { name: "CloudSaveService", pattern: /\bCloudSaveService\b/ },
  { name: "AuthenticationService", pattern: /\bAuthenticationService\b/ },
  { name: "runtime economy activation", pattern: /\bRuntimeEconomyActivationGate\b/ },
  { name: "commercial activation", pattern: /\bCommercialRuntimeActivationGate\b/ },
  { name: "commercial compiled activation", pattern: /\bExternalProviderActivationCompiled\b/ },
  { name: "economy approval assignment", pattern: /\bruntimeActivationApproved\s*=\s*true\b/ },
  { name: "commercial approval assignment", pattern: /\bfinalRuntimeActivationApproved\s*=\s*true\b/ }
];
let forbiddenProductionTokensAbsent = true;
for (const entry of forbiddenProductionPatterns) {
  if (entry.pattern.test(productionDeltaSource)) {
    fail(`Step 2 production delta contains forbidden ${entry.name} wiring.`);
    forbiddenProductionTokensAbsent = false;
  }
}

const saveServiceSource = exists(saveServicePath) ? read(saveServicePath) : "";
const gameStateSource = exists(gameStatePath) ? read(gameStatePath) : "";
const migrationRegistrySource = exists(migrationRegistryPath)
  ? read(migrationRegistryPath) : "";
const envelopeCodecSource = exists(envelopeCodecPath) ? read(envelopeCodecPath) : "";
const envelopeTypesSource = exists(envelopeTypesPath) ? read(envelopeTypesPath) : "";
const journalRepositorySource = exists(journalRepositoryPath)
  ? read(journalRepositoryPath) : "";
const payloadCodecSource = exists(payloadCodecPath) ? read(payloadCodecPath) : "";

const payloadVersionPinned =
  /public\s+int\s+saveVersion\s*=\s*6\s*;/.test(gameStateSource) &&
  /(?:internal|public)\s+const\s+int\s+CurrentPayloadVersion\s*=\s*6\s*;/.test(
    migrationRegistrySource
  );
if (!payloadVersionPinned)
  fail("PlayerSaveData and the migration registry must retain payload version 6.");
const envelopeVersionPinned =
  /public\s+const\s+int\s+CurrentEnvelopeVersion\s*=\s*1\s*;/.test(
    envelopeCodecSource
  );
if (!envelopeVersionPinned)
  fail("SaveEnvelopeCodec must retain independent envelope version 1.");
const primaryKeyMatches = saveServiceSource.match(
  /\bconst\s+string\s+Key\s*=\s*"LegendsOfRus_Save_v1"\s*;/g
)?.length ?? 0;
if (primaryKeyMatches !== 1)
  fail("SaveService must retain exactly one legacy primary key literal LegendsOfRus_Save_v1.");
const compatibilityFacadePresent =
  /public\s+static\s+PlayerSaveData\s+Load\s*\(\s*\)/.test(saveServiceSource) &&
  /public\s+static\s+void\s+Save\s*\(\s*PlayerSaveData\s+\w+\s*\)/.test(
    saveServiceSource
  ) &&
  saveServiceSource.includes("LoadWithResult") &&
  saveServiceSource.includes("TrySave") &&
  saveServiceSource.includes("SaveReset");
if (!compatibilityFacadePresent)
  fail("SaveService must retain its compatibility facade and structured save/recovery APIs.");

const envelopeIntentPinned =
  /enum\s+SaveEnvelopeIntent\s*\{\s*Ordinary\s*=\s*0\s*,\s*ReplaceHistory\s*=\s*1\s*\}/.test(
    scrubCSharp(envelopeTypesSource)
  ) &&
  envelopeCodecSource.includes('"intent="') &&
  envelopeCodecSource.includes('"replace"') &&
  envelopeCodecSource.includes('"ordinary"') &&
  envelopeCodecSource.includes('"intent=ordinary"') &&
  envelopeCodecSource.includes('"intent=replace"') &&
  journalRepositorySource.includes("SaveEnvelopeIntent.ReplaceHistory") &&
  journalRepositorySource.includes("SaveEnvelopeIntent.Ordinary");
if (!envelopeIntentPinned)
  fail("Envelope intent must retain explicit ordinary and replace-history encoding and decoding.");

const payloadSizeCapsPresent =
  /MaximumPayloadCharacters\s*=\s*1024\s*\*\s*1024\s*;/.test(
    envelopeCodecSource
  ) &&
  envelopeCodecSource.includes("MaximumPayloadUtf8Bytes") &&
  envelopeCodecSource.includes("MaximumPayloadBase64Characters") &&
  envelopeCodecSource.includes("MaximumEnvelopeCharacters") &&
  envelopeCodecSource.includes("payload.Length > MaximumPayloadCharacters") &&
  envelopeCodecSource.includes("serializedEnvelope.Length > MaximumEnvelopeCharacters") &&
  envelopeCodecSource.includes("payloadByteCount > MaximumPayloadUtf8Bytes") &&
  envelopeCodecSource.includes("payloadBase64.Length > MaximumPayloadBase64Characters") &&
  /MaximumPayloadCharacters\s*=\s*SaveEnvelopeCodec\.MaximumPayloadCharacters\s*;/.test(
    payloadCodecSource
  ) &&
  payloadCodecSource.includes("serializedPayload.Length > MaximumPayloadCharacters");
if (!payloadSizeCapsPresent)
  fail("Envelope and legacy payload parsing must retain the reviewed one-megachar size boundary.");

const strictRawV6WithLegacyCompatibilityPresent =
  payloadCodecSource.includes("CurrentV6RequiredFields") &&
  payloadCodecSource.includes("duplicateFieldMask") &&
  payloadCodecSource.includes("fieldTypesValid") &&
  payloadCodecSource.includes("SAVE_PAYLOAD_SCHEMA_INVALID") &&
  payloadCodecSource.includes("SAVE_PAYLOAD_SCHEMA_INCOMPLETE") &&
  payloadCodecSource.includes(
    "version == PlayerSaveMigrationRegistry.CurrentPayloadVersion"
  ) &&
  payloadCodecSource.includes("SAVE_PAYLOAD_LEGACY_UNVERSIONED") &&
  /version\s*=\s*1\s*;/.test(payloadCodecSource) &&
  /declaredVersion\s*>\s*PlayerSaveMigrationRegistry\.CurrentPayloadVersion/.test(
    payloadCodecSource
  ) &&
  payloadCodecSource.includes("LegacyPayloadInspection.Valid(declaredVersion)") &&
  payloadCodecSource.includes("Earlier payload schemas remain intentionally permissive") &&
  payloadCodecSource.includes("TryDeserialize(");
if (!strictRawV6WithLegacyCompatibilityPresent) {
  fail(
    "Raw v6 imports must require the complete typed DTO while v1-v5 and " +
    "unversioned legacy imports retain migration compatibility."
  );
}

const canonicalPrefixIndex = envelopeCodecSource.indexOf(
  "TryInspectCanonicalPrefix"
);
const futureEnvelopeGateIndex = envelopeCodecSource.indexOf(
  "inspectedFormatVersion > CurrentEnvelopeVersion"
);
const v1SizeLayoutIndex = envelopeCodecSource.indexOf(
  "// Bound all v1 line splitting"
);
const canonicalFutureEnvelopeDowngradeBarrierPresent =
  envelopeCodecSource.includes("TryInspectCanonicalPrefix") &&
  envelopeCodecSource.includes("inspectedFormatVersion > CurrentEnvelopeVersion") &&
  envelopeCodecSource.includes("TryGetCurrentShapedLines") &&
  envelopeCodecSource.includes("ValidateCurrentStyleOuterIntegrity") &&
  envelopeCodecSource.includes("MaximumEnvelopeCharacters") &&
  envelopeCodecSource.includes("SaveEnvelopeDecodeStatus.UnsupportedEnvelopeVersion") &&
  envelopeCodecSource.includes("ENVELOPE_FORMAT_UNSUPPORTED") &&
  canonicalPrefixIndex >= 0 &&
  futureEnvelopeGateIndex > canonicalPrefixIndex &&
  v1SizeLayoutIndex > futureEnvelopeGateIndex;
if (!canonicalFutureEnvelopeDowngradeBarrierPresent) {
  fail(
    "Canonical different-shape or oversized future envelopes must remain " +
    "unsupported-future evidence and block downgrade."
  );
}

const primaryDurablyCommittedResetAdoptionPresent =
  journalRepositorySource.includes("MarkPrimaryDurablyCommitted") &&
  journalRepositorySource.includes("PrimaryDurablyCommitted") &&
  saveServiceSource.includes("journal.PrimaryDurablyCommitted") &&
  saveServiceSource.includes("result.PrimaryDurablyCommitted") &&
  gameStateSource.includes("!writeCompleted && !result.PrimaryDurablyCommitted") &&
  gameStateSource.includes("Save = reset;") &&
  gameStateSource.includes("LastWriteResult = result;") &&
  gameStateSource.includes("canPersist = writeCompleted;");
if (!primaryDurablyCommittedResetAdoptionPresent) {
  fail(
    "Reset must adopt a primary after its flush is durably committed while " +
    "reporting incomplete redundancy or cleanup."
  );
}

const decideLoadBody = extractCSharpMethodBody(
  journalRepositorySource,
  /static\s+SaveJournalLoadResult\s+DecideLoad\s*\([^)]*\)/
);
const primaryMissingIndex = decideLoadBody?.indexOf(
  "scan.Primary.State == SaveCandidateState.Missing"
) ?? -1;
const backupRecoveryIndex = decideLoadBody?.indexOf("if (scan.Backup.Valid)") ?? -1;
const primaryMissingIsExplicitNoSave = Boolean(
  decideLoadBody &&
  primaryMissingIndex >= 0 &&
  decideLoadBody.indexOf("SaveJournalLoadStatus.NoSave", primaryMissingIndex) >
    primaryMissingIndex &&
  backupRecoveryIndex > primaryMissingIndex &&
  decideLoadBody.includes("SAVE_PRIMARY_MISSING_NO_COMMITTED_SAVE") &&
  !decideLoadBody.includes("SAVE_BACKUP_RECOVERED_PRIMARY_MISSING")
);
if (!primaryMissingIsExplicitNoSave)
  fail("A missing primary must resolve to explicit NoSave before any backup recovery path.");

const replacementIntentStagingGuardPresent = Boolean(
  decideLoadBody &&
  decideLoadBody.includes("scan.Staging.Valid") &&
  decideLoadBody.includes(
    "scan.Staging.Intent == SaveEnvelopeIntent.ReplaceHistory"
  ) &&
  decideLoadBody.includes("scan.Staging.Revision > scan.Backup.Revision") &&
  decideLoadBody.includes("SaveJournalLoadStatus.Unrecoverable") &&
  decideLoadBody.includes("SAVE_HISTORY_REPLACEMENT_INTERRUPTED") &&
  decideLoadBody.includes("SAVE_BACKUP_RECOVERED_PRIMARY_INVALID")
);
if (!replacementIntentStagingGuardPresent)
  fail("A newer replace-history staging intent must block resurrection of an older backup.");

const readPathBodies = [
  extractCSharpMethodBody(
    journalRepositorySource,
    /public\s+SaveJournalLoadResult\s+Load\s*\([^)]*\)/
  ),
  extractCSharpMethodBody(
    journalRepositorySource,
    /JournalScan\s+Scan\s*\([^)]*\)/
  ),
  extractCSharpMethodBody(
    journalRepositorySource,
    /Candidate\s+ReadCandidate\s*\([^)]*\)/
  ),
  decideLoadBody,
  extractCSharpMethodBody(
    journalRepositorySource,
    /static\s+SaveJournalLoadResult\s+LoadSuccess\s*\([^)]*\)/
  ),
  extractCSharpMethodBody(
    journalRepositorySource,
    /static\s+SaveJournalLoadResult\s+LoadFailure\s*\([^)]*\)/
  )
];
const loadReadPathBodiesPresent = readPathBodies.every(
  (body) => typeof body === "string" && body.length > 0
);
const readPathSource = readPathBodies.filter(Boolean).map(scrubCSharp).join("\n");
const noLoadSideEffects = loadReadPathBodiesPresent &&
  !/\bstore\s*\.\s*(?:Write|Delete|Flush)\s*\(/.test(readPathSource) &&
  !/\bTryStore(?:Write|Delete|Flush)\s*\(/.test(readPathSource) &&
  !/\bTry(?:Write|Save|ReplaceHistory)\s*\(/.test(readPathSource) &&
  !/\bPlayerPrefs\b/.test(readPathSource);
if (!noLoadSideEffects)
  fail("Save journal Load/scan/decision path must contain no write, delete, flush, or promotion call.");

const readOnlyLoadPolicyPresent =
  envelopeIntentPinned &&
  primaryMissingIsExplicitNoSave &&
  replacementIntentStagingGuardPresent &&
  noLoadSideEffects &&
  journalRepositorySource.includes("SaveJournalLoadStatus.UnsupportedFutureVersion");
if (!readOnlyLoadPolicyPresent)
  fail("Save journal source is missing the reviewed read-only staging/future/recovery policy.");

const asmdefFiles = projectFiles.filter((file) => file.endsWith(".asmdef"));
const asmdefs = new Map();
for (const file of asmdefFiles) {
  const value = parseJson(file);
  if (!value || typeof value.name !== "string") continue;
  if (asmdefs.has(value.name)) fail(`Duplicate assembly name '${value.name}'.`);
  asmdefs.set(value.name, { file, value });
}

function validateAsmdef(file, expected) {
  const value = exists(file) ? parseJson(file) : null;
  if (!value) return false;
  const valid =
    value.name === expected.name &&
    sameSet(value.references ?? [], expected.references) &&
    sameArray(value.includePlatforms ?? [], expected.includePlatforms) &&
    sameArray(value.excludePlatforms ?? [], []) &&
    value.allowUnsafeCode === false &&
    value.overrideReferences === false &&
    sameArray(value.precompiledReferences ?? [], []) &&
    value.autoReferenced === true &&
    sameArray(value.defineConstraints ?? [], []) &&
    sameArray(value.versionDefines ?? [], []) &&
    value.noEngineReferences === expected.noEngineReferences &&
    (expected.testAssembly
      ? sameArray(value.optionalUnityReferences, ["TestAssemblies"])
      : value.optionalUnityReferences === undefined);
  if (!valid) fail(`${file} does not match the reviewed assembly shape.`);
  return valid;
}

const saveCoreAsmdefValid = validateAsmdef(saveCoreAsmdefPath, {
  name: "LegendsOfRus.SaveCore",
  references: [],
  includePlatforms: [],
  noEngineReferences: true,
  testAssembly: false
});
const saveCoreTestsAsmdefValid = validateAsmdef(saveCoreTestsAsmdefPath, {
  name: "LegendsOfRus.SaveCore.Tests",
  references: ["LegendsOfRus.SaveCore"],
  includePlatforms: ["Editor"],
  noEngineReferences: true,
  testAssembly: true
});
const migrationTestsAsmdefValid = validateAsmdef(migrationTestsAsmdefPath, {
  name: "LegendsOfRus.SaveMigration.Tests",
  references: ["LegendsOfRus.Runtime"],
  includePlatforms: ["Editor"],
  noEngineReferences: false,
  testAssembly: true
});
const runtimeAsmdef = exists(runtimeAsmdefPath) ? parseJson(runtimeAsmdefPath) : null;
const runtimeAsmdefExpectedReferences = [
  "LegendsOfRus.GameplayCore",
  "LegendsOfRus.EconomyCore",
  "LegendsOfRus.CommercialCore",
  "LegendsOfRus.SaveCore",
  "Unity.InputSystem",
  "Unity.ugui"
];
const runtimeAsmdefValid = Boolean(
  runtimeAsmdef &&
  runtimeAsmdef.name === "LegendsOfRus.Runtime" &&
  sameSet(runtimeAsmdef.references ?? [], runtimeAsmdefExpectedReferences) &&
  runtimeAsmdef.allowUnsafeCode === false &&
  runtimeAsmdef.noEngineReferences === false
);
if (!runtimeAsmdefValid)
  fail("Runtime assembly must add only LegendsOfRus.SaveCore to its reviewed references.");

let assemblyGraphAcyclic = true;
const visiting = new Set();
const visited = new Set();
function visitAssembly(name) {
  if (visited.has(name)) return;
  if (visiting.has(name)) {
    assemblyGraphAcyclic = false;
    return;
  }
  visiting.add(name);
  const entry = asmdefs.get(name);
  for (const reference of entry?.value.references ?? []) {
    if (asmdefs.has(reference)) visitAssembly(reference);
  }
  visiting.delete(name);
  visited.add(name);
}
for (const name of asmdefs.keys()) visitAssembly(name);
if (!assemblyGraphAcyclic) fail("Assembly definition graph contains a cycle.");

const testAttributePattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
function countTestAttributes(file) {
  return scrubCSharp(read(file)).match(testAttributePattern)?.length ?? 0;
}
const sourceTestAttributes = allCSharpFiles
  .filter((file) => file.includes("/Tests/"))
  .reduce((total, file) => total + countTestAttributes(file), 0);
const step2SaveTestFiles = projectFiles.filter((file) =>
  file === "Assets/LegendsOfRus/Tests/PlayMode/SaveMigrationRecoveryPlayModeTests.cs" ||
  (file.startsWith("Assets/LegendsOfRus/Tests/SaveMigrationEditMode/") &&
    file.endsWith(".cs")) ||
  (file.startsWith("Assets/LegendsOfRus/Tests/SaveRecoveryEditMode/") &&
    file.endsWith(".cs"))
);
const newTestAttributes = step2SaveTestFiles.reduce(
  (total, file) => total + countTestAttributes(file), 0
);
const inheritedSaveIsolationFixtureTestAttributes =
  inheritedSaveIsolationFixturePaths.reduce(
    (total, file) => total + (exists(file) ? countTestAttributes(file) : 0),
    0
  );
const changedTestFiles = changedCSharpFiles.filter((file) =>
  file.includes("/Tests/")
);
for (const file of changedTestFiles) {
  if (/\[\s*(?:NonParallelizable|Ignore|Explicit)\b/.test(read(file)))
    fail(`${file} contains a forbidden test annotation.`);
}
if (newTestAttributes < 1)
  fail("Step 2 save suites have no source test attributes.");
const selfTestCheckSites = exists(selfTestPath)
  ? Math.max(
      0,
      (scrubCSharp(read(selfTestPath)).match(/\bCheck\s*\(/g)?.length ?? 0) - 1
    )
  : 0;
if (selfTestCheckSites < 1)
  fail("v1.17 Step 2 Editor self-test has no Check sites.");

const currentPngs = projectFiles.filter((file) => file.toLowerCase().endsWith(".png"));
const currentPngInventorySha256 = inventoryFingerprint(root, currentPngs);
if (currentPngs.length !== PNG_FILE_COUNT)
  fail(`Expected exactly ${PNG_FILE_COUNT} approved PNGs, found ${currentPngs.length}.`);
if (currentPngInventorySha256 !== PNG_INVENTORY_SHA256)
  fail("Approved PNG inventory differs from the reviewed Step 1 bytes.");

let changedManifestValid = false;
let changedManifestPaths = [];
if (exists(changedManifestPath)) {
  const source = read(changedManifestPath);
  if (!source.endsWith("\n")) fail(`${changedManifestPath} must end with a newline.`);
  const rawLines = source.split(/\r?\n/);
  if (rawLines[rawLines.length - 1] === "") rawLines.pop();
  changedManifestPaths = rawLines;
  const validLines = changedManifestPaths.every((line) =>
    line.length > 0 &&
    line === line.trim() &&
    !line.startsWith("/") &&
    !line.includes("\\") &&
    !line.split("/").includes("..")
  );
  if (!validLines) fail(`${changedManifestPath} contains an invalid relative path.`);
  if (new Set(changedManifestPaths).size !== changedManifestPaths.length)
    fail(`${changedManifestPath} contains duplicate paths.`);
  changedManifestValid = validLines && sameArray(changedManifestPaths, allowedChangedPaths);
  if (!changedManifestValid)
    fail(`${changedManifestPath} does not equal the exact reviewed delta allowlist.`);
}

const implementation = exists(implementationStatusPath)
  ? parseJson(implementationStatusPath)
  : null;
if (implementation) {
  if (implementation.version !== VERSION ||
      implementation.status !== IMPLEMENTATION_STATUS ||
      implementation.candidate_status !== CANDIDATE_STATUS ||
      implementation.unity_execution_evidence !== "NOT_RETAINED" ||
      implementation.source_project_settings_status !== "ABSENT_FROM_SOURCE_CANDIDATE" ||
      implementation.package_lock_approved !== false ||
      implementation.player_payload_version !== PAYLOAD_VERSION ||
      implementation.storage_envelope_version !== ENVELOPE_VERSION ||
      implementation.raw_current_v6_requires_complete_typed_32_field_schema !== true ||
      implementation.legacy_unversioned_payload_supported_as_v1 !== true ||
      implementation.canonical_future_raw_blocks_downgrade !== true ||
      implementation.canonical_different_shape_or_oversized_future_envelope_blocks_downgrade !== true ||
      implementation.primary_durably_committed_reset_adoption !== true ||
      implementation.inherited_three_key_isolation_fixtures !== 12 ||
      implementation.all_other_inherited_tests_identical_to_step1 !== true ||
      implementation.legacy_game_state_mutator_boolean_is_durable_proof !== false ||
      implementation.copy_on_write_gameplay_mutation_transactions_implemented !== false ||
      implementation.closed_player_facing_transactional_testing_authorized !== false ||
      implementation.player_prefs_delete_all_used !== false ||
      implementation.cloud_save_implemented !== false ||
      implementation.account_recovery_implemented !== false ||
      implementation.economy_activation_authorized !== false ||
      implementation.commercial_activation_authorized !== false ||
      implementation.production_android_build_authorized !== false)
    fail(`${implementationStatusPath} overstates evidence, recovery scope, or activation authority.`);
}

const activationEvidence = exists(activationEvidencePath)
  ? parseJson(activationEvidencePath)
  : null;
let activationEvidenceValid = false;
if (activationEvidence) {
  const retention = activationEvidence.evidenceRetention;
  const capabilities = activationEvidence.effectiveCapabilities;
  const retentionValid = isObject(retention) &&
    Object.entries(retention).every(([key, value]) =>
      value === "NOT_RETAINED" ||
      (key === "step1Compilation" && value === "USER_REPORTED_NOT_RETAINED")
    );
  const capabilitiesValid = isObject(capabilities) &&
    capabilities.localSaveEnvelope === true &&
    capabilities.localCommittedBackupRecovery === true &&
    capabilities.transactionalGameplayMutationDurability === false &&
    capabilities.closedPlayerFacingSaveTesting === false &&
    capabilities.cloudSave === false &&
    capabilities.accountRecovery === false &&
    capabilities.durableCommercialFulfillment === false &&
    capabilities.analyticsCapture === false &&
    capabilities.analyticsUpload === false &&
    capabilities.iap === false &&
    capabilities.rewardedAds === false &&
    capabilities.productionAndroidBuild === false;
  activationEvidenceValid =
    activationEvidence.schemaVersion === 1 &&
    activationEvidence.milestone === VERSION &&
    activationEvidence.status ===
      "SOURCE_CANDIDATE_ONLY_PRODUCTION_ACTIVATION_BLOCKED" &&
    activationEvidence.implementationStatus === IMPLEMENTATION_STATUS &&
    activationEvidence.candidateStatus === CANDIDATE_STATUS &&
    activationEvidence.unityVersion === UNITY_VERSION &&
    activationEvidence.step1UserReportedZeroErrors === true &&
    retentionValid &&
    capabilitiesValid &&
    activationEvidence.packageLockApproved === false &&
    activationEvidence.productionAndroidBuildAuthorized === false &&
    activationEvidence.economyActivationAuthorized === false &&
    activationEvidence.commercialActivationAuthorized === false &&
    activationEvidence.cloudRecoveryAuthorized === false &&
    activationEvidence.publishingAuthorized === false;
  if (!activationEvidenceValid)
    fail(`${activationEvidencePath} contains an unsupported evidence or activation claim.`);
}

const staticReport = exists(staticReportPath) ? parseJson(staticReportPath) : null;
let staticReportValid = false;
if (staticReport) {
  staticReportValid =
    staticReport.version === VERSION &&
    staticReport.status === "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION" &&
    staticReport.passed === true &&
    staticReport.implementation_status === IMPLEMENTATION_STATUS &&
    staticReport.candidate_status === CANDIDATE_STATUS &&
    staticReport.unity_execution_evidence === "NOT_RETAINED" &&
    staticReport.player_payload_version === PAYLOAD_VERSION &&
    staticReport.storage_envelope_version === ENVELOPE_VERSION &&
    staticReport.package_lock_approved === false &&
    staticReport.economy_activation_authorized === false &&
    staticReport.commercial_activation_authorized === false &&
    staticReport.production_android_build_authorized === false &&
    staticReport.project_files === projectFiles.length &&
    staticReport.approved_png_files === currentPngs.length &&
    staticReport.strict_raw_v6_complete_schema_validation ===
      strictRawV6WithLegacyCompatibilityPresent &&
    staticReport.legacy_v1_v5_and_unversioned_import_compatible ===
      strictRawV6WithLegacyCompatibilityPresent &&
    staticReport.canonical_future_envelope_downgrade_barrier ===
      canonicalFutureEnvelopeDowngradeBarrierPresent &&
    staticReport.primary_durably_committed_reset_adoption ===
      primaryDurablyCommittedResetAdoptionPresent &&
    staticReport.inherited_save_isolation_fixture_count ===
      inheritedSaveIsolationFixturePaths.length &&
    staticReport.inherited_save_isolation_fixtures_valid ===
      inheritedSaveIsolationFixturesValid &&
    staticReport.all_other_inherited_tests_identical_to_v117_step01 === true &&
    sameArray(staticReport.failures, []) &&
    sameArray(staticReport.static_failures, []);
  if (!staticReportValid)
    fail(`${staticReportPath} does not match the reviewed source-only facts.`);
}

let baselineFiles = null;
let baselineTree = null;
let baselineTreeValid = null;
let baselineVerifierPassed = null;
let actualChangedPaths = null;
let protectedUnionIdentical = null;
let approvedPngsIdentical = null;
let allOtherInheritedTestsIdentical = null;
let manifestMatchesBaseline = null;

if (baseline) {
  if (!fs.existsSync(baseline) || !fs.statSync(baseline).isDirectory()) {
    fail(`Baseline '${baseline}' is not a directory.`);
  } else {
    try {
      if (fs.realpathSync(baseline) === fs.realpathSync(root))
        fail("Candidate and baseline directories must be different.");
    } catch {
      fail("Candidate or baseline real path could not be resolved.");
    }

    baselineFiles = walkProject(baseline, "baseline");
    baselineTree = treeFingerprint(baseline, baselineFiles);
    baselineTreeValid =
      baselineFiles.length === BASELINE_FILE_COUNT &&
      baselineTree === BASELINE_TREE_SHA256 &&
      exists("MILESTONE_VERSION.txt", baseline) &&
      read("MILESTONE_VERSION.txt", baseline).trim() === BASELINE_MILESTONE_ID;
    if (!baselineTreeValid)
      fail("Supplied v1.17 Step 1 baseline does not match the pinned 457-file tree.");

    if (!exists(BASELINE_VERIFIER_PATH, baseline) ||
        sha256File(BASELINE_VERIFIER_PATH, baseline) !== BASELINE_VERIFIER_SHA256) {
      fail("Supplied baseline verifier does not match the pinned v1.17 Step 1 verifier.");
      baselineVerifierPassed = false;
    } else {
      const execution = spawnSync(
        process.execPath,
        [absolute(BASELINE_VERIFIER_PATH, baseline), "--source-only"],
        { cwd: baseline, encoding: "utf8", timeout: 30000, maxBuffer: 4 * 1024 * 1024 }
      );
      try {
        const result = JSON.parse(execution.stdout);
        baselineVerifierPassed =
          !execution.error &&
          execution.signal === null &&
          execution.status === 0 &&
          result.passed === true &&
          result.source_only === true &&
          result.status === "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION" &&
          result.project_files === BASELINE_FILE_COUNT &&
          result.candidate_tree_sha256 === BASELINE_TREE_SHA256 &&
          result.verifier_sha256 === BASELINE_VERIFIER_SHA256 &&
          result.package_lock_approval_blocked === true &&
          result.package_lock_approved === false &&
          result.economy_activation_authorized === false &&
          result.commercial_activation_authorized === false &&
          sameArray(result.failures, []) &&
          sameArray(result.static_failures, []);
      } catch {
        baselineVerifierPassed = false;
      }
      if (!baselineVerifierPassed)
        fail("Pinned v1.17 Step 1 baseline source verifier did not pass cleanly.");
    }

    const baselineSet = new Set(baselineFiles);
    const candidateSet = new Set(projectFiles);
    const union = [...new Set([...baselineFiles, ...projectFiles])].sort();
    actualChangedPaths = [];
    protectedUnionIdentical = true;
    approvedPngsIdentical = true;
    allOtherInheritedTestsIdentical = true;
    for (const file of union) {
      const inBaseline = baselineSet.has(file);
      const inCandidate = candidateSet.has(file);
      const deleted = inBaseline && !inCandidate;
      const added = !inBaseline && inCandidate;
      const modified = inBaseline && inCandidate &&
        sha256File(file, baseline) !== sha256File(file, root);
      const differs = deleted || added || modified;
      if (!differs) continue;

      actualChangedPaths.push(file);
      if (isProtectedPath(file)) protectedUnionIdentical = false;
      if (file.toLowerCase().endsWith(".png")) approvedPngsIdentical = false;
      if (inBaseline && file.includes("/Tests/") &&
          !inheritedSaveIsolationFixtureSet.has(file)) {
        allOtherInheritedTestsIdentical = false;
      }

      if (deleted) {
        fail(`Candidate deleted inherited path '${file}'.`);
        continue;
      }
      if (!allowedChangedPaths.includes(file))
        fail(`Candidate changed non-allowlisted path '${file}'.`);
    }
    if (!protectedUnionIdentical)
      fail("A protected config, resource, manifest, scene, package, activation, or Android path differs from Step 1.");
    if (!approvedPngsIdentical)
      fail("An approved PNG differs from the v1.17 Step 1 baseline.");
    if (!allOtherInheritedTestsIdentical) {
      fail(
        "An inherited test outside the 12 reviewed save-isolation fixtures " +
        "differs from the v1.17 Step 1 baseline."
      );
    }
    manifestMatchesBaseline = sameArray(actualChangedPaths, changedManifestPaths);
    if (!manifestMatchesBaseline)
      fail("Changed-file manifest does not equal the computed v1.17 Step 1 delta.");
  }
}

const candidateTreeSha256 = treeFingerprint(root, projectFiles);
const verifierSha256 = exists(VERIFIER_PATH) ? sha256File(VERIFIER_PATH) : null;
const passed = failures.length === 0;
const releaseBoundaryVerified = Boolean(
  passed && baseline && baselineTreeValid && baselineVerifierPassed &&
  protectedUnionIdentical && approvedPngsIdentical &&
  inheritedSaveIsolationFixturesValid && allOtherInheritedTestsIdentical &&
  manifestMatchesBaseline && reviewedCandidateBytesExact
);
const status = passed
  ? (baseline
      ? "PASS_STATIC_AND_PROTECTED_BASELINE_VERIFICATION"
      : "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION")
  : "FAIL_SOURCE_OR_BOUNDARY_VERIFICATION";

const report = {
  version: VERSION,
  status,
  passed,
  implementation_status: IMPLEMENTATION_STATUS,
  candidate_status: CANDIDATE_STATUS,
  unity_execution_evidence: "NOT_RETAINED",
  unity_compile_status: "NOT_RETAINED",
  project_validator_execution_status: "NOT_RETAINED",
  editor_self_test_execution_status: "NOT_RETAINED",
  editmode_execution_status: "NOT_RETAINED",
  playmode_execution_status: "NOT_RETAINED",
  crash_interruption_execution_status: "NOT_RETAINED",
  representative_device_status: "NOT_RETAINED",
  source_project_settings_status: "ABSENT_FROM_SOURCE_CANDIDATE",
  source_packages_lock_status: "ABSENT_UNREVIEWED",
  package_lock_approval_blocked: packageLockApprovalBlocked,
  package_lock_approved: packageLockApprovalBlocked ? false : null,
  player_payload_version: payloadVersionPinned ? PAYLOAD_VERSION : null,
  storage_envelope_version: envelopeVersionPinned ? ENVELOPE_VERSION : null,
  primary_save_key_preserved: primaryKeyMatches === 1,
  player_prefs_delete_all_used: !playerPrefsDeleteAllAbsent,
  cloud_or_provider_runtime_wiring_added: !forbiddenProductionTokensAbsent,
  economy_activation_authorized: false,
  commercial_activation_authorized: false,
  production_android_build_authorized: false,
  publishing_authorized: false,
  exact_unity_version: true,
  source_only: sourceOnly,
  release_boundary_verified: releaseBoundaryVerified,
  final_review_pins_ready: finalPinsReady,
  reviewed_candidate_bytes_exact: reviewedCandidateBytesExact,
  reviewed_project_file_count_pin: REVIEWED_PROJECT_FILE_COUNT,
  reviewed_candidate_excluding_verifier_sha256_pin:
    REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256,
  project_files: projectFiles.length,
  project_json_files: allJsonFiles.length,
  project_asmdef_files: asmdefFiles.length,
  csharp_structural_scan_files: changedCSharpFiles.length,
  candidate_tree_sha256: candidateTreeSha256,
  verifier_sha256: verifierSha256,
  protected_prefix_results: protectedPrefixResults,
  approved_png_files: currentPngs.length,
  approved_png_inventory_sha256: currentPngInventorySha256,
  package_manifest_sha256: exists("Packages/manifest.json")
    ? sha256File("Packages/manifest.json") : null,
  project_version_sha256: exists("ProjectSettings/ProjectVersion.txt")
    ? sha256File("ProjectSettings/ProjectVersion.txt") : null,
  editor_build_settings_sha256: exists("ProjectSettings/EditorBuildSettings.asset")
    ? sha256File("ProjectSettings/EditorBuildSettings.asset") : null,
  economy_activation_sha256: exists(economyActivationPath)
    ? sha256File(economyActivationPath) : null,
  commercial_activation_sha256: exists(commercialActivationPath)
    ? sha256File(commercialActivationPath) : null,
  commercial_config_sha256: exists(commercialConfigPath)
    ? sha256File(commercialConfigPath) : null,
  economy_activation_disabled: economyActivationDisabled,
  commercial_activation_disabled: commercialActivationDisabled,
  commercial_config_disabled: commercialConfigDisabled,
  resolved_package_lock_approved_compiled_false: packageLockApprovalBlocked,
  production_android_build_compiled_false: productionAndroidBuildCompiled,
  external_provider_activation_compiled_false: externalProviderActivationCompiled,
  compatibility_facade_present: compatibilityFacadePresent,
  envelope_intent_pinned: envelopeIntentPinned,
  payload_size_caps_present: payloadSizeCapsPresent,
  strict_raw_v6_with_v1_v5_unversioned_compatibility_present:
    strictRawV6WithLegacyCompatibilityPresent,
  canonical_future_envelope_downgrade_barrier_present:
    canonicalFutureEnvelopeDowngradeBarrierPresent,
  primary_durably_committed_reset_adoption_present:
    primaryDurablyCommittedResetAdoptionPresent,
  primary_missing_is_explicit_no_save: primaryMissingIsExplicitNoSave,
  replacement_intent_staging_guard_present:
    replacementIntentStagingGuardPresent,
  load_path_has_no_write_delete_or_flush: noLoadSideEffects,
  read_only_load_policy_present: readOnlyLoadPolicyPresent,
  save_core_asmdef_valid: saveCoreAsmdefValid,
  save_core_tests_asmdef_valid: saveCoreTestsAsmdefValid,
  migration_tests_asmdef_valid: migrationTestsAsmdefValid,
  runtime_asmdef_valid: runtimeAsmdefValid,
  assembly_graph_acyclic: assemblyGraphAcyclic,
  source_test_attributes_informational: sourceTestAttributes,
  new_step2_test_attributes_informational: newTestAttributes,
  inherited_save_isolation_fixture_test_attributes_informational:
    inheritedSaveIsolationFixtureTestAttributes,
  inherited_save_isolation_fixture_count:
    inheritedSaveIsolationFixturePaths.length,
  inherited_save_isolation_fixtures_valid:
    inheritedSaveIsolationFixturesValid,
  editor_self_test_check_sites_informational: selfTestCheckSites,
  source_counts_are_execution_evidence: false,
  static_report_present: exists(staticReportPath),
  static_report_valid: staticReportValid,
  activation_evidence_present: exists(activationEvidencePath),
  activation_evidence_valid: activationEvidenceValid,
  changed_file_manifest_present: exists(changedManifestPath),
  changed_file_manifest_valid: changedManifestValid,
  allowed_changed_paths: allowedChangedPaths,
  baseline_supplied: Boolean(baseline),
  baseline_file_count: baselineFiles?.length ?? null,
  baseline_tree_sha256: baselineTree,
  baseline_tree_valid: baselineTreeValid,
  baseline_source_verifier_passed: baselineVerifierPassed,
  protected_union_identical: protectedUnionIdentical,
  approved_pngs_identical_to_v117_step01: approvedPngsIdentical,
  all_other_inherited_tests_identical_to_v117_step01:
    allOtherInheritedTestsIdentical,
  actual_changed_paths: actualChangedPaths,
  changed_file_manifest_matches_v117_step01: manifestMatchesBaseline,
  failures,
  static_failures: staticFailures
};

console.log(JSON.stringify(report, null, 2));
if (!passed) process.exitCode = 1;
