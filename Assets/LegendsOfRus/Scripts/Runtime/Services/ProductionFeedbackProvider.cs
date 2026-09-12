using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LegendsOfRus
{
    /// <summary>
    /// Persistent concrete output adapter for the provider-neutral gameplay and
    /// meta-UI feedback services. All hierarchy creation and Resources loading
    /// is completed by Initialize; cue playback only touches fixed arrays.
    /// </summary>
    public sealed class ProductionFeedbackProvider :
        MonoBehaviour,
        ICoreLoopSfxSink,
        IMetaUiSfxSink
    {
        public const int SfxVoiceCapacity = 4;

        const int CoreCueCount = (int)CoreLoopFeedbackCue.BoosterRejected + 1;
        const int MetaCueCount = (int)MetaUiMotionCue.ChapterCompleteReveal + 1;
        const float InitialCooldownTime = -1000f;

        readonly AudioSource[] sfxVoices = new AudioSource[SfxVoiceCapacity];
        readonly AudioRecipe[] coreRecipes = new AudioRecipe[CoreCueCount];
        readonly AudioRecipe[] metaRecipes = new AudioRecipe[MetaCueCount];
        readonly float[] coreNextAllowedTime = new float[CoreCueCount];
        readonly float[] metaNextAllowedTime = new float[MetaCueCount];

        CoreLoopFeedbackService coreFeedback;
        MetaUiFeedbackService metaFeedback;
        GameState gameState;
        PlatformHapticProvider haptics;
        AudioSource musicSource;
        AudioListener audioListener;
        AudioClip musicClip;
        AudioRecipe matchFourRecipe;
        AudioRecipe matchFiveRecipe;
        AudioRecipe specialLineRecipe;
        AudioRecipe specialBurstRecipe;
        AudioRecipe specialColorRecipe;
        AudioRecipe rewardButtonRecipe;
        AudioRecipe mapButtonRecipe;
        bool initialized;
        bool hierarchyPrewarmed;
        bool resourcesLoaded;
        bool ownsAudioListener;
        bool applicationPaused;
        bool applicationFocused = true;
        bool musicStarted;
        bool sceneListenerSubscribed;
        int nextVoiceIndex;

        public static ProductionFeedbackProvider Instance { get; private set; }

        public bool IsInitialized => initialized;
        public int PoolCapacity => SfxVoiceCapacity;
        public int PlayedSfxCount { get; private set; }
        public int CooldownDropCount { get; private set; }
        public int MissingClipCount { get; private set; }
        public int MissingClipRequestCount { get; private set; }
        public int VoiceStealCount { get; private set; }
        public bool HasAudioListener => audioListener != null;
        public bool OwnsAudioListener => ownsAudioListener;
        public bool HasMusicClip => musicClip != null;
        public bool IsMusicPlaying => musicSource != null && musicSource.isPlaying;
        public PlatformHapticProvider Haptics => haptics;

        public int ActiveSfxVoiceCount
        {
            get
            {
                int active = 0;
                for (int i = 0; i < sfxVoices.Length; i++)
                {
                    AudioSource source = sfxVoices[i];
                    if (source != null && source.isPlaying)
                        active++;
                }
                return active;
            }
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// Installs both concrete sink pairs atomically. Calling this more than
        /// once rebinds services without rebuilding sources or reloading clips.
        /// </summary>
        public void Initialize(
            CoreLoopFeedbackService coreLoopFeedback,
            MetaUiFeedbackService metaUiFeedback,
            GameState state
        )
        {
            if (Instance != this)
                return;

            if (Application.isPlaying)
                UnityEngine.Object.DontDestroyOnLoad(gameObject);

            PrewarmHierarchy();
            LoadResourcesOnce();

            if (haptics == null || haptics.IsDisposed)
                haptics = PlatformHapticProvider.CreateDefault();

            if (gameState != null)
                gameState.Changed -= OnGameStateChanged;

            if (initialized)
            {
                if (coreFeedback != null && coreFeedback != coreLoopFeedback)
                    coreFeedback.ResetSinks();
                if (metaFeedback != null && metaFeedback != metaUiFeedback)
                    metaFeedback.ResetSinks();
            }

            coreFeedback = coreLoopFeedback;
            metaFeedback = metaUiFeedback;
            gameState = state;

            if (gameState != null)
                gameState.Changed += OnGameStateChanged;

            initialized = true;
            if (isActiveAndEnabled)
            {
                if (coreFeedback != null)
                    coreFeedback.ConfigureSinks(this, haptics);
                if (metaFeedback != null)
                    metaFeedback.ConfigureSinks(this, haptics);
            }
            else
            {
                if (coreFeedback != null)
                    coreFeedback.ResetSinks();
                if (metaFeedback != null)
                    metaFeedback.ResetSinks();
            }
            OnGameStateChanged();
        }

        public void Play(CoreLoopFeedbackSignal signal)
        {
            if (!CanPlaySfx() || signal == null)
                return;

            int cueIndex = (int)signal.Cue;
            if (cueIndex < 0 || cueIndex >= coreRecipes.Length)
                return;

            AudioRecipe recipe = ResolveCoreRecipe(signal, cueIndex);
            PlayRecipe(recipe, cueIndex, coreNextAllowedTime);
        }

        public void Play(MetaUiFeedbackSignal signal)
        {
            if (!CanPlaySfx())
                return;

            int cueIndex = (int)signal.Cue;
            if (cueIndex < 0 || cueIndex >= metaRecipes.Length)
                return;

            AudioRecipe recipe = ResolveMetaRecipe(signal, cueIndex);
            PlayRecipe(recipe, cueIndex, metaNextAllowedTime);
        }

        void PrewarmHierarchy()
        {
            if (hierarchyPrewarmed)
                return;

            audioListener = GetComponent<AudioListener>();
            if (audioListener == null)
                audioListener = gameObject.AddComponent<AudioListener>();
            ownsAudioListener = true;

            if (!sceneListenerSubscribed)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                sceneListenerSubscribed = true;
            }
            RefreshAudioListenerState();

            for (int i = 0; i < sfxVoices.Length; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                ConfigureSource(source, false);
                sfxVoices[i] = source;
            }

            for (int i = 0; i < coreNextAllowedTime.Length; i++)
                coreNextAllowedTime[i] = InitialCooldownTime;
            for (int i = 0; i < metaNextAllowedTime.Length; i++)
                metaNextAllowedTime[i] = InitialCooldownTime;

            musicSource = gameObject.AddComponent<AudioSource>();
            ConfigureSource(musicSource, true);
            hierarchyPrewarmed = true;
        }

        static void ConfigureSource(AudioSource source, bool loop)
        {
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.dopplerLevel = 0f;
            source.pitch = 1f;
            source.volume = 1f;
            source.ignoreListenerPause = false;
            source.ignoreListenerVolume = false;
        }

        void LoadResourcesOnce()
        {
            if (resourcesLoaded)
                return;

            AudioClip swap = LoadClip("Audio/SFX/Gameplay/sfx_game_swap");
            AudioClip reject = LoadClip("Audio/SFX/Gameplay/sfx_game_near_miss");
            AudioClip matchThree = LoadClip("Audio/SFX/Gameplay/sfx_game_match_3");
            AudioClip matchFour = LoadClip("Audio/SFX/Gameplay/sfx_game_match_4");
            AudioClip matchFive = LoadClip("Audio/SFX/Gameplay/sfx_game_match_5");
            AudioClip cascade = LoadClip("Audio/SFX/Gameplay/sfx_game_cascade");
            AudioClip settle = LoadClip("Audio/SFX/Gameplay/sfx_game_fall_settle");
            AudioClip specialCreate =
                LoadClip("Audio/SFX/Gameplay/sfx_game_special_create");
            AudioClip specialActivate =
                LoadClip("Audio/SFX/Gameplay/sfx_game_bomb");
            AudioClip specialLine = LoadClip("Audio/SFX/Gameplay/sfx_game_cross");
            AudioClip specialColor = LoadClip("Audio/SFX/Gameplay/sfx_game_rainbow");
            AudioClip reshuffle = LoadClip("Audio/SFX/Gameplay/sfx_game_reshuffle");
            AudioClip recovery = LoadClip("Audio/SFX/Gameplay/sfx_game_recovery");
            AudioClip booster = LoadClip("Audio/SFX/Gameplay/sfx_game_booster");
            AudioClip boosterComplete =
                LoadClip("Audio/SFX/Gameplay/sfx_game_booster_complete");

            AudioClip tap = LoadClip("Audio/SFX/UI/sfx_ui_tap");
            AudioClip mapNode = LoadClip("Audio/SFX/UI/sfx_ui_map_node");
            AudioClip openModal = LoadClip("Audio/SFX/UI/sfx_ui_open_modal");
            AudioClip closeModal = LoadClip("Audio/SFX/UI/sfx_ui_close_modal");
            AudioClip reward = LoadClip("Audio/SFX/UI/sfx_ui_reward");
            AudioClip win = LoadClip("Audio/SFX/Gameplay/sfx_game_win");
            AudioClip fail = LoadClip("Audio/SFX/Gameplay/sfx_game_fail");
            AudioClip chapter = LoadClip("Audio/SFX/UI/sfx_ui_chapter_unlock");

            SetCoreRecipe(CoreLoopFeedbackCue.SwapAccepted, swap, 0.72f, 1f, 0.035f);
            SetCoreRecipe(CoreLoopFeedbackCue.SwapRejected, reject, 0.82f, 0.93f, 0.08f);
            SetCoreRecipe(CoreLoopFeedbackCue.InvalidSwap, reject, 0.82f, 0.90f, 0.08f);
            SetCoreRecipe(CoreLoopFeedbackCue.MatchResolved, matchThree, 0.82f, 1f, 0.045f);
            SetCoreRecipe(CoreLoopFeedbackCue.CascadeResolved, cascade, 0.86f, 1f, 0.055f);
            SetCoreRecipe(CoreLoopFeedbackCue.FallSettled, settle, 0.56f, 1f, 0.04f);
            SetCoreRecipe(CoreLoopFeedbackCue.SpecialCreated, specialCreate, 0.86f, 1f, 0.08f);
            SetCoreRecipe(CoreLoopFeedbackCue.SpecialActivated, specialActivate, 0.92f, 1f, 0.08f);
            SetCoreRecipe(CoreLoopFeedbackCue.Reshuffled, reshuffle, 0.82f, 1f, 0.22f);
            SetCoreRecipe(CoreLoopFeedbackCue.CommandRecovered, recovery, 0.76f, 0.90f, 0.24f);
            SetCoreRecipe(CoreLoopFeedbackCue.BoosterActivated, booster, 0.92f, 1f, 0.10f);
            SetCoreRecipe(CoreLoopFeedbackCue.BoosterCompleted, boosterComplete, 0.74f, 1f, 0.08f);
            SetCoreRecipe(CoreLoopFeedbackCue.BoosterRejected, reject, 0.82f, 0.88f, 0.10f);

            matchFourRecipe = new AudioRecipe(matchFour, 0.86f, 1.03f, 0.045f);
            matchFiveRecipe = new AudioRecipe(matchFive, 0.90f, 1.06f, 0.045f);
            specialLineRecipe = new AudioRecipe(specialLine, 0.88f, 1f, 0.08f);
            specialBurstRecipe = new AudioRecipe(
                specialActivate,
                0.92f,
                0.97f,
                0.08f
            );
            specialColorRecipe = new AudioRecipe(
                specialColor,
                0.94f,
                1.04f,
                0.08f
            );

            SetMetaRecipe(MetaUiMotionCue.ButtonActivated, tap, 0.72f, 1f, 0.05f);
            SetMetaRecipe(MetaUiMotionCue.ModalEnter, openModal, 0.74f, 1f, 0.08f);
            SetMetaRecipe(MetaUiMotionCue.ModalExit, closeModal, 0.66f, 1f, 0.08f);
            SetMetaRecipe(MetaUiMotionCue.RewardReveal, reward, 0.92f, 1f, 0.20f);
            SetMetaRecipe(MetaUiMotionCue.VictoryReveal, win, 0.96f, 1f, 0.30f);
            SetMetaRecipe(MetaUiMotionCue.FailureReveal, fail, 0.88f, 1f, 0.30f);
            SetMetaRecipe(MetaUiMotionCue.ChapterCompleteReveal, chapter, 0.96f, 1f, 0.30f);

            rewardButtonRecipe = new AudioRecipe(reward, 0.82f, 1f, 0.12f);
            mapButtonRecipe = new AudioRecipe(mapNode, 0.78f, 1f, 0.08f);

            musicClip = LoadClip("Audio/Music/music_ch01_theme");
            musicSource.clip = musicClip;
            resourcesLoaded = true;
        }

        AudioClip LoadClip(string resourcePath)
        {
            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            if (clip == null)
                MissingClipCount++;
            return clip;
        }

        void SetCoreRecipe(
            CoreLoopFeedbackCue cue,
            AudioClip clip,
            float volume,
            float pitch,
            float cooldown
        )
        {
            coreRecipes[(int)cue] = new AudioRecipe(clip, volume, pitch, cooldown);
        }

        void SetMetaRecipe(
            MetaUiMotionCue cue,
            AudioClip clip,
            float volume,
            float pitch,
            float cooldown
        )
        {
            metaRecipes[(int)cue] = new AudioRecipe(clip, volume, pitch, cooldown);
        }

        AudioRecipe ResolveCoreRecipe(CoreLoopFeedbackSignal signal, int cueIndex)
        {
            if (signal.Cue == CoreLoopFeedbackCue.MatchResolved)
            {
                if (signal.Magnitude >= 5 && matchFiveRecipe.Clip != null)
                    return matchFiveRecipe;
                if (signal.Magnitude >= 4 && matchFourRecipe.Clip != null)
                    return matchFourRecipe;
            }

            if (signal.Cue == CoreLoopFeedbackCue.SpecialCreated ||
                signal.Cue == CoreLoopFeedbackCue.SpecialActivated)
            {
                AudioRecipe special = ResolveSpecialRecipe(signal);
                if (special.Clip != null)
                    return signal.Cue == CoreLoopFeedbackCue.SpecialCreated
                        ? special.WithPitch(Mathf.Min(1.15f, special.Pitch + 0.04f))
                        : special;
            }

            AudioRecipe recipe = coreRecipes[cueIndex];
            if (signal.Cue == CoreLoopFeedbackCue.CascadeResolved &&
                recipe.Clip != null)
            {
                float depthLift = Mathf.Min(5, signal.CascadeDepth) * 0.025f;
                return recipe.WithPitch(Mathf.Min(1.15f, recipe.Pitch + depthLift));
            }

            return recipe;
        }

        AudioRecipe ResolveSpecialRecipe(CoreLoopFeedbackSignal signal)
        {
            switch (signal.CombinationKind)
            {
                case SpecialCombinationKind.ColorWithNormal:
                case SpecialCombinationKind.ColorWithRow:
                case SpecialCombinationKind.ColorWithColumn:
                case SpecialCombinationKind.ColorWithBurst:
                case SpecialCombinationKind.ColorWithColor:
                    return specialColorRecipe;

                case SpecialCombinationKind.RowWithBurst:
                case SpecialCombinationKind.ColumnWithBurst:
                case SpecialCombinationKind.BurstWithBurst:
                    return specialBurstRecipe;

                case SpecialCombinationKind.RowWithRow:
                case SpecialCombinationKind.ColumnWithColumn:
                case SpecialCombinationKind.RowWithColumn:
                    return specialLineRecipe;
            }

            switch (signal.SpecialKind)
            {
                case SpecialGemKind.Row:
                case SpecialGemKind.Column:
                    return specialLineRecipe;
                case SpecialGemKind.Burst:
                    return specialBurstRecipe;
                case SpecialGemKind.Color:
                    return specialColorRecipe;
                default:
                    return default;
            }
        }

        AudioRecipe ResolveMetaRecipe(MetaUiFeedbackSignal signal, int cueIndex)
        {
            if (signal.Cue == MetaUiMotionCue.ButtonActivated)
            {
                if (signal.Role == MetaUiControlRole.Reward ||
                    signal.Role == MetaUiControlRole.OneShotReward)
                {
                    if (rewardButtonRecipe.Clip != null)
                        return rewardButtonRecipe;
                }
                else if (signal.Role == MetaUiControlRole.MapNode &&
                    mapButtonRecipe.Clip != null)
                {
                    return mapButtonRecipe;
                }
            }

            return metaRecipes[cueIndex];
        }

        void PlayRecipe(
            AudioRecipe recipe,
            int cueIndex,
            float[] nextAllowedTimes
        )
        {
            if (recipe.Clip == null)
            {
                MissingClipRequestCount++;
                return;
            }

            float now = Time.realtimeSinceStartup;
            if (now < nextAllowedTimes[cueIndex])
            {
                CooldownDropCount++;
                return;
            }
            nextAllowedTimes[cueIndex] = now + recipe.CooldownSeconds;

            AudioSource voice = SelectVoice();
            if (voice == null)
                return;

            voice.clip = recipe.Clip;
            voice.volume = recipe.Volume;
            voice.pitch = recipe.Pitch;
            voice.Play();
            PlayedSfxCount++;
        }

        AudioSource SelectVoice()
        {
            for (int offset = 0; offset < sfxVoices.Length; offset++)
            {
                int index = (nextVoiceIndex + offset) % sfxVoices.Length;
                AudioSource candidate = sfxVoices[index];
                if (candidate == null || candidate.isPlaying)
                    continue;

                nextVoiceIndex = (index + 1) % sfxVoices.Length;
                return candidate;
            }

            AudioSource voice = sfxVoices[nextVoiceIndex];
            nextVoiceIndex = (nextVoiceIndex + 1) % sfxVoices.Length;
            if (voice != null)
            {
                voice.Stop();
                VoiceStealCount++;
            }
            return voice;
        }

        bool CanPlaySfx()
        {
            return initialized && isActiveAndEnabled && hierarchyPrewarmed &&
                !applicationPaused && applicationFocused &&
                gameState != null && gameState.Save != null &&
                gameState.Save.sfx;
        }

        void OnGameStateChanged()
        {
            PlayerSaveData save = gameState != null ? gameState.Save : null;
            if (save == null)
            {
                StopAllSfx();
                ApplyHapticState(false);
                StopMusic();
                return;
            }

            if (!save.sfx)
                StopAllSfx();
            ApplyHapticState(save.vibration);

            ApplyMusicState(save.music);
        }

        void ApplyHapticState(bool vibrationEnabled)
        {
            if (haptics == null)
                return;

            if (
                vibrationEnabled &&
                isActiveAndEnabled &&
                !applicationPaused &&
                applicationFocused
            )
            {
                haptics.Resume();
            }
            else
            {
                haptics.Suspend();
            }
        }

        void ApplyMusicState(bool musicEnabled)
        {
            if (musicSource == null)
                return;

            if (!musicEnabled || musicClip == null)
            {
                StopMusic();
                return;
            }

            if (!isActiveAndEnabled || applicationPaused || !applicationFocused)
            {
                if (musicStarted)
                    musicSource.Pause();
                return;
            }

            if (!musicStarted)
            {
                musicSource.clip = musicClip;
                musicSource.Play();
                musicStarted = true;
            }
            else if (!musicSource.isPlaying)
            {
                musicSource.UnPause();
            }
        }

        void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
            musicStarted = false;
        }

        void StopAllSfx()
        {
            for (int i = 0; i < sfxVoices.Length; i++)
            {
                AudioSource source = sfxVoices[i];
                if (source != null)
                    source.Stop();
            }
        }

        void OnApplicationPause(bool paused)
        {
            applicationPaused = paused;
            HandleApplicationAvailabilityChanged();
        }

        void OnApplicationFocus(bool focused)
        {
            applicationFocused = focused;
            HandleApplicationAvailabilityChanged();
        }

        void HandleApplicationAvailabilityChanged()
        {
            if (applicationPaused || !applicationFocused)
            {
                StopAllSfx();
                if (musicStarted && musicSource != null)
                    musicSource.Pause();
                ApplyHapticState(false);
                return;
            }

            bool vibrationEnabled = gameState != null && gameState.Save != null &&
                gameState.Save.vibration;
            ApplyHapticState(vibrationEnabled);
            bool musicEnabled = gameState != null && gameState.Save != null &&
                gameState.Save.music;
            ApplyMusicState(musicEnabled);
        }

        void OnEnable()
        {
            if (!initialized)
                return;

            RefreshAudioListenerState();
            if (coreFeedback != null)
                coreFeedback.ConfigureSinks(this, haptics);
            if (metaFeedback != null)
                metaFeedback.ConfigureSinks(this, haptics);
            OnGameStateChanged();
        }

        void OnDisable()
        {
            if (!initialized)
                return;

            StopAllSfx();
            if (musicStarted && musicSource != null)
                musicSource.Pause();
            haptics?.Suspend();
            if (coreFeedback != null)
                coreFeedback.ResetSinks();
            if (metaFeedback != null)
                metaFeedback.ResetSinks();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RefreshAudioListenerState();
        }

        void RefreshAudioListenerState()
        {
            if (audioListener == null)
                return;

            AudioListener[] listeners = UnityEngine.Object.FindObjectsByType<AudioListener>(
                FindObjectsInactive.Exclude
            );
            bool externalActiveListener = false;
            for (int i = 0; i < listeners.Length; i++)
            {
                AudioListener candidate = listeners[i];
                if (
                    candidate == null ||
                    candidate == audioListener ||
                    !candidate.enabled ||
                    !candidate.gameObject.activeInHierarchy
                )
                    continue;

                externalActiveListener = true;
                break;
            }

            audioListener.enabled = !externalActiveListener;
        }

        void OnDestroy()
        {
            if (Instance != this)
                return;

            if (gameState != null)
                gameState.Changed -= OnGameStateChanged;
            if (sceneListenerSubscribed)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                sceneListenerSubscribed = false;
            }

            StopAllSfx();
            StopMusic();

            if (coreFeedback != null)
                coreFeedback.ResetSinks();
            if (metaFeedback != null)
                metaFeedback.ResetSinks();

            haptics?.Dispose();
            haptics = null;
            initialized = false;
            Instance = null;
        }

        readonly struct AudioRecipe
        {
            public AudioClip Clip { get; }
            public float Volume { get; }
            public float Pitch { get; }
            public float CooldownSeconds { get; }

            public AudioRecipe(
                AudioClip clip,
                float volume,
                float pitch,
                float cooldownSeconds
            )
            {
                Clip = clip;
                Volume = Mathf.Clamp01(volume);
                Pitch = Mathf.Clamp(pitch, 0.1f, 3f);
                CooldownSeconds = Mathf.Max(0f, cooldownSeconds);
            }

            public AudioRecipe WithPitch(float pitch)
            {
                return new AudioRecipe(Clip, Volume, pitch, CooldownSeconds);
            }
        }
    }
}
