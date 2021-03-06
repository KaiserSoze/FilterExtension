﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FilterExtensions.ConfigNodes
{
    public class Filter : IEquatable<Filter>, ICloneable
    {
        public List<Check> checks { get; set; } // checks are processed in serial (a && b), inversion gives (!a || !b) logic
        public bool invert { get; set; }

        public Filter(ConfigNode node)
        {
            checks = new List<Check>();
            foreach (ConfigNode subNode in node.GetNodes("CHECK"))
            {
                checks.Add(new Check(subNode));
            }
            checks.RemoveAll(c => c.isEmpty());

            bool tmp;
            bool.TryParse(node.GetValue("invert"), out tmp);
            invert = tmp;
        }

        public Filter(Filter f)
        {
            checks = new List<Check>();
            for (int i = 0; i < f.checks.Count; i++)
            {
                if (!f.checks[i].isEmpty())
                    checks.Add(new Check(f.checks[i]));
            }

            invert = f.invert;
        }

        public Filter(bool invert)
        {
            checks = new List<Check>();
            this.invert = invert;
        }

        public ConfigNode toConfigNode()
        {
            ConfigNode node = new ConfigNode("FILTER");
            node.AddValue("invert", this.invert.ToString());
            foreach (Check c in checks)
                node.AddNode(c.toConfigNode());

            return node;
        }

        public object Clone()
        {
            return new Filter(this);
        }

        internal bool checkFilter(AvailablePart part, int depth = 0)
        {
            return invert ? !checks.All(c => c.checkPart(part, depth)) : checks.All(c => c.checkPart(part, depth));
        }

        /// <summary>
        /// compare subcategory filter lists, returning true for matches
        /// </summary>
        /// <param name="fLA"></param>
        /// <param name="fLB"></param>
        /// <returns></returns>
        public static bool compareFilterLists(List<Filter> fLA, List<Filter> fLB)
        {
            if (fLA.Count != fLB.Count && fLA.Count != 0)
                return false;

            return fLA.All(f => fLB.Contains(f));
        }

        public bool Equals(Filter f2)
        {
            if (f2 == null)
                return false;

            if (invert != f2.invert)
                return false;
            return checks.All(c => f2.checks.Contains(c));
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (Check c in checks)
            {
                hash *= c.GetHashCode();
            }

            return hash ^ invert.GetHashCode();
        }
    }
}
