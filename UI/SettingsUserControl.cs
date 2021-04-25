using LiveSplit.Model;
using LiveSplit.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.BugFables.UI
{
  public partial class SettingsUserControl : UserControl
  {
    public enum AutoSplitterMode
    {
      StartEndOnly,
      Glitchless
    }

    public AutoSplitterMode Mode { get; set; }
    public bool settingsHasChanged { get; set; }

    private Split[] glitchlessAllSplits;

    private LiveSplitState liveSplitState;

    public SettingsUserControl(LiveSplitState state)
    {
      InitializeComponent();
      lblVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
      settingsHasChanged = false;
      liveSplitState = state;
      InitSplits();
      rdbStartEndOnly.Checked = true;
      grpSplits.Visible = false;
      btnSetCurrentSplits.Visible = false;
      string currentGroup = "";
      foreach (var split in glitchlessAllSplits)
      {
        flowSplits.SuspendLayout();

        if (currentGroup == "" || split.group != currentGroup)
        {
          Label lbl = new Label();
          lbl.Text = split.group;
          lbl.Width = 400;
          lbl.Height = 15;
          flowSplits.Controls.Add(lbl);
        }
        flowSplits.Controls.Add(new SplitSetting(split.name, split.isEnbaled, split.group, this));

        flowSplits.ResumeLayout(true);

        currentGroup = split.group;
      }
    }
    public Split[] GetSplitsGlitchless()
    {
      return glitchlessAllSplits.Where(x => x.isEnbaled).ToArray();
    }

    public void LoadSettings(XmlNode node)
    {
      Mode = SettingsHelper.ParseEnum(node["Mode"], AutoSplitterMode.StartEndOnly);

      switch (Mode)
      {
        case AutoSplitterMode.StartEndOnly:
          rdbStartEndOnly.Checked = true;
          grpSplits.Visible = false;
          btnSetCurrentSplits.Visible = false;
          break;
        case AutoSplitterMode.Glitchless:
          rdbGlitchless.Checked = true;
          grpSplits.Visible = true;
          btnSetCurrentSplits.Visible = true;
          break;
      }

      XmlReadSplits(node.SelectNodes(".//Splits/Split"));
    }

    private void XmlReadSplits(XmlNodeList splitNodes)
    {
      if (splitNodes == null)
        return;

      SplitSetting[] splitSettings = GetSplitsSettings();

      for (int i = 0; i < splitNodes.Count; i++)
      {
        bool isSplitEnabled = SettingsHelper.ParseBool(splitNodes[i]["IsEnabled"]);
        splitSettings[i].SplitEnabled = isSplitEnabled;
        glitchlessAllSplits[i].isEnbaled = isSplitEnabled;
      }
    }

    public XmlNode SaveSettings(XmlDocument doc)
    {
      XmlElement xmlSettings = doc.CreateElement("Settings");

      SettingsHelper.CreateSetting(doc, xmlSettings, "Version",
          Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
      SettingsHelper.CreateSetting(doc, xmlSettings, "Mode", Mode);

      XmlElement xmlSplits = doc.CreateElement("Splits");
      xmlSettings.AppendChild(xmlSplits);
      SplitSetting[] splitSettings = GetSplitsSettings();

      int currentIndex = 0;
      foreach (var split in splitSettings)
      {
        XmlElement xmlSplit = doc.CreateElement("Split");
        xmlSplits.AppendChild(xmlSplit);

        SettingsHelper.CreateSetting(doc, xmlSplit, "Name", split.SplitName);
        SettingsHelper.CreateSetting(doc, xmlSplit, "Group", split.SplitGroup);
        SettingsHelper.CreateSetting(doc, xmlSplit, "IsEnabled", split.SplitEnabled);

        glitchlessAllSplits[currentIndex].isEnbaled = split.SplitEnabled;

        currentIndex++;
      }

      return xmlSettings;
    }

    private SplitSetting[] GetSplitsSettings()
    {
      List<SplitSetting> splitSettings = new List<SplitSetting>();
      foreach (var control in flowSplits.Controls)
      {
        if (control.GetType() == typeof(SplitSetting))
          splitSettings.Add((SplitSetting)control);
      }

      return splitSettings.ToArray();
    }

    private void InitSplits()
    {
      glitchlessAllSplits = new Split[]
      {
        new Split { group = "Chapter 1", name = "Enter Snakemouth",
                    requiredRoom = GameEnums.Room.SnakemouthBridgeRoom },
        new Split { group = "Chapter 1", name = "Leif Rescue",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.Web } },
        new Split { group = "Chapter 1", name = "Enter Treasure Room",
                    requiredRoom = GameEnums.Room.SnakemouthTreasureRoom },
        new Split { group = "Chapter 1", name = "Spider",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.Spider } },

        new Split { group = "Chapter 2", name = "Enter Golden Settlement",
                    requiredRoom = GameEnums.Room.GoldenSettlementEntrance },
        new Split { group = "Chapter 2", name = "Acolyte Aria",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.AcolyteAria } },
        new Split { group = "Chapter 2", name = "Golden Hills after Big Crank",
                    requiredRoom = GameEnums.Room.GoldenHillsDungeonEntrance,
                    requiredFlags = new GameEnums.Flag[] { GameEnums.Flag.GotBigCrankTopHalf,
                                                           GameEnums.Flag.GotBigCrankBottomHalf } },
        new Split { group = "Chapter 2", name = "Zasp & Mothiva",
                    requiredRoom = GameEnums.Room.GoldenHillsDungeonEntrance,
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.Zasp,
                                                                      GameEnums.Enemy.Mothiva } },
        new Split { group = "Chapter 2", name = "Enter Venus's Garden",
                    requiredRoom = GameEnums.Room.GoldenHillsDungeonBoss },
        new Split { group = "Chapter 2", name = "Venus Guardian",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.VenusGuardian } },

        new Split { group = "Chapter 3", name = "Monsieur Scarlet",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.MonsieurScarlet } },
        new Split { group = "Chapter 3", name = "Merchants Rescue",
                    requiredRoom = GameEnums.Room.DesertCaravanMap,
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.Burglar,
                                                                      GameEnums.Enemy.Thief,
                                                                      GameEnums.Enemy.WaspScout } },
        new Split { group = "Chapter 3", name = "Enter Honey Factory",
                    requiredRoom = GameEnums.Room.HoneyFactoryEntrance },
        new Split { group = "Chapter 3", name = "Gen & Eri Rescue",
                    requiredRoom = GameEnums.Room.FactoryProcessingPuzzle1,
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.BeeBoop } },
        new Split { group = "Chapter 3", name = "Malbee Rescue",
                    requiredRoom = GameEnums.Room.FactoryProcessingMalbee,
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.Abomihoney } },
        new Split { group = "Chapter 3", name = "Ahoneynation",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.Ahoneynation } },
        new Split { group = "Chapter 3", name = "Factory Core after Overseer Rescue",
                    requiredRoom = GameEnums.Room.HoneyFactoryCore,
                    requiredFlags = new GameEnums.Flag[1] { GameEnums.Flag.RescuedOverseer } },
        new Split { group = "Chapter 3", name = "Heavy Drone B-33",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.HeavyDroneB33 } },

        new Split { group = "Chapter 4", name = "Enter Bandit Hideout",
                    requiredRoom = GameEnums.Room.HideoutEntrance },
        new Split { group = "Chapter 4", name = "Enter Bandit Hideout Storage Room",
                    requiredRoom = GameEnums.Room.HideoutWestStorage },
        new Split { group = "Chapter 4", name = "Broodmother",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.Broodmother } },
        new Split { group = "Chapter 4", name = "Astotheles",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.Astotheles } },
        new Split { group = "Chapter 4", name = "Dune Scorpion",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.DuneScorpion } },
        new Split { group = "Chapter 4", name = "Enter Treasure Room",
                    requiredRoom = GameEnums.Room.SandCastleBossRoom },
        new Split { group = "Chapter 4", name = "The Watcher",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.TheWatcher } },

        new Split { group = "Chapter 5", name = "Enter Far Grasslands",
                    requiredRoom = GameEnums.Room.FarGrasslands1 },
        new Split { group = "Chapter 5", name = "Enter The Beast's Room",
                    requiredRoom = GameEnums.Room.SwamplandsBoss },
        new Split { group = "Chapter 5", name = "The Beast",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.TheBeast } },
        new Split { group = "Chapter 5", name = "General Ultimax",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.WaspDriller,
                                                                      GameEnums.Enemy.WaspBomber,
                                                                      GameEnums.Enemy.GeneralUltimax } },

        new Split { group = "Chapter 6", name = "Enter Forsaken Lands",
                    requiredRoom = GameEnums.Room.BarrenLandsEntrance },
        new Split { group = "Chapter 6", name = "Primal Weevil",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.PrimalWeevil } },
        new Split { group = "Chapter 6", name = "Zasp & Mothiva 2",
                    requiredRoom = GameEnums.Room.TermiteColiseum2,
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.Zasp,
                                                                      GameEnums.Enemy.Mothiva } },
        new Split { group = "Chapter 6", name = "Enter Rubber Prison",
                    requiredRoom = GameEnums.Room.RubberPrisonPier },
        new Split { group = "Chapter 6", name = "Rubber Prison Giant's Lair Bridge",
                    requiredRoom = GameEnums.Room.RubberPrisonGiantLairBridge },
        new Split { group = "Chapter 6", name = "ULTIMAX Tank",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.ULTIMAXTank } },

        new Split { group = "Chapter 7", name = "Enter Fridge Outside",
                    requiredRoom = GameEnums.Room.GiantLairFridgeOutside },
        new Split { group = "Chapter 7", name = "Deadlanders Trio",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.DeadLanderAlpha,
                                                                      GameEnums.Enemy.DeadLanderBeta, GameEnums.Enemy.DeadLanderGamma } },
        new Split { group = "Chapter 7", name = "Enter Fridge",
                    requiredRoom = GameEnums.Room.GiantLairFridgeInside },
        new Split { group = "Chapter 7", name = "Exit Fridge",
                    requiredRoom = GameEnums.Room.GiantLairRoachVillage },
        new Split { group = "Chapter 7", name = "Final Boss Room",
                    requiredRoom = GameEnums.Room.GiantLairSaplingPlains },
        new Split { group = "Chapter 7", name = "Wasp King",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.WaspKing } },
        new Split { group = "Chapter 7", name = "The Everlasting Sapling",
                    requiredEnemiesDefeated = new GameEnums.Enemy[] { GameEnums.Enemy.TheEverlastingKing } },
      };
    }

    private void rdbGlitchless_CheckedChanged(object sender, EventArgs e)
    {
      if (rdbGlitchless.Checked)
      {
        Mode = AutoSplitterMode.Glitchless;
        settingsHasChanged = true;
        grpSplits.Visible = true;
        btnSetCurrentSplits.Visible = true;
      }
    }

    private void rdbStartEndOnly_CheckedChanged(object sender, EventArgs e)
    {
      if (rdbStartEndOnly.Checked)
      {
        Mode = AutoSplitterMode.StartEndOnly;
        settingsHasChanged = true;
        grpSplits.Visible = false;
        btnSetCurrentSplits.Visible = false;
      }
    }

    private void btnSetCurrentSplits_Click(object sender, EventArgs e)
    {
      var answer = MessageBox.Show("This action will clear your current splits which will delete all splits and " +
                      "their associated times before adding new splits corresponding to the enabled ones in this window. " +
                      "\n\nAre you sure you want to proceed?", "Clearing current splits",
                      MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
      if (answer == DialogResult.Yes)
      {
        liveSplitState.Run.Clear();
        SplitSetting[] splitSettings = GetSplitsSettings();
        string currentGroup = "";
        foreach (var split in splitSettings)
        {
          if (!split.SplitEnabled)
            continue;

          if (currentGroup == "")
            currentGroup = split.SplitGroup;

          if (currentGroup != split.SplitGroup)
          {
            ISegment lastSplit = liveSplitState.Run.Last();
            lastSplit.Name = "{" + currentGroup + "}" + lastSplit.Name.Remove(0, 1);
            currentGroup = split.SplitGroup;
          }

          liveSplitState.Run.AddSegment("-" + split.SplitName);
        }

        liveSplitState.Run.AddSegment("{" + currentGroup + "}" + "End");

        MessageBox.Show("The operation was performed successfully. You may need to remove and add your splits " +
                        "in your layout for the changes to take effect in LiveSplit.",
                        "Operation successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }
  }
}
