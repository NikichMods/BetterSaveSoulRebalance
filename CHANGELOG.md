# Changelog

## 1.2.1 — 2026-10-19

- Splits Euric's early 100-silver room reward into 10 silver for the room, then 20, 30 and 50 after his three treatments (110 total).
- Saves installment progress through normal player parameters and leaves existing full-paid saves unchanged.
- Fixes Euric's installment initialization by loading his dialogue graph from its actual resource subfolder.
- Reports disabled installments explicitly instead of reporting full readiness after a reward initialization failure.
- Preserves the existing decoration rebalance, organ mechanics and native quest progression.
- In-game checks cover startup, the first 10-silver payment and save/reload without duplicate credit. Later treatment payouts remain subject to extended play verification.

## 1.2.0 (failed candidate)

- Replaces Euric's early 100-silver room reward with four installments: 10 for the room, then 20, 30 and 50 after his three treatments (110 total).
- Tracks paid installments through normal saved player parameters and catches up already-earned treatments when the room reward is claimed late.
- Leaves existing full-paid saves, decoration balance, organ mechanics and native quest progression unchanged.

## 1.1.1

- Initial public release.
- Rebalances selected Better Save Soul Spiritualism technology prices with Soul Gratitude and blue-point progression costs.
- Reworks the strongest Better Save Soul grave-decoration recipes toward later-game materials.
- Adds local Soul Gratitude costs to affected grave decorations and Sin Shard costs to the highest-tier sculptures.
- Displays local Soul Gratitude in manual-crafting requirements and charges it only on completed crafts.
- Preserves Remote Craft Control's normal proportional Gratitude behavior with the rebalance surcharge included in its total cost.
- Keeps display-only Gratitude isolated from shared craft recipe data for compatibility with normal crafting UI and other crafting mods.
- Leaves grave-quality values and save data unchanged.
