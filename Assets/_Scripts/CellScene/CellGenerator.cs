using UnityEngine;
using System.Collections.Generic;

public class CellGenerator : MonoBehaviour {

    public float cellRadius = 25.0f;

    public bool fadeWhenClose = true;
    private float camTransparentDist = 50.0f;

    private Material mat;

    struct TriangleIndices {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int v1, int v2, int v3) {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    static readonly float t = (1f + Mathf.Sqrt(5f)) / 2f;
    readonly Vector3[] goldenVectors = new Vector3[] {
        new Vector3(-1, +t, 0).normalized,
        new Vector3(+1, +t, 0).normalized,
        new Vector3(-1, - t, 0).normalized,
        new Vector3(+1, -t, 0).normalized,

        new Vector3(0, -1, +t).normalized,
        new Vector3(0, +1, +t).normalized,
        new Vector3(0, -1, -t).normalized,
        new Vector3(0, +1, -t).normalized,

        new Vector3(+t, 0, -1).normalized,
        new Vector3(+t, 0, +1).normalized,
        new Vector3(-t, 0, -1).normalized,
        new Vector3(-t, 0, +1).normalized
    };

    int index = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    Dictionary<long, int> table = new Dictionary<long, int>();

    // generates or returns the vertices and indices of a unit icosphere
    public void buildIcosphere(int rec) {
        //float time = Time.realtimeSinceStartup;
        if (rec > 9) {
            Debug.Log("Setting recursion level to maximum of 9");
            rec = 9;
        }

        // initialize variables
        index = 0;
        table.Clear();
        vertices.Clear();
        triangles.Clear();
        List<TriangleIndices> faces = new List<TriangleIndices>();

        // 12 starting points
        for (int i = 0; i < 12; i++) {
            vertices.Add(goldenVectors[i]);
            index++;
        }
        //vertices.AddRange(goldenVectors);
        //index = 12;

        // 20 faces
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));

        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));

        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));

        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));

        for (int i = 0; i < rec; i++) {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();

            // speeds generation up a bit
            // main problem is the fact that the table is getting loaded up too much
            table.Clear();
            foreach (TriangleIndices tri in faces) {
                int a = getMidpoint(tri.v1, tri.v2);
                int b = getMidpoint(tri.v2, tri.v3);
                int c = getMidpoint(tri.v3, tri.v1);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(a, tri.v2, b));
                faces2.Add(new TriangleIndices(c, b, tri.v3));
                // make new triangle upside down and backwards to simplify neighbor calculations
                // this way the triangle is either flipped or not flipped versus having 3 different rotations
                faces2.Add(new TriangleIndices(b, c, a));
            }
            faces = faces2;
        }

        foreach (TriangleIndices tri in faces) {
            triangles.Add(tri.v1);
            triangles.Add(tri.v2);
            triangles.Add(tri.v3);
        }

        //Debug.Log("Icosphere of rec " + rec + " built in " + (Time.realtimeSinceStartup - time) + " seconds, using " + icosphereVerts.Count + " vertices and " + triangles.Count + " indices.");

        //return new VertexData(vertices, triangles);
    }

    private int getMidpoint(int p1, int p2) {
        //generate key from pair of indices
        bool firstIsSmaller = p1 < p2;

        // hash 2 ints into long
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (table.TryGetValue(key, out ret)) {
            // once we find vertex take it out of dictionary to reduce hash collisions
            table.Remove(key);
            return ret;
        }

        vertices.Add(((vertices[p1] + vertices[p2]) * 0.5f).normalized);
        table[key] = index;
        return index++;
    }


    // Use this for initialization
    void Start() {

        buildIcosphere(5);

        float seed = Random.Range(-1000.0f, 1000.0f);

        for (int i = 0; i < vertices.Count; i++) {
            Vector3 v = vertices[i] * cellRadius;
            float n = Noise.fBM(Noise.Simplex3D, v, seed, 0.5f / cellRadius, 3);
            vertices[i] = v + v * n * 0.1f;
        }

        Mesh m = new Mesh();
        m.vertices = vertices.ToArray();
        m.triangles = triangles.ToArray();
        m.RecalculateNormals();
        m.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = m;

        mat = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update() {
        if (!fadeWhenClose) {
            return;
        }
        float d = Vector3.Magnitude(Camera.main.transform.position - transform.position);
        float min = cellRadius * 2.0f;
        float max = min + 20.0f;
        float t = (d - min) / (min - max);
        float a = Mathf.Lerp(0.1f, 0.5f, 1.0f-t);
        Color c = mat.GetColor("_Color");
        c.a = a;
        mat.SetColor("_Color", c);
    }
}
