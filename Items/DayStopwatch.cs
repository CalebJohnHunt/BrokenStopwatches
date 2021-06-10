using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO; // TagCompound for Save and Load
using Terraria;
using System.Collections.Generic; // List<T>
using System;

namespace BrokenStopwatch.Items
{
	public abstract class BaseStopwatch : ModItem
	{
		public override bool CloneNewInstances => true; // Makes this. consistent
		public double time;
		public bool dayTime;

		public override void SetDefaults() 
		{
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.value = 10000;
			item.rare = 2;
			item.UseSound = SoundID.Item1;
		}



		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// CustomStopwatch is unset 
			if (this.time < 0)
			{
				tooltips.Add(new TooltipLine(mod, "SetsTimeTo", "Unset"));
				return;
			}
			TooltipLine t = new TooltipLine(mod, "SetsTimeTo", "Set to " + GetReadableFromTicks());
			tooltips.Add(t);
		}

		public virtual string GetReadableFromTicks()
		{
			double ticks = this.time % 3600; // Ticks after the hour
			double minutes = Math.Floor(ticks / 60); // minutes after the hour
			minutes += 30; // starts at 4:30 or 7:30, so add 30 to minutes
			double hours = Math.Floor(this.time / 3600); // hours
			// Main.NewText($"{this.time}{this.dayTime}"); // Debug
			if (minutes >= 60)
			{
				++hours;
				minutes -= 60;
			}
			hours += this.dayTime ? 4 : 7; // Day starts at 4(am), evening starts at 7(pm)
			double finalHours = hours % 12;
			finalHours += finalHours == 0 ? 12 : 0; // if it's noon/midnight, add 12 to show it
			// If it's day and before noon or evening and after midnight, AM
			string AMPM = this.dayTime && hours < 12 || !this.dayTime && hours >= 12 ? "AM" : "PM"; 
			return $"{finalHours}:{minutes:00} {AMPM}";
		}

		public override bool UseItem(Player player)
		{
			if (player.altFunctionUse != 2)
			{
				// Left click
				if (this.time < 0) return false; // CustomStopwatch hasn't been set
				// All Stopwatches load their time on left click
				Main.dayTime = this.dayTime;
				Main.time = this.time;
			} else
			{
				// Right click
				StopwatchRightClick();
			}
			return true;
		}

		public virtual void StopwatchRightClick()
		{
			Main.NewText("Saved to Stopwatch: " + (this.dayTime ? "Day " : "Night ") + this.time);
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
	}

	public class NoonStopwatch : BaseStopwatch
	{
		public override void SetDefaults()
		{
			// Most of the defaults are the same, so call BaseStopwatch.SetDefaults() first
			this.time = 7.5 * 60 * 60;
			this.dayTime = true;
			base.SetDefaults();
		}
	}

	public class DawnStopwatch : BaseStopwatch
	{
		public override void SetDefaults()
		{
			// Most of the defaults are the same, so call BaseStopwatch.SetDefaults() first
			this.time = 0;
			this.dayTime = true;
			base.SetDefaults();
		}
	}

	public class DuskStopwatch : BaseStopwatch
	{
		public override void SetDefaults()
		{
			// Most of the defaults are the same, so call BaseStopwatch.SetDefaults() first
			this.time = 0;
			this.dayTime = false;
			base.SetDefaults();
		}
	}

	public class MidnightStopwatch : BaseStopwatch
	{
		public override void SetDefaults()
		{
			// Most of the defaults are the same, so call BaseStopwatch.SetDefaults() first
			this.time = 16200;
			this.dayTime = false;
			base.SetDefaults();
		}
	}

	public class CustomStopwatch : BaseStopwatch
	{
		public override void SetDefaults()
		{
			// Most of the defaults are the same, so call BaseStopwatch.SetDefaults() first
			this.time = -1;
			this.dayTime = false;
			base.SetDefaults();
		}

		public override void StopwatchRightClick()
		{
			this.dayTime = Main.dayTime;
			this.time = Main.time;
			base.StopwatchRightClick();
		}

		public override TagCompound Save()
		{
			return new TagCompound {
				{"time", this.time},
				{"dayTime", this.dayTime}
			};
		}

		public override void Load(TagCompound tag)
		{
			this.time = tag.GetDouble("time");
			this.dayTime = tag.GetBool("dayTime");
		}
	}
}