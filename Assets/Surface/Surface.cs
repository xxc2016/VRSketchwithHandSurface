using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace NURBS
{
    // public class SurfaceCP
    // {
    //     public CP cp;
    //     public SurfaceCP prevX = null;
    //     public SurfaceCP nextX = null;
    //     public SurfaceCP prevY = null;
    //     public SurfaceCP nextY = null;
    //
    //     public SurfaceCP(CP cp) => this.cp = cp;
    //
    //     public void UpdateCP(CP cp) => this.cp = cp;
    // }

    public class Surface : System.IDisposable
    {
        // private List<SurfaceCP> sCps;
        public NativeArray<CP> cps;

        public int orderX, orderY, lX, lY;

        public bool loopX, loopY;

        private int Idx(int x, int y) => x + y * lX;
        private int Idx(int x, int y, int lX) => x + y * lX;

        public Surface(List<CP> originalCps, int orderX, int orderY, bool loopX, bool loopY, int lX, int lY)
        {
            this.orderX = orderX;
            this.orderY = orderY;
            this.loopX = loopX;
            this.loopY = loopY;
            this.lX = lX + (loopX ? orderX * 2 : 0);
            this.lY = lY + (loopY ? orderY * 2 : 0);
            cps = new NativeArray<CP>(this.lX * this.lY, Allocator.Persistent);
            for (var y = 0; y < lY; y++)
            for (var x = 0; x < lX; x++)
            {
                cps[Idx(x, y)] = originalCps[Idx(x, y, lX)];
            }

            if (loopX)
            {
                for (var y = 0; y < lY; y++)
                for (var xi = 0; xi < orderX * 2; xi++)
                {
                    cps[Idx(lX + xi, y)] = originalCps[Idx(xi, y, lX)];
                }
            }

            if (loopY)
            {
                for (var yi = 0; yi < orderY * 2; yi++)
                for (var x = 0; x < this.lX; x++)
                {
                    cps[Idx(x, lY + yi)] = cps[Idx(x, yi)];
                }
            }
        }

        public void UpdateCP(Vector2Int i, CP cp)
        {
            // sCps[Idx(i.x, i.y)].UpdateCP(cp);
            cps[Idx(i.x, i.y)] = cp;
            if (loopX)
            {
                if (i.x < orderX * 2)
                {
                    cps[Idx(i.x + lX - orderX * 2, i.y)] = cp;
                }
            }

            if (loopY)
            {
                if (i.y < orderY * 2)
                {
                    cps[Idx(i.x, i.y + lY - orderY * 2)] = cp;
                }
            }

            if (loopX && loopY)
            {
                if (i.x < orderX * 2 && i.y < orderY * 2)
                {
                    cps[Idx(i.x + lX - orderX * 2, i.y + lY - orderY * 2)] = cp;
                }
            }
        }


        // public Vector3 GetCurve(float tx, float ty) => NURBSSurface.GetCurve(cps, tx, ty, order, olX, olY);
        public Vector3 GetCurve(float tx, float ty) =>
            NURBSSurface.GetCurve(cps, tx, ty, orderX, orderY, lX, lY);

        public bool IsAccessible => cps.IsCreated;
        public void Dispose() => cps.Dispose();
    }

    public static class NURBSSurface
    {
        private static int Idx(int x, int y, int lX) => x + y * lX;

        public static Vector3 GetCurve(NativeArray<CP> cps, float tx, float ty, int orderX, int orderY, int lX, int lY)
        {
            var frac = Vector3.zero;
            var deno = 0f;
            var nlX = lX + 2 * orderX;
            var nlY = lY + 2 * orderY;
            tx = Mathf.Min(tx, 1f - 1e-5f);
            ty = Mathf.Min(ty, 1f - 1e-5f);
            CP cp;
            for (var x = 0; x < orderX; x++)
            {
                for (var y = 0; y < orderY; y++)
                {
                    cp = cps[Idx(0, 0, lX)];

                    var bf = BasisFunc(x, orderX, orderX, tx, nlX) * BasisFunc(y, orderY, orderY, ty, nlY);
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }

                for (var y = orderY; y < orderY + lY; y++)
                {
                    cp = cps[Idx(0, y - orderY, lX)];

                    var bf = BasisFunc(x, orderX, orderX, tx, nlX) * BasisFunc(y, orderY, orderY, ty, nlY);
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }

                for (var y = orderY + lY; y < 2 * orderY + lY; y++)
                {
                    cp = cps[Idx(0, lY - 1, lX)];

                    var bf = BasisFunc(x, orderX, orderX, tx, nlX) * BasisFunc(y, orderY, orderY, ty, nlY);
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }
            }

            for (var x = orderX; x < orderX + lX; x++)
            {
                for (var y = 0; y < orderY; y++)
                {
                    cp = cps[Idx(x - orderX, 0, lX)];

                    var bf = BasisFunc(x, orderX, orderX, tx, nlX) * BasisFunc(y, orderY, orderY, ty, nlY);
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }

                for (var y = orderY; y < orderY + lY; y++)
                {
                    cp = cps[Idx(x - orderX, y - orderY, lX)];
                    var bf = BasisFunc(x, orderX, orderX, tx, nlX) * BasisFunc(y, orderY, orderY, ty, nlY);
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }

                for (var y = orderY + lY; y < 2 * orderY + lY; y++)
                {
                    cp = cps[Idx(x - orderX, lY - 1, lX)];

                    var bf = BasisFunc(x, orderX, orderX, tx, nlX) * BasisFunc(y, orderY, orderY, ty, nlY);
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }
            }

            for (var x = orderX + lX; x < 2 * orderX + lX; x++)
            {
                for (var y = 0; y < orderY; y++)
                {
                    cp = cps[Idx(lX - 1, 0, lX)];

                    var bf = BasisFunc(x, orderX, orderX, tx, nlX) * BasisFunc(y, orderY, orderY, ty, nlY);
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }

                for (var y = orderY; y < orderY + lY; y++)
                {
                    cp = cps[Idx(lX - 1, y - orderY, lX)];

                    var bf = BasisFunc(x, orderX, orderX, tx, nlX) * BasisFunc(y, orderY, orderY, ty, nlY);
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }

                for (var y = orderY + lY; y < 2 * orderY + lY; y++)
                {
                    cp = cps[Idx(lX - 1, lY - 1, lX)];

                    var bf = BasisFunc(x, orderX, orderX, tx, nlX) * BasisFunc(y, orderY, orderY, ty, nlY);
                    frac += cp.pos * bf * cp.weight;
                    deno += bf * cp.weight;
                }
            }

            return frac / deno;
        }

        private static float BasisFunc(int j, int k, int order, float t, int l)
        {
            if (k == 0)
            {
                return (t >= KnotVector(j, order, l) && t < KnotVector(j + 1, order, l)) ? 1 : 0;
            }
            else
            {
                var d1 = KnotVector(j + k, order, l) - KnotVector(j, order, l);
                var d2 = KnotVector(j + k + 1, order, l) - KnotVector(j + 1, order, l);
                var c1 = d1 != 0 ? (t - KnotVector(j, order, l)) / d1 : 0;
                var c2 = d2 != 0 ? (KnotVector(j + k + 1, order, l) - t) / d2 : 0;
                return c1 * BasisFunc(j, k - 1, order, t, l) + c2 * BasisFunc(j + 1, k - 1, order, t, l);
            }
        }

        /// <summary>
        /// open uniform knot vector
        /// </summary>
        private static float KnotVector(int j, int order, int len)
        {
            if (j < order) return 0;
            if (j > len - order) return 1;
            return Mathf.Clamp01((j - order) / (float) (len - 2 * order));
        }
    }
}