using UnityEngine;

public class BackGround : MonoBehaviour
{
    public Transform mainCam;     
    public Transform[] backgrounds; 
    public float length;          

    void Update()
    {
       
        if (mainCam.position.x > backgrounds[1].position.x)
        {
            MoveBackground(Vector3.right);
        }
        
        else if (mainCam.position.x < backgrounds[0].position.x)
        {
            MoveBackground(Vector3.left);
        }
    }

    void MoveBackground(Vector3 direction)
    {
        if (direction == Vector3.right)
        {
            
            backgrounds[0].position = backgrounds[1].position + Vector3.right * length;
            SwapBackgrounds();
        }
        else if (direction == Vector3.left)
        {
         
            backgrounds[1].position = backgrounds[0].position + Vector3.left * length;
            SwapBackgrounds();
        }
    }

    void SwapBackgrounds()
    {
    
        Transform temp = backgrounds[0];
        backgrounds[0] = backgrounds[1];
        backgrounds[1] = temp;
    }
}
