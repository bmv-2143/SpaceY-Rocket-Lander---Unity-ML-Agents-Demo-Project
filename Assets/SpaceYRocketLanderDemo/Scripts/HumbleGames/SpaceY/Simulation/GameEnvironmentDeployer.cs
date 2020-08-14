using RoboRyanTron.QuickButtons;
using UnityEngine;

namespace HumbleGames.SpaceY.Simulation
{
    public class GameEnvironmentDeployer : MonoBehaviour
    {
        [SerializeField]
        private bool deployEnvoronmentsInAwake;

        [SerializeField]
        private GameObject gameEnvironmentToClone;

        [SerializeField]
        private float offsetX;

        [SerializeField]
        private float offsetY;

        [SerializeField]
        private int columns;

        [SerializeField]
        private int rows;

        public QuickButton DeployEnvironments = new QuickButton(nameof(classHandle.DeployGameEnvironments));

        private GameObject[,] deployedEnvironments;

        private const GameEnvironmentDeployer classHandle = null;

        private void Awake()
        {
            if (deployEnvoronmentsInAwake)
            {
                DeployGameEnvironments();
            }
        }

        private void DeployGameEnvironments()
        {
            deployedEnvironments = new GameObject[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (row == 0 && column == 0)
                    {
                        deployedEnvironments[row, column] = gameEnvironmentToClone;
                        gameEnvironmentToClone.transform.localPosition = Vector3.zero;
                    }

                    else
                    {
                        deployedEnvironments[row, column] = 
                            Instantiate(gameEnvironmentToClone,
                                        new Vector3(column * offsetX, row * offsetY, 0), 
                                        Quaternion.identity,
                                        transform);
                    }

                }
            }
        }
    }
}
