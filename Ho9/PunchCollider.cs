﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollider : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(col.offset, col.size);
            //Gizmos.DrawIcon(col.bounds.center, "AtmosTrigger");
            Gizmos.matrix = oldMatrix;
        }
    }
}
