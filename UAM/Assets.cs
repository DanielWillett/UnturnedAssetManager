using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAM;

public static class Assets
{
    public static AssetTypeHierarchy[] Hierarchy;
    static Assets()
    {
        Hierarchy = new AssetTypeHierarchy[]
        {
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Hat),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Pants),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Shirt),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Backpack),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Vest),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Mask),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Glasses),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Gun),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Sight),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Tactical),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Grip),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Barrel),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Magazine),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Food),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Water),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Medical),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Melee),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Fuel),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Tool),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Vehicle_Repair_Tool, EAssetType.Tool),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Storage, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Tank, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Generator, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Beacon, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Farm, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Trap, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Structure),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Supply),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Throwable),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Grower),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Optic),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Refill),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Fisher),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Cloud),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Map),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Compass),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Key),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Box),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Arrest_Start),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Arrest_End),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Detonator),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Charge, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Library, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Filter),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Sentry, EAssetType.Storage, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Tire, EAssetType.Vehicle_Repair_Tool, EAssetType.Tool),
            new AssetTypeHierarchy(EAssetCategory.ITEM, EAssetType.Oil_Pump, EAssetType.Barricade),
            new AssetTypeHierarchy(EAssetCategory.EFFECT, EAssetType.Effect),
            new AssetTypeHierarchy(EAssetCategory.OBJECT, EAssetType.Large),
            new AssetTypeHierarchy(EAssetCategory.OBJECT, EAssetType.Medium),
            new AssetTypeHierarchy(EAssetCategory.OBJECT, EAssetType.Small),
            new AssetTypeHierarchy(EAssetCategory.OBJECT, EAssetType.NPC),
            new AssetTypeHierarchy(EAssetCategory.OBJECT, EAssetType.Decal),
            new AssetTypeHierarchy(EAssetCategory.RESOURCE, EAssetType.Resource),
            new AssetTypeHierarchy(EAssetCategory.VEHICLE, EAssetType.Vehicle),
            new AssetTypeHierarchy(EAssetCategory.ANIMAL, EAssetType.Animal),
            new AssetTypeHierarchy(EAssetCategory.MYTHIC, EAssetType.Mythic),
            new AssetTypeHierarchy(EAssetCategory.SKIN, EAssetType.Skin),
            new AssetTypeHierarchy(EAssetCategory.SPAWN, EAssetType.Spawn),
            new AssetTypeHierarchy(EAssetCategory.NPC, EAssetType.Dialogue),
            new AssetTypeHierarchy(EAssetCategory.NPC, EAssetType.Quest),
            new AssetTypeHierarchy(EAssetCategory.NPC, EAssetType.Vendor)
        };
    }
    public static readonly Dictionary<EAssetType, EUseableType> defaultUseableTypes = new Dictionary<EAssetType, EUseableType>(36)
        {
            { EAssetType.Hat, EUseableType.Clothing },
            { EAssetType.Pants, EUseableType.Clothing },
            { EAssetType.Shirt, EUseableType.Clothing },
            { EAssetType.Backpack, EUseableType.Clothing },
            { EAssetType.Vest, EUseableType.Clothing },
            { EAssetType.Mask, EUseableType.Clothing },
            { EAssetType.Glasses, EUseableType.Clothing },
            { EAssetType.Gun, EUseableType.Gun },
            { EAssetType.Food, EUseableType.Consumeable },
            { EAssetType.Water, EUseableType.Consumeable },
            { EAssetType.Medical, EUseableType.Consumeable },
            { EAssetType.Melee, EUseableType.Melee },
            { EAssetType.Fuel, EUseableType.Fuel },
            { EAssetType.Barricade, EUseableType.Barricade },
            { EAssetType.Storage, EUseableType.Barricade },
            { EAssetType.Tank, EUseableType.Barricade },
            { EAssetType.Generator, EUseableType.Barricade },
            { EAssetType.Beacon, EUseableType.Barricade },
            { EAssetType.Farm, EUseableType.Barricade },
            { EAssetType.Trap, EUseableType.Barricade },
            { EAssetType.Structure, EUseableType.Structure },
            { EAssetType.Throwable, EUseableType.Throwable },
            { EAssetType.Grower, EUseableType.Grower },
            { EAssetType.Optic, EUseableType.Optic },
            { EAssetType.Refill, EUseableType.Refill },
            { EAssetType.Fisher, EUseableType.Fisher },
            { EAssetType.Cloud, EUseableType.Cloud },
            { EAssetType.Arrest_Start, EUseableType.Arrest_Start },
            { EAssetType.Arrest_End, EUseableType.Arrest_End },
            { EAssetType.Detonator, EUseableType.Detonator },
            { EAssetType.Charge, EUseableType.Barricade },
            { EAssetType.Library, EUseableType.Barricade },
            { EAssetType.Filter, EUseableType.Filter },
            { EAssetType.Sentry, EUseableType.Barricade },
            { EAssetType.Tire, EUseableType.Tire },
            { EAssetType.Oil_Pump, EUseableType.Barricade }
        };
    private static readonly EAssetType[] _unknownAssetTypeArray = new EAssetType[1] { EAssetType.UNKNOWN };
    public static EAssetType[] GetTypes(string inType, out EAssetCategory category)
    {
        if (!Enum.TryParse(inType, true, out EAssetType child))
        {
            category = EAssetCategory.NONE;
            return _unknownAssetTypeArray;
        }
        for (int i = 0; i < Hierarchy.Length; i++)
        {
            if (Hierarchy[i].type == child)
            {
                EAssetType[] rtn = new EAssetType[Hierarchy[i].parents.Length + 1];
                rtn[0] = child;
                if (Hierarchy[i].parents.Length != 0)
                    Array.Copy(Hierarchy[i].parents, 0, rtn, 1, Hierarchy[i].parents.Length);
                category = Hierarchy[i].category;
                return rtn;
            }
        }
        category = EAssetCategory.NONE;
        return _unknownAssetTypeArray;
    }
    public static EUseableType GetUseableType(string inType)
    {
        if (Enum.TryParse(inType, true, out EUseableType useableType))
            return useableType;
        return EUseableType.UNKNOWN;
    }
}

public enum EAssetType : long
{
    UNKNOWN,
    Hat,
    Pants,
    Shirt,
    Backpack,
    Vest,
    Mask,
    Glasses,
    Gun,
    Sight,
    Tactical,
    Grip,
    Barrel,
    Magazine,
    Food,
    Water,
    Medical,
    Melee,
    Fuel,
    Tool,
    Vehicle_Repair_Tool,
    Barricade,
    Storage,
    Tank,
    Generator,
    Beacon,
    Farm,
    Trap,
    Structure,
    Supply,
    Throwable,
    Grower,
    Optic,
    Refill,
    Fisher,
    Cloud,
    Map,
    Compass,
    Key,
    Box,
    Arrest_Start,
    Arrest_End,
    Detonator,
    Charge,
    Library,
    Filter,
    Sentry,
    Tire,
    Oil_Pump,
    Effect,
    Large,
    Medium,
    Small,
    NPC,
    Decal,
    Resource,
    Vehicle,
    Animal,
    Mythic,
    Skin,
    Spawn,
    Dialogue,
    Quest,
    Vendor
}
public enum EUseableType
{
    UNKNOWN,
    Barricade,
    Battery_Vehicle,
    Carjack,
    Clothing,
    Consumeable,
    Fisher,
    Fuel,
    Grower,
    Gun,
    Melee,
    Optic,
    Refill,
    Structure,
    Throwable,
    Tire,
    Cloud,
    Arrest_Start,
    Arrest_End,
    Detonator,
    Filter,
    Carlockpick,
    Walkie_Talkie
}

public enum ESlotType
{
    NONE,
    PRIMARY,
    SECONDARY,
    TERTIARY,
    ANY
}

public enum EItemRarity
{
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGENDARY,
    MYTHICAL
}

public enum EAssetCategory
{
    NONE,
    ITEM,
    EFFECT,
    OBJECT,
    RESOURCE,
    VEHICLE,
    ANIMAL,
    MYTHIC,
    SKIN,
    SPAWN,
    NPC
}

public struct AssetTypeHierarchy
{
    public EAssetCategory category;
    public EAssetType type;
    public EAssetType[] parents;
    public AssetTypeHierarchy(EAssetCategory category, EAssetType type, params EAssetType[] parents)
    {
        this.category = category;
        this.type = type;
        this.parents = parents;
    }
}
