using System;

namespace LiveSplit.BugFables.GameEnums
{
  public enum Room
  {
    UNASSIGNED = -1,
    TestRoom = 0,
    SnakemouthTreasureRoom = 21,
    GoldenSettlementEntrance = 39,
    GoldenHillsDungeonEntrance = 45,
    GoldenHillsDungeonBoss = 53,
    HoneyFactoryEntrance = 72,
    HoneyFactoryCore = 75,
    DesertCaravanMap = 80,
    FactoryProcessingPuzzle1 = 86,
    FactoryProcessingMalbee = 89,
    HideoutEntrance = 99,
    SandCastleBossRoom = 128,
    FarGrasslands1 = 137,
    BarrenLandsEntrance = 149,
    SwamplandsBoss = 154,
    TermiteColiseum2 = 179,
    RubberPrisonGiantLairBridge = 231,
    GiantLairFridgeInside = 236,  
    GiantLairRoachVillage = 237,
    GiantLairSaplingPlains = 238,
    BugariaEndThrone = 242
  }

  public enum Enemy
  {
    UNASSIGNED = -1,
    Spider = 2,
    Zasp = 3,
    Thief = 6,
    Web = 12,
    Mothiva = 15,
    AcolyteAria = 21,
    VenusGuardian = 24,
    WaspBomber = 26,
    WaspDriller = 27,
    WaspScout = 28,
    MonsieurScarlet = 31,
    Broodmother = 36,
    Burglar = 39,
    Astotheles = 40,
    Ahoneynation = 42,
    BeeBoop = 43,
    HeavyDroneB33 = 46,
    Abomihoney = 48,
    DuneScorpion = 49,
    TheWatcher = 54,
    TheBeast = 69,
    GeneralUltimax = 72,
    PrimalWeevil = 76,
    DeadLanderAlpha = 87,
    DeadLanderBeta = 88,
    DeadLanderGamma = 89,
    WaspKing = 90,
    TheEverlastingKing = 91,
    ULTIMAXTank = 95
  }

  public enum Flag
  {
    UNASSIGNED = -1,
    GotBigCrankTopHalf = 113,
    GotBigCrankBottomHalf = 117,
    RescuedMerchants = 201,
    RescuedMalbee = 211,
    RescuedGenEri = 216,
    RescuedOverseer = 218,
    NewGameStarted = 691
  }

  public enum Song
  {
    UNASSIGNED = -1,
    Title = 12,
    LevelUp = 6
  }
}
