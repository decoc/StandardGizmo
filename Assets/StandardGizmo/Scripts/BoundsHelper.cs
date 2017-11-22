using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoundsHelper : MonoBehaviour {

    /// <summary>
    /// オブジェクトのBoundsを取得する
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <returns></returns>
   public static Bounds GetBounds(Transform targetTransform) {
        var boundsList = GetChildBounds(targetTransform);
        var renderer = targetTransform.gameObject.GetComponent<Renderer>();
        if (renderer != null) { boundsList.Add(renderer.bounds); }

        var bounds = boundsList.First();

        for (int i = 0; i < boundsList.Count; i++) {
            bounds.Encapsulate(boundsList[i]);
        }

        return bounds;
    }

    /// <summary>
    /// 子オブジェクトのBoundsを取得する
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <returns></returns>
    static List<Bounds> GetChildBounds(Transform targetTransform) {
        var boundsList = new List<Bounds>();

        foreach (Transform childTransform in targetTransform) {
            var renderer = childTransform.gameObject.GetComponent<Renderer>();
            if (renderer != null) { boundsList.Add(renderer.bounds); }
            boundsList.AddRange(GetChildBounds(childTransform));
        }

        return boundsList;
    }

}
