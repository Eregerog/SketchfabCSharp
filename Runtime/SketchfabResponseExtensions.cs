using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SketchfabResponseExtensions {
    public static bool TryGetValue<T>(this SketchfabResponse<T> response, out T value) {
        if (response.Success) {
            value = response.Object;
            return true;
        }

        value = default;
        return false;
    }
}
