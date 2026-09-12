import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { spawnSync } from "node:child_process";

const VERSION = "v1.17-step01";
const MILESTONE_ID =
  "v1.17-step01-android-build-configuration-signing-foundation";
const IMPLEMENTATION_STATUS =
  "IMPLEMENTED_IN_SOURCE_PENDING_UNITY_ANDROID_CONFIGURATION_AND_VERIFICATION";
const ANDROID_CANDIDATE_STATUS =
  "DEVELOPMENT_APK_GUARD_READY_PACKAGE_LOCK_APPROVAL_PENDING_PRODUCTION_BLOCKED";
const UNITY_VERSION = "6000.5.9f1";
const POLICY_SHA256 =
  "95eec67ffb9ef8e45ad657524cbfcaa3c852258f4de2c7a54dc53ad18f96751c";
const BASELINE_FILE_COUNT = 429;
const BASELINE_TREE_SHA256 =
  "550afe53ddc4bd55bb84f432d14bade1ed065f6a1e5346639d1ebc067a81afc7";
const BASELINE_VERIFIER_SHA256 =
  "8fca030aad27fe00f42e7bae68e6c539c41b880a9302a25024ff2ac77612810a";
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
const REVIEWED_PROJECT_FILE_COUNT = 457;
const REVIEWED_PROJECT_JSON_FILE_COUNT = 78;
const REVIEWED_PROJECT_ASMDEF_FILE_COUNT = 15;
const REVIEWED_CSHARP_FILE_COUNT = 191;
const REVIEWED_SOURCE_TEST_ATTRIBUTE_COUNT = 483;
const REVIEWED_NEW_EDITMODE_TEST_ATTRIBUTE_COUNT = 53;
const REVIEWED_SELF_TEST_CHECK_SITE_COUNT = 30;
const VERIFIER_PATH = "Tools/verify_v1_17_step01.mjs";
const REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256 =
  "cc17fff27e1253a18f87469e108a9096a4f074ad394ecd61594954c81fb79695";

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
  fail("Release-boundary verification requires --baseline <v1.16-step02-project>.");

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
    } catch (error) {
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
        const childLower = child.toLocaleLowerCase("en-US");
        if (childLower === "assets/plugins" ||
            childLower.startsWith("assets/plugins/"))
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

function treeFingerprint(directory, files) {
  const hash = crypto.createHash("sha256");
  for (const file of files) {
    hash.update(file);
    hash.update("\0");
    hash.update(sha256File(file, directory));
    hash.update("\n");
  }
  return hash.digest("hex");
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

function exactKeys(value, keys, label) {
  if (!isObject(value)) {
    fail(`${label} must be a JSON object.`);
    return false;
  }
  const actual = Object.keys(value).sort();
  const expected = [...keys].sort();
  if (!sameArray(actual, expected)) {
    fail(`${label} keys do not match the reviewed schema.`);
    return false;
  }
  return true;
}

// JSON.parse silently accepts duplicate keys. This scanner rejects them first.
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
    else if (pairs.has(char)) {
      if (stack.pop() !== pairs.get(char)) {
        fail(`${relative} has unbalanced C# delimiters.`, true);
        break;
      }
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

const changedManifestPath =
  "Assets/LegendsOfRus/Documentation/V1_17_STEP01_CHANGED_FILES.txt";
const staticReportPath =
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_17_STEP01.json";
const activationEvidencePath =
  "Assets/LegendsOfRus/Documentation/V1_17_STEP01_ACTIVATION_EVIDENCE.json";
const implementationStatusPath =
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_17_STEP01.json";
const policyPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Config/android_build_foundation_policy.json";
const coreAsmdefPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Core/LegendsOfRus.AndroidBuildCore.Editor.asmdef";
const editorAsmdefPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Unity/LegendsOfRus.AndroidBuild.Editor.asmdef";
const testAsmdefPath =
  "Assets/LegendsOfRus/Tests/AndroidBuildEditMode/LegendsOfRus.AndroidBuild.Editor.Tests.asmdef";
const guardPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildGuard.cs";
const evaluatorPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Core/AndroidBuildPolicyEvaluator.cs";
const reasonCodesPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Core/AndroidBuildReasonCodes.cs";
const stateReaderPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildStateReader.cs";
const loaderPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildPolicyLoader.cs";
const evidenceSourcePath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildEvidence.cs";
const menusPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildMenus.cs";
const editorAssemblyAccessPath =
  "Assets/LegendsOfRus/Editor/AndroidBuild/Unity/AndroidBuildEditorAssemblyAccess.cs";
const selfTestPath =
  "Assets/LegendsOfRus/Scripts/Editor/Step17AndroidBuildSigningFoundationSelfTest.cs";

const allowedChangedPaths = [
  "Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
  implementationStatusPath,
  "Assets/LegendsOfRus/Documentation/NEXT_CHECKPOINT_V1_17_STEP01_ANDROID_VERIFICATION.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_17_STEP02.json",
  "Assets/LegendsOfRus/Documentation/README_FIRST.md",
  staticReportPath,
  activationEvidencePath,
  "Assets/LegendsOfRus/Documentation/V1_17_STEP01_ANDROID_BUILD_CONFIGURATION_SIGNING_FOUNDATION.md",
  changedManifestPath,
  policyPath,
  "Assets/LegendsOfRus/Editor/AndroidBuild/Core/AndroidBuildCoreAssemblyAccess.cs",
  "Assets/LegendsOfRus/Editor/AndroidBuild/Core/AndroidBuildFoundationPolicy.cs",
  evaluatorPath,
  "Assets/LegendsOfRus/Editor/AndroidBuild/Core/AndroidBuildReasonCodes.cs",
  coreAsmdefPath,
  editorAssemblyAccessPath,
  evidenceSourcePath,
  guardPath,
  menusPath,
  loaderPath,
  stateReaderPath,
  editorAsmdefPath,
  selfTestPath,
  "Assets/LegendsOfRus/Tests/AndroidBuildEditMode/AndroidBuildEvidenceEditModeTests.cs",
  "Assets/LegendsOfRus/Tests/AndroidBuildEditMode/AndroidBuildGuardEditModeTests.cs",
  "Assets/LegendsOfRus/Tests/AndroidBuildEditMode/AndroidBuildPolicyEditModeTests.cs",
  "Assets/LegendsOfRus/Tests/AndroidBuildEditMode/AndroidBuildTestFactory.cs",
  testAsmdefPath,
  "MILESTONE_VERSION.txt",
  "README.md",
  "README_V1_17_STEP01.md",
  "Tools/verify_v1_17_step01.mjs"
].sort();

const requiredArtifacts = new Set(allowedChangedPaths);
const projectFiles = walkProject(root, "candidate");
const projectFileSet = new Set(projectFiles);
const reviewedCandidateFiles = projectFiles.filter((file) => file !== VERIFIER_PATH);
const reviewedCandidateBytesExact =
  projectFiles.length === REVIEWED_PROJECT_FILE_COUNT &&
  reviewedCandidateFiles.length === REVIEWED_PROJECT_FILE_COUNT - 1 &&
  treeFingerprint(root, reviewedCandidateFiles) ===
    REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256;
if (!reviewedCandidateBytesExact)
  fail("Candidate files outside the verifier differ from the exact reviewed source boundary.", true);
for (const relative of requiredArtifacts) {
  if (!projectFileSet.has(relative)) fail(`Required artifact '${relative}' is missing.`);
}

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

const protectedHashes = new Map([
  ["Packages/manifest.json", PACKAGE_MANIFEST_SHA256],
  ["ProjectSettings/ProjectVersion.txt", PROJECT_VERSION_SHA256],
  ["ProjectSettings/EditorBuildSettings.asset", EDITOR_BUILD_SETTINGS_SHA256],
  ["Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json", ECONOMY_ACTIVATION_SHA256],
  ["Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json", COMMERCIAL_ACTIVATION_SHA256],
  ["Assets/LegendsOfRus/Resources/Config/commercial_runtime_config.json", COMMERCIAL_CONFIG_SHA256]
]);
for (const [relative, expected] of protectedHashes) {
  if (!exists(relative)) fail(`Protected file '${relative}' is missing.`);
  else if (sha256File(relative) !== expected)
    fail(`Protected file '${relative}' changed from the reviewed bytes.`);
}

const criticalSourceHashes = new Map([
  ["Assets/LegendsOfRus/Editor/AndroidBuild/Core/AndroidBuildFoundationPolicy.cs",
    "98d671719060520cf9e4bd25b39f748887bed73d9fc8c5175fc66a00007e2b07"],
  [evaluatorPath,
    "3e3b6c606b2ea51bec4beadf30276e1b9f4070d8d52d5003334386d84eedc64f"],
  [reasonCodesPath,
    "85a007c88560318438d93b1ec43aef82386580948a12baed78d4b366c0f1c4d3"],
  [guardPath,
    "5dc44265cc63439bdb34cfd4d0546aa10ec79ecac3cf69d198d9245cd0e9198a"],
  [loaderPath,
    "3a171aa64b1b35b7934f416ef1ec93e310e53e66702b55355c1f5113213f467f"],
  [stateReaderPath,
    "b8327dabce54529d2243edf4532df6ee6026ace8b87edf14e5abf6525db84df8"],
  [evidenceSourcePath,
    "8c69d0493e521f2f7ad8d4a03b588be581b3ad441a837225026ec88e0b1f2b2e"],
  [menusPath,
    "ac2c11b5e6225cdd9424b3a82749f22d8b40885cce2eaf7745b8d952f5771b8a"],
  [selfTestPath,
    "a35b374ba34c7e4906b834c41546f6f5d11885b8ad36fbe6cb70ad1d435f0802"]
]);
for (const [relative, expected] of criticalSourceHashes) {
  if (!exists(relative) || sha256File(relative) !== expected)
    fail(`Security-critical source '${relative}' changed from the reviewed bytes.`);
}

const allJsonFiles = projectFiles.filter((file) => file.endsWith(".json"));
if (allJsonFiles.length !== REVIEWED_PROJECT_JSON_FILE_COUNT)
  fail(`Expected exactly ${REVIEWED_PROJECT_JSON_FILE_COUNT} JSON files, found ${allJsonFiles.length}.`, true);
for (const file of allJsonFiles) parseJson(file);

const policyKeys = [
  "schemaVersion", "policyId", "unityVersion", "mode", "requiredScene",
  "developmentApplicationIdentifier", "developmentBundleVersion",
  "developmentBundleVersionCode", "developmentOutputRelativePath",
  "requiredTargetApiLevel", "minimumSupportedApiLevel", "maximumVersionCode",
  "requireIl2Cpp", "requireArm64", "requireGameCategory", "requirePortrait",
  "allowDevelopmentApk", "allowReleaseApk", "allowAppBundle",
  "allowGradleExport", "allowCustomSigning",
  "productionApplicationIdentifierApproved", "versioningApproved",
  "sdkPolicyApproved", "signingIdentityApproved", "playAppSigningApproved",
  "nativePluginInventoryApproved", "sixteenKbPageCompatibilityVerified",
  "dataSafetyApproved", "productionBuildApproved"
];
const policy = exists(policyPath) ? parseJson(policyPath) : null;
const expectedPolicy = {
  schemaVersion: 1,
  policyId: "v1.17-step01-android-build-signing-foundation",
  unityVersion: UNITY_VERSION,
  mode: "development-apk-only",
  requiredScene: "Assets/Scenes/Bootstrap.unity",
  developmentApplicationIdentifier: "com.example.legendsofrus.v117debug",
  developmentBundleVersion: "0.0.0",
  developmentBundleVersionCode: 1,
  developmentOutputRelativePath:
    "Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk",
  requiredTargetApiLevel: 36,
  minimumSupportedApiLevel: 26,
  maximumVersionCode: 2100000000,
  requireIl2Cpp: true,
  requireArm64: true,
  requireGameCategory: true,
  requirePortrait: true,
  allowDevelopmentApk: true,
  allowReleaseApk: false,
  allowAppBundle: false,
  allowGradleExport: false,
  allowCustomSigning: false,
  productionApplicationIdentifierApproved: false,
  versioningApproved: false,
  sdkPolicyApproved: false,
  signingIdentityApproved: false,
  playAppSigningApproved: false,
  nativePluginInventoryApproved: false,
  sixteenKbPageCompatibilityVerified: false,
  dataSafetyApproved: false,
  productionBuildApproved: false
};
if (policy) {
  exactKeys(policy, policyKeys, policyPath);
  if (!sameArray(Object.keys(policy), policyKeys))
    fail(`${policyPath} field order differs from the reviewed schema.`);
  if (JSON.stringify(policy) !== JSON.stringify(expectedPolicy))
    fail(`${policyPath} values differ from the reviewed development-only policy.`);
}
if (!exists(policyPath) || sha256File(policyPath) !== POLICY_SHA256)
  fail(`${policyPath} SHA-256 does not match the pinned policy.`);

const prohibitedExtensions = new Set([
  ".aar", ".aab", ".apk", ".apks", ".dll", ".jar", ".java", ".jks",
  ".key", ".keystore", ".kt", ".p12", ".pem", ".pfx", ".rsp", ".so",
  ".unitypackage", ".xapk"
]);
const prohibitedNames = new Set([
  "androidmanifest.xml", "baseprojecttemplate.gradle", "gradle.properties",
  "gradletemplate.properties", "google-services.json", "googleService-info.plist",
  "launchertemplate.gradle", "local.properties", "maintemplate.gradle",
  "settingstemplate.gradle"
].map((value) => value.toLowerCase()));
const assetsScriptableObjects = projectFiles.filter((file) =>
  file.startsWith("Assets/") && file.toLowerCase().endsWith(".asset")
);
if (assetsScriptableObjects.length !== 0)
  fail("Assets must contain no persistent ScriptableObject asset, including a custom Build Profile, in Step 1.");
for (const file of projectFiles) {
  const lower = file.toLowerCase();
  const extension = path.extname(lower);
  if (prohibitedExtensions.has(extension))
    fail(`Prohibited signing, plugin, or build artifact '${file}' is present.`);
  if (prohibitedNames.has(path.basename(lower)))
    fail(`Prohibited Android configuration artifact '${file}' is present.`);
  if (lower === ".env" || lower.startsWith(".env.") || lower.includes("/.env"))
    fail(`Environment secret file '${file}' is not allowed.`);
  if (lower.startsWith("assets/plugins/") || lower === "assets/plugins")
    fail(`Plugin path '${file}' is not allowed in this milestone.`);
  if (lower.split("/").some((segment) => segment.endsWith(".androidlib")))
    fail(`Android library project path '${file}' is not allowed in this milestone.`);
  if (lower.endsWith(".asmref"))
    fail(`Assembly reference '${file}' is not allowed in this milestone.`);
}

const changedTextExtensions = new Set([
  ".cs", ".json", ".md", ".mjs", ".txt", ".asmdef"
]);
for (const file of allowedChangedPaths) {
  if (!exists(file) || !changedTextExtensions.has(path.extname(file).toLowerCase()))
    continue;
  const source = read(file);
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
  if (highConfidenceSecretPatterns.some((pattern) => pattern.test(source)))
    fail(`${file} contains a prohibited credential or high-confidence secret pattern.`);
}

const newCSharpFiles = allowedChangedPaths.filter((file) => file.endsWith(".cs"));
for (const file of newCSharpFiles) structuralCSharpCheck(file);
const newCSharpSource = newCSharpFiles.map((file) => read(file)).join("\n");
const scrubbedNewCSharp = newCSharpFiles
  .map((file) => scrubCSharp(read(file))).join("\n");
const prohibitedSourceTokens = [
  "BuildPipeline.BuildPlayer(",
  "BuildPlayerWindow.RegisterBuildPlayerHandler",
  "IPostprocessBuildWithReport",
  "OnPostGenerateGradleAndroidProject",
  "PlayerSettings.Android.keystoreName",
  "PlayerSettings.Android.keystorePass",
  "PlayerSettings.Android.keyaliasName",
  "PlayerSettings.Android.keyaliasPass",
  "Environment.GetEnvironmentVariable",
  "Environment.GetCommandLineArgs",
  "EditorPrefs",
  "SessionState",
  "PlayerPrefs",
  "System.Diagnostics.Process",
  "UnityWebRequest",
  "HttpClient",
  "WebClient",
  "AssetDatabase.SaveAssets",
  "File.WriteAllText",
  "File.WriteAllBytes",
  "RuntimeInitializeOnLoadMethod",
  ": MonoBehaviour"
];
for (const token of prohibitedSourceTokens) {
  if (newCSharpSource.includes(token))
    fail(`New Android source contains prohibited token '${token}'.`);
}
if (/PlayerSettings\.(?:Android\.)?[A-Za-z0-9_]+\s*=(?!=)/.test(scrubbedNewCSharp))
  fail("New Android source assigns a PlayerSettings property.");
if (/EditorUserBuildSettings\.[A-Za-z0-9_]+\s*=(?!=)/.test(scrubbedNewCSharp))
  fail("New Android source assigns an EditorUserBuildSettings property.");
if (/PlayerSettings\.Set[A-Za-z0-9_]+\s*\(/.test(scrubbedNewCSharp))
  fail("New Android source calls a PlayerSettings setter.");

const useCustomKeystoreReads =
  newCSharpSource.match(/PlayerSettings\.Android\.useCustomKeystore/g)?.length ?? 0;
if (useCustomKeystoreReads !== 1)
  fail(`Expected exactly one non-sensitive useCustomKeystore read, found ${useCustomKeystoreReads}.`);

const guardSource = exists(guardPath) ? read(guardPath) : "";
const evaluatorSource = exists(evaluatorPath) ? read(evaluatorPath) : "";
const reasonCodesSource = exists(reasonCodesPath) ? read(reasonCodesPath) : "";
const stateReaderSource = exists(stateReaderPath) ? read(stateReaderPath) : "";
const evidenceSource = exists(evidenceSourcePath) ? read(evidenceSourcePath) : "";
const menusSource = exists(menusPath) ? read(menusPath) : "";
const editorAssemblyAccessSource = exists(editorAssemblyAccessPath)
  ? read(editorAssemblyAccessPath) : "";
const allCSharpFiles = projectFiles.filter((file) => file.endsWith(".cs"));
if (allCSharpFiles.length !== REVIEWED_CSHARP_FILE_COUNT)
  fail(`Expected exactly ${REVIEWED_CSHARP_FILE_COUNT} C# files, found ${allCSharpFiles.length}.`, true);
const allCSharpSource = allCSharpFiles.map((file) => read(file)).join("\n");
const preprocessorImplementations =
  allCSharpSource.match(/:\s*IPreprocessBuildWithReport\b/g)?.length ?? 0;
const buildPlayerProcessorImplementations =
  allCSharpSource.match(/:\s*BuildPlayerProcessor\b/g)?.length ?? 0;
if (preprocessorImplementations !== 1)
  fail(`Expected exactly one IPreprocessBuildWithReport implementation, found ${preprocessorImplementations}.`);
if (buildPlayerProcessorImplementations !== 1)
  fail(`Expected exactly one BuildPlayerProcessor implementation, found ${buildPlayerProcessorImplementations}.`);
if (!/private\s+const\s+bool\s+ProductionAndroidBuildCompiled\s*=\s*false\s*;/.test(guardSource))
  fail("AndroidBuildGuard must retain a private const false production switch.");
if (!/callbackOrder[\s\S]*?int\.MaxValue/.test(guardSource))
  fail("AndroidBuildGuard must run at int.MaxValue callback order.");
if (!/throw\s+Denied\s*\(/.test(guardSource) ||
    !/BuildFailedException/.test(guardSource))
  fail("AndroidBuildGuard must deny with BuildFailedException.");
if (!/report\.summary\.platform\s*!=\s*BuildTarget\.Android/.test(guardSource))
  fail("AndroidBuildGuard must pass through non-Android builds explicitly.");
if (!/public\s+sealed\s+class\s+AndroidBuildPlayerOptionsGuard\s*:\s*BuildPlayerProcessor/.test(guardSource) ||
    !/PrepareForBuild\s*\(\s*BuildPlayerContext\s+buildPlayerContext\s*\)/.test(guardSource) ||
    !/options\.target\s*!=\s*BuildTarget\.Android/.test(guardSource) ||
    !/options\.extraScriptingDefines\s*!=\s*null/.test(guardSource) ||
    !/options\.extraScriptingDefines\.Length\s*!=\s*0/.test(guardSource) ||
    !guardSource.includes("AndroidBuildReasonCodes.ScriptingDefinesUnsafe"))
  fail("Android BuildPlayerOptions guard does not fail closed on scheduled Android scripting defines.");
for (const token of [
  "options.targetGroup != BuildTargetGroup.Android",
  "options.subtarget != 0",
  "options.options != BuildOptions.Development",
  "BuildOptions.ShowBuiltPlayer",
  "options.options != uiDevelopment",
  "options.scenes.Length != 1",
  "policy.requiredScene",
  "options.assetBundleManifestPath",
  "AndroidBuildStateReader.ScheduledOutputIsExact(",
  "AndroidBuildReasonCodes.ScheduledTargetMismatch",
  "AndroidBuildReasonCodes.BuildOptionsUnsafe",
  "AndroidBuildReasonCodes.BootstrapSceneMismatch",
  "AndroidBuildReasonCodes.AssetBundleManifestBlocked",
  "AndroidBuildReasonCodes.OutputPathInvalid"
]) {
  if (!guardSource.includes(token))
    fail(`Scheduled Android BuildPlayerOptions contract is missing '${token}'.`);
}
for (const fact of [
  "CustomSigning", "GradleExport", "AppBundle",
  "SplitApkPerCpuArchitecture", "SplitApplicationBinary"
]) {
  if (!new RegExp(`if\\s*\\(facts\\.${fact}\\)`).test(evaluatorSource))
    fail(`AndroidBuildPolicyEvaluator must unconditionally deny ${fact}.`);
}
if (!/if\s*\(\s*!facts\.DevelopmentBuild\s*\)/.test(evaluatorSource))
  fail("AndroidBuildPolicyEvaluator must gate every non-Development Android build.");
const packageLockApprovalBlocked =
  /private\s+const\s+bool\s+ResolvedPackageLockApproved\s*=\s*false\s*;/.test(
    stateReaderSource
  ) &&
  stateReaderSource.includes(
    "facts.PackageLockApproved = ResolvedPackageLockApproved;"
  ) &&
  /if\s*\(\s*!facts\.PackageLockApproved\s*\)/.test(evaluatorSource) &&
  evaluatorSource.includes("AndroidBuildReasonCodes.PackageLockUnapproved") &&
  reasonCodesSource.includes('"ANDROID_PACKAGE_LOCK_UNAPPROVED"') &&
  evidenceSource.includes("public bool packageLockApproved;") &&
  evidenceSource.includes("facts.PackageLockApproved");
if (!packageLockApprovalBlocked)
  fail("The unresolved package lock must remain a compiled fail-closed approval gate with a sanitized denial reason and evidence field.");
for (const approval of [
  "productionApplicationIdentifierApproved", "versioningApproved",
  "sdkPolicyApproved", "signingIdentityApproved", "playAppSigningApproved",
  "nativePluginInventoryApproved", "sixteenKbPageCompatibilityVerified",
  "dataSafetyApproved", "productionBuildApproved"
]) {
  if (!evaluatorSource.includes(`policy.${approval}`))
    fail(`Production approval conjunction is missing '${approval}'.`);
}
if (!stateReaderSource.includes("AndroidSdkVersions") &&
    !stateReaderSource.includes("requiredTargetApiLevel"))
  fail("Android settings reader does not enforce the reviewed target API policy.");
if (!stateReaderSource.includes("AndroidArchitecture.ARM64") ||
    !stateReaderSource.includes("ScriptingImplementation.IL2CPP"))
  fail("Android settings reader must require ARM64 and IL2CPP.");
if (!stateReaderSource.includes("buildApkPerCpuArchitecture") ||
    !stateReaderSource.includes("splitApplicationBinary"))
  fail("Android settings reader must reject both split-output modes.");
if (!stateReaderSource.includes('PlayerSettings.Android.appCategory') ||
    !stateReaderSource.includes('"game"') ||
    !stateReaderSource.includes("UIOrientation.Portrait"))
  fail("Android settings reader must validate game category and Portrait.");
if (!stateReaderSource.includes("BuildProfile.GetActiveBuildProfile() != null") ||
    !stateReaderSource.includes('AssetDatabase.FindAssets("t:BuildProfile")') ||
    !stateReaderSource.includes("profileGuids.Length == 0"))
  fail("Android settings reader must reject active and persistent custom Build Profiles.");
if (!stateReaderSource.includes("EnumerateFileSystemInfos()") ||
    !stateReaderSource.includes("match.Attributes") ||
    !stateReaderSource.includes("FileAttributes.ReparsePoint") ||
    stateReaderSource.includes("if (File.Exists(filePath))"))
  fail("Android output validation must inspect existing path entries, including dangling reparse points, without File.Exists gating.");
const nativeDebugSymbolsGuarded =
  stateReaderSource.includes(
    "UnityEditor.Android.UserBuildSettings+DebugSymbols"
  ) &&
  stateReaderSource.includes("Unity.Android.Types.DebugSymbolLevel") &&
  stateReaderSource.includes("Unity.Android.Types.DebugSymbolFormat") &&
  stateReaderSource.includes('Enum.Parse(levelType, "None", false)') &&
  stateReaderSource.includes('"Zip", "IncludeInBundle", "LegacyExtensions"') &&
  evaluatorSource.includes("!facts.NativeDebugSymbolsDisabled") &&
  evaluatorSource.includes("AndroidBuildReasonCodes.NativeDebugSymbolsBlocked") &&
  evidenceSource.includes("nativeDebugSymbolsDisabled") &&
  menusSource.includes("Native Debug Symbols: None");
if (!nativeDebugSymbolsGuarded)
  fail("Android native debug symbols must be read through the fail-closed Unity 6000.5 API and pinned to None.");
const androidLibraryProjectsBlocked =
  stateReaderSource.includes('"AndroidManifest.xml"') &&
  stateReaderSource.includes("Directory.EnumerateDirectories(") &&
  stateReaderSource.includes('".androidlib"') &&
  stateReaderSource.includes("IsForbiddenAndroidLibraryDirectoryName(") &&
  read(VERIFIER_PATH).includes('entry.name.toLowerCase().endsWith(".androidlib")');
if (!androidLibraryProjectsBlocked)
  fail("Android library projects and manifest overrides must be blocked by the live build reader.");
const embeddedPackagesBlocked =
  stateReaderSource.includes("NoEmbeddedPackages(root)") &&
  stateReaderSource.includes("Directory.EnumerateDirectories(packages)") &&
  read(VERIFIER_PATH).includes('if (relative === "Packages")');
if (!embeddedPackagesBlocked)
  fail("Embedded Unity packages must be blocked by both live and static boundaries.");
const managedPluginExtensionsBlocked =
  stateReaderSource.includes('".dll"') &&
  stateReaderSource.includes('".unitypackage"') &&
  stateReaderSource.includes('".xapk"') &&
  stateReaderSource.includes('".java"') &&
  stateReaderSource.includes('".kt"') &&
  prohibitedExtensions.has(".dll") &&
  prohibitedExtensions.has(".unitypackage") &&
  prohibitedExtensions.has(".xapk") &&
  prohibitedExtensions.has(".java") &&
  prohibitedExtensions.has(".kt") &&
  stateReaderSource.includes("IsForbiddenAndroidArtifactExtension(");
if (!managedPluginExtensionsBlocked)
  fail("The live and static Android artifact scans must mirror managed, packaged, Java, and Kotlin plugin extensions.");
const obsoleteAndroidIsGameCompatibilityAliasConfirmed =
  stateReaderSource.includes("LegacyAndroidGameCategoryEnabled()") &&
  stateReaderSource.includes('"androidIsGame"') &&
  stateReaderSource.includes("GameCategoryConfigured");
if (!obsoleteAndroidIsGameCompatibilityAliasConfirmed)
  fail("Android game category validation must confirm the obsolete androidIsGame compatibility alias alongside appCategory.");
const exactCaseOutputAndCaseFoldedReparseGuarded =
  stateReaderSource.includes("ExactOutputPathTextMatches(") &&
  stateReaderSource.includes("CaseFoldedFileSystemNameMatches(") &&
  stateReaderSource.includes("FileSystemInfo exactMatch") &&
  stateReaderSource.includes("foldedMatches.Count > 1") &&
  stateReaderSource.includes("match == null && foldedMatches.Count == 1") &&
  stateReaderSource.includes("StringComparison.OrdinalIgnoreCase");
if (!exactCaseOutputAndCaseFoldedReparseGuarded)
  fail("Android output validation must require exact path text and a unique case-folded reparse walk on every Editor host.");
const legacyEditorFailClosed =
  editorAssemblyAccessSource.includes(
    "#if UNITY_EDITOR && !UNITY_6000_0_OR_NEWER"
  ) &&
  editorAssemblyAccessSource.includes("#error") &&
  editorAssemblyAccessSource.includes("6000.5.9f1");
if (!legacyEditorFailClosed)
  fail("Pre-Unity-6 Editors must hit a compile-time fail-closed sentinel.");
if (!evidenceSource.includes("DEVELOPMENT_APK_PREFLIGHT_PASSED") ||
    /DEBUG_SIGNED_DEVELOPMENT_ONLY/.test(evidenceSource))
  fail("Preflight evidence status must not imply that an APK was built or signed.");
if (!menusSource.includes('ExecuteMenuItem("File/Build Profiles")') ||
    !menusSource.includes('OpenProjectSettings("Project/Player")'))
  fail("Android menus must expose only the reviewed navigation helpers.");
const executeMenuCalls = menusSource.match(/ExecuteMenuItem\s*\(/g)?.length ?? 0;
const openSettingsCalls = menusSource.match(/OpenProjectSettings\s*\(/g)?.length ?? 0;
if (executeMenuCalls !== 1 || openSettingsCalls !== 1)
  fail("Android menus contain an unreviewed menu/settings navigation call.");

const asmdefFiles = projectFiles.filter((file) => file.endsWith(".asmdef"));
if (asmdefFiles.length !== REVIEWED_PROJECT_ASMDEF_FILE_COUNT)
  fail(`Expected exactly ${REVIEWED_PROJECT_ASMDEF_FILE_COUNT} asmdef files, found ${asmdefFiles.length}.`, true);
const asmdefs = new Map();
for (const file of asmdefFiles) {
  const value = parseJson(file);
  if (!value || typeof value.name !== "string") continue;
  if (asmdefs.has(value.name)) fail(`Duplicate assembly name '${value.name}'.`);
  asmdefs.set(value.name, { file, value });
}
function validateNewAsmdef(file, name, references, noEngineReferences) {
  const value = exists(file) ? parseJson(file) : null;
  if (!value) return false;
  const valid =
    value.name === name &&
    sameArray(value.references, references) &&
    sameArray(value.includePlatforms, ["Editor"]) &&
    sameArray(value.excludePlatforms, []) &&
    value.allowUnsafeCode === false &&
    value.overrideReferences === false &&
    sameArray(value.precompiledReferences, []) &&
    value.autoReferenced === true &&
    sameArray(value.defineConstraints, []) &&
    sameArray(value.versionDefines, []) &&
    value.noEngineReferences === noEngineReferences;
  if (!valid) fail(`${file} does not match the reviewed Editor-only assembly shape.`);
  return valid;
}
const coreAsmdefValid = validateNewAsmdef(
  coreAsmdefPath,
  "LegendsOfRus.AndroidBuildCore.Editor",
  [],
  true
);
const editorAsmdefValid = validateNewAsmdef(
  editorAsmdefPath,
  "LegendsOfRus.AndroidBuild.Editor",
  ["LegendsOfRus.AndroidBuildCore.Editor"],
  false
);
const testAsmdefValid = validateNewAsmdef(
  testAsmdefPath,
  "LegendsOfRus.AndroidBuild.Editor.Tests",
  ["LegendsOfRus.AndroidBuildCore.Editor", "LegendsOfRus.AndroidBuild.Editor"],
  false
);
const testAsmdef = exists(testAsmdefPath) ? parseJson(testAsmdefPath) : null;
if (!testAsmdef || !sameArray(testAsmdef.optionalUnityReferences, ["TestAssemblies"]))
  fail(`${testAsmdefPath} must reference TestAssemblies only.`);

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
function countTestAttributes(file, directory = root) {
  if (!exists(file, directory)) return 0;
  return scrubCSharp(read(file, directory)).match(testAttributePattern)?.length ?? 0;
}
const allTestFiles = allCSharpFiles.filter((file) => file.includes("/Tests/"));
const sourceTestAttributes = allTestFiles.reduce(
  (total, file) => total + countTestAttributes(file),
  0
);
const newTestFiles = allTestFiles.filter((file) =>
  file.startsWith("Assets/LegendsOfRus/Tests/AndroidBuildEditMode/")
);
const newTestAttributes = newTestFiles.reduce(
  (total, file) => total + countTestAttributes(file),
  0
);
for (const file of newTestFiles) {
  if (/\[\s*(?:NonParallelizable|Ignore|Explicit)\b/.test(read(file)))
    fail(`${file} contains a forbidden test annotation.`);
}
const selfTestCheckSites = exists(selfTestPath)
  ? Math.max(0,
      (scrubCSharp(read(selfTestPath)).match(/\bCheck\s*\(/g)?.length ?? 0) - 1)
  : 0;
if (newTestAttributes < 1) fail("Android EditMode suite has no source test attributes.");
if (selfTestCheckSites < 1) fail("v1.17 Step 1 Editor self-test has no Check sites.");
if (sourceTestAttributes !== REVIEWED_SOURCE_TEST_ATTRIBUTE_COUNT)
  fail(`Expected exactly ${REVIEWED_SOURCE_TEST_ATTRIBUTE_COUNT} source test attributes, found ${sourceTestAttributes}.`, true);
if (newTestAttributes !== REVIEWED_NEW_EDITMODE_TEST_ATTRIBUTE_COUNT)
  fail(`Expected exactly ${REVIEWED_NEW_EDITMODE_TEST_ATTRIBUTE_COUNT} new EditMode test attributes, found ${newTestAttributes}.`, true);
if (selfTestCheckSites !== REVIEWED_SELF_TEST_CHECK_SITE_COUNT)
  fail(`Expected exactly ${REVIEWED_SELF_TEST_CHECK_SITE_COUNT} Editor self-test Check sites, found ${selfTestCheckSites}.`, true);

const currentPngs = projectFiles.filter((file) => file.toLowerCase().endsWith(".png"));
const currentPngInventorySha256 = inventoryFingerprint(root, currentPngs);
if (currentPngs.length !== 65)
  fail(`Expected exactly 65 approved PNGs, found ${currentPngs.length}.`);
if (currentPngInventorySha256 !== PNG_INVENTORY_SHA256)
  fail("Approved PNG inventory differs from the reviewed v1.16 Step 2 bytes.");

let changedManifestValid = false;
let changedManifestPaths = [];
if (exists(changedManifestPath)) {
  const source = read(changedManifestPath);
  if (!source.endsWith("\n")) fail(`${changedManifestPath} must end with a newline.`);
  const rawLines = source.split(/\r?\n/);
  rawLines.pop();
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
  changedManifestValid =
    validLines && sameArray(changedManifestPaths, allowedChangedPaths);
  if (!changedManifestValid)
    fail(`${changedManifestPath} does not equal the exact reviewed delta allowlist.`);
}

const implementation = exists(implementationStatusPath)
  ? parseJson(implementationStatusPath)
  : null;
if (implementation) {
  if (implementation.status !== IMPLEMENTATION_STATUS ||
      implementation.android_candidate_status !== ANDROID_CANDIDATE_STATUS ||
      implementation.unity_execution_evidence !== "NOT_RETAINED" ||
      implementation.source_project_settings_status !== "ABSENT_FROM_SOURCE_CANDIDATE" ||
      implementation.package_lock_approved !== false ||
      implementation.development_apk_preflight_available !== false ||
      implementation.policy_sha256 !== POLICY_SHA256 ||
      implementation.production_android_build_compiled !== false ||
      implementation.release_apk_allowed !== false ||
      implementation.app_bundle_allowed !== false ||
      implementation.gradle_export_allowed !== false ||
      implementation.custom_signing_allowed !== false ||
      implementation.native_debug_symbols_allowed !== false ||
      implementation.android_library_projects_allowed !== false ||
      implementation.embedded_packages_allowed !== false ||
      implementation.managed_plugins_allowed !== false ||
      implementation.per_build_scripting_defines_guarded !== true ||
      implementation.scheduled_build_contract_guarded !== true ||
      implementation.custom_build_profiles_allowed !== false ||
      implementation.production_build_authorized !== false ||
      implementation.economy_activation_authorized !== false ||
      implementation.commercial_activation_authorized !== false)
    fail(`${implementationStatusPath} overstates Android or activation authority.`);
}

const evidence = exists(activationEvidencePath)
  ? parseJson(activationEvidencePath)
  : null;
if (evidence) {
  const retentionValid = isObject(evidence.evidenceRetention) &&
    Object.values(evidence.evidenceRetention).every((value) => value === "NOT_RETAINED");
  const approvalsValid = isObject(evidence.approvalDecisions) &&
    Object.values(evidence.approvalDecisions).every((value) => value === "NOT_RECORDED");
  if (evidence.status !== "SOURCE_CANDIDATE_ONLY_PRODUCTION_BLOCKED" ||
      evidence.implementationStatus !== IMPLEMENTATION_STATUS ||
      evidence.androidCandidateStatus !== ANDROID_CANDIDATE_STATUS ||
      evidence.policySha256 !== POLICY_SHA256 ||
      evidence.sourceProjectSettingsStatus !== "ABSENT_FROM_SOURCE_CANDIDATE" ||
      !retentionValid || !approvalsValid ||
      evidence.productionAndroidBuildCompiled !== false ||
      evidence.productionBuildAuthorized !== false ||
      evidence.publishingAuthorized !== false ||
      evidence.economyActivationAuthorized !== false ||
      evidence.commercialActivationAuthorized !== false)
    fail(`${activationEvidencePath} contains an unsupported evidence or approval claim.`);
}

const staticReport = exists(staticReportPath) ? parseJson(staticReportPath) : null;
let staticReportValid = false;
if (staticReport) {
  staticReportValid =
    staticReport.version === VERSION &&
    staticReport.status === "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION" &&
    staticReport.passed === true &&
    staticReport.implementation_status === IMPLEMENTATION_STATUS &&
    staticReport.android_candidate_status === ANDROID_CANDIDATE_STATUS &&
    staticReport.unity_execution_evidence === "NOT_RETAINED" &&
    staticReport.source_project_settings_status === "ABSENT_FROM_SOURCE_CANDIDATE" &&
    staticReport.package_lock_approval_blocked === true &&
    staticReport.package_lock_approved === false &&
    staticReport.preflight_pass_possible === false &&
    staticReport.policy_sha256 === POLICY_SHA256 &&
    staticReport.production_android_build_compiled === false &&
    staticReport.production_build_authorized === false &&
    staticReport.project_files === projectFiles.length &&
    staticReport.project_json_files === allJsonFiles.length &&
    staticReport.project_asmdef_files === asmdefFiles.length &&
    staticReport.csharp_structural_scan_files === allCSharpFiles.length &&
    staticReport.source_test_attributes_informational === sourceTestAttributes &&
    staticReport.new_editmode_source_test_attributes_informational === newTestAttributes &&
    staticReport.editor_self_test_check_sites_informational === selfTestCheckSites &&
    staticReport.build_player_options_guard_present === true &&
    staticReport.per_build_scripting_defines_guarded === true &&
    staticReport.scheduled_build_contract_guarded === true &&
    staticReport.custom_build_profile_assets_absent === true &&
    staticReport.native_debug_symbols_guarded === true &&
    staticReport.android_library_projects_blocked === true &&
    staticReport.embedded_packages_blocked === true &&
    staticReport.managed_plugin_extensions_blocked === true &&
    staticReport.obsolete_android_is_game_compatibility_alias_confirmed === true &&
    staticReport.exact_case_output_and_casefolded_reparse_guarded === true &&
    staticReport.legacy_editor_fail_closed === true &&
    staticReport.approved_png_files === currentPngs.length &&
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
let inheritedTestsIdentical = null;
let manifestMatchesBaseline = null;

if (baseline) {
  if (!fs.existsSync(baseline) || !fs.statSync(baseline).isDirectory()) {
    fail(`Baseline '${baseline}' is not a directory.`);
  } else {
    baselineFiles = walkProject(baseline, "baseline");
    baselineTree = treeFingerprint(baseline, baselineFiles);
    baselineTreeValid =
      baselineFiles.length === BASELINE_FILE_COUNT &&
      baselineTree === BASELINE_TREE_SHA256 &&
      exists("MILESTONE_VERSION.txt", baseline) &&
      read("MILESTONE_VERSION.txt", baseline).trim() ===
        "v1.16-step02-provider-integration-runtime-wiring-activation-review";
    if (!baselineTreeValid)
      fail("Supplied v1.16 Step 2 baseline does not match the pinned 429-file tree.");

    const baselineVerifier = "Tools/verify_v1_16_step02.mjs";
    if (!exists(baselineVerifier, baseline) ||
        sha256File(baselineVerifier, baseline) !== BASELINE_VERIFIER_SHA256) {
      fail("Supplied baseline verifier does not match the pinned v1.16 Step 2 verifier.");
      baselineVerifierPassed = false;
    } else {
      const execution = spawnSync(
        process.execPath,
        [absolute(baselineVerifier, baseline), "--source-only"],
        { cwd: baseline, encoding: "utf8", timeout: 30000 }
      );
      try {
        const result = JSON.parse(execution.stdout);
        baselineVerifierPassed =
          execution.status === 0 &&
          result.passed === true &&
          result.status === "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION" &&
          sameArray(result.failures, []) &&
          sameArray(result.static_failures, []);
      } catch {
        baselineVerifierPassed = false;
      }
      if (!baselineVerifierPassed)
        fail("Pinned v1.16 Step 2 baseline source verifier did not pass cleanly.");
    }

    const baselineSet = new Set(baselineFiles);
    const candidateSet = new Set(projectFiles);
    const union = [...new Set([...baselineFiles, ...projectFiles])].sort();
    actualChangedPaths = [];
    protectedUnionIdentical = true;
    approvedPngsIdentical = true;
    inheritedTestsIdentical = true;
    for (const file of union) {
      const inBaseline = baselineSet.has(file);
      const inCandidate = candidateSet.has(file);
      if (inBaseline && !inCandidate) {
        fail(`Candidate deleted inherited path '${file}'.`);
        protectedUnionIdentical = false;
        continue;
      }
      const differs = !inBaseline ||
        sha256File(file, baseline) !== sha256File(file, root);
      if (differs) actualChangedPaths.push(file);
      if (differs && !allowedChangedPaths.includes(file)) {
        fail(`Candidate changed non-allowlisted inherited path '${file}'.`);
        protectedUnionIdentical = false;
      }
      if (file.toLowerCase().endsWith(".png") && differs)
        approvedPngsIdentical = false;
      if (inBaseline && file.includes("/Tests/") && differs)
        inheritedTestsIdentical = false;
    }
    if (!approvedPngsIdentical)
      fail("An inherited PNG differs from the v1.16 Step 2 baseline.");
    if (!inheritedTestsIdentical)
      fail("An inherited test source or assembly differs from the v1.16 Step 2 baseline.");
    manifestMatchesBaseline = sameArray(actualChangedPaths, changedManifestPaths);
    if (!manifestMatchesBaseline)
      fail("Changed-file manifest does not equal the computed v1.16 Step 2 delta.");
  }
}

const candidateTreeSha256 = treeFingerprint(root, projectFiles);
const verifierSha256 = exists(VERIFIER_PATH)
  ? sha256File(VERIFIER_PATH)
  : null;
const passed = failures.length === 0;
const releaseBoundaryVerified = Boolean(
  passed && baseline && baselineTreeValid && baselineVerifierPassed &&
  protectedUnionIdentical && approvedPngsIdentical && inheritedTestsIdentical &&
  manifestMatchesBaseline
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
  android_candidate_status: ANDROID_CANDIDATE_STATUS,
  unity_execution_evidence: "NOT_RETAINED",
  unity_compile_status: "NOT_RETAINED",
  editor_self_test_execution_status: "NOT_RETAINED",
  editmode_execution_status: "NOT_RETAINED",
  playmode_execution_status: "NOT_RETAINED",
  development_apk_execution_status: "NOT_RETAINED",
  representative_device_status: "NOT_RETAINED",
  source_project_settings_status: "ABSENT_FROM_SOURCE_CANDIDATE",
  package_lock_approval_blocked: packageLockApprovalBlocked,
  package_lock_approved: packageLockApprovalBlocked ? false : null,
  preflight_pass_possible: packageLockApprovalBlocked ? false : null,
  production_android_build_compiled: false,
  production_build_authorized: false,
  publishing_authorized: false,
  economy_activation_authorized: false,
  commercial_activation_authorized: false,
  exact_unity_version: true,
  source_only: sourceOnly,
  release_boundary_verified: releaseBoundaryVerified,
  project_files: projectFiles.length,
  project_json_files: allJsonFiles.length,
  project_asmdef_files: asmdefFiles.length,
  csharp_structural_scan_files: allCSharpFiles.length,
  candidate_tree_sha256: candidateTreeSha256,
  verifier_sha256: verifierSha256,
  policy_sha256: exists(policyPath) ? sha256File(policyPath) : null,
  package_manifest_sha256: exists("Packages/manifest.json")
    ? sha256File("Packages/manifest.json") : null,
  project_version_sha256: exists("ProjectSettings/ProjectVersion.txt")
    ? sha256File("ProjectSettings/ProjectVersion.txt") : null,
  editor_build_settings_sha256: exists("ProjectSettings/EditorBuildSettings.asset")
    ? sha256File("ProjectSettings/EditorBuildSettings.asset") : null,
  policy_exact_and_development_only:
    policy !== null && JSON.stringify(policy) === JSON.stringify(expectedPolicy),
  project_settings_asset_absent:
    !exists("ProjectSettings/ProjectSettings.asset"),
  global_android_build_guard_present: preprocessorImplementations === 1,
  build_player_options_guard_present:
    buildPlayerProcessorImplementations === 1,
  per_build_scripting_defines_guarded:
    buildPlayerProcessorImplementations === 1 &&
    guardSource.includes("extraScriptingDefines"),
  scheduled_build_contract_guarded:
    buildPlayerProcessorImplementations === 1 &&
    guardSource.includes("options.options != BuildOptions.Development") &&
    guardSource.includes("BuildOptions.ShowBuiltPlayer") &&
    guardSource.includes("options.options != uiDevelopment") &&
    guardSource.includes("options.scenes.Length != 1") &&
    guardSource.includes("options.assetBundleManifestPath") &&
    guardSource.includes("ScheduledOutputIsExact("),
  custom_build_profile_assets_absent: assetsScriptableObjects.length === 0,
  native_debug_symbols_guarded: nativeDebugSymbolsGuarded,
  android_library_projects_blocked: androidLibraryProjectsBlocked,
  embedded_packages_blocked: embeddedPackagesBlocked,
  managed_plugin_extensions_blocked: managedPluginExtensionsBlocked,
  obsolete_android_is_game_compatibility_alias_confirmed:
    obsoleteAndroidIsGameCompatibilityAliasConfirmed,
  exact_case_output_and_casefolded_reparse_guarded:
    exactCaseOutputAndCaseFoldedReparseGuarded,
  legacy_editor_fail_closed: legacyEditorFailClosed,
  production_sensitive_android_paths_blocked:
    passed && reviewedCandidateBytesExact,
  credential_bearing_unity_fields_absent:
    passed && reviewedCandidateBytesExact,
  player_settings_mutation_absent:
    passed && reviewedCandidateBytesExact,
  build_pipeline_invocation_absent:
    passed && reviewedCandidateBytesExact,
  runtime_payload_added: !reviewedCandidateBytesExact,
  core_asmdef_valid: coreAsmdefValid,
  editor_asmdef_valid: editorAsmdefValid,
  test_asmdef_valid: testAsmdefValid,
  assembly_graph_acyclic: assemblyGraphAcyclic,
  source_test_attributes_informational: sourceTestAttributes,
  new_editmode_source_test_attributes_informational: newTestAttributes,
  editor_self_test_check_sites_informational: selfTestCheckSites,
  source_counts_are_execution_evidence: false,
  approved_png_files: currentPngs.length,
  approved_png_inventory_sha256: currentPngInventorySha256,
  static_report_present: exists(staticReportPath),
  static_report_valid: staticReportValid,
  activation_evidence_present: exists(activationEvidencePath),
  changed_file_manifest_present: exists(changedManifestPath),
  changed_file_manifest_valid: changedManifestValid,
  baseline_supplied: Boolean(baseline),
  baseline_file_count: baselineFiles?.length ?? null,
  baseline_tree_sha256: baselineTree,
  baseline_tree_valid: baselineTreeValid,
  baseline_source_verifier_passed: baselineVerifierPassed,
  protected_union_identical: protectedUnionIdentical,
  approved_pngs_identical_to_v116_step02: approvedPngsIdentical,
  inherited_tests_identical_to_v116_step02: inheritedTestsIdentical,
  actual_changed_paths: actualChangedPaths,
  changed_file_manifest_matches_v116_step02: manifestMatchesBaseline,
  failures,
  static_failures: staticFailures
};

console.log(JSON.stringify(report, null, 2));
if (!passed) process.exitCode = 1;
