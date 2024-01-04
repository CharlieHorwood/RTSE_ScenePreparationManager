
 RTSE /Core/Scripts/Game/GameManager.cs Edits
in the private bool Build (){}
around line 276 change RandomizeFactionSlots(CurrBuilder.Data.factionSlotIndexSeed?.ToList()); to be wrapped in this
```
if (CurrBuilder.Data.factionSlotIndexSeed == null)
            {
                RandomizeFactionSlots();
            }
            else
            {

                RandomizeFactionSlots(CurrBuilder.Data.factionSlotIndexSeed?.ToList());
            }
```
Then Approximatly Line 360 in private void RandomizeFactionSlots change 
```
 && indexSeedList.Count == factionSlots.Count
// to 
 && indexSeedList.Count <= factionSlots.Count
 ```

