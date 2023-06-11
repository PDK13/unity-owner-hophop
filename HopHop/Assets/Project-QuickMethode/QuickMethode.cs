using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.Security.Cryptography;
using UnityEngine.UI;
using UnityEngine.Events;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QuickMethode
{
    //==================================== Vector

    public class QVector
    {
        //'Vector().magnitude' mean Length & 'Vector.Distance()' mean Distance

        #region ==================================== Pos

        //Middle (Trung điểm)

        public static Vector2 GetMiddlePoint(Vector2 PointA, Vector2 PointB)
        {
            float X = (PointA.x + PointB.x) / 2;
            float Y = (PointA.y + PointB.y) / 2;
            return new Vector2(X, Y);
        }

        public static Vector3 GetMiddlePoint(Vector3 PointA, Vector3 PointB)
        {
            float X = (PointA.x + PointB.x) / 2;
            float Y = (PointA.y + PointB.y) / 2;
            float Z = (PointA.z + PointB.z) / 2;
            return new Vector3(X, Y, Z);
        }

        #endregion

        #region ==================================== Dir

        //Primary

        public static Vector3 GetDir(Vector3 PosStart, Vector3 PosEnd)
        {
            return (PosEnd - PosStart).normalized;
        }

        public static Vector3 GetDir(Transform PointStart, Transform PointEnd)
        {
            return (PointEnd.position - PointStart.position).normalized;
        }

        //Reflech (Phản xạ)

        public static Vector2 GetDirReflect(Vector2 Dir, Collision2D Collision)
        {
            //Get Dir Reflect (Phản xạ) from Dir to!!
            return Vector2.Reflect(Dir, Collision.contacts[0].normal);
        }

        #endregion

        #region ==================================== Distance

        //3D

        public static float GetDistanceX(Vector3 PointA, Vector3 PointB)
        {
            Vector3 PointAZero = new Vector3(PointA.x, 0);
            Vector3 PointBZero = new Vector3(PointB.x, 0);
            return Vector3.Distance(PointAZero, PointBZero);
        }

        public static float GetDistanceY(Vector3 PointA, Vector3 PointB)
        {
            Vector3 PointAZero = new Vector3(0, PointA.y);
            Vector3 PointBZero = new Vector3(0, PointB.y);
            return Vector3.Distance(PointAZero, PointBZero);
        }

        public static float GetDistanceZ(Vector3 PointA, Vector3 PointB)
        {
            Vector3 PointAZero = new Vector3(0, 0, PointA.z);
            Vector3 PointBZero = new Vector3(0, 0, PointB.z);
            return Vector3.Distance(PointAZero, PointBZero);
        }

        //2D

        public static float GetDistanceX2D(Vector2 PointA, Vector2 PointB)
        {
            Vector2 PointAZero = new Vector2(PointA.x, 0);
            Vector2 PointBZero = new Vector2(PointB.x, 0);
            return Vector2.Distance(PointAZero, PointBZero);
        }

        public static float GetDistanceY2D(Vector2 PointA, Vector2 PointB)
        {
            Vector2 PointAZero = new Vector2(0, PointA.y);
            Vector2 PointBZero = new Vector2(0, PointB.y);
            return Vector2.Distance(PointAZero, PointBZero);
        }

        //Transform

        public static float GetDistanceX(Transform PointA, Transform PointB)
        {
            Vector3 PointAZero = new Vector3(PointA.position.x, 0);
            Vector3 PointBZero = new Vector3(PointB.position.x, 0);
            return Vector3.Distance(PointAZero, PointBZero);
        }

        public static float GetDistanceY(Transform PointA, Transform PointB)
        {
            Vector3 PointAZero = new Vector3(0, PointA.position.y);
            Vector3 PointBZero = new Vector3(0, PointB.position.y);
            return Vector3.Distance(PointAZero, PointBZero);
        }

        public static float GetDistanceZ(Transform PointA, Transform PointB)
        {
            Vector3 PointAZero = new Vector3(0, 0, PointA.position.z);
            Vector3 PointBZero = new Vector3(0, 0, PointB.position.z);
            return Vector3.Distance(PointAZero, PointBZero);
        }

        #endregion
    }

    //==================================== Circle

    public class QCircle
    {
        /* IMPORTANCE NOTE:
         * Deg can be understand as Euler in Unity.
         * Deg Caculated from X-Axis Right of World.
         * Pos Caculated from Center Zero of World (Should use Local Pos instead World Pos).
         * Deg caculated from this class all base on 0 to 360 degree.
        */

        #region ==================================== Deg

        //Opposite (Đối diện)

        public static float GetDegOppositeUD(float Deg360)
        {
            if (0f == Deg360 || Deg360 == 360f)
                return 180f;

            if (0f < Deg360 && Deg360 < 90f)
                return 90f + (90f - Deg360);

            if (Deg360 == 90f)
                return 90f;

            if (90f < Deg360 && Deg360 < 180f)
                return 90f - (Deg360 - 90f);

            if (Deg360 == 180f)
                return 0f;

            if (180f < Deg360 && Deg360 < 270f)
                return 270f + (270f - Deg360);

            if (Deg360 == 270f)
                return 270f;

            if (270f < Deg360 && Deg360 < 360f)
                return 270f - (Deg360 - 270f);

            Debug.LogError("Sonething wrong here!");
            return 90f;
        }

        public static float GetDegOppositeLR(float Deg360)
        {
            if (0f == Deg360 || Deg360 == 360f)
                return 0f;

            if (0f < Deg360 && Deg360 < 90f)
                return 360f - (0f + Deg360);

            if (Deg360 == 90f)
                return 270f;

            if (90f < Deg360 && Deg360 < 180f)
                return 180f + (180f - Deg360);

            if (Deg360 == 180f)
                return 180f;

            if (180f < Deg360 && Deg360 < 270f)
                return 180f + (Deg360 - 180f);

            if (Deg360 == 270f)
                return 90f;

            if (270f < Deg360 && Deg360 < 360f)
                return 0f - (360f - Deg360);

            Debug.LogError("Sonething wrong here!");
            return 0f;
        }

        #endregion

        #region ==================================== Pos by Deg

        public static Vector3 GetPosXY(float Deg360, float Radius)
        {
            //Get Pos from Center (0; 0)

            return new Vector3(Mathf.Cos(Deg360 * Mathf.Deg2Rad), Mathf.Sin(Deg360 * Mathf.Deg2Rad), 0) * Radius;
        }

        public static Vector3 GetPosXZ(float Deg360, float Radius)
        {
            //Get Pos from Center (0; 0; 0)

            return new Vector3(Mathf.Cos(Deg360 * Mathf.Deg2Rad), 0, Mathf.Sin(Deg360 * Mathf.Deg2Rad)) * Radius;
        }

        #endregion

        #region ==================================== Deg by Pos & Dir

        public static float GetDeg(Vector2 Dir, bool ConvertDeg360 = true)
        {
            //Get Deg from Center (0; 0)

            float Deg = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;

            if (ConvertDeg360 && Deg < 0)
                Deg = 360 + Deg;

            return Deg;
        }

        public static float GetDeg(Vector2 Center, Vector2 Pos, bool ConvertDeg360 = true)
        {
            //Get Deg from Center

            return GetDeg(Pos - Center, ConvertDeg360);
        }

        #endregion

        #region ==================================== Euler & Quaternion

        public static Quaternion GetEulerToQuaternion(Vector3 Deg)
        {
            Quaternion Quaternion = Quaternion.Euler(Deg);
            return Quaternion;
        }

        public static Vector3 GetQuaternionToEuler(Quaternion Quaternion)
        {
            Vector3 EulerRotation = Quaternion.eulerAngles;
            return EulerRotation;
        }

        #endregion

        #region ==================================== Deg to Target: Use for Face to a Target

        public static float GetDegTargetXZ(Transform TransformMain, Transform TransformTarket) //Check(?!)
        {
            float Distance = Vector3.Distance(TransformMain.transform.position, TransformTarket.position);
            float Deg = TransformMain.transform.eulerAngles.y;

            Vector3 DirStart = QVector.GetDir(TransformMain.transform.position, TransformMain.transform.position + GetPosXZ(-Deg, Distance));
            Vector3 DirEnd = QVector.GetDir(TransformMain.transform.position, TransformMain.transform.position + QVector.GetDir(TransformMain.transform.position, TransformTarket.position) * Distance);

            Vector2 DirFrom = new Vector2(DirStart.x, DirStart.z);
            Vector2 DirTo = new Vector2(DirEnd.x, DirEnd.z);

            return Vector2.Angle(DirFrom, DirTo);
        }

        #endregion
    }

    //==================================== Transform

    public class QTransform
    {
        #region ------------------------------------ Move - Fixed Update

        //Toward

        public static void SetMoveToward(Transform Transform, Vector3 Pos, float DeltaDistance)
        {
            //Pos Move Linear by Distance when called - On World!!
            Vector3 PosMove = Vector3.MoveTowards(Transform.position, Pos, DeltaDistance);
            Transform.position = PosMove;
        }

        public static void SetMoveToward(RectTransform RectTransform, Vector3 PosAnchor, float DeltaDistance)
        {
            //Pos Move Linear by Distance when called - On Canvas!!
            Vector3 PosMove = Vector3.MoveTowards(RectTransform.anchoredPosition, PosAnchor, DeltaDistance);
            RectTransform.anchoredPosition = PosMove;
        }

        //Lerf

        public static void SetMoveLerp(Transform Transform, Vector3 Pos, float DeltaTime)
        {
            //Pos Move none Linear by Time per called!!
            Vector3 PosMove = Vector3.Lerp(Transform.position, Pos, DeltaTime);
            Transform.position = PosMove;
        }

        #endregion

        #region ------------------------------------ Rotation - Fixed Update

        //Toward

        public static void SetRotate3DToward(Transform Transform, Vector3 Deg, float DeltaDeg)
        {
            //Pos Move Linear by Distance when called - On World!!
            Vector3 DegMove = Vector3.MoveTowards(Transform.eulerAngles, Deg, DeltaDeg);
            Transform.eulerAngles = DegMove;
        }

        public static void SetRotate3DToward(RectTransform RectTransform, Vector3 DegAnchor, float DeltaDeg)
        {
            //Pos Move Linear by Distance when called - On Canvas!!
            Vector3 DegMove = Vector3.MoveTowards(RectTransform.eulerAngles, DegAnchor, DeltaDeg);
            RectTransform.eulerAngles = DegMove;
        }

        public static void SetRotate3DTowardTarget(Transform Transform, Vector3 PosTarget, float DeltaDeg)
        {
            //Pos Move Linear by Distance when called!!
            Vector3 Dir = (PosTarget - Transform.position).normalized;
            Vector3 Deg = new Vector3(0, 0, QCircle.GetDeg(Dir));
            Vector3 DegMove = Vector3.MoveTowards(Transform.eulerAngles, Deg, DeltaDeg);
            Transform.eulerAngles = DegMove;
        }

        //Lerf

        public static void SetRotate3DLerp(Transform Transform, Vector3 Deg, float DeltaTime)
        {
            //Pos Move none Linear by Time per called!!
            Vector3 DegMove = Vector3.Lerp(Transform.eulerAngles, Deg, DeltaTime);
            Transform.eulerAngles = DegMove;
        }

        public static void SetRotate3DLerp(RectTransform RectTransform, Vector3 DegAnchor, float DeltaDeg)
        {
            //Pos Move Linear by Distance when called - On Canvas!!
            Vector3 DegMove = Vector3.Lerp(RectTransform.eulerAngles, DegAnchor, DeltaDeg);
            RectTransform.eulerAngles = DegMove;
        }

        public static void SetRotate3DLerpTarget(Transform Transform, Vector3 PosTarget, float DeltaDeg)
        {
            //Pos Move Linear by Distance when called!!
            Vector3 Dir = (PosTarget - Transform.position).normalized;
            Vector3 Deg = new Vector3(0, 0, QCircle.GetDeg(Dir));
            Vector3 DegMove = Vector3.Lerp(Transform.eulerAngles, Deg, DeltaDeg);
            Transform.eulerAngles = DegMove;
        }

        #endregion

        #region ------------------------------------ Rotation Instantly

        //2D

        public static void SetRotate2D(Transform Transform, Vector2 PosStart, Vector2 PosEnd, bool WorldRotate = true)
        {
            Transform.position = PosStart;
            Vector2 Dir = (PosEnd - PosStart).normalized;

            SetRotate2D(Transform, Dir, WorldRotate);
        }

        public static void SetRotate2D(Transform Transform, Vector2 Dir, bool WorldRotate = true)
        {
            float Deg = Mathf.Rad2Deg * Mathf.Atan2(Dir.y, Dir.x);

            SetRotate2D(Transform, Deg, WorldRotate);
        }

        public static void SetRotate2D(Transform Transform, float Deg, bool WorldRotate = true)
        {
            if (WorldRotate)
                Transform.eulerAngles = new Vector3(0, 0, Deg);
            else
                Transform.localEulerAngles = new Vector3(0, 0, Deg);
        }

        //3D

        public static void SetRotate3DXZ(Transform Transform, Vector3 PosStart, Vector3 PosEnd, bool WorldRotate = true)
        {
            Transform.position = PosStart;
            Vector3 Dir = (PosEnd - PosStart).normalized;

            SetRotate3DXZ(Transform, Dir, WorldRotate);
        }

        public static void SetRotate3DXZ(Transform Transform, Vector3 Dir, bool WorldRotate = true)
        {
            float Deg = Mathf.Rad2Deg * Mathf.Atan2(Dir.z, Dir.x);

            SetRotate3DXZ(Transform, Deg, WorldRotate);
        }

        public static void SetRotate3DXZ(Transform Transform, float Deg, bool WorldRotate = true)
        {
            if (WorldRotate)
                Transform.eulerAngles = new Vector3(0, Deg, 0);
            else
                Transform.localEulerAngles = new Vector3(0, Deg, 0);
        }

        #endregion
    }

    //==================================== Collider2D

    public class QCollider2D
    {
        #region ==================================== Border

        public static Vector2 GetBorderPos(Collider2D Collider, params Direction[] Bound)
        {
            //Primary Value: Collider
            Vector2 Pos = Collider.bounds.center;
            Vector2 Size = Collider.bounds.size;
            float Edge = 0f;

            BoxCollider2D ColliderBox = Collider.GetComponent<BoxCollider2D>();
            if (ColliderBox != null)
            {
                Edge = ColliderBox.edgeRadius;

                //Option Value: Auto Tilling
                if (ColliderBox.autoTiling)
                {
                    SpriteRenderer Sprite = Collider.GetComponent<SpriteRenderer>();
                    if (Sprite != null)
                        Size = Sprite.bounds.size;
                }
            }

            //Caculate Position from Value Data
            foreach (Direction DirBound in Bound)
            {
                switch (DirBound)
                {
                    case Direction.Up:
                        Pos.y += (Size.y / 2) + Edge;
                        break;
                    case Direction.Down:
                        Pos.y -= (Size.y / 2) + Edge;
                        break;
                    case Direction.Left:
                        Pos.x -= (Size.x / 2) + Edge;
                        break;
                    case Direction.Right:
                        Pos.x += (Size.x / 2) + Edge;
                        break;
                }
            }
            return Pos;
        }

        public static Vector2 GetBorderPos(Collider2D Collider, params DirectionX[] Bound)
        {
            //Primary Value: Collider
            Vector2 Pos = Collider.bounds.center;
            Vector2 Size = Collider.bounds.size;
            float Edge = 0f;

            BoxCollider2D ColliderBox = Collider.GetComponent<BoxCollider2D>();
            if (ColliderBox != null)
            {
                Edge = ColliderBox.edgeRadius;

                //Option Value: Auto Tilling
                if (ColliderBox.autoTiling)
                {
                    SpriteRenderer Sprite = Collider.GetComponent<SpriteRenderer>();
                    if (Sprite != null)
                        Size = Sprite.bounds.size;
                }
            }

            //Caculate Position from Value Data
            foreach (DirectionX DirBound in Bound)
            {
                switch (DirBound)
                {
                    case DirectionX.Left:
                        Pos.x -= (Size.x / 2) + Edge;
                        break;
                    case DirectionX.Right:
                        Pos.x += (Size.x / 2) + Edge;
                        break;
                }
            }
            return Pos;
        }

        public static Vector2 GetBorderPos(Collider2D Collider, params DirectionY[] Bound)
        {
            //Primary Value: Collider
            Vector2 Pos = Collider.bounds.center;
            Vector2 Size = Collider.bounds.size;
            float Edge = 0f;

            BoxCollider2D ColliderBox = Collider.GetComponent<BoxCollider2D>();
            if (ColliderBox != null)
            {
                Edge = ColliderBox.edgeRadius;

                //Option Value: Auto Tilling
                if (ColliderBox.autoTiling)
                {
                    SpriteRenderer Sprite = Collider.GetComponent<SpriteRenderer>();
                    if (Sprite != null)
                        Size = Sprite.bounds.size;
                }
            }

            //Caculate Position from Value Data
            foreach (DirectionY DirBound in Bound)
            {
                switch (DirBound)
                {
                    case DirectionY.Up:
                        Pos.y += (Size.y / 2) + Edge;
                        break;
                    case DirectionY.Down:
                        Pos.y -= (Size.y / 2) + Edge;
                        break;
                }
            }
            return Pos;
        }

        #endregion

        #region ==================================== Sprite Renderer

        public static void SetMatch(BoxCollider2D Collider, SpriteRenderer SpriteRenderer) //Check again when Rotation!!
        {
            Vector2 PosPrimary = Collider.transform.position;
            Vector2 PosRenderer = SpriteRenderer.bounds.center;
            Vector2 Offset = PosRenderer - PosPrimary;

            Collider.size = SpriteRenderer.bounds.size;
            Collider.offset = Offset;
        }

        #endregion

        #region ==================================== Composite Collider

        public static List<Vector2> GetPointsCenterPos(CompositeCollider2D Collider)
        {
            //NOTE: Composite Collider split Points into Group if they're not contact with each other!!
            List<Vector2> PointCenter = new List<Vector2>();
            for (int i = 0; i < Collider.pathCount; i++)
            {
                //Get Points of this Group!!
                Vector2[] Points = new Vector2[Collider.GetPathPointCount(i)];
                Collider.GetPath(i, Points);
                //Caculate CenterPoint of this Group!!
                float CenterX = 0f;
                float CenterY = 0f;
                for (int j = 0; j < Points.GetLength(0); j++)
                {
                    CenterX += Points[j].x;
                    CenterY += Points[j].y;
                }
                Vector2 Pos = new Vector2(CenterX / Points.GetLength(0), CenterY / Points.GetLength(0));
                PointCenter.Add(Pos);
                //Done caculate Point Center of current Group!!
            }

            //Result local pos Center of Collider!!
            return PointCenter;
        }

        public static List<List<Vector2>> GetPointsBorderPos(CompositeCollider2D Collider, bool WorldPos, bool Square = true)
        {
            Vector2 TileCenter = WorldPos ? Collider.transform.position : Vector3.zero;

            //NOTE: Composite Collider split Points into Group if they're not contact with each other!!
            List<List<Vector2>> PointsBorder = new List<List<Vector2>>();
            for (int Group = 0; Group < Collider.pathCount; Group++)
            {
                //Generate a new Group!!
                PointsBorder.Add(new List<Vector2>());
                //Get Points of this Group!!
                Vector2[] Points = new Vector2[Collider.GetPathPointCount(Group)];
                Collider.GetPath(Group, Points);
                for (int Index = 0; Index < Points.Length; Index++)
                {
                    //Generate new Points into each Group!!
                    if (Square)
                    {
                        Vector2Int Pos = new Vector2Int(Mathf.RoundToInt(Points[Index].x), Mathf.RoundToInt(Points[Index].y));
                        if (Points.Contains(Pos))
                            continue;
                        PointsBorder[Group].Add(TileCenter + Pos);
                    }
                    else
                    {
                        PointsBorder[Group].Add(TileCenter + Points[Index]);
                    }
                }
                //Done Generate current Group!!
            }

            //Result local pos Points of Collider!!
            return PointsBorder;
        }

        #endregion

        #region ==================================== Platform

        public static List<(Vector2 Center, float Length)> GetPlatform(CompositeCollider2D Collider, bool WorldPos)
        {
            Vector2 TileCenter = WorldPos ? Collider.transform.position : Vector3.zero;

            //NOTE: Caculate Platform Pos and Length of Collider on each Group!!
            List<(Vector2 Center, float Length)> Platform = new List<(Vector2 Center, float Length)>();

            List<List<Vector2>> Points = GetPointsBorderPos(Collider, false);
            for (int Group = 0; Group < Points.Count; Group++)
            {
                //Find a highest Point of this Group!!
                int IndexStart = 0;
                float HighStart = 0;
                for (int Index = 0; Index < Points[Group].Count; Index++)
                {
                    if (Points[Group][Index].y <= HighStart)
                        continue;
                    IndexStart = Index;
                    HighStart = Points[Group][Index].y;
                }
                //Check where can Stand!!
                //Check from Left to Right mean Stand, else not Stand!!
                int IndexNext = IndexStart;
                int IndexPrev = IndexNext - 1 >= 0 ? IndexNext - 1 : Points[Group].Count - 1;
                do
                {
                    //Index Run!!
                    IndexNext++;
                    IndexPrev++;
                    if (IndexNext >= Points[Group].Count)
                        IndexNext = 0;
                    if (IndexPrev >= Points[Group].Count)
                        IndexPrev = 0;
                    //Check Start - End Index?!
                    if (IndexNext == IndexStart)
                        break;
                    //Check at Index!!
                    if (Points[Group][IndexPrev].y != Points[Group][IndexNext].y)
                        continue;
                    if (Points[Group][IndexPrev].x <= Points[Group][IndexNext].x)
                        continue;
                    Vector2 PointA = Points[Group][IndexPrev];
                    Vector2 PointB = Points[Group][IndexNext];
                    Vector2 Center = QVector.GetMiddlePoint(PointA, PointB);
                    float Length = QVector.GetDistanceX2D(PointA, PointB);
                    Platform.Add((TileCenter + Center, Length));
                }
                while (IndexNext != IndexStart);
                //Done Check current Group!!
            }

            //Result local Center of Platform!!
            return Platform;
        }

        #endregion
    }

    //==================================== Layer

    public class QLayer
    {
        #region Primary

        public static int GetLayerMask(params string[] LayerName)
        {
            return LayerMask.GetMask(LayerName);
        }

        public static int GetLayerMaskSingle(string LayerName)
        {
            return LayerMask.NameToLayer(LayerName);
        }

        #endregion

        #region GameObject & Component

        public static void SetLayerMask(GameObject GameObject, string LayerName)
        {
            GameObject.layer = GetLayerMaskSingle(LayerName);
        }

        public static void SetLayerMask(PlatformEffector2D Platform, params string[] LayerName)
        {
            Platform.colliderMask = GetLayerMask(LayerName);
        }

        public static void SetLayerMask(AreaEffector2D Platform, params string[] LayerName)
        {
            Platform.colliderMask = GetLayerMask(LayerName);
        }

        public static void SetLayerMask(PointEffector2D Platform, params string[] LayerName)
        {
            Platform.colliderMask = GetLayerMask(LayerName);
        }

        public static void SetLayerMask(SurfaceEffector2D Platform, params string[] LayerName)
        {
            Platform.colliderMask = GetLayerMask(LayerName);
        }

        #endregion
    }

    //==================================== Cast

    public class QCast : QLayer
    {
        #region ==================================== 3D

        #region ------------------------------------ None LayerMask

        public static (GameObject Target, Vector3 Point)? GetLineCast(Vector3 PosStart, Vector3 PosEnd)
        {
            RaycastHit RaycastHit = new RaycastHit();
            Physics.Linecast(PosStart, PosEnd, out RaycastHit);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetRaycast(Vector3 PosStart, Vector3 PosEnd, float Distance)
        {
            Vector3 Dir = (PosEnd - PosStart).normalized;

            RaycastHit RaycastHit = new RaycastHit();
            Physics.Raycast(PosStart, Dir, out RaycastHit, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetRaycastDir(Vector3 PosStart, Vector3 Dir, float Distance)
        {
            RaycastHit RaycastHit = new RaycastHit();
            Physics.Raycast(PosStart, Dir.normalized, out RaycastHit, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetBoxCast(Vector3 PosStart, Vector3 PosEnd, Vector3 Size, Vector3 Rotation, float Distance)
        {
            Vector3 Dir = (PosEnd - PosStart).normalized;
            Quaternion Quaternion = Quaternion.Euler(Rotation);

            RaycastHit RaycastHit = new RaycastHit();
            Physics.BoxCast(PosStart, Size, Dir, out RaycastHit, Quaternion, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetBoxCastDir(Vector3 PosStart, Vector3 Dir, Vector3 Size, Vector3 Rotation, float Distance)
        {
            Quaternion Quaternion = Quaternion.Euler(Rotation);

            RaycastHit RaycastHit = new RaycastHit();
            Physics.BoxCast(PosStart, Size, Dir.normalized, out RaycastHit, Quaternion, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetSphereCast(Vector3 PosStart, Vector3 PosEnd, float Radius, float Distance)
        {
            Vector3 Dir = (PosEnd - PosStart).normalized;

            RaycastHit RaycastHit = new RaycastHit();
            Physics.SphereCast(PosStart, Radius / 2, Dir, out RaycastHit, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetSphereCastDir(Vector3 PosStart, Vector3 Dir, float Radius, float Distance)
        {
            RaycastHit RaycastHit = new RaycastHit();
            Physics.SphereCast(PosStart, Radius / 2, Dir.normalized, out RaycastHit, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static List<GameObject> GetBoxOverlap(Vector3 PosStart, Vector3 Size, Vector3 Rotation)
        {
            Quaternion Quaternion = Quaternion.Euler(Rotation);
            Collider[] ObjectsHit = Physics.OverlapBox(PosStart, Size, Quaternion);

            List<GameObject> ObjectsHitList = new List<GameObject>();
            foreach (Collider ObjectHit in ObjectsHit) ObjectsHitList.Add(ObjectHit.gameObject);

            return ObjectsHitList;
        }

        public static List<GameObject> GetCircleOverlap(Vector3 PosStart, float Size)
        {
            Collider[] ObjectsHit = Physics.OverlapSphere(PosStart, Size);

            List<GameObject> ObjectsHitList = new List<GameObject>();
            foreach (Collider ObjectHit in ObjectsHit) ObjectsHitList.Add(ObjectHit.gameObject);

            return ObjectsHitList;
        }

        #endregion

        #region ------------------------------------ LayerMask

        public static (GameObject Target, Vector3 Point)? GetLineCast(Vector3 PosStart, Vector3 PosEnd, LayerMask Tarket)
        {
            RaycastHit RaycastHit = new RaycastHit();

            Physics.Linecast(PosStart, PosEnd, out RaycastHit, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetRaycast(Vector3 PosStart, Vector3 PosEnd, float Distance, LayerMask Tarket)
        {
            Vector3 Dir = (PosEnd - PosStart).normalized;

            RaycastHit RaycastHit = new RaycastHit();
            Physics.Raycast(PosStart, Dir, out RaycastHit, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetRaycastDir(Vector3 PosStart, Vector3 Dir, float Distance, LayerMask Tarket)
        {
            RaycastHit RaycastHit = new RaycastHit();
            Physics.Raycast(PosStart, Dir.normalized, out RaycastHit, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetBoxCast(Vector3 PosStart, Vector3 PosEnd, Vector3 Size, Vector3 Rotation, float Distance, LayerMask Tarket)
        {
            Vector3 Dir = (PosEnd - PosStart).normalized;
            Quaternion Quaternion = Quaternion.Euler(Rotation);

            RaycastHit RaycastHit = new RaycastHit();
            Physics.BoxCast(PosStart, Size, Dir, out RaycastHit, Quaternion, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetBoxCastDir(Vector3 PosStart, Vector3 Dir, Vector3 Size, Vector3 Rotation, float Distance, LayerMask Tarket)
        {
            Quaternion Quaternion = Quaternion.Euler(Rotation);

            RaycastHit RaycastHit = new RaycastHit();
            Physics.BoxCast(PosStart, Size, Dir.normalized, out RaycastHit, Quaternion, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetSphereCast(Vector3 PosStart, Vector3 PosEnd, float Radius, float Distance, LayerMask Tarket)
        {
            Vector3 Dir = (PosEnd - PosStart).normalized;

            RaycastHit RaycastHit = new RaycastHit();
            Physics.SphereCast(PosStart, Radius / 2, Dir, out RaycastHit, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector3 Point)? GetSphereCastDir(Vector3 PosStart, Vector3 Dir, float Radius, float Distance, LayerMask Tarket)
        {
            RaycastHit RaycastHit = new RaycastHit();
            Physics.SphereCast(PosStart, Radius / 2, Dir.normalized, out RaycastHit, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static List<GameObject> GetBoxOverlap(Vector3 PosStart, Vector3 Size, Vector3 Rotation, LayerMask Tarket)
        {
            Quaternion Quaternion = Quaternion.Euler(Rotation);

            Collider[] ObjectsHit = Physics.OverlapBox(PosStart, Size, Quaternion, Tarket);

            List<GameObject> ObjectsHitList = new List<GameObject>();
            foreach (Collider ObjectHit in ObjectsHit) ObjectsHitList.Add(ObjectHit.gameObject);

            return ObjectsHitList;
        }

        public static List<GameObject> GetCircleOverlap(Vector3 PosStart, float Size, LayerMask Tarket)
        {
            Collider[] ObjectsHit = Physics.OverlapSphere(PosStart, Size, Tarket);

            List<GameObject> ObjectsHitList = new List<GameObject>();
            foreach (Collider ObjectHit in ObjectsHit) ObjectsHitList.Add(ObjectHit.gameObject);

            return ObjectsHitList;
        }

        #endregion

        #endregion

        #region ==================================== 2D

        #region ------------------------------------ None LayerMask

        public static (GameObject Target, Vector2 Point)? GetLineCast2D(Vector2 PosStart, Vector2 PosEnd)
        {
            RaycastHit2D RaycastHit = Physics2D.Linecast(PosStart, PosEnd);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetRaycast2D(Vector2 PosStart, Vector2 PosEnd, float Distance)
        {
            Vector2 Dir = (PosEnd - PosStart).normalized;

            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.Raycast(PosStart, Dir, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetRaycast2DDir(Vector2 PosStart, Vector2 Dir, float Distance)
        {
            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.Raycast(PosStart, Dir.normalized, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetBoxCast2D(Vector2 PosStart, Vector2 PosEnd, Vector2 Size, float Rotation, float Distance)
        {
            Vector2 Dir = (PosEnd - PosStart).normalized;

            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.BoxCast(PosStart, Size, Rotation, Dir, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetBoxCast2DDir(Vector2 PosStart, Vector2 Dir, Vector2 Size, float Rotation, float Distance)
        {
            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.BoxCast(PosStart, Size, Rotation, Dir.normalized, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetCircleCast2D(Vector2 PosStart, Vector2 PosEnd, float Radius, float Distance)
        {
            Vector2 Dir = (PosEnd - PosStart).normalized;

            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.CircleCast(PosStart, Radius / 2, Dir, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetCircleCast2DDir(Vector2 PosStart, Vector2 Dir, float Radius, float Distance)
        {
            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.CircleCast(PosStart, Radius / 2, Dir.normalized, Distance);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static GameObject GetOverlapCircle2D(Vector2 PosStart, float Radius)
        {
            Collider2D ColliderHit = Physics2D.OverlapCircle(PosStart, Radius);

            if (ColliderHit == null) return null;

            return ColliderHit.gameObject;
        }

        public static List<GameObject> GetOverlapCircleAll2D(Vector2 PosStart, float Radius)
        {
            Collider2D[] ColliderHit = Physics2D.OverlapCircleAll(PosStart, Radius);

            List<GameObject> ColliderHitList = new List<GameObject>();
            for (int i = 0; i < ColliderHit.Length; i++) if (ColliderHit[i] != null) ColliderHitList.Add(ColliderHit[i].gameObject);

            return ColliderHitList;
        }

        public static GameObject GetOverlapBox2D(Vector2 PosStart, Vector2 Size, float Rotation)
        {
            Collider2D ColliderHit = Physics2D.OverlapBox(PosStart, Size, Rotation);

            if (ColliderHit == null) return null;

            return ColliderHit.gameObject;
        }

        public static List<GameObject> GetOverlapBoxAll2D(Vector2 PosStart, Vector2 Size, float Rotation)
        {
            Collider2D[] ColliderHit = Physics2D.OverlapBoxAll(PosStart, Size, Rotation);

            List<GameObject> ColliderHitList = new List<GameObject>();
            for (int i = 0; i < ColliderHit.Length; i++) if (ColliderHit[i] != null) ColliderHitList.Add(ColliderHit[i].gameObject);

            return ColliderHitList;
        }

        #endregion

        #region ------------------------------------ LayerMask

        public static (GameObject Target, Vector2 Point)? GetLineCast2D(Vector2 PosStart, Vector2 PosEnd, LayerMask Tarket)
        {
            RaycastHit2D RaycastHit = Physics2D.Linecast(PosStart, PosEnd, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetRaycast2D(Vector2 PosStart, Vector2 PosEnd, float Distance, LayerMask Tarket)
        {
            Vector2 Dir = (PosEnd - PosStart).normalized;

            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.Raycast(PosStart, Dir, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetRaycast2DDir(Vector2 PosStart, Vector2 Dir, float Distance, LayerMask Tarket)
        {
            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.Raycast(PosStart, Dir.normalized, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetBoxCast2D(Vector2 PosStart, Vector2 PosEnd, Vector2 Size, float Rotation, float Distance, LayerMask Tarket)
        {
            Vector2 Dir = (PosEnd - PosStart).normalized;

            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.BoxCast(PosStart, Size, Rotation, Dir, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetBoxCast2DDir(Vector2 PosStart, Vector2 Dir, Vector2 Size, float Rotation, float Distance, LayerMask Tarket)
        {
            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.BoxCast(PosStart, Size, Rotation, Dir.normalized, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetCircleCast2D(Vector2 PosStart, Vector2 PosEnd, float Radius, float Distance, LayerMask Tarket)
        {
            Vector2 Dir = (PosEnd - PosStart).normalized;

            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.CircleCast(PosStart, Radius / 2, Dir, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static (GameObject Target, Vector2 Point)? GetCircleCast2DDir(Vector2 PosStart, Vector2 Dir, float Radius, float Distance, LayerMask Tarket)
        {
            if (Dir == Vector2.zero)
                return (null, PosStart);

            RaycastHit2D RaycastHit = Physics2D.CircleCast(PosStart, Radius / 2, Dir.normalized, Distance, Tarket);

            if (RaycastHit.collider == null)
                return null;

            return (RaycastHit.collider.gameObject, RaycastHit.point);
        }

        public static GameObject GetOverlapCircle2D(Vector2 PosStart, float Radius, LayerMask Tarket)
        {
            Collider2D ColliderHit = Physics2D.OverlapCircle(PosStart, Radius, Tarket);

            if (ColliderHit == null) return null;

            return ColliderHit.gameObject;
        }

        public static List<GameObject> GetOverlapCircleAll2D(Vector2 PosStart, float Radius, LayerMask Tarket)
        {
            Collider2D[] ColliderHit = Physics2D.OverlapCircleAll(PosStart, Radius, Tarket);

            List<GameObject> ColliderHitList = new List<GameObject>();
            for (int i = 0; i < ColliderHit.Length; i++) if (ColliderHit[i] != null) ColliderHitList.Add(ColliderHit[i].gameObject);

            return ColliderHitList;
        }

        public static GameObject GetOverlapBox2D(Vector2 PosStart, Vector2 Size, float Rotation, LayerMask Tarket)
        {
            Collider2D ColliderHit = Physics2D.OverlapBox(PosStart, Size, Rotation, Tarket);

            if (ColliderHit == null) return null;

            return ColliderHit.gameObject;
        }

        public static List<GameObject> GetOverlapBoxAll2D(Vector2 PosStart, Vector2 Size, float Rotation, LayerMask Tarket)
        {
            Collider2D[] ColliderHit = Physics2D.OverlapBoxAll(PosStart, Size, Rotation, Tarket);

            List<GameObject> ColliderHitList = new List<GameObject>();
            for (int i = 0; i < ColliderHit.Length; i++) if (ColliderHit[i] != null) ColliderHitList.Add(ColliderHit[i].gameObject);

            return ColliderHitList;
        }

        #endregion

        #endregion
    }

    //==================================== RecTransform

    public class QRecTransform
    {
        private static Vector2 GetPosAnchorPivotOffset(RectTransform From, Vector2 ToPivot)
        {
            //Pivot is the centre point UI, which is the Anchor Pos of UI!
            Vector2 PivotOffset = new Vector2((From.pivot.x - ToPivot.x) * From.sizeDelta.x, (From.pivot.y - ToPivot.y) * From.sizeDelta.y);
            return PivotOffset;
        }

        private static Vector2 GetAnchorPosPrimary(RectTransform From)
        {
            //Anchor Primary mean Anchor Min(0;0) and Max(0;0) or at the BL of screen!
            Vector2 AnchorPos = From.anchoredPosition;
            Vector2 ScreenSize = QCamera.GetCameraSizePixel();
            Vector2 AnchorPosMin = new Vector2(ScreenSize.x * From.anchorMin.x, ScreenSize.y * From.anchorMin.y);
            Vector2 AnchorPosMax = new Vector2(ScreenSize.x * From.anchorMax.x, ScreenSize.y * From.anchorMax.y);
            Vector2 AnchorPosBD = AnchorPos + (AnchorPosMax - AnchorPosMin) * 0.5f + AnchorPosMin;
            return AnchorPosBD;
        }

        private static Vector2 GetPosAnchorCentre(Vector2 FromAnchorPosPrimary, Vector2 ToAnchorsMin, Vector2 ToAnchorsMax)
        {
            //Anchor Primary mean Anchor Min(0;0) and Max(0;0) or at the BL of screen!
            //Anchor Centre mean Anchor Min(0.5;0.5) and Max(0.5;0.5) or at the centre of screen!
            Vector2 ScreenSize = QCamera.GetCameraSizePixel();
            Vector2 AnchorPosMin = new Vector2(ScreenSize.x * ToAnchorsMin.x, ScreenSize.y * ToAnchorsMin.y);
            Vector2 AnchorPosMax = new Vector2(ScreenSize.x * ToAnchorsMax.x, ScreenSize.y * ToAnchorsMax.y);
            Vector2 AnchorPos = (AnchorPosMax - AnchorPosMin) * 0.5f + AnchorPosMin - FromAnchorPosPrimary;
            return AnchorPos;
        }

        public static Vector2 GetAnchorPos(RectTransform From, Vector2 ToPivot, Vector2 ToAnchorsMin, Vector2 ToAnchorsMax)
        {
            Vector2 AnchorPosPivotOffset = GetPosAnchorPivotOffset(From, ToPivot);
            Vector2 AnchorPosPrimary = GetAnchorPosPrimary(From);
            Vector2 AnchorPosCentre = GetPosAnchorCentre(AnchorPosPrimary, ToAnchorsMin, ToAnchorsMax);
            return (AnchorPosCentre + AnchorPosPivotOffset) * (-1);
        }
    }

    //==================================== Camera

    public class QCamera //Note: Current ORTHOGRAPHIC 2D only!!
    {
        //Required only ONE Main Camera (with tag Main Camera) for the true result!!

        #region ==================================== Pos of World & Canvas

        public static Vector3 GetPosMouseToWorld()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        public static Vector2 GetPosMouseToCanvas()
        {
            //NOTE: The value just apply for RecTransform got Anchors Centre and Pivot Centre!
            return GetPosWorldToCanvas(GetPosMouseToWorld());
        }

        public static Vector2 GetPosWorldToCanvas(Vector3 PosWorld)
        {
            //NOTE: The value just apply for RecTransform got Anchors Centre and Pivot Centre!
            return (Vector2)Camera.main.WorldToScreenPoint(PosWorld) - GetCameraSizePixel() * 0.5f;
        }

        #endregion

        #region ==================================== Camera

        //CAMERA mode ORTHOGRAPHIC - SIZE is a HALF number of UNIT WORLD HEIGHT from Scene to Screen.
        //EX: If Camera orthographic Size is 1, mean need 2 Square 1x1 Unit world to fill full HEIGHT of screen.

        public static Vector2 GetCameraSizePixel()
        {
            return GetCameraSizePixel(Camera.main);
        }

        public static Vector2 GetCameraSizeUnit()
        {
            return GetCameraSizeUnit(Camera.main);
        }

        public static Vector2 GetCameraSizePixel(Camera Camera)
        {
            return new Vector2(Camera.pixelWidth, Camera.pixelHeight);
        }

        public static Vector2 GetCameraSizeUnit(Camera Camera)
        {
            Vector2 SizePixel = GetCameraSizePixel(Camera);
            float HeightUnit = Camera.orthographicSize * 2;
            float WidthUnit = HeightUnit * (SizePixel.x / SizePixel.y);

            return new Vector2(WidthUnit, HeightUnit);
        }

        #endregion

        #region ==================================== Screen

        public static Vector2 GetScreenSizePixel()
        {
            return new Vector2(Screen.width, Screen.height);
        }

        #endregion
    }

    //==================================== Resolution

    public class QResolution
    {
        #region ==================================== Convert

        public enum UnitScaleType { Width, Height, Span, Primary, Tarket, }

        public static Vector2 GetSizeUnitScaled(Sprite SpritePrimary, Sprite SpriteTarket, UnitScaleType SpriteScale)
        {
            return GetSizeUnitScaled(QSprite.GetSpriteSizeUnit(SpritePrimary), QSprite.GetSpriteSizeUnit(SpriteTarket), SpriteScale);
        }

        public static Vector2 GetSizeUnitScaled(Vector2 SizeUnitPrimary, Vector2 SizeUnitTarket, UnitScaleType SpriteScale)
        {
            Vector2 SizeUnitFinal = new Vector2();

            switch (SpriteScale)
            {
                case UnitScaleType.Width:
                    {
                        float OffsetX = SizeUnitTarket.x / SizeUnitPrimary.x;
                        float SizeUnitFinalX = SizeUnitPrimary.x * OffsetX;
                        float SizeUnitFinalY = SizeUnitPrimary.y * OffsetX;
                        SizeUnitFinal = new Vector2(SizeUnitFinalX, SizeUnitFinalY);
                    }
                    break;
                case UnitScaleType.Height:
                    {
                        float OffsetY = SizeUnitTarket.y / SizeUnitPrimary.y;
                        float SizeUnitFinalX = SizeUnitPrimary.x * OffsetY;
                        float SizeUnitFinalY = SizeUnitPrimary.y * OffsetY;
                        SizeUnitFinal = new Vector2(SizeUnitFinalX, SizeUnitFinalY);
                    }
                    break;
                case UnitScaleType.Span:
                    {
                        float OffsetX = SizeUnitTarket.x / SizeUnitPrimary.x;
                        float OffsetY = SizeUnitTarket.y / SizeUnitPrimary.y;
                        if (OffsetX < OffsetY)
                        {
                            SizeUnitFinal = GetSizeUnitScaled(SizeUnitPrimary, SizeUnitTarket, UnitScaleType.Height);
                        }
                        else
                        {
                            SizeUnitFinal = GetSizeUnitScaled(SizeUnitPrimary, SizeUnitTarket, UnitScaleType.Width);
                        }
                    }
                    break;
                case UnitScaleType.Primary:
                    SizeUnitFinal = SizeUnitPrimary;
                    break;
                case UnitScaleType.Tarket:
                    SizeUnitFinal = SizeUnitTarket;
                    break;
            }

            return SizeUnitFinal;
        }

        #endregion
    }

    //==================================== Sprite & Sprite Renderer

    public class QSprite
    {
        #region ==================================== Sprite

        public static Vector2 GetSpriteSizePixel(Sprite Sprite)
        {
            return GetSpriteSizeUnit(Sprite) * GetSpritePixelPerUnit(Sprite) * 1.0f;
        }

        public static Vector2 GetSpriteSizeUnit(Sprite Sprite)
        {
            return Sprite.bounds.size * 1.0f;
        }

        public static float GetSpritePixelPerUnit(Sprite Sprite)
        {
            return Sprite.pixelsPerUnit * 1.0f;
        }

        #endregion

        #region ==================================== Sprite Renderer

        //...

        #endregion

        #region ==================================== Border

        public static Vector2 GetBorderPos(SpriteRenderer SpriteRenderer, params Direction[] Bound)
        {
            //Primary Value: Collider
            Vector2 Pos = SpriteRenderer.bounds.center;
            Vector2 Size = SpriteRenderer.bounds.size;

            //Caculate Position from Value Data
            foreach (Direction DirBound in Bound)
            {
                switch (DirBound)
                {
                    case Direction.Up:
                        Pos.y += Size.y / 2;
                        break;
                    case Direction.Down:
                        Pos.y -= Size.y / 2;
                        break;
                    case Direction.Left:
                        Pos.x -= Size.x / 2;
                        break;
                    case Direction.Right:
                        Pos.x += Size.x / 2;
                        break;
                }
            }
            return Pos;
        }

        public static Vector2 GetBorderPos(SpriteRenderer SpriteRenderer, params DirectionX[] Bound)
        {
            //Primary Value: Collider
            Vector2 Pos = SpriteRenderer.bounds.center;
            Vector2 Size = SpriteRenderer.bounds.size;

            //Caculate Position from Value Data
            foreach (DirectionX DirBound in Bound)
            {
                switch (DirBound)
                {
                    case DirectionX.Left:
                        Pos.x -= Size.x / 2;
                        break;
                    case DirectionX.Right:
                        Pos.x += Size.x / 2;
                        break;
                }
            }
            return Pos;
        }

        public static Vector2 GetBorderPos(SpriteRenderer SpriteRenderer, params DirectionY[] Bound)
        {
            //Primary Value: Collider
            Vector2 Pos = SpriteRenderer.bounds.center;
            Vector2 Size = SpriteRenderer.bounds.size;

            //Caculate Position from Value Data
            foreach (DirectionY DirBound in Bound)
            {
                switch (DirBound)
                {
                    case DirectionY.Up:
                        Pos.y += Size.y / 2;
                        break;
                    case DirectionY.Down:
                        Pos.y -= Size.y / 2;
                        break;
                }
            }
            return Pos;
        }

        #endregion

        #region ==================================== Texture

        //Texture can be used for Window Editor (Button)

        //Ex:
        //Window Editor:
        //Texture2D Texture = QSprite.GetTextureConvert(Sprite);
        //GUIContent Content = new GUIContent("", (Texture)Texture);
        //GUILayout.Button(Content());

        public static Texture2D GetTextureConvert(Sprite Sprite)
        {
            if (Sprite.texture.isReadable == false)
            {
                return null;
            }

            Texture2D Texture = new Texture2D((int)Sprite.rect.width, (int)Sprite.rect.height);

            Color[] ColorPixel = Sprite.texture.GetPixels(
                (int)Sprite.textureRect.x,
                (int)Sprite.textureRect.y,
                (int)Sprite.textureRect.width,
                (int)Sprite.textureRect.height);
            Texture.SetPixels(ColorPixel);
            Texture.Apply();
            return Texture;
        }

        public static string GetTextureConvertName(Sprite Sprite)
        {
            return GetTextureConvert(Sprite).name;
        }

        #endregion
    }

    //==================================== GameObject

    public class QGameObject
    {
        #region ==================================== Create

        public static GameObject SetCreate(GameObject Prepab, Transform Parent = null, bool WorldStay = true)
        {
            GameObject GameObject = MonoBehaviour.Instantiate(Prepab);

            if (Parent != null)
                GameObject.transform.SetParent(Parent, WorldStay);

            GameObject.transform.position = Prepab.transform.position;
            GameObject.transform.localScale = Prepab.transform.localScale;

            return GameObject;
        }

        public static GameObject SetCreate(string Name, Transform Parent = null, bool WorldStay = true)
        {
            GameObject GameObject = new GameObject(Name);

            if (Parent != null)
                GameObject.transform.SetParent(Parent, WorldStay);

            GameObject.transform.position = Vector3.zero;
            GameObject.transform.localScale = Vector3.one;

            return GameObject;
        }

        #endregion

        #region ==================================== Destroy

        public static void SetDestroy(UnityEngine.Object Object)
        {
            //Remove GameObject from Scene or Component from GameObject!!

            if (Object == null) return;

            if (Application.isEditor)
            {
                if (Application.isPlaying)
                    MonoBehaviour.Destroy(Object);
                else
                    MonoBehaviour.DestroyImmediate(Object);
            }
            else
            {
                MonoBehaviour.Destroy(Object);
            }
        }

        #endregion

        #region ==================================== Transform

        public static void SetIndex(Transform Child, int Index)
        {
            if (Child.parent != null)
            {
                if (Index < 0 || Index > Child.parent.childCount - 1) 
                    return;
            }

            Child.SetSiblingIndex(Index);
        }

        #endregion

        #region ==================================== Message

        public static void SetMessage(GameObject Target, string MethodeName, SendMessageOptions Option = SendMessageOptions.RequireReceiver)
        {
            Target.SendMessage(MethodeName, Option);
        }

        #endregion

        #region ==================================== GameObject

        public static string GetNameReplaceClone(string GameObjectName)
        {
            return GameObjectName.Replace("(Clone)", "");
        }

#if UNITY_EDITOR

        ///<summary>
        ///Caution: Unity Editor only!
        ///</summary>
        public static GameObject SetFocus(GameObject GameObjectInScene)
        {
            return Selection.activeGameObject = GameObjectInScene;
        }

        ///<summary>
        ///Caution: Unity Editor only!
        ///</summary>
        public static GameObject GetFocus()
        {
            return Selection.activeGameObject;
        }

#endif

        #endregion
    }

    //==================================== Component

    public class QComponent
    {
        #region Primary

        public static T GetComponent<T>(GameObject GameObject) where T : Component
        {
            //Get Component from GameObject. If null, Add Component to GameObject.

            if (GameObject.GetComponent<T>() == null)
                return GameObject.AddComponent<T>();
            else
                return GameObject.GetComponent<T>();
        }

        public static T GetComponent<T>(Transform GameObject) where T : Component
        {
            //Get Component from GameObject. If null, Add Component to GameObject.

            if (GameObject.GetComponent<T>() == null)
                return GameObject.gameObject.AddComponent<T>();
            else
                return GameObject.gameObject.GetComponent<T>();
        }

        #endregion

        #region Button

        public static void SetButton(Button Button, UnityAction Action)
        {
            //Add an void methode to Action!!

            //Caution: Some version of Unity might not run this after build app!!

            Button.onClick.AddListener(Action);
        }

        #endregion
    }

    //==================================== PlayerPrefs

    public class QPlayerPrefs
    {
        #region ==================================== Set

        #region ------------------------------------ Set Clear

        public static void SetValueClearAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        public static void SetValueClear(string Name)
        {
            if (!GetValueExist(Name))
            {
                Debug.LogError("SetValueClear: Not Exist" + "\"" + Name + "\"");
                return;
            }
            PlayerPrefs.DeleteKey(Name);
            PlayerPrefs.Save();
        }

        #endregion

        #region ------------------------------------ Set Primary

        public static void SetValue(string Name, string Value)
        {
            PlayerPrefs.SetString(Name, Value);
            PlayerPrefs.Save();
        }

        public static void SetValue(string Name, int Value)
        {
            PlayerPrefs.SetInt(Name, Value);
            PlayerPrefs.Save();
        }

        public static void SetValue(string Name, float Value)
        {
            PlayerPrefs.SetFloat(Name, Value);
            PlayerPrefs.Save();
        }

        public static void SetValue(string Name, bool Value)
        {
            PlayerPrefs.SetInt(Name, (Value) ? 1 : 0);
            PlayerPrefs.Save();
        }

        #endregion

        #region ------------------------------------ Set Params

        public static void SetValue(string Name, char Key, params string[] Value)
        {
            PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value));
            PlayerPrefs.Save();
        }

        public static void SetValue(string Name, char Key, params int[] Value)
        {
            PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value));
            PlayerPrefs.Save();
        }

        public static void SetValue(string Name, char Key, params float[] Value)
        {
            PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value));
            PlayerPrefs.Save();
        }

        public static void SetValue(string Name, char Key, params bool[] Value)
        {
            PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value));
            PlayerPrefs.Save();
        }

        #endregion

        #region ------------------------------------ Set Vector

        public static void SetValue(string Name, Vector2 Value)
        {
            SetValue(Name, QEncypt.GetEncyptVector2(';', Value));
        }

        public static void SetValue(string Name, Vector2Int Value)
        {
            SetValue(Name, QEncypt.GetEncyptVector2Int(';', Value));
        }

        public static void SetValue(string Name, Vector3 Value)
        {
            SetValue(Name, QEncypt.GetEncyptVector3(';', Value));
        }

        public static void SetValue(string Name, Vector3Int Value)
        {
            SetValue(Name, QEncypt.GetEncyptVector3Int(';', Value));
        }

        #endregion

        #region ------------------------------------ Set Time

        public static void SetValue(string Name, DateTime Value, string FormatTime)
        {
            SetValue(Name, Value.ToString(FormatTime));
        }

        #endregion

        #endregion

        #region ==================================== Get

        #region ------------------------------------ Get Exist

        public static bool GetValueExist(string Name)
        {
            return PlayerPrefs.HasKey(Name);
        }

        #endregion

        #region ------------------------------------ Get Primary

        public static string GetValueString(string Name, string Default = "")
        {
            return PlayerPrefs.GetString(Name, Default);
        }

        public static int GetValueInt(string Name, int Default = 0)
        {
            return PlayerPrefs.GetInt(Name, Default);
        }

        public static float GetValueFloat(string Name, float Default = 0.0f)
        {
            return PlayerPrefs.GetFloat(Name, Default);
        }

        public static bool GetValueBool(string Name, bool Default = false)
        {
            if (PlayerPrefs.GetInt(Name, 0) == 1)
            {
                return true;
            }
            return Default;
        }

        #endregion

        #region ------------------------------------ Get Params

        public static List<string> GetValueString(string Name, char Key)
        {
            return QEncypt.GetDencyptString(Key, GetValueString(Name));
        }

        public static List<int> GetValueInt(string Name, char Key)
        {
            return QEncypt.GetDencyptInt(Key, GetValueString(Name));
        }

        public static List<float> GetValueFloat(string Name, char Key)
        {
            return QEncypt.GetDencyptFloat(Key, GetValueString(Name));
        }

        public static List<bool> GetValueBool(string Name, char Key)
        {
            return QEncypt.GetDencyptBool(Key, GetValueString(Name));
        }

        #endregion

        #region ------------------------------------ Get Vector

        public static Vector2 SetValueVector2(string Name)
        {
            return QEncypt.GetDencyptVector2(';', GetValueString(Name));
        }

        public static Vector2Int SetValueVector2Int(string Name)
        {
            return QEncypt.GetDencyptVector2Int(';', GetValueString(Name));
        }

        public static Vector3 SetValueVector3(string Name)
        {
            return QEncypt.GetDencyptVector3(';', GetValueString(Name));
        }

        public static Vector3Int SetValueVector3Int(string Name)
        {
            return QEncypt.GetDencyptVector3Int(';', GetValueString(Name));
        }

        #endregion

        #region ------------------------------------ Get Time

        public static DateTime GetValueTime(string Name, string FormatTime)
        {
            return QDateTime.GetConvert(GetValueString(Name), FormatTime);
        }

        #endregion

        #endregion

        #region ==================================== App First Run (Should Remove)

        public static bool SetFirstRun(string PlayerPref = "-First-Run")
        {
            if (GetValueExist(Application.productName + PlayerPref))
            {
                return false;
            }

            SetValue(Application.productName + PlayerPref, true);

            return true;
        }

        #endregion
    }

    //==================================== Resources

    public class QResources
    {
        //NOTE:
        //Folder(s) "Resources" can be created everywhere from root "Assests/*", that can be access by Unity or Application

        //BEWARD:
        //All content(s) in folder(s) "Resources" will be builded to Application, even they ightn't be used in Build-Game Application

        #region ==================================== Prefab

        public static List<GameObject> GetPrefab(params string[] PathChildInResources)
        {
            string PathInResources = QPath.GetPath(QPath.PathType.None, PathChildInResources);
            GameObject[] LoadArray = Resources.LoadAll<GameObject>(PathInResources);
            List<GameObject> LoadList = new List<GameObject>();
            LoadList.AddRange(LoadArray);
            return LoadList;
        }

        #endregion

        #region ==================================== Sprite

        public static List<Sprite> GetSprite(params string[] PathChildInResources)
        {
            string PathInResources = QPath.GetPath(QPath.PathType.None, PathChildInResources);
            Sprite[] LoadArray = Resources.LoadAll<Sprite>(PathInResources);
            List<Sprite> LoadList = new List<Sprite>();
            LoadList.AddRange(LoadArray);
            return LoadList;
        }

        #endregion

        #region ==================================== Text Asset

        public static List<TextAsset> GetTextAsset(params string[] PathChildInResources)
        {
            string PathInResources = QPath.GetPath(QPath.PathType.None, PathChildInResources);
            TextAsset[] LoadArray = Resources.LoadAll<TextAsset>(PathInResources);
            List<TextAsset> LoadList = new List<TextAsset>();
            LoadList.AddRange(LoadArray);
            return LoadList;
        }

        #endregion
    }

    //==================================== File

    public class QPath
    {
        public const string ExamplePath = @"D:/ClassFileIO.txt";

        #region ==================================== Path Get

        public enum PathType { None, Persistent, Assets, Resources, Document, Picture, Music, Video, }

        public static string GetPath(PathType PathType, params string[] PathChild)
        {
            string PathFinal = "";

            switch (PathType)
            {
                case PathType.Persistent:
                    PathFinal = Application.persistentDataPath;
                    break;
                case PathType.Assets:
                    PathFinal = Application.dataPath;
                    break;
                case PathType.Resources:
                    PathFinal = Application.dataPath + @"/resources";
                    break;
                case PathType.Document:
                    PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    break;
                case PathType.Picture:
                    PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    break;
                case PathType.Music:
                    PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    break;
                case PathType.Video:
                    PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                    break;
            }

            foreach (string PathChildAdd in PathChild)
                QEncypt.GetEncyptAdd('/', PathFinal, PathChildAdd, out PathFinal);

            return PathFinal;
        }

        #endregion

        #region ==================================== Path Pannel

#if UNITY_EDITOR

        //Open

        ///<summary>
        ///Caution: Unity Editor only!
        ///</summary>
        public static (bool Result, string Path, string Name) GetPathFolderOpenPanel(string Title, string PathPrimary = "")
        {
            string Path = EditorUtility.OpenFolderPanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "");
            List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
            return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
        }

        ///<summary>
        ///Caution: Unity Editor only!
        ///</summary>
        public static (bool Result, string Path, string Name) GetPathFileOpenPanel(string Title, string Extension, string PathPrimary = "")
        {
            string Path = EditorUtility.OpenFilePanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, Extension);
            List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
            return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
        }

        //Save

        ///<summary>
        ///Caution: Unity Editor only!
        ///</summary>
        public static (bool Result, string Path, string Name) GetPathFolderSavePanel(string Title, string PathPrimary = "")
        {
            string Path = EditorUtility.SaveFolderPanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "");
            List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
            return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
        }

        ///<summary>
        ///Caution: Unity Editor only!
        ///</summary>
        public static (bool Result, string Path, string Name) GetPathFileSavePanel(string Title, string Extension, string PathPrimary = "")
        {
            string Path = EditorUtility.SaveFilePanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "", Extension);
            List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
            return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
        }

#endif

        #endregion

        #region ==================================== Path File Exist

        public static bool GetPathFileExist(string PathFile)
        {
            return File.Exists(PathFile);
        }

        public static bool GetPathFileExist(PathType PathType, params string[] PathChild)
        {
            return File.Exists(GetPath(PathType, PathChild));
        }

        #endregion

        #region ==================================== Path Folder Exist

        public static bool GetPathFolderExist(string PathFolder)
        {
            return Directory.Exists(PathFolder);
        }

        public static bool GetPathFolderExist(PathType PathType, params string[] PathChild)
        {
            return Directory.Exists(GetPath(PathType, PathChild));
        }

        #endregion
    }

    //==================================== FileIO

    public class QFileIO : QPath
    {
        #region ==================================== File IO Write 

        private string TextWrite = "";

        #region ------------------------------------ Write Start

        public void SetWriteStart(string Path)
        {
            SetWriteToFile(Path, GetWriteString());
        } //Call Last

        private void SetWriteToFile(string Path, string Data)
        {
            try
            {
                using (FileStream Stream = File.Create(Path))
                {
                    byte[] Info = new UTF8Encoding(true).GetBytes(Data);
                    Stream.Write(Info, 0, Info.Length);
                }
            }
            catch
            {
                Debug.LogErrorFormat("[Error] File Write Fail: {0}", Path);
            }
        }

        public void SetWriteClear()
        {
            TextWrite = "";
        }

        #endregion

        #region ------------------------------------ Write Add

        public void SetWriteAdd()
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += "";
        }

        public void SetWriteAdd(string DataAdd)
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += DataAdd;
        }

        public void SetWriteAdd(char Key, params string[] DataAdd)
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += QEncypt.GetEncypt(Key, DataAdd);
        }

        public void SetWriteAdd(char Key, params int[] DataAdd)
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += QEncypt.GetEncypt(Key, DataAdd);
        }

        public void SetWriteAdd(char Key, params float[] DataAdd)
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += QEncypt.GetEncypt(Key, DataAdd);
        }

        public void SetWriteAdd(char Key, params bool[] DataAdd)
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += QEncypt.GetEncypt(Key, DataAdd);
        }

        public void SetWriteAdd(int DataAdd)
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += DataAdd.ToString();
        }

        public void SetWriteAdd(float DataAdd)
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += DataAdd.ToString();
        }

        public void SetWriteAdd(double DataAdd)
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += DataAdd.ToString();
        }

        public void SetWriteAdd(bool DataAdd)
        {
            if (TextWrite.Length != 0)
            {
                TextWrite += "\n";
            }

            TextWrite += ((DataAdd) ? "True" : "False");
        }

        public void SetWriteAdd(char Key, Vector2 DataAdd)
        {
            SetWriteAdd(QEncypt.GetEncyptVector2(Key, DataAdd));
        }

        public void SetWriteAdd(char Key, Vector2Int DataAdd)
        {
            SetWriteAdd(QEncypt.GetEncyptVector2Int(Key, DataAdd));
        }

        public void SetWriteAdd(char Key, Vector3 DataAdd)
        {
            SetWriteAdd(QEncypt.GetEncyptVector3(Key, DataAdd));
        }

        public void SetWriteAdd(char Key, Vector3Int DataAdd)
        {
            SetWriteAdd(QEncypt.GetEncyptVector3Int(Key, DataAdd));
        }

        public string GetWriteString()
        {
            return TextWrite;
        }

        #endregion

        #endregion

        #region ==================================== File IO Read

        private List<string> TextRead = new List<string>();
        private int ReadRun = -1;

        #region ------------------------------------ Read Start

        public void SetReadStart(string Path)
        {
            TextRead = GetReadFromFile(Path);
        } //Call First

        public void SetReadStart(TextAsset FileTest)
        {
            TextRead = GetReadFromFile(FileTest);
        } //Call First

        private List<string> GetReadFromFile(string Path)
        {
            try
            {
                List<string> TextRead = new List<string>();
                using (StreamReader Stream = File.OpenText(Path))
                {
                    string ReadRun = "";
                    while ((ReadRun = Stream.ReadLine()) != null)
                    {
                        TextRead.Add(ReadRun);
                    }
                }
                return TextRead;
            }
            catch
            {
                Debug.LogErrorFormat("[Error] File Read Fail: {0}", Path);
                return null;
            }
        }

        private List<string> GetReadFromFile(TextAsset FileTest)
        {
            try
            {
                string ReadRun = FileTest.text;
                List<string> TextRead = QEncypt.GetDencyptString('\n', ReadRun);
                return TextRead;
            }
            catch
            {
                Debug.LogErrorFormat("[Error] File Read Fail: {0}", FileTest.name);
                return null;
            }
        }

        public void SetReadClear()
        {
            TextRead = new List<string>();
            ReadRun = -1;
        }

        #endregion

        #region ------------------------------------ Read Auto

        public void GetReadAuto()
        {
            if (ReadRun >= TextRead.Count - 1) return;
            ReadRun++;
        }

        public string GetReadAutoString()
        {
            if (ReadRun >= TextRead.Count - 1) return "";
            ReadRun++;
            return TextRead[ReadRun];
        }

        public List<string> GetReadAutoString(char Key)
        {
            if (ReadRun >= TextRead.Count - 1) return new List<string>();
            ReadRun++;
            return QEncypt.GetDencyptString(Key, TextRead[ReadRun]);
        }

        public List<int> GetReadAutoInt(char Key)
        {
            if (ReadRun >= TextRead.Count - 1) return new List<int>();
            ReadRun++;
            return QEncypt.GetDencyptInt(Key, TextRead[ReadRun]);
        }

        public List<float> GetReadAutoFloat(char Key)
        {
            if (ReadRun >= TextRead.Count - 1) return new List<float>();
            ReadRun++;
            return QEncypt.GetDencyptFloat(Key, TextRead[ReadRun]);
        }

        public List<bool> GetReadAutoBool(char Key)
        {
            if (ReadRun >= TextRead.Count - 1) return new List<bool>();
            ReadRun++;
            return QEncypt.GetDencyptBool(Key, TextRead[ReadRun]);
        }

        public int GetReadAutoInt()
        {
            if (ReadRun >= TextRead.Count - 1) return 0;
            ReadRun++;
            return int.Parse(TextRead[ReadRun]);
        }

        public float GetReadAutoFloat()
        {
            if (ReadRun >= TextRead.Count - 1) return 0f;
            ReadRun++;
            return float.Parse(TextRead[ReadRun]);
        }

        public double GetReadAutoDouble()
        {
            if (ReadRun >= TextRead.Count - 1) return 0f;
            ReadRun++;
            return double.Parse(TextRead[ReadRun]);
        }

        public bool GetReadAutoBool()
        {
            if (ReadRun >= TextRead.Count - 1) return false;
            ReadRun++;
            return TextRead[ReadRun] == "True";
        }

        public Vector2 GetReadAutoVector2(char Key)
        {
            if (ReadRun >= TextRead.Count - 1) return new Vector2();
            ReadRun++;
            return QEncypt.GetDencyptVector2(Key, TextRead[ReadRun]);
        }

        public Vector2Int GetReadAutoVector2Int(char Key)
        {
            if (ReadRun >= TextRead.Count - 1) return new Vector2Int();
            ReadRun++;
            return QEncypt.GetDencyptVector2Int(Key, TextRead[ReadRun]);
        }

        public Vector3 GetReadAutoVector3(char Key)
        {
            if (ReadRun >= TextRead.Count - 1) return new Vector3();
            ReadRun++;
            return QEncypt.GetDencyptVector3(Key, TextRead[ReadRun]);
        }

        public Vector3Int GetReadAutoVector3Int(char Key)
        {
            if (ReadRun >= TextRead.Count - 1) return new Vector3Int();
            ReadRun++;
            return QEncypt.GetDencyptVector3Int(Key, TextRead[ReadRun]);
        }

        public bool GetReadAutoEnd()
        {
            return ReadRun >= TextRead.Count - 1;
        }

        public int GetReadAutoCurrent()
        {
            return ReadRun;
        }

        public List<string> GetRead()
        {
            return TextRead;
        }

        #endregion

        #endregion
    }

    //==================================== JSON

    public class QJSON
    {
        //NOTE:
        //Type "TextAsset" is a "Text Document" File or "*.txt" File

        //SAMPLE:
        //ClassData Data = ClassFileIO.GetDatafromJson<ClassData>(JsonDataTextDocument);

        public static ClassData GetDataJson<ClassData>(TextAsset JsonDataTextDocument)
        {
            return GetDataJson<ClassData>(JsonDataTextDocument.text);
        }

        public static ClassData GetDataJson<ClassData>(string JsonData)
        {
            return JsonUtility.FromJson<ClassData>(JsonData);
        }

        public static string GetDataJson(object JsonDataClass)
        {
            return JsonUtility.ToJson(JsonDataClass);
        }
    }

    //==================================== Enum

    public class QEnum
    {
        #region ==================================== Name

        public static string[] GetEnumArray<EnumType>()
        {
            return Enum.GetNames(typeof(EnumType));
        }

        public static List<string> GetEnumList<EnumType>()
        {
            return Enum.GetNames(typeof(EnumType)).ToList();
        }

        #endregion

        #region ==================================== Index

        public static int[] GetEnumArrayIndex<EnumType>()
        {
            return Enum.GetValues(typeof(EnumType)).Cast<int>().ToArray();
        }

        public static List<int> GetEnumListIndex<EnumType>()
        {
            return Enum.GetValues(typeof(EnumType)).Cast<int>().ToList();
        }

        #endregion

        #region ==================================== Index - Name

        public static string GetEnumString<EnumType>(int Index)
        {
            return Enum.GetName(typeof(EnumType), Index);
        }

        #endregion
    }

    //==================================== Color

    public class QColor
    {
        #region ==================================== Color

        //Key "ref" use to immediately save value to primary varible param!

        public static void SetColor(ref Color Color, float A)
        {
            Color = new Color(Color.r, Color.g, Color.b, A);
        }

        public static Color GetColor(Color Color, float A)
        {
            return new Color(Color.r, Color.g, Color.b, A);
        }

        #endregion

        #region ==================================== Color - Hex Color

        private static string GetColorHex(Color Color)
        {
            return ColorUtility.ToHtmlStringRGB(Color);
        }

        public static string GetColorHexCode(Color Color)
        {
            string ColorHex = GetColorHex(Color);
            string ColorHexCode = string.Format("#{0}", ColorHex);
            return ColorHexCode;
        }

        public static string GetColorHexFormat(Color Color, string Text)
        {
            string ColorHex = GetColorHex(Color);
            string TextFormat = string.Format("<#{0}>{1}</color>", ColorHex, Text);
            return TextFormat;
        }

        #endregion

        #region ==================================== Color - Mesh Renderer & Material

        //Can be applied to Spine Material!!

        public static void SetMaterial(MeshRenderer MessRenderer, float Alpha)
        {
            if (!MessRenderer.sharedMaterial.HasProperty("_Color"))
                return;

            Color Color = MessRenderer.material.color;
            SetColor(ref Color, Alpha);
            MessRenderer.material.color = Color;
        }

        public static void SetMaterial(MeshRenderer MessRenderer, Color Color, float Alpha)
        {
            if (!MessRenderer.sharedMaterial.HasProperty("_Color"))
                return;

            SetColor(ref Color, Alpha);
            MessRenderer.material.color = Color;
        }

        public static void SetMaterialTint(MeshRenderer MessRenderer, Color Color)
        {
            //Shader of Material must set to "Tint"!!

            MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();

            MessRenderer.GetPropertyBlock(MaterialPropertyBlock);

            int IdColor = Shader.PropertyToID("_Color");
            int IdBlack = Shader.PropertyToID("_Black");

            MaterialPropertyBlock.SetColor(IdColor, Color);
            MaterialPropertyBlock.SetColor(IdBlack, Color.black); //Should be "Color.black"!!

            MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
        }

        public static void SetMaterialTint(MeshRenderer MessRenderer, Color Color, Color Black)
        {
            //Shader of Material must set to "Tint"!!

            MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();

            MessRenderer.GetPropertyBlock(MaterialPropertyBlock);

            int IdColor = Shader.PropertyToID("_Color");
            int IdBlack = Shader.PropertyToID("_Black");

            MaterialPropertyBlock.SetColor(IdColor, Color);
            MaterialPropertyBlock.SetColor(IdBlack, Black); //Should be "Color.black"!!

            MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
        }

        public static void SetMaterialFill(MeshRenderer MessRenderer, Color FillColor, float FillPhase = 1)
        {
            //Shader of Material must set to "Fill"!!

            MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();

            MessRenderer.GetPropertyBlock(MaterialPropertyBlock);

            int IdFillColor = Shader.PropertyToID("_FillColor");
            int IdFillPhase = Shader.PropertyToID("_FillPhase");

            MaterialPropertyBlock.SetColor(IdFillColor, FillColor);
            MaterialPropertyBlock.SetFloat(IdFillPhase, FillPhase);

            MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
        }

        #endregion

        #region ==================================== Color - SpriteRenderer

        public static void SetSprite(SpriteRenderer SpriteRenderer, float Alpha)
        {
            Color Color = SpriteRenderer.color;
            SetColor(ref Color, Alpha);
            SpriteRenderer.color = Color;
        }

        public static void SetSprite(SpriteRenderer SpriteRenderer, Color Color, float Alpha)
        {
            SetColor(ref Color, Alpha);
            SpriteRenderer.color = Color;
        }

        #endregion

        #region ==================================== Color - Image

        public static void SetSprite(Image Image, float Alpha)
        {
            Color Color = Image.color;
            SetColor(ref Color, Alpha);
            Image.color = Color;
        }

        public static void SetSprite(Image Image, Color Color, float Alpha)
        {
            SetColor(ref Color, Alpha);
            Image.color = Color;
        }

        #endregion
    }

    //==================================== Encypt

    public class QEncypt
    {
        #region ==================================== String Split

        public static string[] GetStringSplitArray(string FatherString, char Key)
        {
            return FatherString.Split(Key);
        }

        public static List<string> GetStringSplitList(string FatherString, char Key)
        {
            string[] SplitArray = GetStringSplitArray(FatherString, Key);

            List<string> SplitString = new List<string>();

            SplitString.AddRange(SplitArray);

            return SplitString;
        }

        #endregion

        #region ==================================== String Data

        #region ------------------------------------ String Data Main

        #region String Data Main Encypt

        //List!!

        public static string GetEncypt(char Key, List<string> Data)
        {
            string Encypt = "";

            for (int i = 0; i < Data.Count; i++)
            {
                GetEncyptAdd(Key, Encypt, Data[i], out Encypt);
            }

            return Encypt;
        }

        public static string GetEncypt(char Key, List<int> Data)
        {
            string Encypt = "";

            for (int i = 0; i < Data.Count; i++)
            {
                GetEncyptAdd(Key, Encypt, Data[i], out Encypt);
            }

            return Encypt;
        }

        public static string GetEncypt(char Key, List<float> Data)
        {
            string Encypt = "";

            for (int i = 0; i < Data.Count; i++)
            {
                GetEncyptAdd(Key, Encypt, Data[i], out Encypt);
            }

            return Encypt;
        }

        public static string GetEncypt(char Key, List<bool> Data)
        {
            string Encypt = "";

            for (int i = 0; i < Data.Count; i++)
            {
                GetEncyptAdd(Key, Encypt, Data[i], out Encypt);
            }

            return Encypt;
        }

        //Array - Params!!

        public static string GetEncypt(char Key, params string[] Data)
        {
            string Encypt = "";

            for (int i = 0; i < Data.Length; i++)
            {
                GetEncyptAdd(Key, Encypt, Data[i], out Encypt);
            }

            return Encypt;
        }

        public static string GetEncypt(char Key, params int[] Data)
        {
            string Encypt = "";

            for (int i = 0; i < Data.Length; i++)
            {
                GetEncyptAdd(Key, Encypt, Data[i], out Encypt);
            }

            return Encypt;
        }

        public static string GetEncypt(char Key, params float[] Data)
        {
            string Encypt = "";

            for (int i = 0; i < Data.Length; i++)
            {
                GetEncyptAdd(Key, Encypt, Data[i], out Encypt);
            }

            return Encypt;
        }

        public static string GetEncypt(char Key, params bool[] Data)
        {
            string Encypt = "";

            for (int i = 0; i < Data.Length; i++)
            {
                GetEncyptAdd(Key, Encypt, Data[i], out Encypt);
            }

            return Encypt;
        }

        #endregion

        #region String Data Main Add Encypt

        //Single

        public static void GetEncyptAdd(char Key, string Data, string DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd;
        }

        public static void GetEncyptAdd(char Key, string Data, int DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd.ToString();
        }

        public static void GetEncyptAdd(char Key, string Data, float DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd.ToString();
        }

        public static void GetEncyptAdd(char Key, string Data, bool DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + ((DataAdd) ? "1" : "0");
        }

        //List

        public static void GetEncyptAdd(char Key, string Data, List<string> DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
        }

        public static void GetEncyptAdd(char Key, string Data, List<int> DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
        }

        public static void GetEncyptAdd(char Key, string Data, List<float> DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
        }

        public static void GetEncyptAdd(char Key, string Data, List<bool> DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
        }

        //Array

        public static void GetEncyptAdd(char Key, string Data, string[] DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
        }

        public static void GetEncyptAdd(char Key, string Data, int[] DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
        }

        public static void GetEncyptAdd(char Key, string Data, float[] DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
        }

        public static void GetEncyptAdd(char Key, string Data, bool[] DataAdd, out string DataFinal)
        {
            DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
        }

        #endregion

        #region String Data Main Dencypt

        public static List<string> GetDencyptString(char Key, string Data)
        {
            if (Data.Equals(""))
            {
                return new List<string>();
            }

            return GetStringSplitList(Data, Key);
        }

        public static List<int> GetDencyptInt(char Key, string Data)
        {
            if (Data.Equals(""))
            {
                return new List<int>();
            }

            List<string> DataString = GetDencyptString(Key, Data);

            List<int> DataInt = new List<int>();

            for (int i = 0; i < DataString.Count; i++)
            {
                DataInt.Add(int.Parse(DataString[i]));
            }

            return DataInt;
        }

        public static List<float> GetDencyptFloat(char Key, string Data)
        {
            if (Data.Equals(""))
            {
                return new List<float>();
            }

            List<string> DataString = GetStringSplitList(Data, Key);

            List<float> DataFloat = new List<float>();

            for (int i = 0; i < DataString.Count; i++)
            {
                DataFloat.Add(float.Parse(DataString[i]));
            }

            return DataFloat;
        }

        public static List<bool> GetDencyptBool(char Key, string Data)
        {
            if (Data.Equals(""))
            {
                return new List<bool>();
            }

            List<string> DataString = GetStringSplitList(Data, Key);

            List<bool> DataBool = new List<bool>();

            for (int i = 0; i < DataString.Count; i++)
            {
                string Bool = DataString[i];

                if (Bool == "1")
                {
                    DataBool.Add(true);
                }
                else
                if (Bool == "0")
                {
                    DataBool.Add(false);
                }
            }

            return DataBool;
        }

        #endregion

        #endregion

        #region ------------------------------------ String Data Vector

        #region String Data Vector Encypt

        public static string GetEncyptVector2(char Key, Vector2 Data)
        {
            return Data.x.ToString() + Key + Data.y.ToString();
        }

        public static string GetEncyptVector3(char Key, Vector3 Data)
        {
            return Data.x.ToString() + Key + Data.y.ToString() + Key + Data.z.ToString();
        }

        public static string GetEncyptVector2Int(char Key, Vector2Int Data)
        {
            return Data.x.ToString() + Key + Data.y.ToString();
        }

        public static string GetEncyptVector3Int(char Key, Vector3Int Data)
        {
            return Data.x.ToString() + Key + Data.y.ToString() + Key + Data.z.ToString();
        }

        #endregion

        #region String Data Vector Dencypt

        public static Vector2 GetDencyptVector2(char Key, string Data)
        {
            List<float> Dencypt = GetDencyptFloat(Key, Data);

            return new Vector2(Dencypt[0], Dencypt[1]);
        }

        public static Vector3 GetDencyptVector3(char Key, string Data)
        {
            List<float> Dencypt = GetDencyptFloat(Key, Data);

            return new Vector3(Dencypt[0], Dencypt[1], Dencypt[2]);
        }

        public static Vector2Int GetDencyptVector2Int(char Key, string Data)
        {
            List<int> Dencypt = GetDencyptInt(Key, Data);

            return new Vector2Int(Dencypt[0], Dencypt[1]);
        }

        public static Vector3Int GetDencyptVector3Int(char Key, string Data)
        {
            List<int> Dencypt = GetDencyptInt(Key, Data);

            return new Vector3Int(Dencypt[0], Dencypt[1], Dencypt[2]);
        }

        #endregion

        #endregion

        #endregion
    }

    //==================================== List

    public class QList
    {
        #region Get Data

        public static List<T> GetData<T>(List<T> Data)
        {
            //Use to Get Data from List, not it's Memory Pointer!!
            List<T> DataGet = new List<T>();
            foreach (T Value in Data)
                DataGet.Add(Value);
            return DataGet;
        }

        public static T[] GetData<T>(T[] Data)
        {
            //Use to Get Data from List, not it's Memory Pointer!!
            T[] DataGet = new T[Data.Length];
            for (int i = 0; i < Data.Length; i++)
                DataGet[i] = Data[i];
            return DataGet;
        }

        #endregion

        #region Find Data

        public static T GetComponent<T>(List<GameObject> DataList, GameObject DataFind)
        {
            return DataList.Find(t => DataFind).GetComponent<T>();
        }

        public static T GetComponent<T>(List<Transform> DataList, Transform DataFind)
        {
            return DataList.Find(t => DataFind).GetComponent<T>();
        }

        #endregion

        #region Random

        public static int GetIndexRandom(params int[] Percent)
        {
            //Get random index from percent index list!
            List<(int Index, int Percent)> ListPercent = new List<(int Index, int Percent)>();

            int MaxPercent = 0;
            int MaxIndex = -1;

            int SumPercent = 0;
            for (int i = 0; i < Percent.Length; i++)
            {
                if (Percent[i] >= 100)
                    return i; //Get index of 100% percent!

                ListPercent.Add((i, Percent[i]));
                SumPercent += Percent[i];

                if (Percent[i] > MaxPercent)
                {
                    MaxPercent = Percent[i];
                    MaxIndex = i;
                }
            }

            int SumFixed = 100 - SumPercent;
            if (SumFixed != 0)
            {
                for (int i = 0; i < ListPercent.Count; i++)
                {
                    int ChildPercent = ListPercent[i].Percent + (int)(1.0f * SumFixed / ListPercent.Count);
                    ListPercent[i] = (ListPercent[i].Index, ChildPercent);
                }
            }

            ListPercent = ListPercent.OrderBy(t => t.Percent).ToList(); //Order by!

            int RandomPercent;
            int RandomLast = -1;
            for (int i = 0; i < 10; i++)
            {
                int RandomCurrent = Random.Range(0, 100);

                if (RandomLast == -1)
                    RandomLast = RandomCurrent;
                else
                if (RandomLast == RandomCurrent)
                    continue;
                else
                    RandomLast = RandomCurrent;
            }
            RandomPercent = RandomLast;

            int randomNumber = 0;
            int lastNumber = -1;
            int maxAttempts = 10;
            for (int i = 0; randomNumber == lastNumber && i < maxAttempts; i++)
            {
                randomNumber = Random.Range(0, 10);
            }
            lastNumber = randomNumber;

            int CheckPercent = 0;
            foreach (var Child in ListPercent)
            {
                CheckPercent += Child.Percent;

                if (CheckPercent < RandomPercent)
                    continue;

                return Child.Index; //Get index of higher than random percent!
            }

            return MaxIndex; //Get index of highest percent!
        }

        #endregion
    }

    //==================================== Application

    public class QApplication
    {
        public static void SetTimeScale(float TimeScale = 1)
        {
            Time.timeScale = TimeScale;
        }

        public static void SetFrameRateTarget(int FrameRateTarget = 60)
        {
            Application.targetFrameRate = FrameRateTarget;
        }

        public static void SetPhysicSimulation(SimulationMode2D Mode)
        {
            //From Editor Unity Window: Edit/Project Setting/Physic 2D/Simulation Mode.
            //Mode Fixed Update: Physic will be caculated every Fixed Delta Time, after FixedUpdate methode called (By Default of Unity).
            //Mode Update: Physic will caculated every Delta Time, after every Update methode called (Higher Frame Rate, higher correct Physic caculated, but consumed more CPU resources).
            //Mode Script: Unknow?
            Physics2D.simulationMode = Mode;
        }
    }

    //==================================== Scene

    public class QScene
    {
        public static void SetSceneChance(string SceneName, LoadSceneMode LoadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(SceneName, LoadSceneMode);
        }

        public static (int Index, string Name) GetSceneCurrent()
        {
            return (GetSceneCurrentBuildIndex(), GetSceneCurrentName());
        }

        public static string GetSceneCurrentName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public static int GetSceneCurrentBuildIndex()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }

    //==================================== Control Mouse & Keyboard & Device

    public class QControl
    {
        #region ==================================== Mouse

        public static void SetMouseVisible(bool MouseVisble)
        {
            Cursor.visible = MouseVisble;
        }

        #endregion

        #region ==================================== Keyboard

        public static string GetKeyboardSimple(KeyCode KeyCode)
        {
            switch (KeyCode)
            {
                case KeyCode.Escape:
                    return "Esc";
                case KeyCode.Return:
                    return "Enter";
                case KeyCode.Delete:
                    return "Del";
                case KeyCode.Backspace:
                    return "B-Space";

                case KeyCode.Mouse0:
                    return "L-Mouse";
                case KeyCode.Mouse1:
                    return "R-Mouse";
                case KeyCode.Mouse2:
                    return "M-Mouse";

                case KeyCode.LeftBracket:
                    return "[";
                case KeyCode.RightBracket:
                    return "]";

                case KeyCode.LeftCurlyBracket:
                    return "{";
                case KeyCode.RightCurlyBracket:
                    return "}";

                case KeyCode.LeftParen:
                    return "(";
                case KeyCode.RightParen:
                    return ")";

                case KeyCode.LeftShift:
                    return "L-Shift";
                case KeyCode.RightShift:
                    return "R-Shift";

                case KeyCode.LeftAlt:
                    return "L-Alt";
                case KeyCode.RightAlt:
                    return "R-Alt";

                case KeyCode.PageUp:
                    return "Page-U";
                case KeyCode.PageDown:
                    return "Page-D";
            }

            return KeyCode.ToString();
        }

        #endregion

        #region ==================================== Device (?)

        #region Vibrator

        public static bool VibrateHandle = true;

#if UNITY_ANDROID && !UNITY_EDITOR
        public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
        public static AndroidJavaClass unityPlayer;
        public static AndroidJavaObject currentActivity;
        public static AndroidJavaObject vibrator;
#endif

        public static void SetDeviceVibrate()
        {
            if (VibrateHandle)
            {
                Handheld.Vibrate();
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
                    vibrator.Call("vibrate");
                else
                    Handheld.Vibrate();
            }
        }

        public static void SetDeviceVibrate(float TimeMilisecond)
        {
            if (VibrateHandle)
            {
                Handheld.Vibrate();
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
                    vibrator.Call("vibrate", TimeMilisecond);
                else
                    Handheld.Vibrate();
            }
        }

        public static void SetDeviceVibrate(float[] Pattern, int Repeat) //???
        {
            if (VibrateHandle)
            {
                Handheld.Vibrate();
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
                    vibrator.Call("vibrate", Pattern, Repeat);
                else
                    Handheld.Vibrate();
            }
        }

        public static void SetDeviceVibrateCancel()
        {
            if (VibrateHandle)
            {
                //...
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
                    vibrator.Call("cancel");
            }
        }

        #endregion

        #endregion
    }

    //==================================== Gizmos

    public class QGizmos
    {
        #region ==================================== Primary

        public static void SetLine(Vector3 PosA, Vector3 PosB, Color Color, float SizePoint = 0f)
        {
            Gizmos.color = Color;
            Gizmos.DrawLine(PosA, PosB);

            if (SizePoint != 0)
            {
                SetWireSphere(PosA, SizePoint, Color);
                SetWireSphere(PosB, SizePoint, Color);
            }
        }

        public static void SetRay(Vector3 Pos, Vector3 Dir, float Distance, Color Color, float SizePoint = 0f)
        {
            Vector3 PosA = Pos;
            Vector3 PosB = PosA + Dir.normalized * Distance;

            Gizmos.color = Color;
            Gizmos.DrawLine(PosA, PosB);

            if (SizePoint != 0)
            {
                SetWireSphere(PosA, SizePoint, Color);
                SetWireSphere(PosB, SizePoint, Color);
            }
        }

        public static void SetWireCube(Vector3 Pos, Vector3 Size, Color Color)
        {
            Gizmos.color = Color;
            Gizmos.DrawWireCube(Pos, Size);
        }

        public static void SetWireCube(Vector2 Pos, Vector3 Size, Color Color)
        {
            Gizmos.color = Color;
            Gizmos.DrawWireCube(Pos, Size);
        }

        public static void SetWireSphere(Vector3 Pos, float Size, Color Color)
        {
            Gizmos.color = Color;
            Gizmos.DrawWireSphere(Pos, Size);
        }

        public static void SetWireSphere(Vector2 Pos, float Size, Color Color)
        {
            Gizmos.color = Color;
            Gizmos.DrawWireSphere(Pos, Size);
        }

        #endregion

        #region ==================================== Cast

        public static void SetBoxcast(Vector3 PosStart, Vector3 PosEnd, Vector3 Size, Color Color)
        {
            SetLine(PosStart, PosEnd, Color);
            SetWireCube(PosEnd, Size, Color);
        }

        public static void SetBoxcast(Vector3 PosStart, Vector3 PosEnd, Vector3 Size, float Distance, Color Color)
        {
            Vector3 Dir = (PosEnd - PosStart).normalized;
            SetLine(PosStart, PosStart + Dir * Distance, Color);
            SetWireCube(PosStart + Dir * Distance, Size, Color);
        }

        public static void SetBoxcastDir(Vector3 PosStart, Vector3 Dir, Vector3 Size, float Distance, Color Color)
        {
            SetLine(PosStart, PosStart + Dir * Distance, Color);
            SetWireCube(PosStart + Dir * Distance, Size, Color);
        }

        public static void SetSpherecast(Vector3 PosStart, Vector3 PosEnd, float Size, Color Color)
        {
            SetLine(PosStart, PosEnd, Color);
            SetWireSphere(PosEnd, Size, Color);
        }

        public static void SetSpherecast(Vector3 PosStart, Vector3 PosEnd, float Size, float Distance, Color Color)
        {
            Vector3 Dir = (PosEnd - PosStart).normalized;
            SetLine(PosStart, PosStart + Dir * Distance, Color);
            SetWireSphere(PosStart + Dir * Distance, Size, Color);
        }

        public static void SetSpherecastDir(Vector3 PosStart, Vector3 Dir, float Size, float Distance, Color Color)
        {
            SetLine(PosStart, PosStart + Dir * Distance, Color);
            SetWireSphere(PosStart + Dir * Distance, Size, Color);
        }

        #endregion

        #region ==================================== Camera

        public static void SetCamera(Color Color)
        {
            SetCamera(UnityEngine.Camera.main, Color);
        }

        public static void SetCamera(Camera Camera, Color Color)
        {
            Gizmos.color = Color;

            Vector2 Resolution = QCamera.GetCameraSizeUnit();
            Gizmos.DrawWireCube((Vector2)Camera.transform.position, Resolution);
        }

        #endregion

        #region ==================================== Sprite

        public static void SetSprite2D(SpriteRenderer SpriteRenderer, Color Color)
        {
            Vector2 Size = QSprite.GetSpriteSizeUnit(SpriteRenderer.sprite);
            Vector2 Pos = (Vector2)SpriteRenderer.transform.position;

            Vector2 TL = Vector2.up * Size.y / 2 + Vector2.left * Size.x / 2;
            Vector2 TR = Vector2.up * Size.y / 2 + Vector2.right * Size.x / 2;
            Vector2 BL = Vector2.down * Size.y / 2 + Vector2.left * Size.x / 2;
            Vector2 BR = Vector2.down * Size.y / 2 + Vector2.right * Size.x / 2;

            SetLine(Pos + TL, Pos + TR, Color);
            SetLine(Pos + TR, Pos + BR, Color);
            SetLine(Pos + BR, Pos + BL, Color);
            SetLine(Pos + BL, Pos + TL, Color);
        }

        #endregion

        #region ==================================== Collider

        #region Collider Pos Self

        public static void SetCollider2D(Collider2D Collider, Color Color)
        {
            SetWireCube(Collider.bounds.center, (Vector2)Collider.bounds.size, Color);
        }

        public static void SetCollider2D(BoxCollider2D Collider, Color Color)
        {
            SetWireCube(Collider.bounds.center, (Vector2)Collider.bounds.size + Vector2.one * Collider.edgeRadius * 2, Color);
        }

        public static void SetCollider2D(CircleCollider2D Collider, Color Color)
        {
            SetWireSphere(Collider.bounds.center, Collider.radius, Color);
        }

        public static void SetCollider2D(PolygonCollider2D Collider, Color Color)
        {
            Gizmos.color = Color;

            for (int i = 1; i < Collider.points.Length; i++)
            {
                Gizmos.DrawLine(Collider.points[i - 1], Collider.points[i]);
            }
            Gizmos.DrawLine(Collider.points[0], Collider.points[Collider.points.Length - 1]);
        }

        public static void SetCollider2D(CompositeCollider2D Collider, bool Square, Color Color)
        {
            List<List<Vector2>> Points = QCollider2D.GetPointsBorderPos(Collider, Square);

            if (Points.Count == 0)
                return;

            Vector2 Center = Collider.transform.position;

            for (int Group = 0; Group < Points.Count; Group++)
            {
                for (int Index = 1; Index < Points[Group].Count; Index++)
                    SetLine(Center + Points[Group][Index - 1], Center + Points[Group][Index], Color.red, 0.1f);
                SetLine(Center + Points[Group][0], Center + Points[Group][Points[Group].Count - 1], Color.red, 0.1f);
            }
        }

        #endregion

        #region Collider Pos Free

        public static void SetCollider2D(Vector2 Pos, Collider2D Collider, Color Color)
        {
            SetWireCube(Pos, (Vector2)Collider.bounds.size, Color);
        }

        public static void SetCollider2D(Vector2 Pos, BoxCollider2D Collider, Color Color)
        {
            SetWireCube(Pos, (Vector2)Collider.bounds.size + Vector2.one * Collider.edgeRadius * 2, Color);
        }

        public static void SetCollider2D(Vector2 Pos, CircleCollider2D Collider, Color Color)
        {
            SetWireSphere(Pos, Collider.radius, Color);
        }

        #endregion

        #endregion
    }

    //==================================== Date Time

    public class QDateTime
    {
        public const string DD_MM_YYYY = "dd/MM/yyyy";
        public const string YYYY_MM_DD = "yyyy/MM/dd";

        public const string DDD_DD_MM_YYYY = "ddd dd/MM/yyyy";

        public const string HH_MM_SS = "HH:mm:ss";
        public const string HH_MM_SS_TT = "hh:mm:ss tt";

        public const string DDD_DD_MM_YYYY_HH_MM_SS_TT = "ddd dd/MM/yyyy hh:mm:ss tt";

        public static DateTime Now => DateTime.Now;

        #region ==================================== Time Format

        public static string GetFormat(DateTime Time, string FormatTime, string Special = "en-US")
        {
            if (Special != "")
                return Time.ToString(FormatTime, CultureInfo.CreateSpecificCulture(Special));
            else
                return Time.ToString(FormatTime, DateTimeFormatInfo.InvariantInfo);
        }

        public static DateTime GetConvert(string Time, string FormatTime, string Special = "en-US")
        {
            if (Special != "")
                return DateTime.ParseExact(Time, FormatTime, CultureInfo.CreateSpecificCulture(Special));
            else
                return DateTime.ParseExact(Time, FormatTime, CultureInfo.InvariantCulture);
        }

        #endregion

        #region ==================================== Time Compare

        public static (bool Prev, bool Equa, bool Next) GetCompare(DateTime TimeFrom, DateTime TimeTo)
        {
            if (TimeFrom < TimeTo)
                return (true, false, false); //Past Time!!

            if (TimeFrom > TimeTo)
                return (false, false, true); //Future Time!!

            return (false, true, false); //Now Time (Maybe not)!!
        }

        public static (bool Prev, bool Equa, bool Next) GetCompareDay(DateTime TimeFrom, DateTime TimeTo)
        {
            if (TimeFrom.Year > TimeTo.Year)
                return (false, false, true); //Future Time!!
            if (TimeFrom.Year < TimeTo.Year)
                return (true, false, false); //Past Time!!

            if (TimeFrom.Month > TimeTo.Month)
                return (false, false, true); //Future Time!!
            if (TimeFrom.Month < TimeTo.Month)
                return (true, false, false); //Past Time!!

            if (TimeFrom.Day > TimeTo.Day)
                return (false, false, true); //Future Time!!
            if (TimeFrom.Day < TimeTo.Day)
                return (true, false, false); //Past Time!!

            return (false, true, false); //Now Time (Maybe not)!!
        }

        public static int GetDay(DateTime From, DateTime To)
        {
            return (To - From).Days;
        }

        #endregion
    }

    //==================================== Time Span

    public class QTimeSpan
    {
        public const string HH_MM_SS = @"hh\:mm\:ss";
        public const string HH_MM = @"hh\:mm";
        public const string MM_SS = @"mm\:ss";
        public const string SS = @"ss";

        #region ==================================== Count Format

        public static string GetCountFormat(double Second, string FormatCount)
        {
            TimeSpan TimeConvert = TimeSpan.FromSeconds(Second);
            return TimeConvert.ToString(FormatCount);
        }

        public static long GetCountConvertSecond(int Second = 0, int Minute = 0, int Hour = 0)
        {
            return Second + (60 * Minute) + (60 * 60 * Hour);
        }

        #endregion
    }

    //==================================== Time Countdown

    public class QTimeCountdown
    {
        #region ==================================== Time Value

        public bool Active;
        public string Name;

        public double TimeRemain; //Second!!

        public string TimeShow; //Primary "hh:mm:ss"!!

        public QTimeCountdown(string Name, bool Active, double TimeRemain, string TimeShow)
        {
            this.Name = Name;
            this.Active = Active;
            this.TimeRemain = (int)TimeRemain;
            this.TimeShow = TimeShow;
        }

        #endregion

        #region ==================================== Time Function

        private const string PREF_START = "QTime-Start-";
        private const string PREF_COUNT = "QTime-Count-";

        public static IEnumerator ISetTime(long SecondMax, string Name, Action<QTimeCountdown> Active, string FormatCount = QTimeSpan.HH_MM_SS)
        {
            string REF_START_TIME = PREF_START + Name;
            string REF_COUNT_DOWN = PREF_COUNT + Name;

            DateTime StartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime TimeNow = DateTime.UtcNow;
            double Time = (TimeNow - StartTime).TotalSeconds;
            double TimeStart = double.Parse(QPlayerPrefs.GetValueString(REF_START_TIME, Time.ToString()));
            long TimeCount = (long)double.Parse(QPlayerPrefs.GetValueString(REF_COUNT_DOWN, SecondMax.ToString()));

            QPlayerPrefs.SetValue(REF_START_TIME, TimeStart.ToString());
            QPlayerPrefs.SetValue(REF_COUNT_DOWN, TimeCount.ToString());

            TimeCount -= (long)(TimeNow - StartTime.AddSeconds(TimeStart)).TotalSeconds;
            Debug.LogFormat("[Debug] QTime: {0} remain {1} second(s)!!", Name, TimeCount);

            while (TimeCount > 0)
            {
                string TimeShow = QTimeSpan.GetCountFormat(TimeCount, FormatCount);
                Active?.Invoke(new QTimeCountdown(Name, true, TimeCount, TimeShow));

                yield return new WaitForSeconds(1f);
                TimeCount--;
            }

            QPlayerPrefs.SetValueClear(REF_START_TIME);
            QPlayerPrefs.SetValueClear(REF_COUNT_DOWN);

            Active?.Invoke(new QTimeCountdown(Name, false, 0, ""));
        }

        public static bool GetTimeExist(string Name)
        {
            return QPlayerPrefs.GetValueExist(PREF_START + Name);
        }

        public static void SetTimeClear(string Name)
        {
            QPlayerPrefs.SetValueClear(PREF_START + Name);
            QPlayerPrefs.SetValueClear(PREF_COUNT + Name);
        }

        #endregion
    }

    //==================================== GUI Editor Window & GUI Editor Custom Script

#if UNITY_EDITOR

    ///<summary>
    ///Caution: Unity Editor only!
    ///</summary>
    public class QEditor
    {
        //Can be use for EditorWindow & Editor Script!!

        #region ==================================== GUI Primary

        #region ------------------------------------ Label

        public static void SetLabel(string Label, GUIStyle GUIStyle = null, params GUILayoutOption[] GUILayoutOption)
        {
            if (GUIStyle == null)
                GUILayout.Label(Label, GUILayoutOption);
            else
                GUILayout.Label(Label, GUIStyle, GUILayoutOption);
        }

        public static void SetLabel(Sprite Sprite, params GUILayoutOption[] GUILayoutOption)
        {
            GUILayout.Label(GetImage(Sprite), GUILayoutOption);
        }

        #endregion

        #region ------------------------------------ Button

        public static bool SetButton(string Label, GUIStyle GUIStyle = null, params GUILayoutOption[] GUILayoutOption)
        {
            if (GUIStyle == null)
                return GUILayout.Button(Label, GUILayoutOption);
            else
                return GUILayout.Button(Label, GUIStyle, GUILayoutOption);
        }

        public static bool SetButton(Sprite Sprite, params GUILayoutOption[] GUILayoutOption)
        {
            return GUILayout.Button(GetImage(Sprite), GUILayoutOption);
        }

        #endregion

        #region ------------------------------------ Field

        #region Field Text

        //String

        public static string SetField(string Value, GUIStyle GUIStyle = null, params GUILayoutOption[] GUILayoutOption)
        {
            if (GUIStyle == null)
                return EditorGUILayout.TextField("", Value, GUILayoutOption);
            else
                return EditorGUILayout.TextField("", Value, GUIStyle, GUILayoutOption);
        }

        public static string SetFieldPassword(string Value, GUIStyle GUIStyle = null, params GUILayoutOption[] GUILayoutOption)
        {
            if (GUIStyle == null)
                return EditorGUILayout.PasswordField("", Value, GUILayoutOption);
            else
                return EditorGUILayout.PasswordField("", Value, GUIStyle, GUILayoutOption);
        }

        //Number

        public static int SetField(int Value, GUIStyle GUIStyle = null, params GUILayoutOption[] GUILayoutOption)
        {
            if (GUIStyle == null)
                return EditorGUILayout.IntField("", Value, GUILayoutOption);
            else
                return EditorGUILayout.IntField("", Value, GUIStyle, GUILayoutOption);
        }

        public static long SetField(long Value, GUIStyle GUIStyle = null, params GUILayoutOption[] GUILayoutOption)
        {
            if (GUIStyle == null)
                return EditorGUILayout.LongField("", Value, GUILayoutOption);
            else
                return EditorGUILayout.LongField("", Value, GUIStyle, GUILayoutOption);
        }

        public static float SetField(float Value, GUIStyle GUIStyle = null, params GUILayoutOption[] GUILayoutOption)
        {
            if (GUIStyle == null)
                return EditorGUILayout.FloatField("", Value, GUILayoutOption);
            else
                return EditorGUILayout.FloatField("", Value, GUIStyle, GUILayoutOption);
        }

        public static double SetField(double Value, GUIStyle GUIStyle = null, params GUILayoutOption[] GUILayoutOption)
        {
            if (GUIStyle == null)
                return EditorGUILayout.DoubleField("", Value, GUILayoutOption);
            else
                return EditorGUILayout.DoubleField("", Value, GUIStyle, GUILayoutOption);
        }

        #endregion

        #region Field Vector

        //Vector2

        public static Vector2 SetField(Vector2 Value, params GUILayoutOption[] GUILayoutOption)
        {
            return EditorGUILayout.Vector2Field("", Value, GUILayoutOption);
        }

        public static Vector2Int SetField(Vector2Int Value, params GUILayoutOption[] GUILayoutOption)
        {
            return EditorGUILayout.Vector2IntField("", Value, GUILayoutOption);
        }

        //Vector3

        public static Vector3 SetField(Vector3 Value, params GUILayoutOption[] GUILayoutOption)
        {
            return EditorGUILayout.Vector2Field("", Value, GUILayoutOption);
        }

        public static Vector3Int SetField(Vector3Int Value, params GUILayoutOption[] GUILayoutOption)
        {
            return EditorGUILayout.Vector3IntField("", Value, GUILayoutOption);
        }

        #endregion

        #region Field Else

        public static Color SetField(Color Value, params GUILayoutOption[] GUILayoutOption)
        {
            return EditorGUILayout.ColorField(Value, GUILayoutOption);
        }

        public static GameObject SetField(GameObject Value, params GUILayoutOption[] GUILayoutOption)
        {
            return (GameObject)EditorGUILayout.ObjectField("", Value, typeof(GameObject), true, GUILayoutOption);
        }

        #endregion

        #endregion

        #region ------------------------------------ Horizontal

        public static void SetHorizontalBegin()
        {
            GUILayout.BeginHorizontal();
        }

        public static void SetHorizontalEnd()
        {
            GUILayout.EndHorizontal();
        }

        #endregion

        #region ------------------------------------ Vertical

        public static void SetVerticalBegin()
        {
            GUILayout.BeginVertical();
        }

        public static void SetVerticalEnd()
        {
            GUILayout.EndVertical();
        }

        #endregion

        #region ------------------------------------ Scroll View

        public static Vector2 SetScrollViewBegin(Vector2 ScrollPos, params GUILayoutOption[] GUILayoutOption)
        {
            return EditorGUILayout.BeginScrollView(ScrollPos, GUILayoutOption);
        }

        public static void SetScrollViewEnd()
        {
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region ------------------------------------ Popup

        public static int SetPopup(int IndexChoice, string[] ListChoice, params GUILayoutOption[] GUILayoutOption)
        {
            return EditorGUILayout.Popup("", IndexChoice, ListChoice, GUILayoutOption);
        }

        public static int SetPopup(int IndexChoice, List<string> ListChoice, params GUILayoutOption[] GUILayoutOption)
        {
            return EditorGUILayout.Popup("", IndexChoice, ListChoice.ToArray(), GUILayoutOption);
        }

        public static int SetPopup<EnumType>(int IndexChoice, params GUILayoutOption[] GUILayoutOption)
        {
            return EditorGUILayout.Popup("", IndexChoice, QEnum.GetEnumList<EnumType>().ToArray(), GUILayoutOption);
        }

        #endregion

        #region Chance Check

        public static void SetChanceCheckBegin()
        {
            EditorGUI.BeginChangeCheck();
        }

        public static bool SetChanceCheckEnd()
        {
            return EditorGUI.EndChangeCheck();
        }

        #endregion

        #region ------------------------------------ Else

        public static void SetBackground(Color Color)
        {
            GUI.backgroundColor = Color;
        }

        public static void SetSpace(float Space)
        {
            GUILayout.Space(Space);
        }

        #endregion

        #endregion

        #region ==================================== GUI Varible

        #region ------------------------------------ GUI Layout Option

        private static float WIDTH_OFFSET = 4f;

        public static GUILayoutOption GetGUILayoutWidth(EditorWindow This, float WidthPercent = 1, float WidthOffset = 0, bool HeightBase = false)
        {
            return GetGUIWidth((!HeightBase ? GetWindowWidth(This) : GetWindowHeight(This)) * WidthPercent - WidthOffset - WIDTH_OFFSET);
        }

        public static GUILayoutOption GetGUILayoutHeight(EditorWindow This, float HeightPercent = 1, float HeightOffset = 0, bool WidthBase = false)
        {
            return GetGUIHeight((!WidthBase ? GetWindowHeight(This) : GetWindowWidth(This)) * HeightPercent - HeightOffset);
        }

        #endregion

        #region ------------------------------------ GUI Panel Size

        public static float GetWindowWidth(EditorWindow This)
        {
            return This.position.width;
        }

        public static float GetWindowHeight(EditorWindow This)
        {
            return This.position.height;
        }

        #endregion

        #region ------------------------------------ GUI Panel Size Value

        public static GUILayoutOption GetGUIWidth(float Width)
        {
            return GUILayout.Width(Width);
        }

        public static GUILayoutOption GetGUIHeight(float Height)
        {
            return GUILayout.Height(Height);
        }

        #endregion

        #region ------------------------------------ GUI Style

        public static GUIStyle GetGUILabel(FontStyle FontStyle, TextAnchor Alignment)
        {
            GUIStyle GUIStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle,
                alignment = Alignment,
            };
            return GUIStyle;
        }

        public static GUIStyle GetGUITextField(FontStyle FontStyle, TextAnchor Alignment)
        {
            GUIStyle GUIStyle = new GUIStyle(GUI.skin.textField)
            {
                fontStyle = FontStyle,
                alignment = Alignment,
            };
            return GUIStyle;
        }

        public static GUIStyle GetGUITextArea(FontStyle FontStyle, TextAnchor Alignment)
        {
            GUIStyle GUIStyle = new GUIStyle(GUI.skin.textArea)
            {
                fontStyle = FontStyle,
                alignment = Alignment,
            };
            return GUIStyle;
        }

        public static GUIStyle GetGUIButton(FontStyle FontStyle, TextAnchor Alignment)
        {
            GUIStyle GUIStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle,
                alignment = Alignment,
            };
            return GUIStyle;
        }

        #endregion

        #region ------------------------------------ GUI Image

        public static GUIContent GetImage(Sprite Sprite)
        {
            Texture2D Texture = QSprite.GetTextureConvert(Sprite);

            if (Texture != null)
            {
                return new GUIContent("", (Texture)Texture);
            }
            else
            {
                return new GUIContent("");
            }
        }

        #endregion

        #endregion

        #region ==================================== GUI Control

        public static void SetUnFocus()
        {
            //Call will Lost Focus when Editor Focus on Typing or etc!!
            GUIUtility.keyboardControl = 0;
        }

        #endregion

        //#region ==================================== GUI Quick-Use!!

        //private static float WIDTH_OFFSET = 4f;

        //#region ------------------------------------ Label

        //public static void SetLabel(string Label, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUILabel(FontStyle.Bold, TextAnchor.MiddleCenter);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            SetLabel(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            SetLabel(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        SetLabel(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static void SetLabel(string Label, EditorWindow This, FontStyle FontStyle, TextAnchor TextAnchor, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUILabel(FontStyle, TextAnchor);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            SetLabel(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            SetLabel(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        SetLabel(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static void SetLabel(Sprite Sprite, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            SetLabel(Sprite, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            SetLabel(Sprite, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        SetLabel(Sprite, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //#endregion

        //#region ------------------------------------ Button

        //public static bool SetButton(string Label, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUIButton(FontStyle.Normal, TextAnchor.MiddleCenter);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetButton(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetButton(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetButton(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static bool SetButton(string Label, EditorWindow This, FontStyle FontStyle, TextAnchor TextAnchor, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUIButton(FontStyle, TextAnchor);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetButton(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetButton(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetButton(Label, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static bool SetButton(Sprite Sprite, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetButton(Sprite, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetButton(Sprite, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetButton(Sprite, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //#endregion

        //#region ------------------------------------ Field

        //#region Field Text

        ////String

        //public static string SetField(string Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUITextField(FontStyle.Normal, TextAnchor.UpperLeft);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static string SetField(string Value, EditorWindow This, FontStyle FontStyle, TextAnchor TextAnchor, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUIButton(FontStyle, TextAnchor);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static string SetFieldArea(string Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUITextArea(FontStyle.Normal, TextAnchor.UpperLeft);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static string SetFieldArea(string Value, EditorWindow This, FontStyle FontStyle, TextAnchor TextAnchor, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUIButton(FontStyle, TextAnchor);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static string SetFieldPassword(string Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUITextField(FontStyle.Normal, TextAnchor.MiddleLeft);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetFieldPassword(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetFieldPassword(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetFieldPassword(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static string SetFieldPassword(string Value, EditorWindow This, FontStyle FontStyle, TextAnchor TextAnchor, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUIButton(FontStyle, TextAnchor);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetFieldPassword(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetFieldPassword(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetFieldPassword(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        ////Number

        //public static int SetField(int Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUITextField(FontStyle.Normal, TextAnchor.MiddleLeft);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static int SetField(int Value, EditorWindow This, FontStyle FontStyle, TextAnchor TextAnchor, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUIButton(FontStyle, TextAnchor);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static long SetField(long Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUITextField(FontStyle.Normal, TextAnchor.MiddleLeft);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static long SetField(long Value, EditorWindow This, FontStyle FontStyle, TextAnchor TextAnchor, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUIButton(FontStyle, TextAnchor);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static float SetField(float Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUITextField(FontStyle.Normal, TextAnchor.MiddleLeft);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static float SetField(float Value, EditorWindow This, FontStyle FontStyle, TextAnchor TextAnchor, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUIButton(FontStyle, TextAnchor);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static double SetField(double Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUITextField(FontStyle.Normal, TextAnchor.MiddleLeft);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static double SetField(double Value, EditorWindow This, FontStyle FontStyle, TextAnchor TextAnchor, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    GUIStyle GUIStyle = GetGUIButton(FontStyle, TextAnchor);

        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GUIStyle, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //#endregion

        //#region Field Vector

        ////Vector2

        //public static Vector2 SetField(Vector2 Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static Vector2Int SetField(Vector2Int Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        ////Vector3

        //public static Vector3 SetField(Vector3 Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static Vector3Int SetField(Vector3Int Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //#endregion

        //#region Field Else

        //public static Color SetField(Color Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static GameObject SetField(GameObject Value, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return SetField(Value, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //#endregion

        //#endregion

        //#region ------------------------------------ Scroll View
        //#endregion

        //#region ------------------------------------ Popup

        //public static int SetPopup(int IndexChoice, string[] ListChoice, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return EditorGUILayout.Popup("", IndexChoice, ListChoice, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return EditorGUILayout.Popup("", IndexChoice, ListChoice, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return EditorGUILayout.Popup("", IndexChoice, ListChoice, GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static int SetPopup(int IndexChoice, List<string> ListChoice, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return EditorGUILayout.Popup("", IndexChoice, ListChoice.ToArray(), GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return EditorGUILayout.Popup("", IndexChoice, ListChoice.ToArray(), GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return EditorGUILayout.Popup("", IndexChoice, ListChoice.ToArray(), GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //public static int SetPopup<EnumType>(int IndexChoice, EditorWindow This, float WidthPercent = 1, float HeightPercent = 0, bool HeightScaleByWidth = true)
        //{
        //    if (HeightPercent != 0)
        //    {
        //        if (HeightScaleByWidth)
        //            return EditorGUILayout.Popup("", IndexChoice, QEnum.GetEnumList<EnumType>().ToArray(), GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetWidth(This) * HeightPercent));
        //        else
        //            return EditorGUILayout.Popup("", IndexChoice, QEnum.GetEnumList<EnumType>().ToArray(), GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET), GetHeight(GetHeight(This) * HeightPercent));
        //    }
        //    else
        //        return EditorGUILayout.Popup("", IndexChoice, QEnum.GetEnumList<EnumType>().ToArray(), GetWidth(GetWidth(This) * WidthPercent - WIDTH_OFFSET));
        //}

        //#endregion

        //#endregion
    }

#endif

    //==================================== Assets Database

#if UNITY_EDITOR

    ///<summary>
    ///Caution: Unity Editor only!
    ///</summary>
    public class QAssetsDatabase : QPath
    {
        //Folder "Assets" is the main root of all assets in project, that can find any assets from it.

        public const string ExamplePathAssets = "Assets/Scene";

        #region ==================================== Create Folder

        public static string SetCreateFolder(params string[] PathChildInAssets)
        {
            List<string> Path = new List<string>();

            string PathString = "";

            for (int i = 0; i < PathChildInAssets.Length; i++)
            {
                Path.Add(PathChildInAssets[i]);

                QEncypt.GetEncyptAdd('/', PathString, PathChildInAssets[i], out PathString);

                SetCreateFolderExist(Path.ToArray());
            }

            return PathString;
        }

        private static string SetCreateFolderExist(params string[] PathChildInAssets)
        {
            //If Root Folder not Exist, then can't Create new Folder from that Root Folder

            string PathInAssets = "Assets";

            for (int i = 0; i < PathChildInAssets.Length - 1; i++)
            {
                QEncypt.GetEncyptAdd('/', PathInAssets, PathChildInAssets[i], out PathInAssets);
            }

            string PathFolderInAssets = PathChildInAssets[PathChildInAssets.Length - 1];

            if (QPath.GetPathFolderExist(PathType.Assets, PathChildInAssets))
            {
                //Debug.LogWarningFormat("[Debug] Folder Exist!!\n{0}", PathInAssets + "/" + PathFolderInAssets);

                return "";
            }

            try
            {
                string PathString = AssetDatabase.CreateFolder(PathInAssets, PathFolderInAssets);

                SetRefresh();

                return AssetDatabase.GUIDToAssetPath(PathString);
            }
            catch
            {
                //Debug.LogWarningFormat("[Debug] Root Folder not Exist!!\n{0}", PathInAssets + "/" + PathFolderInAssets);

                return "";
            }
        }

        #endregion

        #region ==================================== Delete

        public static void SetDelete(PathType PathType, params string[] PathChild)
        {
            FileUtil.DeleteFileOrDirectory(QPath.GetPath(PathType, PathChild) + ".meta");
            FileUtil.DeleteFileOrDirectory(QPath.GetPath(PathType, PathChild));

            SetRefresh();
        }

        #endregion

        #region ==================================== Refresh

        public static void SetRefresh()
        {
            AssetDatabase.Refresh();
        }

        #endregion

        #region ==================================== Get

        public static List<GameObject> GetPrefab(params string[] PathChildInAssets)
        {
            string Path = QPath.GetPath(PathType.Assets, PathChildInAssets);

            if (!GetPathFolderExist(Path)) return new List<GameObject>();

            List<GameObject> ObjectsFound = new List<GameObject>();

            string[] GUIDPathUnityFound = AssetDatabase.FindAssets("t:prefab", new string[] { Path });

            foreach (string GUIDPath in GUIDPathUnityFound)
            {
                string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
                GameObject ObjectFound = AssetDatabase.LoadAssetAtPath<GameObject>(AssetsSinglePath);
                ObjectsFound.Add(ObjectFound);
            }

            return ObjectsFound;
        }

        public static List<Sprite> GetSprite(params string[] PathChildInAssets)
        {
            string Path = QPath.GetPath(PathType.Assets, PathChildInAssets);

            if (!GetPathFolderExist(Path)) return new List<Sprite>();

            List<Sprite> ObjectsFound = new List<Sprite>();

            string[] GUIDPathUnityFound = AssetDatabase.FindAssets("t:sprite", new string[] { Path });

            foreach (string GUIDPath in GUIDPathUnityFound)
            {
                string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
                Sprite ObjectFound = AssetDatabase.LoadAssetAtPath<Sprite>(AssetsSinglePath);
                ObjectsFound.Add(ObjectFound);
            }

            return ObjectsFound;
        }

        #endregion
    }

#endif

    //==================================== Encypt 256 Bit

    public class QEncypt256Bit //From: Tạ Xuân Hiển
    {
        //This constant is used to determine the keysize of the encryption algorithm in bits.
        //We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int KEY_SIZE = 256;

        //This constant determines the number of iterations for the password bytes generation function.
        private const int DERIVATION_ITERATIONS = 1000;

        public static string SetEncrypt(string Data, string Pass)
        {
            //Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            //so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = SetGenerate256BitsOfRandomEntropy();
            var ivStringBytes = SetGenerate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(Data);
            using (var password = new Rfc2898DeriveBytes(Pass, saltStringBytes, DERIVATION_ITERATIONS))
            {
                var keyBytes = password.GetBytes(KEY_SIZE / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                //Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string SetDecrypt(string Data, string Pass)
        {
            //Get the complete stream of bytes that represent:
            //[32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(Data);
            //Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KEY_SIZE / 8).ToArray();
            //Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KEY_SIZE / 8).Take(KEY_SIZE / 8).ToArray();
            //Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((KEY_SIZE / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((KEY_SIZE / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(Pass, saltStringBytes, DERIVATION_ITERATIONS))
            {
                var keyBytes = password.GetBytes(KEY_SIZE / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] SetGenerate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; //32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                //Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }

    //==================================== AIML - Machine Learning Primary

    public class QAIML
    {
        //Simple AIML: Input (0) >> Hidden (Neutral) (L-1) >> Output (L) >> Desired

        public QAIML()
        {
            SetReset();
        }

        //Number of Layer in Neural (Input - Hidden - Output) (*)
        private int m_LayerCount = 2;

        //List of Neural in Layer (Include Input - Hidden - Output) (*)
        private List<List<float>> m_Activation;

        //Number of Neural in Layer (Include Input - Hidden - Output) (0)
        private List<int> m_NeuralCount;

        //List of Weight[L][L-1] Layer L (Include Hidden - Output) (-1)
        private List<List<List<float>>> m_Weight;

        //List of Bias Layer (Include Input - Hidden) (-1)
        private List<float> m_Bias;

        //List of Sum Layer (Include Hidden - Output) (-1)
        private List<List<float>> m_Sum;

        //List of Error Layer (Include Hidden - Output) (-1)
        private List<List<float>> m_Error;

        //List of Neural Output wanted
        private List<float> m_Desired;

        /// <summary>
        /// Reset Neural Network
        /// </summary>
        public void SetReset()
        {
            m_NeuralCount = new List<int>();
            m_Activation = new List<List<float>>();
            m_Weight = new List<List<List<float>>>();
            m_Bias = new List<float>();
            m_Sum = new List<List<float>>();
            m_Error = new List<List<float>>();
            m_Desired = new List<float>();
        }

        #region ==================================== Set - Should call in order

        #region ------------------------------------ Set Primary

        #region New Version

        /// <summary>
        /// Start create new Neural Network
        /// </summary>
        /// <param name="RandomNumber">If "True", Weight & Bias will gain random value</param>
        public void SetNeuralNetworkCreate(params int[] NeutralCountEachLayer)
        {
            //Neutral Network Generate
            m_LayerCount = NeutralCountEachLayer.Length;
            for (int lay = 0; lay < m_LayerCount; lay++)
            {
                m_NeuralCount.Add(NeutralCountEachLayer[lay]);
            }

            //Activation
            m_Activation = new List<List<float>>();
            for (int lay = 0; lay < m_LayerCount; lay++)
            {
                m_Activation.Add(new List<float> { });
                for (int neu = 0; neu < m_NeuralCount[lay]; neu++)
                {
                    m_Activation[lay].Add(0);
                }
            }

            //Weight
            m_Weight = new List<List<List<float>>>();
            for (int lay = 0; lay < m_LayerCount - 1; lay++)
            {
                m_Weight.Add(new List<List<float>> { });
                for (int neuY = 0; neuY < m_NeuralCount[lay + 1]; neuY++)
                {
                    m_Weight[lay].Add(new List<float> { });
                    for (int neuX = 0; neuX < m_NeuralCount[lay]; neuX++)
                    {
                        m_Weight[lay][neuY].Add(0.0f);
                    }
                }
            }

            //Bias
            m_Bias = new List<float>();
            for (int lay = 0; lay < m_LayerCount - 1; lay++)
            {
                m_Bias.Add(0.0f);
            }

            //Sum
            m_Sum = new List<List<float>>();
            for (int lay = 1; lay < m_LayerCount; lay++)
            {
                m_Sum.Add(new List<float> { });
                for (int neu = 0; neu < m_NeuralCount[lay]; neu++)
                {
                    m_Sum[lay - 1].Add(0.0f);
                }
            }

            //Error
            m_Error = new List<List<float>>();
            for (int lay = 0; lay < m_LayerCount - 1; lay++)
            {
                m_Error.Add(new List<float> { });
                for (int neu = 0; neu < m_NeuralCount[lay + 1]; neu++)
                {
                    m_Error[lay].Add(0.0f);
                }
            }

            //Desired
            m_Desired = new List<float>();
            for (int neu = 0; neu < m_NeuralCount[m_LayerCount - 1]; neu++)
            {
                m_Desired.Add(0.0f);
            }
        }

        #endregion

        #region Old Version

        /// <summary>
        /// Set Number of Layer
        /// </summary>
        /// <param name="LayerCount"></param>
        public void SetLayerCount(int LayerCount)
        {
            QPlayerPrefs.SetValue("LC", (LayerCount < 0) ? 2 : LayerCount);
        }

        /// <summary>
        /// Set new Number of Neutral count of each Neural Layer
        /// </summary>
        /// <param name="Layer"></param>
        /// <param name="NeuralCount"></param>
        public void SetNeuralCount(int Layer, int NeuralCount)
        {
            if (Layer >= 0)
            {
                QPlayerPrefs.SetValue("NC_" + Layer.ToString(), (NeuralCount > 0) ? NeuralCount : 0);
            }
        }

        /// <summary>
        /// Start create new Neural Network
        /// </summary>
        /// <param name="RandomNumber">If "True", Weight & Bias will gain random value</param>
        public void SetNeuralNetworkCreate(bool RandomNumber = false)
        {
            //LayerCount
            this.m_LayerCount = QPlayerPrefs.GetValueInt("LC");

            //NeuralCount
            m_NeuralCount = new List<int>();
            for (int lay = 0; lay < this.m_LayerCount; lay++)
            {
                m_NeuralCount.Add(QPlayerPrefs.GetValueInt("NC_" + lay.ToString()));
            }

            //Activation
            m_Activation = new List<List<float>>();
            for (int lay = 0; lay < this.m_LayerCount; lay++)
            {
                m_Activation.Add(new List<float> { });
                for (int neu = 0; neu < m_NeuralCount[lay]; neu++)
                {
                    m_Activation[lay].Add(0);
                }
            }

            //Weight
            m_Weight = new List<List<List<float>>>();
            for (int lay = 0; lay < this.m_LayerCount - 1; lay++)
            {
                m_Weight.Add(new List<List<float>> { });
                for (int neuY = 0; neuY < m_NeuralCount[lay + 1]; neuY++)
                {
                    m_Weight[lay].Add(new List<float> { });
                    for (int neuX = 0; neuX < m_NeuralCount[lay]; neuX++)
                    {
                        if (RandomNumber)
                        {
                            System.Random Rand = new System.Random();
                            float Value = (float)((this.m_LayerCount * Rand.Next(1, 500) + m_NeuralCount[lay] * Rand.Next(500, 1000) + neuX * Rand.Next(100, 200) + neuY * Rand.Next(200, 300) + lay * Rand.Next(300, 400)) * Rand.Next(1, 50) / 100000.0) / 100.0f;
                            m_Weight[lay][neuY].Add(Value);
                        }
                        else
                        {
                            m_Weight[lay][neuY].Add(0.0f);
                        }

                    }
                }
            }

            //Bias
            m_Bias = new List<float>();
            for (int lay = 0; lay < this.m_LayerCount - 1; lay++)
            {
                if (RandomNumber)
                {
                    System.Random Rand = new System.Random();
                    float Value = 0;
                    m_Bias.Add(Value);
                }
                else
                {
                    m_Bias.Add(0.0f);
                }
            }

            //Sum
            m_Sum = new List<List<float>>();
            for (int lay = 1; lay < this.m_LayerCount; lay++)
            {
                m_Sum.Add(new List<float> { });
                for (int neu = 0; neu < m_NeuralCount[lay]; neu++)
                {
                    m_Sum[lay - 1].Add(0.0f);
                }
            }

            //Error
            m_Error = new List<List<float>>();
            for (int lay = 0; lay < this.m_LayerCount - 1; lay++)
            {
                m_Error.Add(new List<float> { });
                for (int neu = 0; neu < m_NeuralCount[lay + 1]; neu++)
                {
                    m_Error[lay].Add(0.0f);
                }
            }

            //Desired
            m_Desired = new List<float>();
            for (int neu = 0; neu < m_NeuralCount[this.m_LayerCount - 1]; neu++)
            {
                m_Desired.Add(0.0f);
            }
        }

        #endregion

        #endregion

        #region ------------------------------------ Set Runtime

        #region Set Input - Start before First Result and after every Result and Desired

        /// <summary>
        /// Set new Input
        /// </summary>
        /// <param name="InputNew"></param>
        /// <param name="InputValue"></param>
        public void SetInputLayerInput(int InputNew, float InputValue)
        {
            if (InputNew >= 0 && InputNew < m_NeuralCount[0])
            {
                m_Activation[0][InputNew] = InputValue;
            }
        }

        /// <summary>
        /// Set new Input
        /// </summary>
        /// <param name="mInputList"></param>
        public void SetInputLayerInput(List<float> InputList)
        {
            if (InputList == null)
            {
                return;
            }

            if (m_Activation[0].Count == InputList.Count)
            {
                m_Activation[0] = InputList;
            }
            else
            {
                for (int i = 0; i < InputList.Count; i++)
                {
                    m_Activation[0][i] = InputList[i];
                }
            }
        }

        /// <summary>
        /// Set Input
        /// </summary>
        /// <param name="InputList"></param>
        /// <param name="From"></param>
        public void SetInputLayerInput(List<float> InputList, int From)
        {
            if (InputList == null)
            {
                return;
            }

            for (int i = 0; i < InputList.Count; i++)
            {
                m_Activation[0][i + From] = InputList[i];
            }
        }

        #endregion

        #region Set Desired - After First Result and after every Result, AI will learn from Desired

        /// <summary>
        /// Set new Output Desired
        /// </summary>
        /// <param name="NeutralDesired"></param>
        /// <param name="ValueDesired"></param>
        public void SetInputDesired(int NeutralDesired, float ValueDesired)
        {
            if (NeutralDesired >= 0 && NeutralDesired < m_NeuralCount[m_LayerCount - 1])
            {
                m_Desired[NeutralDesired] = ValueDesired;
            }
        }

        #endregion

        /// <summary>
        /// Set new Bias in Layer
        /// </summary>
        /// <param name="Layer"></param>
        /// <param name="Bias"></param>
        public void SetInputBias(int Layer, float Bias)
        {
            if (Layer < m_LayerCount && Layer >= 0)
            {
                this.m_Bias[Layer] = Bias;
            }
        }

        /// <summary>
        /// Set new Weight with X (L-1) << Y (L)
        /// </summary>
        /// <param name="Layer">L</param>
        /// <param name="NeuralY">Y (L)</param>
        /// <param name="NeuralX">X (L-1)</param>
        /// <param name="Weight"></param>
        public void SetInputWeight(int Layer, int NeuralY, int NeuralX, float Weight)
        {
            if (Layer < m_LayerCount - 1 && Layer >= 0)
            {
                this.m_Weight[Layer][NeuralY][NeuralX] = Weight;
            }
        }

        #endregion

        #endregion

        #region ==================================== Get

        /// <summary>
        /// Get Number of Layer
        /// </summary>
        /// <returns></returns>
        public int GetOutputLayerCount()
        {
            return m_LayerCount;
        }

        /// <summary>
        /// Get Number of Neural of Layer
        /// </summary>
        /// <param name="Layer"></param>
        /// <returns></returns>
        public int GetOutputNeuralCount(int Layer)
        {
            return m_NeuralCount[Layer];
        }

        /// <summary>
        /// Get List of Neural of Input
        /// </summary>
        /// <returns></returns>
        public List<float> GetOutputLayerInput()
        {
            return m_Activation[0];
        }

        /// <summary>
        /// Get Neural from Input
        /// </summary>
        /// <param name="Neural"></param>
        /// <returns></returns>
        public float GetOutputLayer(int Neural)
        {
            return m_Activation[0][Neural];
        }

        /// <summary>
        /// Get Output Desired
        /// </summary>
        /// <returns></returns>
        public List<float> GetOutputDesired()
        {
            return m_Desired;
        }

        /// <summary>
        /// Get List of Neural of Output
        /// </summary>
        /// <returns></returns>
        public List<float> GetOutputLayerOutput()
        {
            return m_Activation[m_LayerCount - 1];
        }

        /// <summary>
        /// Get Neural of Output
        /// </summary>
        /// <param name="Neural"></param>
        /// <returns></returns>
        public float GetOutputLayerOutput(int Neural)
        {
            return m_Activation[m_LayerCount - 1][Neural];
        }

        #endregion

        #region ==================================== File

        /// <summary>
        /// Check AIML File Exist
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public bool GetFileExist(string Path)
        {
            return QFileIO.GetPathFileExist(Path);
        }

        /// <summary>
        /// Save Current AIML Data to File work on this Script
        /// </summary>
        /// <param name="Path"></param>
        public void SetFileSave(string Path)
        {
            QFileIO FileIO = new QFileIO();

            FileIO.SetWriteAdd("LayerCount:");
            FileIO.SetWriteAdd(m_LayerCount);

            FileIO.SetWriteAdd("NeuralCount:");
            for (int lay = 0; lay < m_LayerCount; lay++)
            {
                FileIO.SetWriteAdd(m_NeuralCount[lay]);
            }

            FileIO.SetWriteAdd("Bias:");
            for (int lay = 0; lay < m_LayerCount - 1; lay++)
            {
                FileIO.SetWriteAdd(m_Bias[lay]);
            }

            FileIO.SetWriteAdd("Weight:");
            for (int lay = 0; lay < m_LayerCount - 1; lay++)
            {
                //Check from Layer (L-1)
                for (int neuY = 0; neuY < m_NeuralCount[lay + 1]; neuY++)
                {
                    //Check from Neutral Y of Layer (L)
                    for (int neuX = 0; neuX < m_NeuralCount[lay]; neuX++)
                    {
                        //Check from Neutral X of Layer (L-1)
                        FileIO.SetWriteAdd(m_Weight[lay][neuY][neuX]);
                    }
                }
            }

            FileIO.SetWriteStart(Path);
        }

        /// <summary>
        /// Read AIML Data from File work on this Script
        /// </summary>
        /// <param name="Path"></param>
        public void SetFileOpen(string Path)
        {
            QFileIO FileIO = new QFileIO();

            FileIO.SetReadStart(Path);

            string FileIORead;

            FileIORead = FileIO.GetReadAutoString();
            int LayerCount = FileIO.GetReadAutoInt();
            SetLayerCount(LayerCount);

            FileIORead = FileIO.GetReadAutoString();
            for (int lay = 0; lay < LayerCount; lay++)
            {
                SetNeuralCount(lay, FileIO.GetReadAutoInt());
            }

            SetNeuralNetworkCreate(false);

            FileIORead = FileIO.GetReadAutoString();
            for (int lay = 0; lay < LayerCount - 1; lay++)
            {
                SetInputBias(lay, FileIO.GetReadAutoFloat());
            }

            FileIORead = FileIO.GetReadAutoString();
            for (int lay = 0; lay < LayerCount - 1; lay++)
            {
                //Check from Layer (L-1)
                for (int neuY = 0; neuY < m_NeuralCount[lay + 1]; neuY++)
                {
                    //Check from Neutral Y of Layer (L)
                    for (int neuX = 0; neuX < m_NeuralCount[lay]; neuX++)
                    {
                        //Check from Neutral X of Layer (L-1)
                        SetInputWeight(lay, neuY, neuX, FileIO.GetReadAutoFloat());
                    }
                }
            }
        }

        #endregion

        #region ==================================== FeedForward

        /// <summary>
        /// Caculate Sum of between two Layer X (L-1) >> Y (L)
        /// </summary>
        /// <param name="Layer"></param>
        private void SetFeedForwardSum(int Layer)
        {
            for (int neuY = 0; neuY < m_NeuralCount[Layer]; neuY++)
            {
                //Check Layer Y (L)

                //Sum = Weight * m_Activation(L-1) + Bias
                m_Sum[Layer - 1][neuY] = m_Bias[Layer - 1];

                for (int neuX = 0; neuX < m_NeuralCount[Layer - 1]; neuX++)
                {
                    //Check Layer X (L-1)
                    m_Sum[Layer - 1][neuY] += m_Weight[Layer - 1][neuY][neuX] * m_Activation[Layer - 1][neuX];
                }
            }
        }

        /// <summary>
        /// Caculate Sigmoid
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public float GetFeedForwardSigmoidSingle(float Value)
        {
            return (float)1.0 / ((float)1.0 + (float)Math.Exp(-Value));
        }

        /// <summary>
        /// Caculate Sigmoid from Sum of Layer Y (L)
        /// </summary>
        /// <param name="Layer"></param>
        private void SetFeedForwardSigmoid(int Layer)
        {
            for (int neuY = 0; neuY < m_NeuralCount[Layer]; neuY++)
            {
                //Check Layer Y (L)

                //m_Activation(L) = Sigmoid(Sum)
                m_Activation[Layer][neuY] = GetFeedForwardSigmoidSingle(m_Sum[Layer - 1][neuY]);
            }
        }

        /// <summary>
        /// Active FeedForward
        /// </summary>
        public void SetFeedForward()
        {
            for (int lay = 1; lay < m_LayerCount; lay++)
            {
                SetFeedForwardSum(lay);
                SetFeedForwardSigmoid(lay);
            }
        }

        #endregion

        #region ==================================== BackPropagation

        /// <summary>
        /// Caculate Error between Layer Output >> Desired
        /// </summary>
        private void SetBackPropagationErrorOuput()
        {
            int layerY = m_LayerCount - 1;
            for (int neuY = 0; neuY < m_NeuralCount[layerY]; neuY++)
            {
                //Check Layer Y (L) with Desired
                m_Error[layerY - 1][neuY] = -(m_Desired[neuY] - m_Activation[layerY][neuY]);
            }
        }

        /// <summary>
        /// Caculate Sigmoid
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public float GetBackPropagationSigmoidSingle(float Value)
        {
            float Sigmoid = GetFeedForwardSigmoidSingle(Value);
            return Sigmoid * ((float)1.0 - Sigmoid);
        }

        /// <summary>
        /// Caculate Error between Layer Y (L) >> Layer Z (L+1)
        /// </summary>
        /// <param name="Layer"></param>
        private void SetBackPropagationErrorHidden(int Layer)
        {
            int layerZ = Layer + 1;
            int layerY = Layer;
            for (int neuY = 0; neuY < m_NeuralCount[layerY]; neuY++)
            {
                //Check Layer Y (L) with Layer Z (L+1)
                m_Error[layerY - 1][neuY] = 0;

                for (int neuZ = 0; neuZ < m_NeuralCount[layerZ]; neuZ++)
                {
                    //Check Layer Y (L) with Layer Z (L+1)
                    m_Error[layerY - 1][neuY] += m_Error[layerZ - 1][neuZ] * GetBackPropagationSigmoidSingle(m_Sum[layerZ - 1][neuZ]) * m_Weight[layerZ - 1][neuZ][neuY];
                }
            }
        }

        /// <summary>
        /// Set Weight Layer
        /// </summary>
        /// <param name="Layer"></param>
        private void SetBackPropagationUpdate(int Layer)
        {
            //Layer Output
            int layerX = Layer - 1;
            int layerY = Layer;
            for (int neuX = 0; neuX < m_NeuralCount[layerX]; neuX++)
            {
                //Check Layer X (L-1) >> Layer Y (L)
                for (int neuY = 0; neuY < m_NeuralCount[layerY]; neuY++)
                {
                    //Check Layer X (L-1) >> Layer Y (L)
                    m_Weight[layerY - 1][neuY][neuX] -= (float)0.5 * (m_Error[layerY - 1][neuY] * GetBackPropagationSigmoidSingle(m_Sum[layerY - 1][neuY]) * m_Activation[layerX][neuX]);
                }
            }
        }

        /// <summary>
        /// Active BackPropagation
        /// </summary>
        public void SetBackPropagation()
        {
            for (int lay = m_LayerCount - 1; lay > 0; lay--)
            {
                //Check Layer X (L-1) >> Layer Y (L) >> Layer Z (L+1)

                //Caculate Error
                if (lay == m_LayerCount - 1)
                {
                    SetBackPropagationErrorOuput();
                }
                else
                {
                    SetBackPropagationErrorHidden(lay);
                }
            }

            for (int lay = m_LayerCount - 1; lay > 0; lay--)
            {
                //Check Layer X (L-1) >> Layer Y (L) >> Layer Z (L+1)

                //Set
                SetBackPropagationUpdate(lay);
            }
        }

        #endregion

        #region ==================================== Delay

        /// <summary>
        /// Delay Time
        /// </summary>
        private int m_BrainDelayTime = 3;
        private int m_BrainDelayTimeCurrent = 0;

        /// <summary>
        /// Set Delay Time
        /// </summary>
        /// <param name="Value"></param>
        public void SetBrainDelayTime(int Value)
        {
            m_BrainDelayTime = Value;
        }

        /// <summary>
        /// Check Delay Time
        /// </summary>
        /// <returns>Will return "True" if Delay Time = 0</returns>
        public bool GetBrainDelayTimeValue()
        {
            if (m_BrainDelayTimeCurrent > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check Delay Time & Continue work on Delay Time
        /// </summary>
        /// <returns>Will return "True" if Delay Time = 0</returns>
        public bool GetBrainDelayTimeOver()
        {
            if (m_BrainDelayTimeCurrent > 0)
            {
                m_BrainDelayTimeCurrent -= 1;
                return false;
            }
            m_BrainDelayTimeCurrent = m_BrainDelayTime;
            return true;
        }

        #endregion
    }

    //==================================== Email

    public class QEmail
    {
        public static bool GetEmail(string EmailCheck)
        {
            //Check Not Invalid
            if (!GetEmailNotInvalid(EmailCheck))
            {
                return false;
            }

            //Lower AIL
            EmailCheck = EmailCheck.ToLower();

            return
                GetEmailGmail(EmailCheck) &&
                GetEmailYahoo(EmailCheck);
        }

        private static bool GetEmailNotInvalid(string EmailCheck)
        {
            //Check SPACE
            if (EmailCheck.Contains(" "))
            {
                return false;
            }

            //Check @
            bool CheckAAExist = false;
            for (int i = 0; i < EmailCheck.Length; i++)
            {
                if (!CheckAAExist && EmailCheck[i] == '@')
                {
                    CheckAAExist = true;
                }
                else
                if (CheckAAExist && EmailCheck[i] == '@')
                {
                    return false;
                }
            }
            if (!CheckAAExist)
            {
                return false;
            }

            //All Check Done
            return true;
        }

        private static bool GetEmailGmail(string EmailCheck)
        {
            //Check if GMAIL
            if (EmailCheck.Contains("@gmail.com"))
            {
                //Get ASCII
                byte[] ba_Ascii = Encoding.ASCII.GetBytes(EmailCheck);

                //First Character (Just '0-9' and 'a-z')
                if (ba_Ascii[0] >= 48 && ba_Ascii[0] <= 57 ||
                    ba_Ascii[0] >= 97 && ba_Ascii[0] <= 122)
                {
                    //Next Character (Just '0-9' and 'a-z' and '.')
                    for (int i = 1; i < EmailCheck.Length; i++)
                    {
                        if (EmailCheck[i] == '@')
                        {
                            break;
                        }

                        if (ba_Ascii[i] >= 48 && ba_Ascii[i] <= 57 ||
                            ba_Ascii[i] >= 97 && ba_Ascii[i] <= 122 ||
                            EmailCheck[i] == '.')
                        {

                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            //All Check Done
            return true;
        }

        private static bool GetEmailYahoo(string EmailCheck)
        {
            //Check if GMAIL
            if (EmailCheck.Contains("@yahoo.com"))
            {
                //Get ASCII
                byte[] ba_Ascii = Encoding.ASCII.GetBytes(EmailCheck);

                //First Character (Just 'a-z')
                if (ba_Ascii[0] >= 97 && ba_Ascii[0] <= 122)
                {
                    //Next Character (Just '0-9' and 'a-z' and '.' and '_')
                    for (int i = 1; i < EmailCheck.Length; i++)
                    {
                        if (EmailCheck[i] == '@')
                        {
                            break;
                        }

                        if (ba_Ascii[i] >= 48 && ba_Ascii[i] <= 57 ||
                            ba_Ascii[i] >= 97 && ba_Ascii[i] <= 122 ||
                            EmailCheck[i] == '.' ||
                            EmailCheck[i] == '_')
                        {

                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            //All Check Done
            return true;
        }
    }

    //==================================== Math

    public class QMath
    {
        public static (double Value, String String) GetDiscountPercent(double PrimaryValue, double DiscoundValue)
        {
            //Ex: Primary-Value = 40 & Discound-Value = 20 >> Mean Discound-Percent = 50%
            double DiscountPercent = (PrimaryValue - DiscoundValue) / PrimaryValue * 100;
            return (DiscountPercent, "-" + DiscountPercent.ToString() + "%");
        }
    }

    //==================================== Enum

    public enum Opption { Yes = 1, No = 0 }

    public enum Direction { Up, Down, Left, Right, None }

    public enum DirectionX { Left = -1, Right = 1, None = 0 }

    public enum DirectionY { Up = 1, Down = -1, None = 0 }
}

//namespace QuickManager
//{
//    public class QScene
//    {
//        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
//        public static void OnAfterSceneLoad()
//        {
//            Debug.Log("[Debug] On After Scene Load!");
//        }
//    }
//}