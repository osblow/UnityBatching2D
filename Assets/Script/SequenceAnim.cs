using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SequenceAnim : MonoBehaviour 
{
    public bool UseBatching = true; // use batching or not

    public float AnimSpeed = 1.0f; // sequence animation speed
    public Material Mat;
    public int MaxAmount = 100;
    public Vector2 IndividualScale = new Vector2(2.0f, 2.0f); // sub-mesh size
    public int TextureWidth = 4;
    public int TextureHeight = 2;
    public Vector3 m_range = new Vector3(30.0f, 30.0f, 30.0f); // random position range

    public GameObject SpriteObj; // the quad
    private List<Mesh> m_spriteObjMeshes = new List<Mesh>();

    private Mesh m_mesh;
    private List<Vector2> m_meshUVs = new List<Vector2>();
    private List<int> m_phases = new List<int>();


	// Use this for initialization
	void Start () 
    {
        if (UseBatching)
        {
            m_mesh = gameObject.AddComponent<MeshFilter>().mesh;
            Renderer render = gameObject.AddComponent<MeshRenderer>();
            render.material = Mat;

            InitBatchingSprites(MaxAmount);
        }
        else
        {
            InitNormalSprites(MaxAmount);
        }

	}

    private float m_timeElapse = 0;
	// Update is called once per frame
	void Update () 
    {
        if (m_timeElapse > 0.1f)
        {
            if (UseBatching)
            {
                UpdateBatchingAnim();
            }
            else
            {
                UpdateNormalSprites();
            }
            m_timeElapse = 0;
        }

        m_timeElapse += Time.deltaTime * AnimSpeed;
	}

    void InitBatchingSprites(int amount)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int[] trianglesTemplate = new int[] { 0, 1, 2, 3, 0, 2 };

        for (int i = 0; i < amount; i++)
        {
            // vertices
            Vector3 pos0 = new Vector3();
            pos0.x = Random.Range(0, m_range.x);
            pos0.y = Random.Range(0, m_range.y);
            pos0.z = Random.Range(0, m_range.z);

            Vector3 pos1 = pos0 + new Vector3(0, 0, IndividualScale.y);
            Vector3 pos2 = pos0 + new Vector3(IndividualScale.x, 0, IndividualScale.y);
            Vector3 pos3 = pos0 + new Vector3(IndividualScale.x, 0, 0);

            vertices.Add(pos0);
            vertices.Add(pos1);
            vertices.Add(pos2);
            vertices.Add(pos3);

            // triangles
            for (int j = 0; j < 6; j++)
            {
                triangles.Add(trianglesTemplate[j] + i * 4);
            }

            // uvs
            m_meshUVs.Add(new Vector2(0, 1.0f - 1.0f / TextureHeight));                   // bottom left
            m_meshUVs.Add(new Vector2(0, 1.0f));                                      // top left
            m_meshUVs.Add(new Vector2(1.0f / TextureWidth, 1.0f));                    // top right
            m_meshUVs.Add(new Vector2(1.0f / TextureWidth, 1.0f - 1.0f / TextureHeight)); // bottom right

            // every four vertices make up an individual element
            // they have a phase for their animation
            m_phases.Add(Random.Range(0, TextureWidth * TextureHeight - 1));
        }

        // apply to mesh
        m_mesh.SetVertices(vertices);
        m_mesh.SetTriangles(triangles, 0);
        m_mesh.SetUVs(0, m_meshUVs);
    }

    void UpdateBatchingAnim()
    {
        for (int i = 0; i < m_phases.Count; i++)
        {
            int phase = m_phases[i]++;

            float uOffset = phase % TextureWidth;
            float vOffset = (phase / TextureWidth) % TextureHeight;

            uOffset *= 1.0f / TextureWidth;
            vOffset *= 1.0f / TextureHeight;

            m_meshUVs[i * 4] = new Vector2(uOffset, 1.0f - vOffset - 1.0f / TextureHeight);
            m_meshUVs[i * 4 + 1] = new Vector2(uOffset, 1.0f - vOffset);
            m_meshUVs[i * 4 + 2] = new Vector2(uOffset + 1.0f / TextureWidth, 1.0f - vOffset);
            m_meshUVs[i * 4 + 3] = new Vector2(uOffset + 1.0f / TextureWidth, 1.0f - vOffset - 1.0f / TextureHeight);
        }

        m_mesh.SetUVs(0, m_meshUVs);
    }

    void InitNormalSprites(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 pos = new Vector3();
            pos.x = Random.Range(0, m_range.x);
            pos.y = Random.Range(0, m_range.y);
            pos.z = Random.Range(0, m_range.z);

            GameObject g = Instantiate(SpriteObj);
            g.transform.position = pos;
            g.transform.parent = transform;

            m_spriteObjMeshes.Add(g.GetComponent<MeshFilter>().mesh);

            m_phases.Add(Random.Range(0, TextureWidth * TextureHeight - 1));
        }
    }

    void UpdateNormalSprites()
    {
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < m_phases.Count; i++)
        {
            uvs = new List<Vector2>(m_spriteObjMeshes[i].uv);

            int phase = m_phases[i]++;

            float uOffset = phase % TextureWidth;
            float vOffset = (phase / TextureWidth) % TextureHeight;

            uOffset *= 1.0f / TextureWidth;
            vOffset *= 1.0f / TextureHeight;

            uvs[0] = new Vector2(uOffset, 1.0f - vOffset - 1.0f / TextureHeight);
            uvs[2] = new Vector2(uOffset, 1.0f - vOffset);
            uvs[1] = new Vector2(uOffset + 1.0f / TextureWidth, 1.0f - vOffset);
            uvs[3] = new Vector2(uOffset + 1.0f / TextureWidth, 1.0f - vOffset - 1.0f / TextureHeight);

            m_spriteObjMeshes[i].SetUVs(0, uvs);
        }

        
    }
}
