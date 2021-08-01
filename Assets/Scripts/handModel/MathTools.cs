using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTools
{

    //计算空间中两条直线的交点
    public static Vector3 calculateIntersection(List<Vector3> line1, List<Vector3> line2)
    {
        Vector3 p1 = line1[0];//第一条直线的第一个点
        Vector3 p2 = line1[1];//第一条直线的第二个点
        Vector3 a = p1 - p2;

        Vector3 p3 = line2[0];//第二条直线的第一个点
        Vector3 p4 = line2[1];//第二条直线的第二个点
        Vector3 b = p3 - p4;

        //检查是否可以构成两条直线
        if(p1.Equals(p2) || p3.Equals(p4)){
            Debug.Log("不能构成直线");
        }
        //判断两条直线是否平行,两个向量的叉乘是否为0
        if (Vector3.Cross(a,b).Equals(new Vector3(0,0,0)))
        {
            Debug.Log("两条直线平行");
        }

        //判断辅助向量是否垂直于向量b,如果垂直说明共面
        Vector3 auxiliary = Vector3.Cross(p3 - p1, a);
        if (Vector3.Dot(auxiliary, b) != 0)
        {
            Debug.Log("两条直线异面");
        }

        //现在两条直线必相交，求交点

        //向量叉乘得到面积
        double area1 = Vector3.Cross(auxiliary, b).magnitude / 2;       
        double area2 = Vector3.Cross(p2 + p3 - p1 - p4, b).magnitude / 2;
        double specificValue = area1 / area2;
        //已知p1o/p1p2 求o点的位置2
        return p1 + (p2 - p1) * (float)specificValue;

    }
}
