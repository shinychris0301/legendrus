#!/usr/bin/env node
import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';

const root = path.resolve(process.argv[2] || '.');
const kit = path.join(root, 'Assets/LegendsOfRus/Art/Production/UI/CanonicalKit');
const failures = [];
function check(c,m){ if(!c) failures.push(m); }
function text(rel){ return fs.readFileSync(path.join(root,rel),'utf8'); }
function exists(rel){ return fs.existsSync(path.join(root,rel)); }
function sha(rel){ return crypto.createHash('sha256').update(fs.readFileSync(path.join(root,rel))).digest('hex'); }

check(exists('MILESTONE_VERSION.txt'), 'milestone file missing');
check(text('MILESTONE_VERSION.txt').trim()==='v1.20-step03.2-canonical-ui-kit-reconstruction','milestone mismatch');
const pngs = fs.readdirSync(kit).filter(x=>x.toLowerCase().endsWith('.png')).sort();
check(pngs.length===34, 'expected 34 canonical UI PNGs, got '+pngs.length);
const manifest=JSON.parse(text('Assets/LegendsOfRus/Art/Production/UI/CanonicalKit/canonical_ui_kit_manifest.json'));
check(manifest.assetCount===34,'manifest assetCount mismatch');
check(manifest.textBakedIntoSprites===false,'baked text must remain false');
check(manifest.runtimeScreenshotCutoverAuthorized===false,'runtime cutover must remain false');
for(const a of manifest.assets){
  const rel='Assets/LegendsOfRus/Art/Production/UI/CanonicalKit/'+a.file;
  check(exists(rel),'missing asset '+a.file);
  if(exists(rel)) check(sha(rel)===a.sha256,'sha mismatch '+a.file);
}
const activation=JSON.parse(text('Assets/LegendsOfRus/Documentation/V1_20_STEP03_2_ACTIVATION_BOUNDARY.json'));
for(const k of ['runtimeScreenshotCutoverAuthorized','screenshotDeletionAuthorized','screenRouterMutationAuthorized','lockedArtOverwriteAuthorized','automaticArtGenerationEnabled','productionContentPublishAuthorized','playConsoleActionEnabled'])
  check(activation[k]===false, k+' must be false');
check(activation.spriteCount===34,'activation sprite count mismatch');
check(activation.requiredPrefabCount===21,'activation prefab count mismatch');
const cfg=JSON.parse(text('Assets/LegendsOfRus/Resources/Config/production_ui_kit_v120_step03_2.json'));
check(cfg.screenshotRuntimeCutoverAuthorized===false,'config cutover must be false');
check(cfg.screenshotDeletionAuthorized===false,'config deletion must be false');
check(cfg.requiredPrefabs.length===21,'expected 21 required prefabs');
const designDir=path.join(root,'Assets/LegendsOfRus/Resources/DesignScreens');
check(fs.readdirSync(designDir).filter(x=>x.endsWith('.png')).length===22,'expected 22 retained screenshot placeholders');
const router=text('Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs');
const registry=text('Assets/LegendsOfRus/Scripts/Runtime/UI/DesignScreenRegistry.cs');
check(router.includes('BuildDesignLayer(id, root)'), 'legacy design layer unexpectedly changed');
check(registry.includes('DesignScreens/Home'), 'legacy design registry unexpectedly changed');
const builder=text('Assets/LegendsOfRus/Scripts/Editor/Step20CanonicalUiKitBuilder.cs');
check(builder.includes('PrefabUtility.SaveAsPrefabAsset'),'prefab builder missing');
check(builder.includes('TextureImporterFormat.ASTC_6x6'),'Android ASTC import policy missing');
check(builder.includes('spriteBorder'),'9-slice border configuration missing');
const marker=text('Assets/LegendsOfRus/Scripts/Runtime/UI/CanonicalUiKitMarker.cs');
check(marker.includes('v1.20-step03.2'),'marker style version missing');

const report={version:'v1.20-step03.2',status:failures.length?'FAIL':'PASS_STATIC_SOURCE_VERIFICATION',passed:failures.length===0,canonicalUiPngCount:pngs.length,requiredPrefabCount:cfg.requiredPrefabs.length,runtimeScreenshotPlaceholderCount:fs.readdirSync(designDir).filter(x=>x.endsWith('.png')).length,failures};
console.log(JSON.stringify(report,null,2));
process.exit(failures.length?1:0);
