using Unity.Cinemachine;
using UnityEngine;

namespace shootstar
{


    public class cinemachine_Extension : MonoBehaviour
    {
        private CameraManager cameraManager;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            cameraManager=shootingstarGameManager.Instance.cameraManager;
            cameraManager.cinemachineCameras.Add(this.gameObject.GetComponent<CinemachineCamera>());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}