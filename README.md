# Better Save Soul Rebalance

A focused balance mod for **Graveyard Keeper 1.407** and the **Better Save Soul** DLC.

Better Save Soul Rebalance integrates the DLC more naturally into the base-game progression. It keeps the DLC useful, but makes its strongest technologies and grave decorations require progression-appropriate resources instead of functioning as unusually cheap shortcuts.

## What it changes

- Splits Euric's early room-repair reward across four milestones: 10 silver for the room, then 20, 30 and 50 silver after his three treatments (110 silver in total).
- Preserves old saves that already received the original full reward; later installments are not added on top of it.
- Rebalances selected **Spiritualism technology costs** with additional Soul Gratitude requirements.
- Adds substantial **blue-point costs** to the high-tier Better Save Soul grave-decoration technologies.
- Reworks the material costs of the DLC's strongest stone and marble grave decorations toward later-game components.
- Adds recurring **Soul Gratitude costs** to those grave decorations.
- Adds **Sin Shard** costs to the highest-tier sculpted grave decorations.
- Shows the local Soul Gratitude cost directly in the normal manual-crafting recipe UI.
- Charges that local Gratitude only when a manual craft actually completes; cancelling an unfinished craft does not spend it.
- Preserves the game's normal Remote Craft Control behavior while folding the added local Gratitude surcharge into the remote cost.

The mod does **not** reduce the grave-quality values of Better Save Soul decorations. Installment progress uses one ordinary saved player parameter; existing money and quest progress are not rewritten.

## Requirements

- Graveyard Keeper 1.407
- Better Save Soul DLC
- BepInEx 5.x

## Download

The current release is **1.2.1**, available from [GitHub Releases](https://github.com/NikichMods/BetterSaveSoulRebalance/releases/latest).

## Installation

1. Download the DLL from GitHub Releases.
2. With the game closed, copy it into `Graveyard Keeper/BepInEx/plugins/` as `BetterSaveSoulRebalance.dll`, replacing the previous copy. Keep only one copy of this mod in the plugins folder.
3. Restart the game.

## Compatibility

The mod changes selected Better Save Soul technology prices and grave-craft definitions. Other mods that alter the same records may conflict or cause the affected target to be left unchanged by the safety checks.

The reward changes also use Euric's native dialogue/treatment scripts. Saves that
already received his original full gold keep it and do not join the installment
plan. In 1.2.1, startup, the 10-silver room reward and saving/loading after that
reward have been checked in play; the three treatment payouts and old full-paid
save behavior still await extended play verification.
