# Better Save Soul Rebalance — Test Build Log

Every handed DLL is immutable and tied to exact committed source plus build artifact identity.

## 1.2.1 — Euric installments (provisionally accepted for publication)

- Date: 2026-10-19. Development branch: `dev/1.2.1`.
- Fix: distinguish native resource lookup addresses from live graph names. Load
  Euric through `CustomFlowScript.GetGraph("WGO scripts/npc_euric")`; retain
  the existing `npc_euric` live-name hook. The two treatment-zone paths are
  unchanged. The native ResourceManager table proves all three addresses resolve
  to the exact previously audited assets. The old NPC address is absent.
- The initializer returns success/failure; the plugin no longer prints full
  readiness when installments are disabled. Payout policy, continuation hooks,
  ledger, decoration tables and native quest behavior are unchanged.
- Local Roslyn actual-reference compilation succeeds with warnings as errors.
  All 469 policy/continuation assertions pass. Actual 1.407 money-port/Append
  checks and four native Item ledger serialization roundtrips pass. Native
  graph-anchor/downstream-task checks pass; decoration tables are unchanged.
- Private failed-1.2.0 support-log snapshot SHA-256:
  `9b37b02dda135edbe98952547082d6fb3fb83d731bb834faaea9e6d8aa5a55a7`.
- Requested runtime check: startup `BSSR_EURIC_READY`, room payment +10 silver
  from an unclaimed save, save/reload with no repeat payment, then the existing
  +20/+30/+50 treatment and legacy-save checks. A save that claimed the full gold
  under 1.2.0 stays legacy/full-paid; it cannot test positive installments.
- Exact source: `cddf45d1bff188dd489434f4744e2a618de008fa`; frozen local
  `candidate/1.2.1` at that source. CI run `39241280286`, job `118022431857`:
  successful Release build, 0 compiler warnings/errors, all 469 assertions pass.
- Artifact `BetterSaveSoulRebalance-1.2.1`, ID `12180938449`; ZIP digest
  `sha256:a1fc2b8ec5668b73674042715b74e7b7e18867d4701adbb8995c245bf37eb1fd`.
- Raw DLL `BetterSaveSoulRebalance-1.2.1.dll`, 38400 bytes, SHA-256
  `36c33ff433852f57727dbace4538a1ed89fb37180d381c9472837585e89f34a2`.
  Assembly/file version `1.2.1.0`; product version contains the exact source SHA.
- Downloaded ZIP and raw DLL match CI digests. Actual-native contract checks
  also pass against the exact CI DLL. Installed with the game closed under the
  canonical DLL filename; installed hash matches. Previous 1.2.0 payload and failed
  support log are preserved outside the loader directory. One project payload;
  saves and unrelated mods unchanged. No game launch or successful installed
  reward initialization/payout is claimed by installation.
- Status: **provisionally accepted for stable publication by explicit player
  instruction, 2026-10-19**. The exact tested DLL is promoted without rebuilding.
  Acceptance covers the observed startup, room payment and post-payment reload;
  treatment and legacy full-paid runtime checks are deferred to ordinary play.
  This version-specific exception is recorded in AGENTS.md.
- Runtime evidence, 2026-10-19: startup succeeds with `BSSR_EURIC_READY`.
  The player reports receiving 10 silver for the room. The previous Unity player
  log survives the restart and directly records
  `BSSR_EURIC_PAYMENT stage=room silver=10`. Payment-log snapshot SHA-256:
  `33607fee6652484b5f5809b34645686a1df44c9b288098843794dce2627e25f6`.
  The restarted BepInEx log also reaches READY without reward disable/payment
  messages; snapshot SHA-256:
  `65652560632d3c385a564ff44d9f53cda020188418b6b1183a0b0b3f6f9e226b`.
- Initial reload attempt (superseded by the successful check below): the player reported no repeat credit
  after restart, but the last saved payload inspected read-only still precedes
  room completion: room claim unconsumed, paid ledger zero, one room-repair flag
  zero. The previous-session log orders that save before the reward conversation
  and has no later save. Request a save after the 10-silver payment, then reload
  that exact post-payment save. No save mutation or new binary is required.
- Corrected save/reload check, 2026-10-19: **passed**. The support log orders
  one room payment, a save, a new native load cycle and another save, with no
  subsequent payment or reward-disable event. Read-only native deserialization
  confirms paid ledger=1, room reward consumed and money=39.62002 in both the
  current payload and its first backup (the earlier pre-payment payload held
  29.62002). The player confirms no extra credit after loading. This proves room
  enrollment and ordinary save/reload retention; the observed load is within the
  running process, not a separately evidenced full process restart.
- Successful-check log SHA-256:
  `3ec0f95b9c01cbb3a738805ebcf4743e1b11c284230a90a07ea14ffb3b465e94`.
  Deferred installed checks: treatment payments +20/+30/+50, their duplicate
  suppression and preservation of legacy full-paid saves. No new build required.

## 1.2.0 — Euric reward installments (runtime test failed)

- Date: 2026-10-19. Development branch: `dev/1.2.0`.
- Player-approved schedule: 10 silver for claiming the room-repair reward, then
  20/30/50 after the three treatments; 110 total. Decoration balance is explicitly
  considered resolved and remains unchanged. No new organ restrictions.
- Exact candidate/build source: `e5446fadee285f51223701c87c0086fc41148d3f`.
- Frozen local candidate ref: `candidate/1.2.0` at that same source.
- Clean CI: run `39238441980`, job `118013130921`, Release build success with
  0 compiler warnings/errors; all 469 payment-policy/continuation assertions pass.
- CI artifact: `BetterSaveSoulRebalance-1.2.0`, ID `12179558612`;
  archive digest `sha256:fdf41d067870e3c2764128d688648de7bd3ceca5d3646f0afbef1f82a9d046f9`.
- Exact raw DLL: `BetterSaveSoulRebalance-1.2.0.dll`, 37888 bytes;
  SHA-256 `3ff0e71898deeb5394889cf368902cc296e057d0dc16c45fcb5464915ff24e76`;
  assembly/file version `1.2.0.0`; product version includes the exact source SHA.
- The downloaded CI archive and DLL hashes match the workflow output. No local
  rebuild substitutes for this handed candidate.
- Local checks against exact CI bytes and actual 1.407 assemblies pass: native
  monetary input projection/action construction, native FlowOutput.Append with
  the production-generated typed callback, and native Item parameter binary
  roundtrips for paid masks 1/3/7/15 while retaining an unrelated money value.
- Static checks confirm all three speech-completion anchors and downstream wait
  tasks against the exact native resource data. The existing technology/decoration
  table source is byte-equivalent to the accepted main baseline.
- Read-only native save census found both pre-claim and legacy full-paid saves;
  actual saves were not changed. This establishes available test populations,
  not payout or full game reload acceptance.
- Installed with the game closed as canonical `BetterSaveSoulRebalance.dll`;
  installed digest matches the CI DLL. The previous accepted 1.1.1 payload and
  preinstall support log are preserved outside the plugin directory. Exactly one
  active payload for this plugin; unrelated mods/configuration/saves unchanged.
- Requested runtime return:
  1. Confirm startup reaches `BSSR_EURIC_READY` without a reward-feature failure.
  2. On a pre-claim save, receive the room payment and confirm +10 silver.
  3. Save/reload and confirm no repeat reward; completed treatments should pay
     +20/+30/+50 once through the ordinary native path.
  4. Legacy full-paid saves receive no new treatment payments. Late room claims
     catch up already-earned stages without paying any stage twice.
- Runtime result, 2026-10-19: **failed**. The installed 1.2.0 loads, but its startup preflight reports `Error loading resource "FlowCanvas/npc_euric"`, then `BSSR_EURIC_DISABLED initialization` / `Missing native graph: npc_euric`. The ordinary room-completion conversation executes later; the player reports receiving the stock 100 silver. Read-only inspection of the resulting save confirms the native room-claim blacklist is set and the installment ledger remains zero. The generic plugin `ready` message does not establish that the reward feature is enabled. Follow-up native ResourceManager inspection proves the requested path is absent: the NPC graph resides in the WGO scripts subfolder. This closes the initial timing hypothesis; see the 1.2.1 correction above.
- Status: **runtime test failed; immutable candidate, not accepted**.
  `main`, `baseline/1.1.1-accepted`, and stable Release `v1.1.1` remain unchanged.

## 1.0.0 — legacy handed balance candidate

- Legacy repository: `SoulDLCRebalance-legacy-private`.
- Exact runtime source: `ea5e62a7894e5959ce706c4497452c934159a6c2`.
- CI run: `34508205848`; artifact `SoulDLCRebalance-1.0.0` (`10164695855`).
- Raw DLL SHA-256: `04e7556214c80d5b91f2478edf73c2f350bf5e9c54ed78031bc805435f04dfe3`.
- Runtime scope: approved Spiritualism technology prices, grave-recipe/material changes, local manual-crafting Soul Gratitude surcharge, and combined Remote Craft Gratitude totals.
- Canonical legacy status: handed for player playtest; stable acceptance was not recorded.

## 1.1.0 — public migration candidate

- Date: 2026-09-11.
- Branch: `dev/1.1.0`.
- Runtime base: exact 1.0.0 production logic and `docs/BALANCE_SPEC.md`.
- Intended runtime balance changes relative to 1.0.0: none.
- Identity changes: public plugin name `Better Save Soul Rebalance`, assembly/DLL `BetterSaveSoulRebalance`, version `1.1.0`; BepInEx GUID and runtime namespace/class are preserved.
- Exact candidate/build source: `f9e47c07f2fbb2afd4687c9385ce7dd6a002f76a`.
- Frozen candidate ref: `candidate/1.1.0` at the same source commit.
- Clean CI: run `34642110816`, job `103404110517`, success with 0 warnings / 0 errors.
- Artifact: `BetterSaveSoulRebalance-1.1.0` (`10280322500`), archive digest `sha256:4873d4cad1b02cfa427f9276fd316c9a6e494cc5ec84d48411f51977782e7820`, retention 7 days.
- Raw DLL SHA-256: `6f45c4710e6f4557129bfdf5335d10d6252698d7a2d80ed066c662d555f41317`.
- A preceding pre-handoff build from `3eb3e554553e21f7d30a6c31510bfc56e4e4a8aa` compiled with one CS0108 warning because the private logging helper name `Info` hid an inherited `BaseUnityPlugin.Info` member. It was not handed to the player. The helper was renamed to `InfoOnce` with no intended runtime behavior change, then rebuilt cleanly as the candidate above.
- Requested player test:
  1. confirm the plugin loads as `Better Save Soul Rebalance 1.1.0`;
  2. inspect selected Spiritualism technology prices and confirm the expected Soul Gratitude/blue-point values;
  3. inspect at least one affected grave recipe and confirm its materials plus local Gratitude presentation;
  4. if convenient, complete one affected manual craft and confirm Gratitude is charged once on completion;
  5. report overall balance acceptance or any regression.
- Player result: **rejected, 2026-09-11**. With 1.1.0 loaded, the normal `CraftGUI` at Stone Cutter II became visibly corrupted and closing the crafting window produced a `CraftItemGUI.OnOut()` `NullReferenceException`. Repeating the same save/mod-stack test with only Better Save Soul Rebalance removed restored normal crafting UI behavior and produced no corresponding crafting exception.
- Root cause: the 1.1.0 local-Gratitude display prefix temporarily appended `gratitude_as_item` to the shared `CraftDefinition.needs` during `CraftItemGUI.Redraw()`. Verified game IL builds `_multiquality_ids` one entry per original need and later passes both lists together to `BaseItemCellGUI.DrawIngredients(...)`; the temporary extra need broke that parallel-list invariant. It also exposed display-only pseudo-data to other installed `CraftItemGUI.Redraw()` prefixes such as Queue Everything.
- Status: **rejected / immutable; do not publish or move `candidate/1.1.0`**.

## 1.1.1 — crafting UI safety fix

- Date: 2026-09-11.
- Branch: `dev/1.1.1`.
- Runtime base: frozen 1.1.0 balance behavior; no approved balance numbers changed.
- Fix: remove shared `CraftDefinition.needs` mutation from the local-Gratitude display path. Inject `gratitude_as_item` only into copied renderer arguments at `BaseItemCellGUI.DrawIngredients(...)`, extend the copied `_multiquality_ids` list in lockstep, and fail closed if the available ingredient-cell shape is insufficient or unexpected.
- Expected compatibility effect: Queue Everything / Max Buttons Redux and vanilla crafting logic continue to see the real physical recipe only; the pseudo-item exists only for the final ingredient renderer call.
- Exact candidate/build source: `d684b783408d23de212ccc1d91ac51add72edf93`.
- Frozen candidate ref: `candidate/1.1.1` at the exact build source above.
- Accepted baseline ref: `baseline/1.1.1-accepted` at the exact build source above.
- Clean CI: run `34644829243`, job `103412980183`, successful Release build with 0 compiler warnings / 0 errors.
- Artifact: `BetterSaveSoulRebalance-1.1.1` (`10282001244`), archive digest `sha256:f154df6836a5bef026079f3a3062292a330f057ce3ec1fb64f26781d48d8aea2`, retention 7 days.
- Raw DLL SHA-256: `c17de0a66e590d93863ff47defd937ffefe82cf47bb2721749177025e5545cee`.
- Requested player test:
  1. reproduce the Stone Cutter II path that broke under 1.1.0 and confirm normal layout/navigation/close behavior;
  2. after unlocking an affected BSS grave recipe if necessary, confirm the local Soul Gratitude icon/value still appears with the physical ingredients;
  3. if convenient, complete one affected manual craft and confirm Gratitude is charged exactly once on completion;
  4. provide `LogOutput.log` if any crafting UI error or warning appears.
- Player result: **accepted, 2026-09-11**. The player confirmed the previously broken crafting UI path now behaves normally. The supplied runtime log shows Better Save Soul Rebalance 1.1.1 loading and reaching `ready`, with repeated normal `CraftGUI` open/close and crafting operations across the tested session and no recurrence of the 1.1.0 `CraftItemGUI.OnOut()` / crafting-UI exception.
- Acceptance scope: the 1.1.1 crafting-UI regression fix is accepted and the numbered DLL above is the immutable stable release artifact. No balance values changed relative to the approved candidate behavior.
- Stable promotion: accepted source was promoted to `main` with documentation/release metadata only; the numbered DLL was not rebuilt.
- Release publication: GitHub Actions run `34646071830` downloaded artifact `BetterSaveSoulRebalance-1.1.1`, verified the raw DLL SHA-256 above, and published GitHub Release `v1.1.1` targeting exact source `d684b783408d23de212ccc1d91ac51add72edf93`.
- Release: ID `387325673`; raw DLL asset ID `557984815`; published asset digest `sha256:c17de0a66e590d93863ff47defd937ffefe82cf47bb2721749177025e5545cee`.
- Status: **accepted stable / published**.

## 1.2.1 promotion identity

- Player explicitly requested provisional acceptance, documentation reconciliation,
  promotion to main and a GitHub Release, with follow-up fixes if ordinary play
  reveals defects. Deferred checks above are not represented as passed.
- Accepted baseline: `baseline/1.2.1-accepted` at exact tested runtime source
  `cddf45d1bff188dd489434f4744e2a618de008fa`.
- Release manifest selects CI run `39241280286` and artifact
  `BetterSaveSoulRebalance-1.2.1`; publication verifies the exact tested DLL hash.
  Release target `v1.2.1` is the tested source, not a later documentation commit.
- Promotion changes documentation/release metadata and release identity validation
  only. Runtime source remains byte-equivalent to the tested source. No rebuild.
- Publication result will be recorded after the release workflow completes.
