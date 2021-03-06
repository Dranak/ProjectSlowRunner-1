﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(InspectElement)),CanEditMultipleObjects]
public class InspectElement_Map : Editor
{
    public override void OnInspectorGUI()
    {
        //When you click on a tile, she's had to a list. If you choose to add a type, the type will be add to all tiles in this list
        InspectElement map = (InspectElement)target; 
        if (!map.elementTest.test.Contains(map))
            map.elementTest.test.Add(map);

        if (DrawDefaultInspector()) //-> if something have changed
        { }

        EditorGUILayout.Space();
        if (GUILayout.Button("City")) //-> so if we decide the tile is a City block, we change the material and his property
        {
            foreach (InspectElement element in map.elementTest.test){
                element.GetComponent<MeshRenderer>().material = map.elementTest.city;
                element.type = InspectElement.Tyle_Type.City;
            }
            map.elementTest.test.Clear(); //important to clear the list, unless you will change over and over tiles selected before
        }

        if (GUILayout.Button("Grass")) // -> same for grass block
        {
            foreach (InspectElement element in map.elementTest.test){
                element.GetComponent<MeshRenderer>().material = map.elementTest.grass;
                element.type = InspectElement.Tyle_Type.Grass;
            }
            map.elementTest.test.Clear();
        }

        if (GUILayout.Button("ActualizeBoxCollider"))
        {
            foreach (Transform element in map.elementTest.Roads_Position)
            {
                var go = new GameObject();
                go.name = "DetectionCollision_Swipe";
                BoxCollider bxCollider = go.AddComponent<BoxCollider>();
                bxCollider.center = element.transform.position;
                bxCollider.gameObject.layer = 9; //-> 9 is layerMask value for road
                bxCollider.size = new Vector3(map.elementTest.scaleX, 1, map.elementTest.scaleZ);
                go.transform.SetParent(element);
            }
        }

        if (GUILayout.Button("EMERGENCY"))
        {
            map.elementTest.Roads_Position.Clear();
            map.elementTest.test.Clear();
            map.elementTest.Monuments_Position.Clear();
        }

            //Monument and Road Block are specific case -> like they are event for the player, we need to keep track of them
            if (GUILayout.Button("Monument"))
        {
            //Check for neighbour in Genereate method
            foreach (InspectElement element in map.elementTest.test){
                element.GetComponent<MeshRenderer>().material = map.elementTest.monument_Mat;
                element.type = InspectElement.Tyle_Type.Monument_Source;
                if (!map.elementTest.Monuments_Position.Contains(element.transform))
                    map.elementTest.Monuments_Position.Add(element.transform);
            }
            map.elementTest.test.Clear();
        }

        if (GUILayout.Button("Malus"))
        {
            //Check for neighbour in Genereate method
            foreach (InspectElement element in map.elementTest.test)
            {
                element.GetComponent<MeshRenderer>().material = map.elementTest.Malus_Mat;
                element.type = InspectElement.Tyle_Type.Malus_Source;
                if (!map.elementTest.Malus_Position.Contains(element.transform))
                    map.elementTest.Malus_Position.Add(element.transform);
            }
            map.elementTest.test.Clear();
        }

        if (GUILayout.Button("Road"))
        {
            foreach (InspectElement element in map.elementTest.test){
                element.GetComponent<MeshRenderer>().material = map.elementTest.road;
                element.type = InspectElement.Tyle_Type.Road;
                if (!map.elementTest.Roads_Position.Contains(element.transform))
                    map.elementTest.Roads_Position.Add(element.transform);
            }
            map.elementTest.test.Clear();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Clear Selection"))
            map.elementTest.test.Clear();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Clear")) //-> this button clear all materials, prefabs, and event into the map
        {
            //Clear all tiles
            foreach (Transform temp in map.elementTest.isoSphere.transform){
                temp.GetComponent<MeshRenderer>().material = map.elementTest.default_Mat;
                var tmp = temp.GetComponent<InspectElement>();
                tmp.type = InspectElement.Tyle_Type.Empty;
                tmp.Event = InspectElement.Tyle_Evenement.Empty;
                tmp.neighborHex.Clear();

                if (temp.GetComponent<Walkable>() != null)
                    DestroyImmediate(temp.GetComponent<Walkable>());

                if (temp.transform.childCount > 0){
                    foreach (Transform child in temp.transform)
                        DestroyImmediate(child.gameObject);
                }
            }
            for (int i = 0; i < map.elementTest.Roads_Position.Count; i++)
            {
                map.elementTest.Roads_Position[i].GetComponent<InspectElement>().neighborHex.Clear();
                map.elementTest.Roads_Position[i].GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
            }
            for (int i = 0; i < map.elementTest.Monuments_Position.Count; i++)
                map.elementTest.Monuments_Position[i].GetComponent<InspectElement>().neighborHex.Clear();

            for (int i = 0; i < map.elementTest.Malus_Position.Count; i++)
                map.elementTest.Malus_Position[i].GetComponent<InspectElement>().neighborHex.Clear();
            map.elementTest.Roads_Position.Clear();
            map.elementTest.Monuments_Position.Clear();
            map.elementTest.Malus_Position.Clear();
            map.elementTest.test.Clear();
        }


        //GENERATION

        if (GUILayout.Button("Generate"))
        {
            if (EditorUtility.DisplayDialog("Confirm Generation ? ", " Make sure every road are interconnected\n\n And that you have place everything where you really want", "Place", "Do not Generate"))
            {
                //So we iterate over the list of the road position, if a walkable script is not found, we had it and we connect it with all of his neighbor
                for (int i = 0; i < map.elementTest.Roads_Position.Count; i++){
                    for (int j = 0; j < map.elementTest.Roads_Position.Count; j++){
                        if (map.elementTest.Roads_Position[i] != map.elementTest.Roads_Position[j]){
                            if (Vector3.Distance(map.elementTest.Roads_Position[i].transform.position, map.elementTest.Roads_Position[j].transform.position) < map.distance_Check){
                                map.elementTest.Roads_Position[i].GetComponent<InspectElement>().neighborHex.Add(map.elementTest.Roads_Position[j]);
                            }
                        }
                    }
                    if (map.elementTest.Roads_Position[i].GetComponent<InspectElement>().Event == InspectElement.Tyle_Evenement.Restaurant){
                        map.elementTest.Roads_Position[i].GetComponent<MeshRenderer>().sharedMaterial = map.elementTest.restaurant_Mat;
                    }

                    if (map.elementTest.Roads_Position[i].GetComponent<InspectElement>().Event == InspectElement.Tyle_Evenement.Chantier){
                        map.elementTest.Roads_Position[i].GetComponent<MeshRenderer>().sharedMaterial = map.elementTest.chantier_Mat;
                    }
                }
            }

            foreach (Transform temp in map.elementTest.Roads_Position)
            {
                //Crossroads
                if (temp.GetComponent<InspectElement>().type == InspectElement.Tyle_Type.Road || temp.GetComponent<InspectElement>().type == InspectElement.Tyle_Type.CrossRoads){
                    if (temp.GetComponent<Walkable>() == null){
                        var walk_Script = temp.gameObject.AddComponent<Walkable>();
                        foreach (Transform element in temp.gameObject.GetComponent<InspectElement>().neighborHex){
                            WalkPath walk = new WalkPath();
                            walk.target = element;
                            walk.active = true;
                            walk_Script.possiblePaths.Add(walk);
                        }
                    }
                }
            }

            //Monument
            /*if (map.elementTest.Monuments_Position.Count != 0){
                for (int j = 0; j < map.elementTest.isoSphere.childCount; j++){
                    for (int i = 0; i < map.elementTest.Monuments_Position.Count; i++){
                        if (Vector3.Distance(map.elementTest.isoSphere.GetChild(j).position, map.elementTest.Monuments_Position[i].position) < map.distance_Check && map.elementTest.isoSphere.GetChild(j) != map.elementTest.Monuments_Position[i]){
                            if(map.elementTest.isoSphere.GetChild(j).GetComponent<InspectElement>().Event == InspectElement.Tyle_Evenement.Empty)
                                map.elementTest.isoSphere.GetChild(j).GetComponent<InspectElement>().Event = InspectElement.Tyle_Evenement.Monument;
                        }
                    }
                }
            }*/

            /*if (map.elementTest.Malus_Position.Count != 0)
            {
                for (int j = 0; j < map.elementTest.isoSphere.childCount; j++)
                {
                    for (int i = 0; i < map.elementTest.Malus_Position.Count; i++)
                    {
                        if (Vector3.Distance(map.elementTest.isoSphere.GetChild(j).position, map.elementTest.Malus_Position[i].position) < map.distance_Check && map.elementTest.isoSphere.GetChild(j) != map.elementTest.Malus_Position[i])
                        {
                            if (map.elementTest.isoSphere.GetChild(j).GetComponent<InspectElement>().Event == InspectElement.Tyle_Evenement.Empty)
                                map.elementTest.isoSphere.GetChild(j).GetComponent<InspectElement>().Event = InspectElement.Tyle_Evenement.Monument;
                        }
                    }
                }
            }*/

            //Prefabs instantiation
            /*for (int i = 0; i < map.elementTest.isoSphere.transform.childCount; i++){
                //We spawn prefabs depending of the type of the event we found on the planet
                //We calculate the normal to spawn the object on the ground of the tile
                if (map.elementTest.isoSphere.transform.GetChild(i).GetComponent<InspectElement>().type == InspectElement.Tyle_Type.Monument_Source){
                    Vector3 normal = map.elementTest.isoSphere.transform.position - map.elementTest.isoSphere.transform.GetChild(i).transform.position; //Normal calculation of the actual tile

                    GameObject gameObject_ = Instantiate(map.elementTest.monumentPrefab[Random.Range(0, map.elementTest.monumentPrefab.Length)], map.elementTest.isoSphere.transform.GetChild(i));
                    gameObject_.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);
                    gameObject_.transform.localPosition = new Vector3(0, 0, 0);
                    gameObject_.transform.up = -normal;
                }
                else if (map.elementTest.isoSphere.transform.GetChild(i).GetComponent<InspectElement>().type == InspectElement.Tyle_Type.City){
                    Vector3 normal = map.elementTest.isoSphere.transform.position - map.elementTest.isoSphere.transform.GetChild(i).transform.position;

                    GameObject gameObject_ = Instantiate(map.elementTest.cityPrefab[Random.Range(0, map.elementTest.cityPrefab.Length)], map.elementTest.isoSphere.transform.GetChild(i));
                    gameObject_.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);
                    gameObject_.transform.localPosition = new Vector3(0, 0, 0);
                    gameObject_.transform.up = -normal;
                }
                else if (map.elementTest.isoSphere.transform.GetChild(i).GetComponent<InspectElement>().type == InspectElement.Tyle_Type.Grass){
                    Vector3 normal = map.elementTest.isoSphere.transform.position - map.elementTest.isoSphere.transform.GetChild(i).transform.position;

                    GameObject gameObject_ = Instantiate(map.elementTest.grassPrefab[Random.Range(0, map.elementTest.grassPrefab.Length)], map.elementTest.isoSphere.transform.GetChild(i));
                    gameObject_.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);
                    gameObject_.transform.localPosition = new Vector3(0, 0, 0);
                    gameObject_.transform.up = -normal;
                }
                
            }*/
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        //Rare utilisation Button -> if we import a map previously contruct, she will not have all of her events, we can use the Update button to replace everything
        //
        //Issue where encounters with materials -> use of the shared material, instead it create instance of material and it can break the game later on
        //
        if (GUILayout.Button("Update"))
        {
            if (EditorUtility.DisplayDialog("Confirm Update ? ", "You want to use update button only in the case where you have imported a new planet", "Update", "Do not Update")){
                for (int i = 0; i < map.elementTest.isoSphere.transform.childCount; i++){
                    if (map.elementTest.isoSphere.transform.GetChild(i).GetComponent<MeshRenderer>().sharedMaterial == map.elementTest.road || map.elementTest.isoSphere.transform.GetChild(i).GetComponent<MeshRenderer>().sharedMaterial == map.elementTest.crossroadsMat){
                        InspectElement tile = map.elementTest.isoSphere.transform.GetChild(i).GetComponent<InspectElement>();
                        tile.type = InspectElement.Tyle_Type.Road;
                        if (!map.elementTest.Roads_Position.Contains(tile.transform))
                            map.elementTest.Roads_Position.Add(tile.transform);
                    }
                    else if (map.elementTest.isoSphere.transform.GetChild(i).GetComponent<MeshRenderer>().sharedMaterial == map.elementTest.grass){
                        InspectElement tile = map.elementTest.isoSphere.transform.GetChild(i).GetComponent<InspectElement>();
                        tile.type = InspectElement.Tyle_Type.Grass;
                    }
                    else if (map.elementTest.isoSphere.transform.GetChild(i).GetComponent<MeshRenderer>().sharedMaterial == map.elementTest.monument_Mat){
                        InspectElement tile = map.elementTest.isoSphere.transform.GetChild(i).GetComponent<InspectElement>();
                        tile.type = InspectElement.Tyle_Type.Monument_Source;
                        if (!map.elementTest.Monuments_Position.Contains(tile.transform))
                            map.elementTest.Monuments_Position.Add(tile.transform);
                    }
                    else if (map.elementTest.isoSphere.transform.GetChild(i).GetComponent<MeshRenderer>().sharedMaterial == map.elementTest.city){
                        InspectElement tile = map.elementTest.isoSphere.transform.GetChild(i).GetComponent<InspectElement>();
                        tile.type = InspectElement.Tyle_Type.City;
                    }
                }

                //Monument
                if (map.elementTest.Monuments_Position.Count != 0){
                    for (int j = 0; j < map.elementTest.isoSphere.childCount; j++){
                        for (int i = 0; i < map.elementTest.Monuments_Position.Count; i++){
                            if (Vector3.Distance(map.elementTest.isoSphere.GetChild(j).position, map.elementTest.Monuments_Position[i].position) < map.distance_Check && map.elementTest.isoSphere.GetChild(j) != map.elementTest.Monuments_Position[i])
                                map.elementTest.isoSphere.GetChild(j).GetComponent<InspectElement>().Event = InspectElement.Tyle_Evenement.Monument;
                        }
                    }
                }

                for (int i = 0; i < map.elementTest.Roads_Position.Count; i++)
                {
                    for (int j = 0; j < map.elementTest.Roads_Position.Count; j++)
                    {
                        //TODO : Check if needed in Upgrade Case
                        /*if (map.elementTest.Roads_Position[i] != map.elementTest.Roads_Position[j]){
                            /*if (Vector3.Distance(map.elementTest.Roads_Position[i].transform.position, map.elementTest.Roads_Position[j].transform.position) < map.distance_Check)
                                map.elementTest.Roads_Position[i].GetComponent<InspectElement>().neighborHex.Add(map.elementTest.Roads_Position[j]);
                        }*/
                    }
                    /*if (map.elementTest.Roads_Position[i].GetComponent<InspectElement>().Event == InspectElement.Tyle_Evenement.Trafic_Jam)
                        map.elementTest.Roads_Position[i].GetComponent<MeshRenderer>().material = map.elementTest.traficJam_Mat;*/
                    if (map.elementTest.Roads_Position[i].GetComponent<InspectElement>().Event == InspectElement.Tyle_Evenement.Restaurant){
                        map.elementTest.Roads_Position[i].GetComponent<MeshRenderer>().material = map.elementTest.restaurant_Mat;
                    }
                    else if (map.elementTest.Roads_Position[i].GetComponent<InspectElement>().Event == InspectElement.Tyle_Evenement.Chantier){
                        map.elementTest.Roads_Position[i].GetComponent<MeshRenderer>().material = map.elementTest.chantier_Mat;
                    }
                    else if (map.elementTest.Roads_Position[i].GetComponent<InspectElement>().Event == InspectElement.Tyle_Evenement.Monument){
                        map.elementTest.Roads_Position[i].GetComponent<MeshRenderer>().material = map.elementTest.monument_Mat;
                    }
                    else if (map.elementTest.Roads_Position[i].GetComponent<InspectElement>().Event == InspectElement.Tyle_Evenement.Feux_Rouge){
                        map.elementTest.Roads_Position[i].GetComponent<MeshRenderer>().material = map.elementTest.feux_Rouge_Mat;
                    }
                }
            }
        }

        //Not used anymore
        /*if (GUILayout.Button("Clear unnecessary color")){
            if (EditorUtility.DisplayDialog("Confirm Update ? ", "This will clear all unnecessary color in the map", "Update", "Do not Update")){
                foreach (Transform element in map.elementTest.isoSphere.transform){
                    if (element.GetComponent<InspectElement>().type != InspectElement.Tyle_Type.CrossRoads && element.GetComponent<InspectElement>().type != InspectElement.Tyle_Type.Road)
                        element.GetComponent<MeshRenderer>().material = map.elementTest.default_Mat;
                }
            }
        }*/
    }
}
