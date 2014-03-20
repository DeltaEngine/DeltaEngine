using System;
using System.Collections.Generic;

namespace $safeprojectname$.Enemy.Creeps
{
  public class GroupData
  {
    public GroupData(string name, List<string> creepList, float spawnInterval)
    {
      Name = name;
      CreepSpawnInterval = spawnInterval;
      CreepList = new List<CreepType>();
      foreach (string creepName in creepList)
        CreepList.Add(FindAppropriateCreepType(creepName));
    }

    public string Name { get; set; }
    public float CreepSpawnInterval { get; set; }
    public List<CreepType> CreepList { get; set; }

    public static CreepType FindAppropriateCreepType(string name)
    {
      CreepType creepType;
      switch (name)
      {
      case "Cloth":
        creepType = CreepType.Cloth;
        break;
      case "Iron":
        creepType = CreepType.Iron;
        break;
      case "Paper":
        creepType = CreepType.Paper;
        break;
      case "Wood":
        creepType = CreepType.Wood;
        break;
      case "Glass":
        creepType = CreepType.Glass;
        break;
      case "Sand":
        creepType = CreepType.Sand;
        break;
      case "Plastic":
        creepType = CreepType.Plastic;
        break;
      default:
        throw new InvalidCreepName(); //ncrunch: no coverage
      }
      return creepType;
    }

    public class InvalidCreepName : Exception {}
  }
}