using UnityEngine;

namespace HumbleGames.Utils
{
    /// <summary>
    /// Draws Circle with LineRenderer
    /// 
    /// based on: https://forum.unity3d.com/threads/linerenderer-to-create-an-ellipse.74028/
    /// </summary>
    public class DrawCircle : MonoBehaviour
    {
        
        [SerializeField]
        public float radiusXY;

        [SerializeField]
        public int segments;

        // Radius
        private float xRadius;
        private float yRadius;
        private float zRadius;

        // Center Position
        private float xCenterOffset;
        private float yCenterOffset;
        private float zCenterOffset;

        private LineRenderer line;

        // ----------------------------------------------------------------------------------------
        //                              Unity Lifecycle Methods
        // ----------------------------------------------------------------------------------------

        void Start()
        {

            line = gameObject.GetComponent<LineRenderer>();

            line.positionCount = segments + 1;
            line.useWorldSpace = false;

            /*
             * NOTE: When using local space circle radius has scalling issues (it scales not by
             * the expected values). It is not clear if it is my fault or some bug in Unity.
             * 
             * Using worldSpace fixes this issue.
             * 
             * Will compensate cirlces position by setting center offset
             */
            line.useWorldSpace = true;

            SetRadius(radiusXY);
            SetupCenterOffset();

            // draw lines
            CreatePoints(segments);
        }


        private void Update()
        {
            SetRadius(radiusXY);
            CreatePoints(segments);
            SetupCenterOffset();
        }

        private void SetupCenterOffset()
        {
            xCenterOffset = gameObject.transform.position.x;
            yCenterOffset = gameObject.transform.position.y;
            zCenterOffset = gameObject.transform.position.z;
        }

        private void SetUpRadius()
        {
            // setup circle radius
            SetRadius(radiusXY);
        }

        private void SetRadius(float newRadius)
        {
            xRadius = newRadius;
            yRadius = newRadius;
        }

        void CreatePoints(int segs)
        {
            float x;
            float y;
            float z;

            float angle = 20f;

            for (int i = 0; i < (segs + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * xRadius + xCenterOffset;
                y = Mathf.Cos(Mathf.Deg2Rad * angle) * yRadius + yCenterOffset;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * zRadius + zCenterOffset;

                line.SetPosition(i, new Vector3(x, y, z));
                angle += (360f / segments);
            }
        }
    }
}