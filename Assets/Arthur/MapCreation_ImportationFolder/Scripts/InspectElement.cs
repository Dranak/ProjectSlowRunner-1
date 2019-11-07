﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectElement : MonoBehaviour
{
    public MapEditor_MainController elementTest;

    public enum Tyle_Type
    {
        Empty,
        Road,
        City,
        Grass,
        CrossRoads,
        Monument_Source,
    }

    public enum Tyle_Evenement
    {
        Empty,
        Monument,
        Trafic_Jam,
        Concert,
    }

    public Tyle_Type type;
    public Tyle_Evenement Event;

    public float distance_Check;

    public List<Transform> neighborHex;
    public bool visited;
}