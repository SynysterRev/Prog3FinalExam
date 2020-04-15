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
    public delegate void DelegateIsStop(int _id);
    public event DelegateIsStop OnStop;


    int idCharacter;
    [SerializeField] Face[] faces = new Face[6];
    public bool isLocked;
    public bool isUseForSupply;
    public bool isUseToMove;

    public bool HasBeenUsed;
    Rigidbody rgbd;
    Face upperFace;
    bool CanCheckFace = false;

    int id;

    Vector3 newPosition;
    // Start is called before the first frame update

    public void Initialize(int _idCharacter, int _idDie)
    {
        rgbd = GetComponent<Rigidbody>();
        int x = Random.Range(0, 11) % 2 == 0 ? 1 : -1;
        int z = Random.Range(0, 11) % 2 == 0 ? 1 : -1;
        /*  Vector3 force = new Vector3(Random.Range(30.0f, 50.0f) * x, 0.0f, Random.Range(30.0f, 50.0f) * z);
          rgbd.AddForce(force, ForceMode.Impulse);*/
        idCharacter = _idCharacter;
        HasBeenUsed = false;
        isLocked = false;
        isUseToMove = false;
        id = _idDie;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (rgbd != null && rgbd.velocity.magnitude <= 0.1f && upperFace == null && CanCheckFace)
        {
            DetectUpperFace();
        }
    }

    public bool IsCorrectSupply(Ressources _supply)
    {
        if (upperFace == null) return false;
        Debug.Log(upperFace.typeSupplieFace + " " + _supply);
        return upperFace.typeSupplieFace == _supply;
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
        {
            rgbd.isKinematic = true;
            CanCheckFace = false;
            OnStop(id);
        }
        // Debug.Log(upperFace.typeSupplieFace);
    }

    public void UnlockLockDice()
    {
        if (!HasBeenUsed)
        {
            isLocked = !isLocked;
        }
    }

    public void MoveDice(Vector3 _position, bool _reduce)
    {
        if (_reduce)
            transform.localScale = Vector3.one * 0.5f;
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y = 0.0f;
        transform.rotation = Quaternion.Euler(rot);
        transform.position = _position;
    }

    public void LockForSupply(Ressources _supplyNeeded)
    {
        Debug.Log("in lock fuct");
        if (!HasBeenUsed)
        {
            Debug.Log("locked");
            if (_supplyNeeded == upperFace.typeSupplieFace)
            {
                isUseForSupply = true;
                isLocked = true;
            }
        }
    }

    public bool UseForMovement()
    {
        if (!HasBeenUsed)
        {
            isUseToMove = !isUseToMove;
            return isUseToMove;
        }
        return false;
    }

    public void ReturnDieToOwner()
    {
        isUseForSupply = false;
        isLocked = false;
        HasBeenUsed = true;
        transform.localScale = Vector3.one;
        transform.position = new Vector3(0.0f, 101.0f, 0.0f);
        //Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<BoardGame>())
        {
            CanCheckFace = true;
        }
    }
}

[System.Serializable]
public class Face
{
    public Ressources typeSupplieFace;
    public bool isTrash;
    public Direction direction;
}
