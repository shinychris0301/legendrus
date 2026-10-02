# Gameplay Mechanics R7 — Capture reliability investigation

No manual sessions were found in the default MechanicsPlaytests directory. This pass investigates the render failure left open by R6 rather than treating automated ratings as player feedback. No gameplay balance or artwork change is justified by the available records.

## Verified finding

A debugger run reproduced the segmentation fault while Unity rendered a UI Canvas. The stack includes NVIDIA `libnvidia-glcore.so.595.91.07`, `GfxDeviceGLES::DrawBufferRanges`, `CanvasBatchIntermediateRenderer_Render`, and `CameraScripting::Render`. `mono_pmip` did not identify the fault address as a managed method. This locates the failure in the native OpenGL draw path; it does not establish the precise driver defect or rule out an interaction with Unity. Evidence: [debugger stack](QA/Debug02/stack.log).

Detaching the camera target, removing mesh diagnostics, reusing the camera, rendering through the normal player loop, and forcing a uniform Canvas vertex layout did not provide a reliable fix. **All experimental C# edits were reverted to the task-start files.** No runtime, shader, prefab, font, art, package, graphics-backend or color-space changes are retained.

## Process-only driver experiment

NVIDIA documents that `__GL_WRITE_TEXT_SECTION=0` disables executable-memory optimizations in its OpenGL driver. [NVIDIA environment-variable documentation](https://download.nvidia.com/XFree86/Linux-x86_64/595.71.05/README/openglenvvariables.html).

Applied only to the verification child process, this allowed Storage6, Seal2 and Recording5 to pass with their captures. It did **not** reliably complete the combined suite, or the separate Rounds suite. Disabling the shader disk cache alone also did not fix the failure. No system-wide setting, shell profile, driver installation or user cache was changed/deleted. This is a partial mitigation, not a driver repair or a fully reliable capture solution.

## Run record

- Verify01: target detachment/diagnostic markers; native SIGSEGV.
- Verify02: mesh diagnostic isolation; native SIGSEGV.
- Verify03: persistent capture camera; native SIGSEGV.
- Verify04: normal-frame capture; native SIGSEGV.
- Debug01: debugger stopped on Mono's realtime signal37; not a gameplay failure result.
- Debug02: debugger ignored that signal and captured the native OpenGL stack.
- Verify05: original C# with shader disk cache disabled; native SIGSEGV.
- Verify06: original C# with executable-memory optimization disabled; Storage6 passed together.
- Verify07: all18 attempted together with that driver option; native SIGSEGV at Seal capture, no result XML. Earlier captures do not constitute a passing suite.
- Verify08: uniform vertex-layout experiment; native SIGSEGV. Reverted.
- Verify09: original C#, separate Seal process with driver option; Seal2 passed.
- Verify10: separate processes with the driver option; Recording5 passed, Rounds crashed before result XML. Mechanics/Spark stages were not reached.

R6's earlier18 behavioral passes remain historical. These intermediate OpenGL results covered13 tests. They are separate from the final18-test Vulkan acceptance below. No physical Android/IL2CPP test or human playtest was performed.

## Authorized verification

The user explicitly approved “Allow Vulkan for verification only.” Verify11 runs all18 checks in one Unity process with `-force-vulkan`, preserving project settings. The runner has an explicit `--vulkan-verification` option; no persistent Editor configuration is changed. **Verify11 passed18/18 PlayMode tests, zero failures/skips, in one process with screenshots and mesh assertions enabled.** The Unity log confirms Vulkan initialization; the command record shows no NVIDIA environment override for this final run.

The final review uses only captures from the successful Verify11 run. Its groups cover feedback, all five round starts/results, bomb/gem impacts, Spark flight/creation and seal layers. Earlier incomplete runs are kept as diagnostic evidence, not acceptance evidence.

Reproduce the approved verification from the project root:

```sh
python3 ProjectDocs/AI/GameplayMechanics/R7/Sources/Verify.py ANewRun All --vulkan-verification
```

The run ID must be new to avoid overwriting evidence. `--nvidia-execmem-workaround` remains an explicitly named diagnostic option, not a reliable fix or the recommended verification path. No C# workaround was retained; all gameplay and test sources are byte-for-byte task-start versions.

See QA/Scope.json for the final project hash audit and QA/BrowserReview.json for capture-viewer checks. Manual sessions are still needed for difficulty, round-design and effect-readability decisions.

## Final artifact checks

The review contains 64 actual 576×1280 Unity captures, all from Verify11. Browser checks passed for all 128 capture/viewport combinations at widths 430 and 1440, plus navigation, native-pixel toggle, local links and zero JavaScript errors. Selected feedback, bomb, Spark-flight and cracked-seal captures were visually inspected. These are automated fixtures, not human ratings or frame-rate measurements.

The final hash audit reports 7,311 unchanged files and zero changed, added or deleted paths across Assets, Packages and ProjectSettings. Restored task-start Addressables link.xml and metadata; removed 18 generated test-scene/metadata paths absent from the initial inventory. Only R7 review/report/scripts/evidence and the task/session documentation remain as this pass's deliverables.

Next: collect manual attempts on the five sandbox rounds without solution spoilers, rate difficulty and goal/effect clarity, then review the saved session JSONs before balancing or expanding campaign content. Default OpenGL reliability and physical-device verification remain open.
