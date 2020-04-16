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
    [SerializeField] GameObject outlineCube;
    Material outlineToChange;
    [SerializeField] Shader shader;

    public bool isLocked;
    public bool isUseForSupply;
    public bool isUseToMove;
    bool isAddedToList;

    public bool HasBeenUsed;
    Rigidbody rgbd;
    Face upperFace;
    bool CanCheckFace = false;

    public int id;

    Dictionary<Direction, Vector3> directionFace = new Dictionary<Direction, Vector3>();
    Vector3 newPosition;
    // Start is called before the first frame update

    public void Initialize(int _idCharacter, int _idDie)
    {
        rgbd = GetComponent<Rigidbody>();
        idCharacter = _idCharacter;
        id = _idDie;
        rgbd.isKinematic = true;
        upperFace = null;
        isAddedToList = false;
        outlineToChange = outlineCube.GetComponent<Renderer>().sharedMaterial = new Material(shader);
        outlineToChange.SetFloat("_Thickness", 0.0f);
    }

    public void RollDie(Vector3 _position)
    {
        ResetNoneSupplyDie(_position);
        //outlineToChange.SetFloat("_Thickness", 0.0f);
        gameObject.layer = 0;
        rgbd.isKinematic = false;
        int x = Random.Range(0, 11) % 2 == 0 ? 1 : -1;
        int z = Random.Range(0, 11) % 2 == 0 ? 1 : -1;
       // transform.position = _position;
        Vector3 force = new Vector3(Random.Range(30.0f, 50.0f) * x, 0.0f, Random.Range(30.0f, 50.0f) * z);
        rgbd.AddForce(force, ForceMode.Impulse);
       /* CanCheckFace = false;
        upperFace = null;
        HasBeenUsed = false;
        isLocked = false;
        isUseToMove = false;*/

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
        return upperFace.typeSupplieFace == _supply;
    }

    void DetectUpperFace()
    {
        float bestDot = -1.0f;
        Direction bestValue = Direction.up;
        directionFace.Clear();
        directionFace.Add(Direction.up, transform.up);
        directionFace.Add(Direction.down, -transform.up);
        directionFace.Add(Direction.forward, transform.forward);
        directionFace.Add(Direction.backward, -transform.forward);
        directionFace.Add(Direction.right, transform.right);
        directionFace.Add(Direction.left, -transform.right);
        foreach (KeyValuePair<Direction, Vector3> kvp in directionFace)
        {
            float dot = Vector3.Dot(kvp.Value, Vector3.up);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestValue = kvp.Key;
            }
        }
        if(bestDot > 0.5f)
        {
            //Debug.Log()
            upperFace = faces[(int)bestValue];
        }
       /* if (transform.up == Vector3.up)
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
        }*/
        if (upperFace != null)
        {
            rgbd.isKinematic = true;
            CanCheckFace = false;
            OnStop(id);
        }
        // Debug.Log(upperFace.typeSupplieFace);
    }

    public bool IsUpperFacePlane()
    {
        if (upperFace != null)
        {
            return upperFace.typeSupplieFace == Ressources.plane;
        }
        return false;
    }
    public void UnlockLockDice()
    {
        Debug.Log("locked");
        if (!HasBeenUsed && !isAddedToList)
        {
            isLocked = !isLocked;
            if(isLocked)
            {
                outlineToChange.SetFloat("_Thickness", 4.0f);
                outlineToChange.SetColor("_Color", Color.red);
            }
            else
            {
                outlineToChange.SetFloat("_Thickness", 0.0f);
            }
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

    public void ResetNoneSupplyDie(Vector3 _position)
    {
        transform.position = _position;
        isLocked = false;
        HasBeenUsed = false;
        isUseToMove = false;
        rgbd.isKinematic = true;
        CanCheckFace = false;
        upperFace = null;
        isAddedToList = false;
        outlineToChange.SetFloat("_Thickness", 0.0f);
        gameObject.layer = 2;
    }

    public void AddedToListToPossibleSupply(bool _isAdded)
    {
        isAddedToList = _isAdded;
        isLocked = false;
        if (_isAdded)
        {
            outlineToChange.SetFloat("_Thickness", 4.0f);
            outlineToChange.SetColor("_Color", Color.yellow);
        }
        else
        {
            outlineToChange.SetFloat("_Thickness", 0.0f);
        }
    }

    public void LockForSupply(Ressources _supplyNeeded)
    {
        if (!HasBeenUsed)
        {
            if (_supplyNeeded == upperFace.typeSupplieFace)
            {
                outlineToChange.SetFloat("_Thickness", 0.0f);
                isUseForSupply = true;
                isLocked = true;
            }
        }
    }

    public bool UseForMovement(bool _isPlane)
    {
        if (!HasBeenUsed)
        {
            isUseToMove = !isUseToMove;
            if(isUseToMove && _isPlane)
            {
                outlineToChange.SetFloat("_Thickness", 4.0f);
                outlineToChange.SetColor("_Color", Color.blue);
            }
            else if(isUseToMove && !_isPlane)
            {
                outlineToChange.SetFloat("_Thickness", 4.0f);
                outlineToChange.SetColor("_Color", Color.green);
            }
            else
            {
                outlineToChange.SetFloat("_Thickness", 0.0f);
            }
            return isUseToMove;
        }
        return false;
    }

    public void ReturnDieToOwner()
    {
        isUseForSupply = false;
        isLocked = false;
        isAddedToList = false;
        HasBeenUsed = true;
        transform.localScale = Vector3.one;
        transform.position = new Vector3(0.0f, 101.0f, 0.0f);
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
