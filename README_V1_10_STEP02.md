# Legends of Rus — v1.10 Step 2

Open the project in Unity 6000.5.9f1.

1. Wait for compilation.
2. Run `Legends of Rus > Validate Commercial Vertical Slice`.
3. Run `Legends of Rus > Tests > Run v1.10 Step 2 BoardModel Self-Tests`.
4. Expected result: `22/22`.
5. Open Unity Test Runner and run `LegendsOfRus.GameplayCore.Tests` EditMode tests.
6. Open `Assets/Scenes/Bootstrap.unity` and test Gameplay.

The approved visual design is unchanged. This milestone extracts deterministic puzzle
state and rules into `LegendsOfRus.GameplayCore.BoardModel`.
