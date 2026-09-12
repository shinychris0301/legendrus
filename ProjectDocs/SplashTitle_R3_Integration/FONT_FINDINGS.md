# Font investigation — unresolved source identity

## Evidence from the supplied material

The reference is `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/SplashTitle.png` (941×1672). Its PNG metadata contains no typeface family or originating font. The project contains no TTF/OTF/TTC/WOFF font binaries. `Art/Fonts/Production/README.md` describes an intentionally empty font slot and a development fallback.

The logo is authored lettering in the generated `legends_of_rus_ornate_crest.png`; using that file preserves its supplied lettering without guessing a typeface. The regenerated logo itself differs somewhat from the earlier screenshot, so this does not establish pixel identity with the older reference.

The other reference labels show serif letterforms. `Typography_Comparison_Unverified.jpg` compares their appearance with local Liberation Serif, EB Garamond and FreeSerif renderings. These are comparisons, not proven identifications. Times New Roman is an additional OS fallback candidate in the development resolver, not an assertion that it created the reference.

**Result: the exact original live-text font has not been identified or supplied. No substitute has been marked exact.**

## Runtime handling

The config has explicit RegularFont/StartFont fields. The source-evidence string and verified flag remain empty/false. Runtime development fallbacks report their source in the Console and runtime capture context. A release-build guard rejects unverified typography. Locally imported fonts are used through explicit serialized references; the script does not blindly select the first font in a directory.

## Official sources checked

- Unity 6.5 Sprite texture import settings: https://docs.unity3d.com/6000.5/Documentation/Manual/texture-type-sprite.html
- Unity 6.5 Application.version: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Application-version.html
- Unity 6.5 OS-font resolution: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Font.CreateDynamicFontFromOSFont.html
- Unity 6.5 installed font names: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Font.GetOSInstalledFontNames.html
- Unity 6.5 character coverage checks: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Font.HasCharacter.html
- Microsoft Times New Roman family documentation: https://learn.microsoft.com/en-us/typography/font-list/times-new-roman

These sources document APIs and font families; none identifies a typeface from this particular generated reference image.
