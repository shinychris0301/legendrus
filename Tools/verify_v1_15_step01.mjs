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
    "A v1.14 Step 4 baseline is required for release verification. " +
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

function exists(relative, directory = root) {
  return fs.existsSync(path.join(directory, relative));
}

function sha256(file) {
  return crypto.createHash("sha256").update(fs.readFileSync(file)).digest("hex");
}

function parseJson(relative, directory = root) {
  try {
    return JSON.parse(read(relative, directory));
  } catch (error) {
    fail(`JSON ${relative}: ${error.message}`);
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

function expectArray(value, pointer) {
  if (!Array.isArray(value)) {
    fail(`${pointer} must be an array.`);
    return [];
  }
  return value;
}

function safeNonnegativeInteger(value, pointer) {
  if (!Number.isSafeInteger(value) || value < 0) {
    fail(`${pointer} must be a nonnegative safe integer.`);
    return false;
  }
  return true;
}

function safePositiveInteger(value, pointer) {
  if (!Number.isSafeInteger(value) || value <= 0) {
    fail(`${pointer} must be a positive safe integer.`);
    return false;
  }
  return true;
}

function uniqueNonemptyIds(items, pointer) {
  const result = new Set();
  for (let index = 0; index < items.length; index++) {
    const id = items[index]?.id;
    if (typeof id !== "string" || id.length === 0) {
      fail(`${pointer}[${index}].id must be a nonempty string.`);
      continue;
    }
    if (result.has(id)) fail(`${pointer} contains duplicate id '${id}'.`);
    result.add(id);
  }
  return result;
}

function validateAmount(value, pointer, allowedIds, allowEmptyId = false) {
  exactKeys(value, ["id", "amount"], pointer);
  const id = value?.id;
  if (typeof id !== "string" || (!allowEmptyId && id.length === 0))
    fail(`${pointer}.id must be ${allowEmptyId ? "a string" : "a nonempty string"}.`);
  if (id && allowedIds && !allowedIds.has(id))
    fail(`${pointer}.id '${id}' does not reference a configured id.`);
  safeNonnegativeInteger(value?.amount, `${pointer}.amount`);
}

function checkedLongTotal(values, pointer) {
  const longMax = 9223372036854775807n;
  let total = 0n;
  for (const value of values) {
    if (!Number.isSafeInteger(value) || value < 0) {
      fail(`${pointer} contains an unsafe or negative value.`);
      continue;
    }
    total += BigInt(value);
    if (total > longMax) fail(`${pointer} exceeds signed Int64 capacity.`);
  }
  return total;
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

const economyPath = "Assets/LegendsOfRus/Resources/Config/economy.json";
const levelsPath = "Assets/LegendsOfRus/Resources/Config/chapter01_levels.json";
const changedManifestPath =
  "Assets/LegendsOfRus/Documentation/V1_15_STEP01_CHANGED_FILES.txt";
const staticReportPath =
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_15_STEP01.json";

const required = [
  "README_V1_15_STEP01.md",
  "MILESTONE_VERSION.txt",
  economyPath,
  levelsPath,
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_15_STEP01.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_15_STEP02.json",
  "Assets/LegendsOfRus/Documentation/V1_15_STEP01_ECONOMY_CONFIGURATION_SIMULATION_FOUNDATION.md",
  changedManifestPath,
  staticReportPath,
  "Assets/LegendsOfRus/Scripts/EconomyCore/EconomyCatalog.cs",
  "Assets/LegendsOfRus/Scripts/EconomyCore/EconomyCatalogValidator.cs",
  "Assets/LegendsOfRus/Scripts/EconomyCore/EconomyClockMath.cs",
  "Assets/LegendsOfRus/Scripts/EconomyCore/EconomyDeterministicJson.cs",
  "Assets/LegendsOfRus/Scripts/EconomyCore/EconomyLedger.cs",
  "Assets/LegendsOfRus/Scripts/EconomyCore/EconomyReports.cs",
  "Assets/LegendsOfRus/Scripts/EconomyCore/EconomySimulator.cs",
  "Assets/LegendsOfRus/Scripts/EconomyCore/LegendsOfRus.EconomyCore.asmdef",
  "Assets/LegendsOfRus/Scripts/Runtime/Economy/EconomyCatalogLoader.cs",
  "Tools/verify_v1_15_step01.mjs"
];

for (const file of required)
  if (!exists(file)) fail(`Required final artifact ${file} is missing.`);

const jsonFiles = files("*.json");
const parsedJson = new Map();
for (const file of jsonFiles) parsedJson.set(file, parseJson(file));

const economy = parsedJson.get(economyPath);
const levels = parsedJson.get(levelsPath);

if (economy) {
  exactKeys(economy, [
    "schemaVersion", "catalogId", "proposalOnly", "authoritative",
    "currencies", "startingBalances", "energy", "boosters", "continueRule",
    "dailyReward", "rewardDoubling", "progression"
  ], "economy");
  if (economy.schemaVersion !== 1) fail("economy.schemaVersion must equal 1.");
  if (economy.catalogId !== "v1.15-step01-proposal")
    fail("economy.catalogId must equal 'v1.15-step01-proposal'.");
  if (economy.proposalOnly !== true) fail("economy.proposalOnly must be true.");
  if (economy.authoritative !== false) fail("economy.authoritative must be false.");

  const currencies = expectArray(economy.currencies, "economy.currencies");
  for (let index = 0; index < currencies.length; index++)
    exactKeys(currencies[index], ["id"], `economy.currencies[${index}]`);
  const currencyIds = uniqueNonemptyIds(currencies, "economy.currencies");
  if (currencyIds.size !== 2 || !currencyIds.has("gold") || !currencyIds.has("gems"))
    fail("economy.currencies must contain exactly 'gold' and 'gems'.");

  const startingBalances = expectArray(economy.startingBalances, "economy.startingBalances");
  for (let index = 0; index < startingBalances.length; index++)
    validateAmount(startingBalances[index], `economy.startingBalances[${index}]`, currencyIds);
  const startingIds = uniqueNonemptyIds(startingBalances, "economy.startingBalances");
  if (startingIds.size !== currencyIds.size || [...currencyIds].some((id) => !startingIds.has(id)))
    fail("economy.startingBalances must define every configured currency exactly once.");

  exactKeys(economy.energy, [
    "capacity", "startingAmount", "levelEntryCost", "retryCost", "refillIntervalMinutes"
  ], "economy.energy");
  safePositiveInteger(economy.energy?.capacity, "economy.energy.capacity");
  safeNonnegativeInteger(economy.energy?.startingAmount, "economy.energy.startingAmount");
  safePositiveInteger(economy.energy?.levelEntryCost, "economy.energy.levelEntryCost");
  safeNonnegativeInteger(economy.energy?.retryCost, "economy.energy.retryCost");
  safePositiveInteger(economy.energy?.refillIntervalMinutes, "economy.energy.refillIntervalMinutes");
  if (economy.energy?.startingAmount > economy.energy?.capacity)
    fail("economy.energy.startingAmount cannot exceed capacity.");

  const boosters = expectArray(economy.boosters, "economy.boosters");
  for (let index = 0; index < boosters.length; index++) {
    const booster = boosters[index];
    exactKeys(booster, [
      "id", "startingQuantity", "purchaseEnabled", "priceDefined", "price"
    ], `economy.boosters[${index}]`);
    safeNonnegativeInteger(booster?.startingQuantity, `economy.boosters[${index}].startingQuantity`);
    if (typeof booster?.purchaseEnabled !== "boolean" || typeof booster?.priceDefined !== "boolean")
      fail(`economy.boosters[${index}] purchase flags must be boolean.`);
    validateAmount(
      booster?.price,
      `economy.boosters[${index}].price`,
      currencyIds,
      booster?.priceDefined === false
    );
    if (booster?.purchaseEnabled && !booster?.priceDefined)
      fail(`economy.boosters[${index}] cannot enable purchase without a price.`);
  }
  const boosterIds = uniqueNonemptyIds(boosters, "economy.boosters");
  const requiredBoosters = ["bogatyr_mace", "firebird", "living_light", "sea_pearl"];
  if (boosterIds.size !== requiredBoosters.length || requiredBoosters.some((id) => !boosterIds.has(id)))
    fail(`economy.boosters must contain exactly ${requiredBoosters.join(", ")}.`);

  exactKeys(economy.continueRule, [
    "enabled", "priceDefined", "price", "grantedMoves", "maxPerAttempt", "freeExitAndRetry"
  ], "economy.continueRule");
  validateAmount(economy.continueRule?.price, "economy.continueRule.price", currencyIds);
  safePositiveInteger(economy.continueRule?.grantedMoves, "economy.continueRule.grantedMoves");
  safeNonnegativeInteger(economy.continueRule?.maxPerAttempt, "economy.continueRule.maxPerAttempt");
  for (const key of ["enabled", "priceDefined", "freeExitAndRetry"])
    if (typeof economy.continueRule?.[key] !== "boolean")
      fail(`economy.continueRule.${key} must be boolean.`);

  exactKeys(economy.dailyReward, ["enabled", "cadenceUtcDays", "grants", "energyGrant"], "economy.dailyReward");
  if (typeof economy.dailyReward?.enabled !== "boolean")
    fail("economy.dailyReward.enabled must be boolean.");
  safePositiveInteger(economy.dailyReward?.cadenceUtcDays, "economy.dailyReward.cadenceUtcDays");
  safeNonnegativeInteger(economy.dailyReward?.energyGrant, "economy.dailyReward.energyGrant");
  const dailyGrants = expectArray(economy.dailyReward?.grants, "economy.dailyReward.grants");
  for (let index = 0; index < dailyGrants.length; index++)
    validateAmount(dailyGrants[index], `economy.dailyReward.grants[${index}]`, currencyIds);
  uniqueNonemptyIds(dailyGrants, "economy.dailyReward.grants");

  exactKeys(economy.rewardDoubling, [
    "enabled", "eligibleCurrencyId", "multiplier", "maxClaimsPerReward", "optional"
  ], "economy.rewardDoubling");
  if (!currencyIds.has(economy.rewardDoubling?.eligibleCurrencyId))
    fail("economy.rewardDoubling.eligibleCurrencyId must reference a currency.");
  safePositiveInteger(economy.rewardDoubling?.multiplier, "economy.rewardDoubling.multiplier");
  safePositiveInteger(economy.rewardDoubling?.maxClaimsPerReward, "economy.rewardDoubling.maxClaimsPerReward");
  for (const key of ["enabled", "optional"])
    if (typeof economy.rewardDoubling?.[key] !== "boolean")
      fail(`economy.rewardDoubling.${key} must be boolean.`);

  exactKeys(economy.progression, ["chapterId", "normalProgressRequiresPurchase", "levels"], "economy.progression");
  if (economy.progression?.chapterId !== 1) fail("economy.progression.chapterId must equal 1.");
  if (economy.progression?.normalProgressRequiresPurchase !== false)
    fail("economy.progression.normalProgressRequiresPurchase must be false.");
  const progressionLevels = expectArray(economy.progression?.levels, "economy.progression.levels");
  for (let index = 0; index < progressionLevels.length; index++) {
    const level = progressionLevels[index];
    const pointer = `economy.progression.levels[${index}]`;
    exactKeys(level, [
      "id", "levelNumber", "requiresStore", "requiresAd", "requiresPurchase",
      "requiredBoosterId", "requiredBoosterQuantity", "firstClearGrants",
      "firstClearBoosterGrants"
    ], pointer);
    if (level?.levelNumber !== index + 1)
      fail(`${pointer}.levelNumber must equal ${index + 1}.`);
    if (level?.id !== `chapter01.level${String(index + 1).padStart(2, "0")}`)
      fail(`${pointer}.id does not match its Chapter 1 level number.`);
    for (const key of ["requiresStore", "requiresAd", "requiresPurchase"])
      if (level?.[key] !== false) fail(`${pointer}.${key} must be false.`);
    safeNonnegativeInteger(level?.requiredBoosterQuantity, `${pointer}.requiredBoosterQuantity`);
    if (level?.requiredBoosterQuantity > 0 && !boosterIds.has(level?.requiredBoosterId))
      fail(`${pointer}.requiredBoosterId does not reference a configured booster.`);
    if (level?.requiredBoosterQuantity === 0 && level?.requiredBoosterId !== "")
      fail(`${pointer}.requiredBoosterId must be empty when quantity is zero.`);
    const grants = expectArray(level?.firstClearGrants, `${pointer}.firstClearGrants`);
    for (let grant = 0; grant < grants.length; grant++)
      validateAmount(grants[grant], `${pointer}.firstClearGrants[${grant}]`, currencyIds);
    uniqueNonemptyIds(grants, `${pointer}.firstClearGrants`);
    const boosterGrants = expectArray(level?.firstClearBoosterGrants, `${pointer}.firstClearBoosterGrants`);
    for (let grant = 0; grant < boosterGrants.length; grant++)
      validateAmount(boosterGrants[grant], `${pointer}.firstClearBoosterGrants[${grant}]`, boosterIds);
    uniqueNonemptyIds(boosterGrants, `${pointer}.firstClearBoosterGrants`);
  }
  uniqueNonemptyIds(progressionLevels, "economy.progression.levels");
  if (progressionLevels.length !== 10)
    fail(`economy.progression.levels must contain exactly 10 levels, found ${progressionLevels.length}.`);

  checkedLongTotal([
    ...startingBalances.map((item) => item?.amount),
    ...dailyGrants.map((item) => item?.amount),
    ...progressionLevels.flatMap((level) => level?.firstClearGrants?.map((item) => item?.amount) ?? []),
    ...boosters.map((item) => item?.startingQuantity),
    ...progressionLevels.flatMap((level) => level?.firstClearBoosterGrants?.map((item) => item?.amount) ?? [])
  ], "economy aggregate totals");

  if (levels?.levels && progressionLevels.length === levels.levels.length) {
    for (let index = 0; index < levels.levels.length; index++) {
      const authored = levels.levels[index];
      const proposed = progressionLevels[index];
      const currencyMap = new Map((proposed.firstClearGrants ?? []).map((item) => [item.id, item.amount]));
      const boosterMap = new Map((proposed.firstClearBoosterGrants ?? []).map((item) => [item.id, item.amount]));
      const expected = {
        gold: authored.rewards?.gold,
        gems: authored.rewards?.gems,
        bogatyr_mace: authored.rewards?.boosterMace,
        firebird: authored.rewards?.boosterFirebird,
        living_light: authored.rewards?.boosterLivingLight,
        sea_pearl: authored.rewards?.boosterSeaPearl
      };
      for (const id of ["gold", "gems"])
        if ((currencyMap.get(id) ?? 0) !== expected[id])
          fail(`Economy first-clear ${id} differs from authored reward for ${authored.id}.`);
      for (const id of requiredBoosters)
        if ((boosterMap.get(id) ?? 0) !== expected[id])
          fail(`Economy first-clear ${id} differs from authored reward for ${authored.id}.`);
    }
  }

  const goldTotal = progressionLevels.reduce((sum, level) =>
    sum + (level?.firstClearGrants?.find((item) => item.id === "gold")?.amount ?? 0), 0);
  const gemTotal = progressionLevels.reduce((sum, level) =>
    sum + (level?.firstClearGrants?.find((item) => item.id === "gems")?.amount ?? 0), 0);
  if (goldTotal !== 1850 || gemTotal !== 40)
    fail(`Chapter 1 reward totals must be gold=1850/gems=40, found ${goldTotal}/${gemTotal}.`);
}

if (levels) {
  if (levels.schemaVersion !== 3 || !Array.isArray(levels.levels) || levels.levels.length !== 10)
    fail("Chapter 1 authored level schema/count changed.");
  const rewardFields = ["gold", "gems", "boosterMace", "boosterFirebird", "boosterLivingLight", "boosterSeaPearl"];
  for (let index = 0; index < (levels.levels?.length ?? 0); index++) {
    const reward = levels.levels[index]?.rewards;
    for (const field of rewardFields)
      safeNonnegativeInteger(reward?.[field], `chapter01_levels.levels[${index}].rewards.${field}`);
  }
  const sum = (field) => levels.levels.reduce((total, level) => total + level.rewards[field], 0);
  const totals = rewardFields.map((field) => sum(field));
  const expected = [1850, 40, 2, 2, 2, 2];
  if (!expected.every((value, index) => totals[index] === value))
    fail(`Authored Chapter 1 rewards changed: [${totals.join(", ")}].`);
  checkedLongTotal(totals, "Chapter 1 authored reward totals");
}

const gameStatePath = "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs";
const saveServicePath = "Assets/LegendsOfRus/Scripts/Runtime/Services/SaveService.cs";
if (!exists(gameStatePath) || !/public\s+int\s+saveVersion\s*=\s*6\s*;/.test(read(gameStatePath)))
  fail("PlayerSaveData.saveVersion must remain 6.");
if (!exists(saveServicePath) || !/const\s+string\s+Key\s*=\s*"LegendsOfRus_Save_v1"\s*;/.test(read(saveServicePath)))
  fail("SaveService key must remain 'LegendsOfRus_Save_v1'.");

const economyCoreAsmdefPath =
  "Assets/LegendsOfRus/Scripts/EconomyCore/LegendsOfRus.EconomyCore.asmdef";
const economyCoreAsmdef = exists(economyCoreAsmdefPath) ? parseJson(economyCoreAsmdefPath) : null;
if (economyCoreAsmdef) {
  if (economyCoreAsmdef.name !== "LegendsOfRus.EconomyCore")
    fail("EconomyCore asmdef name changed.");
  if (!Array.isArray(economyCoreAsmdef.references) || economyCoreAsmdef.references.length !== 0)
    fail("EconomyCore asmdef must have zero assembly references.");
  if (economyCoreAsmdef.noEngineReferences !== true)
    fail("EconomyCore asmdef must set noEngineReferences=true.");
}

const economyCoreFiles = files("Assets/LegendsOfRus/Scripts/EconomyCore/*.cs");
const forbiddenEconomyCore = /\b(?:UnityEngine|UnityEditor|MonoBehaviour|ScriptableObject|PlayerPrefs|Resources|GameState|PlayerSaveData|SaveService|ScreenRouter|MonetizationService|AnalyticsService|DateTime\.(?:Now|UtcNow)|UnityEngine\.Random|System\.Random|File|Directory|HttpClient|WebRequest)\b/;
for (const file of economyCoreFiles) {
  const match = scrubCSharp(read(file)).match(forbiddenEconomyCore);
  if (match) fail(`${file} contains forbidden EconomyCore dependency '${match[0]}'.`);
}
const adapterFiles = files("Assets/LegendsOfRus/Scripts/Runtime/Economy/*.cs");
const forbiddenMutation = /\b(?:PlayerPrefs|GameState|PlayerSaveData|SaveService|SpendEnergy|SpendGold|SpendGems|AddEnergy|AddGold|AddGems|TryUseBooster|AddBooster|CompleteLevel|ClaimDailyReward|MockPurchase)\b/;
for (const file of adapterFiles) {
  const match = scrubCSharp(read(file)).match(forbiddenMutation);
  if (match) fail(`${file} violates proposal-only non-mutation boundary with '${match[0]}'.`);
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
  if (preprocessorDepth !== 0) fail(`${file} has unmatched #if/#endif directives.`);
}

function isProtected(relative) {
  if (relative.endsWith(".png")) return true;
  if (relative.startsWith("Assets/LegendsOfRus/Scripts/GameplayCore/")) return true;
  if (relative.startsWith("Assets/LegendsOfRus/Scripts/Runtime/")) {
    if (relative === "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef") return false;
    if (relative.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Economy/")) return false;
    return true;
  }
  if (relative.startsWith("Assets/LegendsOfRus/Resources/")) return relative !== economyPath;
  if (relative.startsWith("Assets/LegendsOfRus/Data/")) return true;
  if (relative.startsWith("Assets/Scenes/")) return true;
  if (relative.startsWith("ProjectSettings/")) return true;
  if (relative.startsWith("Packages/")) return true;
  if (relative.startsWith("Assets/LegendsOfRus/Documentation/") &&
      path.basename(relative).includes("V1_14_STEP04")) return true;
  if (relative === "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_15_STEP01.json") return true;
  return false;
}

let pngFilesCompared = 0;
let pngFilesIdentical = null;
let protectedFilesCompared = 0;
let protectedFilesIdentical = null;
let changedManifestMatchesBaseline = null;
let actualChangedFiles = [];

const manifestLines = exists(changedManifestPath)
  ? read(changedManifestPath).split(/\r?\n/).map((line) => line.trim()).filter((line) =>
      line && !/^(?:Added|Modified|Changed files|Files)$/i.test(line)
    ).sort()
  : [];

if (baseline) {
  if (!fs.existsSync(baseline)) {
    fail(`Baseline directory does not exist: ${baseline}`);
  } else {
    const currentFiles = projectFilesAt(root);
    const baselineFiles = projectFilesAt(baseline);
    const currentPngs = currentFiles.filter((file) => file.endsWith(".png"));
    const baselinePngs = baselineFiles.filter((file) => file.endsWith(".png"));
    pngFilesCompared = currentPngs.length;
    pngFilesIdentical = currentPngs.length === 65 && baselinePngs.length === 65 &&
      currentPngs.length === baselinePngs.length &&
      currentPngs.every((file, index) => file === baselinePngs[index]);
    if (currentPngs.length !== 65 || baselinePngs.length !== 65)
      fail(`Expected 65 approved PNGs in current and baseline, found ${currentPngs.length}/${baselinePngs.length}.`);
    if (!pngFilesIdentical) fail("PNG path list differs from the v1.14 Step 4 baseline.");
    for (const file of currentPngs) {
      if (!exists(file, baseline) || sha256(path.join(root, file)) !== sha256(path.join(baseline, file))) {
        pngFilesIdentical = false;
        fail(`Approved PNG differs from v1.14 Step 4: ${file}`);
      }
    }

    const currentProtected = currentFiles.filter(isProtected);
    const baselineProtected = baselineFiles.filter(isProtected);
    protectedFilesCompared = currentProtected.length;
    protectedFilesIdentical = currentProtected.length === baselineProtected.length &&
      currentProtected.every((file, index) => file === baselineProtected[index]);
    if (!protectedFilesIdentical)
      fail("Protected runtime/gameplay/data/settings/docs/scenes/packages path list differs from v1.14 Step 4.");
    for (const file of baselineProtected) {
      if (!exists(file) || sha256(path.join(root, file)) !== sha256(path.join(baseline, file))) {
        protectedFilesIdentical = false;
        fail(`Protected baseline file differs from v1.14 Step 4: ${file}`);
      }
    }

    const union = new Set([...currentFiles, ...baselineFiles]);
    actualChangedFiles = [...union].filter((file) => {
      const current = path.join(root, file);
      const original = path.join(baseline, file);
      if (!fs.existsSync(current) || !fs.existsSync(original)) return true;
      return sha256(current) !== sha256(original);
    }).sort();
    changedManifestMatchesBaseline = manifestLines.length === actualChangedFiles.length &&
      actualChangedFiles.every((file, index) => file === manifestLines[index]);
    if (!changedManifestMatchesBaseline) {
      const missing = actualChangedFiles.filter((file) => !manifestLines.includes(file));
      const extra = manifestLines.filter((file) => !actualChangedFiles.includes(file));
      fail(`Changed-file manifest mismatch; missing=[${missing.join(",")}], extra=[${extra.join(",")}].`);
    }
  }
}

function amountFrom(items, id) {
  return Array.isArray(items) ? items.find((item) => item?.id === id)?.amount : undefined;
}

function validateSimulationReport(file, report) {
  if (!isPlainObject(report)) {
    fail(`${file} must contain a simulation report object.`);
    return;
  }
  let simulation = report;
  if (Object.hasOwn(report, "simulation")) {
    if (report.schemaVersion !== 1 || report.reportId !== "v1.15-step01-economy-simulation")
      fail(`${file} coordinator schemaVersion/reportId is invalid.`);
    if (report.nonAuthoritative !== true || report.authority !== "proposal-comparison-only")
      fail(`${file} must be explicitly non-authoritative proposal comparison evidence.`);
    if (report.load?.succeeded !== true || report.load?.errorCode !== "" || report.load?.error !== "")
      fail(`${file} records a failed or degraded catalog load.`);
    const fixed = report.fixedInputs;
    if (fixed?.startTick !== 0 || fixed?.ticksPerMinute !== 1 ||
        fixed?.allowStore !== false || fixed?.allowAds !== false ||
        fixed?.allowPurchases !== false || fixed?.optionalPaidContinuesPerLevel !== 0)
      fail(`${file} fixed purchase-free inputs changed.`);
    if (!isPlainObject(report.prototypeComparison) || report.prototypeComparison.verdict === "Fail")
      fail(`${file} prototype comparison is missing or failed.`);
    simulation = report.simulation;
  }
  if (!isPlainObject(simulation)) {
    fail(`${file} has no simulation payload.`);
    return;
  }
  if (simulation.catalogId !== "v1.15-step01-proposal") fail(`${file} has the wrong catalogId.`);
  if (!["chapter01-purchase-free-baseline", "purchase-free-chapter01"].includes(simulation.scenarioId))
    fail(`${file} has the wrong scenarioId.`);
  if (simulation.verdict === "Fail" || !["Pass", "Incomplete"].includes(simulation.verdict))
    fail(`${file} simulation verdict must be Pass or non-failing Incomplete.`);
  if (simulation.validation?.verdict !== "Pass") fail(`${file} catalog validation verdict must be Pass.`);
  if (simulation.elapsedMinutes !== 150 || simulation.endingEnergy !== 0)
    fail(`${file} must end after 150 minutes with zero energy.`);
  if (amountFrom(simulation.endingWallet?.balances, "gold") !== 14390 ||
      amountFrom(simulation.endingWallet?.balances, "gems") !== 390)
    fail(`${file} ending wallet must be gold=14390 and gems=390.`);
  const expectedBoosters = new Map([
    ["bogatyr_mace", 9], ["firebird", 7], ["living_light", 10], ["sea_pearl", 12]
  ]);
  for (const [id, amount] of expectedBoosters)
    if (amountFrom(simulation.endingBoosters, id) !== amount)
      fail(`${file} ending booster ${id} must equal ${amount}.`);
  const metrics = new Map((simulation.metrics ?? []).map((item) => [item.id, item.value]));
  for (const [id, value] of [["levels-completed", 10], ["elapsed-wait", 150], ["ending-energy", 0], ["paid-continues-applied", 0]])
    if (metrics.get(id) !== value) fail(`${file} metric ${id} must equal ${value}.`);
  const checks = new Map((simulation.checks ?? []).map((item) => [item.id, item.verdict]));
  if (checks.get("purchase-free-progression") !== "Pass")
    fail(`${file} purchase-free progression check must pass.`);
}

const simulationReportFiles = jsonFiles.filter((file) =>
  /economy.*simulation|simulation.*economy/i.test(path.basename(file)) && file !== staticReportPath
);
for (const file of simulationReportFiles)
  validateSimulationReport(file, parsedJson.get(file));

if (exists(staticReportPath)) {
  const staticReport = parsedJson.get(staticReportPath);
  if (staticReport && staticReport.version !== "v1.15-step01")
    fail(`${staticReportPath} version must equal 'v1.15-step01'.`);
  if (staticReport && Array.isArray(staticReport.failures) && staticReport.failures.length > 0)
    fail(`${staticReportPath} records ${staticReport.failures.length} unresolved failure(s).`);
}

const testPattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
let discoveredTests = 0;
for (const file of csharpFiles.filter((file) => file.includes("/Tests/")))
  discoveredTests += read(file).match(testPattern)?.length ?? 0;
const economyTestFiles = csharpFiles.filter((file) =>
  file.includes("/Tests/") && /Economy/i.test(path.basename(file))
);
let economyTargetedTests = 0;
for (const file of economyTestFiles)
  economyTargetedTests += read(file).match(testPattern)?.length ?? 0;
if (economyTargetedTests === 0)
  fail("No v1.15 Step 1 Economy EditMode/PlayMode tests were discovered.");
const economySelfTestFiles = csharpFiles.filter((file) =>
  file.includes("/Scripts/Editor/") && /Economy.*SelfTest/i.test(path.basename(file))
);
let economyEditorChecks = 0;
for (const file of economySelfTestFiles)
  economyEditorChecks += Math.max(0, (read(file).match(/\bCheck\s*\(/g)?.length ?? 0) - 1);
if (economyEditorChecks === 0)
  fail("No v1.15 Step 1 Economy Editor self-test check sites were discovered.");

const result = {
  version: "v1.15-step01",
  all_project_json_parses: !failures.some((failure) => failure.startsWith("JSON ")),
  project_json_files: jsonFiles.length,
  exact_economy_schema_and_unknown_key_gate: !failures.some((failure) => failure.startsWith("economy." ) || failure.startsWith("economy ")),
  proposal_only: economy?.proposalOnly === true,
  authoritative: economy?.authoritative ?? null,
  configured_currencies: economy?.currencies?.length ?? 0,
  configured_boosters: economy?.boosters?.length ?? 0,
  configured_chapter01_levels: economy?.progression?.levels?.length ?? 0,
  authored_chapter01_reward_totals: {
    gold: 1850,
    gems: 40,
    bogatyr_mace: 2,
    firebird: 2,
    living_light: 2,
    sea_pearl: 2
  },
  purchase_free_chapter01_expected: {
    elapsed_minutes: 150,
    ending_gold: 14390,
    ending_gems: 390,
    ending_energy: 0,
    ending_boosters: {
      bogatyr_mace: 9,
      firebird: 7,
      living_light: 10,
      sea_pearl: 12
    },
    store_uses: 0,
    ad_uses: 0,
    purchases: 0
  },
  simulation_reports_validated: simulationReportFiles.length,
  save_version_unchanged: exists(gameStatePath) && /public\s+int\s+saveVersion\s*=\s*6\s*;/.test(read(gameStatePath)),
  save_key_unchanged: exists(saveServicePath) && /"LegendsOfRus_Save_v1"/.test(read(saveServicePath)),
  economy_core_source_files: economyCoreFiles.length,
  economy_core_forbidden_dependencies_absent: !failures.some((failure) => failure.includes("forbidden EconomyCore dependency")),
  proposal_runtime_adapter_non_mutating: !failures.some((failure) => failure.includes("proposal-only non-mutation")),
  csharp_structural_scan_files: csharpFiles.length,
  csharp_delimiters_balanced: !failures.some((failure) => failure.includes("delimiters")),
  preprocessor_directives_balanced: !failures.some((failure) => failure.includes("#if/#endif")),
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_step01_economy_targeted_tests: economyTargetedTests,
  step01_economy_editor_self_test_check_sites: economyEditorChecks,
  project_png_files_compared: baseline ? pngFilesCompared : 0,
  project_png_files_byte_identical_to_v1_14_step04: baseline ? pngFilesIdentical : null,
  protected_boundary_files_compared: baseline ? protectedFilesCompared : 0,
  protected_boundary_files_byte_identical_to_v1_14_step04: baseline ? protectedFilesIdentical : null,
  changed_file_manifest_paths: manifestLines.length,
  actual_changed_paths: baseline ? actualChangedFiles.length : null,
  changed_file_manifest_matches_v1_14_step04_baseline: changedManifestMatchesBaseline,
  required_final_artifacts_present: required.every((file) => exists(file)),
  unity_execution: "PENDING_UNITY_EDITOR",
  failures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (failures.length > 0) process.exitCode = 1;
