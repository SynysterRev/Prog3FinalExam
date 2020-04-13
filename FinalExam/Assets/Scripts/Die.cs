using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    int idPlayer;
    Face[] faces = new Face[6];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Face
{
   public Ressources typeSupplieFace;
   public bool isTrash;
}
