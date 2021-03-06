﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FilterExtensions.Utility
{
    using ModuleWheels;
    
    public static class PartType
    {
        /// <summary>
        /// check the part against another subcategory. Hard limited to a depth of 10
        /// </summary>
        public static bool checkSubcategory(AvailablePart part, string[] value, int depth)
        {
            if (depth > 10)
                return false;
            foreach (string s in value)
            {
                FilterExtensions.ConfigNodes.customSubCategory subcategory;
                if (Core.Instance.subCategoriesDict.TryGetValue(s, out subcategory) && subcategory.checkFilters(part, depth + 1))
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// steamlined/combined checks on parts, or checks that don't need extra options
        /// </summary>
        public static bool checkCustom(AvailablePart part, string[] value)
        {
            bool testVal = false;
            foreach (string s in value)
            {
                switch (s)
                {
                    case "adapter":
                        testVal = isAdapter(part);
                        break;
                    case "multicoupler":
                        testVal = isMultiCoupler(part);
                        break;
                    case "purchased":
                        testVal = !Editor.instance.ready || ResearchAndDevelopment.PartModelPurchased(part);
                        break;
                }
                if (testVal)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// checks the stock part category
        /// </summary>
        public static bool checkCategory(AvailablePart part, string[] value)
        {
            switch (part.category)
            {
                case PartCategories.Pods:
                    if (value.Contains("Pods", StringComparer.OrdinalIgnoreCase))
                        return true;
                    break;
                case PartCategories.Propulsion:
                    if (value.Contains("Engines", StringComparer.OrdinalIgnoreCase) && isEngine(part))
                        return true;
                    if (value.Contains("Fuel Tanks", StringComparer.OrdinalIgnoreCase) && !isEngine(part))
                        return true;
                    break;
                case PartCategories.Engine:
                    if (value.Contains("Engines", StringComparer.OrdinalIgnoreCase))
                        return true;
                    break;
                case PartCategories.FuelTank:
                    if (value.Contains("Fuel Tanks", StringComparer.OrdinalIgnoreCase))
                        return true;
                    break;
                case PartCategories.Control:
                    if (value.Contains("Control", StringComparer.OrdinalIgnoreCase))
                        return true;
                    break;
                case PartCategories.Structural:
                    if (value.Contains("Structural", StringComparer.OrdinalIgnoreCase))
                        return true;
                    break;
                case PartCategories.Aero:
                    if (value.Contains("Aerodynamics", StringComparer.OrdinalIgnoreCase))
                        return true;
                    break;
                case PartCategories.Utility:
                    if (value.Contains("Utility", StringComparer.OrdinalIgnoreCase))
                        return true;
                    break;
                case PartCategories.Science:
                    if (value.Contains("Science", StringComparer.OrdinalIgnoreCase))
                        return true;
                    break;
                case PartCategories.none:
                    if (value.Contains("None", StringComparer.OrdinalIgnoreCase))
                        return true;
                    break;
            }
            return false;
        }


        
        /// <summary>
        /// check the user visible names of each part module against a string list
        /// </summary>
        public static bool checkModuleTitle(AvailablePart part, string[] values, bool contains = true)
        {
            if (part.moduleInfos == null)
                return false;

            foreach (AvailablePart.ModuleInfo mi in part.moduleInfos)
            {
                if (contains == values.Contains(mi.moduleName, StringComparer.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// check the part module type against a string list
        /// </summary>
        public static bool checkModuleName(AvailablePart part, string[] value, bool contains = true)
        {
            if (part.partPrefab == null || part.partPrefab.Modules == null)
                return false;

            if (contains)
                return value.Any(s => checkModuleNameType(part, s) || part.partPrefab.Modules.Contains(s));
            else
                return value.Any(s => !checkModuleNameType(part, s) && !part.partPrefab.Modules.Contains(s));
        }
        
        /// <summary>
        /// provides a typed check for stock modules which then allows for inheritance to work
        /// </summary>
        public static bool checkModuleNameType(AvailablePart part, string value)
        {
            switch (value)
            {
                case "ModuleAblator":
                    return part.partPrefab.Modules.Contains<ModuleAblator>();
                case "ModuleActiveRadiator":
                    return part.partPrefab.Modules.Contains<ModuleActiveRadiator>();
                case "ModuleAerodynamicLift":
                    return part.partPrefab.Modules.Contains<ModuleAerodynamicLift>();
                case "ModuleAeroSurface":
                    return part.partPrefab.Modules.Contains<ModuleAeroSurface>();
                case "ModuleAlternator":
                    return part.partPrefab.Modules.Contains<ModuleAlternator>();
                case "ModuleAnalysisResource":
                    return part.partPrefab.Modules.Contains<ModuleAnalysisResource>();
                case "ModuleAnchoredDecoupler":
                    return part.partPrefab.Modules.Contains<ModuleAnchoredDecoupler>();
                case "ModuleAnimateGeneric":
                    return part.partPrefab.Modules.Contains<ModuleAnimateGeneric>();
                case "ModuleAnimateHeat":
                    return part.partPrefab.Modules.Contains<ModuleAnimateHeat>();
                case "ModuleAnimationGroup":
                    return part.partPrefab.Modules.Contains<ModuleAnimationGroup>();
                case "ModuleAnimatorLandingGear":
                    return part.partPrefab.Modules.Contains<ModuleAnimatorLandingGear>();
                case "ModuleAsteroid":
                    return part.partPrefab.Modules.Contains<ModuleAsteroid>();
                case "ModuleAsteroidAnalysis":
                    return part.partPrefab.Modules.Contains<ModuleAsteroidAnalysis>();
                case "ModuleAsteroidDrill":
                    return part.partPrefab.Modules.Contains<ModuleAsteroidDrill>();
                case "ModuleAsteroidInfo":
                    return part.partPrefab.Modules.Contains<ModuleAsteroidInfo>();
                case "ModuleAsteroidResource":
                    return part.partPrefab.Modules.Contains<ModuleAsteroidResource>();
                case "ModuleBiomeScanner":
                    return part.partPrefab.Modules.Contains<ModuleBiomeScanner>();
                case "ModuleCargoBay":
                    return part.partPrefab.Modules.Contains<ModuleCargoBay>();
                case "ModuleCommand":
                    return part.partPrefab.Modules.Contains<ModuleCommand>();
                case "ModuleConductionMultiplier":
                    return part.partPrefab.Modules.Contains<ModuleConductionMultiplier>();
                case "ModuleControlSurface":
                    return part.partPrefab.Modules.Contains<ModuleControlSurface>();
                case "ModuleCoreHeat":
                    return part.partPrefab.Modules.Contains<ModuleCoreHeat>();
                case "ModuleDataTransmitter":
                    return part.partPrefab.Modules.Contains<ModuleDataTransmitter>();
                case "ModuleDecouple":
                    return part.partPrefab.Modules.Contains<ModuleDecouple>();
                case "ModuleDeployableRadiator":
                    return part.partPrefab.Modules.Contains<ModuleDeployableRadiator>();
                case "ModuleDeployableSolarPanel":
                    return part.partPrefab.Modules.Contains<ModuleDeployableSolarPanel>();
                case "ModuleDisplaceTweak":
                    return part.partPrefab.Modules.Contains<ModuleDisplaceTweak>();
                case "ModuleDockingNode":
                    return part.partPrefab.Modules.Contains<ModuleDockingNode>();
                case "ModuleDragModifier":
                    return part.partPrefab.Modules.Contains<ModuleDragModifier>();
                case "ModuleEffectTest":
                    return part.partPrefab.Modules.Contains<ModuleEffectTest>();
                case "ModuleEngines":
                    return part.partPrefab.Modules.Contains<ModuleEngines>();
                case "ModuleEnginesFX":
                    return part.partPrefab.Modules.Contains<ModuleEnginesFX>();
                case "ModuleEnviroSensor":
                    return part.partPrefab.Modules.Contains<ModuleEnviroSensor>();
                case "ModuleFuelJettison":
                    return part.partPrefab.Modules.Contains<ModuleFuelJettison>();
                case "ModuleGenerator":
                    return part.partPrefab.Modules.Contains<ModuleGenerator>();
                case "ModuleGimbal":
                    return part.partPrefab.Modules.Contains<ModuleGimbal>();
                case "ModuleGPS":
                    return part.partPrefab.Modules.Contains<ModuleGPS>();
                case "ModuleGrappleNode":
                    return part.partPrefab.Modules.Contains<ModuleGrappleNode>();
                case "ModuleHighDefCamera":
                    return part.partPrefab.Modules.Contains<ModuleHighDefCamera>();
                case "ModuleJettison":
                    return part.partPrefab.Modules.Contains<ModuleJettison>();
                case "ModuleJointMotor":
                    return part.partPrefab.Modules.Contains<ModuleJointMotor>();
                case "ModuleJointMotorTest":
                    return part.partPrefab.Modules.Contains<ModuleJointMotorTest>();
                case "ModuleJointPivot":
                    return part.partPrefab.Modules.Contains<ModuleJointPivot>();
                case "ModuleLiftingSurface":
                    return part.partPrefab.Modules.Contains<ModuleLiftingSurface>();
                case "ModuleLight":
                    return part.partPrefab.Modules.Contains<ModuleLight>();
                case "ModuleOrbitalScanner":
                    return part.partPrefab.Modules.Contains<ModuleOrbitalScanner>();
                case "ModuleOrbitalSurveyor":
                    return part.partPrefab.Modules.Contains<ModuleOrbitalSurveyor>();
                case "ModuleOverheatDisplay":
                    return part.partPrefab.Modules.Contains<ModuleOverheatDisplay>();
                case "ModuleParachute":
                    return part.partPrefab.Modules.Contains<ModuleParachute>();
                case "ModulePhysicMaterial":
                    return part.partPrefab.Modules.Contains<ModulePhysicMaterial>();
                case "ModuleProceduralFairing":
                    return part.partPrefab.Modules.Contains<ModuleProceduralFairing>();
                case "ModuleRCS":
                    return part.partPrefab.Modules.Contains<ModuleRCS>();
                case "ModuleReactionWheel":
                    return part.partPrefab.Modules.Contains<ModuleReactionWheel>();
                case "ModuleRemoteController":
                    return part.partPrefab.Modules.Contains<ModuleRemoteController>();
                case "ModuleResourceConverter":
                    return part.partPrefab.Modules.Contains<ModuleResourceConverter>();
                case "ModuleResourceHarvester":
                    return part.partPrefab.Modules.Contains<ModuleResourceHarvester>();
                case "ModuleResourceIntake":
                    return part.partPrefab.Modules.Contains<ModuleResourceIntake>();
                case "ModuleResourceScanner":
                    return part.partPrefab.Modules.Contains<ModuleResourceScanner>();
                case "ModuleRotatingJoint":
                    return part.partPrefab.Modules.Contains<ModuleRotatingJoint>();
                case "ModuleSampleCollector":
                    return part.partPrefab.Modules.Contains<ModuleSampleCollector>();
                case "ModuleSampleContainer":
                    return part.partPrefab.Modules.Contains<ModuleSampleContainer>();
                case "ModuleSAS":
                    return part.partPrefab.Modules.Contains<ModuleSAS>();
                case "ModuleScienceContainer":
                    return part.partPrefab.Modules.Contains<ModuleScienceContainer>();
                case "ModuleScienceConverter":
                    return part.partPrefab.Modules.Contains<ModuleScienceConverter>();
                case "ModuleScienceExperiment":
                    return part.partPrefab.Modules.Contains<ModuleScienceExperiment>();
                case "ModuleScienceLab":
                    return part.partPrefab.Modules.Contains<ModuleScienceLab>();
                case "ModuleSeeThroughObject":
                    return part.partPrefab.Modules.Contains<ModuleSeeThroughObject>();
                case "ModuleStatusLight":
                    return part.partPrefab.Modules.Contains<ModuleStatusLight>();
                case "ModuleSurfaceFX":
                    return part.partPrefab.Modules.Contains<ModuleSurfaceFX>();
                case "ModuleTestSubject":
                    return part.partPrefab.Modules.Contains<ModuleTestSubject>();
                case "ModuleToggleCrossfeed":
                    return part.partPrefab.Modules.Contains<ModuleToggleCrossfeed>();
                case "ModuleTripLogger":
                    return part.partPrefab.Modules.Contains<ModuleTripLogger>();
                case "ModuleWheelBase":
                    return part.partPrefab.Modules.Contains<ModuleWheelBase>();
                case "FXModuleAnimateThrottle":
                    return part.partPrefab.Modules.Contains<FXModuleAnimateThrottle>();
                case "FXModuleConstrainPosition":
                    return part.partPrefab.Modules.Contains<FXModuleConstrainPosition>();
                case "FXModuleLookAtConstraint":
                    return part.partPrefab.Modules.Contains<FXModuleLookAtConstraint>();
                case "ModuleWheelBogey":
                    return part.partPrefab.Modules.Contains<ModuleWheelBogey>();
                case "ModuleWheelBrakes":
                    return part.partPrefab.Modules.Contains<ModuleWheelBrakes>();
                case "ModuleWheelDamage":
                    return part.partPrefab.Modules.Contains<ModuleWheelDamage>();
                case "ModuleWheelDeployment":
                    return part.partPrefab.Modules.Contains<ModuleWheelDeployment>();
                case "ModuleWheelLock":
                    return part.partPrefab.Modules.Contains<ModuleWheelLock>();
                case "ModuleWheelMotor":
                    return part.partPrefab.Modules.Contains<ModuleWheelMotor>();
                case "ModuleWheelMotorSteering":
                    return part.partPrefab.Modules.Contains<ModuleWheelMotorSteering>();
                case "ModuleWheelSteering":
                    return part.partPrefab.Modules.Contains<ModuleWheelSteering>();
                case "ModuleWheelSubmodule":
                    return part.partPrefab.Modules.Contains<ModuleWheelSubmodule>();
                case "ModuleWheelSuspension":
                    return part.partPrefab.Modules.Contains<ModuleWheelSuspension>();
                default:
                    return false;
            }
        }

        /// <summary>
        /// check the part name/id exactly matches one in the list
        /// </summary>
        public static bool checkName(AvailablePart part, string[] value)
        {
            return value.Contains(part.name.Replace('.', '_'), StringComparer.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// check the user viewable part title contains any of the listed values for a partial match
        /// </summary>
        public static bool checkTitle(AvailablePart part, string[] value)
        {
            
            return value.Any(s => part.title.IndexOf(s, StringComparison.OrdinalIgnoreCase) != -1);
        }

        /// <summary>
        /// check the resources the part holds
        /// </summary>
        public static bool checkResource(AvailablePart part, string[] value, bool contains = true)
        {
            if (part.partPrefab == null || part.partPrefab.Resources == null)
                return false;

            foreach (PartResource r in part.partPrefab.Resources)
            {
                if (r.maxAmount > 0 && contains == value.Contains(r.resourceName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// check the propellants this engine uses
        /// </summary>
        public static bool checkPropellant(AvailablePart part, string[] value, bool contains = true)
        {
            foreach (ModuleEngines e in part.partPrefab.Modules.GetModules<ModuleEngines>())
            {
                foreach (Propellant p in e.propellants)
                {
                    if (contains == value.Contains(p.name))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// check the tech required to unlock the part outside sandbox
        /// </summary>
        public static bool checkTech(AvailablePart part, string[] value)
        {
            return value.Contains(part.TechRequired);
        }

        /// <summary>
        /// check against the manufacturer of the part
        /// </summary>
        public static bool checkManufacturer(AvailablePart part, string[] value)
        {
            return value.Contains(part.manufacturer);
        }

        /// <summary>
        /// checks against the root GameData folder name for a part.
        /// </summary>
        public static bool checkFolder(AvailablePart part, string[] value)
        {
            string path;
            if (Core.Instance.partPathDict.TryGetValue(part.name, out path))
                return value.Contains(path.Substring(0, path.IndexOfAny(new char[] { '\\', '/' })));
            return false;
        }

        /// <summary>
        /// check against the full path from GameData to the part. eg Squad/Parts
        /// </summary>
        public static bool checkPath(AvailablePart part, string[] value)
        {
            string path;
            if (Core.Instance.partPathDict.TryGetValue(part.name, out path))
                return value.Any(s => path.StartsWith(s, StringComparison.OrdinalIgnoreCase));

            return false;
        }

        /// <summary>
        /// checks against the attach node sizes on the part
        /// </summary>
        public static bool checkPartSize(AvailablePart part, string[] value, bool contains, ConfigNodes.Check.Equality equality)
        {
            if (part.partPrefab == null || part.partPrefab.attachNodes == null)
                return false;

            if (equality == ConfigNodes.Check.Equality.Equals)
            {
                foreach (AttachNode node in part.partPrefab.attachNodes)
                {
                    if (contains)
                    {
                        if (value.Contains(node.size.ToString(), StringComparer.OrdinalIgnoreCase))
                            return true;
                    }
                    else
                    {
                        if (!value.Contains(node.size.ToString(), StringComparer.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
            else // only compare against the first value here
            {
                if (value.Length > 1)
                    Core.Log("Size comparisons against multiple values when not using Equals only use the first value. Value list is: {0}", string.Join(", ", value));

                int i;
                if (int.TryParse(value[0], out i))
                {
                    if (equality == ConfigNodes.Check.Equality.GreaterThan)
                    {
                        part.partPrefab.attachNodes.Any(n => n.size > i);
                            return true;
                    }
                    else if (equality == ConfigNodes.Check.Equality.LessThan)
                    {
                        part.partPrefab.attachNodes.Any(n => n.size < i);
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// check against the number of crew this part can hold
        /// </summary>
        public static bool checkCrewCapacity(AvailablePart part, string[] value, ConfigNodes.Check.Equality equality)
        {
            if (part.partPrefab == null)
                return false;

            if (equality == ConfigNodes.Check.Equality.Equals)
                return value.Contains(part.partPrefab.CrewCapacity.ToString(), StringComparer.OrdinalIgnoreCase);
            else // only compare against the first value here
            {
                if (value.Length > 1)
                    Core.Log("Crew comparisons against multiple values when not using Equals only use the first value. Value list is: {0}", string.Join(", ", value));

                double d;
                if (double.TryParse(value[0], out d))
                {
                    if (equality == ConfigNodes.Check.Equality.GreaterThan && part.partPrefab.CrewCapacity > d)
                        return true;
                    else if (equality == ConfigNodes.Check.Equality.LessThan && part.partPrefab.CrewCapacity < d)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// check the part mass against a list of values
        /// </summary>
        public static bool checkMass(AvailablePart part, string[] value, ConfigNodes.Check.Equality equality)
        {
            if (part.partPrefab == null)
                return false;

            if (equality == ConfigNodes.Check.Equality.Equals)
                return value.Contains(part.partPrefab.mass.ToString(), StringComparer.OrdinalIgnoreCase);
            else
            {
                if (value.Length > 1)
                    Core.Log("Mass comparisons against multiple values when not using Equals only use the first value. Value list is: {0}", string.Join(", ", value));

                double d;
                if (double.TryParse(value[0], out d))
                {
                    if (equality == ConfigNodes.Check.Equality.GreaterThan && part.partPrefab.mass > d)
                        return true;
                    else if (equality == ConfigNodes.Check.Equality.LessThan && part.partPrefab.mass < d)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// check the part cost against a string list
        /// </summary>
        public static bool checkCost(AvailablePart part, string[] value, ConfigNodes.Check.Equality equality)
        {
            if (equality == ConfigNodes.Check.Equality.Equals)
                return value.Contains(part.cost.ToString(), StringComparer.OrdinalIgnoreCase);
            else
            {
                if (value.Length > 1)
                    Core.Log("Cost comparisons against multiple values when not using Equals only use the first value. Value list is: {0}", string.Join(", ", value));

                double d;
                if (double.TryParse(value[0], out d))
                {
                    if (equality == ConfigNodes.Check.Equality.GreaterThan && part.cost > d)
                        return true;
                    else if (equality == ConfigNodes.Check.Equality.LessThan && part.cost < d)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// check the impact speed at which the part will explode
        /// </summary>
        public static bool checkCrashTolerance(AvailablePart part, string[] value, ConfigNodes.Check.Equality equality)
        {
            if (part.partPrefab == null)
                return false;

            if (equality == ConfigNodes.Check.Equality.Equals)
                return value.Contains(part.partPrefab.crashTolerance.ToString());
            else
            {
                if (value.Length > 1)
                    Core.Log("Crash tolerance comparisons against multiple values when not using Equals only use the first value. Value list is: {0}", string.Join(", ", value));

                float f;
                if (float.TryParse(value[0], out f))
                {
                    if (equality == ConfigNodes.Check.Equality.GreaterThan && part.partPrefab.crashTolerance > f)
                        return true;
                    else if (equality == ConfigNodes.Check.Equality.LessThan && part.partPrefab.crashTolerance < f)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// compares against the part max temp
        /// </summary>
        public static bool checkTemperature(AvailablePart part, string[] value, ConfigNodes.Check.Equality equality)
        {
            if (part.partPrefab == null)
                return false;

            if (equality == ConfigNodes.Check.Equality.Equals)
                return value.Contains(part.partPrefab.maxTemp.ToString(), StringComparer.OrdinalIgnoreCase);
            else
            {
                if (value.Length > 1)
                    Core.Log("Temperature comparisons against multiple values when not using Equals only use the first value. Value list is: {0}", string.Join(", ", value));
                double d;
                if (double.TryParse(value[0], out d))
                {
                    if (equality == ConfigNodes.Check.Equality.GreaterThan && part.partPrefab.maxTemp > d)
                        return true;
                    else if (equality == ConfigNodes.Check.Equality.LessThan && part.partPrefab.maxTemp < d)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// bulkhead profiles used to id part shapes for stock editor. parts with no profiles get dumped in srf
        /// </summary>
        public static bool checkBulkHeadProfiles(AvailablePart part, string[] value, bool contains)
        {
            if (part.bulkheadProfiles == null)
                return value.Contains("srf");
            
            foreach (string s in part.bulkheadProfiles.Split(',').Select(s => s.Trim()))
            {
                if (contains == value.Contains(s, StringComparer.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static bool checkTags(AvailablePart part, string[] value, bool contains)
        {
            if (string.IsNullOrEmpty(part.tags))
                return false;

            foreach (string s in part.tags.Split(new char[4] { ' ', ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray())
            {
                if (contains == value.Contains(s))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// checks if the part can be used to control a vessel
        /// </summary>
        public static bool isCommand(AvailablePart part)
        {
            return isMannedPod(part) || isDrone(part) || part.partPrefab.Modules.Contains<KerbalSeat>();
        }
        
        /// <summary>
        /// checks if the part is an engine
        /// </summary>
        public static bool isEngine(AvailablePart part)
        {
            return part.partPrefab.Modules.Contains<ModuleEngines>();
        }

        /// <summary>
        /// checks if the part can be used to control a vessel and holds crew
        /// </summary>
        public static bool isMannedPod(AvailablePart part)
        {
            return part.partPrefab.Modules.Contains<ModuleCommand>() && part.partPrefab.CrewCapacity > 0;
        }

        /// <summary>
        /// checks if the part can be used to control a vessel and doesn't hold crew
        /// </summary>
        public static bool isDrone(AvailablePart part)
        {
            return part.partPrefab.Modules.Contains<ModuleCommand>() && part.partPrefab.CrewCapacity == 0;
        }

        /// <summary>
        /// checks if the part has multiple bottom attach nodes
        /// </summary>
        public static bool isMultiCoupler(AvailablePart part)
        {
            if (part.partPrefab == null || part.partPrefab.attachNodes == null)
                return false;

            if (part.partPrefab.attachNodes.Count <= 2 || part.title.Contains("Cargo Bay"))
                return false;
            float pos = part.partPrefab.attachNodes.Last().position.y;
            if (part.partPrefab.attachNodes.FindAll(n => n.position.y == pos).Count > 1 && part.partPrefab.attachNodes.FindAll(n => n.position.y == pos).Count < part.partPrefab.attachNodes.Count)
                return true;
            
            return false;
        }

        /// <summary>
        /// checks if the part has two attach nodes and they are different sizes
        /// </summary>
        public static bool isAdapter(AvailablePart part)
        {
            if (part.partPrefab == null || part.partPrefab.attachNodes == null || part.partPrefab.attachNodes.Count != 2 || isCommand(part))
                return false;
            return part.partPrefab.attachNodes[0].size != part.partPrefab.attachNodes[1].size;
        }
    }
}
