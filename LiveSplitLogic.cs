using LiveSplit.BugFables.UI;
using System;
using System.IO;

namespace LiveSplit.BugFables
{
  public class LiveSplitLogic
  {
    private enum EndTimeState
    {
      NotArrivedYet,
      ArrivedInRoom,
      SongLevelUpStarting,
      SongLevelIsPlaying,
      SongIsFading
    }

    private const int newGameEventId = 8;
    
    private EndTimeState currentEndTimeState = EndTimeState.NotArrivedYet;

    private GameMemory gameMemory;

    // Defaults to true so that the ShouldStart logic won't trigger if LiveSplit
    // is opened whtn a file is loaded
    private bool oldNewGameStarted = true;
    private byte[] oldEnemyEncounter = null;
    private string pathLogFile = "";

    private Split[] splits;
    private SettingsUserControl settings;

    public LiveSplitLogic(GameMemory gameMemory, SettingsUserControl settings)
    {
      this.gameMemory = gameMemory;
      this.settings = settings;
      this.pathLogFile = Directory.GetCurrentDirectory() + @"\Components\LiveSplit.BugFables-log.txt";
      InitSplits();
      LogToFile("STARTED");
    }

    private void LogToFile(string message)
    {
      if (!File.Exists(pathLogFile))
      {
        var fs = File.Create(pathLogFile);
        fs.Close();
      }

      string currentTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

      using (StreamWriter file = new StreamWriter(pathLogFile, append: true))
      {
        file.WriteLine("[" + currentTimeStamp + "] " + message);
      }
    }

    private void InitSplits()
    {
      splits = settings.GetSplitsGlitchless();
    }

    public bool ShouldStart()
    {
      byte[] flags;
      bool inEvent = false;
      int lastEvent = -1;

      try
      {
        if (!gameMemory.ReadFlags(out flags))
        {
          LogToFile("ShouldStart: Couldn't read the flags");
          return false;
        }
        if (!gameMemory.ReadInEvent(out inEvent))
        {
          LogToFile("ShouldStart: Couldn't read inevent");
          return false;
        }
        if (!gameMemory.ReadLastEvent(out lastEvent))
        {
          LogToFile("ShouldStart: Couldn't read lastevent");
          return false;
        }
      }
      catch (Exception ex)
      {
        LogToFile("ShouldStart: Unhandled exception while reading memory: " + ex.Message);
        return false;
      }
      bool newNewGameStarted = BitConverter.ToBoolean(flags, (int)GameEnums.Flag.NewGameStarted);
      bool shouldStart = (newNewGameStarted && !oldNewGameStarted);

      if (newNewGameStarted != oldNewGameStarted)
        LogToFile("ShouldStart: NewGameStarted was " + oldNewGameStarted + " and now " + newNewGameStarted);

      oldNewGameStarted = newNewGameStarted;
      
      // Having the new game started flag being toggled to true isn't enough: we need to make sure the value was
      // changed as part of being in the new game event because it's possible it was set as part of the save load event.
      // It also improves the reliability since this check can be destructive if a false positive happen
      return shouldStart && inEvent && lastEvent == newGameEventId;
    }

    public bool ShouldSplit(int currentSplitIndex, int currentRunSplitsCount)
    {
      if (settings.Mode != SettingsUserControl.AutoSplitterMode.StartEndOnly &&
          currentSplitIndex != currentRunSplitsCount - 1)
      {
        return ShouldMidSplit(currentSplitIndex);
      }

      return ShouldEnd();
    }

    private bool ShouldMidSplit(int currentSplitIndex)
    {
      if (splits.Length - 1 < currentSplitIndex)
        return false;

      int currentRoomId;
      byte[] flags;
      byte[] enemyEncounter;
      long battlePtr;
      try
      {
        if (!gameMemory.ReadCurrentRoomId(out currentRoomId))
          return false;
        if (!gameMemory.ReadFlags(out flags))
          return false;
        if (!gameMemory.ReadEnemyEncounter(out enemyEncounter))
          return false;
        if (!gameMemory.ReadBattlePtr(out battlePtr))
          return false;
      }
      catch (Exception)
      {
        return false;
      }

      if (oldEnemyEncounter == null)
      {
        oldEnemyEncounter = new byte[GameMemory.nbrBytesEnemyEncounter];
        enemyEncounter.CopyTo(oldEnemyEncounter, 0);
      }

      Split split = splits[currentSplitIndex];

      if (!MidSplitRoomCheck(split, currentRoomId))
      {
        enemyEncounter.CopyTo(oldEnemyEncounter, 0);
        return false;
      }
      if (!MidSplitFlagsCheck(split, flags))
      {
        enemyEncounter.CopyTo(oldEnemyEncounter, 0);
        return false;
      }
      if (!MidSplitEnemyDefeatedCheck(split, enemyEncounter, battlePtr))
        return false;

      enemyEncounter.CopyTo(oldEnemyEncounter, 0);

      return true;
    }

    private bool MidSplitRoomCheck(Split split, int currentRoomId)
    {
      return (split.requiredRoom == GameEnums.Room.UNASSIGNED ||
              currentRoomId == (int)split.requiredRoom);
    }

    private bool MidSplitFlagsCheck(Split split, byte[] flags)
    {
      if (split.requiredFlags != null && split.requiredFlags.Length != 0)
      {
        bool allFlagsTrue = true;
        foreach (var requiredFlag in split.requiredFlags)
        {
          if (!BitConverter.ToBoolean(flags, (int)requiredFlag))
          {
            allFlagsTrue = false;
            break;
          }
        }

        return allFlagsTrue;
      }

      return true;
    }

    private bool MidSplitEnemyDefeatedCheck(Split split, byte[] enemyEncounter, long battlePtr)
    {
      if (split.requiredEnemiesDefeated != null && split.requiredEnemiesDefeated.Length != 0)
      {
        if (battlePtr != 0)
          return false;

        bool allEnemiesDefeated = true;
        foreach (var requiredEnemy in split.requiredEnemiesDefeated)
        {
          int newNbrDefeated = gameMemory.GetNbrDefeatedForEnemyId(enemyEncounter, requiredEnemy);
          int oldNbrDefeated = gameMemory.GetNbrDefeatedForEnemyId(oldEnemyEncounter, requiredEnemy);
          if (newNbrDefeated <= oldNbrDefeated)
          {
            allEnemiesDefeated = false;
            break;
          }
        }

        return allEnemiesDefeated;
      }

      return true;
    }

    private bool ShouldEnd()
    {
      int currentSong;
      long musicCoroutine;
      int currentRoomId;

      try
      {
        if (!gameMemory.ReadFirstMusicId(out currentSong))
        {
          LogToFile("ShouldEnd: Couldn't read the first music id");
          return false;
        }
        if (!gameMemory.ReadMusicCoroutineInProgress(out musicCoroutine))
        {
          LogToFile("ShouldEnd: Couldn't read the music coroutine");
          return false;
        }
        if (!gameMemory.ReadCurrentRoomId(out currentRoomId))
        {
          LogToFile("ShouldEnd: Couldn't read the current room id");
          return false;
        }
      }
      catch (Exception ex)
      {
        LogToFile("ShouldEnd: Unhandled exception while reading memory: " + ex.Message);
        return false;
      }

      if (currentEndTimeState == EndTimeState.NotArrivedYet)
      {
        if (currentRoomId == (int)GameEnums.Room.BugariaEndThrone)
        {
          currentEndTimeState = EndTimeState.ArrivedInRoom;
          LogToFile("ShouldEnd: Just arrived in the room");
        }
      }
      else
      {
        bool newMusicCouroutineInProgress = (musicCoroutine != 0);

        if (currentEndTimeState == EndTimeState.ArrivedInRoom && currentSong == (int)GameEnums.Song.LevelUp)
        {
          currentEndTimeState = EndTimeState.SongLevelUpStarting;
          LogToFile("ShouldEnd: The level up song is starting");
        }
        else if (currentEndTimeState == EndTimeState.SongLevelUpStarting && !newMusicCouroutineInProgress)
        {
          currentEndTimeState = EndTimeState.SongLevelIsPlaying;
          LogToFile("ShouldEnd: The level up song is playing");
        }
        else if (currentEndTimeState == EndTimeState.SongLevelIsPlaying && newMusicCouroutineInProgress)
        {
          currentEndTimeState = EndTimeState.SongIsFading;
          LogToFile("ShouldEnd: The level up song is fading");
        }
        else if (currentEndTimeState == EndTimeState.SongIsFading && !newMusicCouroutineInProgress)
        {
          currentEndTimeState = EndTimeState.NotArrivedYet;
          LogToFile("ShouldEnd: The level up song is done fading");
          return true;
        }
      }

      return false;
    }

    public void ResetLogic()
    {
      oldEnemyEncounter = null;
      currentEndTimeState = EndTimeState.NotArrivedYet;
      oldNewGameStarted = true;

      InitSplits();
      LogToFile("LOGIC RESET");
    }
  }
}
