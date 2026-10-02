# Source pipeline

OriginalReference.png is an unchanged copy of the supplied local image. Assessment.json records source bounds and submitted-input hashes. ScenarioRequests.py records the API workflow; Scenario/ contains four completed source-alpha jobs, billing and downloaded outputs.

InitialPreparation.py is the historical input preparation. Do not rerun it over already-submitted inputs. RefineFaces.py corrects only native working images and repair masks, leaving submitted Inputs/*.png unchanged. PrepareFinal.py consumes those native working images and Scenario alpha, preserves RGB outside recorded repair masks, restores intentionally opaque interiors, writes Final/runtime sprites and builds alpha review sheets. Its source hash assertions prevent silent drift of submitted inputs.

Bounded local text/control removal is the only RGB repair. Original Russian heading is retained; English heading and language labels are rendered live. No additional Scenario RGB generation or paid resubmission was used.
