using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UAM.Panels;
public class WeaponPanel : Panel
{
    public WeaponPanel() : base(EAssetCategory.ITEM, EAssetType.Melee, EAssetType.Throwable, EAssetType.Gun, EAssetType.Food, EAssetType.Water, EAssetType.Medical)
    {
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1d, GridUnitType.Star) });

        /*
         * BladeIDs <int>
         * BladeID_0 <ushort>
         * BladeID_1 <ushort>
         *  or if there's one just BladeID <ushort>
         * Range <float>
         * Player_Damage <float>
         * Player_Leg_Multiplier <float>
         * Player_Arm_Multiplier <float>
         * Player_Spine_Multiplier <float>
         * Player_Skull_Multiplier <float>
         * Player_Damage_Bleeding <EBleedingType>
         * Player_Damage_Bones <EBreakType>
         * Player_Damage_Food <float>
         * Player_Damage_Water <float>
         * Player_Damage_Virus <float>
         * Player_Damage_Hallucination <float>
         * 
         * Zombie_Damage <float>
         * Zombie_Leg_Multiplier <float>
         * Zombie_Arm_Multiplier <float>
         * Zombie_Spine_Multiplier <float>
         * Zombie_Skull_Multiplier <float>
         * 
         * Animal_Damage <float>
         * Animal_Leg_Multiplier <float>
         * Animal_Spine_Multiplier <float>
         * Animal_Skull_Multiplier <float>
         * 
         * Barricade_Damage <float>
         * Structure_Damage <float>
         * Vehicle_Damage <float>
         * Resource_Damage <float>
         * Object_Damage <float> - defaults to resource damage if not present
         * 
         * Durability <float>
         * Wear <byte>
         * Invulnerable <flag>
         * Allow_Flesh_Fx <boolean> - true if its not there or if it's there with true
         * Stun_Zombie_Always / Stun_Zombie_Never - sets EZombieStunOverride value
         * Bypass_Allowed_To_Damage_Player <boolean>
         */

        AddElementsToGrid();
    }

    public override void GetStringPairs(List<StringPair> refStringPairs, List<StringPair> refLocalStringPairs)
    {

    }
    public override void Populate(AssetFile file, bool isNewItem)
    {

    }
}