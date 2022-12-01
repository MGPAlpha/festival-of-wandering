using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AimAssistSystem
{
    private const float TARGET_AIM_RANGE = .1f;

    private class AimAssistTarget {
        
        public BoxCollider2D collider;
        public Transform transform;
        public float t;
        
        public AimAssistTarget(BoxCollider2D target) : this(target, target.transform, 0) {

        }

        private AimAssistTarget(BoxCollider2D target, Transform transform, float t) {
            this.collider = target;
            this.transform = transform;
            this.t = t;
        }

        public float RecalculateT(Vector3 aimOrigin) {
            this.t = CalculateTValue(aimOrigin, this.transform.position);
            return this.t;
        }

        public AimAssistTarget ModSpaceDuplicate(int multiple) {
            AimAssistTarget dupe = new AimAssistTarget(this.collider, this.transform, this.t + multiple);
            return dupe;
        }
    }

    private static float CalculateTValue(Vector3 aimOrigin, Vector3 targetPos) {
        Vector2 disp = targetPos - aimOrigin;
        return DirectionToT(disp);
    }

    private static float DirectionToT(Vector2 direction) {
        return Mathf.Atan2(direction.y, direction.x) / Mathf.PI / 2 + .5f;
    }

    private static List<AimAssistTarget> targets = new List<AimAssistTarget>();

    public static void RegisterTarget(BoxCollider2D target) {
        targets.Add(new AimAssistTarget(target));
    }

    public static void DeleteTarget(BoxCollider2D target) {
        foreach (AimAssistTarget deletionCandidate in targets) {
            if (deletionCandidate.collider == target) {
                targets.Remove(deletionCandidate);
                return;
            }
        }
    }

    private static void UpdateTValuesAndSort(Vector3 aimOrigin) {
        for (int i = 0; i < targets.Count; i++) {
            AimAssistTarget currTarget = targets[i];
            float currT = currTarget.RecalculateT(aimOrigin);
            for (int j = i - 1; j >= 0; j--) {
                if (targets[j].t <= currT) {
                    break;
                }
                targets[j+1] = targets[j];
                targets[j] = currTarget;
            }
        }
    }

    private static int BinarySearchTargets(float t) {
        int lowIndex = 0;
        int highIndex = targets.Count - 1;
        if (t < targets[lowIndex].t) return -1;
        if (t > targets[highIndex].t) return highIndex;
        while (lowIndex != highIndex) {
            int mid = (lowIndex + highIndex) / 2;
            if (t >= targets[mid].t && t <= targets[mid+1].t) {
                return mid;
            }
            if (t > targets[mid+1].t) {
                lowIndex = mid+1;
            } else {
                highIndex = mid;
            }
        }
        return -1;
    }

    private static AimAssistTarget ModSpaceTarget(int index) {
        AimAssistTarget output = targets[MathfExtension.Mod(index, targets.Count)];
        int modOffset = (int)Mathf.Floor((float)index / targets.Count);
        
        if (modOffset != 0) {
            output = output.ModSpaceDuplicate(modOffset);
        }
        return output;
    }

    private static float ProjectedPointComparator(float aVal, bool aInFront, float bVal, bool bInFront) {
        if (aInFront && bInFront) {
            return aVal - bVal;
        }
        if (!aInFront && !bInFront) {
            if (Mathf.Sign(aVal) == Mathf.Sign(bVal)) {
                return bVal - aVal;
            } else {
                return aVal - bVal;
            }
        }
        if (!aInFront) {
            return Mathf.Sign(aVal);
        } else{
            return Mathf.Sign(bVal);
        }
    }

    private static Vector2 GenerateAimBoxBounds(AimAssistTarget target, Vector3 aimOrigin) {
        Vector3 targetCenter = target.collider.bounds.center;
        Vector2 targetDir = (targetCenter - aimOrigin).normalized;
        Vector2 projector = new Vector2(targetDir.y, -targetDir.x);
        List<Vector2> corners = new List<Vector2>{
            targetCenter + target.collider.bounds.extents,
            targetCenter - target.collider.bounds.extents,
            targetCenter + target.collider.bounds.extents.FlipX(),
            targetCenter + target.collider.bounds.extents.FlipY()
        };

        List<float> projectedCorners = new List<float>();
        List<bool> projectionInFront = new List<bool>();
        foreach (Vector2 corner in corners) {
            Vector2 originToCorner = corner - (Vector2)aimOrigin;
            float forwardDot = Vector2.Dot(originToCorner, targetDir);
            Vector2 projectedPoint = originToCorner / Vector2.Dot(originToCorner, targetDir);
            projectedCorners.Add(Vector2.Dot(projectedPoint, projector));
            projectionInFront.Add(forwardDot >= 0);
        }


        int minIndex = 0;
        int maxIndex = 0;

        for (int i = 1; i < projectedCorners.Count; i++) {
            if (ProjectedPointComparator(projectedCorners[i], projectionInFront[i], projectedCorners[minIndex], projectionInFront[minIndex]) < 0) {
                minIndex = i;
            }
            if (ProjectedPointComparator(projectedCorners[i], projectionInFront[i], projectedCorners[maxIndex], projectionInFront[maxIndex]) > 0) {
                maxIndex = i;
            }           
        }

        // Debug.Log("MinIndex " + minIndex + " MaxIndex " + maxIndex);

        Vector2 corner1 = corners[minIndex] - (Vector2)aimOrigin;
        Vector2 corner2 = corners[maxIndex] - (Vector2)aimOrigin;
        // Debug.Log("Corner 1 " + corner1 + " Corner 2 " + corner2);
        float viewAngle = Vector2.Angle(corner1, corner2) / 360f;

        Vector2 bounds = new Vector2(TARGET_AIM_RANGE, viewAngle/2);

        return bounds;

    }

    public static Vector2 TransformAim(Vector3 aimOrigin, Vector2 aimInput) {
        
        if (targets.Count <= 0) return aimInput;

        UpdateTValuesAndSort(aimOrigin);

        float aimT = DirectionToT(aimInput);

        int targetIntervalIndex = BinarySearchTargets(aimT);

        List<AimAssistTarget> relevantTargets = new List<AimAssistTarget>{
            ModSpaceTarget(targetIntervalIndex-1),
            ModSpaceTarget(targetIntervalIndex),
            ModSpaceTarget(targetIntervalIndex+1),
            ModSpaceTarget(targetIntervalIndex+2)
        };

        List<Vector2> targetAimBoxBounds = new List<Vector2> {
            GenerateAimBoxBounds(relevantTargets[0], aimOrigin),
            GenerateAimBoxBounds(relevantTargets[1], aimOrigin),
            GenerateAimBoxBounds(relevantTargets[2], aimOrigin),
            GenerateAimBoxBounds(relevantTargets[3], aimOrigin)
        };

        // Debug.Log("Aim box is " + targetAimBoxBounds[0]);

        List<Vector2> splinePoints = new List<Vector2>();
        splinePoints.Add(new Vector2(relevantTargets[0].t, relevantTargets[0].t));
        int closestSplinePoint = 1;

        for (int i = 0; i < relevantTargets.Count - 1; i++) {
            AimAssistTarget target1 = relevantTargets[i];
            AimAssistTarget target2 = relevantTargets[i+1];
            Vector2 bounds1 = targetAimBoxBounds[i];
            Vector2 bounds2 = targetAimBoxBounds[i+1];

            Vector2 t1Point = new Vector2(target1.t, target1.t);
            Vector2 t2Point = new Vector2(target2.t, target2.t);

            float tDiff = target2.t - target1.t;

            if (tDiff > 0) {

                float idealTargetSeparation = (bounds1.x + bounds2.x) * 3 / 2;

                float boundsSquishFactor = Mathf.Min(1, tDiff / idealTargetSeparation);

                splinePoints.Add(t1Point + bounds1 * boundsSquishFactor);
                if (aimT > splinePoints[splinePoints.Count-1].x) closestSplinePoint = splinePoints.Count-1;
                splinePoints.Add(t2Point - bounds2 * boundsSquishFactor);
                if (aimT > splinePoints[splinePoints.Count-1].x) closestSplinePoint = splinePoints.Count-1;

            }

            splinePoints.Add(t2Point);
            if (aimT > splinePoints[splinePoints.Count-1].x) closestSplinePoint = splinePoints.Count-1;
        }
        
        // string splinePointsPrint = "";
        // for (int i = 0; i < splinePoints.Count; i++) {
        //     splinePointsPrint += splinePoints[i].ToString() + " ";
        // }
        //Debug.Log(splinePointsPrint);

        //Debug.Log("T value " + aimT);


        Vector2 splineControl1 = splinePoints[closestSplinePoint - 1];
        Vector2 splineControl2 = splinePoints[closestSplinePoint];
        Vector2 splineControl3 = splinePoints[closestSplinePoint + 1];
        Vector2 splineControl4 = splinePoints[closestSplinePoint + 2];
        //Debug.Log("Chosen spline controls" + splineControl1 + " " + splineControl2 + " " + splineControl3 + " " + splineControl4);
        Vector2 splineSecant12 = splineControl2 - splineControl1;
        Vector2 splineSecant23 = splineControl3 - splineControl2;
        Vector2 splineSecant34 = splineControl4 - splineControl3;
        float splineVel12 = splineSecant12.x != 0 ? splineSecant12.y/splineSecant12.x : 0;
        float splineVel23 = splineSecant23.x != 0 ? splineSecant23.y/splineSecant23.x : 0;
        float splineVel34 = splineSecant34.x != 0 ? splineSecant34.y/splineSecant34.x : 0;
        float splineR2 = (splineVel12 + splineVel23)/2;
        float splineR3 = (splineVel23 + splineVel34)/2;

        float alpha = splineR2 / splineVel23;
        float beta = splineR3 / splineVel23;

        float alphaBetaSq = alpha*alpha + beta*beta;
        if (alphaBetaSq > 9) {
            float tau = 3 / Mathf.Sqrt(alphaBetaSq);

            splineR2 = tau * alpha * splineVel23;
            splineR3 = tau * beta * splineVel23;
        }

        //Debug.Log("R2 " + splineR2 + " R3 " + splineR3);

        float splineT = Mathf.InverseLerp(splineControl2.x, splineControl3.x, aimT);

        float t2 = splineT*splineT;
        float t3 = splineT*t2;

        // Matrix4x4 tMatrix = new Matrix4x4(new Vector4(t3, t2, aimT, 1), Vector4.zero, Vector4.zero, Vector4.zero).transpose;
        Vector4 tVector = new Vector4(t3, t2, splineT, 1);
        Vector4 gVector = new Vector4(splineControl2.y, splineControl3.y, splineR2, splineR3);

        float outT = Vector4.Dot(tVector, HERMITE_M * gVector);

        //Debug.Log("Output " + outT);

        float outAngle = outT * (Mathf.PI*2) - Mathf.PI;

        Vector2 outAim = new Vector2(Mathf.Cos(outAngle), Mathf.Sin(outAngle));

        return outAim;



    }

    private static Matrix4x4 HERMITE_M = new Matrix4x4(
        new Vector4(2,-2,1,1),
        new Vector4(-3,3,-2,-1),
        new Vector4(0,0,1,0),
        new Vector4(1,0,0,0)
    ).transpose; 

}
