using UnityEngine;

namespace HumbleGames.SpaceY
{
    public class UserInput : MonoBehaviour
    {

        [HideInInspector]
        public static UserInput Instance
        {
            get; private set;
        }

        void Awake()
        {
            // ----------------------------------- Unity Singleton ----------------------------------------
            //Check if instance already exists
            if (Instance == null)
            {
                //if not, set instance to this
                Instance = this;
            }

            //If instance already exists and it's not this:
            else if (Instance != this)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one 
                // instance of a DataCollector.
                Destroy(gameObject);
                return;
            }

            // Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void MapKeysToMlAgentsActions(float[] actionsOut)
        {
            // --------------------------------------------------------------------------------------------------------
            // Rocket Controls:
            // 
            //       S   
            //       ^
            //    Q | | E
            //      | |
            //      | |
            //    A | | D
            //     /   \
            //       W
            // 
            // --------------------------------------------------------------------------------------------------------

            // Move forward (main bottom\stern engine)
            if (Input.GetKey(KeyCode.W))
            {
                //Debug.LogFormat("{0}: {1}: Key: W", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 1f;
            }

            // Move backwards (auxiliary top\bow engine)
            else if (Input.GetKey(KeyCode.S))
            {
                //Debug.LogFormat("{0}: {1}: Key: S", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 2f;
            }

            // Top left auxiliary engine
            else if (Input.GetKey(KeyCode.Q))
            {
                //Debug.LogFormat("{0}: {1}: Key: Q", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 3f;
            }

            // Top right auxiliary engine
            else if (Input.GetKey(KeyCode.E))
            {
                //Debug.LogFormat("{0}: {1}: Key: E", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 4f;
            }

            // Bottom left auxiliary engine
            else if (Input.GetKey(KeyCode.A))
            {
                //Debug.LogFormat("{0}: {1}: Key: A", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 5f;
            }

            // Bottom right auxiliary engine
            else if (Input.GetKey(KeyCode.D))
            {
                //Debug.LogFormat("{0}: {1}: Key: D", LOG_TAG, nameof(classHelper.MapKeysToActions));
                actionsOut[0] = 6f;
            }

            // No keys detected - do nothing
            else
            {
                actionsOut[0] = 0;
            }
        }
    }
}