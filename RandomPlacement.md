
 RTSE /Core/Scripts/Game/GameManager.cs Edits
in the private bool Build (){}
around line 275 above RandomizeFactionSlots(CurrBuilder.Data.factionSlotIndexSeed?.ToList()); add
```
if (CurrBuilder.Data.factionSlotIndexSeed == null)
	RandomizeFactionSlots();
```
Then Approximatly Line 360 in private void RandomizeFactionSlots change 
```
 && indexSeedList.Count == factionSlots.Count
// to 
 && indexSeedList.Count <= factionSlots.Count
 ```

