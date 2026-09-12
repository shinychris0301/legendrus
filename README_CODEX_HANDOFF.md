# Legends of Rus — ChatGPT to Codex handoff
Prepared 2026-09-12 from the visible project conversation.

This is a context package, NOT a Unity update, APK, new Loading design, or export of the entire chat transcript. No production code, settings, font files, or runtime assets are replaced.

## Install without disturbing your working project
1. Use the newest LOCAL Unity project that you have run successfully. Do not replace it with an older archive for the sake of this handoff.
2. Open its root in VS Code: the directory containing Assets/, Packages/, and ProjectSettings/.
3. Extract this package to a temporary folder. Read its contents first.
4. Copy ProjectDocs/AI/ into the project, preserving any existing notes. Put the supplied AGENTS.md at the project root only if no AGENTS.md already exists. Otherwise MERGE the relevant instructions into the existing file. Check for AGENTS.override.md too. Do not blindly overwrite project instructions.
5. Open the official OpenAI Codex extension, sign in, and start a local project chat.
6. Paste ProjectDocs/AI/CodexStartPrompt.txt into that chat. Attach the relevant reference images as image inputs; a path alone is not evidence that an image was inspected.
7. First establish that Codex sees the actual project and understands the accepted baseline. Then proceed with Visual Page 02 — Loading Style Alignment.

## Contents
- AGENTS.md: concise durable rules and startup reading order.
- ProjectDocs/AI/ConversationHandoff.md: accepted decisions, file map, known issues, and limits of the evidence.
- ProjectDocs/AI/CurrentTask.md: the current Loading alignment task and acceptance requirements.
- ProjectDocs/AI/SessionLog.md: a record to update at the end of each working session.
- ProjectDocs/AI/CodexStartPrompt.txt: the first message for Codex.
- ProjectDocs/AI/References/: copied reference images and provenance, NOT runtime dependencies.

## Reference priority
The latest user-approved LOCAL Splash render and its active resources take precedence over older screenshots. The enclosed earlier Splash capture predates later color/icon corrections; do not match final colors to it. The old Loading image is a concept reference explicitly identified as stylistically inconsistent, despite its original ApprovedScreens folder name.

## Keep the handoff current
Before ending a Codex session, ask it to update CurrentTask.md and SessionLog.md with changed paths, actual test outcomes, user approvals, blockers, and the next action. Keep stable rules in AGENTS.md rather than appending the whole chat. Review and keep Git checkpoints; never commit signing secrets. A new Codex chat should read those files and inspect the current working tree before changing anything.

## Official product documentation consulted
The following are documentation sources, not a guarantee that this browser conversation can be imported automatically into the IDE:
- IDE extension: `https://developers.openai.com/codex/ide/`
- ChatGPT-account authentication: `https://help.openai.com/en/articles/11369540-using-codex-with-your-chatgpt-plan`
- Project instructions: `https://developers.openai.com/codex/guides/agents-md/`
- Prompting and image attachments: `https://developers.openai.com/codex/prompting/`
- Resuming Codex CLI sessions: `https://developers.openai.com/codex/cli/reference/`
The developer documentation currently redirects to the official ChatGPT Learn documentation. Interface wording may vary by extension version.
