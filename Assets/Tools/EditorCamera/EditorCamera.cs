using UnityEngine;

namespace Assets.Tools.EditorCamera
{
    public class EditorCamera : MonoBehaviour
    {
        public float forwardSpeedScale = 100;
        public float rightSpeedScale = 100;
        public float upSpeedScale = 100;

        public float sensitivityX = 50;
        public float sensitivityY = 2;

        public float speed = 1;
        public float sensitivity = 1;

        [HideInInspector]
        public bool isSaveOnScene = false;

        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity * sensitivityX * Time.deltaTime * 200f, 0);
                transform.Rotate(-Input.GetAxis("Mouse Y") * sensitivity * sensitivityY * Time.deltaTime * 200f, 0, 0);
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            }

            float curSpeed = speed * Time.deltaTime;

            if (Input.GetKey(KeyCode.LeftShift))
                curSpeed *= 10.0f;

            if (Input.GetKey(KeyCode.LeftControl))
                curSpeed *= 0.1f;

            Vector3 forwardSpeed = forwardSpeedScale * transform.forward * Input.GetAxis("Vertical");
            Vector3 rightSpeed = rightSpeedScale * transform.right * Input.GetAxis("Horizontal");
            Vector3 upSpeed = upSpeedScale * transform.up * Input.GetAxis("UpAxis");

            transform.position = transform.position + (forwardSpeed + rightSpeed + upSpeed) * curSpeed;
        }

    }
}