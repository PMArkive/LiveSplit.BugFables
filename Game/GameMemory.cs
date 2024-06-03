using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiveSplit.BugFables
{
  public class GameMemory
  {
    private string moduleName = "";
    private const string ProcessName = "Bug Fables";

    private enum BfVersion
    {
      UNASSIGNED,
      v110,
      v113MonoBleedingEdge
    }

    private BfVersion currentBfVersion = BfVersion.UNASSIGNED;

    // Version specific information
    private const string moduleNameOldMono = "mono.dll";
    private const string moduleNameNewMono = "mono-2.0-bdwgc.dll";
    
    const int baseAddrMainManagerStaticPath110 = 0x00501AC8;
    readonly List<int> offsetPathPrefixMainManagerStatic110 = new List<int> { 0x20, 0x150 };
    private const int numFlags110 = 750;
    
    const int baseAddrMainManagerStaticPath113MonoBleedingEdge = 0x0048FA90;
    readonly List<int> offsetPathPrefixMainManagerStatic113MonoBleedingEdge = new List<int> { 0xBD0, 0x0, 0x60 };

    // Version agnostics essentials
    private int baseAddrMainManagerPath = 0;
    private List<int> offsetPathPrefixMainManagerStatic = new List<int>();
    private int numFlags = -1;

    public const int nbrBytesEnemyEncounter = 256 * 2 * sizeof(int);

    // General purpose offsets
    const int offsetArrayFirstElement = 0x20;

    // MainManager static offsets
    const int offsetMainManagerInstance = 0x10;
    const int offsetMainManagerMap = 0x20;
    const int offsetMainManagerBattle = 0x40;

    // MainManger offsets
    const int offsetMainMangerMusicIdArray = 0x160;
    const int offsetMainManagerLastEvent = 0x3b0;

    // MainManger instance offsets
    const int offsetMainManagerFlagsArray = 0x160;
    const int offsetMainManagerMusicCoroutine = 0x58;
    const int offsetMainManagerEnemyEncounter = 0x190;
    const int offsetMainManagerInEvent = 0x25e;
    
    // MapControl instance offsets
    const int offsetMapControlMapId = 0x108;

    // Unity specific offsets
    const int offsetUnityCachedPtr = 0x10;
    readonly List<int> offsetPathUnityGameObjectName = new List<int> { offsetUnityCachedPtr, 0x30, 0x60, 0x0 };

    private Process BfGameProcess = null;

    private DeepPointer DPMainManagerMusicCoroutine = null;
    private DeepPointer DPMainManagerCurrentRoomName = null;
    private DeepPointer DPMainManagerFlags = null;
    private DeepPointer DPMainManagerFirstMusicId = null;
    private DeepPointer DPMainManagerBattle = null;
    private DeepPointer DPMainManagerBattlePtr = null;
    private DeepPointer DPMainManagerEnemyEncounter = null;
    private DeepPointer DPMainManagerLastEvent = null;
    private DeepPointer DPMainManagerInEvent = null;

    public GameMemory()
    {
      ProcessHook();
    }

    public bool ProcessHook()
    {
      Process proc = Process.GetProcessesByName(ProcessName).FirstOrDefault();

      // Already hooked
      if (BfGameProcess != null && proc != null)
        return true;
      // Already unhooked
      if (BfGameProcess == null && proc == null)
        return false;

      // New hook
      if (BfGameProcess == null && proc != null)
      {
        currentBfVersion = DetermineCurrentGameVersion(proc);
        InitVersionSpecificVariables();
        InitDeepPointers();
        BfGameProcess = proc;
        return true;
      }

      // New Unhook
      if (BfGameProcess != null && proc == null)
        ResetEverything();

      return false;
    }

    public bool ReadEnemyEncounter(out byte[] enemyEncounter)
    {
      enemyEncounter = new byte[nbrBytesEnemyEncounter];
      if (!DPMainManagerEnemyEncounter.DerefBytes(BfGameProcess, nbrBytesEnemyEncounter, out enemyEncounter))
      {
        enemyEncounter = null;
        return false;
      }
      return true;
    }
    public int GetNbrDefeatedForEnemyId(byte[] enemyEncounter, GameEnums.Enemy enemy)
    {
      return BitConverter.ToInt32(enemyEncounter, (int)enemy * sizeof(int) * 2 + 4);
    }

    public bool ReadFlags(out byte[] flags)
    {
      flags = new byte[numFlags];
      if (!DPMainManagerFlags.DerefBytes(BfGameProcess, numFlags, out flags))
      {
        flags = null;
        return false;
      }
      return true;
    }

    public bool ReadFirstMusicId(out int songId)
    {
      if (!DPMainManagerFirstMusicId.Deref<int>(BfGameProcess, out songId))
      {
        songId = -1;
        return false;
      }
      return true;
    }

    public bool ReadMusicCoroutineInProgress(out long musicCoroutine)
    {
      if (!DPMainManagerMusicCoroutine.Deref<long>(BfGameProcess, out musicCoroutine))
      {
        musicCoroutine = -1;
        return false;
      }
      return true;
    }

    public bool ReadBattlePtr(out long battlePtr)
    {
      if (!DPMainManagerBattlePtr.Deref<long>(BfGameProcess, out battlePtr))
      {
        if (!DPMainManagerBattle.Deref<long>(BfGameProcess, out battlePtr))
        {
          battlePtr = -1;
          return false;
        }
        battlePtr = 0;
        return true;
      }
      return true;
    }

    public bool ReadCurrentRoomId(out int roomId)
    {
      StringBuilder sb = new StringBuilder();
      if (DPMainManagerCurrentRoomName.DerefString(BfGameProcess, ReadStringType.ASCII, sb))
      {
        string roomName = sb.ToString();
        if (!int.TryParse(roomName, out roomId))
        {
          roomId = -1;
          return false;
        }
        return true;
      }

      roomId = -1;
      return false;
    }

    public bool ReadInEvent(out bool inEvent)
    {
      return DPMainManagerInEvent.Deref(BfGameProcess, out inEvent);
    }
    
    public bool ReadLastEvent(out int inEvent)
    {
      return DPMainManagerLastEvent.Deref(BfGameProcess, out inEvent);
    }

    private void InitDeepPointers()
    {
      DPMainManagerMusicCoroutine = new(moduleName, baseAddrMainManagerPath,
        GetFullOffsetPathFromParts([offsetMainManagerMusicCoroutine]));

      DPMainManagerCurrentRoomName = new(moduleName, baseAddrMainManagerPath,
        GetFullOffsetPathFromParts([offsetMainManagerMap, offsetMapControlMapId]));

      DPMainManagerCurrentRoomName = new DeepPointer(moduleName, baseAddrMainManagerPath,
        GetFullOffsetPathFromParts(new List<int> { offsetMainManagerMap }, offsetPathUnityGameObjectName));

      DPMainManagerFlags = new DeepPointer(moduleName, baseAddrMainManagerPath,
        GetFullOffsetPathFromParts(new List<int> { offsetMainManagerInstance, offsetMainManagerFlagsArray,
                                                   offsetArrayFirstElement }));

      DPMainManagerFirstMusicId = new DeepPointer(moduleName, baseAddrMainManagerPath,
        GetFullOffsetPathFromParts(new List<int> { offsetMainMangerMusicIdArray, offsetArrayFirstElement }));

      DPMainManagerBattlePtr = new DeepPointer(moduleName, baseAddrMainManagerPath,
      GetFullOffsetPathFromParts(new List<int> { offsetMainManagerBattle, offsetUnityCachedPtr }));

      DPMainManagerEnemyEncounter = new(moduleName, baseAddrMainManagerPath,
      GetFullOffsetPathFromParts([offsetMainManagerInstance, offsetMainManagerEnemyEncounter, offsetArrayFirstElement]));
      
      DPMainManagerInEvent = new(moduleName, baseAddrMainManagerPath,
        GetFullOffsetPathFromParts([offsetMainManagerInstance, offsetMainManagerInEvent]));
      
      DPMainManagerLastEvent = new(moduleName, baseAddrMainManagerPath,
        GetFullOffsetPathFromParts([offsetMainManagerLastEvent]));
    }

    private int[] GetFullOffsetPathFromParts(List<int> mainParts)
    {
      List<int> pathList = new List<int>();
      pathList.AddRange(offsetPathPrefixMainManagerStatic);
      pathList.AddRange(mainParts);
      return pathList.ToArray();
    }

    private void InitVersionSpecificVariables()
    {
      switch (currentBfVersion)
      {
        case BfVersion.v110:
          moduleName = moduleNameOldMono;
          baseAddrMainManagerPath = baseAddrMainManagerStaticPath110;
          offsetPathPrefixMainManagerStatic.AddRange(offsetPathPrefixMainManagerStatic110);
          numFlags = numFlags110;
          break;
        case BfVersion.v113MonoBleedingEdge:
          moduleName = moduleNameNewMono;
          baseAddrMainManagerPath = baseAddrMainManagerStaticPath113MonoBleedingEdge;
          offsetPathPrefixMainManagerStatic.AddRange(offsetPathPrefixMainManagerStatic113MonoBleedingEdge);
          numFlags = numFlags110;
          break;
        case BfVersion.UNASSIGNED:
          Console.WriteLine("Got an unassigned version!");
          break;
      }
    }

    private void ResetEverything()
    {
      BfGameProcess = null;
      currentBfVersion = BfVersion.UNASSIGNED;
      baseAddrMainManagerPath = 0;
      offsetPathPrefixMainManagerStatic.Clear();

      DPMainManagerMusicCoroutine = null;
      DPMainManagerCurrentRoomName = null;
      DPMainManagerFlags = null;
      DPMainManagerFirstMusicId = null;
      DPMainManagerBattle = null;
      DPMainManagerBattlePtr = null;
      DPMainManagerEnemyEncounter = null;
      DPMainManagerInEvent = null;
      DPMainManagerLastEvent = null;
      numFlags = -1;
    }

    private BfVersion DetermineCurrentGameVersion(Process proc)
    {
      foreach (ProcessModule procModule in proc.Modules)
      {
        if (procModule.ModuleName == moduleNameNewMono)
        {
          return BfVersion.v113MonoBleedingEdge;
        }

        if (procModule.ModuleName == moduleNameOldMono)
        {
          return BfVersion.v110;
        }
      }

      return BfVersion.v113MonoBleedingEdge;
    }
  }
}
