# Euric reward installments — design and evidence

Status: player approved 10 + 20 + 30 + 50 silver (110 total), 2026-10-19.

## Adopted scope — 2026-10-19

The player considers the existing decoration production costs, balance and arrival
timing resolved. Preserve the accepted decoration package. No new organ-modification
restriction is requested.

Retain the full original one-gold reward entitlement. Replace the early lump sum
with four installments distributed through Euric's treatment arc. The player
approved ten silver initially and a ten-silver total bonus for deferred payment.
Reducing the total reward was rejected.

## Approved schedule

| Milestone | Silver |
| --- | ---: |
| Claim reward after repairing Euric's room | 10 |
| Finish the first treatment | 20 |
| Finish the second treatment | 30 |
| Finish the third/final treatment | 50 |
| Total | 110 |

The treatment schedule lets Euric pay personally and spans his treatment arc.
The village restoration epilogue happens after that arc. Moving the final payment
to the end of that epilogue would require a different delivery context; it must
not silently be represented as an in-person Euric payment. This distinction was
presented to the player along with the schedule question.

The player explicitly selected this schedule over the 100-silver alternative and
moving the final payment to the village epilogue.

## Verified static evidence

Target: Graveyard Keeper 1.407, installed resources.assets SHA-256
`215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`,
Assembly-CSharp.dll SHA-256
`e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`.

- Serialized `npc_euric` FlowScript, asset path ID 150524, contains a single
  `Flow_AddMoney` node (serialized node reference 634) with constant `Value=100`.
  The incoming edge is the completion setter for `dlc_souls_s6_a_1` (632), and the
  outgoing edge continues at 636. The reward dialogue consumes
  `@souls_s_s6_a_repaired_ask`; room repair completion itself also occurs through
  a distinct macro. Do not pay merely on every room-complete task notification.
- The first treatment is authored in `npc_euric`, dialogue family
  `souls_s_s16_1_*`. The later report/availability action is a separate branch.
- The second treatment is authored in `on_enter_ev_s_s25_gd_zone`, path ID 150708,
  dialogue family `souls_s_s25_*`.
- The third treatment is authored in `on_enter_ev_s_s34_gd_zone`, path ID 150709,
  dialogue family `souls_s_s34_*`. The next-day conversation in `npc_euric` is
  separate from the treatment itself.
- The village aftermath/restoration has later dialogue families through s41;
  do not equate the third treatment with completion of every DLC quest.
- Native `Flow_AddMoney.RegisterPorts` captures a float input named `Value`.
  Its flow handler applies it through `MainGame.me.player.AddToParams("money", v)`,
  calls `SmartSpeechEngine.OnMoneyCollected(v)`, then continues its native `Out`.
  Preserve both the money mutation and native presentation/notification path.
- `FlowNode.GatherPorts` registers ports, deserializes input defaults and validates
  connections. `FlowGraph.OnGraphStarted` owns initialization/binding. A change
  at either boundary must respect lifecycle order, cloning and already initialized
  instances; an existing type/member name alone does not prove safe graph surgery.

These are static owner/path findings, not installed payout acceptance. Private
asset projections and selected-method IL remain outside the repository.

## Save compatibility and implementation gate

Proposed invariant: players who already received the original full gold retain it
and do not receive the remaining installments again. Saves before reward claim
can enter the installment plan. No retroactive deductions or file rewriting.

Research must establish how enrollment and paid milestones survive reload using
the native save owner; distinguish a normal new quest bookkeeping parameter from
a retrospective migration. Do not infer enrollment from the already-consumed
stock reward phrase, because that cannot distinguish old full payment from a new
first installment across restarts.

## Production gate — READY for candidate implementation, 2026-10-19

Observable change is confined to the approved four payouts. Preserve all accepted
decoration/organ rules, quest dialogue, task transitions and graph connections.

Native-owner comparison: serialized graph replacement or adding/rebinding nodes
introduces unnecessary topology/lifecycle work. A global money hook would couple
unrelated rewards. Use the existing bound `FlowOutput.pointer` delegate extension
point instead: a single `FlowGraph.OnGraphStarted` postfix recognizes only three
exact graph names, validates the exact native money/task/text/port shape, and
appends to the relevant continuations with the native `FlowOutput.Append` method.
Reopening the mechanism checkpoint after inspecting that method eliminated the
unnecessary original-delegate wrapper: the host combines callbacks itself, and
the original continuation runs first. Reuse the host's `Flow_AddMoney` action for
later payouts; only the original reward's constant input changes from 100 to 10.

Final writers: `FlowGraph.OnGraphStarted` binds native connections before returning;
the postfix attaches after this binding. `Flow_Talk` invokes `On Finished` from its
actual speech completion callback. Treatment anchors are the Euric-spoken final
lines `souls_s_s16_1_10`, `souls_s_s25_14`, `souls_s_s34_9`. Preserve their original
continuations exactly. Idempotence checks recognize owned callbacks in the native
multicast delegate on restarts. The original room-payment dialogue keeps its
native permanent phrase-blacklist guard; no parallel quest lifetime is invented.

Preflight all three native resource graphs through `CustomFlowScript.GetGraph`
(the native FlowCanvas resource loader), ports and monetary helpers before reducing
the initial reward. Unsupported shapes retain stock behavior and report a bounded
failure. No resource-file or whole-graph replacement is needed.

Use one namespaced paid-bit mask in the native player's Item parameters, persisted
by the ordinary game save. This is normal new-feature bookkeeping, with no direct
save-file editing, retrospective deductions, quest-state rewrites or migration.
The first paid installment enrolls the save. A consumed original reward without
that enrollment remains legacy/full-paid and receives no extra installments.

Late room claim/catch-up reads the existing `npc_euric` wait tasks
`dlc_souls_s16_1`, `dlc_souls_s25`, `dlc_souls_s34_w8_day`: each is introduced after
its respective treatment. Visible or Complete records establish the earned stage;
do not use treatment-entry blacklisting as proof of finished treatment. Read the
existing KnownNPC via GetNPC/GetQuestState without constructing NPC/quest state.

Cheapest checks: deterministic plan/duplicate/catch-up/failure/continuation tests,
static validation against exact native graph projections, actual native parameter
serialization checks and clean candidate build. Installed loader/port binding,
native payout presentation and full game reload are separate player-runtime gates;
compilation or pure fixtures are not claimed as their acceptance.

## Pre-candidate checks

- Local actual-reference Release-style Roslyn compile: no warnings/errors.
- Payment-policy/continuation suite: 469 assertions pass, including all valid paid
  masks, legacy non-enrollment, earned-stage catch-up, duplicate suppression,
  invalid save records, typed value-argument preservation, native-first callback
  ordering, multicast identity and preservation of original exceptions.
- Actual 1.407 assembly check constructs the native Flow_AddMoney node, gathers
  real ports, projects the Value input, appends the production-generated typed
  callback with native FlowOutput.Append and exercises that native delegate.
- Native SmartSerializer Item binary roundtrips retain ledger masks 1/3/7/15 and
  an unrelated money parameter. This proves the ledger's serialization layer,
  not a full installed-game save/reload.
- No actual save was modified during these checks.

## Resource lookup correction — READY for 1.2.1, 2026-10-19

The 1.2.0 installed test fails at resource preflight, before any reward hook is
activated. Direct inspection of the installed `globalgamemanagers` ResourceManager
establishes the actual entries:

- `flowcanvas/wgo scripts/npc_euric` -> resource path ID 150524;
- `flowcanvas/on_enter_ev_s_s25_gd_zone` -> 150708;
- `flowcanvas/on_enter_ev_s_s34_gd_zone` -> 150709.

`CustomFlowScript.GetGraph(string)` prepends `FlowCanvas/` to its argument. It
does not search subfolders. Passing `npc_euric` therefore requests a nonexistent
entry; the correct argument is `WGO scripts/npc_euric`. The graph's own name stays
`npc_euric`, so resource lookup paths and live graph matching must be separate.
The native resource pool caches null lookup results; retrying the wrong key or
adding a delayed initializer cannot fix the missing path. This closes the timing
hypothesis from the first failure report.

The existing input/continuation mechanism remains justified: this is a local
resource-address defect, not evidence that graph surgery or global money hooks
are needed. Change only the preflight path and make feature initialization return
its actual status to the plugin's summary log. Preserve payout amounts, graph
names, hook timing, ledger and failure containment. Verify all three addresses
against the native resource table, repeat existing native contract/payment checks,
then test real installed initialization and the room payout. Static path checks
do not establish installed graph deserialization or payout acceptance.

## Accepted release and verification scope

The 1.2.0 candidate failed installed resource preflight and is immutable.
Release 1.2.1 corrects resource lookup; `TEST_BUILD_LOG.md` owns
source/build/hash/install identity and deferred runtime checks. The normal
startup and first 10-silver payment are now verified in the installed game, as is
ordinary post-payment save/reload with retained enrollment and no duplicate.
On 2026-10-19 the player explicitly accepted this exact artifact provisionally
and requested stable GitHub publication, deferring the three treatment payouts
and legacy full-paid save checks to ordinary play. These checks remain unverified;
release acceptance does not change their evidence status. No runtime source or
binary rebuild is part of promotion. Future defects require a new numbered build.
