using System.Collections.Generic;
using UnityEngine;

namespace NURBS
{
    public static class Pipeline
    {
        private static SurfaceCpsData _surfaceCpsData;
        //private static GameObject _gameObject;
        private static SurfaceHandler _surfaceHandler;

        //public static void PanAndRender(List<CP> splineCps1, List<CP> splineCps2, int orderX, int orderY, bool loopX,
        //    bool loopY)
        //{
        //    _surfaceCpsData = ScriptableObject.CreateInstance<SurfaceCpsData>();
        //    _surfaceCpsData.orderX = orderX;
        //    _surfaceCpsData.orderY = orderY;
        //    _surfaceCpsData.loopX = loopX;
        //    _surfaceCpsData.loopY = loopY;
        //    _surfaceCpsData.size.x = 10;
        //    _surfaceCpsData.size.y = 10;
        //    _surfaceCpsData.count.x = splineCps1.Count;
        //    _surfaceCpsData.count.y = splineCps2.Count;
        //    _surfaceCpsData.cps = new List<CP>();
        //    for (var y = 0; y < splineCps2.Count; y++)
        //    for (var x = 0; x < splineCps1.Count; x++)
        //    {
        //        _surfaceCpsData.cps.Add(new CP(splineCps1[x].pos + splineCps2[y].pos,
        //            splineCps1[x].weight * splineCps2[y].weight));
        //    }

        //    _gameObject = new GameObject();
        //    _surfaceHandler = _gameObject.AddComponent<SurfaceHandler>();
        //    _surfaceHandler.Data = _surfaceCpsData;
        //    _surfaceHandler.division = new Vector2Int(100, 100);
        //    _surfaceHandler.Start();
        //    _surfaceHandler.OnDestroy();
        //}

        //public static void RotateAndRender(List<CP> splineCps, Vector3 axis, float rotationAngle, int orderX,
        //    int orderY, bool loopX)
        //{
        //    _surfaceCpsData = ScriptableObject.CreateInstance<SurfaceCpsData>();
        //    _surfaceCpsData.orderX = orderX;
        //    _surfaceCpsData.orderY = orderY;
        //    _surfaceCpsData.loopX = loopX;
        //    _surfaceCpsData.loopY = Mathf.Abs(Mathf.Abs(rotationAngle) - 360f) < 1e-5f;
        //    _surfaceCpsData.size.x = 10;
        //    _surfaceCpsData.size.y = 10;
        //    _surfaceCpsData.count.x = splineCps.Count;
        //    _surfaceCpsData.count.y = Mathf.CeilToInt(rotationAngle / 30f) + 1;
        //    _surfaceCpsData.cps = new List<CP>();
        //    var t = rotationAngle / (_surfaceCpsData.count.y - 1);
        //    if (_surfaceCpsData.loopY)
        //    {
        //        _surfaceCpsData.count.y = 12;
        //        t = 360f / _surfaceCpsData.count.y;
        //    }

        //    for (var y = 0; y < _surfaceCpsData.count.y; y++)
        //    for (var x = 0; x < _surfaceCpsData.count.x; x++)
        //    {
        //        var newPos = Quaternion.AngleAxis(y * t, axis) * splineCps[x].pos;
        //        _surfaceCpsData.cps.Add(new CP(newPos, splineCps[x].weight));
        //    }

        //    _gameObject = new GameObject();
        //    _surfaceHandler = _gameObject.AddComponent<SurfaceHandler>();
        //    _surfaceHandler.Data = _surfaceCpsData;
        //    _surfaceHandler.division = new Vector2Int(100, 100);
        //    _surfaceHandler.Start();
        //    _surfaceHandler.OnDestroy();
        //}

        public static void PanAndRender(out GameObject _gameObject, List<CP> splineCps1, List<CP> splineCps2, int orderX = 2, int orderY = 2, bool loopX = false,
            bool loopY = false)
        {
            _surfaceCpsData = ScriptableObject.CreateInstance<SurfaceCpsData>();
            _surfaceCpsData.orderX = orderX;
            _surfaceCpsData.orderY = orderY;
            _surfaceCpsData.loopX = loopX;
            _surfaceCpsData.loopY = loopY;
            _surfaceCpsData.size.x = 10;
            _surfaceCpsData.size.y = 10;
            _surfaceCpsData.count.x = splineCps1.Count;
            _surfaceCpsData.count.y = splineCps2.Count;
            _surfaceCpsData.cps = new List<CP>();
            for (var y = 0; y < splineCps2.Count; y++)
                for (var x = 0; x < splineCps1.Count; x++)
                {
                    _surfaceCpsData.cps.Add(new CP(splineCps1[x].pos + splineCps2[y].pos,
                        splineCps1[x].weight * splineCps2[y].weight));
                }

            _gameObject = new GameObject();
            _surfaceHandler = _gameObject.AddComponent<SurfaceHandler>();
            _surfaceHandler.Data = _surfaceCpsData;
            _surfaceHandler.division = new Vector2Int(100, 100);
            _surfaceHandler.Generate();
            //_surfaceHandler.RuinSelf();
        }

        public static void RotateAndRender(out GameObject _gameObject, List<CP> splineCps, Vector3 axis, float rotationAngle, int orderX =2 ,
            int orderY = 2, bool loopX = false)
        {
            _surfaceCpsData = ScriptableObject.CreateInstance<SurfaceCpsData>();
            _surfaceCpsData.orderX = orderX;
            _surfaceCpsData.orderY = orderY;
            _surfaceCpsData.loopX = loopX;
            _surfaceCpsData.loopY = Mathf.Abs(Mathf.Abs(rotationAngle) - 360f) < 1e-5f;
            _surfaceCpsData.size.x = 10;
            _surfaceCpsData.size.y = 10;
            _surfaceCpsData.count.x = splineCps.Count;
            _surfaceCpsData.count.y = Mathf.CeilToInt(rotationAngle / 30f) + 1;
            _surfaceCpsData.cps = new List<CP>();
            var t = rotationAngle / (_surfaceCpsData.count.y - 1);
            if (_surfaceCpsData.loopY)
            {
                _surfaceCpsData.count.y = 12;
                t = 360f / _surfaceCpsData.count.y;
            }

            for (var y = 0; y < _surfaceCpsData.count.y; y++)
                for (var x = 0; x < _surfaceCpsData.count.x; x++)
                {
                    var newPos = Quaternion.AngleAxis(y * t, axis) * splineCps[x].pos;
                    _surfaceCpsData.cps.Add(new CP(newPos, splineCps[x].weight));
                }

            _gameObject = new GameObject();
            _surfaceHandler = _gameObject.AddComponent<SurfaceHandler>();
            _surfaceHandler.Data = _surfaceCpsData;
            _surfaceHandler.division = new Vector2Int(100, 100);
            _surfaceHandler.Generate();
            //_surfaceHandler.RuinSelf();
        }

        // public static void cylinder(float radius, float height, int orderX, int orderY)
        // {
        // }
        //
        // public static void cone(float radius, float height, int orderX, int orderY)
        // {
        // }
        //
        // public static void sphere(float radius, int orderX, int orderY)
        // {
        // }
    }
}