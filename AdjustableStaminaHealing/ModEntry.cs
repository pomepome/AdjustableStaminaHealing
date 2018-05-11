using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AdjustableStaminaHealing
{
    using Player = StardewValley.Farmer;
    public class ModEntry : Mod
    {
        private readonly float MaxHealing = 2.0f;
        private readonly float MinHealing = -2.0f;

        Config config = null;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            GameEvents.OneSecondTick += OnGameUpdate;
            InputEvents.ButtonPressed += OnButtonPressed;
            CorrectHealingValue();
            Log("Finished initialization.");
        }

        public void OnGameUpdate(object sender, EventArgs args)
        {
            if(!Context.IsWorldReady)
            {
                return;
            }
            Player player = Game1.player;
            double healing = config.HealingValuePerSeconds;
            if (config.NotHealWhileMoving && ((player.isMoving() && !player.isRidingHorse()) || player.UsingTool) && healing >= 0)
            {
                return;
            }
            if(healing >= 0 && player.Stamina == player.MaxStamina)
            {
                return;
            }
            player.Stamina += (float)healing;
        }

        public void OnButtonPressed(object sender, EventArgs args)
        {
            if(IsPressed(config.DecreaseKey) && IsPressed(config.IncreaseKey))
            {
                return;
            }
            else if(IsPressed(config.DecreaseKey))
            {
                AddValueToHealing(-0.1);
            }
            else if(IsPressed(config.IncreaseKey))
            {
                AddValueToHealing(0.1);
            }
        }

        private void Log(string format, params object[] args)
        {
            Monitor.Log(string.Format(format, args));
        }
        private bool IsPressed(Keys keys)
        {
            return Keyboard.GetState().IsKeyDown(keys);
        }
        private void AddValueToHealing(double val)
        {
            config.HealingValuePerSeconds = Round(config.HealingValuePerSeconds + val, 3);
            if (CorrectHealingValue())
            {
                Helper.WriteConfig(config);
                Log("Healing value changed to {0}", config.HealingValuePerSeconds);
                if (config.HealingValuePerSeconds == 0)
                {
                    ShowHUDMessage("Stamina won't be modified by ASH itself.");
                }
                else
                {
                    double f = config.HealingValuePerSeconds > 0 ? config.HealingValuePerSeconds : -config.HealingValuePerSeconds;
                    ShowHUDMessage(Format("Stamina will be {0} by {1} per sec.", (config.HealingValuePerSeconds > 0 ? "increased" : "decreased"), f));
                }
            }
        }
        private bool CorrectHealingValue()
        {
            if(config.HealingValuePerSeconds > MaxHealing)
            {
                config.HealingValuePerSeconds = MaxHealing;
            }
            else if(config.HealingValuePerSeconds < MinHealing)
            {
                config.HealingValuePerSeconds = MinHealing;
            }
            else
            {
                return true;
            }
            config.HealingValuePerSeconds = Round(config.HealingValuePerSeconds, 3);
            return false;
        }
        private double Round(double val, int exponent)
        {
            return Math.Round(val, exponent, MidpointRounding.AwayFromZero);
        }
        private string Format(string format, params object[] args)
        {
            return string.Format(format, args);
        }
        private void ShowHUDMessage(string message,int duration = 3500)
        {
            HUDMessage hudMessage = new HUDMessage(message, 3);
            hudMessage.noIcon = true;
            hudMessage.timeLeft = duration;
            Game1.addHUDMessage(hudMessage);
        }
    }
}
