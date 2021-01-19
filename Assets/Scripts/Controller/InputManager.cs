using UnityEngine;
using System.Collections;

namespace Zombie
{
    public class InputManager : MonoBehaviour
    {
        Controller controller;
        Transform camTransform;
        Transform camHolder;

        CityGenerater cityGenerater;
        public GameObject Sphere;
        public LayerMask mylayerMask;


        private void Start()
        {
            cityGenerater = FindObjectOfType<CityGenerater>();
            cityGenerater.Execute();
            gameObject.transform.position = new Vector3(cityGenerater.GetSpotX()*5, 0.1f, cityGenerater.GetSpotZ()*5);
            controller = GetComponent<Controller>();
            controller.Execute();
            camTransform = Camera.main.transform;
            if(camTransform == null)
            {
                Debug.Log("No camera found with the MainCamera tag");
            }
            camHolder = camTransform.parent;
            controller.LoadWeapon(controller.defaultWeapon);


            GameObject localSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MeshFilter meshFilter = localSphere.GetComponent<MeshFilter>();
            localSphere.SetActive(false);
            MeshFilter meshFilter2 = Sphere.AddComponent<MeshFilter>();
            meshFilter2.mesh = meshFilter.mesh;

        }

        void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 moveDirection = camHolder.forward * vertical;
            moveDirection += camHolder.right * horizontal;
            moveDirection.Normalize();
            controller.MoveCharacter(moveDirection);
            controller.HandleAnimation(moveDirection);


            float delta = Time.deltaTime;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layermask = (1 << 9);
            if (Physics.Raycast(ray, out hit, float.MaxValue, layermask))
            {
                controller.RotateToDirectionMouse(hit.point, delta);
            }
            if (Input.GetButton("Fire1"))
            {
                controller.HandleShooting();
            }

            RaycastHit hit2;
            int layermask2 = (1 << 10 | 1 << 11);

            if (Physics.Raycast(camTransform.transform.position, (Sphere.transform.position - camTransform.transform.position).normalized, out hit2, float.MaxValue, layermask2))
            {
                if(hit2.collider.gameObject.tag.CompareTo("Player") == 0)
                {
                    Sphere.transform.localScale = new Vector3(0, 0, 0);
                }
                else
                {
                    Sphere.transform.localScale = new Vector3(10, 1, 10);
                }
            }
        }
    }
}