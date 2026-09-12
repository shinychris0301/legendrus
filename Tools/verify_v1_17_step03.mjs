import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { spawnSync } from "node:child_process";

const VERSION = "v1.17-step03";
const MILESTONE_ID = "v1.17-step03-crash-error-reporting-foundation";
const IMPLEMENTATION_STATUS =
  "IMPLEMENTED_IN_SOURCE_PENDING_V1_17_STEP02_PREREQUISITE_AND_STEP03_UNITY_VERIFICATION";
const CANDIDATE_STATUS =
  "CRASH_ERROR_REPORTING_FOUNDATION_DETACHED_PRODUCTION_TRANSPORT_BLOCKED";
const UNITY_VERSION = "6000.5.9f1";

const BASELINE_FILE_COUNT = 487;
const BASELINE_TREE_SHA256 =
  "a32d5f2a0175cf11db1c5317c7ceb5253fa2e6faa3348cd7f5a0e89d0ca31f30";
const BASELINE_VERIFIER_SHA256 =
  "a6fcd17a70cf23d1a5afce5100ec76cfb6fe05a3f66230c8fa06c930f9152091";
const BASELINE_MILESTONE_ID =
  "v1.17-step02-save-migration-recovery-foundation";
const BASELINE_VERIFIER_PATH = "Tools/verify_v1_17_step02.mjs";

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

// FINALIZATION PINS. These deliberately fail until every non-verifier byte is
// frozen, reviewed, and these two values are replaced with exact facts.
const REVIEWED_PROJECT_FILE_COUNT = 511;
const REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256 =
  "843adf73719e57def4c4761ea85dd61727a357499eb7b4662f0eeb5cc4eefb84";

const VERIFIER_PATH = "Tools/verify_v1_17_step03.mjs";
const changedManifestPath =
  "Assets/LegendsOfRus/Documentation/V1_17_STEP03_CHANGED_FILES.txt";
const staticReportPath =
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_17_STEP03.json";
const implementationStatusPath =
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_17_STEP03.json";
const activationEvidencePath =
  "Assets/LegendsOfRus/Documentation/V1_17_STEP03_ACTIVATION_EVIDENCE.json";
const checkpointPath =
  "Assets/LegendsOfRus/Documentation/NEXT_CHECKPOINT_V1_17_STEP03_UNITY_VERIFICATION.json";
const nextMilestonePath =
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_17_STEP04.json";
const foundationDocumentPath =
  "Assets/LegendsOfRus/Documentation/V1_17_STEP03_CRASH_ERROR_REPORTING_FOUNDATION.md";
const selfTestPath =
  "Assets/LegendsOfRus/Scripts/Editor/Step17CrashErrorReportingFoundationSelfTest.cs";
const diagnosticsPrefix = "Assets/LegendsOfRus/Scripts/DiagnosticsCore";
const diagnosticsAsmdefPath = diagnosticsPrefix +
  "/LegendsOfRus.DiagnosticsCore.asmdef";
const diagnosticsAssemblyAccessPath =
  diagnosticsPrefix + "/DiagnosticsCoreAssemblyAccess.cs";
const modelsPath = diagnosticsPrefix + "/ErrorReportModels.cs";
const policyPath = diagnosticsPrefix + "/ErrorReportPolicy.cs";
const reasonCodesPath = diagnosticsPrefix + "/ErrorReportReasonCodes.cs";
const validatorPath = diagnosticsPrefix + "/ErrorReportValidator.cs";
const determinismPath = diagnosticsPrefix + "/ErrorReportDeterminism.cs";
const bufferPath = diagnosticsPrefix + "/BoundedErrorReportBuffer.cs";
const transportPath = diagnosticsPrefix + "/ErrorReportTransport.cs";
const pipelinePath = diagnosticsPrefix + "/ErrorReportingPipeline.cs";
const editModeTestsAsmdefPath =
  "Assets/LegendsOfRus/Tests/DiagnosticsEditMode/LegendsOfRus.DiagnosticsCore.Tests.asmdef";
const editModeTestsPath =
  "Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs";
const playModeTestsAsmdefPath =
  "Assets/LegendsOfRus/Tests/DiagnosticsPlayMode/LegendsOfRus.DiagnosticsCore.PlayModeTests.asmdef";
const playModeTestsPath =
  "Assets/LegendsOfRus/Tests/DiagnosticsPlayMode/ErrorReportFoundationPlayModeTests.cs";
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

const allowedChangedPaths = [
  "Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
  implementationStatusPath,
  checkpointPath,
  nextMilestonePath,
  "Assets/LegendsOfRus/Documentation/README_FIRST.md",
  staticReportPath,
  activationEvidencePath,
  changedManifestPath,
  foundationDocumentPath,
  selfTestPath,
  diagnosticsAssemblyAccessPath,
  bufferPath,
  determinismPath,
  modelsPath,
  policyPath,
  reasonCodesPath,
  transportPath,
  validatorPath,
  pipelinePath,
  diagnosticsAsmdefPath,
  editModeTestsPath,
  editModeTestsAsmdefPath,
  playModeTestsPath,
  playModeTestsAsmdefPath,
  "MILESTONE_VERSION.txt",
  "README.md",
  "README_V1_17_STEP03.md",
  VERIFIER_PATH
].sort();

const protectedPrefixPins = [
  ["Assets/LegendsOfRus/Resources", 56,
    "afc6f3bfad1acd64c02537fcf0e0d0ef0db049b5e7b953247c75e6bf6cce8669"],
  ["Assets/LegendsOfRus/Resources/Config", 8,
    "4ab534bd45fd178b233d8d4a8367313a62ee1830033bf66128b5f5180c1f3f95"],
  ["Assets/Scenes", 2,
    "0940801e6d1174dab075d0f3671ef72a7c5d26624a368209fc7f0d1a5348172a"],
  ["Packages", 1,
    "8ea8fe4b3512dab71a526fb3d76cea7e07709725d36a6ff270660c2b181b2ce6"],
  ["ProjectSettings", 2,
    "9d1402e81b07019f5f41afb56d4765e6851ccfda08221943d16d78edd61e5a4f"],
  ["Assets/LegendsOfRus/Editor/AndroidBuild", 13,
    "53c750bc6a9db555238b58e88b82d138c86bc170f449460151bc652303071e53"],
  ["Assets/LegendsOfRus/Scripts/EconomyCore", 9,
    "cd1303ed19a5a80c457035a387f77ea80f733f99a89f783396208ea5e877853f"],
  ["Assets/LegendsOfRus/Scripts/RuntimeEconomy", 5,
    "956f5595c78fbbb29977e734c7a58156e0a59271732bd39b1ca6edb38c8a6a2c"],
  ["Assets/LegendsOfRus/Scripts/CommercialCore", 27,
    "40731664c8acb17b0524dd4a96c54087082d0cfaf99c81562975970bde88eff4"],
  ["Assets/LegendsOfRus/Scripts/CommercialRuntime", 14,
    "cf0b7a9225163f041c2636ab5f1db3bcab2a76f44b374b0e7ffff990e36f56f3"],
  ["Assets/LegendsOfRus/Scripts/SaveCore", 7,
    "ee5d82dabe658c0457add21690393fe14e7164f62a1ec7d7c520b0a737d23ad2"],
  ["Assets/LegendsOfRus/Scripts/Runtime", 52,
    "e2caefb1eb30a4f5a7d1a3346a3ce0835e0185f7cb4ffa620dfe1cc59e6e49fd"]
].map(function (item) {
  return { prefix: item[0], files: item[1], sha256: item[2] };
});

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
    fail(name + " requires a directory argument.");
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
  if (!knownArguments.has(argument)) fail("Unknown argument '" + argument + "'.");
}
if (sourceOnly && baseline)
  fail("Choose either --source-only or --baseline, not both.");
if (!sourceOnly && !baseline)
  fail("Release-boundary verification requires --baseline <v1.17-step02-project>.");

const generatedRootNames = new Set([
  ".git", ".gradle", "Library", "Temp", "Logs", "obj", "Build", "Builds",
  "UserSettings"
]);

function walkProject(directory, label, recordFailure = true) {
  const output = [];
  const caseFolded = new Map();
  function report(message) {
    if (recordFailure) fail(label + ": " + message);
  }
  function visit(relative) {
    const full = path.join(directory, relative);
    let entries;
    try {
      entries = fs.readdirSync(full, { withFileTypes: true })
        .sort(function (left, right) {
          return left.name.localeCompare(right.name);
        });
    } catch {
      report("cannot enumerate '" + (relative || ".") + "'.");
      return;
    }
    for (const entry of entries) {
      const child = relative ? path.posix.join(relative, entry.name) : entry.name;
      if (!relative && generatedRootNames.has(entry.name)) {
        report("generated or repository directory '" + entry.name + "' is not allowed.");
        continue;
      }
      const childFull = path.join(directory, child);
      let stat;
      try {
        stat = fs.lstatSync(childFull);
      } catch {
        report("cannot inspect '" + child + "'.");
        continue;
      }
      if (stat.isSymbolicLink()) {
        report("symbolic link '" + child + "' is not allowed.");
        continue;
      }
      if (stat.isDirectory()) {
        const lower = child.toLocaleLowerCase("en-US");
        if (lower === "assets/plugins" || lower.startsWith("assets/plugins/"))
          report("plugin directory '" + child + "' is not allowed.");
        if (child.startsWith("Assets/") &&
            entry.name.toLowerCase().endsWith(".androidlib"))
          report("Android library project directory '" + child + "' is not allowed.");
        if (relative === "Packages")
          report("embedded package directory '" + child + "' is not allowed.");
        visit(child);
        continue;
      }
      if (!stat.isFile()) {
        report("special filesystem entry '" + child + "' is not allowed.");
        continue;
      }
      const folded = child.toLocaleLowerCase("en-US");
      if (caseFolded.has(folded) && caseFolded.get(folded) !== child)
        report("case-fold collision '" + caseFolded.get(folded) + "' and '" + child + "'.");
      else
        caseFolded.set(folded, child);
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
    left.every(function (item, index) { return item === right[index]; });
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
    while (/\s/.test(source[index] || "")) index++;
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
    const character = source[index];
    if (character === "{") return object(location);
    if (character === "[") return array(location);
    if (character === '"') {
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
      if (seen.has(key)) duplicates.push(location + "." + key);
      seen.add(key);
      whitespace();
      if (source[index] !== ":") throw new Error("expected colon");
      index++;
      value(location + "." + key);
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
      value(location + "[" + item++ + "]");
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
    fail(relative + " is missing.");
    return null;
  }
  const source = read(relative, directory);
  try {
    if (duplicateJsonKeys(source).length > 0) {
      fail(relative + " contains duplicate JSON keys.");
      return null;
    }
    return JSON.parse(source);
  } catch {
    fail(relative + " is not strict valid JSON.");
    return null;
  }
}

function scrubCSharp(source) {
  let output = "";
  let index = 0;
  let state = "code";
  while (index < source.length) {
    const character = source[index];
    const next = source[index + 1] || "";
    if (state === "line") {
      if (character === "\n") { output += "\n"; state = "code"; }
      else output += " ";
      index++;
      continue;
    }
    if (state === "block") {
      if (character === "*" && next === "/") {
        output += "  "; index += 2; state = "code";
      } else { output += character === "\n" ? "\n" : " "; index++; }
      continue;
    }
    if (state === "string" || state === "char") {
      const quote = state === "string" ? '"' : "'";
      if (character === "\\") { output += "  "; index += 2; }
      else if (character === quote) { output += " "; index++; state = "code"; }
      else { output += character === "\n" ? "\n" : " "; index++; }
      continue;
    }
    if (state === "verbatim") {
      if (character === '"' && next === '"') { output += "  "; index += 2; }
      else if (character === '"') { output += " "; index++; state = "code"; }
      else { output += character === "\n" ? "\n" : " "; index++; }
      continue;
    }
    if (character === "/" && next === "/") {
      output += "  "; index += 2; state = "line";
    } else if (character === "/" && next === "*") {
      output += "  "; index += 2; state = "block";
    } else if (character === "@" && next === '"') {
      output += "  "; index += 2; state = "verbatim";
    } else if (character === '"') {
      output += " "; index++; state = "string";
    } else if (character === "'") {
      output += " "; index++; state = "char";
    } else { output += character; index++; }
  }
  return output;
}

// Remove comments while retaining string contents. This is used for positive
// source markers so comments cannot satisfy a production-boundary check.
function stripCSharpComments(source) {
  let output = "";
  let index = 0;
  let state = "code";
  while (index < source.length) {
    const character = source[index];
    const next = source[index + 1] || "";
    if (state === "line") {
      if (character === "\n") {
        output += "\n";
        state = "code";
      } else output += " ";
      index++;
      continue;
    }
    if (state === "block") {
      if (character === "*" && next === "/") {
        output += "  ";
        index += 2;
        state = "code";
      } else {
        output += character === "\n" ? "\n" : " ";
        index++;
      }
      continue;
    }
    if (state === "string" || state === "char") {
      const quote = state === "string" ? '"' : "'";
      output += character;
      if (character === "\\" && index + 1 < source.length) {
        output += next;
        index += 2;
      } else {
        index++;
        if (character === quote) state = "code";
      }
      continue;
    }
    if (state === "verbatim") {
      output += character;
      if (character === '"' && next === '"') {
        output += next;
        index += 2;
      } else {
        index++;
        if (character === '"') state = "code";
      }
      continue;
    }
    if (character === "/" && next === "/") {
      output += "  ";
      index += 2;
      state = "line";
    } else if (character === "/" && next === "*") {
      output += "  ";
      index += 2;
      state = "block";
    } else if (character === "@" && next === '"') {
      output += "@\"";
      index += 2;
      state = "verbatim";
    } else if (character === '"') {
      output += character;
      index++;
      state = "string";
    } else if (character === "'") {
      output += character;
      index++;
      state = "char";
    } else {
      output += character;
      index++;
    }
  }
  return output;
}

function extractCSharpBlock(source, declarationPattern) {
  const scrubbed = scrubCSharp(source);
  const match = declarationPattern.exec(scrubbed);
  if (!match) return null;
  const opening = scrubbed.indexOf("{", match.index + match[0].length);
  if (opening < 0) return null;
  let depth = 0;
  for (let index = opening; index < scrubbed.length; index++) {
    if (scrubbed[index] === "{") depth++;
    else if (scrubbed[index] === "}") {
      depth--;
      if (depth === 0) {
        return {
          source: source.slice(opening, index + 1),
          scrubbed: scrubbed.slice(opening, index + 1)
        };
      }
    }
  }
  return null;
}

function exactEnumMembers(source, enumName, expected) {
  const escaped = enumName.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
  const block = extractCSharpBlock(
    source,
    new RegExp("\\benum\\s+" + escaped + "\\s*")
  );
  if (!block) return false;
  const body = block.scrubbed.slice(1, -1).trim();
  if (!body) return expected.length === 0;
  const members = body.split(",").map(function (part) {
    const normalized = part.replace(/\s+/g, " ").trim();
    const match = normalized.match(/^([A-Za-z_]\w*)\s*=\s*(-?\d+)$/);
    return match ? match[1] + "=" + match[2] : null;
  });
  return !members.includes(null) && sameArray(members, expected);
}

function classBlock(source, className) {
  const escaped = className.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
  return extractCSharpBlock(
    source,
    new RegExp("\\bclass\\s+" + escaped + "\\b[^\\{]*")
  );
}

function publicPropertyInventory(block) {
  if (!block) return [];
  const properties = [];
  const pattern = /\bpublic\s+([A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*(?:<[^{};()]+>)?(?:\[\])?\??)\s+([A-Za-z_]\w*)\s*\{/g;
  let match;
  while ((match = pattern.exec(block.scrubbed)) !== null)
    properties.push(match[1].replace(/\s+/g, "") + " " + match[2]);
  return properties.sort();
}

function publicMethodInventory(block) {
  if (!block) return [];
  const methods = [];
  const pattern = /^\s*public\s+(static\s+)?([A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*(?:<[^{};()]+>)?(?:\[\])?\??)\s+([A-Za-z_]\w*)\s*\(/gm;
  let match;
  while ((match = pattern.exec(block.scrubbed)) !== null) {
    methods.push(
      (match[1] ? "static " : "") +
      match[2].replace(/\s+/g, "") + " " + match[3]
    );
  }
  return methods.sort();
}

function publicFieldInventory(block) {
  if (!block) return [];
  const fields = [];
  const pattern = /^\s*public\s+(?:(?:static|readonly|const)\s+)*([A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*(?:<[^{};()]+>)?(?:\[\])?\??)\s+([A-Za-z_]\w*)\s*(?:=[^;]*)?;/gm;
  let match;
  while ((match = pattern.exec(block.scrubbed)) !== null)
    fields.push(match[1].replace(/\s+/g, "") + " " + match[2]);
  return fields.sort();
}

function constructorInventory(block, className) {
  if (!block) return [];
  const escaped = className.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
  const pattern = new RegExp(
    "^\\s*(?:(public|internal|protected|private)\\s+)?" + escaped +
      "\\s*\\(([^)]*)\\)",
    "gm"
  );
  const constructors = [];
  let match;
  while ((match = pattern.exec(block.scrubbed)) !== null) {
    const access = match[1] || "private";
    const parameters = match[2].trim()
      ? match[2].split(",").map(function (part) {
          return part.trim().replace(/\s+/g, " ").replace(/\s*=\s*/g, "=");
        }).join(",")
      : "";
    constructors.push(access + "(" + parameters + ")");
  }
  return constructors.sort();
}

function caseLabelInventory(block, enumName) {
  if (!block) return [];
  const escaped = enumName.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
  const pattern = new RegExp("\\bcase\\s+" + escaped + "\\.([A-Za-z_]\\w*)\\s*:", "g");
  const labels = [];
  let match;
  while ((match = pattern.exec(block.scrubbed)) !== null) labels.push(match[1]);
  return labels;
}

function exactCaseReturnMappings(block, enumName, expected) {
  if (!block) return false;
  const code = stripCSharpComments(block.source);
  const labels = caseLabelInventory(block, enumName);
  if (!sameArray(labels, expected.map(function (entry) { return entry[0]; })))
    return false;
  for (const entry of expected) {
    const member = entry[0].replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    const wire = entry[1].replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    const pattern = new RegExp(
      "\\bcase\\s+" + enumName + "\\." + member +
        "\\s*:\\s*return\\s+\"" + wire + "\"\\s*;"
    );
    if (!pattern.test(code)) return false;
  }
  return true;
}

function structuralCSharpCheck(relative) {
  const source = read(relative);
  const scrubbed = scrubCSharp(source);
  const pairs = new Map([["}", "{"], [")", "("], ["]", "["]]);
  const openings = new Set(["{", "(", "["]);
  const stack = [];
  for (const character of scrubbed) {
    if (openings.has(character)) stack.push(character);
    else if (pairs.has(character) && stack.pop() !== pairs.get(character)) {
      fail(relative + " has unbalanced C# delimiters.", true);
      return;
    }
  }
  if (stack.length > 0) fail(relative + " has unbalanced C# delimiters.", true);
  let depth = 0;
  for (const line of source.split(/\r?\n/)) {
    if (/^\s*#if\b/.test(line)) depth++;
    if (/^\s*#endif\b/.test(line)) depth--;
    if (depth < 0) {
      fail(relative + " has unbalanced preprocessor directives.", true);
      return;
    }
  }
  if (depth !== 0) fail(relative + " has unbalanced preprocessor directives.", true);
}

function filesUnder(files, prefix) {
  return files.filter(function (file) {
    return file === prefix || file.startsWith(prefix + "/");
  });
}

function isProtectedPath(file) {
  return file.toLowerCase().endsWith(".png") ||
    protectedPrefixPins.some(function (pin) {
      return file === pin.prefix || file.startsWith(pin.prefix + "/");
    });
}

const projectFiles = walkProject(root, "candidate");
const projectFileSet = new Set(projectFiles);
for (const relative of allowedChangedPaths) {
  if (!projectFileSet.has(relative)) fail("Required artifact '" + relative + "' is missing.");
}

const finalPinsReady =
  Number.isInteger(REVIEWED_PROJECT_FILE_COUNT) &&
  REVIEWED_PROJECT_FILE_COUNT > BASELINE_FILE_COUNT &&
  /^[0-9a-f]{64}$/.test(REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256);
if (!finalPinsReady) {
  fail(
    "Final reviewed candidate file-count/hash pins are unset; freeze all other Step 3 bytes and repin the FINALIZATION PINS.",
    true
  );
}
const reviewedCandidateFiles = projectFiles.filter(function (file) {
  return file !== VERIFIER_PATH;
});
const candidateExcludingVerifierSha256 = treeFingerprint(
  root,
  reviewedCandidateFiles
);
const reviewedCandidateBytesExact = finalPinsReady &&
  projectFiles.length === REVIEWED_PROJECT_FILE_COUNT &&
  reviewedCandidateFiles.length === REVIEWED_PROJECT_FILE_COUNT - 1 &&
  candidateExcludingVerifierSha256 ===
    REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256;
if (finalPinsReady && !reviewedCandidateBytesExact)
  fail("Candidate files outside the verifier differ from the exact reviewed source boundary.", true);

const milestone = exists("MILESTONE_VERSION.txt")
  ? read("MILESTONE_VERSION.txt").trim() : null;
if (milestone !== MILESTONE_ID)
  fail("MILESTONE_VERSION.txt must equal '" + MILESTONE_ID + "'.");
if (!exists("ProjectSettings/ProjectVersion.txt") ||
    read("ProjectSettings/ProjectVersion.txt").trim() !==
      "m_EditorVersion: " + UNITY_VERSION)
  fail("ProjectVersion.txt must pin exactly Unity " + UNITY_VERSION + ".");
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
for (const entry of directProtectedHashes) {
  const relative = entry[0];
  const expected = entry[1];
  if (!exists(relative)) fail("Protected file '" + relative + "' is missing.");
  else if (sha256File(relative) !== expected)
    fail("Protected file '" + relative + "' changed from the reviewed bytes.");
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
    fail("Protected inventory '" + pin.prefix + "' differs from the reviewed Step 2 bytes.");
}

const allJsonFiles = projectFiles.filter(function (file) {
  return file.endsWith(".json");
});
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
  fail("Runtime economy activation evidence must remain incomplete and activation false.");

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
  commercialActivation && commercialActivation.environment === "disabled" &&
  commercialFalseFields.every(function (field) {
    return commercialActivation[field] === false;
  })
);
if (!commercialActivationDisabled)
  fail("Commercial activation approvals and provider environment must remain disabled.");

const commercialConfig = parseJson(commercialConfigPath);
const commercialConfigDisabled = Boolean(
  commercialConfig && commercialConfig.environment === "disabled" &&
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
  )?.length || 0;
const packageLockApprovalBlocked =
  resolvedPackageLockConstants === 1 &&
  androidStateReaderSource.includes(
    "facts.PackageLockApproved = ResolvedPackageLockApproved;"
  ) &&
  /if\s*\(\s*!facts\.PackageLockApproved\s*\)/.test(androidEvaluatorSource) &&
  androidEvaluatorSource.includes("AndroidBuildReasonCodes.PackageLockUnapproved");
if (!packageLockApprovalBlocked)
  fail("ResolvedPackageLockApproved must remain a compiled false fail-closed Android gate.");
const productionAndroidBuildCompiledFalse =
  /private\s+const\s+bool\s+ProductionAndroidBuildCompiled\s*=\s*false\s*;/.test(
    androidGuardSource
  );
if (!productionAndroidBuildCompiledFalse)
  fail("ProductionAndroidBuildCompiled must remain false.");
const externalProviderActivationCompiledFalse =
  /private\s+const\s+bool\s+ExternalProviderActivationCompiled\s*=\s*false\s*;/.test(
    commercialActivationSource
  );
if (!externalProviderActivationCompiledFalse)
  fail("ExternalProviderActivationCompiled must remain false.");

const prohibitedExtensions = new Set([
  ".aar", ".aab", ".apk", ".apks", ".dll", ".jar", ".java", ".jks",
  ".key", ".keystore", ".kt", ".p12", ".pem", ".pfx", ".rsp", ".so",
  ".unitypackage", ".xapk"
]);
for (const file of projectFiles) {
  const lower = file.toLowerCase();
  if (prohibitedExtensions.has(path.extname(lower)))
    fail("Prohibited provider, signing, plugin, or build artifact '" + file + "' is present.");
  if (lower === ".env" || lower.startsWith(".env.") || lower.includes("/.env"))
    fail("Environment secret file '" + file + "' is not allowed.");
  if (lower.endsWith(".asmref"))
    fail("Assembly reference '" + file + "' is not allowed in this milestone.");
}

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
const textExtensions = new Set([".cs", ".json", ".md", ".mjs", ".txt", ".asmdef"]);
for (const file of allowedChangedPaths) {
  if (!exists(file) || !textExtensions.has(path.extname(file).toLowerCase()))
    continue;
  const source = read(file);
  if (highConfidenceSecretPatterns.some(function (pattern) {
    return pattern.test(source);
  }))
    fail(file + " contains a prohibited credential or high-confidence secret pattern.");
}

const allCSharpFiles = projectFiles.filter(function (file) {
  return file.endsWith(".cs");
});
const changedCSharpFiles = allowedChangedPaths.filter(function (file) {
  return file.endsWith(".cs") && exists(file);
});
for (const file of changedCSharpFiles) structuralCSharpCheck(file);

let playerPrefsDeleteAllAbsent = true;
for (const file of allCSharpFiles) {
  if (/\bPlayerPrefs\s*\.\s*DeleteAll\s*\(/.test(scrubCSharp(read(file)))) {
    fail(file + " invokes forbidden PlayerPrefs.DeleteAll().");
    playerPrefsDeleteAllAbsent = false;
  }
}

const diagnosticsFiles = projectFiles.filter(function (file) {
  return file.startsWith(diagnosticsPrefix + "/") && file.endsWith(".cs");
});
const diagnosticsSource = diagnosticsFiles.map(function (file) {
  return read(file);
}).join("\n");
const diagnosticsCode = scrubCSharp(diagnosticsSource);
const diagnosticsNoComments = stripCSharpComments(diagnosticsSource);
const expectedDiagnosticsFiles = [
  diagnosticsAssemblyAccessPath,
  bufferPath,
  determinismPath,
  modelsPath,
  policyPath,
  reasonCodesPath,
  transportPath,
  validatorPath,
  pipelinePath
].sort();
if (!sameArray(diagnosticsFiles.sort(), expectedDiagnosticsFiles))
  fail("DiagnosticsCore C# inventory must equal the exact reviewed source boundary.");

const forbiddenDiagnosticsPatterns = [
  ["Unity dependency", /\b(?:using\s+UnityEngine|UnityEngine\s*\.|MonoBehaviour\b|GameObject\b)/],
  ["Unity log callback", /\b(?:logMessageReceived|logMessageReceivedThreaded)\b/],
  ["runtime bootstrap", /\b(?:RuntimeInitializeOnLoadMethod|RuntimeBootstrap|Application\s*\.)\b/],
  ["console or debug logging", /\b(?:Debug\s*\.\s*Log\w*|Console\s*\.\s*Write)\b/],
  ["file or stream persistence", /\b(?:System\.IO|FileStream|StreamWriter|File\s*\.|Directory\s*\.|PlayerPrefs)\b/],
  ["network API", /\b(?:System\.Net|HttpClient|WebClient|UnityWebRequest|Socket|TcpClient|UdpClient|Dns\s*\.)\b/],
  ["task or background worker", /\b(?:System\.Threading\.Tasks|ThreadPool|Task\s*\.|Task<|BackgroundWorker)\b/],
  ["thread creation", /\bnew\s+(?:System\.Threading\.)?Thread\s*\(/],
  ["timer creation", /\bnew\s+(?:System\.Threading\.)?Timer\s*\(/],
  ["process launch", /\b(?:System\.Diagnostics\.)?(?:Process|ProcessStartInfo)\b/],
  ["native invocation", /\b(?:DllImport|LibraryImport)\b/],
  ["environment identifier", /\bEnvironment\s*\.\s*(?:MachineName|UserName|UserDomainName|GetEnvironmentVariable)\b/],
  ["nondeterministic identifier", /\bGuid\s*\.\s*NewGuid\s*\(/],
  ["type or namespace alias", /\busing\s+[A-Za-z_]\w*\s*=/],
  ["provider SDK", /\b(?:Firebase|Crashlytics|Sentry|Bugsnag|AppCenter|PlayFab|Backtrace|Datadog|NewRelic)\b/i],
  ["wall clock", /\b(?:DateTime|DateTimeOffset)\s*\.\s*(?:Now|UtcNow)\b/],
  ["random source", /\b(?:System\.)?Random\b/]
];
let diagnosticsForbiddenDependenciesAbsent = true;
for (const item of forbiddenDiagnosticsPatterns) {
  if (item[1].test(diagnosticsCode)) {
    fail("DiagnosticsCore contains forbidden " + item[0] + ".");
    diagnosticsForbiddenDependenciesAbsent = false;
  }
}
if (/(?:\$+@|@\$+|\$+)\s*"/.test(diagnosticsNoComments)) {
  fail("DiagnosticsCore contains an interpolated string whose executable expressions cannot be safely source-scrubbed.");
  diagnosticsForbiddenDependenciesAbsent = false;
}

const publicSensitiveSurfacePatterns = [
  /\bpublic\s+(?:[\w<>\[\],.?]+\s+)+(?:Message|Condition|StackTrace|Exception|Metadata|Context|Attachment|Identifier|PlayerId|AccountId|DeviceId|SessionId|SavePayload|Purchase|Receipt|Transaction|Credential|Endpoint|Url)\b/i,
  /\bErrorReport(?:Draft|Record)\s*\([^)]*\b(?:message|condition|stackTrace|exception|metadata|context|attachment|identifier|playerId|accountId|deviceId|sessionId|savePayload|purchase|receipt|transaction|credential|endpoint|url)\b/i,
  /\b(?:Dictionary|IDictionary)<[^>]+>/
];
const sensitiveNameBlacklistClear = publicSensitiveSurfacePatterns.every(function (pattern) {
  return !pattern.test(diagnosticsCode);
}) &&
  !/public\s+string\s+(?:ErrorCode|SiteCode)\b/.test(diagnosticsCode) &&
  /public\s+ErrorReportCode\s+ErrorCode\b/.test(diagnosticsCode) &&
  /public\s+ErrorReportSiteCode\s+SiteCode\b/.test(diagnosticsCode);

const modelsSource = exists(modelsPath) ? read(modelsPath) : "";
const policySource = exists(policyPath) ? read(policyPath) : "";
const validatorSource = exists(validatorPath) ? read(validatorPath) : "";
const determinismSource = exists(determinismPath) ? read(determinismPath) : "";
const bufferSource = exists(bufferPath) ? read(bufferPath) : "";
const transportSource = exists(transportPath) ? read(transportPath) : "";
const pipelineSource = exists(pipelinePath) ? read(pipelinePath) : "";
const diagnosticsAssemblyAccessSource = exists(diagnosticsAssemblyAccessPath)
  ? read(diagnosticsAssemblyAccessPath) : "";

const modelsCode = stripCSharpComments(modelsSource);
const policyCode = stripCSharpComments(policySource);
const validatorCode = stripCSharpComments(validatorSource);
const determinismCode = stripCSharpComments(determinismSource);
const bufferCode = stripCSharpComments(bufferSource);
const transportCode = stripCSharpComments(transportSource);
const pipelineCode = stripCSharpComments(pipelineSource);
const diagnosticsAssemblyAccessCode = stripCSharpComments(
  diagnosticsAssemblyAccessSource
);

const kindMembers = ["NonFatal=0", "ManagedCrash=1"];
const subsystemMembers = [
  "Bootstrap=0", "Runtime=1", "Gameplay=2", "Save=3", "Economy=4",
  "Commercial=5", "Ui=6", "Audio=7", "Content=8"
];
const errorCodeMembers = [
  "BootstrapFailure=0", "RuntimeInvariantFailure=1",
  "ManagedCrashUnhandled=2", "Match3PlanFailure=3", "SaveReadFailure=4",
  "SaveWriteFailure=5", "SaveMigrationFailure=6",
  "EconomyTransactionFailure=7", "CommercialAdapterFailure=8",
  "UiPresentationFailure=9", "AudioPlaybackFailure=10",
  "ContentLoadFailure=11"
];
const siteCodeMembers = [
  "BootstrapInitialize=0", "RuntimeInvariantCheck=1", "RuntimeUnhandled=2",
  "Match3ResolvePlan=3", "SaveReadDecode=4", "SaveWriteCommit=5",
  "SaveMigrationApply=6", "EconomyTransactionCommit=7",
  "CommercialAdapterInvoke=8", "UiPresentationUpdate=9",
  "AudioPlaybackInvoke=10", "ContentResourceLoad=11"
];
const deliveryStateMembers = [
  "Pending=0", "RetryScheduled=1", "RetryExhausted=2",
  "PermanentlyRejected=3"
];
const transportDispositionMembers = [
  "Delivered=0", "RetryableFailure=1", "PermanentFailure=2"
];

const exactEnumCatalogs =
  /\bpublic\s+enum\s+ErrorReportKind\b/.test(modelsCode) &&
  /\bpublic\s+enum\s+ErrorReportSubsystem\b/.test(modelsCode) &&
  /\bpublic\s+enum\s+ErrorReportCode\b/.test(modelsCode) &&
  /\bpublic\s+enum\s+ErrorReportSiteCode\b/.test(modelsCode) &&
  /\bpublic\s+enum\s+ErrorReportDeliveryState\b/.test(modelsCode) &&
  /\binternal\s+enum\s+ErrorReportTransportDisposition\b/.test(
    transportCode
  ) &&
  exactEnumMembers(modelsSource, "ErrorReportKind", kindMembers) &&
  exactEnumMembers(modelsSource, "ErrorReportSubsystem", subsystemMembers) &&
  exactEnumMembers(modelsSource, "ErrorReportCode", errorCodeMembers) &&
  exactEnumMembers(modelsSource, "ErrorReportSiteCode", siteCodeMembers) &&
  exactEnumMembers(modelsSource, "ErrorReportDeliveryState", deliveryStateMembers) &&
  exactEnumMembers(
    transportSource,
    "ErrorReportTransportDisposition",
    transportDispositionMembers
  );

const draftBlock = classBlock(modelsSource, "ErrorReportDraft");
const recordBlock = classBlock(modelsSource, "ErrorReportRecord");
const operationResultBlock = classBlock(modelsSource, "ErrorReportOperationResult");
const expectedDraftProperties = [
  "ErrorReportCode ErrorCode", "ErrorReportKind Kind",
  "ErrorReportSiteCode SiteCode", "ErrorReportSubsystem Subsystem"
].sort();
const expectedRecordProperties = [
  "ErrorReportCode ErrorCode", "ErrorReportDeliveryState DeliveryState",
  "ErrorReportKind Kind", "ErrorReportSiteCode SiteCode",
  "ErrorReportSubsystem Subsystem", "int DeliveryAttempts",
  "int SchemaVersion", "long FirstUtcTicks", "long LastUtcTicks",
  "long NextAttemptUtcTicks", "long OccurrenceCount", "long Sequence",
  "string Fingerprint"
].sort();
const expectedOperationResultProperties = [
  "bool Accepted", "bool Succeeded", "long Sequence", "string Fingerprint",
  "string ReasonCode"
].sort();
const exactModelPropertyInventory =
  sameArray(publicPropertyInventory(draftBlock), expectedDraftProperties) &&
  sameArray(publicPropertyInventory(recordBlock), expectedRecordProperties) &&
  sameArray(
    publicPropertyInventory(operationResultBlock),
    expectedOperationResultProperties
  );
const exactModelConstructorInventory =
  sameArray(constructorInventory(draftBlock, "ErrorReportDraft"), [
    "public(ErrorReportKind kind,ErrorReportSubsystem subsystem," +
      "ErrorReportCode errorCode,ErrorReportSiteCode siteCode)"
  ]) &&
  sameArray(constructorInventory(recordBlock, "ErrorReportRecord"), [
    "internal(int schemaVersion,long sequence,string fingerprint," +
      "ErrorReportKind kind,ErrorReportSubsystem subsystem," +
      "ErrorReportCode errorCode,ErrorReportSiteCode siteCode," +
      "long firstUtcTicks,long lastUtcTicks,long occurrenceCount," +
      "ErrorReportDeliveryState deliveryState,int deliveryAttempts," +
      "long nextAttemptUtcTicks)"
  ]) &&
  sameArray(
    constructorInventory(operationResultBlock, "ErrorReportOperationResult"),
    ["private(bool accepted,string reasonCode,long sequence,string fingerprint)"]
  );
const exactModelPublicMethodInventory =
  sameArray(publicMethodInventory(draftBlock), []) &&
  sameArray(publicMethodInventory(recordBlock), [
    "ErrorReportRecord DetachedCopy"
  ]) &&
  sameArray(publicMethodInventory(operationResultBlock), [
    "static ErrorReportOperationResult Reject",
    "static ErrorReportOperationResult Success"
  ]);
const exactModelPublicFieldInventory =
  sameArray(publicFieldInventory(draftBlock), []) &&
  sameArray(publicFieldInventory(recordBlock), []) &&
  sameArray(publicFieldInventory(operationResultBlock), []);
const callerSuppliedCodeStringsAccepted = !(
  exactModelPropertyInventory && exactModelConstructorInventory &&
  exactModelPublicMethodInventory && exactModelPublicFieldInventory &&
  !/\bErrorReportDraft\s*\([^)]*\bstring\b/.test(draftBlock?.scrubbed || "")
);
const minimizedPublicSurface =
  sensitiveNameBlacklistClear && exactModelPropertyInventory &&
  exactModelConstructorInventory && exactModelPublicMethodInventory &&
  exactModelPublicFieldInventory &&
  callerSuppliedCodeStringsAccepted === false;
if (!minimizedPublicSurface)
  fail("Diagnostics public contracts differ from the exact minimized model surface.");

const validErrorCodeBlock = extractCSharpBlock(
  validatorSource,
  /public\s+static\s+bool\s+IsValidErrorCode\s*\([^)]*\)\s*/
);
const validSiteCodeBlock = extractCSharpBlock(
  validatorSource,
  /public\s+static\s+bool\s+IsValidSiteCode\s*\([^)]*\)\s*/
);
const reviewedTupleBlock = extractCSharpBlock(
  validatorSource,
  /public\s+static\s+bool\s+IsReviewedTuple\s*\([^)]*\)\s*/
);
const validSubsystemBlock = extractCSharpBlock(
  validatorSource,
  /static\s+bool\s+IsValidSubsystem\s*\([^)]*\)\s*/
);
const validDeliveryStateBlock = extractCSharpBlock(
  validatorSource,
  /static\s+bool\s+IsValidDeliveryState\s*\([^)]*\)\s*/
);
const expectedErrorNames = errorCodeMembers.map(function (member) {
  return member.split("=")[0];
});
const expectedSiteNames = siteCodeMembers.map(function (member) {
  return member.split("=")[0];
});
const expectedSubsystemNames = subsystemMembers.map(function (member) {
  return member.split("=")[0];
});
const expectedDeliveryNames = deliveryStateMembers.map(function (member) {
  return member.split("=")[0];
});
const validatorCatalogsExact =
  sameArray(
    caseLabelInventory(validErrorCodeBlock, "ErrorReportCode"),
    expectedErrorNames
  ) &&
  sameArray(
    caseLabelInventory(validSiteCodeBlock, "ErrorReportSiteCode"),
    expectedSiteNames
  ) &&
  sameArray(
    caseLabelInventory(validSubsystemBlock, "ErrorReportSubsystem"),
    expectedSubsystemNames
  ) &&
  sameArray(
    caseLabelInventory(validDeliveryStateBlock, "ErrorReportDeliveryState"),
    expectedDeliveryNames
  ) &&
  /return\s+kind\s*==\s*ErrorReportKind\.NonFatal\s*\|\|\s*kind\s*==\s*ErrorReportKind\.ManagedCrash\s*;/.test(
    validatorCode
  );

const reviewedTupleMappings = [
  ["BootstrapFailure", "Bootstrap", "BootstrapInitialize"],
  ["RuntimeInvariantFailure", "Runtime", "RuntimeInvariantCheck"],
  ["ManagedCrashUnhandled", "Runtime", "RuntimeUnhandled"],
  ["Match3PlanFailure", "Gameplay", "Match3ResolvePlan"],
  ["SaveReadFailure", "Save", "SaveReadDecode"],
  ["SaveWriteFailure", "Save", "SaveWriteCommit"],
  ["SaveMigrationFailure", "Save", "SaveMigrationApply"],
  ["EconomyTransactionFailure", "Economy", "EconomyTransactionCommit"],
  ["CommercialAdapterFailure", "Commercial", "CommercialAdapterInvoke"],
  ["UiPresentationFailure", "Ui", "UiPresentationUpdate"],
  ["AudioPlaybackFailure", "Audio", "AudioPlaybackInvoke"],
  ["ContentLoadFailure", "Content", "ContentResourceLoad"]
];
let reviewedTupleMappingsExact = Boolean(reviewedTupleBlock) && sameArray(
  caseLabelInventory(reviewedTupleBlock, "ErrorReportCode"),
  expectedErrorNames
);
const reviewedTupleCode = reviewedTupleBlock
  ? stripCSharpComments(reviewedTupleBlock.source) : "";
for (const mapping of reviewedTupleMappings) {
  const pattern = new RegExp(
    "\\bcase\\s+ErrorReportCode\\." + mapping[0] +
      "\\s*:\\s*return\\s+draft\\.Subsystem\\s*==\\s*" +
      "ErrorReportSubsystem\\." + mapping[1] +
      "\\s*&&\\s*draft\\.SiteCode\\s*==\\s*ErrorReportSiteCode\\." +
      mapping[2] + "\\s*;"
  );
  if (!pattern.test(reviewedTupleCode)) reviewedTupleMappingsExact = false;
}

const kindWireMappings = [
  ["NonFatal", "nonfatal"], ["ManagedCrash", "managed_crash"]
];
const subsystemWireMappings = [
  ["Bootstrap", "bootstrap"], ["Runtime", "runtime"],
  ["Gameplay", "gameplay"], ["Save", "save"], ["Economy", "economy"],
  ["Commercial", "commercial"], ["Ui", "ui"], ["Audio", "audio"],
  ["Content", "content"]
];
const errorWireMappings = [
  ["BootstrapFailure", "BOOTSTRAP_FAILURE"],
  ["RuntimeInvariantFailure", "RUNTIME_INVARIANT_FAILURE"],
  ["ManagedCrashUnhandled", "MANAGED_CRASH_UNHANDLED"],
  ["Match3PlanFailure", "MATCH3_PLAN_FAILURE"],
  ["SaveReadFailure", "SAVE_READ_FAILURE"],
  ["SaveWriteFailure", "SAVE_WRITE_FAILURE"],
  ["SaveMigrationFailure", "SAVE_MIGRATION_FAILURE"],
  ["EconomyTransactionFailure", "ECONOMY_TRANSACTION_FAILURE"],
  ["CommercialAdapterFailure", "COMMERCIAL_ADAPTER_FAILURE"],
  ["UiPresentationFailure", "UI_PRESENTATION_FAILURE"],
  ["AudioPlaybackFailure", "AUDIO_PLAYBACK_FAILURE"],
  ["ContentLoadFailure", "CONTENT_LOAD_FAILURE"]
];
const siteWireMappings = [
  ["BootstrapInitialize", "bootstrap.initialize"],
  ["RuntimeInvariantCheck", "runtime.invariant.check"],
  ["RuntimeUnhandled", "runtime.unhandled"],
  ["Match3ResolvePlan", "match3.resolve.plan"],
  ["SaveReadDecode", "save.read.decode"],
  ["SaveWriteCommit", "save.write.commit"],
  ["SaveMigrationApply", "save.migration.apply"],
  ["EconomyTransactionCommit", "economy.transaction.commit"],
  ["CommercialAdapterInvoke", "commercial.adapter.invoke"],
  ["UiPresentationUpdate", "ui.presentation.update"],
  ["AudioPlaybackInvoke", "audio.playback.invoke"],
  ["ContentResourceLoad", "content.resource.load"]
];
const deliveryWireMappings = [
  ["Pending", "pending"], ["RetryScheduled", "retry_scheduled"],
  ["RetryExhausted", "retry_exhausted"],
  ["PermanentlyRejected", "permanently_rejected"]
];
const wireCatalogsExact =
  exactCaseReturnMappings(
    extractCSharpBlock(
      determinismSource,
      /internal\s+static\s+string\s+Kind\s*\([^)]*\)\s*/
    ),
    "ErrorReportKind",
    kindWireMappings
  ) &&
  exactCaseReturnMappings(
    extractCSharpBlock(
      determinismSource,
      /internal\s+static\s+string\s+Subsystem\s*\([^)]*\)\s*/
    ),
    "ErrorReportSubsystem",
    subsystemWireMappings
  ) &&
  exactCaseReturnMappings(
    extractCSharpBlock(
      determinismSource,
      /internal\s+static\s+string\s+ErrorCode\s*\([^)]*\)\s*/
    ),
    "ErrorReportCode",
    errorWireMappings
  ) &&
  exactCaseReturnMappings(
    extractCSharpBlock(
      determinismSource,
      /internal\s+static\s+string\s+SiteCode\s*\([^)]*\)\s*/
    ),
    "ErrorReportSiteCode",
    siteWireMappings
  ) &&
  exactCaseReturnMappings(
    extractCSharpBlock(
      determinismSource,
      /internal\s+static\s+string\s+DeliveryState\s*\([^)]*\)\s*/
    ),
    "ErrorReportDeliveryState",
    deliveryWireMappings
  );

const closedReviewedCodeCatalog =
  exactEnumCatalogs && validatorCatalogsExact &&
  reviewedTupleMappingsExact && wireCatalogsExact;
const schemaAndValidationPinned =
  /CurrentSchemaVersion\s*=\s*1\s*;/.test(modelsCode) &&
  /MaximumErrorCodeLength\s*=\s*64\s*;/.test(modelsCode) &&
  /MaximumSiteCodeLength\s*=\s*64\s*;/.test(modelsCode) &&
  closedReviewedCodeCatalog &&
  validatorCode.includes("ValidateDraft") &&
  validatorCode.includes("ValidateRecord") &&
  validatorCode.includes("CodeTupleInvalid") &&
  validatorCode.includes("IsLowerHexSha256");
if (!schemaAndValidationPinned)
  fail("Diagnostics schema, exact reviewed catalogs, tuple map, or wire map drifted.");

const policyLimitsPinned =
  /DefaultCapacity\s*=\s*64\s*;/.test(policyCode) &&
  /MaximumCapacity\s*=\s*256\s*;/.test(policyCode) &&
  /10L\s*\*\s*TimeSpan\.TicksPerMinute/.test(policyCode) &&
  /7L\s*\*\s*TimeSpan\.TicksPerDay/.test(policyCode) &&
  /30L\s*\*\s*TimeSpan\.TicksPerDay/.test(policyCode) &&
  /DefaultMaximumDeliveryAttempts\s*=\s*5\s*;/.test(policyCode) &&
  /MaximumDeliveryAttemptsLimit\s*=\s*10\s*;/.test(policyCode) &&
  /30L\s*\*\s*TimeSpan\.TicksPerSecond/.test(policyCode) &&
  /TimeSpan\.TicksPerHour/.test(policyCode) &&
  policyCode.includes("RetryDelayTicks") &&
  policyCode.includes("SaturatingAdd");
if (!policyLimitsPinned)
  fail("Diagnostics capacity, retention, deduplication, or retry policy limits drifted.");

const deterministicFingerprintPinned =
  determinismCode.includes("SHA256.Create()") &&
  determinismCode.includes("UTF8Encoding(false, true)") &&
  determinismCode.includes("CultureInfo.InvariantCulture") &&
  determinismCode.includes("TryComputeFingerprint") &&
  determinismCode.includes("ErrorReportSchema.CurrentSchemaVersion") &&
  determinismCode.includes("ErrorCode(draft.ErrorCode)") &&
  determinismCode.includes("SiteCode(draft.SiteCode)") &&
  determinismCode.includes('const string Hex = "0123456789abcdef"');
if (!deterministicFingerprintPinned)
  fail("Diagnostics fingerprint must remain deterministic lower-hex SHA-256 over the minimized tuple.");

const canonicalSerializerPinned =
  determinismCode.includes("Serialize(ErrorReportRecord record)") &&
  determinismCode.includes("SerializeTrace") &&
  determinismCode.includes('Name(json, "schemaVersion")') &&
  determinismCode.includes('Name(json, "sequence")') &&
  determinismCode.includes('Name(json, "fingerprint")') &&
  determinismCode.includes('Name(json, "errorCode")') &&
  determinismCode.includes('Name(json, "siteCode")') &&
  determinismCode.includes("ordered.Sort(CompareRecords)") &&
  determinismCode.includes("left.Sequence.CompareTo(right.Sequence)") &&
  determinismCode.includes("CultureInfo.InvariantCulture") &&
  !/JsonUtility|JsonConvert|System\.Text\.Json/.test(determinismCodeSafe());
if (!canonicalSerializerPinned)
  fail("Diagnostics canonical JSON serializer markers are incomplete or provider-dependent.");

function determinismCodeSafe() {
  return scrubCSharp(determinismSource);
}

const boundedBufferPinned =
  bufferCode.includes("sealed class BoundedErrorReportBuffer") &&
  bufferCode.includes("records.Count >= policy.Capacity") &&
  bufferCode.includes("policy.DeduplicationWindowTicks") &&
  bufferCode.includes("policy.RetentionTicks") &&
  bufferCode.includes("existing.OccurrenceCount == long.MaxValue") &&
  bufferCode.includes("records[i].DetachedCopy()") &&
  bufferCode.includes("new ReadOnlyCollection<ErrorReportRecord>") &&
  bufferCode.includes("MarkRetryableFailure") &&
  bufferCode.includes("MarkPermanentFailure") &&
  bufferCode.includes("MarkDelivered");
if (!boundedBufferPinned)
  fail("Bounded buffer is missing reviewed capacity, deduplication, retention, snapshot, or delivery-state markers.");

const inclusiveDedupAndHardRetentionPinned =
  /utcTicks\s*-\s*existing\.LastUtcTicks\s*>\s*policy\.DeduplicationWindowTicks/.test(
    scrubCSharp(bufferSource)
  ) &&
  /utcTicks\s*-\s*record\.FirstUtcTicks\s*>=\s*policy\.RetentionTicks/.test(
    scrubCSharp(bufferSource)
  );
if (!inclusiveDedupAndHardRetentionPinned)
  fail("Deduplication must be inclusive and hard retention must use first occurrence.");

const pipelineBlock = classBlock(pipelineSource, "ErrorReportingPipeline");
const exactPipelinePublicSurface =
  sameArray(publicPropertyInventory(pipelineBlock), [
    "ErrorReportPolicy Policy", "bool CaptureAllowed",
    "bool NetworkTransportAllowed", "int BufferedCount"
  ].sort()) &&
  sameArray(publicMethodInventory(pipelineBlock), [
    "ErrorReportOperationResult TryCapture",
    "ErrorReportOperationResult TryFlushOne",
    "IReadOnlyList<ErrorReportRecord> Snapshot",
    "int PruneExpired", "string CanonicalSnapshot"
  ].sort()) &&
  sameArray(publicFieldInventory(pipelineBlock), []) &&
  sameArray(constructorInventory(pipelineBlock, "ErrorReportingPipeline"), [
    "internal(bool captureAllowed,ErrorReportPolicy policy," +
      "IErrorReportTransport transport)",
    "public(bool captureAllowed,ErrorReportPolicy policy=null)"
  ].sort());

const friendAssemblies = [];
const friendPattern = /\bInternalsVisibleTo\s*\(\s*"([^"]+)"\s*\)/g;
let friendMatch;
while ((friendMatch = friendPattern.exec(diagnosticsAssemblyAccessCode)) !== null)
  friendAssemblies.push(friendMatch[1]);
friendAssemblies.sort();
const exactFriendAssemblyBoundary = sameArray(friendAssemblies, [
  "Assembly-CSharp-Editor",
  "LegendsOfRus.DiagnosticsCore.PlayModeTests",
  "LegendsOfRus.DiagnosticsCore.Tests"
].sort());

const exactInternalTransportBoundary =
  /\binternal\s+enum\s+ErrorReportTransportDisposition\b/.test(transportCode) &&
  /\binternal\s+sealed\s+class\s+ErrorReportTransportResult\b/.test(
    transportCode
  ) &&
  /\binternal\s+interface\s+IErrorReportTransport\b/.test(transportCode) &&
  /\binternal\s+sealed\s+class\s+DisabledErrorReportTransport\s*:\s*IErrorReportTransport\b/.test(
    transportCode
  ) &&
  exactPipelinePublicSurface && exactFriendAssemblyBoundary;

function typeBaseGraph(source) {
  const graph = new Map();
  const pattern = /\b(class|struct|interface)\s+([A-Za-z_]\w*)(?:\s*<[^>{}]+>)?\s*(?::\s*([^{}]+))?\s*\{/g;
  let match;
  while ((match = pattern.exec(source)) !== null) {
    const bases = (match[3] || "").split(",").map(function (value) {
      return value.trim().replace(/<.*>/g, "").split(".").pop().trim();
    }).filter(Boolean);
    graph.set(match[2], { kind: match[1], bases });
  }
  return graph;
}

const diagnosticsTypeGraph = typeBaseGraph(diagnosticsCode);
function derivesFrom(typeName, target, active = new Set()) {
  if (active.has(typeName)) return false;
  active.add(typeName);
  const entry = diagnosticsTypeGraph.get(typeName);
  if (!entry) return false;
  return entry.bases.some(function (base) {
    return base === target || derivesFrom(base, target, new Set(active));
  });
}
const shippedTransportImplementations = [...diagnosticsTypeGraph.entries()]
  .filter(function (entry) {
    return (entry[1].kind === "class" || entry[1].kind === "struct") &&
      derivesFrom(entry[0], "IErrorReportTransport");
  })
  .map(function (entry) { return entry[0]; })
  .sort();
const onlyDisabledTransportShipped =
  sameArray(shippedTransportImplementations, ["DisabledErrorReportTransport"]);
if (!onlyDisabledTransportShipped)
  fail("DisabledErrorReportTransport must be the only shipped transport implementation.");

const networkApiPattern =
  /\b(?:System\.Net|HttpClient|WebClient|UnityWebRequest|Socket|TcpClient|UdpClient|Dns\s*\.)\b/;
const providerSdkPattern =
  /\b(?:Firebase|Crashlytics|Sentry|Bugsnag|AppCenter|PlayFab|Backtrace|Datadog|NewRelic)\b/i;
const diagnosticsNetworkApiAbsent =
  !networkApiPattern.test(diagnosticsCode) &&
  !providerSdkPattern.test(diagnosticsCode) &&
  !/\b(?:DllImport|LibraryImport)\b/.test(diagnosticsCode);
const networkTransportCompiledFalse =
  /\bprivate\s+const\s+bool\s+NetworkTransportCompiled\s*=\s*false\s*;/.test(
    pipelineCode
  );
const publicTransportInjectionPresent = !(
  exactInternalTransportBoundary && onlyDisabledTransportShipped
);

const retryAndFailureIsolationPinned =
  /private\s+const\s+bool\s+NetworkTransportCompiled\s*=\s*false\s*;/.test(
    pipelineCode
  ) &&
  /NetworkTransportAllowed\s*\{\s*get\s*\{\s*return\s+false\s*;\s*\}\s*\}/.test(
    scrubCSharp(pipelineSource)
  ) &&
  pipelineCode.includes("if (!isOnline)") &&
  pipelineCode.includes("buffer.PruneExpired(utcTicks)") &&
  pipelineCode.includes("if (supportsNetwork && !NetworkTransportCompiled)") &&
  pipelineCode.includes("transportResult == null") &&
  pipelineCode.includes("catch (Exception)") &&
  pipelineCode.includes("TryGetNextDue") &&
  pipelineCode.includes("TryFlushOne") &&
  bufferCode.includes("policy.RetryDelayTicks(attempts)") &&
  bufferCode.includes("attempts >= policy.MaximumDeliveryAttempts") &&
  transportCode.includes("bool DeliveryEnabled") &&
  transportCode.includes("bool SupportsNetwork") &&
  pipelineCode.includes("DisabledErrorReportTransport.Instance") &&
  pipelineCode.includes("TryBeginOperation") &&
  pipelineCode.includes("EndOperation") &&
  exactInternalTransportBoundary &&
  diagnosticsNetworkApiAbsent &&
  /SupportsNetwork\s*\{\s*get\s*\{\s*return\s+false\s*;/.test(
    scrubCSharp(transportSource)
  );
if (!retryAndFailureIsolationPinned)
  fail("Offline, disabled, network-blocked, retry, and adapter-failure isolation markers are incomplete.");

const asmdefFiles = projectFiles.filter(function (file) {
  return file.endsWith(".asmdef");
});
const asmdefs = new Map();
for (const file of asmdefFiles) {
  const value = parseJson(file);
  if (!value || typeof value.name !== "string") continue;
  if (asmdefs.has(value.name)) fail("Duplicate assembly name '" + value.name + "'.");
  asmdefs.set(value.name, { file, value });
}

function validateAsmdef(file, expected) {
  const value = exists(file) ? parseJson(file) : null;
  if (!value) return false;
  const valid =
    value.name === expected.name &&
    sameSet(value.references || [], expected.references) &&
    sameArray(value.includePlatforms || [], expected.includePlatforms) &&
    sameArray(value.excludePlatforms || [], []) &&
    value.allowUnsafeCode === false &&
    value.overrideReferences === false &&
    sameArray(value.precompiledReferences || [], []) &&
    value.autoReferenced === true &&
    sameArray(value.defineConstraints || [], []) &&
    sameArray(value.versionDefines || [], []) &&
    value.noEngineReferences === expected.noEngineReferences &&
    (expected.testAssembly
      ? sameArray(value.optionalUnityReferences, ["TestAssemblies"])
      : value.optionalUnityReferences === undefined);
  if (!valid) fail(file + " does not match the reviewed assembly shape.");
  return valid;
}

const diagnosticsAsmdefValid = validateAsmdef(diagnosticsAsmdefPath, {
  name: "LegendsOfRus.DiagnosticsCore",
  references: [],
  includePlatforms: [],
  noEngineReferences: true,
  testAssembly: false
});
const editModeTestsAsmdefValid = validateAsmdef(editModeTestsAsmdefPath, {
  name: "LegendsOfRus.DiagnosticsCore.Tests",
  references: ["LegendsOfRus.DiagnosticsCore"],
  includePlatforms: ["Editor"],
  noEngineReferences: true,
  testAssembly: true
});
const playModeTestsAsmdefValid = validateAsmdef(playModeTestsAsmdefPath, {
  name: "LegendsOfRus.DiagnosticsCore.PlayModeTests",
  references: ["LegendsOfRus.DiagnosticsCore"],
  includePlatforms: [],
  noEngineReferences: false,
  testAssembly: true
});

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
  for (const reference of entry?.value.references || []) {
    if (asmdefs.has(reference)) visitAssembly(reference);
  }
  visiting.delete(name);
  visited.add(name);
}
for (const name of asmdefs.keys()) visitAssembly(name);
if (!assemblyGraphAcyclic) fail("Assembly definition graph contains a cycle.");

const testAttributePattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
function countTestAttributes(file) {
  return scrubCSharp(read(file)).match(testAttributePattern)?.length || 0;
}
const sourceTestAttributes = allCSharpFiles
  .filter(function (file) { return file.includes("/Tests/"); })
  .reduce(function (total, file) { return total + countTestAttributes(file); }, 0);
const diagnosticsTestFiles = [editModeTestsPath, playModeTestsPath]
  .filter(function (file) { return exists(file); });
const diagnosticsTestAttributes = diagnosticsTestFiles.reduce(
  function (total, file) { return total + countTestAttributes(file); }, 0
);
let diagnosticsTestPolicyValid = diagnosticsTestFiles.length === 2;
const forbiddenTestPatterns = [
  ["forbidden annotation", /\[\s*(?:NonParallelizable|Ignore|Explicit)\b/],
  ["wall-clock delay", /\b(?:Thread\s*\.\s*Sleep|Task\s*\.\s*Delay|WaitForSeconds)\b/],
  ["wall clock", /\b(?:DateTime|DateTimeOffset)\s*\.\s*(?:Now|UtcNow)\b/],
  ["random source", /\b(?:System\.)?Random\b/],
  ["network API", /\b(?:System\.Net|HttpClient|UnityWebRequest|WebClient|Socket)\b/],
  ["global save deletion", /\bPlayerPrefs\s*\.\s*DeleteAll\s*\(/]
];
for (const file of diagnosticsTestFiles) {
  const source = scrubCSharp(read(file));
  for (const item of forbiddenTestPatterns) {
    if (item[1].test(source)) {
      fail(file + " contains " + item[0] + ".");
      diagnosticsTestPolicyValid = false;
    }
  }
}
if (diagnosticsTestAttributes < 1) {
  fail("Step 3 diagnostics suites have no source test attributes.");
  diagnosticsTestPolicyValid = false;
}

const playModeTestSource = exists(playModeTestsPath) ? read(playModeTestsPath) : "";
const playModeTestCode = scrubCSharp(playModeTestSource);
const saveKeyLiterals = [
  '"LegendsOfRus_Save_v1"',
  '"LegendsOfRus_Save_v1_Backup"',
  '"LegendsOfRus_Save_v1_Staging"'
];
const playModePureAndSaveIsolated =
  saveKeyLiterals.every(function (literal) {
    return playModeTestSource.includes(literal);
  }) &&
  /\bPlayerPrefs\s*\.\s*HasKey\s*\(/.test(playModeTestCode) &&
  /\bPlayerPrefs\s*\.\s*GetString\s*\(/.test(playModeTestCode) &&
  /\bPlayerPrefs\s*\.\s*SetString\s*\(/.test(playModeTestCode) &&
  /\bPlayerPrefs\s*\.\s*DeleteKey\s*\(/.test(playModeTestCode) &&
  !/\bnew\s+GameObject\s*\(|\bObject\s*\.\s*Instantiate\s*\(/.test(
    playModeTestCode
  );
if (!playModePureAndSaveIsolated)
  fail("Diagnostics PlayMode tests must create no GameObject and preserve all three save keys.");

const selfTestSource = exists(selfTestPath) ? read(selfTestPath) : "";
const selfTestCode = scrubCSharp(selfTestSource);
const selfTestCheckSites = Math.max(
  0,
  (selfTestCode.match(/\bCheck\s*\(/g)?.length || 0) - 1
);
if (selfTestCheckSites < 20)
  fail("v1.17 Step 3 Editor self-test must contain at least 20 Check sites.");
const selfTestCoverageMarkers = [
  "ErrorReportPolicy", "TryComputeFingerprint", "CanonicalSnapshot",
  "DefaultCapacity", "MaximumCapacity", "Deduplicated", "RetentionTicks",
  "Offline", "RetryScheduled", "RetryDelayTicks", "RetryExhausted",
  "PermanentlyRejected", "TransportFailure", "NetworkTransportBlocked",
  "DisabledErrorReportTransport", "TryFlushOne", "Snapshot"
];
const selfTestCoverageComplete = selfTestCoverageMarkers.every(function (marker) {
  return selfTestSource.includes(marker);
});
if (!selfTestCoverageComplete)
  fail("Step 3 Editor self-test is missing required diagnostics coverage markers.");

const currentPngs = projectFiles.filter(function (file) {
  return file.toLowerCase().endsWith(".png");
});
const currentPngInventorySha256 = inventoryFingerprint(root, currentPngs);
if (currentPngs.length !== PNG_FILE_COUNT)
  fail("Expected exactly " + PNG_FILE_COUNT + " approved PNGs, found " + currentPngs.length + ".");
if (currentPngInventorySha256 !== PNG_INVENTORY_SHA256)
  fail("Approved PNG inventory differs from the reviewed Step 2 bytes.");

let changedManifestValid = false;
let changedManifestPaths = [];
if (exists(changedManifestPath)) {
  const source = read(changedManifestPath);
  if (!source.endsWith("\n")) fail(changedManifestPath + " must end with a newline.");
  const rawLines = source.split(/\r?\n/);
  if (rawLines[rawLines.length - 1] === "") rawLines.pop();
  changedManifestPaths = rawLines;
  const validLines = changedManifestPaths.every(function (line) {
    return line.length > 0 && line === line.trim() && !line.startsWith("/") &&
      !line.includes("\\") && !line.split("/").includes("..");
  });
  if (!validLines) fail(changedManifestPath + " contains an invalid relative path.");
  if (new Set(changedManifestPaths).size !== changedManifestPaths.length)
    fail(changedManifestPath + " contains duplicate paths.");
  const sorted = sameArray(changedManifestPaths, [...changedManifestPaths].sort());
  if (!sorted) fail(changedManifestPath + " must be sorted exactly.");
  changedManifestValid = validLines && sorted &&
    sameArray(changedManifestPaths, allowedChangedPaths);
  if (!changedManifestValid)
    fail(changedManifestPath + " does not equal the exact reviewed delta allowlist.");
}

const implementation = exists(implementationStatusPath)
  ? parseJson(implementationStatusPath) : null;
let implementationStatusValid = false;
if (implementation) {
  implementationStatusValid =
    implementation.version === VERSION &&
    implementation.milestone === VERSION &&
    implementation.status === IMPLEMENTATION_STATUS &&
    implementation.candidate_status === CANDIDATE_STATUS &&
    implementation.unity_version === UNITY_VERSION &&
    implementation.pre_entry_source_candidate === true &&
    implementation.step2_entry_evidence_retained === false &&
    implementation.unity_execution_evidence === "NOT_RETAINED" &&
    implementation.unity_compile_status === "NOT_RETAINED" &&
    implementation.project_validator_status === "NOT_RETAINED" &&
    implementation.editor_self_test_status === "NOT_RETAINED" &&
    implementation.editmode_test_status === "NOT_RETAINED" &&
    implementation.playmode_test_status === "NOT_RETAINED" &&
    implementation.representative_device_status === "NOT_RETAINED" &&
    implementation.diagnostics_schema_version === 1 &&
    implementation.engine_free_diagnostics_core === true &&
    implementation.typed_minimized_records_only === true &&
    implementation.closed_reviewed_code_catalog === true &&
    implementation.caller_supplied_code_strings_accepted === false &&
    implementation.process_memory_buffer_only === true &&
    implementation.durable_diagnostics_persistence_implemented === false &&
    implementation.default_buffer_capacity === 64 &&
    implementation.maximum_buffer_capacity === 256 &&
    implementation.raw_message_field_present === false &&
    implementation.raw_stack_trace_field_present === false &&
    implementation.exception_object_field_present === false &&
    implementation.arbitrary_metadata_field_present === false &&
    implementation.identifier_field_present === false &&
    implementation.save_or_purchase_payload_field_present === false &&
    implementation.unity_log_subscription_implemented === false &&
    implementation.runtime_bootstrap_wiring_implemented === false &&
    implementation.production_crash_sdk_installed === false &&
    implementation.production_crash_transport_compiled === false &&
    implementation.network_transport_implemented === false &&
    implementation.public_transport_injection_present === false &&
    implementation.background_upload_implemented === false &&
    implementation.package_lock_approved === false &&
    implementation.economy_activation_authorized === false &&
    implementation.commercial_activation_authorized === false &&
    implementation.cloud_recovery_authorized === false &&
    implementation.production_android_build_authorized === false &&
    implementation.publishing_authorized === false;
  if (!implementationStatusValid)
    fail(implementationStatusPath + " overstates evidence, data scope, or activation authority.");
}

const activationEvidence = exists(activationEvidencePath)
  ? parseJson(activationEvidencePath) : null;
let activationEvidenceValid = false;
if (activationEvidence) {
  const retention = activationEvidence.evidenceRetention;
  const capabilities = activationEvidence.effectiveCapabilities;
  const retentionValid = isObject(retention) &&
    Object.values(retention).every(function (value) {
      return value === "NOT_RETAINED";
    });
  const capabilitiesValid = isObject(capabilities) &&
    capabilities.typedMinimizedDrafts === true &&
    capabilities.closedReviewedCodeCatalog === true &&
    capabilities.boundedProcessMemoryBuffer === true &&
    capabilities.deterministicDeduplication === true &&
    capabilities.deterministicRetryPolicy === true &&
    capabilities.canonicalLocalInspection === true &&
    capabilities.durableDiagnosticsPersistence === false &&
    capabilities.unityLogCapture === false &&
    capabilities.runtimeDiagnosticsWiring === false &&
    capabilities.productionCrashTransport === false &&
    capabilities.publicTransportInjection === false &&
    capabilities.networkUpload === false &&
    capabilities.providerSdk === false &&
    capabilities.productionAndroidBuild === false;
  activationEvidenceValid =
    activationEvidence.schemaVersion === 1 &&
    activationEvidence.milestone === VERSION &&
    activationEvidence.status ===
      "PRE_ENTRY_SOURCE_CANDIDATE_PRODUCTION_TRANSPORT_BLOCKED" &&
    activationEvidence.implementationStatus === IMPLEMENTATION_STATUS &&
    activationEvidence.candidateStatus === CANDIDATE_STATUS &&
    activationEvidence.unityVersion === UNITY_VERSION &&
    activationEvidence.step2EntryEvidenceRetained === false &&
    retentionValid && capabilitiesValid &&
    activationEvidence.packageLockApproved === false &&
    activationEvidence.productionCrashTransportCompiled === false &&
    activationEvidence.productionCrashTransportAuthorized === false &&
    activationEvidence.productionAndroidBuildAuthorized === false &&
    activationEvidence.economyActivationAuthorized === false &&
    activationEvidence.commercialActivationAuthorized === false &&
    activationEvidence.cloudRecoveryAuthorized === false &&
    activationEvidence.publishingAuthorized === false;
  if (!activationEvidenceValid)
    fail(activationEvidencePath + " contains an unsupported evidence or activation claim.");
}

const checkpoint = exists(checkpointPath) ? parseJson(checkpointPath) : null;
const checkpointValid = Boolean(
  checkpoint &&
  checkpoint.status === "PENDING_RETAINED_PREREQUISITE_AND_UNITY_EVIDENCE" &&
  checkpoint.implementationStatus === IMPLEMENTATION_STATUS &&
  checkpoint.candidateStatus === CANDIDATE_STATUS &&
  checkpoint.step2EntryEvidenceRetained === false &&
  checkpoint.packageLockApproved === false &&
  checkpoint.productionCrashTransportAuthorized === false &&
  checkpoint.productionBuildAuthorized === false &&
  Array.isArray(checkpoint.requiredSequence) && checkpoint.requiredSequence.length >= 8 &&
  Array.isArray(checkpoint.exitCriteria) && checkpoint.exitCriteria.length >= 8
);
if (!checkpointValid)
  fail(checkpointPath + " must preserve the pending prerequisite and production hard stop.");

const nextMilestone = exists(nextMilestonePath) ? parseJson(nextMilestonePath) : null;
const nextMilestoneValid = Boolean(
  nextMilestone && nextMilestone.milestone === "v1.17-step04" &&
  nextMilestone.status ===
    "PLANNED_AFTER_V1_17_STEP03_PREREQUISITE_AND_UNITY_VERIFICATION" &&
  nextMilestone.productionBuildAuthorized === false &&
  Array.isArray(nextMilestone.entryCriteria) && nextMilestone.entryCriteria.length >= 4
);
if (!nextMilestoneValid)
  fail(nextMilestonePath + " must remain gated on Step 3 prerequisite and Unity verification.");

const staticReport = exists(staticReportPath) ? parseJson(staticReportPath) : null;
let staticReportValid = false;
if (staticReport) {
  staticReportValid =
    staticReport.version === VERSION &&
    staticReport.status === "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION" &&
    staticReport.passed === true &&
    staticReport.implementation_status === IMPLEMENTATION_STATUS &&
    staticReport.candidate_status === CANDIDATE_STATUS &&
    staticReport.pre_entry_source_candidate === true &&
    staticReport.step2_entry_evidence_retained === false &&
    staticReport.unity_execution_evidence === "NOT_RETAINED" &&
    staticReport.production_crash_transport_compiled ===
      !networkTransportCompiledFalse &&
    staticReport.production_crash_transport_authorized === false &&
    staticReport.network_transport_implemented ===
      !diagnosticsNetworkApiAbsent &&
    staticReport.runtime_diagnostics_wiring_implemented === false &&
    staticReport.package_lock_approved === false &&
    staticReport.economy_activation_authorized === false &&
    staticReport.commercial_activation_authorized === false &&
    staticReport.production_android_build_authorized === false &&
    staticReport.project_files === projectFiles.length &&
    staticReport.approved_png_files === currentPngs.length &&
    staticReport.diagnostics_core_asmdef_valid === diagnosticsAsmdefValid &&
    staticReport.minimized_public_surface === minimizedPublicSurface &&
    staticReport.schema_and_validation_pinned ===
      schemaAndValidationPinned &&
    staticReport.closed_reviewed_code_catalog ===
      closedReviewedCodeCatalog &&
    staticReport.caller_supplied_code_strings_accepted ===
      callerSuppliedCodeStringsAccepted &&
    staticReport.public_transport_injection_present ===
      publicTransportInjectionPresent &&
    staticReport.policy_limits_pinned === policyLimitsPinned &&
    staticReport.deterministic_fingerprint_pinned ===
      deterministicFingerprintPinned &&
    staticReport.canonical_serializer_pinned === canonicalSerializerPinned &&
    staticReport.bounded_buffer_pinned === boundedBufferPinned &&
    staticReport.inclusive_dedup_and_hard_retention_pinned ===
      inclusiveDedupAndHardRetentionPinned &&
    staticReport.retry_and_failure_isolation_pinned ===
      retryAndFailureIsolationPinned &&
    staticReport.only_disabled_transport_shipped === onlyDisabledTransportShipped &&
    staticReport.all_inherited_tests_identical_to_v117_step02 === true &&
    staticReport.source_counts_are_execution_evidence === false &&
    sameArray(staticReport.failures, []) &&
    sameArray(staticReport.static_failures, []);
  if (!staticReportValid)
    fail(staticReportPath + " does not match the reviewed source-only facts.");
}

let baselineFiles = null;
let baselineTree = null;
let baselineTreeValid = null;
let baselineVerifierPassed = null;
let actualChangedPaths = null;
let protectedUnionIdentical = null;
let approvedPngsIdentical = null;
let allInheritedTestsIdentical = null;
let noInheritedFilesDeleted = null;
let manifestMatchesBaseline = null;

if (baseline) {
  if (!fs.existsSync(baseline) || !fs.statSync(baseline).isDirectory()) {
    fail("Baseline '" + baseline + "' is not a directory.");
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
      fail("Supplied v1.17 Step 2 baseline does not match the pinned 487-file tree.");

    if (!exists(BASELINE_VERIFIER_PATH, baseline) ||
        sha256File(BASELINE_VERIFIER_PATH, baseline) !== BASELINE_VERIFIER_SHA256) {
      fail("Supplied baseline verifier does not match the pinned v1.17 Step 2 verifier.");
      baselineVerifierPassed = false;
    } else {
      const execution = spawnSync(
        process.execPath,
        [absolute(BASELINE_VERIFIER_PATH, baseline), "--source-only"],
        {
          cwd: baseline,
          encoding: "utf8",
          timeout: 30000,
          maxBuffer: 4 * 1024 * 1024
        }
      );
      try {
        const result = JSON.parse(execution.stdout);
        baselineVerifierPassed =
          !execution.error && execution.signal === null && execution.status === 0 &&
          result.passed === true && result.source_only === true &&
          result.status === "PASS_SOURCE_ONLY_BOUNDARY_VERIFICATION" &&
          result.project_files === BASELINE_FILE_COUNT &&
          result.candidate_tree_sha256 === BASELINE_TREE_SHA256 &&
          result.verifier_sha256 === BASELINE_VERIFIER_SHA256 &&
          result.reviewed_candidate_bytes_exact === true &&
          result.package_lock_approval_blocked === true &&
          result.package_lock_approved === false &&
          result.economy_activation_authorized === false &&
          result.commercial_activation_authorized === false &&
          result.production_android_build_authorized === false &&
          sameArray(result.failures, []) && sameArray(result.static_failures, []);
      } catch {
        baselineVerifierPassed = false;
      }
      if (!baselineVerifierPassed)
        fail("Pinned v1.17 Step 2 baseline source verifier did not pass cleanly.");
    }

    const baselineSet = new Set(baselineFiles);
    const candidateSet = new Set(projectFiles);
    const union = [...new Set([...baselineFiles, ...projectFiles])].sort();
    actualChangedPaths = [];
    protectedUnionIdentical = true;
    approvedPngsIdentical = true;
    allInheritedTestsIdentical = true;
    noInheritedFilesDeleted = true;
    for (const file of union) {
      const inBaseline = baselineSet.has(file);
      const inCandidate = candidateSet.has(file);
      const deleted = inBaseline && !inCandidate;
      const added = !inBaseline && inCandidate;
      const modified = inBaseline && inCandidate &&
        sha256File(file, baseline) !== sha256File(file, root);
      if (!deleted && !added && !modified) continue;
      actualChangedPaths.push(file);

      if (isProtectedPath(file)) protectedUnionIdentical = false;
      if (file.toLowerCase().endsWith(".png")) approvedPngsIdentical = false;
      if (inBaseline && file.includes("/Tests/"))
        allInheritedTestsIdentical = false;
      if (deleted) {
        noInheritedFilesDeleted = false;
        fail("Candidate deleted inherited path '" + file + "'.");
        continue;
      }
      if (!allowedChangedPaths.includes(file))
        fail("Candidate changed non-allowlisted path '" + file + "'.");
    }
    if (!protectedUnionIdentical)
      fail("A protected runtime, save, config, resource, scene, package, activation, commercial, economy, or Android path differs from Step 2.");
    if (!approvedPngsIdentical)
      fail("An approved PNG differs from the v1.17 Step 2 baseline.");
    if (!allInheritedTestsIdentical)
      fail("An inherited test differs from the v1.17 Step 2 baseline.");
    manifestMatchesBaseline = sameArray(actualChangedPaths, changedManifestPaths);
    if (!manifestMatchesBaseline)
      fail("Changed-file manifest does not equal the computed v1.17 Step 2 delta.");
  }
}

const candidateTreeSha256 = treeFingerprint(root, projectFiles);
const verifierSha256 = exists(VERIFIER_PATH) ? sha256File(VERIFIER_PATH) : null;
const passed = failures.length === 0;
const releaseBoundaryVerified = Boolean(
  passed && baseline && baselineTreeValid && baselineVerifierPassed &&
  protectedUnionIdentical && approvedPngsIdentical && allInheritedTestsIdentical &&
  noInheritedFilesDeleted && manifestMatchesBaseline && reviewedCandidateBytesExact
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
  pre_entry_source_candidate: true,
  step2_entry_evidence_retained: false,
  unity_execution_evidence: "NOT_RETAINED",
  unity_compile_status: "NOT_RETAINED",
  project_validator_execution_status: "NOT_RETAINED",
  editor_self_test_execution_status: "NOT_RETAINED",
  editmode_execution_status: "NOT_RETAINED",
  playmode_execution_status: "NOT_RETAINED",
  representative_device_status: "NOT_RETAINED",
  source_project_settings_status: "ABSENT_FROM_SOURCE_CANDIDATE",
  source_packages_lock_status: "ABSENT_UNREVIEWED",
  exact_unity_version: true,
  source_only: sourceOnly,
  release_boundary_verified: releaseBoundaryVerified,
  final_review_pins_ready: finalPinsReady,
  reviewed_candidate_bytes_exact: reviewedCandidateBytesExact,
  reviewed_project_file_count_pin: REVIEWED_PROJECT_FILE_COUNT,
  reviewed_candidate_excluding_verifier_sha256_pin:
    REVIEWED_CANDIDATE_EXCLUDING_VERIFIER_SHA256,
  candidate_excluding_verifier_sha256:
    candidateExcludingVerifierSha256,
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
  package_lock_approval_blocked: packageLockApprovalBlocked,
  package_lock_approved: packageLockApprovalBlocked ? false : null,
  production_android_build_compiled_false: productionAndroidBuildCompiledFalse,
  external_provider_activation_compiled_false:
    externalProviderActivationCompiledFalse,
  production_crash_transport_compiled: !networkTransportCompiledFalse,
  production_crash_transport_authorized: false,
  network_transport_implemented: !diagnosticsNetworkApiAbsent,
  public_transport_injection_present: publicTransportInjectionPresent,
  runtime_diagnostics_wiring_implemented: false,
  economy_activation_authorized: false,
  commercial_activation_authorized: false,
  cloud_recovery_authorized: false,
  production_android_build_authorized: false,
  publishing_authorized: false,
  diagnostics_csharp_files: diagnosticsFiles.length,
  diagnostics_forbidden_dependencies_absent:
    diagnosticsForbiddenDependenciesAbsent,
  minimized_public_surface: minimizedPublicSurface,
  schema_and_validation_pinned: schemaAndValidationPinned,
  closed_reviewed_code_catalog: closedReviewedCodeCatalog,
  caller_supplied_code_strings_accepted:
    callerSuppliedCodeStringsAccepted,
  policy_limits_pinned: policyLimitsPinned,
  deterministic_fingerprint_pinned: deterministicFingerprintPinned,
  canonical_serializer_pinned: canonicalSerializerPinned,
  bounded_buffer_pinned: boundedBufferPinned,
  inclusive_dedup_and_hard_retention_pinned:
    inclusiveDedupAndHardRetentionPinned,
  retry_and_failure_isolation_pinned: retryAndFailureIsolationPinned,
  only_disabled_transport_shipped: onlyDisabledTransportShipped,
  diagnostics_core_asmdef_valid: diagnosticsAsmdefValid,
  diagnostics_editmode_tests_asmdef_valid: editModeTestsAsmdefValid,
  diagnostics_playmode_tests_asmdef_valid: playModeTestsAsmdefValid,
  assembly_graph_acyclic: assemblyGraphAcyclic,
  source_test_attributes_informational: sourceTestAttributes,
  diagnostics_test_attributes_informational: diagnosticsTestAttributes,
  diagnostics_test_policy_valid: diagnosticsTestPolicyValid,
  diagnostics_playmode_pure_and_save_isolated: playModePureAndSaveIsolated,
  editor_self_test_check_sites_informational: selfTestCheckSites,
  editor_self_test_coverage_complete: selfTestCoverageComplete,
  player_prefs_delete_all_used: !playerPrefsDeleteAllAbsent,
  source_counts_are_execution_evidence: false,
  implementation_status_valid: implementationStatusValid,
  activation_evidence_present: exists(activationEvidencePath),
  activation_evidence_valid: activationEvidenceValid,
  checkpoint_valid: checkpointValid,
  next_milestone_valid: nextMilestoneValid,
  static_report_present: exists(staticReportPath),
  static_report_valid: staticReportValid,
  changed_file_manifest_present: exists(changedManifestPath),
  changed_file_manifest_valid: changedManifestValid,
  allowed_changed_paths: allowedChangedPaths,
  baseline_supplied: Boolean(baseline),
  baseline_file_count: baselineFiles?.length || null,
  baseline_tree_sha256: baselineTree,
  baseline_tree_valid: baselineTreeValid,
  baseline_source_verifier_passed: baselineVerifierPassed,
  protected_union_identical: protectedUnionIdentical,
  approved_pngs_identical_to_v117_step02: approvedPngsIdentical,
  all_inherited_tests_identical_to_v117_step02: allInheritedTestsIdentical,
  no_inherited_files_deleted: noInheritedFilesDeleted,
  actual_changed_paths: actualChangedPaths,
  changed_file_manifest_matches_v117_step02: manifestMatchesBaseline,
  failures,
  static_failures: staticFailures
};

console.log(JSON.stringify(report, null, 2));
if (!passed) process.exitCode = 1;
