# PLAT-1: replacing Windows-only touches

Two Windows-only dependencies in the legacy app need cross-platform
replacements: `My.Computer.Audio.Play` + hardcoded `.wav` paths
(`timer_state_message()`, `legacy/rats_exe/RATS/Form1.cs:3188, 3195, 3208`), and
fonts (`Arial Black`, sized `Courier New`) in the scoreboard/display
windows that aren't guaranteed present on Linux/Mac.

## Fonts — decided

**Bundle `.ttf` files with the app and register them for the process at
startup via `ctypes`**, rather than relying on whatever fonts happen to be
installed on the machine running the app. This guarantees the scoreboard
looks the same everywhere, matching the legacy app's exact visual choices
rather than hoping a similar system font is present.

Why this over the alternatives:
- Tk's font system asks the OS for a font by family name — it doesn't parse
  font files itself, so a bundled `.ttf` sitting in the app folder does
  nothing on its own. It has to be *registered* with the OS font system for
  the process, before Tkinter can reference that family name.
- A pure-`ctypes` per-OS registration call needs no extra dependency (vs. a
  third-party library like `tkextrafont`, which is a compiled extension —
  adds a dependency on prebuilt wheels existing for whatever platform/arch
  this ends up running on).
- Each target OS has a "load this font temporarily for my process, no
  install/admin needed" API:
  - **Windows**: `AddFontResourceEx` (`gdi32.dll`)
  - **macOS**: `CTFontManagerRegisterFontsForURL` (CoreText)
  - **Linux**: `FcConfigAppFontAddFile` (fontconfig — what Tk's Xft
    rendering already uses)
- None of these permanently install anything system-wide; the font is only
  available to this process, for its lifetime.

**Shape for `APP-1`**: a single `register_bundled_font(path: Path) -> None`
helper that dispatches to the right `ctypes` call based on `sys.platform`,
called once at startup for each bundled `.ttf`. Font files live in
`contest_app/src/rats/assets/fonts/` (or similar, exact path TBD when `APP-1` sets up
the package layout).

## Sounds — decided (partially)

**Bundle `.wav` files at a relative path alongside the app** (matches the
legacy app's `chimes.wav`/`chord.wav`/`tada.wav`/`notify.wav` on state
transitions 2/4/5 and elsewhere) — no more hardcoded `C:\Windows\Media\...`
paths.

**Still open**: which cross-platform playback library. `winsound`
(`My.Computer.Audio.Play`'s closest Python equivalent) is Windows-only, same
problem as the original. Options to pick between when `APP-1` gets there:
- `simpleaudio` — small, WAV-focused, no audio backend to configure
- `pygame.mixer` — heavier dependency (all of `pygame`) if nothing else in
  the app needs it, but very reliable
- `playsound` — simplest API, but has had platform-specific reliability
  issues historically (worth checking current state before relying on it)

Not deciding this now since it's a small, low-risk, easily-swappable choice
that doesn't affect any other design decision — defer to `APP-1`.

## Not a gate

Neither of the above blocks starting `APP-1` — they only matter once the
port reaches the specific display windows (fonts) and `timer_state_message`
equivalent (sounds). Tracked here so the decision is recorded ahead of time
rather than made ad hoc mid-port.
