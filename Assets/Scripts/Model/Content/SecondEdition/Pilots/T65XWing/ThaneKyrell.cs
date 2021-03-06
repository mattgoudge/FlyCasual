﻿using System.Collections;
using System.Collections.Generic;
using Ship;
using ActionsList.SecondEdition;
using System;
using SubPhases;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class ThaneKyrell : T65XWing
        {
            public ThaneKyrell() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Thane Kyrell",
                    5,
                    48,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ThaneKyrellAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 3
                );
            }
        }
    }
}
namespace Abilities.SecondEdition
{
    public class ThaneKyrellAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddThaneKyrellAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddThaneKyrellAbility;
        }

        protected virtual void AddThaneKyrellAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ThaneKyrellDiceModificationSE() { HostShip = HostShip });
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class ThaneKyrellDiceModificationSE : GenericAction
    {
        public ThaneKyrellDiceModificationSE()
        {
            Name = DiceModificationName = "Thane Kyrell's ability";
        }

        public override bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;
            if (Combat.DiceRollAttack.CriticalSuccesses == 0 && Combat.DiceRollAttack.Successes == 0 && Combat.DiceRollAttack.Focuses == 0) return false;
            if (!Combat.Defender.Damage.HasFacedownCards) return false;

            return true;
        }

        public override void ActionEffect(Action callback)
        {
            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Thane Kyrell's ability",
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    EventHandler = StartSubphase
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
        }

        private void StartSubphase(object sender, System.EventArgs e)
        {
            var spendDiceSubPhase = Phases.StartTemporarySubPhaseNew<ThaneKyrellDecisionSubPhase>(Name, Triggers.FinishTrigger);
            spendDiceSubPhase.HostShip = HostShip;
            spendDiceSubPhase.ShowSkipButton = true;
            spendDiceSubPhase.OnSkipButtonIsPressed = DecisionSubPhase.ConfirmDecision;
            spendDiceSubPhase.DecisionOwner = HostShip.Owner;
            spendDiceSubPhase.Start();
        }
    }
}

namespace SubPhases
{
    public class ThaneKyrellDecisionSubPhase : SpendDiceResultDecisionSubPhase
    {
        public GenericShip HostShip;

        protected override void PrepareDiceResultEffects()
        {
            DescriptionShort = "Thane Kyrell";
            DescriptionLong = "Select a die result to spend to flip a damage card";
            ImageSource = HostShip;

            AddSpendDiceResultEffect(DieSide.Crit, "Critical result", delegate { SpendResultToFlip(DieSide.Crit); });
            AddSpendDiceResultEffect(DieSide.Success, "Hit result", delegate { SpendResultToFlip(DieSide.Success); });
            AddSpendDiceResultEffect(DieSide.Focus, "Focus result", delegate { SpendResultToFlip(DieSide.Focus); });
        }

        private void SpendResultToFlip(DieSide side)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Combat.DiceRollAttack.RemoveType(side);
            //Combat.DiceRollAttack.OrganizeDicePositions();

            var damageCardSubPhase = Phases.StartTemporarySubPhaseNew<SelectDamageCardDecisionSubPhase>(Name, Triggers.FinishTrigger);
            damageCardSubPhase.RegisterDamageCardHandler(HandleDamageCard);
            damageCardSubPhase.withDamageCards = Combat.Defender;
            damageCardSubPhase.Start();
        }

        private void HandleDamageCard(GenericDamageCard card)
        {
            card.Expose(DecisionSubPhase.ConfirmDecision);
        }
    }
}