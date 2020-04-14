using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    up,
    down,
    forward,
    backward,
    right,
    left,
    total
}
public class Die : MonoBehaviour
{
    int idCharacter;
    [SerializeField] Face[] faces = new Face[6];
    public bool isLocked;
    Rigidbody rgbd;
    Face upperFace;
    // Start is called before the first frame update

    public void Initialize(int _idCharacter)
    {
        rgbd = GetComponent<Rigidbody>();
        int x = Random.Range(0, 11) % 2 == 0 ? 1 : -1;
        int z = Random.Range(0, 11) % 2 == 0 ? 1 : -1;
        Vector3 force = new Vector3(Random.Range(30.0f, 50.0f) * x, 0.0f, Random.Range(30.0f, 50.0f) * z);
        rgbd.AddForce(force, ForceMode.Impulse);
        idCharacter = _idCharacter;
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (rgbd != null && rgbd.velocity.magnitude <= 0.1f && upperFace == null)
        {
            DetectUpperFace();
        }
    }

    void DetectUpperFace()
    {
        if (transform.up == Vector3.up)
        {
            upperFace = faces[0];
        }
        else if (-transform.up == Vector3.up)
        {
            upperFace = faces[1];
        }
        else if (transform.forward == Vector3.up)
        {
            upperFace = faces[2];
        }
        else if (-transform.forward == Vector3.up)
        {
            upperFace = faces[3];
        }
        else if (transform.right == Vector3.up)
        {
            upperFace = faces[4];
        }
        else if (-transform.right == Vector3.up)
        {
            upperFace = faces[5];
        }
        if (upperFace != null)
            Debug.Log(upperFace.typeSupplieFace);
    }
}

[System.Serializable]
public class Face
{
    public Ressources typeSupplieFace;
    public bool isTrash;
    public Direction direction;
}
