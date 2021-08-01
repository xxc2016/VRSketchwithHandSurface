using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NURBS
{
    public static class RamerDouglasPeucker
    {
        public static List<CP> Reduce(List<CP> cps, float tolerance)
        {
            if (cps == null || cps.Count < 3) return cps;
            tolerance *= tolerance;
            if (tolerance <= float.Epsilon) return cps;

            var firstIdx = 0;
            var lastIdx = cps.Count - 1;
            var idx2Keep = new List<int> {firstIdx, lastIdx};

            while (cps[firstIdx].pos.Equals(cps[lastIdx].pos))
            {
                lastIdx--;
            }

            Reduce(cps, tolerance, firstIdx, lastIdx, ref idx2Keep);

            idx2Keep.Sort();

            return idx2Keep.Select(i => cps[i]).ToList();
        }

        private static void Reduce(List<CP> cps, float squareTolerance, int firstIdx, int lastIdx,
            ref List<int> idx2Keep)
        {
            var maxDistance = 0f;
            var idx = 0;
            for (var i = firstIdx; i < lastIdx; i++)
            {
                var distance = SquarePerpendicularDistance(cps[firstIdx], cps[lastIdx], cps[i]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    idx = i;
                }
            }

            if (maxDistance > squareTolerance && idx != 0)
            {
                idx2Keep.Add(idx);
                Reduce(cps, squareTolerance, firstIdx, idx, ref idx2Keep);
                Reduce(cps, squareTolerance, idx, lastIdx, ref idx2Keep);
            }
        }

        private static float SquarePerpendicularDistance(CP first, CP last, CP cp)
        {
            // 16S^2 = 4AB - (C - A - B)^2
            // S = .5 * vA * h
            // h^2 = 4S^2 / A
            var v1 = first.pos;
            var v2 = last.pos;
            var v3 = cp.pos;
            var A = Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2) + Mathf.Pow(v1.z - v2.z, 2);
            var B = Mathf.Pow(v1.x - v3.x, 2) + Mathf.Pow(v1.y - v3.y, 2) + Mathf.Pow(v1.z - v3.z, 2);
            if (A == 0) return B;
            var C = Mathf.Pow(v3.x - v2.x, 2) + Mathf.Pow(v3.y - v2.y, 2) + Mathf.Pow(v3.z - v2.z, 2);
            var squareArea = (4 * A * B - Mathf.Pow(C - A - B, 2)) / 4f;
            return squareArea / A;
        }
    }
}