﻿using System.Collections.Generic;
using System.Linq;
using SabberStoneCore.Enums;
using SabberStoneCore.Actions;
using SabberStoneCore.Enchants;
using SabberStoneCore.Model;

namespace SabberStoneCore.Tasks.SimpleTasks
{
    public class PotionGenerating : SimpleTask
    {
        private List<Card> _kazakusPotionSpells;

        private List<Card> KazakusPotionSpells => _kazakusPotionSpells ?? (_kazakusPotionSpells = GetKazakusPotionSpells());

        public PotionGenerating(List<int> scriptTags = null)
        {
            ScriptTags = scriptTags;
        }

        public List<int> ScriptTags { get; set; }

        public override TaskState Process()
        {
            
            var minion = Source as Minion;
            if (minion != null && ScriptTags == null)
            {
                Generic.CreateChoice.Invoke(Controller, ChoiceType.GENERAL, ChoiceAction.KAZAKUS, minion.Card.Entourage.Select(Cards.FromId).ToList());
                return TaskState.COMPLETE;

            }

            Game.Log(LogLevel.INFO, BlockType.PLAY, "PotionGenerating", $"Current scripttags = {string.Join(",", ScriptTags)}");

            if (ScriptTags.Count < 3)
            {

                var cost = KazakusPotionSpells.First(p => 
                    p[GameTag.TAG_SCRIPT_DATA_NUM_1] == ScriptTags[0]).Cost;
                var cardIdList = KazakusPotionSpells.Where(p => 
                    p[GameTag.TAG_SCRIPT_DATA_NUM_1] < 1000 && p.Cost == cost &&
                    (ScriptTags.Count != 2 || p[GameTag.TAG_SCRIPT_DATA_NUM_1] != ScriptTags[1])).ToList();

                var cardList = new List<Card>();
                while (cardList.Count < 3)
                {
                    var card = Util<Card>.Choose(cardIdList);
                    cardList.Add(card);
                    cardIdList.RemoveAll(p => p == card);
                }

                Generic.CreateChoice.Invoke(Controller, ChoiceType.GENERAL, ChoiceAction.KAZAKUS, cardList);
                return TaskState.COMPLETE;
            }

            // create card ...
            var baseCard = KazakusPotionSpells.First(p => p[GameTag.TAG_SCRIPT_DATA_NUM_1] == ScriptTags[0]);
            var spell1 = KazakusPotionSpells.First(p => p.Cost == baseCard.Cost && p[GameTag.TAG_SCRIPT_DATA_NUM_1] == ScriptTags[1]);
            var spell2 = KazakusPotionSpells.First(p => p.Cost == baseCard.Cost && p[GameTag.TAG_SCRIPT_DATA_NUM_1] == ScriptTags[2]);
            baseCard.Text = "(1) " + spell1.Text + "(2) " + spell2.Text;
            baseCard.Enchantments = new List<Enchantment>();
            baseCard.Enchantments.AddRange(spell1.Enchantments);
            spell1.Requirements.ToList().ForEach(p =>
            {
                if (!baseCard.Requirements.ContainsKey(p.Key))
                    baseCard.Requirements.Add(p.Key, p.Value);
            });
            baseCard.Enchantments.AddRange(spell2.Enchantments);
            spell2.Requirements.ToList().ForEach(p =>
            {
                if (!baseCard.Requirements.ContainsKey(p.Key))
                    baseCard.Requirements.Add(p.Key, p.Value);
            });

            var task = new AddCardTo(baseCard, EntityType.HAND)
            {
                Game = Controller.Game,
                Controller = Controller,
                Source = Source as IPlayable,
                Target = Target as IPlayable
            };
            Controller.Game.TaskQueue.Enqueue(task);

            return TaskState.COMPLETE;
        }

        private static List<Card> GetKazakusPotionSpells()
        {
            var list = Cards.All.Where(p => p.Id.StartsWith("CFM_621t") 
            && !p.Id.Equals("CFM_621t")
            //&& !p.Id.Equals("CFM_621t11")
            //&& !p.Id.Equals("CFM_621t12")
            //&& !p.Id.Equals("CFM_621t13")
            && !p.Id.Equals("CFM_621t14")
            && !p.Id.Equals("CFM_621t15")
            ).ToList();

            return list;
        }

        private void ProcessSplit(List<Card>[] cardsToDiscover, ChoiceAction choiceAction)
        {
 
        }

        public override ISimpleTask Clone()
        {
            var clone = new PotionGenerating(ScriptTags);
            clone.Copy(this);
            return clone;
        }
    }
}