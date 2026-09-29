using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;

public static class BucketPrefabCreator
{
    private const string PrefabPath = "Assets/Gameplay/WaterBucket.prefab";

    [MenuItem("Tools/Survival/Create or Update Water Bucket Prefab")]
    private static void CreateBucket()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Gameplay"))
            AssetDatabase.CreateFolder("Assets", "Gameplay");

        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        var bucket = existing != null
            ? PrefabUtility.LoadPrefabContents(PrefabPath)
            : new GameObject("WaterBucket");

        var rigidbody = bucket.GetComponent<Rigidbody>();
        if (rigidbody == null) rigidbody = bucket.AddComponent<Rigidbody>();
        rigidbody.mass = 1f;
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        rigidbody.linearDamping = 1f;
        rigidbody.angularDamping = 5f;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        var oldCollider = bucket.GetComponent<CapsuleCollider>();
        if (oldCollider != null) Object.DestroyImmediate(oldCollider);
        var collider = bucket.GetComponent<BoxCollider>();
        if (collider == null) collider = bucket.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.26f, 0.34f, 0.26f);
        collider.center = new Vector3(0f, 0.17f, 0f);
        if (bucket.GetComponent<XRGrabInteractable>() == null)
            bucket.AddComponent<XRGrabInteractable>();
        var grab = bucket.GetComponent<XRGrabInteractable>();
        grab.colliders.Clear();
        grab.colliders.Add(collider);
        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grab.throwOnDetach = false;
        if (bucket.GetComponent<WaterBucket>() == null)
            bucket.AddComponent<WaterBucket>();

        DestroyChild(bucket.transform, "BucketBody");
        DestroyChild(bucket.transform, "BucketHandle");

        var body = new GameObject("BucketBody");
        body.name = "BucketBody";
        body.transform.SetParent(bucket.transform, false);
        var meshPath = "Assets/Gameplay/WaterBucketMesh.asset";
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        if (mesh == null)
        {
            mesh = BuildOpenBucketMesh();
            AssetDatabase.CreateAsset(mesh, meshPath);
        }
        body.AddComponent<MeshFilter>().sharedMesh = mesh;
        body.AddComponent<MeshRenderer>().sharedMaterial =
            CreateMaterial("BucketGray", new Color(0.28f, 0.30f, 0.32f));

        var handle = new GameObject("BucketHandle");
        handle.transform.SetParent(bucket.transform, false);
        for (int i = 0; i < 12; i++)
        {
            float a = Mathf.PI * i / 12f;
            float b = Mathf.PI * (i + 1) / 12f;
            var start = new Vector3(0.18f * Mathf.Cos(a), 0.34f + 0.18f * Mathf.Sin(a), 0f);
            var end = new Vector3(0.18f * Mathf.Cos(b), 0.34f + 0.18f * Mathf.Sin(b), 0f);
            var bar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bar.name = "HandleSegment";
            bar.transform.SetParent(handle.transform, false);
            bar.transform.localPosition = (start + end) * 0.5f;
            bar.transform.localRotation = Quaternion.FromToRotation(Vector3.up, end - start);
            bar.transform.localScale = new Vector3(0.014f, (end - start).magnitude * 0.5f, 0.014f);
            Object.DestroyImmediate(bar.GetComponent<Collider>());
            bar.GetComponent<Renderer>().sharedMaterial = body.GetComponent<Renderer>().sharedMaterial;
        }

        var water = bucket.transform.Find("WaterInside");
        if (water == null)
        {
            water = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
            water.name = "WaterInside";
            water.SetParent(bucket.transform, false);
            Object.DestroyImmediate(water.GetComponent<Collider>());
        }
        water.localPosition = new Vector3(0f, 0.28f, 0f);
        water.localScale = new Vector3(0.155f, 0.004f, 0.155f);
        water.GetComponent<Renderer>().sharedMaterial = CreateMaterial("BucketWaterBlue", new Color(0.1f, 0.35f, 0.85f));

        var serialized = new SerializedObject(bucket.GetComponent<WaterBucket>());
        serialized.FindProperty("waterInside").objectReferenceValue = water.gameObject;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        water.gameObject.SetActive(false);

        var prefab = PrefabUtility.SaveAsPrefabAsset(bucket, PrefabPath);
        if (existing != null) PrefabUtility.UnloadPrefabContents(bucket);
        else Object.DestroyImmediate(bucket);
        Selection.activeObject = prefab;
        Debug.Log("Updated " + PrefabPath + " with an open bucket body and handle.");
    }

    private static void DestroyChild(Transform parent, string name)
    {
        var child = parent.Find(name);
        if (child != null) Object.DestroyImmediate(child.gameObject);
    }

    private static Mesh BuildOpenBucketMesh()
    {
        const int sides = 32;
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        for (int i = 0; i < sides; i++)
        {
            float a = Mathf.PI * 2f * i / sides;
            float b = Mathf.PI * 2f * (i + 1) / sides;
            Vector3 outerBottomA = new Vector3(Mathf.Cos(a) * 0.13f, 0f, Mathf.Sin(a) * 0.13f);
            Vector3 outerBottomB = new Vector3(Mathf.Cos(b) * 0.13f, 0f, Mathf.Sin(b) * 0.13f);
            Vector3 outerTopA = new Vector3(Mathf.Cos(a) * 0.18f, 0.34f, Mathf.Sin(a) * 0.18f);
            Vector3 outerTopB = new Vector3(Mathf.Cos(b) * 0.18f, 0.34f, Mathf.Sin(b) * 0.18f);
            Vector3 innerBottomA = new Vector3(Mathf.Cos(a) * 0.115f, 0.025f, Mathf.Sin(a) * 0.115f);
            Vector3 innerBottomB = new Vector3(Mathf.Cos(b) * 0.115f, 0.025f, Mathf.Sin(b) * 0.115f);
            Vector3 innerTopA = new Vector3(Mathf.Cos(a) * 0.16f, 0.34f, Mathf.Sin(a) * 0.16f);
            Vector3 innerTopB = new Vector3(Mathf.Cos(b) * 0.16f, 0.34f, Mathf.Sin(b) * 0.16f);

            AddQuad(vertices, triangles, outerBottomA, outerTopA, outerTopB, outerBottomB);
            AddQuad(vertices, triangles, innerBottomB, innerTopB, innerTopA, innerBottomA);
            AddQuad(vertices, triangles, outerTopA, innerTopA, innerTopB, outerTopB);
            AddTriangle(vertices, triangles, innerBottomA, Vector3.up * 0.025f, innerBottomB);
            AddTriangle(vertices, triangles, outerBottomA, outerBottomB, Vector3.zero);
        }

        var result = new Mesh { name = "WaterBucketOpenMesh" };
        result.SetVertices(vertices);
        result.SetTriangles(triangles, 0);
        result.RecalculateNormals();
        result.RecalculateBounds();
        return result;
    }

    private static void AddQuad(List<Vector3> vertices, List<int> triangles,
        Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        AddTriangle(vertices, triangles, a, b, c);
        AddTriangle(vertices, triangles, a, c, d);
    }

    private static void AddTriangle(List<Vector3> vertices, List<int> triangles,
        Vector3 a, Vector3 b, Vector3 c)
    {
        int start = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);
    }

    private static Material CreateMaterial(string name, Color color)
    {
        var path = "Assets/Gameplay/" + name + ".mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        var material = new Material(shader) { color = color };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }
}
