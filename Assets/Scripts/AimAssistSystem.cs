using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AimAssistSystem
{


    private class AimAssistTarget {
        
        public Collider2D collider;
        public Transform transform;
        public float t;
        
        public AimAssistTarget(Collider2D target) {
            this.collider = target;
            this.transform = target.transform;
            this.t = 0;
        }

        public float RecalculateT(Vector3 aimOrigin) {
            this.t = CalculateTValue(aimOrigin, this.transform.position);
            return this.t;
        }
    }

    private static float CalculateTValue(Vector3 aimOrigin, Vector3 targetPos) {
        Vector2 disp = targetPos - aimOrigin;
        return Mathf.Atan2(disp.y, disp.x) / Mathf.PI / 2 + .5f;
    }

    private static List<AimAssistTarget> targets = new List<AimAssistTarget>();

    public static void RegisterTarget(Collider2D target) {
        targets.Add(new AimAssistTarget(target));
    }

    public static void DeleteTarget(Collider2D target) {
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

    public static Vector2 TransformAim(Vector3 aimOrigin, Vector2 aimInput) {
        if (targets.Count <= 0) return aimInput;

        UpdateTValuesAndSort(aimOrigin);

        return aimInput;

    }

}
