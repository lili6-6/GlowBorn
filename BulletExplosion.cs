using shootstar;
using UnityEngine;

public class BulletExplosion : MonoBehaviour
{
    private CameraManager camManager;

    private void Start()
    {
        camManager=shootingstarGameManager.Instance.cameraManager;
    }

    public void Explode()
    {
        //Debug.Log("Shake Camera!");
        camManager.ShakeCurrentCamera();
    }
}
