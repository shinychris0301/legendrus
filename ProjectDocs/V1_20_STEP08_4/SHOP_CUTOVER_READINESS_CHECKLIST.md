# Shop conditional cutover readiness checklist

Step 8.4 does not apply a cutover. The readiness evaluator may report eligibility only when all of the following are true:

- 36/36 Step 8.4 Unity runtime captures exist.
- Shop 941x1672 runtime render passes correlation >= 0.80 and MAE <= 0.10 against the direct approved Shop reference.
- Step 8.4 provider-boundary gate passes with the accepted authority hashes unchanged.
- Shop candidate is screenshot-free, has no Button components, and has no ReferenceOnly/DesignScreens/ProjectDocs dependencies.
- The active production registry still contains exactly 10 accepted core-loop routes and does not include Shop.
- Full Wave 2 cutover remains unauthorized; the other eight Wave 2 pages remain outside this readiness decision.

A future explicit Shop cutover milestone must create a rollback backup before registry/prefab mutation and must be followed by full functional regression plus physical Android acceptance.
