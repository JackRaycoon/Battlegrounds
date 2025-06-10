using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.WSA;
using static Unity.Burst.Intrinsics.Arm;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class SpellDatabase
{
   private static SpellDatabase instance;
   public BoardFiller boardFiller;
   public static SpellDatabase Instance
   {
      get
      {
         if (instance == null)
         {
            instance = new SpellDatabase();
         }
         return instance;
      }
   }
   private SpellDatabase()
   {
      // »нициализаци€ базы данных заклинаний
      InitializeSpellDatabase();
   }

   private Dictionary<string, Spell> spellDatabase = new();

   public Spell GetSpellByName(string name)
   {
      if (spellDatabase.ContainsKey(name))
      {
         return spellDatabase[name].Copy();
      }
      else
      {
         // ќбработка случа€, когда заклинание не найдено
         return null;
      }
   }

   private void InitializeSpellDatabase()
   {
      //Tests

      //Spells - Special
      AddSpellCast("Triple Reward 1", TrippleReward1);
      AddSpellCast("Triple Reward 2", TrippleReward2);
      AddSpellCast("Triple Reward 3", TrippleReward3);
      AddSpellCast("Triple Reward 4", TrippleReward4);
      AddSpellCast("Triple Reward 5", TrippleReward5);
      AddSpellCast("Triple Reward 6", TrippleReward6);
      AddSpellCast("Triple Reward 7", TrippleReward7);

      //Spells - Tavern
      AddSpellCast("Alliance Flag", AllianceFlag_Cast, AllianceFlag_Calc);
      AddSpellCast("Allied Mace", AlliedMace_Cast, AlliedMace_Calc);
      AddSpellCast("Allied Buckler", AlliedBuckler_Cast, AlliedBuckler_Calc);
      AddSpellCast("Tavern Dish Banana", TavernDishBanana_Cast, TavernDishBanana_Calc);
      AddSpellCast("Them Apples", ThemApples_Cast, ThemApples_Calc);
      AddSpellCast("Tavern Coin", TavernCoin_Cast, TavernCoin_Calc);
      AddSpellCast("Recruit a Trainee", RecruitTrainee_Cast);
      AddSpellCast("Enchanted Lasso", EnchantedLasso_Cast, null, EnchantedLasso_Valid);

      //Minion Skills
      //Calc Effects
      AddEffect("BeetlesStats", BeetlesStats_Calc);

      //Triggers - Demons
      AddEffect("Wrath Weaver Trigger", WrathWeaverTrigger_Cast);
      AddEffect("Wrath Weaver Golden Trigger", WrathWeaverTrigger_CastGolden);
      AddEffect("Picky Eater BC", ConsumeOne_Cast);
      AddEffect("Picky Eater Golden BC", ConsumeOneDoubleStats_Cast);

      //Triggers - Mechs
      AddEffect("Shielded Minibot Trigger", ShieldedMinibotTrigger_Cast);
      AddEffect("Shielded Minibot Golden Trigger", ShieldedMinibotTrigger_CastGolden);

      //Triggers - Dragons
      AddEffect("Dozy Whelp Trigger", DozyWhelpTrigger_Cast);
      AddEffect("Dozy Whelp Golden Trigger", DozyWhelpTrigger_CastGolden);

      //Triggers - Murlocs
      AddEffect("Blazing Skyfin Trigger", BlazingSkyfinTrigger_Cast);
      AddEffect("Blazing Skyfin Golden Trigger", BlazingSkyfinTrigger_CastGolden);

      //Battlecry - Demons
      AddEffect("Backstage Security BC", BackstageSecurityBC_Cast, BackstageSecurityBC_Calc);
      AddEffect("Backstage Security Golden BC", BackstageSecurityBC_CastGolden, BackstageSecurityBC_Calc);
      AddEffect("Vulgar Homunculus BC", VulgarHomunculusBC_Cast, VulgarHomunculusBC_Calc);
      AddEffect("Vulgar Homunculus Golden BC", VulgarHomunculusBC_CastGolden, VulgarHomunculusBC_Calc);
      AddEffect("Ominous Seer BC", OminousSeerBC_Cast);
      AddEffect("Ominous Seer Golden BC", OminousSeerBC_CastGolden);

      //Battlecry - Undeads
      AddEffect("Acherus Veteran BC", AcherusVeteranBC_Cast);
      AddEffect("Acherus Veteran Golden BC", AcherusVeteranBC_CastGolden);

      //Battlecry - Beasts
      AddEffect("Alleycat BC", AlleycatBC_Cast);
      AddEffect("Alleycat Golden BC", AlleycatBC_CastGolden);

      //Battlecry - Pirates
      AddEffect("Aureate Laureate BC", AureateLaureateBC_Cast);

      //Battlecry - Murlocs
      AddEffect("Bubble Gunner BC", BubbleGunnerBC_Cast);
      AddEffect("Bubble Gunner Golden BC", BubbleGunnerBC_CastGolden);

      //Deathrattle - Demons
      AddEffect("Fiendish Servant DT", FiendishServantDT_Cast);
      AddEffect("Fiendish Servant Golden DT", FiendishServantDT_CastGolden);

      AddEffect("Icky Imp DT", IckyImpDT_Cast);
      AddEffect("Icky Imp Golden DT", IckyImpDT_CastGolden);

      AddEffect("Imprisoner DT", ImprisonerDT_Cast);
      AddEffect("Imprisoner Golden DT", ImprisonerDT_CastGolden);

      //Deathrattle - Beasts
      AddEffect("Kindly Grandmother DT", KindlyGrandmotherDT_Cast);
      AddEffect("Kindly Grandmother Golden DT", KindlyGrandmotherDT_CastGolden);
      AddEffect("Buzzing Vermin DT", BuzzingVerminDT_Cast);
      AddEffect("Buzzing Vermin Golden DT", BuzzingVerminDT_CastGolden);
      AddEffect("Manasaber DT", ManasaberDT_Cast);
      AddEffect("Manasaber Golden DT", ManasaberDT_CastGolden);

      //Deathrattle - Mechs
      AddEffect("Cord Puller DT", CordPullerDT_Cast);
      AddEffect("Cord Puller Golden DT", CordPullerDT_CastGolden);
      AddEffect("Auto Assembler DT", AutoAssemblerDT_Cast);
      AddEffect("Auto Assembler Golden DT", AutoAssemblerDT_CastGolden);

      //Start Turn - Neutral
      AddEffect("Beleaguered Battler ST", BeleagueredBattlerST_Cast);
      
      //End Turn - Neutral
      AddEffect("Tavern Tipper ET", TavernTipperET_Cast);
      AddEffect("Tavern Tipper Golden ET", TavernTipperET_CastGolden);
      AddEffect("Passenger ET", PassengerET_Cast, PassengerET_Calc);
      AddEffect("Passenger Golden ET", PassengerET_CastGolden, PassengerET_CalcGolden);

      //End Turn - Mechs
      AddEffect("Lullabot ET", LullabotET_Cast);
      AddEffect("Lullabot Golden ET", LullabotET_CastGolden);  

      //Start of Combat - Murlocs
      AddEffect("Flighty Scout SC", FlightyScoutSC_Cast);
      AddEffect("Flighty Scout Golden SC", FlightyScoutSC_CastGolden);

      //Hero Abilities
      AddAbility("Bloodfury old", BloodfuryOld_Cast, null, BloodfuryOld_Valid);
      AddAbility("Bloodfury", Bloodfury_Cast);
   }

   // астер всегда на самой первой позиции листа целей.
   private void AddSpellCast(string name, Action<List<Card>> cast,
      Func<List<Card>, List<long>> calc = null,
      Func<List<Card>, bool> valid = null)
   {
      Spell spell = new(name, boardFiller)
      {
         cast = cast,
         calc = calc,
         valid = valid
      };
      spellDatabase.Add(name, spell);
   }
   private void AddEffect(string name, Action<List<Card>> cast,
      Func<List<Card>, List<long>> calc = null,
      Func<List<Card>, bool> valid = null)
   {
      Spell spell = new(name, boardFiller, SpellSO.SpellType.Effect)
      {
         cast = cast,
         calc = calc,
         valid = valid
      };
      spellDatabase.Add(name, spell);
   }   
   private void AddEffect(string name, Func<List<Card>, List<long>> calc)
   {
      Spell spell = new(name, boardFiller, SpellSO.SpellType.Effect)
      {
         calc = calc,
      };
      spellDatabase.Add(name, spell);
   }

   private void AddAbility(string name, Action<List<Card>> cast,
      Func<List<Card>, List<long>> calc = null,
      Func<List<Card>, bool> valid = null)
   {
      Spell spell = new(name, boardFiller, SpellSO.SpellType.HeroAbility)
      {
         cast = cast,
         calc = calc,
         valid = valid
      };
      spellDatabase.Add(name, spell);
   }

   private void AddPassive(string name, Action<Card, List<Card>> passive, Action<Card, List<Card>> reverse, 
      Func<List<Card>, List<long>> calc = null)
   {
      Spell spell = new(name, boardFiller, SpellSO.SpellType.Effect)
      {
         passive = passive,
         reverse = reverse,
         calc = calc
      };
      spellDatabase.Add(name, spell);
   }
   /*private void AddPassive(string name, Func<List<Card>, List<int>> calc)
   {
      Spell spell = new(name, true)
      {
         calc = calc
      };
      spellDatabase.Add(name, spell);
   }
   private void AddSkillPassive(string name)
   {
      Spell spell = new(name, true);
      spellDatabase.Add(name, spell);
   }*/





   //Tripple Rewards
   public void TrippleReward1(List<Card> targets)
   {
      TrippleReward(1);
   }
   public void TrippleReward2(List<Card> targets)
   {
      TrippleReward(2);
   }
   public void TrippleReward3(List<Card> targets)
   {
      TrippleReward(3);
   }
   public void TrippleReward4(List<Card> targets)
   {
      TrippleReward(4);
   }
   public void TrippleReward5(List<Card> targets)
   {
      TrippleReward(5);
   }
   public void TrippleReward6(List<Card> targets)
   {
      TrippleReward(6);
   }
   public void TrippleReward7(List<Card> targets)
   {
      TrippleReward(7);
   }

   public void TrippleReward(int tier)
   {
      var allTier = boardFiller.tavernController.currentPool.Where(card => card.tavernLevel == tier).ToList();
      var resList = new List<Card>();
      while(resList.Count < 3)
      {
         if (allTier.Count == 0)
            break;
         var random = allTier[Random.Range(0, allTier.Count)];
         allTier.Remove(random);
         resList.Add(new(random));
         if (!boardFiller.glossaryController.glossaryPull.Contains(random) && random.backInPool)
            boardFiller.glossaryController.glossaryPull.Add(random);
      }
      boardFiller.discoverController.EnableDiscover(resList, PullInHand);
   }

   public void PullInHand(Card card)
   {
      if(boardFiller.tavernController.minionsPool[card.data].copies > 0)
         boardFiller.tavernController.minionsPool[card.data].copies--;
      boardFiller.boardController.AddInHand(card);
   }

   //Alliance Flag

   public void AllianceFlag_Cast(List<Card> targets)
   {
      var resList = new List<Card>
      {
         GetSpellByName("Allied Mace"),
         GetSpellByName("Allied Buckler")
      };
      boardFiller.discoverController.EnableDiscover(resList, TargetOn);
   }
   private List<long> AllianceFlag_Calc(List<Card> targets)
   {
      return new List<long> { 4 + PlayerData.Instance.runInfo.tavernSpellPowerATK, 
                             3 + PlayerData.Instance.runInfo.tavernSpellPowerHP };
   }
   private List<long> AlliedMace_Calc(List<Card> targets)
   {
      return new List<long> { 4 + PlayerData.Instance.runInfo.tavernSpellPowerATK};
   }
   private List<long> AlliedBuckler_Calc(List<Card> targets)
   {
      return new List<long> { 3 + PlayerData.Instance.runInfo.tavernSpellPowerHP };
   }

   public void TargetOn(Card spell)
   {
      boardFiller.handCardAlways.isExternalSpellCast = true;
      boardFiller.handCardAlways.EnableTargetSelection(spell as Spell);
   }

   public void AlliedMace_Cast(List<Card> targets)
   {
      int atkBuff = 4;

      var caster = targets[0]; //hero
      var target = targets[1];
      if (GameController.isFightNow)
      {
         target.inFightATKBuff += atkBuff;
      }
      else
      {
         target.permanentATKBuff += atkBuff;
      }
      //target.permanentHPBuff += hpBuff;
      //target.CUR_HP += hpBuff;
   }

   public void AlliedBuckler_Cast(List<Card> targets)
   {
      int hpBuff = 3;

      var caster = targets[0]; //hero
      var target = targets[1];

      if (GameController.isFightNow)
      {
         target.inFightHPBuff += hpBuff;
         target.CUR_HP += hpBuff;
         target.bonusKeywordsInFight.Add(Card.BonusKeyword.Taunt);
      }
      else
      {
         target.permanentHPBuff += hpBuff;
         target.CUR_HP += hpBuff;
         target.bonusKeywords.Add(Card.BonusKeyword.Taunt);
      }
   }

   //TavernDishBanana
   private void TavernDishBanana_Cast(List<Card> targets)
   {
      var calc = TavernDishBanana_Calc(targets);
      long atkBuff = calc[0];
      long hpBuff = calc[1];

      var caster = targets[0]; //hero
      var target = targets[1];
      if (GameController.isFightNow)
      {
         target.inFightATKBuff += atkBuff;
         target.inFightHPBuff += hpBuff;
         target.CUR_HP += hpBuff;
      }
      else
      {
         target.permanentATKBuff += atkBuff;
         target.permanentHPBuff += hpBuff;
         target.CUR_HP += hpBuff;
      }
   }

   private List<long> TavernDishBanana_Calc(List<Card> targets)
   {
      return new List<long> { 2 + PlayerData.Instance.runInfo.tavernSpellPowerATK, 
                             2 + PlayerData.Instance.runInfo.tavernSpellPowerHP };
   }

   //Them Apples
   private void ThemApples_Cast(List<Card> targets)
   {
      var calc = ThemApples_Calc(targets);
      long atkBuff = calc[0];
      long hpBuff = calc[1];

      var caster = targets[0]; //hero
      targets.Remove(caster);
      foreach (Card target in targets)
      {
         if (target.fieldCardObject.GetComponent<FieldCardFiller>().isTavern && target is not Spell)
         {
            target.permanentATKBuff += atkBuff;
            target.permanentHPBuff += hpBuff;
            target.CUR_HP += hpBuff;
         }
      }
   }

   private List<long> ThemApples_Calc(List<Card> targets)
   {
      return new List<long> { 1 + PlayerData.Instance.runInfo.tavernSpellPowerATK,
                             2 + PlayerData.Instance.runInfo.tavernSpellPowerHP };
   }

   //Tavern Coin
   private void TavernCoin_Cast(List<Card> targets)
   {
      var calc = TavernCoin_Calc(targets);
      long money = calc[0];

      for(long i = 0; i < money; i++)
      {
         PlayerData.Instance.curMoneyCount++;
      }
      boardFiller.boardController.moneyController.UpdateMoney();
   }

   private List<long> TavernCoin_Calc(List<Card> targets)
   {
      return new List<long> { 1 };
   }

   //Recruit a Trainee
   private void RecruitTrainee_Cast(List<Card> targets)
   {
      var oneTier = boardFiller.tavernController.currentPool.Where(card => card.tavernLevel == 1).ToList();
      if (oneTier.Count != 0)
      {
         var random = oneTier[Random.Range(0, oneTier.Count)];
         Card card = new(random);
         if (boardFiller.tavernController.minionsPool[card.data].copies > 0)
            boardFiller.tavernController.minionsPool[card.data].copies--;
         boardFiller.boardController.AddInHand(card);
         if (!boardFiller.glossaryController.glossaryPull.Contains(random) && random.backInPool)
            boardFiller.glossaryController.glossaryPull.Add(random);
      }
   }

   //Enchanted Lasso
   private void EnchantedLasso_Cast(List<Card> targets)
   {
      var minions = TavernController.tavernCards.Where(card => card is not Spell).ToList();
      var minion = minions[Random.Range(0, minions.Count)];
      boardFiller.boardController.AddInHand(minion);
      boardFiller.boardController.RemoveMinionFromTavern(minion);
   }
   private bool EnchantedLasso_Valid(List<Card> targets)
   {
      var minions = TavernController.tavernCards.Where(card => card is not Spell).ToList();
      return minions.Count != 0;
   }

   //Consume One
   private void ConsumeOne_Cast(List<Card> targets)
   {
      TavernController.ConsumeFromTavern(targets[0]);
   }
   private void ConsumeOneDoubleStats_Cast(List<Card> targets)
   {
      TavernController.ConsumeFromTavern(targets[0], true);
   }

   //Beetles Stats
   private List<long> BeetlesStats_Calc(List<Card> targets)
   {
      return new List<long> { 2 + PlayerData.Instance.runInfo.beetlesATKBuff, 2 + PlayerData.Instance.runInfo.beetlesHPBuff };
   }

   //Wrath Weaver
   private void WrathWeaverTrigger_Cast(List<Card> targets)
   {
      var calc = BackstageSecurityBC_Calc(targets);
      long dmg = calc[0];

      var caster = targets[0];

      PlayerData.Instance.character.SelfDamage(dmg);
      caster.permanentATKBuff += 2;
      caster.permanentHPBuff += 1;
      caster.CUR_HP += 1;
   }
   private void WrathWeaverTrigger_CastGolden(List<Card> targets)
   {
      var calc = BackstageSecurityBC_Calc(targets);
      long dmg = calc[0];

      var caster = targets[0];
      for (int i = 0; i < 2; i++)
      {
         PlayerData.Instance.character.SelfDamage(dmg);
         caster.permanentATKBuff += 2;
         caster.permanentHPBuff += 1;
         caster.CUR_HP += 1;
      }
   }

   //Shielded Minibot
   private void ShieldedMinibotTrigger_Cast(List<Card> targets)
   {
      var caster = targets[0];
      var target = targets[1];

      if(caster.cardAbilityInfo.castsShieldedMinibot < 1)
      {
         target.bonusKeywordsInFight.Add(Card.BonusKeyword.DivineShield);
         caster.cardAbilityInfo.castsShieldedMinibot++;
      }
   }
   private void ShieldedMinibotTrigger_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      var target = targets[1];

      if (caster.cardAbilityInfo.castsShieldedMinibot < 2)
      {
         target.bonusKeywordsInFight.Add(Card.BonusKeyword.DivineShield);
         caster.cardAbilityInfo.castsShieldedMinibot++;
      }
   }

   //Dozy Whelp
   private void DozyWhelpTrigger_Cast(List<Card> targets)
   {
      var caster = targets[0];

      caster.permanentATKBuff++;
   }
   private void DozyWhelpTrigger_CastGolden(List<Card> targets)
   {
      var caster = targets[0];

      caster.permanentATKBuff+=2;
   }

   //Blazing Skyfin
   private void BlazingSkyfinTrigger_Cast(List<Card> targets)
   {
      var caster = targets[0];

      caster.permanentATKBuff++;
      caster.permanentHPBuff++;
      caster.CUR_HP++;
      if (GameController.isFightNow)
         caster.beforeBattleCurHP++;
   }
   private void BlazingSkyfinTrigger_CastGolden(List<Card> targets)
   {
      var caster = targets[0];

      caster.permanentATKBuff+=2;
      caster.permanentHPBuff+=2;
      caster.CUR_HP += 2;
      if (GameController.isFightNow)
         caster.beforeBattleCurHP += 2;
   }

   //Backstage Security
   private void BackstageSecurityBC_Cast(List<Card> targets)
   {
      var calc = BackstageSecurityBC_Calc(targets);
      long dmg = calc[0];

      PlayerData.Instance.character.SelfDamage(dmg);
   }
   private void BackstageSecurityBC_CastGolden(List<Card> targets)
   {
      var calc = BackstageSecurityBC_Calc(targets);
      long dmg = calc[0];

      PlayerData.Instance.character.SelfDamage(dmg);
      PlayerData.Instance.character.SelfDamage(dmg);
   }
   private List<long> BackstageSecurityBC_Calc(List<Card> targets)
   {
      var caster = targets[0];
      return new List<long> { 1 };
   }

   //Vulgar Homunculus
   private void VulgarHomunculusBC_Cast(List<Card> targets)
   {
      var calc = VulgarHomunculusBC_Calc(targets);
      long dmg = calc[0];

      PlayerData.Instance.character.SelfDamage(dmg);
   }
   private void VulgarHomunculusBC_CastGolden(List<Card> targets)
   {
      var calc = VulgarHomunculusBC_Calc(targets);
      long dmg = calc[0];

      PlayerData.Instance.character.SelfDamage(dmg);
      PlayerData.Instance.character.SelfDamage(dmg);
   }
   private List<long> VulgarHomunculusBC_Calc(List<Card> targets)
   {
      var caster = targets[0];
      return new List<long> { 2 };
   }

   //Ominous Seer
   private void OminousSeerBC_Cast(List<Card> targets)
   {
      PlayerData.Instance.runInfo.discountOnSpells++;
   }
   private void OminousSeerBC_CastGolden(List<Card> targets)
   {
      PlayerData.Instance.runInfo.discountOnSpells+=2;
   }

   //Acherus Veteran 
   private void AcherusVeteranBC_Cast(List<Card> targets)
   {
      var caster = targets[0];
      var target = targets[1];
      if (GameController.isFightNow)
      {
         target.inFightATKBuff += caster.ATK;
      }
      else
      {
         target.permanentATKBuff += caster.ATK;
      }
   }
   private void AcherusVeteranBC_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      var target = targets[1];
      if (GameController.isFightNow)
      {
         target.inFightATKBuff += caster.ATK * 2;
      }
      else
      {
         target.permanentATKBuff += caster.ATK * 2;
      }
   }

   //Alleycat
   private void AlleycatBC_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         boardFiller.gameController.Summon(
               new("Tabbycat"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         boardFiller.boardController.Summon(
               new("Tabbycat"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void AlleycatBC_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         boardFiller.gameController.Summon(
               new("Tabbycat Golden"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         boardFiller.boardController.Summon(
               new("Tabbycat Golden"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }

   //Aureate Laureate
   private void AureateLaureateBC_Cast(List<Card> targets)
   {
      var caster = targets[0];

      var all = Resources.LoadAll<CardSO>("Cards/Minions");
      var data = all.FirstOrDefault(card => card.name == $"{caster.data.name} Golden");
      long hpDiff = data.hp - caster.baseHP;
      caster.data = data;
      caster.isGolden = true;
      caster.CUR_HP += hpDiff;
      caster.cardAbilityInfo.isGoldenedAureateLaureate = true;
      //caster.fieldCardObject.GetComponent<FieldCardFiller>().Fill();
   }

   //Bubble Gunner
   private void BubbleGunnerBC_Cast(List<Card> targets)
   {
      var caster = targets[0];

      List<Card.BonusKeyword> bonusKeywords = new()
      {
         Card.BonusKeyword.Windfury,
         Card.BonusKeyword.Reborn,
         Card.BonusKeyword.DivineShield,
         Card.BonusKeyword.Taunt,
         Card.BonusKeyword.Stealth,
         Card.BonusKeyword.Venomous
      };

      if (GameController.isFightNow)
      {
         foreach (var key in caster.bonusKeywordsInFight)
            bonusKeywords.Remove(key);
         if(bonusKeywords.Count > 0)
         {
            var randomKey = bonusKeywords[Random.Range(0, bonusKeywords.Count)];
            caster.bonusKeywordsInFight.Add(randomKey);
         }
      }
      else
      {
         foreach (var key in caster.bonusKeywords)
            bonusKeywords.Remove(key);
         if (bonusKeywords.Count > 0)
         {
            var randomKey = bonusKeywords[Random.Range(0, bonusKeywords.Count)];
            caster.bonusKeywords.Add(randomKey);
         }
      }
   }
   private void BubbleGunnerBC_CastGolden(List<Card> targets)
   {
      var caster = targets[0];

      List<Card.BonusKeyword> bonusKeywords = new()
      {
         Card.BonusKeyword.Windfury,
         Card.BonusKeyword.Reborn,
         Card.BonusKeyword.DivineShield,
         Card.BonusKeyword.Taunt,
         Card.BonusKeyword.Stealth,
         Card.BonusKeyword.Venomous
      };

      if (GameController.isFightNow)
      {
         foreach (var key in caster.bonusKeywordsInFight)
            bonusKeywords.Remove(key);
         for(int i = 0; i < 2; i++)
            if (bonusKeywords.Count > 0)
            {
               var randomKey = bonusKeywords[Random.Range(0, bonusKeywords.Count)];
               caster.bonusKeywordsInFight.Add(randomKey);
               bonusKeywords.Remove(randomKey);
            }
      }
      else
      {
         foreach (var key in caster.bonusKeywords)
            bonusKeywords.Remove(key);
         for (int i = 0; i < 2; i++)
            if (bonusKeywords.Count > 0)
            {
               var randomKey = bonusKeywords[Random.Range(0, bonusKeywords.Count)];
               caster.bonusKeywords.Add(randomKey);
               bonusKeywords.Remove(randomKey);
            }
      }
   }

   //Fiendish Servant
   private void FiendishServantDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      var casterFiller = caster.fieldCardObject.GetComponent<FieldCardFiller>();
      List<Card> availableTargets = new(targets);
      availableTargets.Remove(caster);
      foreach (Card card in targets)
         if (card.fieldCardObject.GetComponent<FieldCardFiller>().isEnemy != casterFiller.isEnemy)
            availableTargets.Remove(card);

      if (availableTargets.Count == 0) return;

      var target = availableTargets[Random.Range(0, availableTargets.Count)];
      if (GameController.isFightNow)
      {
            target.inFightATKBuff += caster.ATK;
      }
      else
      {
         target.permanentATKBuff += caster.ATK;
      }
   }
   private void FiendishServantDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      var casterFiller = caster.fieldCardObject.GetComponent<FieldCardFiller>();
      List<Card> availableTargets = new(targets);
      availableTargets.Remove(caster);
      foreach (Card card in targets)
         if (card.fieldCardObject.GetComponent<FieldCardFiller>().isEnemy != casterFiller.isEnemy)
            availableTargets.Remove(card);

      if (availableTargets.Count == 0) return;
      for (int i = 0; i < 2; i++)
      {
         var target = availableTargets[Random.Range(0, availableTargets.Count)];
         if (GameController.isFightNow)
         {
            target.inFightATKBuff += caster.ATK;
         }
         else
         {
            target.permanentATKBuff += caster.ATK;
         }
      }
   }

   //Icky Imp
   private void IckyImpDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow) 
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 2; i++)
            boardFiller.gameController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 2; i++)
            boardFiller.boardController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) : 
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void IckyImpDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 4; i++)
            boardFiller.gameController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 4; i++)
            boardFiller.boardController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1);
      }
   }

   //Imprisoner
   private void ImprisonerDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow) 
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 1; i++)
            boardFiller.gameController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 1; i++)
            boardFiller.boardController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) : 
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void ImprisonerDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 2; i++)
            boardFiller.gameController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 2; i++)
            boardFiller.boardController.Summon(
               new("Imp"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1);
      }
   }

   //Kindly Grandmother
   private void KindlyGrandmotherDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         boardFiller.gameController.Summon(
               new("Big Bad Wolf"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         boardFiller.boardController.Summon(
               new("Big Bad Wolf"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void KindlyGrandmotherDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         boardFiller.gameController.Summon(
               new("Big Bad Wolf Golden"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         boardFiller.boardController.Summon(
               new("Big Bad Wolf Golden"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }

   //Buzzing Vermin
   private void BuzzingVerminDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         boardFiller.gameController.Summon(
               new("Beetle"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );

         //GiveHPSummonedBeetles(casterTeam, );
      }
      else
      {
         boardFiller.boardController.Summon(
               new("Beetle"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void BuzzingVerminDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for(int i = 0; i < 2; i++)
            boardFiller.gameController.Summon(
                  new("Beetle"),
                  caster,
                  caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
                  );
      }
      else
      {
         for (int i = 0; i < 2; i++)
            boardFiller.boardController.Summon(
               new("Beetle"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }

   //Manasaber
   private void ManasaberDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;
         for (int i = 0; i < 2; i++)
            boardFiller.gameController.Summon(
               new("Cubling"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 2; i++)
            boardFiller.boardController.Summon(
               new("Cubling"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void ManasaberDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 4; i++)
            boardFiller.gameController.Summon(
               new("Cubling"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 4; i++)
            boardFiller.boardController.Summon(
               new("Cubling"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }

   //Cord Puller
   private void CordPullerDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 1; i++)
            boardFiller.gameController.Summon(
               new("Microbot"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 1; i++)
            boardFiller.boardController.Summon(
               new("Microbot"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void CordPullerDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         for (int i = 0; i < 2; i++)
            boardFiller.gameController.Summon(
               new("Microbot"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         for (int i = 0; i < 2; i++)
            boardFiller.boardController.Summon(
               new("Microbot"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1);
      }
   }

   //Auto Assembler
   private void AutoAssemblerDT_Cast(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         boardFiller.gameController.Summon(
               new("Ancestral Automaton"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         boardFiller.boardController.Summon(
               new("Ancestral Automaton"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }
   private void AutoAssemblerDT_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      if (GameController.isFightNow)
      {
         List<Card> casterTeam = boardFiller.gameController.playerTeam;
         if (!boardFiller.gameController.playerTeam.Contains(caster))
            casterTeam = boardFiller.gameController.enemyTeam;

         boardFiller.gameController.Summon(
               new("Ancestral Automaton Golden"),
               caster,
               caster.isDeath ? casterTeam.IndexOf(caster) : casterTeam.IndexOf(caster) + 1
               );
      }
      else
      {
         boardFiller.boardController.Summon(
               new("Ancestral Automaton Golden"),
               caster,
               caster.isDeath ? PlayerData.Instance.playerMinions.IndexOf(caster) :
               PlayerData.Instance.playerMinions.IndexOf(caster) + 1
               );
      }
   }

   //Beleaguered Battler
   private void BeleagueredBattlerST_Cast(List<Card> targets)
   {
      var caster = targets[0];
      caster.permanentATKBuff -= 1;
   }

   //Tavern Tipper
   private void TavernTipperET_Cast(List<Card> targets)
   {
      var caster = targets[0];
      for(int i = 0; i < PlayerData.Instance.curMoneyCount; i++)
      {
         caster.permanentATKBuff += 1;
         caster.permanentHPBuff += 1;
         caster.CUR_HP += 1;
      }
   }
   private void TavernTipperET_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      for(int i = 0; i < PlayerData.Instance.curMoneyCount; i++)
      {
         caster.permanentATKBuff += 2;
         caster.permanentHPBuff += 2;
         caster.CUR_HP += 2;
      }
   }

   //Passenger
   private void PassengerET_Cast(List<Card> targets)
   {
      var caster = targets[0];
      List<CardSO> glossary = boardFiller.glossaryController.glossaryPull;
      List<CardSO> pool = boardFiller.tavernController.currentPool;

      List<CardSO> filtered = pool.Except(glossary).ToList();
      if(filtered.Count > 0)
      {
         glossary.Add(filtered[Random.Range(0, filtered.Count)]);
      }

      var calc = PassengerET_Calc(targets);
      long atk = calc[0];
      long hp = calc[1];
      caster.permanentATKBuff += atk;
      caster.permanentHPBuff += hp;
      caster.CUR_HP += hp;
   }
   private void PassengerET_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      List<CardSO> glossary = boardFiller.glossaryController.glossaryPull;
      List<CardSO> pool = boardFiller.tavernController.currentPool;

      for(int i = 0; i< 2; i++)
      {
         List<CardSO> filtered = pool.Except(glossary).ToList();
         if (filtered.Count > 0)
         {
            glossary.Add(filtered[Random.Range(0, filtered.Count)]);
         }
      }

      var calc = PassengerET_CalcGolden(targets);
      long atk = calc[0];
      long hp = calc[1];
      caster.permanentATKBuff += atk;
      caster.permanentHPBuff += hp;
      caster.CUR_HP += hp;
   }
   private List<long> PassengerET_Calc(List<Card> targets)
   {
      //var caster = targets[0];
      long count = boardFiller.glossaryController.glossaryPull.Count;
      long tens = count / 10;
      return new List<long> { tens, tens * 2 };
   }
   private List<long> PassengerET_CalcGolden(List<Card> targets)
   {
      //var caster = targets[0];
      long count = boardFiller.glossaryController.glossaryPull.Count;
      long tens = count / 10;
      return new List<long> { tens * 2, tens * 4 };
   }

   //Lullabot
   private void LullabotET_Cast(List<Card> targets)
   {
      var caster = targets[0];
      caster.permanentHPBuff += 1;
      caster.CUR_HP += 1;
   }
   private void LullabotET_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      caster.permanentHPBuff += 2;
      caster.CUR_HP += 2;
   }

   //Flighty Scout
   private void FlightyScoutSC_Cast(List<Card> targets)
   {
      var caster = targets[0];
      List<Card> casterTeam = boardFiller.gameController.playerTeam;

      boardFiller.gameController.Summon(
            new(caster, 1),
            caster,
            casterTeam.Count
            );
   }
   private void FlightyScoutSC_CastGolden(List<Card> targets)
   {
      var caster = targets[0];
      List<Card> casterTeam = boardFiller.gameController.playerTeam;

      boardFiller.gameController.Summon(
            new(caster, 2),
            caster,
            casterTeam.Count
            );
   }


   //Bloodfury
   private void Bloodfury_Cast(List<Card> targets)
   {
      var caster = targets[0];
      var demon = targets[1];

      if (demon.minionType1 == CardSO.MinionType.Demon ||
          demon.minionType2 == CardSO.MinionType.Demon)
         TavernController.ConsumeFromTavern(demon);
      else
      {
         demon.minionType1 = CardSO.MinionType.Demon;
         demon.minionType2 = CardSO.MinionType.None;
      }
   }
   private void BloodfuryOld_Cast(List<Card> targets)
   {
      var caster = targets[0];
      var demon = targets[1];

      TavernController.ConsumeFromTavern(demon);
   }
   private bool BloodfuryOld_Valid(List<Card> targets)
   {
      if (targets.Count != 2) return true;
      return targets[1].minionType1 == CardSO.MinionType.Demon || 
             targets[1].minionType2 == CardSO.MinionType.Demon;
   }

   private void GiveHPSummonedBeetles(List<Card> team, long hp)
   {
      foreach(var card in team)
      {
         if(card.data.name == "Beetle" || card.data.name == "Beetle Golden")
         {
            card.CUR_HP += hp;
            if (GameController.isFightNow)
               card.beforeBattleCurHP += hp;
         }
      }
   }
}
