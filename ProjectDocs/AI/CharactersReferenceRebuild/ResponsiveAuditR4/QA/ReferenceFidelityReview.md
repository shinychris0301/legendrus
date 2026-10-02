# Characters — R4 visual and interaction review

Approved title, portraits, card art, Ivan-only highlight and original Russian button/footer lettering remain unchanged. Header/footer still attach to safe-area edges. Enlarged touch targets do not scale the visible controls.

The deliberate visual difference is the bottom of the collection viewport: it ends 45 reference pixels earlier to reserve a non-overlapping Info touch area. Reference-size screens show less of the future row initially; the row is still fully revealable by upward drag. At the scroll limit cards shift farther upward. Tall screens continue to reveal all rows; spare space is intentional.

Inspected fresh reference-size Russian, narrow English and scrolled future-card captures. Native tests confirm the expanded controls and Info are reachable, masked cards cannot intercept fixed controls, filter/preview changes preserve Ivan as the active hero, and relocalized Info closes correctly.

Compared all 12 matching R3 captures. Six are pixel-identical; one differs by nine pixels at max channel delta one. The remaining five include the shortened/scrolled collection region; tiny raster differences outside that region are recorded in BaselineComparison.json. All artwork/font/meta files are byte-identical to the task-start snapshot.

Remaining differences from the original concept include reconstructed scenery/surfaces, live detail typography and truthful unavailable stats/upgrade/ownership labels. No exact source-font claim. Simulated safe-area and native pointer-handler tests do not replace Android device testing.
