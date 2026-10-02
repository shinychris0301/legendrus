# Rounds 71–80 — reviewed outcome text

Ivan remains the speaker. Each victory line reflects the completed round; failure lines are retry guidance, not a diagnosis of the player’s last attempt. Accepted dialogue scenes and existing reaction keys are preserved.

| Round | Outcome | Russian | English |
| --- | --- | --- | --- |
| 71 | victory | Самоцветов хватило. Заглянем в соседний двор. | We have enough gems. Let us look in on the next courtyard. |
| 71 | failure | Буду следить за обоими цветами: одного для цели недостаточно. | I will watch both target colors. Gathering just one is not enough. |
| 72 | victory | Завалы по обе стороны убраны. Соседи смогут пройти. | The piles on both sides are cleared. The neighbors can get through. |
| 72 | failure | Проверю обе группы ящиков и счётчик нужных самоцветов. | I will check both groups of crates and the target gem counter. |
| 73 | victory | От толстой наледи ничего не осталось. Двор стал светлее. | The thick ice is gone. The courtyard looks brighter. |
| 73 | failure | Трещина во льду ещё не значит, что клетка очищена. | A crack in the ice does not mean the cell is clear yet. |
| 74 | victory | Замки на тропинках сняты. Ещё один путь к соседям открыт. | The locks along the paths are gone. Another way to the neighbors is open. |
| 74 | failure | Цепи разбросаны по полю. Осмотрю и края, и середину. | The chains are spread across the board. I will check the edges and center. |
| 75 | victory | Оба пера у нас, калитки свободны. Можно нести помощь дальше. | We have both feathers, and the gates are clear. We can carry help onward. |
| 75 | failure | Под перьями есть крепкие цепи. Сохраню ходы для повторных ударов. | There are sturdy chains beneath the feathers. I will save moves for more hits. |
| 76 | victory | По краям больше нет льда. Солнечный уголок снова наш. | No ice remains at the edges. This sunny corner is ours again. |
| 76 | failure | Не упущу лёд у краёв поля, пока собираю нужные самоцветы. | I will watch the ice at the board edges while gathering the target gems. |
| 77 | victory | Обе горсти самоцветов собраны. Искры осветят наш путь. | Both handfuls of gems are gathered. Their sparks will light our way. |
| 77 | failure | Преград нет, но самоцветов нужно много. Сверюсь с обеими целями. | There are no obstacles, but we need many gems. I will check both goals. |
| 78 | victory | Ни ящиков, ни цепей, ни льда. Вот теперь здесь можно пройти. | No crates, chains or ice remain. Now we can pass through. |
| 78 | failure | Здесь три вида преград. Проверю, какой ещё остался. | There are three kinds of obstacles here. I will check which remain. |
| 79 | victory | Третье перо тоже добралось вниз. Все три тропы расчищены. | The third feather reached the bottom too. All three paths are clear. |
| 79 | failure | Прослежу путь каждого из трёх перьев и проверю оставшийся лёд. | I will follow each of the three feathers and check the remaining ice. |
| 80 | victory | Дворы соединены свободным проходом. Вместе пережить ночь легче. | A clear passage links the courtyards. The night is easier to face together. |
| 80 | failure | Проверю цепи в центре, ящики по бокам и оба пера. | I will check the central chains, the side crates and both feathers. |

## Mechanics and scenario review

- **71:** Two color goals of sixteen each; no obstacles or delivery items.
- **72:** Ten single-layer crates in two side groups and a twenty-gem color goal.
- **73:** All six ice cells have two layers; obstacle completion requires full removal. One color goal also applies.
- **74:** Eight single-layer chains distributed from column zero to seven; one color goal; six tile colors.
- **75:** Six crates, four chains and two feathers. Two lowest chains at row five have two layers.
- **76:** Four single-layer ice cells only at columns zero and seven; one color goal.
- **77:** Two color goals of twenty-four each, larger than round seventy-one; no obstacles or board items.
- **78:** Four crates, four chains and four ice cells in separate groups, all single-layer. No collection or delivery goal.
- **79:** Three feathers in columns one, three and six; six single-layer ice cells. Avoid claiming ice is an immovable crate.
- **80:** Eight side crates, six central chains and two feathers. Preserve the existing courtyard_paths milestone and its next destination.

The unchanged round-70 scene leads toward the courtyards; the unchanged round-80 scene confirms their connection and leads toward the hearths. No new character, reward, item rule or story transition is added. Round 73 distinguishes cracking ice from fully clearing it; LevelGoalRuntime counts destroyed obstacles. Round 79 does not describe ice as an immovable crate.

Exact goals, layers and positions are preserved in [AppliedCopy.json](AppliedCopy.json).
