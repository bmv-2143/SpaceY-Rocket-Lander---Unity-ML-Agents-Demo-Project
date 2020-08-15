# SpaceY (Unity ML-Agents demo project)

## About

This project was created for [Unity ML-Agents Challenge I](https://connect.unity.com/challenges/ml-agents-1). Its initial version won
a [Creative Application Award](https://blogs.unity3d.com/2018/02/28/introducing-the-winners-of-the-first-ml-agents-challenge/) in that challenge.



This project demonstrates how [Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents) (machine learning framework) can be used to teach
a rocket to lift off from one planet, then fly and land onto another one.

You can find a [video](https://youtu.be/NBz9mrxIMJo) that shows how a neural network drives the rocket
here:

[![SpaceY demo video](http://img.youtube.com/vi/NBz9mrxIMJo/0.jpg)](https://youtu.be/NBz9mrxIMJo)

## Simulation description

- The rocket has six engines that apply different accelerations at different positions
of the rocket body.
- The planets have gravity that acts in their vicinity

- The rocket must not leave the rectangular simulation area. It is a loss if it leaves it.
- The rocket can touch the planets only with its legs, otherwise it is a loss.
- The rocket must touch the second planet with both of its legs and keep touching it for a while for a successful landing.

The provided trained model ("Assets/SpaceYRocketLanderDemo/TrainedModels") has more than 98% success rate.

## Downloads

* [SpaceY v1.0 (Windows_x86-64)](../develop/Builds/SpaceY_v1.0_windows_x86-64.zip)

# Controls
"ML-Agent\Player control" button in the top right corner switches the simulation control mode to a player or the trained neural network control.

In "Player control" mode you can control the rocket with the following keys:

* "Q" - upper left side engine;
* "E" - upper right side engine;
* "A" - bottom left side engine;
* "D" - bottom right side engine;
* "S" - main bottom engine;
* "W" - top stop engine.

* "Esc" - close application

## Unity Editor \ Ml-Agents Package compatibility

SpaceY v1.0 supports:

* Unity Editor 2019.4.6f1
* Unity ML-Agents Package v1.0.3

It might work with later Unity Editor and ML-Agents Package versions, but this is not guaranteed.

## Training environment setup

Follow the instructions to setup the training environment:

* [Anaconda Download & Install](https://learn.unity.com/tutorial/anaconda-download-install?uv=2019.3&courseId=5e470160edbc2a15578b13d7&projectId=5ec83171edbc2a44309bf22e)
* [Anaconda Environment Setup](https://learn.unity.com/tutorial/anaconda-environment-setup?uv=2019.3&courseId=5e470160edbc2a15578b13d7&projectId=5ec83171edbc2a44309bf22e)

## Training

* Training yaml-config file is located in "Assets/SpaceYRocketLanderDemo/.Training/Session_XX" folder.
* The resulting training files are located in "Assets/SpaceYRocketLanderDemo/.Training/Session_XX/results"

The ".Training" folder is not displayed in Unity Editor (use your file manager to navigate to it).

### Setup training

1. Select the "Default" Behavior Type  in "Assets/SpaceYRocketLanderDemo/Prefabs/Rocket.prefab" Behavior Parameters (script component).

2. Select a Simulation Mode = "Training" in "Assets/SpaceYRocketLanderDemo/ScriptableObjects/SimulationConfig.asset"

   In "Demo" mode particles and some other objects\components that make a simulation look better are enabled.   

   In "Training" mode the mentioned above visual improvement objects\components are disabled.

3. You can use "Simulations object \ Game Environment Deployer" script in RocketLander scene to assist in cloning of Game Simulation Training Environments

4. You can use "Simulations object \ Success Based Curriculum Trainer" script in RocketLander scene to
turn ON\OFF the success based curriculum training (see below) and to setup curriculum params.

5. Issue a training command and press Play button in Unity Editor.

### Start demo mode

1. Select the "Heuristic only" Behavior Type  in "Assets/SpaceYRocketLanderDemo/Prefabs/Rocket.prefab" Behavior Parameters (script component).

2. Select "Demo" Simulation Mode in "Assets/SpaceYRocketLanderDemo/ScriptableObjects/SimulationConfig.asset"

3. Press Play button in Unity Editor

### Success based Curriculum learning

"Simulations object \ Success Based Curriculum Trainer" script in RocketLander scene can turn ON\OFF success based curriculum training - when a model reaches a specified success rate (e.g. more than 85%), it switches next set of curriculum params.

### Training commands

Some useful training commands [TRAINING_COMMANDS](../develop/Assets/SpaceYRocketLanderDemo/.Training/TRAINING_COMMANDS.txt)

### Training tips

Afters some experiments it seems that the fastest way to train (on my hardware: Intel Core i5-3570K (2012), Nvidia RTX 2070 Super) is to train using CPU instead of GPU, use TimeScale=1 with a large number of training environments >= 100. Training at a higher TimeScale values with lower number of simulation environments and\or using GPU instead of CPU produced worse results.