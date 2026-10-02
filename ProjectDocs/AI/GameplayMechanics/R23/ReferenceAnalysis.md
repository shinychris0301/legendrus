# R23 — Reference observations

Reviewed Dream Games’ own animated Rocket and Dynamite examples from the [Royal Kingdom Power-Ups help page](https://dreamgames.helpshift.com/hc/en/6-royal-kingdom-1676903479/faq/177-power-ups/?p=all). These are compact official demonstrations, not a device recording of a complete level. No exact frame-for-frame parity is claimed.

Sources examined locally for analysis only:
- [Rocket demonstration](https://d2duuy9yo5pldo.cloudfront.net/dreamgames/37647125-0672-4e8a-afcf-7a8156a1f7ce.gif):400×711,231frames; inspected creation around frames33–48 and activation around186–192.
- [Dynamite demonstration](https://d2duuy9yo5pldo.cloudfront.net/dreamgames/9ebedd75-5c5f-419e-ae3d-b382a6a4696e.gif):400×711,218frames; inspected formation around108 and activation170–178.

Observed: the Rocket settles into a solid item quickly. Its activation visibly separates moving heads with exhaust between them. Dynamite impacts combine a bright blast with many recognizable fragments from the affected pieces. These observations support a change in emphasis: formation should make the special item easy to recognize, while travel, contacts and physical debris carry activation impact.

Our design inference: R22's long deformed formation and decorative orbiting textures do not solve the more important activation gap. R23 uses a shorter solid reveal, independent crystal projectile heads/exhaust, stronger expanding bomb flame shapes, and gem pieces sampled from the real approved sprites. Timings are our implementation choices, not measurements attributed to Royal Kingdom. Existing gameplay contact times and footprints stay authoritative.

The Royal Kingdom files were downloaded to /tmp for analysis. They are not imported into Assets or shipped in the game. Runtime uses this project's approved artwork only. This pass does not reproduce the competitor’s UI, characters or item designs.

Capture issue found locally: the older art recorder allocated a camera/targets and compressed PNGs on every sample. Its sample intervals could be much longer than the nominal interval, omitting short poses. The new test-only recorder reuses render targets and buffers raw RGB samples, then encodes after playback. Timestamps still determine playback duration; this does not establish Android frame rate or eliminate all capture overhead.
