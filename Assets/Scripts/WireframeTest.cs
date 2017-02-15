using UnityEngine;
using UnityEngine.UI;

enum Mode
{
    Face = 0,
    WireFrame,
    WireFrameNoDuplicates,
    WireFrameOnlyEdges
}

public class WireframeTest : MonoBehaviour
{
    private int[][] _trianglesList;

    private Mode _mode;
    private Mesh[] _meshList;

    public Text text;

	// Use this for initialization
	void Start () {
	    Transform children = gameObject.GetComponentInChildren<Transform> ();
	    int childCount = children.childCount;

	    if (childCount == 0) return;

	    _meshList = new Mesh[childCount];
	    _trianglesList = new int[childCount][];

	    int i = 0;
	    foreach (Transform obj in children)
	    {
	        GameObject go = obj.gameObject;
	        Mesh mesh = go.GetComponent<MeshFilter>().mesh;

	        _meshList[i] = mesh;
	        _trianglesList[i] = mesh.triangles;

	        ++i;
	    }

	    _mode = Mode.WireFrame;
	    _change();
	}

    private void _change()
    {
        text.text = "Mode: " + _mode;

        int len = _meshList.Length;

        for (int i = 0; i < len; i++)
        {
            Mesh mesh = _meshList[i];
            int[] triangles = _trianglesList[i];


            if (_mode == Mode.Face)
            {
                mesh.SetTriangles(triangles, 0);
            }
            else
            {
                int[] indices;

                switch (_mode)
                {
                    case Mode.WireFrameOnlyEdges:
                        indices = Wireframe.MakeIndicesOnlyEdges(triangles, mesh.vertices, 5);
                        break;

                    case Mode.WireFrameNoDuplicates:
                        indices = Wireframe.MakeIndicesNoDuplicates(triangles, mesh.vertices);
                        break;

                    default:
                        indices = Wireframe.MakeIndices(triangles);
                        break;
                }

                text.text += "\n[" + mesh.name + "] lines: " + indices.Length / 2 + ", vertices: " + mesh.vertices.Length;

                mesh.SetIndices(indices, MeshTopology.Lines, 0);
            }



        }
    }



    // Update is called once per frame
	void Update () {
	    transform.Rotate(Vector3.up, 0.3f);
	    transform.Rotate(Vector3.right, 0.2f);



	    if (Input.GetMouseButtonDown(0))
	    {
	        switch (_mode)
	        {
	            case Mode.Face:
	                _mode = Mode.WireFrame;
	                break;

	            case Mode.WireFrame:
	                _mode = Mode.WireFrameNoDuplicates;
	                break;

	            case Mode.WireFrameNoDuplicates:
	                _mode = Mode.WireFrameOnlyEdges;
	                break;

	            case Mode.WireFrameOnlyEdges:
	                _mode = Mode.Face;
	                break;
	        }

	        _change();
	    }



	}
}
