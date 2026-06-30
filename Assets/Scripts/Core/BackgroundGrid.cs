using UnityEngine;

namespace BulletHeaven.Core
{
    public class BackgroundGrid : MonoBehaviour
    {
        [SerializeField] Color bgColor   = new Color(0.07f, 0.08f, 0.11f);
        [SerializeField] Color lineColor = new Color(0.22f, 0.25f, 0.35f);
        [SerializeField] float gridSize  = 2f;
        [SerializeField] int   halfCells = 20; // covers 40x40 world units at gridSize=2

        Camera       _cam;
        MeshFilter   _mf;
        MeshRenderer _mr;

        void Awake()
        {
            _cam = Camera.main;
            if (_cam != null)
            {
                _cam.clearFlags      = CameraClearFlags.SolidColor;
                _cam.backgroundColor = bgColor;
            }

            _mf = gameObject.AddComponent<MeshFilter>();
            _mr = gameObject.AddComponent<MeshRenderer>();
            _mr.sortingOrder      = -32768;
            _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _mr.receiveShadows    = false;

            var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.SetColor("_BaseColor", lineColor);
            mat.hideFlags = HideFlags.HideAndDontSave;
            _mr.sharedMaterial = mat;

            BuildMesh();
        }

        void LateUpdate()
        {
            if (_cam == null) return;
            Vector2 c = _cam.transform.position;
            transform.position = new Vector3(
                Mathf.Floor(c.x / gridSize) * gridSize,
                Mathf.Floor(c.y / gridSize) * gridSize,
                0f);
        }

        void BuildMesh()
        {
            float ext = halfCells * gridSize;
            var verts   = new System.Collections.Generic.List<Vector3>();
            var indices = new System.Collections.Generic.List<int>();
            int idx = 0;

            for (int col = -halfCells; col <= halfCells; col++)
            {
                float x = col * gridSize;
                verts.Add(new Vector3(x, -ext, 0));
                verts.Add(new Vector3(x,  ext, 0));
                indices.Add(idx++); indices.Add(idx++);
            }
            for (int row = -halfCells; row <= halfCells; row++)
            {
                float y = row * gridSize;
                verts.Add(new Vector3(-ext, y, 0));
                verts.Add(new Vector3( ext, y, 0));
                indices.Add(idx++); indices.Add(idx++);
            }

            var mesh = new Mesh { name = "GridMesh" };
            mesh.SetVertices(verts);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            mesh.RecalculateBounds();
            _mf.mesh = mesh;
        }
    }
}
